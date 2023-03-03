using BookShop.Areas.Identity.Data;
using BookShop.Areas.Identity.Services;
using BookShop.Classes;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BookShop.Controllers
{
    //Ignore this controller to avoid interference with swagger
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountController : Controller
    {
        private readonly IApplicationRoleManager _roleManager;
        private readonly IApplicationUserManager _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _singInManager;
        private readonly ISmsSender _smsSender;
        private readonly IConvertDate _convertDate;

        public AccountController(IApplicationRoleManager roleManager, IApplicationUserManager userManager,IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, ISmsSender smsSender, IConvertDate convertDate)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _singInManager = signInManager;
            _smsSender = smsSender;
            _convertDate = convertDate;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                DateTime BirthDateMildai = _convertDate.ConvertShamsiToMiladi(ViewModel.BirthDate);
                var user = new ApplicationUser {UserName=ViewModel.UserName, Email=ViewModel.Email, PhoneNumber=ViewModel.PhoneNumber, RegisterDate=DateTime.Now, IsActive=true,BirthDate = BirthDateMildai };
                IdentityResult result = await _userManager.CreateAsync(user,ViewModel.Password);

                if(result.Succeeded)
                {
                    var role = _roleManager.FindByIdAsync("کاربر");
                    if (role != null)
                        await _roleManager.CreateAsync(new ApplicationRole("کاربر"));

                    result = await _userManager.AddToRoleAsync(user,"کاربر");
                    await _userManager.AddClaimAsync(user,new Claim(ClaimTypes.DateOfBirth,BirthDateMildai.ToShortDateString()/*ToString("MM/dd")*/));
                    if (result.Succeeded)
                    {
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", values: new { userId = user.Id, code = code }, protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(ViewModel.Email, "تایید ایمیل حساب کاربری سایت بوکشاپ", $"<div dir='rtl' style='font-family:tahoma;font-size:14px;'>لطفا با کلیک بر روی لینک روبرو ایمیل خود را تایید کنید <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>کلیک کنید</a></div>");

                        return RedirectToAction("Index","Home", new {id = "ConfirmEmail" });
                    }
                }
                foreach(var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty/*""*/,item.Description);
                }
            }
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return RedirectToAction("Index","Home");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound($"Unabe To load user with ID '{userId}'");
            //If code is true, Confirmation column setting to true
            var result = await _userManager.ConfirmEmailAsync(user,code);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Error Confirming Email for user with ID '{userId}'");

            return View();
        }

        [HttpGet]
        public IActionResult SignIn(string ReturnUrl=null)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SingInViewModel ViewModel)
        {
            if (Captcha.ValidateCaptchaCode(ViewModel.CaptchaCode, HttpContext))
            {
                if (ModelState.IsValid)
                {
                    var User = await _userManager.FindByNameAsync(ViewModel.UserName);
                    if (User != null)
                    {
                        if (User.IsActive)
                        {
                            var result = await _singInManager.PasswordSignInAsync(ViewModel.UserName, ViewModel.Password, ViewModel.RememberMe, true);
                            if (result.Succeeded)
                            {
                                User.LastVisitDateTime = DateTime.Now;
                                await _userManager.UpdateAsync(User);
                                return RedirectToAction("Index", "Home");
                            }
                            if (result.IsLockedOut)
                            {
                                ModelState.AddModelError(string.Empty, "حساب کاربری شما به مدت 20 دقیقه به دلیل تلاش های ناموفق قفل شده است");
                                return View();
                            }
                            if (result.RequiresTwoFactor)
                                return RedirectToAction("SendCode", new {RememberMe = ViewModel.RememberMe });
                        }
                    }
                    ModelState.AddModelError(string.Empty, "نام کاربری یا کلمه عبور شما صحیح نمی باشد");
                }
            }
            else
            {
                ModelState.AddModelError("", "کد امنیتی صحیح نمی باشد");
            }
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut()
        {
            await _singInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Route("get-captcha-image")]//when you enter this address, the captcha is creating for you
        public IActionResult GetCaptchaImage()
        {
            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel ViewModel)
        {
            if(ModelState.IsValid)
            {
                var User = await _userManager.FindByEmailAsync(ViewModel.Email);
                if (User == null)
                    ModelState.AddModelError(string.Empty, "ایمیل شما صحیح نمی باشد");
                else
                {
                    if (!await _userManager.IsEmailConfirmedAsync(User))
                        ModelState.AddModelError(string.Empty, "لطفا با تایید ایمیل حساب کاربری خود را فعال نمایید");
                    else
                    {
                        var Code = await _userManager.GeneratePasswordResetTokenAsync(User);
                        var CallBackUrl =Url.Action("ResetPassword","Account",values:new {Code },protocol:Request.Scheme);
                        await _emailSender.SendEmailAsync(ViewModel.Email,"بازنشانی کلمه عبور", $"<p style='font-family:tahoma;font-size:14px'>برای بازنشانی کلمه عبور <a href='{HtmlEncoder.Default.Encode(CallBackUrl)}'>اینجا کلیک کنید</a></p>");

                        return RedirectToAction("ForgetPasswordConfirmation");
                    }
                }
            }
            return View(ViewModel);
        }

        [HttpGet]
        public IActionResult ForgetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string Code = null)
        {
            if (Code == null)
                return NotFound();
            else
            {
                var ViewModel = new ResetPasswordViewModel { Code = Code };
                return View(ViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByEmailAsync(ViewModel.Email);
                if (User == null)
                    ModelState.AddModelError(string.Empty, "ایمیل شما صحیح نمی باشد.");
                else
                {
                    var Result = await _userManager.ResetPasswordAsync(User, ViewModel.Code, ViewModel.Password);
                    if (Result.Succeeded)
                        return RedirectToAction("ResetPasswordConfirmation");
                    else
                    {
                        foreach (var error in Result.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(ViewModel);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SendSms()
        {
            string[] phone = { "09189141427" };
            string Status = await _smsSender.SendAuthSmsAsync("5647",phone);
            if (Status == "Success")
                ViewBag.Alert = "ارسال پیامک با موفقیت انجام شد";
            else
                ViewBag.Alert = "در ارسال پیامک خطایی رخ داد";

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SendCode(bool RememberMe)
        {
            var FactorOptions = new List<SelectListItem>();
            var User = await _singInManager.GetTwoFactorAuthenticationUserAsync();

            //Generating New Key And Persist it for GetValidTwoFactorProvidersAsync(). It gets Email, Phone, Authenticator
            var key = await _userManager.GetAuthenticatorKeyAsync(User); // get the key
            if (string.IsNullOrEmpty(key))
            {
                // if no key exists, generate one and persist it
                await _userManager.ResetAuthenticatorKeyAsync(User);
                // get the key we just created
                key = await _userManager.GetAuthenticatorKeyAsync(User);
            }
            //End

            if (User == null)
                return NotFound();

            var UserFactors = await _userManager.GetValidTwoFactorProvidersAsync(User);
            foreach (var item in UserFactors)
            {
                if(item == "Authenticator")
                {
                    FactorOptions.Add(new SelectListItem { Text="اپلیکیشن احراز هویت",Value=item});
                }
                else
                {
                    FactorOptions.Add( new SelectListItem { Text = (item == "Email" ? "ارسال ایمیل" : "ارسال پیامک"), Value = item });
                }
            }
            return View(new SendCodeViewModel { Providers = FactorOptions, RememberMe = RememberMe});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel ViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(ViewModel);
            }
            if (ViewModel.SelectedProvider != "Authenticator")
            {
                var User = await _singInManager.GetTwoFactorAuthenticationUserAsync();
                if (User == null)
                    return NotFound();

                var Code = await _userManager.GenerateTwoFactorTokenAsync(User, ViewModel.SelectedProvider);
                if (string.IsNullOrWhiteSpace(Code))
                    return NotFound("Error");

                var Message = "<p style='direction:rtl; font-size:14px;font-family:tahoma'>کد اعتبار سنجی شما:  " + Code + "</p>";

                if (ViewModel.SelectedProvider == "Email")
                    await _emailSender.SendEmailAsync(User.Email, "کد اعتبار سنجی", Message);
                else if (ViewModel.SelectedProvider == "Phone")
                {
                    string ResponseSms = await _smsSender.SendAuthSmsAsync(Code, new string[] { User.PhoneNumber });
                    if (ResponseSms == "Failed")
                    {
                        ModelState.AddModelError(string.Empty, "در ارسال پیامک خطایی رخ داده است");
                        return View(ViewModel);
                    }
                }

                return RedirectToAction("VerifyCode", new { Provider = ViewModel.SelectedProvider, RememberMe = ViewModel.RememberMe });
            }
            else
                return RedirectToAction("LoginWith2fa", new { RememberMe=ViewModel.RememberMe});
        }

        [HttpGet]
        public async Task<IActionResult> LoginWith2fa(bool RememberMe)
        {
            var user = _singInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                return NotFound();

            return View(new LoginWith2faViewModel {RememberMe = RememberMe });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWIth2fa(LoginWith2faViewModel ViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(ViewModel);
            }
            else
            {
                var user = await _singInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                    return NotFound();
                var authenticationCode = ViewModel.TwoFactorCode.Replace(" ",string.Empty).Replace("-",string.Empty);
                var Result = await _singInManager.TwoFactorAuthenticatorSignInAsync(authenticationCode, ViewModel.RememberMe, ViewModel.RememberMachine);
                if (Result.Succeeded)
                    return RedirectToAction("Index", "Home");
                else if (Result.IsLockedOut)
                    ModelState.AddModelError(string.Empty, "حساب کاربری شما به مدت 20 دقیقه به دلیل تلاش ناموفق قفل شد");
                else
                    ModelState.AddModelError(string.Empty, "کد اعتبار سنجی شما نامعتبر است");

                return View(ViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> VerifyCode(string Provider, bool RememberMe)
        {
            var User = await _singInManager.GetTwoFactorAuthenticationUserAsync();
            if (User == null)
                return NotFound();

            return View(new VerifyCodeViewModel { Provider = Provider, RememberMe = RememberMe});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel ViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(ViewModel);
            }
            var Result = await _singInManager.TwoFactorSignInAsync(ViewModel.Provider, ViewModel.Code,ViewModel.RememberMe,ViewModel.RememberBrowser);
            if(Result.Succeeded)
            {
                return RedirectToAction("Index","Home");
            }
            else if (Result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty,"حساب کاربری شما به دلیل تلاش های ناموفق به مدت 20 دقیقه قفل شد");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "کد اعتبار سنجی صحیح نمی باشد");
            }
            return View(ViewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            UserSidebarViewModel Sidebar = new UserSidebarViewModel()
            {
                FullName = user.FirstName + " " + user.LastName,
                LastVisit = user.LastVisitDateTime,
                RegisterDate = user.RegisterDate,
                Image = user.Image
            };
            return View(new ChangePasswordViewModel { UserSidebar = Sidebar});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel ViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();
            if (ModelState.IsValid)
            {
                var ChangePassResult = await _userManager.ChangePasswordAsync(user,ViewModel.OldPassword,ViewModel.NewPassword);
                if (ChangePassResult.Succeeded)
                    ViewBag.Alert = "کلمه عبور شما با موفقیت تغییر یافت";

                else
                {
                    foreach (var item in ChangePassResult.Errors)
                        ModelState.AddModelError(string.Empty, item.Description);
                }
            }
            UserSidebarViewModel Sidebar = new UserSidebarViewModel()
            {
                FullName = user.FirstName + " " + user.LastName,
                LastVisit = user.LastVisitDateTime,
                RegisterDate = user.RegisterDate,
                Image = user.Image
            };
            ViewModel.UserSidebar = Sidebar;
            return View(ViewModel);
        }

        [Route("Get/All/Trees")]
        public IActionResult GoToRedirect()
        {
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public async Task<IActionResult> LoginWithRecoveryCode()
        {
            var user = await _singInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                return NotFound();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel ViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(ViewModel);
            }

            var user = await _singInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                return NotFound();

            var RecoveryCode = ViewModel.RecoveryCode.Replace(" ", string.Empty);
            var Result = await _singInManager.TwoFactorRecoveryCodeSignInAsync(RecoveryCode);
            if (Result.Succeeded)
                return RedirectToAction("Index", "Home");

            else if (Result.IsLockedOut)
                ModelState.AddModelError(string.Empty, "حساب کاربری شما به مدت 20 دقیقه به دلیل تلاش های ناموفق قفل شد.");

            else
                ModelState.AddModelError(string.Empty, "کد بازیابی شما نامعتبر است.");

            return View(ViewModel);
        }
    
        public IActionResult AccessDenied(string ReturnUrl=null)
        {
            return View();
        }    

        [Authorize(Policy ="HappyBirthDay")]
        public IActionResult HappyBirthDay()
        {
            return View();
        }
    }
}
