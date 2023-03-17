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
        public async Task<IActionResult> Upload(IEnumerable<IFormFile> files /*same name with input name*/)
        {
            foreach(var item in files)
            {
                var UploadsRootFolder = Path.Combine(_env.WebRootPath, "GalleryFiles");
                if(!Directory.Exists( UploadsRootFolder ) )
                {
                    Directory.CreateDirectory(UploadsRootFolder);
                }
                if(item != null)
                {
                    string FileExtension = Path.GetExtension(item.FileName);
                    string NewFileName = string.Concat(Guid.NewGuid(), FileExtension);
                    string path = Path.Combine(UploadsRootFolder, NewFileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }
                }
            }
            ViewBag.Alert = "عملیات آپلود با موفقیت انجام شد";
            return View();
        }
    }
}
