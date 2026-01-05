# Domain Entity vs Content Entity — When to Use What

**Purpose:** This document clarifies when to use **Content tables** (CONTENT_ITEM/VERSION/etc.) vs **Entity tables** (ENTITY_DEFINITION/ENTITY_INSTANCE) in ContentOS.

**TL;DR:**
- **Content = Publishable Information** (pages, articles, docs, templates)
- **Entity = Operational Data** (records, transactions, business objects)

---

## The Two Data Models

ContentOS provides two primary data models for different purposes:

### 1. Content Model (Publishing-Centric)
**Tables:** CONTENT_NODE, CONTENT_ITEM, CONTENT_VERSION, CONTENT_FIELD_VALUE, CONTENT_LAYOUT, ROUTE

**Purpose:** Authoring and publishing information for human consumption

**Characteristics:**
- Tree/hierarchical navigation
- Friendly URLs and routing
- Multi-version history (draft → published)
- Layout composition (regions, components)
- Localization per field
- Approval workflows
- Preview/staging
- SEO metadata

**Best For:**
- Blog posts, articles, pages
- Documentation and knowledge base
- Marketing/landing pages
- Policy documents
- Help center content
- UI page templates

---

### 2. Entity Model (Data-Centric)
**Tables:** ENTITY_DEFINITION, ENTITY_INSTANCE, ENTITY_RELATIONSHIP

**Purpose:** Managing operational and transactional data

**Characteristics:**
- Flexible JSON schema
- Simple CRUD operations
- State machines (not publishing workflows)
- Performance-optimized for queries
- Domain-specific business logic
- Cross-entity relationships
- Real-time updates

**Best For:**
- Transactional records (orders, payments, tickets)
- Business entities (customers, products, cases)
- Operational data (inventory, appointments, shipments)
- User-generated data (comments, reviews, submissions)
- System metadata (settings, configurations)

---

## Decision Matrix

| Question | Content Model | Entity Model |
|----------|---------------|--------------|
| **Needs SEO & public URLs?** | ✅ Yes | ❌ No |
| **Needs tree/hierarchy?** | ✅ Yes | ⚠️ Optional* |
| **Needs layout composition?** | ✅ Yes | ❌ No |
| **Needs versioning?** | ✅ Yes | ⚠️ Rarely |
| **Needs draft/publish workflow?** | ✅ Yes | ❌ No |
| **Needs localization?** | ✅ Yes | ⚠️ Sometimes |
| **Real-time updates?** | ⚠️ Rarely | ✅ Yes |
| **High transaction volume?** | ❌ No | ✅ Yes |
| **User-facing presentation?** | ✅ Yes | ⚠️ Via template |
| **CRUD by end-users?** | ⚠️ Authors only | ✅ Yes |

*ENTITY_INSTANCE supports parent_instance_id for hierarchies if needed

---

## Scenario 1: Case Management System

### What Uses Content Tables ✅

#### 1. Knowledge Base Articles
```sql
CONTENT_ITEM: "How to File a Case"
- Route: /help/filing/how-to-file-a-case
- Tree: /help/filing/
- Versions: v1.0, v1.1, v2.0 (legal updates)
- Localization: en, es, fr
- Layout: Hero + Steps + FAQ
- Workflow: Draft → Legal Review → Published
```

**Why Content?**
- Public-facing page (needs SEO)
- Friendly URL required
- Legal accuracy requires versioning
- Multi-language support
- Marketing can update layout
- Needs approval before publishing

---

#### 2. Case Detail Page Template
```sql
CONTENT_ITEM: "Case Detail Page Template"
- Route: /portal/cases/{caseId}
- Layout composition:
  - Header region: CaseHeader component
  - Main region: CaseDetails, ActivityFeed, Comments
  - Sidebar: CaseActions, RelatedCases
- One template, reused for ALL cases
```

**Why Content?**
- Defines presentation/layout
- Can be versioned (UI improvements)
- Can be A/B tested
- Can be localized
- Marketing/design team can modify

---

#### 3. Dashboard Page
```sql
CONTENT_ITEM: "Case Dashboard"
- Route: /portal/dashboard
- Layout:
  - Stats widgets
  - Recent activity
  - Quick actions
  - Charts and graphs
```

**Why Content?**
- Page layout definition
- Component composition
- Can be personalized per user role
- Design team can iterate

---

### What Uses Entity Tables ✅

#### 1. Case Records
```sql
ENTITY_INSTANCE: Case #12345
{
  "entityDefinitionKey": "case",
  "data": {
    "caseNumber": "CASE-2024-12345",
    "title": "Cannot access account",
    "description": "User reports login errors...",
    "status": "Open",
    "priority": "High",
    "assignedTo": "user-456",
    "createdAt": "2024-01-04T10:00:00Z",
    "dueDate": "2024-01-11T10:00:00Z"
  }
}
```

**Why Entity?**
- Operational data (not content)
- No SEO needed
- No layout composition
- No versioning (current state matters)
- Real-time updates (status changes)
- High volume (100,000+ cases)

---

#### 2. Case Activities
```sql
ENTITY_INSTANCE: Case Activity #789
{
  "entityDefinitionKey": "caseActivity",
  "data": {
    "caseId": "case-12345",
    "activityType": "status_change",
    "oldValue": "Open",
    "newValue": "InProgress",
    "performedBy": "user-456",
    "timestamp": "2024-01-04T14:30:00Z",
    "note": "Started investigation"
  }
}
```

**Why Entity?**
- Transactional data
- Audit trail (append-only)
- No presentation layer needed
- Very high volume

---

#### 3. Case Comments
```sql
COMMENT table (or ENTITY_INSTANCE):
{
  "caseId": "case-12345",
  "commentText": "Customer responded via email",
  "isInternal": false,
  "createdBy": "user-456",
  "createdOn": "2024-01-04T15:00:00Z"
}
```

**Why Entity?**
- User-generated data
- Simple text (no layout)
- No versioning needed
- Real-time updates

---

### Architecture Pattern

```
┌───────────────────────────────────────────────────────┐
│         CASE MANAGEMENT ARCHITECTURE                   │
├───────────────────────────────────────────────────────┤
│                                                        │
│  CONTENT LAYER (Presentation):                        │
│  ├── Knowledge Base (~500 articles)                   │
│  ├── Help Center (~200 articles)                      │
│  ├── Policy Documents (~50 docs)                      │
│  ├── Page Templates (~20 templates)                   │
│  │   ├── Case Detail Template                         │
│  │   ├── Case List Template                           │
│  │   ├── Dashboard Template                           │
│  │   └── Search Results Template                      │
│  └── Public Pages (~50 pages)                         │
│                                                        │
│  ENTITY LAYER (Data):                                 │
│  ├── Cases (~100,000 records)                         │
│  ├── Case Activities (~1,000,000 records)             │
│  ├── Case Parties (~200,000 records)                  │
│  ├── Hearings (~50,000 records)                       │
│  ├── Documents (~500,000 attachments)                 │
│  └── Comments (~2,000,000 records)                    │
│                                                        │
│  RUNTIME BINDING:                                     │
│  User visits: /portal/cases/case-12345                │
│  1. Load Case Entity (ENTITY_INSTANCE)                │
│  2. Load Template (CONTENT_ITEM)                      │
│  3. Bind data to template                             │
│  4. Render final HTML                                 │
│                                                        │
└───────────────────────────────────────────────────────┘
```

---

## Scenario 2: E-Commerce System

### What Uses Content Tables ✅

#### 1. Product Detail Page
```sql
CONTENT_ITEM: "iPhone 15 Pro Product Page"
- Route: /products/iphone-15-pro
- Localization: en-US, es-MX, fr-CA
- Layout composition:
  - Hero: ProductHero component
  - Features: FeatureGrid component
  - Specs: ProductSpecs component (binds to product entity)
  - Reviews: ReviewsWidget component
- Workflow: Marketing → Legal → Published
- A/B Testing: Test different hero images
```

**Why Content?**
- Marketing/presentation layer
- SEO critical
- Multi-language campaigns
- Layout composition
- Seasonal variations
- Needs approval workflow

---

#### 2. Category Landing Page
```sql
CONTENT_ITEM: "Smartphones Category Page"
- Route: /catalog/smartphones
- Layout:
  - Hero banner (seasonal campaign)
  - Featured products grid
  - Comparison widget
  - Buying guide content
- Localized: Different content per market
```

**Why Content?**
- Marketing campaigns
- SEO landing page
- Seasonal updates
- A/B testing layouts
- Content authoring needed

---

#### 3. Promotional Pages
```sql
CONTENT_ITEM: "Holiday Sale 2024"
- Route: /promotions/holiday-sale-2024
- Layout: Custom promotional design
- Time-limited: Valid 12/1 - 12/31
- Localized per market
```

**Why Content?**
- Campaign-specific
- Custom layouts
- Time-sensitive versioning
- Marketing team authoring

---

### What Uses Entity Tables ✅

#### 1. Product Master Data
```sql
ENTITY_INSTANCE: Product SKU
{
  "entityDefinitionKey": "product",
  "data": {
    "sku": "IPHONE-15-PRO-256-BLK",
    "name": "iPhone 15 Pro 256GB Black",
    "manufacturerPartNumber": "MU793LL/A",
    "barcode": "194253868798",
    "basePrice": 999.99,
    "costPrice": 750.00,
    "weight": 221,
    "dimensions": {"length": 159.9, "width": 76.7, "height": 8.25},
    "supplier": "Apple Inc.",
    "category": "electronics/phones/smartphones",
    "status": "active",
    "taxable": true
  }
}
```

**Why Entity?**
- SKU/inventory data
- Real-time pricing
- Operational data
- No layout needed
- High-frequency updates
- Business logic applies

---

#### 2. Inventory Levels
```sql
ENTITY_INSTANCE: Inventory
{
  "entityDefinitionKey": "inventory",
  "data": {
    "sku": "IPHONE-15-PRO-256-BLK",
    "locations": {
      "NY_WAREHOUSE": 150,
      "LA_WAREHOUSE": 75,
      "CHI_WAREHOUSE": 50
    },
    "available": 215,
    "reserved": 10,
    "incoming": 100,
    "expectedDate": "2024-01-15"
  }
}
```

**Why Entity?**
- Real-time stock levels
- Transactional updates
- High frequency changes
- No presentation needed

---

#### 3. Orders & Payments
```sql
ENTITY_INSTANCE: Order
{
  "entityDefinitionKey": "order",
  "data": {
    "orderNumber": "ORD-2024-00123",
    "status": "Shipped",
    "customer": "customer-456",
    "items": [
      {"sku": "IPHONE-15-PRO-256-BLK", "qty": 1, "price": 999.99}
    ],
    "subtotal": 999.99,
    "tax": 75.00,
    "shipping": 0.00,
    "total": 1074.99,
    "shippingAddress": {...},
    "paymentStatus": "Paid",
    "trackingNumber": "1Z999AA10123456784"
  }
}
```

**Why Entity?**
- Transactional record
- State machine (Cart → Paid → Shipped → Delivered)
- Real-time updates
- No versioning needed
- High volume

---

#### 4. Price Rules
```sql
ENTITY_INSTANCE: Price Rule
{
  "entityDefinitionKey": "priceRule",
  "data": {
    "ruleKey": "holiday_sale_2024",
    "discountType": "percentage",
    "discountValue": 15,
    "validFrom": "2024-12-01",
    "validUntil": "2024-12-31",
    "applicableProducts": ["category:electronics"],
    "minimumPurchase": 100.00,
    "stackable": false
  }
}
```

**Why Entity?**
- Business rules
- Real-time calculation
- Frequent changes
- No presentation layer

---

### Architecture Pattern

```
┌───────────────────────────────────────────────────────┐
│         E-COMMERCE ARCHITECTURE                        │
├───────────────────────────────────────────────────────┤
│                                                        │
│  CONTENT LAYER (Marketing/Presentation):              │
│  ├── Product Detail Pages (~500 templates)            │
│  │   → Bind to product entity at runtime             │
│  ├── Category Landing Pages (~100 pages)              │
│  ├── Promotional Pages (~50 campaigns)                │
│  ├── Brand Story Pages (~20 pages)                    │
│  ├── Shopping Guides (~200 articles)                  │
│  ├── Help Center (~300 articles)                      │
│  └── Email Templates (~50 templates)                  │
│                                                        │
│  ENTITY LAYER (Transactional Data):                   │
│  ├── Products (~50,000 SKUs)                          │
│  ├── Product Variants (~200,000 variants)             │
│  ├── Inventory (~50,000 stock records)                │
│  ├── Price Rules (~1,000 rules)                       │
│  ├── Orders (~1,000,000 orders)                       │
│  ├── Order Items (~3,000,000 line items)              │
│  ├── Customers (~500,000 accounts)                    │
│  ├── Payments (~1,000,000 transactions)               │
│  ├── Shipments (~800,000 shipments)                   │
│  └── Reviews (~100,000 reviews)                       │
│                                                        │
│  RUNTIME BINDING:                                     │
│  User visits: /products/iphone-15-pro                 │
│  1. Load Product Entity (SKU, price, inventory)       │
│  2. Load Content Template (product detail page)       │
│  3. Bind product data to template                     │
│  4. Render final page with real-time data             │
│                                                        │
└───────────────────────────────────────────────────────┘
```

---

## Key Insights

### 1. Templates vs Data
**Content = Templates** (created once, reused many times)  
**Entity = Data** (many instances)

```
Product Detail Page Template (CONTENT_ITEM)
    ↓ binds to
Product #1, #2, #3... #50,000 (ENTITY_INSTANCE)
```

### 2. Versioning Requirements
**Content:** Marketing wants to improve layouts → versioning  
**Entity:** Current state matters → no versioning (or simple state machine)

### 3. Update Frequency
**Content:** Weekly/monthly updates (campaigns, content refresh)  
**Entity:** Real-time updates (orders, inventory, status changes)

### 4. Authoring vs CRUD
**Content:** Authored by content teams, needs approval  
**Entity:** Created/updated by users, API calls, automated processes

### 5. Volume Characteristics
**Content:** Low volume (hundreds to thousands)  
**Entity:** High volume (thousands to millions)

---

## Anti-Patterns to Avoid

### ❌ Don't Duplicate Data
```
WRONG:
- Product Entity (SKU, price, inventory)
- Product Content (duplicate of above) ← DUPLICATION

RIGHT:
- Product Entity (SKU, price, inventory)
- Product Page Template (presentation only)
```

### ❌ Don't Use Content for Transactional Data
```
WRONG:
- Order as CONTENT_ITEM ← Orders aren't "content"

RIGHT:
- Order as ENTITY_INSTANCE
```

### ❌ Don't Use Entity for Public Pages
```
WRONG:
- Blog post as ENTITY_INSTANCE ← Loses SEO, routing, layout

RIGHT:
- Blog post as CONTENT_ITEM
```

---

## When to Use Both (Hybrid)

Some domains need **both models**:

### E-Commerce Product
- **Product DATA** → ENTITY_INSTANCE (SKU, price, inventory)
- **Product PAGE** → CONTENT_ITEM (marketing, layout, SEO)
- Runtime: Bind entity to content template

### Case Management
- **Case DATA** → ENTITY_INSTANCE (case records)
- **Case PORTAL PAGES** → CONTENT_ITEM (UI templates)
- Runtime: Bind case data to template

### Hospital System
- **Patient DATA** → ENTITY_INSTANCE (medical records)
- **Patient PORTAL PAGES** → CONTENT_ITEM (UI templates)
- **Help Center** → CONTENT_ITEM (health information articles)

---

## Summary Table

| Domain | Content Use Cases | Entity Use Cases |
|--------|-------------------|------------------|
| **Case Management** | Knowledge base, Help center, Page templates, Policies | Cases, Activities, Parties, Comments, Attachments |
| **E-Commerce** | Product pages, Category pages, Campaigns, Guides | Products, Orders, Payments, Inventory, Customers |
| **Hospital** | Health articles, Patient portal templates, Policies | Patients, Appointments, Records, Prescriptions |
| **Blog** | Blog posts, Pages, Categories (presentation) | Comments, Author profiles, Subscribers |
| **Library** | Help articles, Portal templates, Policies | Books, Members, Loans, Reservations |
| **Ticketing** | Help center, Portal templates, KB | Tickets, Comments, Attachments, Activities |

---

## Conclusion

**The Rule of Thumb:**

- **If it's FOR READING by humans and needs presentation** → Content Model
- **If it's FOR PROCESSING by systems and business logic** → Entity Model
- **If you need BOTH presentation and data** → Use both, bind at runtime

**Remember:**
- Content = How to display
- Entity = What to display
- Runtime binding = The magic that connects them

**This separation enables:**
- ✅ Clean architecture
- ✅ Performance optimization per use case
- ✅ Independent scaling
- ✅ Team autonomy (content team vs engineering team)
- ✅ A/B testing and experimentation
- ✅ Multi-channel delivery (web, mobile, API)

---

End of document.
