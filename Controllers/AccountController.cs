﻿using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager ,ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl)
        {
            //LoginViewModel model = new LoginViewModel
            //{
            //    ReturnUrl = returnUrl,
            //    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            //};

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Regsitering the user
                var user = new ApplicationUser 
                { 
                    UserName = model.Email, 
                    Email = model.Email , 
                    City = model.City , 
                    Gender= model.Gender 
                };
                var result = await _userManager.CreateAsync(user , model.Password);

                //Login The User
                if(result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmLink = Url.Action("ConfirmEmail", "Account", 
                        new { userId = user.Id, token = token }, Request.Scheme);

                  _logger.LogInformation(confirmLink);

                    if(_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Admin");
                    }
                    //await _signInManager.SignInAsync(user, isPersistent: false); // Session Cookie instead of a permenaent cookie
                    //return RedirectToAction("Index" , "home");
                    //ViewBag.ErrorTitle = "Registration successful";
                    //ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                    //        "email, by clicking on the confirmation link we have emailed you";
                    return RedirectToAction("RegisterComplete", "Account");

                }
                foreach (var error in result.Errors) 
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
            }
            return View(model);
        }

        public IActionResult RegisterComplete()
        {
            ViewBag.ErrorTitle = "Registration successful";
            ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                    "email, by clicking on the confirmation link we have emailed you";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model , string? returnUrl)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user != null && !user.EmailConfirmed && await _userManager.CheckPasswordAsync(user,model.Password))
                {
                    ModelState.AddModelError(String.Empty, "Email is Not Confirmed");
                    return View(model);
                } 

                //Login the User
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

                if(result.Succeeded)
                {
                    if(!string.IsNullOrEmpty(returnUrl)) 
                    {
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }
                if(result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                    ModelState.AddModelError(String.Empty, "Invalid Login Credentials");
            }
            return View(model);
        }

        [HttpPost]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
          var user  = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use");    
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("NotFound");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }



        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(model.Email);
                // If the user is found AND Email is confirmed
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Generate the reset password token
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // Build the password reset link
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    // Log the password reset link
                    _logger.LogInformation(passwordResetLink);

                    // Send the user to Forgot Password Confirmation view
                    return View("ForgotPasswordConfirmation");
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist or is not confirmed
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            // If password reset token or email is null, most likely the
            // user tried to tamper the password reset link
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    // reset the user password
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    // Display validation errors. For example, password reset token already
                    // used to change the password or password complexity rules not met
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                // To avoid account enumeration and brute force attacks, don't
                // reveal that the user does not exist
                return View("ResetPasswordConfirmation");
            }
            // Display validation errors if model state is not valid
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                // ChangePasswordAsync changes the user password
                var result = await _userManager.ChangePasswordAsync(user,
                    model.CurrentPassword, model.NewPassword);

                // The new password did not meet the complexity rules or
                // the current password is incorrect. Add these errors to
                // the ModelState and rerender ChangePassword view
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                // Upon successfully changing the password refresh sign-in cookie
                await _signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }



        //public IActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
        //                        new { ReturnUrl = returnUrl });
        //    var properties = _signInManager
        //        .ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        //    return new ChallengeResult(provider, properties);
        //}

        //[AllowAnonymous]
        //public async Task<IActionResult>
        //    ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        //{
        //    returnUrl = returnUrl ?? Url.Content("~/");

        //    LoginViewModel loginViewModel = new LoginViewModel
        //    {
        //        ReturnUrl = returnUrl,
        //        ExternalLogins =
        //                (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
        //    };

        //    if (remoteError != null)
        //    {
        //        ModelState
        //            .AddModelError(string.Empty, $"Error from external provider: {remoteError}");

        //        return View("Login", loginViewModel);
        //    }

        //    // Get the login information about the user from the external login provider
        //    var info = await _signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //    {
        //        ModelState
        //            .AddModelError(string.Empty, "Error loading external login information.");

        //        return View("Login", loginViewModel);
        //    }

        //    // If the user already has a login (i.e if there is a record in AspNetUserLogins
        //    // table) then sign-in the user with this external login provider
        //    var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
        //        info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        //    if (signInResult.Succeeded)
        //    {
        //        return LocalRedirect(returnUrl);
        //    }
        //    // If there is no record in AspNetUserLogins table, the user may not have
        //    // a local account
        //    else
        //    {
        //        // Get the email claim value
        //        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        //        if (email != null)
        //        {
        //            // Create a new user without password if we do not have a user already
        //            var user = await _userManager.FindByEmailAsync(email);

        //            if (user == null)
        //            {
        //                user = new ApplicationUser
        //                {
        //                    UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
        //                    Email = info.Principal.FindFirstValue(ClaimTypes.Email)
        //                };

        //                await _userManager.CreateAsync(user);
        //            }

        //            // Add a login (i.e insert a row for the user in AspNetUserLogins table)
        //            await _userManager.AddLoginAsync(user, info);
        //            await _signInManager.SignInAsync(user, isPersistent: false);

        //            return LocalRedirect(returnUrl);
        //        }

        //        // If we cannot find the user email we cannot continue
        //        ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
        //        ViewBag.ErrorMessage = "Please contact support on Pragim@PragimTech.com";

        //        return View("Error");
        //    }
        //}

    }
}
