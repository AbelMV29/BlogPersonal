using BlogPersonal.Extras;
using BlogPersonal.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace BlogPersonal.Services
{
    public class CloudinaryPhotoService : ICloudService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryPhotoService(CloudinarySettings cloudinarySettings)
        {
            var cuentaCloud = new Account(cloudinarySettings.Cloud, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret);
            _cloudinary = new Cloudinary(cuentaCloud);
        }
        public async Task<string?> UploadPreviewPost(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = ConfigureImageTransformation(new Transformation().Height(150).Width(400).Crop("scale"), stream, file.FileName);
            return await SubirMedia(uploadParams);
        }
        public async Task<string?> UploadMedia(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = ConfigureImageTransformation(new Transformation().Height(1.0).Width(1.0).Crop("scale"), stream, file.FileName);
            return await SubirMedia(uploadParams);
        }

        public async Task<string?> SubirMedia(RawUploadParams uploadParams)
        {
            try
            {
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.Url.ToString();
            }
            catch
            {
                return null;
            }
        }

        public ImageUploadParams ConfigureImageTransformation(Transformation transformation, Stream stream, string name)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription
                {
                    FileName = name,
                    Stream = stream
                },
                Transformation = transformation
            };
            return uploadParams;
        }
    }
}
