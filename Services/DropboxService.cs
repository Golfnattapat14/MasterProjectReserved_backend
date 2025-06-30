using Azure.Core;
using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ResDb;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MasterWord.Services
{
    public class DropboxService
    {
        private readonly string _accessToken;

        public DropboxService(IConfiguration configuration)
        {
            _accessToken = configuration["Dropbox:AccessToken"];
        }

        public async Task<FileUploadResult> UploadFileAsync(IFormFile file, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_accessToken))
                return new FileUploadResult { Success = false, ErrorMessage = "Dropbox access token is not configured." };

            using var dbx = new DropboxClient(_accessToken);
            var fileName = Path.GetFileName(file.FileName);
            var dropboxPath = "/" + fileName;

            using var stream = file.OpenReadStream();
            try
            {
                var uploadResult = await dbx.Files.UploadAsync(
                    dropboxPath,
                    WriteMode.Overwrite.Instance,
                    body: stream
                );

                var sharedLink = await dbx.Sharing.CreateSharedLinkWithSettingsAsync(dropboxPath);
                var updatedDate = DateTime.Now;
                return new FileUploadResult
                {
                    Success = true,
                    FileName = fileName,
                    FileUrl = GetDownloadUrl(sharedLink.Url)
                };
            }
            catch (System.Exception ex)
            {
                return new FileUploadResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<DropboxDeleteResult> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken)
        {
            try
            {
                var dropboxPath = ExtractDropboxPath(fileUrl);
                using var dbx = new DropboxClient(_accessToken);
                await dbx.Files.DeleteV2Async(dropboxPath);
                var updatedDate = DateTime.Now;

                return new DropboxDeleteResult { Success = true };
            }
            catch (System.Exception ex)
            {
                return new DropboxDeleteResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        private string ExtractDropboxPath(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                throw new System.ArgumentException("File URL is null or empty.", nameof(fileUrl));

            if (fileUrl.StartsWith("/"))
                return fileUrl;

            try
            {
                var uri = new System.Uri(fileUrl);
                var segments = uri.Segments;
                if (segments.Length > 0)
                {
                    var fileName = segments[segments.Length - 1];
                    var cleanFileName = fileName.Split('?')[0];
                    if (!string.IsNullOrWhiteSpace(cleanFileName))
                        return "/" + cleanFileName;
                }
            }
            catch
            {
                var lastSlash = fileUrl.LastIndexOf('/');
                if (lastSlash >= 0 && lastSlash < fileUrl.Length - 1)
                {
                    var fileName = fileUrl.Substring(lastSlash + 1);
                    var cleanFileName = fileName.Split('?')[0];
                    if (!string.IsNullOrWhiteSpace(cleanFileName))
                        return "/" + cleanFileName;
                }
            }

            throw new System.ArgumentException("Could not extract Dropbox path from URL.", nameof(fileUrl));
        }

        public string GetDownloadUrl(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return string.Empty;
            int idx = fileUrl.LastIndexOf("=0");
            if (idx >= 0 && idx == fileUrl.Length - 2)
            {
                return fileUrl.Substring(0, idx) + "=1";
            }
            return fileUrl;
        }
    }
}