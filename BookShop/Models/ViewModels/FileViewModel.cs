using BookShop.Attributes;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace BookShop.Models.ViewModels
{
    public class UploadLargeFile
    {
        [UploadFileSizeAttribute(1900000)]
        public IFormFile File { get; set; }
    }

    public class UploadFileResult
    {
        public UploadFileResult()
        {

        }
        public UploadFileResult(bool _IsSuccess, List<string> _Errors)
        {
            IsSuccess = _IsSuccess;
            Errors= _Errors;
        }
        public bool? IsSuccess { get; set; }
        public List<string> Errors { get; set; }
    }
}
