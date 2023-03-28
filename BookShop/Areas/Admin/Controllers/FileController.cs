using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FileController : Controller
    {
        private readonly IHostingEnvironment _env;
        public FileController(IHostingEnvironment env)
        {
            _env = env;
        }
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload(IEnumerable<IFormFile> files /*same name with input name*/)
        {
            try
            {
                foreach (var item in files)
                {
                    var UploadsRootFolder = Path.Combine(_env.WebRootPath, "GalleryFiles");
                    if (!Directory.Exists(UploadsRootFolder))
                    {
                        Directory.CreateDirectory(UploadsRootFolder);
                    }
                    if (item != null)
                    {
                        string FileExtension = Path.GetExtension(item.FileName);
                        string NewFileName = string.Concat(Guid.NewGuid(), FileExtension);
                        string path = Path.Combine(UploadsRootFolder, NewFileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await item.CopyToAsync(stream);
                        }
                        CompressImage(NewFileName);
                    }
                }
                //ViewBag.Alert = "عملیات آپلود با موفقیت انجام شد";
                //return View();
                return new JsonResult("success");
            }
            catch
            {
                return new EmptyResult();
            }
            
        }

        public IActionResult ImageProcess()
        {
            var PathFolder = $"{_env.WebRootPath}/images/";
            using (var Image = new MagickImage(PathFolder + "avatar-1.png"))
            {
                //Image.Resize(300,300);
                Image.Quality = 30;
                Image.Write(PathFolder + "output-image-quality.png");
                CompressImage(PathFolder + "output-image-quality.png");
            }
            return View();
        }

        public void CompressImage(string path)
        {
            var Image = new FileInfo(path);
            var Optimizer = new ImageOptimizer();
            Optimizer.Compress(Image);
            Image.Refresh();
        }

        public IActionResult SaveImage2Pdf()
        {
            var FolderPath = $"{_env.WebRootPath}/images/";
            using (var Image = new MagickImage(FolderPath + "logo-header.png"))
            {
                Image.Write(FolderPath + "PdfImage.pdf");
            }
            var FileStream = new FileStream(FolderPath + "PdfImage.pdf", FileMode.Open, FileAccess.Read);
            return new FileStreamResult(FileStream, "application/pdf");
        }
    }
}
