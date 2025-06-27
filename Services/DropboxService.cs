using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using ResDb;

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

                return new FileUploadResult
                {
                    Success = true,
                    FileName = fileName,
                    FileUrl = sharedLink.Url
                };
            }
            catch (System.Exception ex)
            {
                return new FileUploadResult { Success = false, ErrorMessage = ex.Message };
            }
        }
    }
}

   