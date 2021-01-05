using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Demo;
using IdentityServer.Models;
using IdentityServer4;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    public class AuthenticationController : Controller
    {

        private readonly TestUserStore _users;

        public AuthenticationController(TestUserStore users = null)
        {
            _users = users ?? new TestUserStore(TestUsers.Users);
        }

        [Route("login", Name = "UrlLogin")]
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
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
                if (_users.ValidateCredentials(model.Email, model.Password)) 
                {
                    var user = _users.FindByUsername(model.Email);

                    var isuser = new IdentityServerUser(user.SubjectId)
                    {
                        DisplayName = user.Username
                    };

                    await HttpContext.SignInAsync(isuser);

                    return Redirect(model.ReturnUrl);
                }
            }
            ModelState.AddModelError("Email", "Login / mot de passe invalide");
            return View();
        }
    }
}
