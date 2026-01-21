using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Moq;

namespace OmniChannel.API.Tests.Repositories
{
    [TestFixture]
    public class ActivityUploadRepositoryTests
    {
        private IConfiguration _configuration;

        [SetUp]
        public void SetUp()
        {
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                { "AzureBlob:ContainerName", "test-container" }
                })
                .Build();
        }

        [Test]
        public void GenerateUploadSasUrl_ThrowsArgumentException_WhenFilenameIsEmpty()
        {
            // Arrange
            var blobServiceClientMock = new Mock<BlobServiceClient>();
            var repository = new ActivityUploadRepository(
                blobServiceClientMock.Object,
                _configuration);

            // Act + Assert
            Assert.Throws<ArgumentException>(() =>
                repository.GenerateUploadSasUrl(""));
        }
    }
}
