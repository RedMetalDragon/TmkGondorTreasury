# Stripe Invoice Payment Succeeded Event - Data Reference

## Overview

The `invoice.payment_succeeded` webhook event is triggered when a Stripe invoice is successfully paid. This document outlines all the data available from this event and how it can be used in your application.

## Event Structure

```jsonc
{
  "object": { /* Invoice Object */ },
  "previous_attributes": null /* Changes from previous state */
}
```

## Key Business Data Points

### 💰 Payment Information

| Field | Type | Description | Business Use |
|-------|------|-------------|--------------|
| `amount_due` | integer | Amount that was due in cents | Revenue tracking |
| `amount_paid` | integer | Amount actually paid in cents | Payment confirmation |
| `amount_remaining` | integer | Outstanding amount in cents | Outstanding balance |
| `currency` | string | Three-letter ISO currency code | Multi-currency support |
| `total` | integer | Total invoice amount in cents | Financial reporting |
| `subtotal` | integer | Subtotal before tax in cents | Tax calculations |
| `paid` | boolean | Whether invoice is fully paid | Payment status |
| `status` | string | Invoice status (`paid`, `open`, etc.) | Workflow management |

### 👤 Customer Information

| Field | Type | Description | Business Use |
|-------|------|-------------|--------------|
| `customer` | string | Stripe customer ID | Customer lookup |
| `customer_email` | string | Customer's email address | Communications |
| `customer_name` | string | Customer's full name | Personalization |
| `customer_phone` | string | Customer's phone number | Contact information |
| `customer_address` | object | Customer's billing address | Shipping/billing |

### 📋 Subscription Details

| Field | Type | Description | Business Use |
|-------|------|-------------|--------------|
| `subscription` | string | Stripe subscription ID | Subscription management |
| `billing_reason` | string | Why invoice was created | Business intelligence |
| `subscription_details.metadata.txnId` | string | Your transaction ID | Internal tracking |

### 📅 Timing Information

| Field | Type | Description | Business Use |
|-------|------|-------------|--------------|
| `created` | timestamp | When invoice was created | Timeline tracking |
| `effective_at` | timestamp | When invoice became effective | Billing period start |
| `period_start` | timestamp | Billing period start | Service period tracking |
| `period_end` | timestamp | Billing period end | Service period tracking |
| `paid_at` | timestamp | When payment was completed | Payment processing time |

### 📄 Line Items Details

Each invoice contains line items with detailed product information:

| Field | Type | Description | Business Use |
|-------|------|-------------|--------------|
| `lines.data[].description` | string | Human-readable description | Customer communication |
| `lines.data[].amount` | integer | Line item amount in cents | Item-level revenue |
| `lines.data[].quantity` | integer | Quantity of items | Usage tracking |
| `lines.data[].period.start` | timestamp | Service period start for item | Service tracking |
| `lines.data[].period.end` | timestamp | Service period end for item | Service tracking |
| `lines.data[].price.id` | string | Stripe price ID | Product identification |
| `lines.data[].price.nickname` | string | Price nickname | Product naming |
| `lines.data[].plan.id` | string | Stripe plan ID | Legacy plan tracking |
| `lines.data[].metadata` | object | Custom metadata | Business logic |

## Common Business Use Cases

### 1. Revenue Recognition
```javascript
const revenue = {
  amount: invoice.amount_paid / 100, // Convert cents to dollars
  currency: invoice.currency,
  customer_id: invoice.customer,
  transaction_date: new Date(invoice.status_transitions.paid_at * 1000),
  billing_period: {
    start: new Date(invoice.period_start * 1000),
    end: new Date(invoice.period_end * 1000)
  }
};
```

### 2. Customer Service Access
```javascript
const serviceUpdate = {
  customer_id: invoice.customer,
  subscription_id: invoice.subscription,
  service_active: invoice.paid,
  next_billing_date: new Date(invoice.lines.data[0].period.end * 1000),
  plan_name: invoice.lines.data[0].price.nickname
};
```

### 3. Communication Triggers
```javascript
const emailData = {
  to: invoice.customer_email,
  customer_name: invoice.customer_name,
  amount_paid: `$${(invoice.amount_paid / 100).toFixed(2)}`,
  invoice_url: invoice.hosted_invoice_url,
  receipt_url: invoice.invoice_pdf,
  service_period: `${new Date(invoice.period_start * 1000).toLocaleDateString()} - ${new Date(invoice.period_end * 1000).toLocaleDateString()}`
};
```

### 4. Analytics and Reporting
```javascript
const analyticsData = {
  revenue: invoice.amount_paid / 100,
  mrr_contribution: invoice.billing_reason === 'subscription_cycle' ? invoice.amount_paid / 100 : 0,
  customer_ltv_update: invoice.customer,
  churn_risk_update: false, // Payment succeeded
  product_performance: invoice.lines.data.map(item => ({
    product_id: item.price.product,
    revenue: item.amount / 100,
    quantity: item.quantity
  }))
};
```

## Important Fields for Different Business Functions

### 💳 Billing Team
- `amount_paid`, `currency`, `total`
- `invoice_pdf`, `hosted_invoice_url`
- `payment_intent`, `charge`
- `billing_reason`

### 🛒 Product Team
- `lines.data[].price.product`
- `lines.data[].quantity`
- `lines.data[].description`
- `subscription_details.metadata`

### 📊 Analytics Team
- `amount_paid`, `currency`
- `customer`, `subscription`
- `billing_reason`
- `period_start`, `period_end`

### 🎯 Marketing Team
- `customer_email`, `customer_name`
- `amount_paid` (for LTV calculations)
- `billing_reason` (new vs. renewal)
- `subscription` (for segmentation)

### 🛠️ Engineering Team
- `id` (invoice ID for references)
- `subscription` (for account management)
- `metadata` (custom business logic)
- `webhooks_delivered_at` (for debugging)

## Metadata Usage

The `txnId` in the subscription metadata (`subscription_details.metadata.txnId`) appears to be your internal transaction ID. This is valuable for:

- Correlating Stripe events with internal systems
- Tracking payments across multiple platforms
- Debugging payment flows
- Customer support inquiries

## Date Handling

All Stripe timestamps are Unix timestamps (seconds since epoch). Convert them to JavaScript Date objects:

```javascript
const paidDate = new Date(invoice.status_transitions.paid_at * 1000);
const servicePeriodStart = new Date(invoice.period_start * 1000);
const servicePeriodEnd = new Date(invoice.period_end * 1000);
```

## Error Handling Considerations

Even in successful payments, monitor these fields for edge cases:

- `amount_remaining` should be 0 for full payments
- `attempt_count` indicates if payment required multiple attempts
- `collection_method` shows if payment was automatic or manual
- `billing_reason` helps distinguish between different types of charges

## Security Considerations

- Never log full payment details in plain text
- Mask sensitive customer information in logs
- Use Stripe's `customer` ID instead of storing payment details
- Verify webhook signatures before processing

## Integration Recommendations

1. **Immediate Actions**: Update customer account status, send confirmation emails
2. **Async Processing**: Update analytics, generate reports, sync with other systems
3. **Monitoring**: Track payment processing times, failure rates, revenue metrics
4. **Customer Experience**: Provide immediate service access, send receipts

## Sample Implementation Checklist

- [ ] Extract customer information for account updates
- [ ] Calculate revenue and update financial records
- [ ] Grant/extend service access
- [ ] Send payment confirmation email
- [ ] Update customer LTV and analytics
- [ ] Log successful payment for audit trails
- [ ] Update subscription status if applicable
- [ ] Trigger any business-specific workflows