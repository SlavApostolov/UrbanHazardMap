
using UrbanHazardMap.Services;

namespace UrbanHazardMap.Infrastructure
{
    public class PhotoStorageService : IPhotoStorageService
    {
        private const long MaxFileSize = 5 * 1024 * 1024;
        private readonly string _uploadsFolder;

        public PhotoStorageService(IWebHostEnvironment environment)
        {
            var webRoot = environment.WebRootPath?? Path.Combine(
                    environment.ContentRootPath,
                    "wwwroot");

            _uploadsFolder = Path.Combine(webRoot, "uploads", "reports");

            Directory.CreateDirectory(_uploadsFolder);
        }

        public async Task<string> SaveAsync(Stream content, string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();

            if (extension != ".jpg" &&
                extension != ".jpeg" &&
                extension != ".png")
            {
                throw new ArgumentException("Only JPG and PNG photos are allowed.");
            }

            using var buffer = new MemoryStream();

            var chunk = new byte[81920];
            long totalBytes = 0;
            int bytesRead;

            while ((bytesRead = await content.ReadAsync(chunk.AsMemory())) > 0)
            {
                totalBytes += bytesRead;

                if (totalBytes > MaxFileSize)
                {
                    throw new ArgumentException("Photo must not exceed 5 MB.");
                }

                await buffer.WriteAsync(chunk.AsMemory(0, bytesRead));
            }

            buffer.Position = 0;

            var header = new byte[8];
            var headerLength = await buffer.ReadAsync(header.AsMemory());

            bool isJpeg =
                headerLength >= 3 &&
                header[0] == 0xFF &&
                header[1] == 0xD8 &&
                header[2] == 0xFF;

            bool isPng =
                headerLength >= 8 &&
                header[0] == 0x89 &&
                header[1] == 0x50 &&
                header[2] == 0x4E &&
                header[3] == 0x47 &&
                header[4] == 0x0D &&
                header[5] == 0x0A &&
                header[6] == 0x1A &&
                header[7] == 0x0A;

            if ((extension == ".png" && !isPng) ||
                ((extension == ".jpg" ||
                  extension == ".jpeg") && !isJpeg))
            {
                throw new ArgumentException("The file content does not match its image type.");
            }

            var safeExtension = isPng ? ".png" : ".jpg";

            var fileName =$"{Guid.NewGuid():N}{safeExtension}";

            var fullPath = Path.Combine(_uploadsFolder, fileName);

            buffer.Position = 0;

            await using (var file = new FileStream(
                fullPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None))
            {
                await buffer.CopyToAsync(file);
            }

            return $"/uploads/reports/{fileName}";
        }

        public Task<Stream?> OpenReadAsync(string photoPath)
        {
            if (string.IsNullOrWhiteSpace(photoPath))
            {
                return Task.FromResult<Stream?>(null);
            }

            var fileName = Path.GetFileName(photoPath.Replace('\\', '/'));

            var extension = Path.GetExtension(fileName);
            var stem = Path.GetFileNameWithoutExtension(fileName);

            if (!Guid.TryParseExact(stem, "N", out _) ||
                (extension != ".jpg" &&
                 extension != ".png"))
            {
                return Task.FromResult<Stream?>(null);
            }

            var fullPath = Path.Combine(_uploadsFolder, fileName);

            if (!File.Exists(fullPath))
            {
                return Task.FromResult<Stream?>(null);
            }

            Stream stream = new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            return Task.FromResult<Stream?>(stream);
        }
    }
}
