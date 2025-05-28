using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace LFFSSK.API
{
    public class ApiFunc
    {
        private string _TraceCategory = "ApiFunc";

        public bool GetMenu(out ApiModel.GetMenu.Response res)
        {
            bool isTrue = false;
            res = null;

            try
            {
                ApiRequest apiRequest = new ApiRequest();

                string apiUrl = GeneralVar.ApiURL + "/v1/menumanager/menus?locationid="+GeneralVar.LocationID+"&menuProfileId="+ GeneralVar.MenuID +"&source="+ GeneralVar.Source;
                
                isTrue = apiRequest.SendGetRequest(apiUrl, null, GeneralVar.AppID,GeneralVar.token,GeneralVar.auth, out string responseBody);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] GetMenu API Response = {0}\n", responseBody), _TraceCategory);

                //Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] GetCategory API Response = {0}\n", responseBody), _TraceCategory);

                if (isTrue)
                {
                    res = JsonConvert.DeserializeObject<ApiModel.GetMenu.Response>(responseBody);
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] GetCategory = {0}", ex.Message), _TraceCategory);
            }
            return isTrue;
        }

        public bool FnBOrder(ApiModel.FnBOrders.Request req, out ApiModel.FnBOrders.Response res)
        {
            bool isTrue = false;
            res = null;

            try
            {
                ApiRequest apiRequest = new ApiRequest();

                string apiUrl = GeneralVar.ApiURL + "/v2/sales/fnbOrders";
                string requestBody = JsonConvert.SerializeObject(req);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] FnBOrder API Request = {0}\n", requestBody), _TraceCategory);

                isTrue = apiRequest.SendPostRequest(apiUrl, null, null, GeneralVar.AppID, GeneralVar.token, GeneralVar.auth, requestBody, out string responseBody);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] FnBOrder API Response = {0}\n", responseBody), _TraceCategory);

                if (isTrue)
                {
                    res = JsonConvert.DeserializeObject<ApiModel.FnBOrders.Response>(responseBody);
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] FnBOrder = {0}", ex.Message), _TraceCategory);
            }
            return isTrue;
        }

        public bool InitOrder(ApiModel.InitialOrderRequest req, out ApiModel.InitialOrderResponse res)
        {
            bool isTrue = false;
            res = null;

            try
            {
                ApiRequest apiRequest = new ApiRequest();

                string apiUrl = GeneralVar.ApiLB + "/api/SSK/InitOrder";
                string requestBody = JsonConvert.SerializeObject(req);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] InitOrder API Request = {0}\n", requestBody), _TraceCategory);

                isTrue = apiRequest.SendPostRequest(apiUrl, null, null, null, null, null, requestBody, out string responseBody);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] InitOrder API Response = {0}\n", responseBody), _TraceCategory);

                if (isTrue)
                {
                    res = JsonConvert.DeserializeObject<ApiModel.InitialOrderResponse>(responseBody);
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] InitOrder = {0}", ex.Message), _TraceCategory);
            }
            return isTrue;
        }

        public bool PostOrder(ApiModel.PostOrderRequest req, out ApiModel.PostOrderResponse res)
        {
            bool isTrue = false;
            res = null;

            try
            {
                ApiRequest apiRequest = new ApiRequest();

                string apiUrl = GeneralVar.ApiLB + "/api/SSK/PostOrder";
                string requestBody = JsonConvert.SerializeObject(req);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] PostOrder API Request = {0}\n", requestBody), _TraceCategory);

                isTrue = apiRequest.SendPostRequest(apiUrl, null, null, GeneralVar.AppID, GeneralVar.token, GeneralVar.auth, requestBody, out string responseBody);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] PostOrder API Response = {0}\n", responseBody), _TraceCategory);

                if (isTrue)
                {
                    res = JsonConvert.DeserializeObject<ApiModel.PostOrderResponse>(responseBody);
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] PostOrder = {0}", ex.Message), _TraceCategory);
            }
            return isTrue;
        }
    }
}

