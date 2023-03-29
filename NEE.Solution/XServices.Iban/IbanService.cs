using NEE.Core.Contracts;
using NEE.Core.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XServices.Iban
{
    public class IbanService : XServiceBase
    {
        public readonly string ServiceName = "ΗΔΙΚΑ Service Ταυτοποίησης IBAN - ΑΦΜ";

        private readonly string _callBaseUrl = "IbanVatValidator/Validate/HB/";

        private WebServiceConnectionString _ibanWsConStr;


        private string accessToken;

        public IbanService(string ibanWsConStr)
        {
            _ibanWsConStr = new WebServiceConnectionString(ibanWsConStr);
        }

        /// NOTE: Use only for internal/debug reasons.
        /// </remarks>
        public static IbanService CreateDefault()
        {
            // var ret = new IbanService("Idika.IbanValidator|uxL6{m]a#wybEZD5|https://www.idika.gov.gr/iban/api/"); // http://localhost:52806/");
            var ret = new IbanService("Idika.IbanValidator|uxL6{m]a#wybEZD5| http://localhost:52806/"); //");
            return ret;

        }

        public async Task<GetIbanValidateResponse> ValidateIbanAsync(GetIbanValidateRequest req)
        {
            //  return await GetAsync<GetIbanValidateResponse>($"{_callBaseUrl}/{req.IBAN}/{req.AFM}");        // with AFM and IBAN

            var ReqJson = JsonHelper.Serialize(req, true);
            return await PostAsync<GetIbanValidateResponse>($"{_callBaseUrl}", ReqJson);
        }


        private string GetUrl(string path)
        {
            if (path.StartsWith("/"))
                path = path.Substring(1);
            return $"{_ibanWsConStr.Url}{path}";
        }
        public T Get<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(Get(path));
        }
        public async Task<T> GetAsync<T>(string path)
        {
            var s = await GetAsync(path);
            return JsonConvert.DeserializeObject<T>(s);
        }
        public string Get(string path)
        {
            try
            {
                return TryGet(path);
            }
            catch (WebException ex) when (ex.Message.Contains("401"))
            {
                ResetAuthentication();
                return TryGet(path);
            }
        }
        public Task<string> GetAsync(string path)
        {
            try
            {
                return TryGetAsync(path);
            }
            catch (WebException ex) when (ex.Message.Contains("401"))
            {
                ResetAuthentication();
                return TryGetAsync(path);
            }
            catch (AggregateException aex) when (aex.InnerExceptions.Any(ex => ex.Message.Contains("401")))
            {
                ResetAuthentication();
                return TryGetAsync(path);
            }
        }
        private string TryGet(string path)
        {
            WebClient client = new WebClient();
            SetupAuthentication(client);
            Stream data = client.OpenRead(GetUrl(path));
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            reader.Close();
            data.Close();
            return s;
        }
        private Task<string> TryGetAsync(string path)
        {
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            SetupAuthentication(client);
            return client.DownloadStringTaskAsync(GetUrl(path));
        }
        private void ResetAuthentication()
        {
            accessToken = null;
        }
        public string Post(string path, string body)
        {
            try
            {
                return TryPost(path, body);
            }
            catch (WebException ex) when (ex.Message.Contains("401"))
            {
                ResetAuthentication();
                return TryPost(path, body);
            }
        }
        public Task<string> PostAsync(string path, string body)
        {
            try
            {
                return TryPostAsync(path, body);
            }
            catch (WebException ex) when (ex.Message.Contains("401"))
            {
                ResetAuthentication();
                return PostAsync(path, body);
            }
            catch (AggregateException aex) when (aex.InnerExceptions.Any(ex => ex.Message.Contains("401")))
            {
                ResetAuthentication();
                return PostAsync(path, body);
            }
        }
        private string TryPost(string path, string body)
        {
            WebClient client = new WebClient();
            SetupAuthentication(client);
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            client.Headers.Add(HttpRequestHeader.Accept, "application/json");
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] responseArray = client.UploadData(GetUrl(path), encoding.GetBytes(body));
            var responseString = encoding.GetString(responseArray);
            return responseString;
        }
        private Task<string> TryPostAsync(string path, string body)
        {
            WebClient client = new WebClient();
            SetupAuthentication(client);
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            client.Headers.Add(HttpRequestHeader.Accept, "application/json");
            // Gkikas changed the bytes to String in order to use Async 
            client.Encoding = System.Text.Encoding.UTF8;
            return client.UploadStringTaskAsync(GetUrl(path), body);
        }
        public T Post<T>(string path, string body)
        {
            return JsonConvert.DeserializeObject<T>(Post(path, body));
        }
        public async Task<T> PostAsync<T>(string path, string body)
        {
            var s = await PostAsync(path, body);
            return JsonConvert.DeserializeObject<T>(s);
        }
        #region Post Object to WebAPI
        public string PostObject(string path, object body)
        {
            try
            {
                return TryPostObject(path, body);
            }
            catch (WebException ex) when (ex.Message.Contains("401"))
            {
                ResetAuthentication();
                return TryPostObject(path, body);
            }
        }
        public Task<string> PostObjectAsync(string path, object body)
        {
            try
            {
                return PostObjectAsync(path, body);
            }
            catch (WebException ex) when (ex.Message.Contains("401"))
            {
                ResetAuthentication();
                return PostObjectAsync(path, body);
            }
            catch (AggregateException aex) when (aex.InnerExceptions.Any(ex => ex.Message.Contains("401")))
            {
                ResetAuthentication();
                return PostObjectAsync(path, body);
            }
        }
        private string TryPostObject(string path, object body)
        {
            WebClient client = new WebClient();
            SetupAuthentication(client);
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            client.Headers.Add(HttpRequestHeader.Accept, "application/json");
            JsonSerializerSettings settings = new JsonSerializerSettings();
            var data = JsonConvert.SerializeObject(body, settings);
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] responseArray = client.UploadData(GetUrl(path), encoding.GetBytes(data));
            var responseString = encoding.GetString(responseArray);
            return responseString;
        }
        private Task<string> TryPostObjectAsync(string path, object body)
        {
            WebClient client = new WebClient();
            SetupAuthentication(client);
            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            client.Headers.Add(HttpRequestHeader.Accept, "application/json");
            client.Encoding = System.Text.Encoding.UTF8;
            JsonSerializerSettings settings = new JsonSerializerSettings();
            string data = JsonConvert.SerializeObject(body, settings);
            return client.UploadStringTaskAsync(GetUrl(path), data);
        }
        public T PostObject<T>(string path, object body)
        {
            return JsonConvert.DeserializeObject<T>(PostObject(path, body));
        }
        public async Task<T> PostObjectAsync<T>(string path, object body)
        {
            var s = await PostObjectAsync(path, body);
            return JsonConvert.DeserializeObject<T>(s);
        }
        #endregion
        class TokenInfo
        {
            public string access_token { get; set; }
        }
        public string RequestAccessToken()
        {
            WebClient client = new WebClient();
            string xx = GetUrl("Token");
            string yy = $"grant_type=password&username={_ibanWsConStr.Uid}&password={_ibanWsConStr.Pwd}";
            var tokenInfoString = client.UploadString(GetUrl("Token"), $"grant_type=password&username={_ibanWsConStr.Uid}&password={_ibanWsConStr.Pwd}");
            var tokenInfo = JsonConvert.DeserializeObject<TokenInfo>(tokenInfoString);
            return tokenInfo.access_token;
        }
        private void SetupAuthentication(WebClient client)
        {
            if (!string.IsNullOrWhiteSpace(_ibanWsConStr.Uid) && !string.IsNullOrWhiteSpace(_ibanWsConStr.Pwd))
            {
                if (accessToken == null)
                {
                    accessToken = RequestAccessToken();
                }
                client.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {accessToken}");
            }
        }
    }
}
