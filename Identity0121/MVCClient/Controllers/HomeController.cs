﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCClient.Models;

namespace MVCClient.Controllers
{
    [Authorize(AuthenticationSchemes = "OpenIdConnect")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        
        public async Task<IActionResult> Index()
        {
            var u = User;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");

            var role = User.FindAll(x => x.Type == "role");
            
            var test = User.FindFirst(x => x.Type == "preferred_username")?.Value;

            return View();
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Privacy()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            HttpClient client = new HttpClient();
            client.SetBearerToken(accessToken);
            client.BaseAddress = new Uri("https://localhost:5501/");
            var result = await client.GetAsync("weatherforecast");
            var code = result.StatusCode;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
