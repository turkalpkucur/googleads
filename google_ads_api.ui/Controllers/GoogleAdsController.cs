using google_ads_api.services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
            Google.Ads.GoogleAds.V18.Services.GoogleAdsRow result= await _googleAdsService.GetCustomers(refreshToken);
            return Ok(result);
        }


    }
}
