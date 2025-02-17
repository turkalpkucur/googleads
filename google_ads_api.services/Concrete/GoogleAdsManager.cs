using Google.Ads.Gax.Examples;
using Google.Ads.GoogleAds;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V18.Common;
using Google.Ads.GoogleAds.V18.Errors;
using Google.Ads.GoogleAds.V18.Services;
using google_ads_api.services.Abstract;
using google_ads_api.services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Google.Ads.GoogleAds.V18.Enums.ConsentStatusEnum.Types;

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
                string scope = "https://www.googleapis.com/auth/adwords";
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


        public async Task<List<string>> ListAvaiableCustomers(string refreshToken)
        {
            try
            {
                string[] customerNames = new string[] { };
                GoogleAdsConfig config = new GoogleAdsConfig()
                {
                    DeveloperToken = "yQJX5fsBmfb_2fAYttQ5HA",
                    OAuth2Mode = Google.Ads.Gax.Config.OAuth2Flow.APPLICATION,
                    OAuth2ClientId = "124948101982-v7tnt3p8icvjmrm7f37n6kine232oli3.apps.googleusercontent.com",
                    OAuth2ClientSecret = "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",
                    //    OAuth2SecretsJsonPath = "credentials.json",
                    OAuth2RefreshToken = refreshToken,
                    LoginCustomerId = "4698070815"
                };
                GoogleAdsClient client = new GoogleAdsClient(config);
  
                try
                {
                    Google.Ads.GoogleAds.V18.Services.CustomerServiceClient customerService = client.GetService(Services.V18.CustomerService);
                    customerNames = customerService.ListAccessibleCustomers();
                }
                catch (GoogleAdsException e)
                {
                    Console.WriteLine("Failure:");
                    Console.WriteLine($"Message: {e.Message}");
                    Console.WriteLine($"Failure: {e.Failure}");
                    Console.WriteLine($"Request ID: {e.RequestId}");
                    throw;
                }
                return customerNames.ToList();
                  
            }
            catch (Exception ex)
            {
                return null;
            }
        }


     


        public async Task UploadOfflineConversion(string refreshToken,  double  conversionValue,  string conversionTime, long conversionActionId, ConsentStatus? adUserDataConsent, string gclid, string gbraid, string wbraid)
        {
            long customerId = Convert.ToInt64("4698070815");
            GoogleAdsConfig config = new GoogleAdsConfig()
            {
                DeveloperToken = "yQJX5fsBmfb_2fAYttQ5HA",
                OAuth2Mode = Google.Ads.Gax.Config.OAuth2Flow.APPLICATION,
                OAuth2ClientId = "124948101982-v7tnt3p8icvjmrm7f37n6kine232oli3.apps.googleusercontent.com",
                OAuth2ClientSecret = "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",
                //    OAuth2SecretsJsonPath = "credentials.json",
                OAuth2RefreshToken = refreshToken,
                LoginCustomerId = "4698070815"
            };
            GoogleAdsClient client = new GoogleAdsClient(config);


            ConversionUploadServiceClient conversionUploadService =
               client.GetService(Services.V18.ConversionUploadService);

            // Creates a click conversion by specifying currency as USD.
            ClickConversion clickConversion = new ClickConversion()
            {
                ConversionAction = ResourceNames.ConversionAction(customerId, conversionActionId),
                ConversionValue = conversionValue,
                ConversionDateTime = conversionTime,
                CurrencyCode = "USD",
            };

            // Sets the consent information, if provided.
            if (adUserDataConsent != null)
            {
                // Specifies whether user consent was obtained for the data you are uploading. See
                // https://www.google.com/about/company/user-consent-policy
                // for details.
                clickConversion.Consent = new Consent()
                {
                    AdUserData = (ConsentStatus)adUserDataConsent
                };
            }

            // Verifies that exactly one of gclid, gbraid, and wbraid is specified, as required.
            // See https://developers.google.com/google-ads/api/docs/conversions/upload-clicks
            // for details.
            string[] ids = { gclid, gbraid, wbraid };
            int idCount = ids.Where(id => !string.IsNullOrEmpty(id)).Count();

            if (idCount != 1)
            {
                throw new ArgumentException($"Exactly 1 of gclid, gbraid, or wbraid is " +
                    $"required, but {idCount} ID values were provided");
            }

            // Sets the single specified ID field.
            if (!string.IsNullOrEmpty(gclid))
            {
                clickConversion.Gclid = gclid;
            }
            else if (!string.IsNullOrEmpty(wbraid))
            {
                clickConversion.Wbraid = wbraid;
            }
            else if (!string.IsNullOrEmpty(gbraid))
            {
                clickConversion.Gbraid = gbraid;
            }

            try
            {
                // Issues a request to upload the click conversion.
                UploadClickConversionsResponse response =
                    conversionUploadService.UploadClickConversions(
                        new UploadClickConversionsRequest()
                        {
                            CustomerId = customerId.ToString(),
                            Conversions = { clickConversion },
                            PartialFailure = true,
                            ValidateOnly = false
                        });

                // Prints the result.
                ClickConversionResult uploadedClickConversion = response.Results[0];
                Console.WriteLine($"Uploaded conversion that occurred at " +
                    $"'{uploadedClickConversion.ConversionDateTime}' from Google " +
                    $"Click ID '{uploadedClickConversion.Gclid}' to " +
                    $"'{uploadedClickConversion.ConversionAction}'.");
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
