# TechWayFit.ContentOS.Infrastructure.Storage

## Purpose
Concrete implementations of blob/file storage.

## Responsibilities
- **Local File System**: Store files on disk
- **Azure Blob Storage**: Integration with Azure Storage
- **AWS S3**: Integration with Amazon S3
- **IBlobStore Implementation**: Unified interface for all providers

## Key Principles
- **Provider-agnostic**: Business logic uses IBlobStore interface
- **Configuration-driven**: Choose provider via appsettings
- **Multi-tenant**: Tenant isolation in blob paths/containers
- **Stream-based**: Efficient handling of large files

## Structure
```
Providers/
  LocalFileSystemBlobStore.cs
  AzureBlobStore.cs
  S3BlobStore.cs
DependencyInjection.cs
```

## Usage
In API/Program.cs:
```csharp
// Option 1: Local file system
services.AddLocalBlobStorage(configuration);

// Option 2: Azure Blob Storage
services.AddAzureBlobStorage(configuration);

// Option 3: AWS S3
services.AddS3BlobStorage(configuration);
```

## Configuration
```json
{
  "BlobStorage": {
    "Provider": "LocalFileSystem", // or "Azure" or "S3"
    "LocalFileSystem": {
      "RootPath": "./storage"
    },
    "Azure": {
      "ConnectionString": "...",
      "ContainerName": "contentos"
    },
    "S3": {
      "BucketName": "contentos",
      "Region": "us-east-1"
    }
  }
}
```

## Dependencies
- Abstractions (IBlobStore interface)
- Azure.Storage.Blobs (optional)
- AWSSDK.S3 (optional)
