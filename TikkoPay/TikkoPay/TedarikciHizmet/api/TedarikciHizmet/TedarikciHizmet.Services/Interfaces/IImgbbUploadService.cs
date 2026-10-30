namespace TedarikciHizmet.Services.Interfaces
{
    public interface IImgbbUploadService
    {
        Task<string> UploadImageAsync(byte[] fileContent);
        Task<List<string>> UploadImagesAsync(IEnumerable<byte[]> files);
    }
}
