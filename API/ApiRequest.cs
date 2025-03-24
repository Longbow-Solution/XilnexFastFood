using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace LFFSSK.API
{
    public class ApiRequest
    {
        readonly string _TraceCategory = "ApiRequest";
        public bool SendPostRequest(string apiUrl, string xApiKey, string accessToken, string appID, string token ,string auth,string requestBody, out string responseBody)
        {
            bool isSent = false;
            responseBody = String.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    // Set xApiKey
                    if (!String.IsNullOrEmpty(xApiKey))
                    {
                        client.DefaultRequestHeaders.Add("x-api-key", xApiKey);
                    }

                    // Set accessToken
                    if (!String.IsNullOrEmpty(accessToken))
                    {
                        client.DefaultRequestHeaders.Add("accessToken", accessToken);
                    }

                    if(!String.IsNullOrEmpty(appID))
                    {
                        client.DefaultRequestHeaders.Add("appid", appID);
                    } 
                    
                    if(!String.IsNullOrEmpty(token))
                    {
                        client.DefaultRequestHeaders.Add("token", token);
                    } 
                    
                    if(!String.IsNullOrEmpty(auth))
                    {
                        client.DefaultRequestHeaders.Add("auth", auth);
                    } 
                    
                    var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;

                    if (response.StatusCode == HttpStatusCode.OK && response.IsSuccessStatusCode)
                    {
                        responseBody = response.Content.ReadAsStringAsync().Result;
                        isSent = true;
                    }
                    else
                    {
                        throw new Exception(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, $"[Error] SendPostRequest = The request timed out", _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, $"[Error] SendPostRequest = {ex.Message}", _TraceCategory);
            }

            return isSent;
        }
        public bool SendGetRequest(string apiUrl, string xApiKey, string appID, string token, string auth, out string responseBody)
        {
            bool isSent = false;
            responseBody = String.Empty;
            try
            {
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                using (var client = new HttpClient())
                {
                    // Set headers
                    if (!String.IsNullOrEmpty(xApiKey))
                    {
                        client.DefaultRequestHeaders.Add("x-api-key", xApiKey);
                    }

                    if (!String.IsNullOrEmpty(appID))
                    {
                        client.DefaultRequestHeaders.Add("appid", appID);
                    }

                    if (!String.IsNullOrEmpty(token))
                    {
                        client.DefaultRequestHeaders.Add("token", token);
                    }

                    if (!String.IsNullOrEmpty(auth))
                    {
                        client.DefaultRequestHeaders.Add("auth", auth);
                    }

                    HttpResponseMessage response = client.GetAsync(apiUrl).Result;

                    if (response.StatusCode == HttpStatusCode.OK && response.IsSuccessStatusCode)
                    {
                        isSent = true;
                        responseBody = response.Content.ReadAsStringAsync().Result;

                    }
                    else
                    {
                        throw new Exception(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, $"[Error] SendGetRequest = The request timed out", _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, $"[Error] SendGetRequest = {ex.Message}", _TraceCategory);
            }

            return isSent;
        }

        public bool SendPutRequest(string apiUrl, string xApiKey, string requestBody, out string responseBody)
        {
            bool isSent = false;
            responseBody = String.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    // Set headers
                    if (!String.IsNullOrEmpty(xApiKey))
                    {
                        client.DefaultRequestHeaders.Add("x-api-key", xApiKey);
                    }

                    // Create a StringContent with the request body
                    var content = new StringContent("", Encoding.UTF8, "application/json");

                    // Send a PUT request with the request body
                    HttpResponseMessage response = client.PutAsync(apiUrl, content).Result;

                    if (response.StatusCode == HttpStatusCode.OK && response.IsSuccessStatusCode)
                    {
                        responseBody = response.Content.ReadAsStringAsync().Result;
                        isSent = true;
                    }
                    else
                    {
                        throw new Exception(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, $"[Error] SendPutRequest = The request timed out", _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, $"[Error] SendPutRequest = {ex.Message}", _TraceCategory);
            }

            return isSent;
        }

    }
}
