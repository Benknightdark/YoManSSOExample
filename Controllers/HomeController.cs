using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OAthLib.Services;
using Web.Models;

namespace Web.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private LineService _lineService { get; set; }
        private FBService _fBService { get; set; }
        private GoogleService _googleService { get; set; }
        private LinkedInService _linkedService { get; set; }

        public HomeController (ILogger<HomeController> logger, LineService lineService, FBService fBService, GoogleService googleService
        ,LinkedInService linkedService
        ) {

            _logger = logger;
            _lineService = lineService;
            _fBService = fBService;
            _googleService = googleService;
            _linkedService=linkedService;
        }

        public async Task<IActionResult> Index () {
            ViewBag.AccessUrl = await _lineService.GetAccessUrl ();
            ViewBag.FBAccessUrl = await _fBService.GetAccessUrl ();
            ViewBag.GoogelAccessUrl = await _googleService.GetAccessUrl ();
            ViewBag.LinkedAccessUrl=await _linkedService.GetAccessUrl();

            return View ();
        }

        public async Task<IActionResult> Privacy () {
            return View ();
        }
        public async Task<IActionResult> FBCallBack () {
            var Code = Request.Query["Code"];
            _logger.LogInformation (Code);

            _logger.LogInformation ("====================================");
            var Data = await _fBService.GetAccessToken (Code);
            _logger.LogInformation (Data.access_token);
            _logger.LogInformation ("====================================");
            _logger.LogInformation ((await _fBService.GetUserProfile (Data)).name);

            return RedirectToAction ("Index");
        }
        public async Task<IActionResult> LinkedInCallBack () {
            var Code = Request.Query["Code"];
            _logger.LogInformation (Code);

            //  _logger.LogInformation("====================================");
             var Data = await _linkedService.GetAccessToken(Code); 
            _logger.LogInformation(Data.access_token);
            //             _logger.LogInformation(Data.id_token);
            var profile=await _linkedService.GetUserProfile(Data);

            //  _logger.LogInformation("====================================");
             _logger.LogInformation(profile.id);
             _logger.LogInformation(profile.localizedFirstName);
             _logger.LogInformation(profile.localizedLastName);

            return RedirectToAction ("Index");
        }
        public async Task<IActionResult> GoogleCallBack () {
            var Code = Request.Query["Code"];
            _logger.LogInformation (Code);

            _logger.LogInformation ("====================================");
            var Data = await _googleService.GetAccessToken (Code);
            // _logger.LogInformation(Data.access_token);
            _logger.LogInformation (Data.id_token);

            _logger.LogInformation ("====================================");
            _logger.LogInformation ((await _googleService.GetUserProfile (Data)).id);

            return RedirectToAction ("Index");
        }
        public async Task<IActionResult> LineCallBack () {
            var dd = Request.Query["code"];

            if (!string.IsNullOrEmpty (dd)) {
                var token = await _lineService.GetAccessToken (dd);
                _logger.LogInformation (JsonConvert.SerializeObject (token));
                _logger.LogInformation ("====================================");
                _logger.LogInformation (await _lineService.GetEmail (token));
                _logger.LogInformation ("====================================");

                var profile = await _lineService.GetProfile (token);
                _logger.LogInformation (JsonConvert.SerializeObject (profile));

                _logger.LogInformation ("====================================");
            }
            return RedirectToAction ("Index");
        }

        [ResponseCache (Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error () {
            return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}