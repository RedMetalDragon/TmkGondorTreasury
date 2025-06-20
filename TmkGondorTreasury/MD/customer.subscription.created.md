# Stripe Webhook Event: `customer.subscription.created`

## Overview

This webhook event is triggered when a customer successfully creates a new subscription in Stripe. This document describes the structure and key elements of this webhook payload.

## Event Structure

The `customer.subscription.created` event contains the following main components:

| Field | Description |
|-------|-------------|
| `id` | Unique identifier for this event |
| `object` | Always "event" |
| `type` | "customer.subscription.created" |
| `data` | Contains the subscription object and related data |
| `created` | Unix timestamp when this event was created |

## Subscription Object

The subscription object is located in `data.object` and contains detailed information about the newly created subscription:

| Field | Description |
|-------|-------------|
| `id` | Unique identifier for this subscription (format: `sub_*`) |
| `object` | Always "subscription" |
| `status` | Current status of the subscription (e.g., "active", "trialing", "past_due") |
| `customer` | ID of the customer this subscription belongs to |
| `current_period_start` | Unix timestamp for the start of the current billing period |
| `current_period_end` | Unix timestamp for the end of the current billing period |
| `items` | List of items included in this subscription |
| `default_payment_method` | ID of the default payment method for this subscription |
| `latest_invoice` | ID of the most recent invoice for this subscription |

## Subscription Items

The `items` field contains a list of subscription items, each representing a product or service the customer has subscribed to:

| Field | Description |
|-------|-------------|
| `id` | Unique identifier for this subscription item |
| `object` | Always "subscription_item" |
| `price` | Price object associated with this item |
| `quantity` | Quantity of this item in the subscription |

## Example JSON
```json
{
  "id": "evt_1234567890",
  "object": "event",
  "type": "customer.subscription.created",
  "data": {
    "object": {
      "id": "sub_1234567890",
      "object": "subscription",
      "status": "active",
      "customer": "cus_1234567890",
      "current_period_start": 1610000000,
      "current_period_end": 1612678400,
      "items": {
        "object": "list",
        "data": [ 
          {
            "id": "si_1234567890",
            "object": "subscription_item",
            "price": {
              "id": "price_1234567890",
              "object": "price",
              "product": "prod_1234567890"
            },
            "quantity": 1
          }
        ]
      },
      "default_payment_method": "pm_1234567890",
      "latest_invoice": "in_1234567890"
    }
  },
  "created": 1610000000
}
```