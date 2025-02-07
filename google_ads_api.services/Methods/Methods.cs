using System.Text;

namespace google_ads_api.services.Methods
{
    public class Methods
    {
        public static string urlEncodeForGoogle(string url)
        {
            string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-.~";
            StringBuilder result = new StringBuilder();
            foreach (char symbol in url)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append("%" + ((int)symbol).ToString("X2"));
                }
            }

            return result.ToString();

        }

     
    }
}
