using google_ads_api.services.Models;
using System.Threading.Tasks;

namespace google_ads_api.services.Abstract
{
    public interface IGoogleAdsService
    {
        public string GetAuthCode();
        public Task<GoogleTokenDto> GetTokens(string code);
        public Task GetCustomers(string refreshToken);
    }

}
