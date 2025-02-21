using Microsoft.AspNetCore.Mvc;
using System.IO;
using VodPlatform.Services.Interface;
using VodPlatform.Models;
using Microsoft.Extensions.Logging;

namespace VodPlatform.Services
{
    public class VideoServices : IVideoStream
    {
        private readonly ILogger<VideoServices> _logger;
        private readonly string Server = @"Z:\";

        public VideoServices(ILogger<VideoServices> logger)
        {
            _logger = logger;
        }

        public async Task<FileStreamResult> SendData(string movieName, string file)
        {
            if (string.IsNullOrEmpty(movieName) || string.IsNullOrEmpty(file))
            {
                _logger.LogWarning("Invalid parameters: movieName or file is null or empty.");
                throw new ArgumentException("Invalid movie name or file.");
            }

            var filePath = Path.Combine(Server, movieName, file);

            if (!File.Exists(filePath))
            {
                _logger.LogWarning("File {FilePath} not found.", filePath);
                throw new FileNotFoundException($"File {file} not found.");
            }

            var fileExtension = Path.GetExtension(file).ToLower();
            var mimeTypes = new Dictionary<string, string>
            {
                { ".m4s", "video/mp4" },
                { ".mpd", "application/dash+xml" },
                { ".mp4", "video/mp4" },
                { ".avi", "video/x-msvideo" }
            };

            var mimeType = mimeTypes.GetValueOrDefault(fileExtension, "application/octet-stream");

            try
            {
                _logger.LogInformation("Attempting to stream file: {FilePath}. MIME Type: {MimeType}", filePath, mimeType);
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return new FileStreamResult(fileStream, mimeType);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogError("Unauthorized access to file {FilePath}.", filePath);
                throw new UnauthorizedAccessException($"Access to {filePath} is denied.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while trying to stream the file: {FilePath}.", filePath);
                throw;
            }
        }

        public async Task<List<string>> ListMovies()
        {
            if (!Directory.Exists(Server))
            {
                _logger.LogWarning("Server directory {Server} does not exist.", Server);
                return new List<string>();
            }

            try
            {
                _logger.LogInformation("Listing movies from server directory: {Server}", Server);
                var movieFolders = Directory.GetDirectories(Server)
                                             .Select(d => Path.GetFileName(d))
                                             .ToList();
                _logger.LogInformation("Found {MovieCount} movie folders.", movieFolders.Count);
                return movieFolders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while listing movie folders from server directory: {Server}", Server);
                throw;
            }
        }
    }
}
