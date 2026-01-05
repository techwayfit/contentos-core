# AI Vector Search & Image Similarity Guide

**A Beginner's Guide to Understanding Embeddings, Vector Search, and Image Similarity**

---

## Table of Contents
1. [What Are Embeddings?](#what-are-embeddings)
2. [How Vector Search Works](#how-vector-search-works)
3. [Distance Metrics Explained](#distance-metrics-explained)
4. [Search Algorithms Compared](#search-algorithms-compared)
5. [Image Search Techniques](#image-search-techniques)
6. [RAG (Retrieval Augmented Generation)](#rag-retrieval-augmented-generation)
7. [Decision Guide: Which Algorithm to Use?](#decision-guide)
8. [Implementation in ContentOS](#implementation-in-contentos)

---

## What Are Embeddings?

### The Basic Concept

**Embeddings** convert complex data (text, images, audio) into **numbers** that computers can compare.

```
Text:     "cute cat playing"
          ↓ (AI Model converts to numbers)
Embedding: [0.23, -0.15, 0.87, 0.45, -0.32, ...]  ← 512-1536 numbers
```

### Why Do We Need Embeddings?

**Problem:** How do you tell if two images are similar?
- You can't directly compare pixels (too noisy)
- File names don't capture visual content
- Tags/metadata are incomplete

**Solution:** Convert images to embeddings
```
Image A: "red car"     → [0.8, 0.2, 0.1, ...]
Image B: "blue car"    → [0.7, 0.3, 0.1, ...]  ← Similar! (car context)
Image C: "red apple"   → [0.1, 0.9, 0.8, ...]  ← Different!
```

### Visual Representation

```
3D Space Visualization (simplified from 512 dimensions):

         Red Objects
              ↑
              |   🍎 (red apple)
              |
              |
    🚗 ← ─ ─ ┼ ─ ─ → 🚙  (cars cluster together)
    (red)    |    (blue)
             |
             |
          Vehicles →
```

**Key Insight:** Similar things are **close together** in embedding space!

---

## How Vector Search Works

### Step-by-Step Example

**Scenario:** You search for "sports car" in an image library with 1 million images.

#### Step 1: Convert Your Query to an Embedding
```
Your text: "sports car"
           ↓ (CLIP model)
Query Vector: [0.85, 0.12, -0.33, 0.67, ...]
```

#### Step 2: Compare with All Images
```
Database has 1,000,000 images, each with an embedding:

Image 1: [0.82, 0.15, -0.30, 0.65, ...] → Distance: 0.08 ✓ Very similar!
Image 2: [0.10, 0.95, -0.88, 0.22, ...] → Distance: 0.92 ✗ Not similar
Image 3: [0.79, 0.18, -0.35, 0.70, ...] → Distance: 0.12 ✓ Similar!
...
Image 1M: [0.45, 0.67, 0.11, -0.22, ...] → Distance: 0.55 ✗ Not similar
```

#### Step 3: Return Top Results
```
Rank   Image      Distance    Description
────   ─────      ────────    ───────────
  1    Image_1     0.08       Red Ferrari
  2    Image_3     0.12       Blue Porsche
  3    Image_47    0.15       Yellow Lamborghini
  ...
```

### The Performance Problem

**Without indexing:** 
- Must compare query against ALL 1 million vectors
- 1 million distance calculations per search
- **Very slow!** (seconds to minutes)

**With indexing:**
- Smart data structures skip most comparisons
- Check only ~1,000 vectors (0.1% of database)
- **Fast!** (milliseconds)

---

## Distance Metrics Explained

### 1. Cosine Similarity (Most Common)

**What it measures:** The **angle** between two vectors (ignores length)

#### Visual Explanation
```
         Vector A (long)
              ↗
             /
            /
           / 15° angle ← Small angle = Very similar!
          /
         /
        ↗ Vector B (short)
       /
      /
```

```
         Vector C
              ↗
             /
            /
           / 85° angle ← Large angle = Not similar
          /
    ←────┘
    Vector D
```

**Formula:**
```
Similarity = cos(θ) = (A · B) / (|A| × |B|)

Results:
  1.0  = Identical direction (perfect match)
  0.5  = 60° angle (somewhat similar)
  0.0  = 90° angle (unrelated)
 -1.0  = 180° angle (opposite)
```

**When to use:**
- ✅ Image embeddings (CLIP, ResNet)
- ✅ Text embeddings (GPT, BERT)
- ✅ When embeddings are normalized to unit length

**SQL Example:**
```sql
-- pgvector operator: <=>
-- Lower distance = more similar
SELECT * FROM IMAGE_EMBEDDING 
ORDER BY embedding_vector <=> $query_vector 
LIMIT 10;
```

---

### 2. Euclidean Distance (L2)

**What it measures:** Straight-line distance in space (like measuring with a ruler)

#### Visual Explanation
```
2D Space:

Point A (3, 8) ●
               |╲
               | ╲
               |  ╲  Distance = √[(5-3)² + (5-8)²]
               |   ╲             = √[4 + 9] = 3.6
               |    ╲
               |     ╲
Point B (5, 5) ──────● 
```

**Formula:**
```
Distance = √[(x₁-x₂)² + (y₁-y₂)² + ... + (z₁-z₂)²]
```

**Real Example:**
```
Image A: [0.8, 0.2, 0.5]
Image B: [0.7, 0.3, 0.4]
Distance = √[(0.8-0.7)² + (0.2-0.3)² + (0.5-0.4)²]
         = √[0.01 + 0.01 + 0.01] = 0.17
```

**When to use:**
- ✅ Non-normalized embeddings
- ✅ When magnitude matters (e.g., confidence scores)
- ⚠️ Sensitive to scale (one large dimension can dominate)

**SQL Example:**
```sql
-- pgvector operator: <->
SELECT * FROM IMAGE_EMBEDDING 
ORDER BY embedding_vector <-> $query_vector 
LIMIT 10;
```

---

### 3. Dot Product (Inner Product)

**What it measures:** How much two vectors "align" (includes magnitude)

#### Visual Explanation
```
Long vectors = Higher scores

Vector A (long) ────────→
                 ↗
Vector B (short) →  Small angle + different lengths
                    Dot Product = |A| × |B| × cos(θ)
```

**Formula:**
```
Dot Product = (a₁ × b₁) + (a₂ × b₂) + ... + (aₙ × bₙ)
```

**Real Example:**
```
Vector A: [0.5, 0.8, 0.3]
Vector B: [0.6, 0.7, 0.4]
Dot Product = (0.5×0.6) + (0.8×0.7) + (0.3×0.4)
            = 0.30 + 0.56 + 0.12 = 0.98
```

**When to use:**
- ✅ **Fastest** computation (no sqrt needed)
- ✅ Normalized embeddings
- ✅ When you need maximum speed

---

### Comparison Example

**Same query, different metrics:**

```
Query:    [1.0, 0.0, 0.0]
Image A:  [0.9, 0.1, 0.0]  
Image B:  [0.0, 1.0, 0.0]  
Image C:  [0.5, 0.5, 0.0]

Metric          Image A    Image B    Image C    Winner
──────────────  ─────────  ─────────  ─────────  ──────
Cosine Sim       0.994      0.000      0.707     A (highest)
Euclidean        0.141      1.414      0.707     A (lowest)
Dot Product      0.900      0.000      0.500     A (highest)
                 ↑ All agree: Image A is most similar!
```

---

## Search Algorithms Compared

### The Challenge: Searching Large Databases

**Problem:**
- 1 million images × 512 numbers each = 512 million comparisons per search!
- At 1 microsecond per comparison = 0.5 seconds minimum
- **Too slow for real-time search**

**Solution:** Use indexes to skip most comparisons

---

### Algorithm 1: Brute Force KNN (Baseline)

**How it works:**
```
Query: "red car"

Check ALL images:
[✓] Image 1     distance: 0.15
[✓] Image 2     distance: 0.82
[✓] Image 3     distance: 0.23
[✓] Image 4     distance: 0.91
...
[✓] Image 1M    distance: 0.67

Sort by distance → Return top 10
```

**Diagram:**
```
Query →  [Compare] → Image 1
     →  [Compare] → Image 2
     →  [Compare] → Image 3
     →  [Compare] → ...
     →  [Compare] → Image 1,000,000
                     ↓
                  Sort & Return Top 10
```

**Pros:**
- ✅ 100% accuracy (finds exact nearest neighbors)
- ✅ Simple to implement
- ✅ No setup/training needed

**Cons:**
- ❌ O(n) complexity (linear with database size)
- ❌ **Very slow** for large datasets

**When to use:**
- Fewer than 10,000 images
- Development/testing
- When you need perfect accuracy

---

### Algorithm 2: IVFFlat (Inverted File with Flat Compression)

**How it works: Cluster-based search**

#### Step 1: Training Phase (One-time setup)
```
Group all 1M images into 100 clusters using k-means:

Cluster 1: Sports cars      (10,000 images)
Cluster 2: Sedans           (15,000 images)
Cluster 3: Cats             (8,000 images)
...
Cluster 100: Mountains      (12,000 images)
```

#### Step 2: Search Phase
```
Query: "red sports car"
  ↓
Find 5 nearest cluster centers:
  Cluster 1: Sports cars     (distance: 0.2) ✓ Check this
  Cluster 2: Sedans          (distance: 0.4) ✓ Check this
  Cluster 78: Red objects    (distance: 0.5) ✓ Check this
  Cluster 99: Cats           (distance: 0.9) ✗ Skip
  Cluster 100: Mountains     (distance: 0.95) ✗ Skip
  ↓
Only search ~30,000 images (3% of database!)
```

**Visual Diagram:**
```
Database with 100 clusters:

     ┌─────────┐
     │ Cluster │  10K sports cars
     │    1    │  ← Query is closest to this cluster
     └─────────┘
         ↑
        / \
       /   \
  Query    ┌─────────┐
  "red     │ Cluster │  15K sedans
  sports   │    2    │  ← Also close, check this
   car"    └─────────┘
      
      Far clusters (skipped):
      ┌─────────┐  ┌─────────┐
      │Cluster 3│  │Cluster 4│
      │  Cats   │  │Mountains│
      └─────────┘  └─────────┘
```

**Configuration:**
```sql
CREATE INDEX idx_image_ivfflat 
  ON IMAGE_EMBEDDING USING ivfflat (embedding_vector vector_cosine_ops)
  WITH (lists = 100);
  
-- Tuning guide:
-- lists = sqrt(total_rows) for balanced performance
-- More lists = faster search, but lower recall
```

**Trade-offs:**
```
Clusters  Images Checked  Speed      Recall
────────  ──────────────  ─────      ──────
  10          100,000      5x faster   98%
  100         10,000       50x faster  95%
  1000        1,000        500x faster 85%
```

**Pros:**
- ✅ **10-100x faster** than brute force
- ✅ Fast to build (minutes for 1M vectors)
- ✅ Good recall (90-95%)

**Cons:**
- ❌ Not 100% accurate (might miss some results)
- ❌ Requires tuning `lists` parameter
- ❌ Quality depends on cluster quality

**When to use:**
- 100K - 10M images
- When 90-95% recall is acceptable
- Rapid development/iteration

---

### Algorithm 3: HNSW (Hierarchical Navigable Small World)

**How it works: Multi-layer graph navigation**

Think of it like **hierarchical map navigation:**
- **Layer 3 (top):** Interstate highways (sparse, long jumps)
- **Layer 2:** State highways (medium density)
- **Layer 1:** City streets (dense connections)
- **Layer 0 (bottom):** Every image (complete graph)

#### Visual Diagram
```
Layer 3 (sparse):    A ─────────────────→ B
                     ↓                     ↓
                     
Layer 2 (medium):    C → D → E            F → G
                     ↓   ↓   ↓            ↓   ↓
                     
Layer 1 (dense):     H─I─J─K─L          M─N─O─P
                     ↓ ↓ ↓ ↓ ↓          ↓ ↓ ↓ ↓
                     
Layer 0 (all):       [10,000 images with local connections]
```

#### Search Process
```
Query: "Find similar to this cat image"

Step 1: Start at top layer (Layer 3)
  Current: Node A
  Check neighbors: B is closer → Jump to B
  
Step 2: Drop to Layer 2
  Current: Node B → Maps to Node F
  Check neighbors: G is closer → Move to G
  
Step 3: Drop to Layer 1
  Current: Node G → Maps to Node N
  Check neighbors: O, P → P is closest
  
Step 4: Layer 0 (ground level)
  Current: Node P
  Check all neighbors in detail
  Found: Top 10 most similar images!
  
Total checks: ~50 nodes (out of 1,000,000!)
```

**Configuration:**
```sql
CREATE INDEX idx_image_hnsw 
  ON IMAGE_EMBEDDING USING hnsw (embedding_vector vector_cosine_ops)
  WITH (m = 16, ef_construction = 64);

-- Parameters explained:
-- m = 16:  Each node connects to 16 neighbors (more = better recall)
-- ef_construction = 64: Search width during building (higher = better quality)

-- Query-time tuning:
SET hnsw.ef_search = 100;  -- Search more neighbors at runtime
```

**Pros:**
- ✅ **Best recall** (>99%) at high speed
- ✅ Scalable to billions of vectors
- ✅ Consistent query performance (log-like)

**Cons:**
- ❌ **Slow to build** (hours for 10M+ vectors)
- ❌ More memory usage than IVFFlat
- ❌ Updates are expensive (not for high-write workloads)

**When to use:**
- Production systems
- >1M images
- When recall/accuracy is critical
- Read-heavy workload

---

### Algorithm Comparison Chart

```
Algorithm     Speed        Recall    Build Time   Memory    Best For
────────────  ───────────  ────────  ──────────   ────────  ────────────────
Brute Force   Slowest      100%      None         Low       <10K images
IVFFlat       Fast (50x)   90-95%    Minutes      Medium    100K-10M images
HNSW          Fast (100x)  >99%      Hours        High      >1M images, prod
Product Quant Ultra-fast   85-90%    Hours        Very Low  >100M images
```

---

## Image Search Techniques

### 1. Direct Image Similarity (What You've Used)

**Scenario:** "Find images similar to this one"

```
User uploads:  🚗 (image of red car)
              ↓ (Convert to embedding)
Query Vector: [0.8, 0.2, 0.5, ...]
              ↓ (Search database)
Results:      🚗 🚙 🏎️ (similar cars)
```

**SQL:**
```sql
SELECT attachment_id, file_name
FROM IMAGE_EMBEDDING
WHERE tenant_id = $tenant_id
ORDER BY embedding_vector <=> $uploaded_image_embedding
LIMIT 10;
```

---

### 2. Text-to-Image Search (Multimodal)

**The Magic:** Search images using **text descriptions**!

**How it works with CLIP (Contrastive Language-Image Pre-training):**

```
Training Phase (done by OpenAI):
  Image: 🐱         Text: "cute cat"
         ↓                  ↓
    CLIP Image          CLIP Text
      Encoder            Encoder
         ↓                  ↓
    [0.2, 0.8, ...]    [0.2, 0.8, ...]  ← Same embedding space!
         └──────────┬──────────┘
              Embeddings are close together
```

**Your Search:**
```
User types: "red sports car at sunset"
           ↓ (CLIP Text Encoder)
Text Vector: [0.85, 0.12, -0.33, ...]
           ↓ (Search image embeddings)
Results:    🏎️🌅 🚗🌆 🏁🌄 (matching images!)
```

**SQL:**
```sql
-- Text query converted to embedding via CLIP
SELECT ie.attachment_id, a.file_name, a.storage_path
FROM IMAGE_EMBEDDING ie
JOIN ATTACHMENT a ON ie.attachment_id = a.id
WHERE ie.tenant_id = $tenant_id
  AND ie.embedding_model = 'clip-vit-large-patch14'  -- Must use CLIP!
ORDER BY ie.embedding_vector <=> $text_query_embedding
LIMIT 10;
```

**Why CLIP is Special:**
- ✅ Shared embedding space (text and images)
- ✅ No need for manual tags/labels
- ✅ Understands context ("beach at sunset" ≠ "beach at noon")

---

### 3. Hybrid Search (Vector + Metadata)

**Problem:** Pure vector search ignores useful metadata

**Example:**
```
User: "Find large images of red cars"
      ↑         ↑              ↑
   Metadata  Metadata      Semantic
```

**Two-stage filtering:**
```
Stage 1: Metadata filters (fast database index)
  - width >= 1920
  - height >= 1080
  - dominant_color = 'red'
  ↓ Reduces 1M images to 50K

Stage 2: Vector search on filtered set
  - embedding_vector <=> query_vector
  ↓ Returns top 10 from 50K
```

**SQL:**
```sql
SELECT ie.attachment_id, a.file_name,
       ie.embedding_vector <=> $query_vector AS similarity
FROM IMAGE_EMBEDDING ie
JOIN ATTACHMENT a ON ie.attachment_id = a.id
WHERE ie.tenant_id = $tenant_id
  -- Metadata filters (uses regular indexes)
  AND (ie.image_metadata->>'width')::int >= 1920
  AND (ie.image_metadata->>'height')::int >= 1080
  AND ie.image_metadata->'dominant_colors' ? 'red'
  -- Vector search on filtered subset
ORDER BY similarity
LIMIT 10;
```

**Performance Impact:**
```
Without metadata filtering:  Search 1,000,000 vectors
With metadata filtering:     Search 50,000 vectors (20x faster!)
```

---

### 4. Color-Based Search

**Technique:** Combine embeddings with color histograms

**Color Histogram:**
```
Image:  🌅 (sunset photo)

Red:     ████████████████ 60%
Orange:  ████████ 30%
Yellow:  ███ 10%
Blue:    █ 5%
Other:   1%

Histogram Vector: [0.60, 0.30, 0.10, 0.05, 0.01]
```

**Combined Scoring:**
```
Result Score = (0.7 × Semantic Similarity) + (0.3 × Color Similarity)

Example:
  Image A: Sports car (red) 
    Semantic: 0.9 (very similar car)
    Color:    0.8 (red matches)
    Combined: 0.7×0.9 + 0.3×0.8 = 0.87 ✓ Best match
    
  Image B: Sports car (blue)
    Semantic: 0.9 (very similar car)
    Color:    0.2 (color doesn't match)
    Combined: 0.7×0.9 + 0.3×0.2 = 0.69 ✗ Lower rank
```

---

### 5. Perceptual Hashing (Duplicate Detection)

**Different from embeddings!** Finds **exact or near-exact** duplicates.

**How it works:**
```
Original Image:     Resize to 8×8    Convert to grayscale    Compare pixels
┌─────────────┐    ┌────────┐        ┌────────┐
│ 🏠          │ →  │████░░░░│   →    │76543210│  → Hash: 10110010...
│   (1920px)  │    │████░░░░│        │87654321│
└─────────────┘    └────────┘        └────────┘
                   64 pixels          64-bit hash
```

**Finding duplicates:**
```
Hash A: 10110010101...  (Original image)
Hash B: 10110010100...  (Same image, slightly compressed)
        ↑↑↑↑↑↑↑↑↑↑↑
        Differs in only 1 bit → Same image!

Hamming Distance = count different bits
  0 bits different  = Identical
  1-5 bits different = Near duplicate (crop, compress)
  10+ bits different = Different images
```

**Use cases:**
- ✅ Deduplication (storage savings)
- ✅ Copyright detection
- ✅ Prevent duplicate uploads

**SQL:**
```sql
-- XOR hashes and count different bits
SELECT attachment_id, file_name,
       bit_count(phash # $query_phash) AS hamming_distance
FROM IMAGE_PHASH
WHERE bit_count(phash # $query_phash) <= 5  -- 5-bit threshold
ORDER BY hamming_distance;
```

---

## RAG (Retrieval Augmented Generation)

### What is RAG?

**Problem:** AI models have limited knowledge (training data cutoff)

**Solution:** Give AI access to **your documents** at query time

```
Traditional AI:
  User: "What's our return policy?"
        ↓
  AI (from training data): "I don't have access to specific policies..."
  ❌ Not helpful

RAG-Enhanced AI:
  User: "What's our return policy?"
        ↓
  Step 1: Search your documents → Find "Return Policy v3.2"
        ↓
  Step 2: Pass document + question to AI
        ↓
  AI: "According to your policy, you offer 30-day returns..."
  ✅ Accurate, up-to-date!
```

### RAG Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    RAG SYSTEM                            │
├─────────────────────────────────────────────────────────┤
│                                                           │
│  User Question: "What's our return policy?"              │
│        ↓                                                  │
│  ┌──────────────────────────────────────┐               │
│  │  STEP 1: RETRIEVAL                    │               │
│  │  ───────────────────────               │               │
│  │  1. Convert question to embedding      │               │
│  │  2. Search CONTENT_RAG_CHUNKS table   │               │
│  │  3. Find top 5 relevant chunks         │               │
│  └──────────────────────────────────────┘               │
│        ↓                                                  │
│  Retrieved Context:                                       │
│  ┌────────────────────────────────────┐                 │
│  │ Chunk 1: "30-day return policy..." │                 │
│  │ Chunk 2: "Items must be unused..." │                 │
│  │ Chunk 3: "Original packaging..."   │                 │
│  └────────────────────────────────────┘                 │
│        ↓                                                  │
│  ┌──────────────────────────────────────┐               │
│  │  STEP 2: AUGMENTATION                 │               │
│  │  ──────────────────────                │               │
│  │  Build prompt with context:            │               │
│  │                                         │               │
│  │  "Context: [chunks above]              │               │
│  │   Question: What's our return policy?  │               │
│  │   Answer based on context above:"      │               │
│  └──────────────────────────────────────┘               │
│        ↓                                                  │
│  ┌──────────────────────────────────────┐               │
│  │  STEP 3: GENERATION                    │               │
│  │  ────────────────                       │               │
│  │  Send to AI (GPT-4, Claude, etc.)      │               │
│  └──────────────────────────────────────┘               │
│        ↓                                                  │
│  AI Response:                                             │
│  "According to the policy, you have 30 days to           │
│   return items. They must be unused and in original      │
│   packaging..."                                           │
│        ↓                                                  │
│  User receives accurate answer with citations!           │
│                                                           │
└─────────────────────────────────────────────────────────┘
```

### Why Chunking is Critical

**Problem:** Documents are too long for AI context windows

```
Full Document (10,000 words):
┌────────────────────────────────────────┐
│ Section 1: Introduction (1000 words)   │
│ Section 2: Returns (500 words) ← ✓    │
│ Section 3: Shipping (2000 words)       │
│ Section 4: Warranties (3000 words)     │
│ Section 5: Contact (500 words)         │
│ ... (3000 more words)                  │
└────────────────────────────────────────┘
      ↓ Too long! Can't fit in context
      
Chunked Approach:
┌────────────┐ ┌────────────┐ ┌────────────┐
│ Chunk 1:   │ │ Chunk 2:   │ │ Chunk 3:   │
│ Intro      │ │ Returns ✓  │ │ Shipping   │
│ (500 words)│ │ (500 words)│ │ (500 words)│
└────────────┘ └────────────┘ └────────────┘
                     ↑
              Only this chunk is relevant!
              Send just this to AI
```

### Chunking Strategy

**Option 1: Fixed-size chunking**
```
Document: "The quick brown fox jumps over the lazy dog. The dog was sleeping..."

Chunk size: 10 words, Overlap: 3 words

Chunk 1: "The quick brown fox jumps over the lazy dog. The"
Chunk 2: "dog. The dog was sleeping under the tree. Birds"
                ↑↑↑ Overlap ensures context continuity
```

**Option 2: Semantic chunking (better)**
```
Document with sections:
  # Introduction
  This is the introduction...
  
  # Return Policy          ← Chunk boundary
  30-day returns...
  
  # Shipping Policy        ← Chunk boundary
  Free shipping over $50...

Chunks align with document structure!
```

### Three-Table RAG Architecture

```
┌─────────────────────────────────────────────────────────┐
│                  TABLE STRUCTURE                         │
├─────────────────────────────────────────────────────────┤
│                                                           │
│  CONTENT_RAG_CHUNKS (Text management)                    │
│  ┌────────────────────────────────────────┐             │
│  │ id: uuid-123                            │             │
│  │ source_id: content-item-456             │             │
│  │ chunk_index: 2                          │             │
│  │ chunk_text: "30-day return policy..."   │ ← For display
│  │ chunk_tokens: 150                       │             │
│  │ metadata: {section: "returns", page: 5} │             │
│  └────────────────────────────────────────┘             │
│        ↓ One-to-many                                      │
│                                                           │
│  CONTENT_EMBEDDING (Search vectors)                      │
│  ┌────────────────────────────────────────┐             │
│  │ id: embedding-789                       │             │
│  │ chunk_id: uuid-123  ← Links to chunk   │             │
│  │ embedding_model: "text-ada-002"         │             │
│  │ embedding_vector: [0.23, -0.15, ...]    │ ← Searchable
│  │ locale: "en-US"                         │             │
│  └────────────────────────────────────────┘             │
│                                                           │
│  IMAGE_EMBEDDING (Separate for images)                   │
│  ┌────────────────────────────────────────┐             │
│  │ id: img-embedding-999                   │             │
│  │ attachment_id: image-888                │             │
│  │ embedding_model: "clip-vit-large"       │             │
│  │ embedding_vector: [0.87, 0.45, ...]     │             │
│  └────────────────────────────────────────┘             │
│                                                           │
└─────────────────────────────────────────────────────────┘
```

### RAG Query Flow

```sql
-- Step 1: User asks question
-- "What's the return policy?"
-- ↓ Convert to embedding via OpenAI

-- Step 2: Search for relevant chunks
WITH ranked_chunks AS (
  SELECT 
    ce.chunk_id,
    crc.chunk_text,
    crc.chunk_metadata,
    ce.embedding_vector <=> $question_embedding AS similarity
  FROM CONTENT_EMBEDDING ce
  JOIN CONTENT_RAG_CHUNKS crc ON ce.chunk_id = crc.id
  WHERE ce.tenant_id = $tenant_id
    AND ce.locale = 'en-US'
    AND crc.is_active = true
  ORDER BY similarity
  LIMIT 5
)
SELECT chunk_text FROM ranked_chunks;

-- Step 3: Results
-- ┌──────────────────────────────────────────┐
-- │ "30-day return policy for all items..."  │
-- │ "Items must be unused and in original..." │
-- │ "Refunds processed within 7 business..." │
-- └──────────────────────────────────────────┘

-- Step 4: Build prompt
-- Context: [chunks above]
-- Question: What's the return policy?
-- Answer:

-- Step 5: Send to GPT-4 → Get answer with citations!
```

---

## Decision Guide

### Flowchart: Which Algorithm Should I Use?

```
START: How many images/documents?
  │
  ├─ < 10,000
  │    └─→ Use: BRUTE FORCE (no index)
  │        - Simple, 100% accurate
  │        - Fast enough for small datasets
  │
  ├─ 10,000 - 100,000
  │    └─→ Use: IVFFlat (lists = 100)
  │        - Good balance of speed and accuracy
  │        - Fast to build
  │
  ├─ 100,000 - 10,000,000
  │    │
  │    ├─ Need max speed? (90% accuracy OK?)
  │    │   └─→ Use: IVFFlat (lists = 1000)
  │    │
  │    └─ Need max accuracy? (can wait for indexing?)
  │        └─→ Use: HNSW (m=16, ef=64)
  │
  └─ > 10,000,000
       │
       ├─ Limited memory?
       │   └─→ Use: Product Quantization (external system)
       │
       └─ Have memory?
           └─→ Use: HNSW (m=32, ef=128)
```

### Quick Reference Table

| Your Situation | Recommended Approach |
|----------------|---------------------|
| **Just learning AI** | Start with cosine similarity + brute force |
| **Building MVP** | IVFFlat index, 90% recall is fine |
| **Production system** | HNSW for best quality |
| **Text → Image search** | Use CLIP embeddings + text-to-image |
| **Find duplicates** | Perceptual hashing (phash) |
| **Content search for AI** | RAG with chunking |
| **Limited budget** | IVFFlat (cheaper than HNSW) |
| **Need 99%+ accuracy** | HNSW or brute force |

---

## Implementation in ContentOS

### Database Schema

```sql
-- 1. Text chunks (no embeddings, just text management)
CREATE TABLE CONTENT_RAG_CHUNKS (
  id UUID PRIMARY KEY,
  tenant_id UUID NOT NULL,
  source_type VARCHAR(50),      -- 'content_item', 'attachment', 'comment'
  source_id UUID,
  chunk_index INT,
  chunk_text TEXT,              -- Original text for citations
  chunk_tokens INT,
  chunk_metadata JSONB,         -- {section, page, heading}
  created_on TIMESTAMP,
  is_active BOOLEAN
);

-- 2. Text embeddings (for semantic search)
CREATE TABLE CONTENT_EMBEDDING (
  id UUID PRIMARY KEY,
  tenant_id UUID NOT NULL,
  chunk_id UUID REFERENCES CONTENT_RAG_CHUNKS(id),
  embedding_model VARCHAR(100),
  embedding_vector vector(1536),  -- Adjust dimension per model
  locale VARCHAR(10),
  tags TEXT[],
  indexed_at TIMESTAMP,
  is_active BOOLEAN
);

-- Vector index for fast search
CREATE INDEX idx_content_embedding_vector 
  ON CONTENT_EMBEDDING USING hnsw (embedding_vector vector_cosine_ops)
  WITH (m = 16, ef_construction = 64);

-- 3. Image embeddings (separate table for different needs)
CREATE TABLE IMAGE_EMBEDDING (
  id UUID PRIMARY KEY,
  tenant_id UUID NOT NULL,
  attachment_id UUID REFERENCES ATTACHMENT(id),
  embedding_model VARCHAR(100),
  embedding_vector vector(512),   -- CLIP uses 512 dims
  image_metadata JSONB,
  created_on TIMESTAMP,
  is_active BOOLEAN
);

-- Vector index for image search
CREATE INDEX idx_image_embedding_vector 
  ON IMAGE_EMBEDDING USING hnsw (embedding_vector vector_cosine_ops)
  WITH (m = 16, ef_construction = 64);
```

### Example Queries

#### 1. Simple Image Similarity
```sql
-- Find images similar to uploaded image
SELECT ie.attachment_id, a.file_name,
       ie.embedding_vector <=> $query_embedding AS similarity
FROM IMAGE_EMBEDDING ie
JOIN ATTACHMENT a ON ie.attachment_id = a.id
WHERE ie.tenant_id = $tenant_id
ORDER BY similarity
LIMIT 10;
```

#### 2. Text-to-Image Search (CLIP)
```sql
-- Search images using text query
SELECT ie.attachment_id, a.file_name,
       ie.embedding_vector <=> $text_query_embedding AS similarity
FROM IMAGE_EMBEDDING ie
JOIN ATTACHMENT a ON ie.attachment_id = a.id
WHERE ie.tenant_id = $tenant_id
  AND ie.embedding_model LIKE 'clip%'  -- Must use multimodal model
ORDER BY similarity
LIMIT 10;
```

#### 3. RAG Document Search
```sql
-- Find relevant document chunks for AI
SELECT crc.chunk_text, crc.chunk_metadata,
       ce.embedding_vector <=> $question_embedding AS relevance
FROM CONTENT_EMBEDDING ce
JOIN CONTENT_RAG_CHUNKS crc ON ce.chunk_id = crc.id
WHERE ce.tenant_id = $tenant_id
  AND ce.locale = 'en-US'
  AND crc.is_active = true
ORDER BY relevance
LIMIT 5;
```

#### 4. Hybrid Search (Metadata + Vector)
```sql
-- Combine filters with semantic search
SELECT ie.attachment_id, a.file_name,
       ie.embedding_vector <=> $query_embedding AS similarity
FROM IMAGE_EMBEDDING ie
JOIN ATTACHMENT a ON ie.attachment_id = a.id
WHERE ie.tenant_id = $tenant_id
  AND ie.is_active = true
  -- Metadata filters (fast index lookup)
  AND (ie.image_metadata->>'width')::int >= 1024
  AND ie.image_metadata->'dominant_colors' ? 'blue'
  -- Vector search on filtered subset
ORDER BY similarity
LIMIT 10;
```

---

## Key Takeaways

### For Beginners

1. **Embeddings = Numbers representing meaning**
   - Images, text, audio → Arrays of numbers
   - Similar things have similar numbers

2. **Distance = Similarity**
   - Cosine similarity: Most common (use this!)
   - Smaller distance = more similar

3. **Indexes = Speed**
   - Small dataset (<10K): No index needed
   - Medium (10K-1M): Use IVFFlat
   - Large (>1M): Use HNSW

4. **Start Simple, Scale Up**
   - Begin with brute force search
   - Add IVFFlat when slow
   - Move to HNSW for production

### Common Mistakes to Avoid

❌ **Using wrong distance metric**
   - CLIP embeddings → Use cosine similarity
   - Don't mix metrics from different models

❌ **Not filtering by tenant**
   - Always filter `WHERE tenant_id = $tenant_id` first
   - Security + performance

❌ **Forgetting to normalize embeddings**
   - If using dot product, normalize to unit length
   - Cosine similarity handles this automatically

❌ **Building HNSW index for small datasets**
   - Overkill for <100K vectors
   - Takes hours to build, no benefit

❌ **Not chunking for RAG**
   - Long documents don't fit in AI context
   - Chunk to 500-1000 tokens

---

## Next Steps

### Phase 1: Learning (Now)
- ✅ Understand concepts (this document)
- ✅ Experiment with small dataset (<1K images)
- ✅ Use cosine similarity + brute force

### Phase 2: Development
- Add CONTENT_RAG_CHUNKS, CONTENT_EMBEDDING, IMAGE_EMBEDDING tables
- Implement basic search endpoints
- Test with IVFFlat index

### Phase 3: Production
- Switch to HNSW for better recall
- Add hybrid search (metadata + vectors)
- Implement RAG for AI features

### Phase 4: Optimization
- Monitor performance metrics
- Tune index parameters
- Add re-ranking, diversity filters

---

## Glossary

| Term | Simple Explanation |
|------|-------------------|
| **Embedding** | Numbers representing meaning of text/image/etc |
| **Vector** | Array of numbers (e.g., [0.23, -0.15, 0.87]) |
| **Dimension** | How many numbers in a vector (512, 1536, etc.) |
| **Cosine Similarity** | Measure of angle between vectors (0-1) |
| **KNN** | K-Nearest Neighbors - find K most similar items |
| **Index** | Data structure for fast search (like book index) |
| **IVFFlat** | Index using clusters (fast, 90% accuracy) |
| **HNSW** | Index using graph (fastest, 99% accuracy) |
| **CLIP** | AI model for text + image embeddings |
| **RAG** | Retrieval Augmented Generation (AI + search) |
| **Chunking** | Splitting documents into smaller pieces |
| **Multimodal** | Works with multiple types (text + images) |
| **Perceptual Hash** | Fingerprint for duplicate detection |

---

**Document Version:** 1.0  
**Last Updated:** January 5, 2026  
**Author:** ContentOS Team  
**Purpose:** Educational guide for AI/ML beginners implementing vector search
