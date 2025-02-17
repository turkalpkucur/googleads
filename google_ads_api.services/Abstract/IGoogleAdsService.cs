using Google.Ads.GoogleAds.Lib;
using google_ads_api.services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Google.Ads.GoogleAds.V18.Enums.ConsentStatusEnum.Types;

namespace google_ads_api.services.Abstract
{
    public interface IGoogleAdsService
    {
        public string GetAuthCode();
        public Task<GoogleTokenDto> GetTokens(string code);
        public Task<List<string>> ListAvaiableCustomers(string refreshToken);

        public Task UploadOfflineConversion(string refreshToken, double conversionValue,   string conversionTime, long conversionActionId, ConsentStatus? adUserDataConsent, string gclid, string gbraid, string wbraid);
    }
}