using Google.Type;
using google_ads_api.services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Google.Ads.GoogleAds.V18.Enums.ConsentStatusEnum.Types;

namespace google_ads_api.ui.Controllers
{
    public class GoogleAdsController : Controller
    {
        private readonly IGoogleAdsService _googleAdsService;
        public GoogleAdsController(IGoogleAdsService googleAdsService)
        {
            _googleAdsService = googleAdsService;
        }
        public IActionResult Index(services.Models.GoogleTokenDto obj)
        {
            return View(obj);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleAuth()
        {
            return Json(_googleAdsService.GetAuthCode());
        }

        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            string code = HttpContext.Request.Query["code"];
            string scope = HttpContext.Request.Query["scope"];

            //get token method
            services.Models.GoogleTokenDto token = await _googleAdsService.GetTokens(code);
            //return Ok(token);
            return View("Index", token);
        }


        [HttpPost]
        public async Task<IActionResult> ListAvaiableCustomers(string refreshToken)
        {
            List<string> result = await _googleAdsService.ListAvaiableCustomers(refreshToken);
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> UploadOfflineConversion(string refreshToken, string conversionValue,   long conversionActionId, string gclId, string gbraId, string wbraId)
        {
            ConsentStatus consentStatus = ConsentStatus.Unknown;
            
            string conversionTime = string.Format("{0:yyyy-MM-dd hh:mm:ss}{1}",System.DateTime.UtcNow, "+02:00");
      
  
            await _googleAdsService.UploadOfflineConversion(refreshToken, Convert.ToDouble(conversionValue), conversionTime, conversionActionId, consentStatus, gclId, gbraId, wbraId);
            return Ok();
        }


    }
}
