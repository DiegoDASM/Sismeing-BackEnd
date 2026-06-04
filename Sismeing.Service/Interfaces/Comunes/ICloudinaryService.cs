namespace Sismeing.Service.Interfaces.Comunes
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(Stream fileStream, string fileName, string folder);
    }
}
