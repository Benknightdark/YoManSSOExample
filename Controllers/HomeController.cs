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
        private MSService _msService;
        public HomeController (ILogger<HomeController> logger, LineService lineService, FBService fBService, GoogleService googleService, LinkedInService linkedService, MSService msService) {

            _logger = logger;
            _lineService = lineService;
            _fBService = fBService;
            _googleService = googleService;
            _linkedService = linkedService;
            _msService = msService;
        }

        public async Task<IActionResult> Index () {
            // Line OAuth2 登入網址
            ViewBag.AccessUrl = await _lineService.GetAccessUrl ();
            // Facebook OAuth2 登入網址
            ViewBag.FBAccessUrl = await _fBService.GetAccessUrl ();
            // Google OAuth2 登入網址
            ViewBag.GoogelAccessUrl = await _googleService.GetAccessUrl ();
            // LinkedIn OAuth2 登入網址
            ViewBag.LinkedAccessUrl = await _linkedService.GetAccessUrl ();
            // Microsoft OAuth2 登入網址
            ViewBag.MSAccessUrl = await _msService.GetAccessUrl ();

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
            _logger.LogInformation ((await _fBService.GetProfile (Data)).name);
                            _logger.LogInformation ("====================================");
            _logger.LogInformation ((await _fBService.GetAppicationInfo (Data)).name);

            return RedirectToAction ("Index");
        }
        public async Task<IActionResult> LinkedInCallBack () {
            var Code = Request.Query["Code"];
            _logger.LogInformation (Code);

            //  _logger.LogInformation("====================================");
            var Data = await _linkedService.GetAccessToken (Code);
            _logger.LogInformation (Data.access_token);
            //             _logger.LogInformation(Data.id_token);
            var profile = await _linkedService.GetProfile (Data);

            //  _logger.LogInformation("====================================");
            _logger.LogInformation (profile.id);
            _logger.LogInformation (profile.localizedFirstName);
            _logger.LogInformation (profile.localizedLastName);

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
            _logger.LogInformation ((await _googleService.GetProfile (Data)).id);

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
        public async Task<IActionResult> MSCallBack () {
            var dd = Request.Query["code"];
            _logger.LogInformation (dd);
            if (!string.IsNullOrEmpty (dd)) {
                var token = await _msService.GetAccessToken (dd);

                 _logger.LogInformation ((token.access_token));
                // _logger.LogInformation ("====================================");
                // _logger.LogInformation (await _lineService.GetEmail (token));
                // _logger.LogInformation ("====================================");

             var profile = await _msService.GetProfile (token);
                _logger.LogInformation ( (profile.displayName));
                _logger.LogInformation ( (profile.id));

                // _logger.LogInformation ("====================================");
            }
            return RedirectToAction ("Index");
        }

        [ResponseCache (Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error () {
            return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}