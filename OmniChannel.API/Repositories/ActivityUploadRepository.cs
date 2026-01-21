using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using OmniChannel.API.Repositories;

public class ActivityUploadRepository : IActivityUploadRepository
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public ActivityUploadRepository(
        BlobServiceClient blobServiceClient,
        IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = configuration["AzureBlob:ContainerName"]!;
    }

    public string GenerateUploadSasUrl(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Filename is required", nameof(fileName));

        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(fileName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15)
        };

        sasBuilder.SetPermissions(
            BlobSasPermissions.Write | BlobSasPermissions.Create);

        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }
}
