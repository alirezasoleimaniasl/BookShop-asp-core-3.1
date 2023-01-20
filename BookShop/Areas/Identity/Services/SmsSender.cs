using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Ghasedak;

namespace BookShop.Areas.Identity.Services
{
    public class SmsSender : ISmsSender
    {
        public async Task<string> SendAuthSmsAsync(string Code,string[] PhoneNumber)
        {
            var sms = new Ghasedak.Core.Api("d88e1354970fb0a6031d01b93e045f63bd015346e4522876a33e5a18ce0d3f62");
            var result = await sms.VerifyAsync(1, "AutoVerify", PhoneNumber, Code);
            //var result = await sms.SendSMSAsync(Code, PhoneNumber);
            if (result.Result.Code == 200)
                return "Success";
            else
                return "Failed";
        }
    }
}
