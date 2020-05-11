using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ATMInfoAPI.Lib
{
    public class BRest
    {
        public static HttpClient GetHttpClient(string proxyUrl = null)
        {
            if (string.IsNullOrEmpty(proxyUrl))
            {
                return new HttpClient();
            }

            WebProxy proxy = new WebProxy
            {
                Address = new Uri(proxyUrl),
                UseDefaultCredentials = true
            };

            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy,
            };

            return new HttpClient(httpClientHandler);
        }

        public static HttpClient SetBasicAuth(HttpClient client, string username, string password)
        {
            AuthenticationHeaderValue authHeader =
                new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));

            client.DefaultRequestHeaders.Authorization = authHeader;
            return client;
        }

        public static async Task<string> HttpPostAsync(HttpClient client, string url, Dictionary<string, string> data)
        {
            FormUrlEncodedContent encodedData = new FormUrlEncodedContent(data);
            HttpResponseMessage response = await client.PostAsync(url, encodedData);
            return await response.Content.ReadAsStringAsync();
        }

        public static string HttpPost(HttpClient client, string url, Dictionary<string, string> data)
        {
            FormUrlEncodedContent encodedData = new FormUrlEncodedContent(data);
            return HttpPost(client, url, encodedData);
        }

        public static string HttpPost(HttpClient client, string url, object data)
        {
            string json = SerializeRequest(data);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            return HttpPost(client, url, content);
        }

        private static string HttpPost(HttpClient client, string url, HttpContent content)
        {
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public static string HttpPut(HttpClient client, string url, object data)
        {
            string json = SerializeRequest(data);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            return HttpPut(client, url, content);
        }

        public static string HttpDelete(HttpClient client, string url)
        {
            HttpResponseMessage response = client.DeleteAsync(url).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        private static string HttpPut(HttpClient client, string url, HttpContent content)
        {
            HttpResponseMessage response = client.PutAsync(url, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public static async Task<string> HttpGetAsync(HttpClient client, string url)
        {
            return await client.GetStringAsync(url);
        }

        public static string HttpGet(HttpClient client, string url)
        {
            return client.GetStringAsync(url).Result;
        }

        public static T DeserializeResponse<T>(string response)
        {
            try
            {
                return (T)JsonConvert.DeserializeObject(response, typeof(T));
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static string SerializeRequest<T>(T requestObject)
        {
            try
            {
                return JsonConvert.SerializeObject(requestObject);
            }
            catch
            {
                return null;
            }
        }
    }
}
