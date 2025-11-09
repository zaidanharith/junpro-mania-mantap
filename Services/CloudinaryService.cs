using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Threading.Tasks;

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

        public async Task<string?> UploadImageAsync(string filePath, string folder = "BOzea/Users")
        {
            try
            {
                Console.WriteLine($"[CloudinaryService] Starting upload from: {filePath}");
                Console.WriteLine($"[CloudinaryService] Target folder: {folder}");

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(filePath),
                    Folder = folder,
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill")
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                Console.WriteLine($"[CloudinaryService] Upload status code: {result.StatusCode}");
                Console.WriteLine($"[CloudinaryService] Upload result: {result.JsonObj}");

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"[CloudinaryService] Upload successful: {result.SecureUrl}");
                    return result.SecureUrl.ToString();
                }

                Console.WriteLine($"[CloudinaryService] Upload failed with status: {result.StatusCode}");
                if (result.Error != null)
                {
                    Console.WriteLine($"[CloudinaryService] Error message: {result.Error.Message}");
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CloudinaryService] Exception during upload: {ex.Message}");
                Console.WriteLine($"[CloudinaryService] Stack trace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine($"Cloudinary upload error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            try
            {
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);
                return result.Result == "ok";
            }
            catch
            {
                return false;
            }
        }
    }
}
