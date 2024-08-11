namespace BlogPersonal.Interfaces
{
    public interface ICloudService
    {
        Task<string?> UploadPreviewPost(IFormFile file);
        Task<string?> UploadMedia(IFormFile file);
    }
}
