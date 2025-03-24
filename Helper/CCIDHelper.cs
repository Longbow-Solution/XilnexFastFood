using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace LFFSSK
{
    class CCIDHelper
    {
        public static string GetCCid()
        {
            try
            {
                // put our webservice URI here
                Uri uri = new Uri("http://103.233.3.201:8080/KFC-Ccid/api/Ccid");

                // creating header to JSON - no payload since this is GET
                HttpContent httpContent = new StringContent("", Encoding.UTF8, "application/json");

                // getting the response
                var t = Task.Run(() => GetURI(uri, httpContent));
                t.Wait();
                return t.Result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return "-1";

        }


        static async Task<string> GetURI(Uri u, HttpContent c)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage result = await client.GetAsync(u);
                if (result.IsSuccessStatusCode)
                {
                    return await result.Content.ReadAsStringAsync();
                }
                return "-1";
            }

        }
    }
}