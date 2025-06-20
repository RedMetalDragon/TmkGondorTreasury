# Stripe Customer Subscription Created Event - Data Reference

## Overview

The `customer.subscription.created` webhook event is triggered when a new subscription is created for a customer in Stripe. This document outlines all the data available from this event and how it can be used in your application for subscription management, billing, and customer lifecycle tracking.

## Event Structure

```jsonc
{
  "object": { /* Subscription Object */ },
  "previous_attributes": null /* Changes from previous state */
}
```

## Key Business Data Points

### 🎯 Subscription Core Information

| Field | Type | Description | Business Use |
|-------|------|-------------|--------------|
| `id` | string | Unique subscription ID | Primary key for subscription tracking |
| `status` | string | Subscription status (`incomplete`, `active`, `trialing`, etc.) | Subscription lifecycle management |
| `customer` | string | Stripe customer ID | Customer relationship tracking |
| `created` | timestamp | When subscription was created | Onboarding timeline tracking |
| `start_date` | timestamp | When subscription period starts | Service provisioning |
| `current_period_start` | timestamp | Current billing period start | Billing cycle tracking |
| `current_period_end` | timestamp | Current billing period end | Next billing date |
| `cancel_at_period_end` | boolean | Whether subscription cancels at period end | Churn management |
| `canceled_at` | timestamp | When subscription was canceled | Churn analysis |
| `ended_at` | timestamp | When subscription ended | Lifecycle completion |

### 💰 Billing and Payment Information

| Field | Type | Description | Business Use |
|-------|------|-------------|--------------|
| `currency` | string | Three-letter ISO currency code | Multi-currency support |
| `collection_method` | string | How payments are collected | Payment workflow management |
| `billing_cycle_anchor` | timestamp | Day of month billing occurs | Billing schedule optimization |
| `default_payment_method` | string | Default payment method ID | Payment processing |
| `latest_invoice` | string | Most recent invoice ID | Invoice tracking |
| `quantity` | integer | Subscription quantity | Usage/seat management |

### 🏷️ Product and Pricing Details

| Field | Type | Description | Business Use |
|-------|------|-------------|--------------|
| `plan.id` | string | Stripe plan/price ID | Product identification |
| `plan.nickname` | string | Human-readable plan name | Customer communication |
| `plan.amount` | integer | Price in cents | Revenue calculations |
| `plan.currency` | string | Plan currency | Multi-currency handling |
| `plan.interval` | string | Billing interval (`month`, `year`) | Billing frequency |
| `plan.interval_count` | integer | Number of intervals between charges | Custom billing periods |
| `plan.product` | string | Stripe product ID | Product analytics |
| `plan.usage_type` | string | How usage is calculated | Usage-based billing |

### 🔄 Trial Information

| Field | Type | Description | Business Use |
|-------|------|-------------|--------------|
| `trial_start` | timestamp | Trial period start date | Trial management |
| `trial_end` | timestamp | Trial period end date | Trial expiration tracking |
| `trial_settings` | object | Trial configuration | Trial behavior management |

### 📊 Subscription Items Details

Each subscription contains items with detailed product information:

| Field | Type | Description | Business Use |
|-------|------|-------------|--------------|
| `items.data[].id` | string | Subscription item ID | Item-level tracking |
| `items.data[].price.id` | string | Price ID for this item | Pricing management |
| `items.data[].price.nickname` | string | Price display name | Customer communication |
| `items.data[].price.unit_amount` | integer | Price per unit in cents | Revenue calculations |
| `items.data[].quantity` | integer | Quantity of this item | Usage tracking |
| `items.data[].price.recurring.interval` | string | Billing frequency | Billing management |

### 🚫 Cancellation Information

| Field | Type | Description | Business Use |
|-------|------|-------------|--------------|
| `cancellation_details.reason` | string | Cancellation reason | Churn analysis |
| `cancellation_details.comment` | string | Customer feedback | Product improvement |
| `cancellation_details.feedback` | string | Structured feedback | Retention insights |
| `cancel_at` | timestamp | Scheduled cancellation date | Proactive retention |

## Common Business Use Cases

### 1. New Subscription Onboarding
```javascript
const newSubscription = {
  subscription_id: subscription.id,
  customer_id: subscription.customer,
  plan_name: subscription.plan.nickname,
  monthly_value: subscription.plan.amount / 100,
  billing_interval: subscription.plan.interval,
  start_date: new Date(subscription.start_date * 1000),
  next_billing: new Date(subscription.current_period_end * 1000),
  status: subscription.status,
  trial_end: subscription.trial_end ? new Date(subscription.trial_end * 1000) : null,
  internal_txn_id: subscription.metadata.txnId
};
```

### 2. Revenue Forecasting
```javascript
const revenueData = {
  monthly_recurring_revenue: subscription.plan.interval === 'month' ? 
    (subscription.plan.amount * subscription.quantity) / 100 : 0,
  annual_recurring_revenue: subscription.plan.interval === 'year' ? 
    (subscription.plan.amount * subscription.quantity) / 100 : 
    ((subscription.plan.amount * subscription.quantity) / 100) * 12,
  customer_lifetime_value_contribution: subscription.plan.amount / 100,
  billing_frequency: subscription.plan.interval,
  seat_count: subscription.quantity
};
```

### 3. Customer Service Access Provisioning
```javascript
const serviceProvisioning = {
  customer_id: subscription.customer,
  subscription_id: subscription.id,
  plan_type: subscription.plan.nickname,
  access_level: subscription.plan.product, // Map to internal access levels
  quantity_allowed: subscription.quantity,
  service_start: new Date(subscription.start_date * 1000),
  trial_period: {
    is_trial: subscription.trial_end !== null,
    trial_end: subscription.trial_end ? new Date(subscription.trial_end * 1000) : null
  },
  billing_status: subscription.status
};
```

### 4. Welcome Communications
```javascript
const welcomeEmailData = {
  customer_id: subscription.customer,
  plan_name: subscription.plan.nickname,
  monthly_cost: `$${(subscription.plan.amount / 100).toFixed(2)}`,
  billing_frequency: subscription.plan.interval,
  next_billing_date: new Date(subscription.current_period_end * 1000).toLocaleDateString(),
  trial_info: subscription.trial_end ? {
    has_trial: true,
    trial_end_date: new Date(subscription.trial_end * 1000).toLocaleDateString(),
    trial_days_remaining: Math.ceil((subscription.trial_end * 1000 - Date.now()) / (1000 * 60 * 60 * 24))
  } : { has_trial: false }
};
```

### 5. Analytics and Tracking
```javascript
const analyticsEvent = {
  event: 'subscription_created',
  customer_id: subscription.customer,
  subscription_id: subscription.id,
  plan_id: subscription.plan.id,
  product_id: subscription.plan.product,
  revenue_impact: subscription.plan.amount / 100,
  billing_cycle: subscription.plan.interval,
  trial_conversion: subscription.trial_end === null, // No trial = direct conversion
  subscription_quantity: subscription.quantity,
  created_timestamp: subscription.created,
  internal_correlation: subscription.metadata.txnId
};
```

## Important Fields for Different Business Functions

### 💳 Billing Team
- `id`, `customer`, `status`
- `current_period_start`, `current_period_end`
- `collection_method`, `latest_invoice`
- `billing_cycle_anchor`
- `plan.amount`, `currency`, `quantity`

### 🛒 Product Team
- `plan.product`, `plan.nickname`
- `items.data[].price.id`
- `quantity`, `plan.usage_type`
- `metadata.txnId` (internal tracking)
- `trial_start`, `trial_end`

### 📊 Analytics Team
- `created`, `start_date`
- `plan.amount`, `plan.interval`
- `customer`, `status`
- `trial_end` (trial vs. direct conversion)
- `quantity` (seat-based metrics)

### 🎯 Marketing Team
- `customer` (for customer journey tracking)
- `plan.nickname` (product preference analysis)
- `trial_end` (trial effectiveness)
- `created` (conversion timing)
- `metadata.txnId` (campaign attribution)

### 👥 Customer Success Team
- `customer`, `subscription.id`
- `status`, `trial_end`
- `plan.nickname` (service level)
- `current_period_end` (renewal dates)
- `cancel_at_period_end` (at-risk customers)

### 🛠️ Engineering Team
- `id` (subscription management)
- `customer` (account linking)
- `metadata` (custom business logic)
- `status` (service provisioning)
- `items.data` (feature access control)

## Subscription Status Meanings

| Status | Description | Business Action |
|--------|-------------|-----------------|
| `incomplete` | Subscription created but payment pending | Monitor for payment completion |
| `incomplete_expired` | Payment failed and won't be retried | Follow up with customer |
| `trialing` | In trial period | Prepare trial conversion campaigns |
| `active` | Subscription is active and paid | Provide full service access |
| `past_due` | Payment failed but will be retried | Send payment reminder |
| `canceled` | Subscription has been canceled | Revoke access, exit survey |
| `unpaid` | Payment failed and retries exhausted | Suspend service |

## Trial Management

### Trial Detection
```javascript
const trialInfo = {
  has_trial: subscription.trial_end !== null,
  trial_start: subscription.trial_start ? new Date(subscription.trial_start * 1000) : null,
  trial_end: subscription.trial_end ? new Date(subscription.trial_end * 1000) : null,
  trial_days_total: subscription.trial_start && subscription.trial_end ? 
    Math.ceil((subscription.trial_end - subscription.trial_start) / (60 * 60 * 24)) : 0,
  is_trial_active: subscription.status === 'trialing'
};
```

### Trial End Behavior
The `trial_settings.end_behavior.missing_payment_method` field indicates what happens when trial ends without payment method:
- `create_invoice`: Creates invoice (requires manual payment)
- `cancel`: Automatically cancels subscription

## Metadata Usage

The `metadata.txnId` field appears to be an internal transaction/correlation ID. This is valuable for:

- Correlating Stripe subscriptions with internal systems
- Tracking subscription sources (campaigns, referrals)
- Linking to internal customer records
- Debugging subscription flows
- Customer support inquiries

## Date Handling

All Stripe timestamps are Unix timestamps (seconds since epoch):

```javascript
const subscriptionDates = {
  created: new Date(subscription.created * 1000),
  period_start: new Date(subscription.current_period_start * 1000),
  period_end: new Date(subscription.current_period_end * 1000),
  trial_end: subscription.trial_end ? new Date(subscription.trial_end * 1000) : null,
  billing_anchor: new Date(subscription.billing_cycle_anchor * 1000)
};
```

## Revenue Calculations

### Monthly Recurring Revenue (MRR)
```javascript
const calculateMRR = (subscription) => {
  const amount = subscription.plan.amount / 100; // Convert cents to dollars
  const quantity = subscription.quantity;
  
  switch (subscription.plan.interval) {
    case 'month':
      return amount * quantity;
    case 'year':
      return (amount * quantity) / 12;
    case 'week':
      return (amount * quantity) * (52/12); // ~4.33 weeks per month
    case 'day':
      return (amount * quantity) * 30; // Approximate
    default:
      return 0;
  }
};
```

### Annual Recurring Revenue (ARR)
```javascript
const calculateARR = (subscription) => {
  return calculateMRR(subscription) * 12;
};
```