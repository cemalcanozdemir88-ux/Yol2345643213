namespace TedarikciHizmet.Services.Interfaces
{
    public interface ITikkoUploadService
    {
        Task<string> UploadImageAsync(byte[] fileContent, string contentType);
        Task<List<string>> UploadImagesAsync(IEnumerable<(byte[] File, string ContentType)> files);
    }
}
