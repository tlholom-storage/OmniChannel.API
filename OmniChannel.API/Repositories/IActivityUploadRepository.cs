namespace OmniChannel.API.Repositories
{
    public interface IActivityUploadRepository
    {
        string GenerateUploadSasUrl(string fileName);
    }
}
