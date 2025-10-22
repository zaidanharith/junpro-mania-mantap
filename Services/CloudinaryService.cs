using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace BOZea.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(string cloudName, string apiKey, string apiSecret)
        {
            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        // Upload dari file path lokal
        public string UploadImage(string filePath, string folder = "images")
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(filePath),
                Folder = folder
            };

            var result = _cloudinary.Upload(uploadParams);
            return result.SecureUrl.ToString();
        }
    }
}
