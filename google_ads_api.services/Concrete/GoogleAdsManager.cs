using Google.Ads.GoogleAds;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V18.Errors;
using Google.Ads.GoogleAds.V18.Services;
using google_ads_api.services.Abstract;
using google_ads_api.services.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace google_ads_api.services.Concrete
{
    public class GoogleAdsManager : IGoogleAdsService
    {
        private readonly HttpClient _httpClient;
        public GoogleAdsManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public string GetAuthCode()
        {
            try
            {
                string scopeURL1 = "https://accounts.google.com/o/oauth2/auth?redirect_uri={0}&prompt={1}&response_type={2}&client_id={3}&scope={4}&access_type={5}";
                var redirectURL = "https://localhost:44339/googleads/callback";
                string prompt = "consent";
                string response_type = "code";
                string clientID = "124948101982-v7tnt3p8icvjmrm7f37n6kine232oli3.apps.googleusercontent.com";
                string scope = "https://www.googleapis.com/auth/calendar";
                string access_type = "offline";
                string redirect_uri_encode = Methods.Methods.urlEncodeForGoogle(redirectURL);
                var mainURL = string.Format(scopeURL1, redirect_uri_encode, prompt, response_type, clientID, scope, access_type);

                return mainURL;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public async Task<GoogleTokenDto> GetTokens(string code)
        {

            var clientId = "124948101982-v7tnt3p8icvjmrm7f37n6kine232oli3.apps.googleusercontent.com";
            string clientSecret = "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";
            var redirectURL = "https://localhost:44339/googleads/callback";
            var tokenEndpoint = "https://accounts.google.com/o/oauth2/token";
            var content = new StringContent($"code={code}&redirect_uri={Uri.EscapeDataString(redirectURL)}&client_id={clientId}&client_secret={clientSecret}&grant_type=authorization_code", Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleTokenDto>(responseContent);
                return tokenResponse;
            }
            else
            {
                // Handle the error case when authentication fails
                throw new Exception($"Failed to authenticate: {responseContent}");
            }
        }


        public async Task<int> GetCustomers(string refreshToken)
        {
            try
            {
                GoogleAdsConfig config = new GoogleAdsConfig()
                {
                    DeveloperToken = "******",
                    OAuth2Mode = Google.Ads.Gax.Config.OAuth2Flow.APPLICATION,
                    OAuth2ClientId = "124948101982-v7tnt3p8icvjmrm7f37n6kine232oli3.apps.googleusercontent.com",
                    OAuth2ClientSecret = "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",
                    OAuth2RefreshToken = refreshToken,
                    LoginCustomerId = "627-912-0547"
                };
                GoogleAdsClient client = new GoogleAdsClient(config);

            }
            catch
            {


            }
        }


        public void Run(GoogleAdsClient client, long customerId)
        {
            // Get the GoogleAdsService.
            GoogleAdsServiceClient googleAdsService = client.GetService(
                Services.V18.GoogleAdsService);

            // Create a query that will retrieve all campaigns.
            string query = @"SELECT
                    campaign.id,
                    campaign.name,
                    campaign.network_settings.target_content_network
                FROM campaign
                ORDER BY campaign.id";

            try
            {
                // Issue a search request.
                googleAdsService.SearchStream(customerId.ToString(), query,
                    delegate (SearchGoogleAdsStreamResponse resp)
                    {
                        foreach (GoogleAdsRow googleAdsRow in resp.Results)
                        {
                            Console.WriteLine("Campaign with ID {0} and name '{1}' was found.",
                                googleAdsRow.Campaign.Id, googleAdsRow.Campaign.Name);
                        }
                    }
                );
            }
            catch (GoogleAdsException e)
            {
                Console.WriteLine("Failure:");
                Console.WriteLine($"Message: {e.Message}");
                Console.WriteLine($"Failure: {e.Failure}");
                Console.WriteLine($"Request ID: {e.RequestId}");
                throw;
            }
        }
    }
}
