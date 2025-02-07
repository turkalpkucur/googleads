using Google.Api.Ads.AdManager.Lib;
using Google.Api.Ads.AdManager.Util.v202411;
using Google.Api.Ads.AdManager.v202411;
using Google.Api.Ads.Common.Lib;
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

         
                using (CompanyService companyService = new CompanyService())
                {

                    int pageSize = StatementBuilder.SUGGESTED_PAGE_LIMIT;
                    StatementBuilder statementBuilder =
                        new StatementBuilder().OrderBy("id ASC").Limit(pageSize);
                    
                    // Retrieve a small amount of companies at a time, paging through until all
                    // companies have been retrieved.
                    int totalResultSetSize = 0;
                    do
                    {
                        CompanyPage page =
                            companyService.getCompaniesByStatement(statementBuilder.ToStatement());

                        // Print out some information for each company.
                        if (page.results != null)
                        {
                            totalResultSetSize = page.totalResultSetSize;
                            int i = page.startIndex;
                            foreach (Company company in page.results)
                            {
                                Console.WriteLine(
                                    "{0}) Company with ID {1}, name \"{2}\", and type \"{3}\" was " +
                                    "found.",
                                    i++, company.id, company.name, company.type);
                            }
                        }

                        statementBuilder.IncreaseOffsetBy(pageSize);
                    } while (statementBuilder.GetOffset() < totalResultSetSize);
                    return totalResultSetSize;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
