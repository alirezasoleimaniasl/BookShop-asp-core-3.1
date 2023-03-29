using BookShop.Attributes;
using Microsoft.AspNetCore.Http;

namespace BookShop.Models.ViewModels
{
    public class UploadLargeFile
    {
        [UploadFileSizeAttribute(1900000)]
        public IFormFile File { get; set; }
    }
}
