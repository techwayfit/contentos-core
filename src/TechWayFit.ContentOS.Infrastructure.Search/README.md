# TechWayFit.ContentOS.Infrastructure.Search

## Purpose
Concrete implementations of search indexing and querying.

## Responsibilities
- **Lucene.NET**: Local full-text search implementation
- **Azure Cognitive Search**: Cloud search integration
- **OpenSearch/Elasticsearch**: Distributed search (future)
- **ISearchIndex Implementation**: Unified search interface

## Key Principles
- **Provider-agnostic**: Business logic uses ISearchIndex interface
- **Configuration-driven**: Choose search provider via appsettings
- **Multi-tenant**: Tenant isolation in indexes
- **Real-time**: Support for real-time indexing and search

## Structure
```
Providers/
  LuceneSearchIndex.cs
  AzureSearchIndex.cs
Models/
  SearchDocument.cs
  SearchQuery.cs
DependencyInjection.cs
```

## Usage
In API/Program.cs:
```csharp
// Option 1: Lucene.NET (local)
services.AddLuceneSearch(configuration);

// Option 2: Azure Cognitive Search
services.AddAzureSearch(configuration);
```

## Configuration
```json
{
  "Search": {
    "Provider": "Lucene", // or "Azure" or "OpenSearch"
    "Lucene": {
      "IndexPath": "./indexes"
    },
    "Azure": {
      "ServiceName": "contentos-search",
      "ApiKey": "...",
      "IndexName": "contentos"
    }
  }
}
```

## Features
- Full-text search
- Faceted search
- Filtering and sorting
- Highlighting
- Suggestions/autocomplete
- Multi-language support

## Dependencies
- Abstractions (ISearchIndex interface)
- Lucene.NET (optional)
- Azure.Search.Documents (optional)
