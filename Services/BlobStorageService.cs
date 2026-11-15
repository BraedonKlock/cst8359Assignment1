using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Assignment1.Services;

public class BlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString(
            "AzureBlobStorage")
            ?? throw new InvalidOperationException(
                "Azure Blob Storage connection string is missing.");

        var blobServiceClient = new BlobServiceClient(connectionString);

        _containerClient = blobServiceClient
            .GetBlobContainerClient("event-banners");
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        // Generate a unique filename to prevent overwriting other images.
        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (extension != ".jpg" &&
            extension != ".jpeg" &&
            extension != ".png" &&
            extension != ".gif" &&
            extension != ".webp")
        {
            throw new ArgumentException("Unsupported image format.");
        }

        if (file.Length == 0 || file.Length > 5 * 1024 * 1024)
        {
            throw new ArgumentException(
                "Image must be between 1 byte and 5 MB.");
        }

        string blobName = $"{Guid.NewGuid()}{extension}";

        BlobClient blobClient = _containerClient.GetBlobClient(blobName);

        await using Stream stream = file.OpenReadStream();

        await blobClient.UploadAsync(
            stream,
            new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });

        return blobClient.Uri.ToString();
    }
}
