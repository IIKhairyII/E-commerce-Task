using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Net;

namespace e_commerce.Controllers
{
    [ApiController]
    [Route("api/fileManager")]
    public class FilesManagerController : ControllerBase
    {
        public FilesManagerController(
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment, 
            IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        public async Task<bool> FileManager(IFormFile image, string folder, string imagePath)
        {
            try
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpGet("download")]
        public FileResult download([FromQuery] string imageName)
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            rootFolder = Path.Combine(rootFolder, _configuration.GetRequiredSection("ImagesFolder").Value);
            var fileInfo = new FileInfo(Path.Combine(rootFolder, imageName));
            if (!fileInfo.Exists)
                return null;
         
            string mimeType = MimeMapping.MimeUtility.GetMimeMapping(fileInfo.Name);
            return File(fileInfo.OpenRead(), mimeType);
        }
    }
}
