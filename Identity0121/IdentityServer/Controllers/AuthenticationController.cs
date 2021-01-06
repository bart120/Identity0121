using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.AspIdentity;
using IdentityServer.Demo;
using IdentityServer.Models;
using IdentityServer4;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    public class AuthenticationController : Controller
    {

        //private readonly TestUserStore _users;
        private readonly UserManager<User> _userMgr;
        private readonly SignInManager<User> _signInMgr;

        public AuthenticationController(/*TestUserStore users = null*/UserManager<User> userMgr, SignInManager<User> signInMgr)
        {
            //_users = users ?? new TestUserStore(TestUsers.Users);
            _signInMgr = signInMgr;
            _userMgr = userMgr;
        }

        [Route("login", Name = "UrlLogin")]
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            //AD authenticated
            /*if (this.User.Identity.IsAuthenticated)
            {
                return Redirect(returnUrl);
            }*/


            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }



        [Route("login")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInMgr.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    //var user = await _userMgr.FindByNameAsync(model.Email);
                    return Redirect(model.ReturnUrl);
                }
                
                
                /*if (_users.ValidateCredentials(model.Email, model.Password)) 
                {
                    var user = _users.FindByUsername(model.Email);

                    var isuser = new IdentityServerUser(user.SubjectId)
                    {
                        DisplayName = user.Username
                    };

                    await HttpContext.SignInAsync(isuser);

                    return Redirect(model.ReturnUrl);
                }*/


            }
            ModelState.AddModelError("Email", "Login / mot de passe invalide");
            return View();

            //with AD
            var adContext = new PrincipalContext(ContextType.Domain);
            if(adContext.ValidateCredentials(model.Email, model.Password))
            {

            }
        }

        [Route("exter", Name ="exter")]
        [HttpGet]
        public IActionResult Exter(string returnUrl)
        {
            var callback = Url.Action("SigninExter");
            var props = new AuthenticationProperties
            {
                RedirectUri = "https://localhost:5001/Authentication/SigninExter",
                Items =
                {
                    {"scheme" ,  IdentityServerConstants.ExternalCookieAuthenticationScheme},
                    {"returnUrl", returnUrl }
                }
            };

            return Challenge(props, "microsoftaccount");

        }

        //[Route("signin-microsoft", Name = "SigninExter")]
        public async Task<IActionResult> SigninExter(string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync("microsoftaccount");
            
            return BadRequest();
        }
    }
}
