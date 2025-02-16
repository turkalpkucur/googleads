using google_ads_api.services.Models;
using System.Threading.Tasks;

namespace google_ads_api.services.Abstract
{
    public interface IGoogleAdsService
    {
        public string GetAuthCode();
        public Task<GoogleTokenDto> GetTokens(string code);
        public Task<Google.Ads.GoogleAds.V18.Services.GoogleAdsRow> GetCustomers(string refreshToken);
    }

}
