namespace TechWayFit.ContentOS.Contracts.Dtos;

public sealed record MediaMetadataResponse(
    Guid MediaId,
    string FileName,
    string ContentType,
    long SizeInBytes,
    string Url,
    DateTime CreatedAt);
