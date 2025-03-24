using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LFFSSK.Helper
{
    public class RazerPayIntegration
    {
        const string TraceCategory = "RazerPayIntegration";
        TraceSwitch SwcTraceLevel = new TraceSwitch("SwcTraceLevel", "Trace Level Switch");

        public RazerPayIntegration()
        {

        }

        public RazerPaymentResponse RazerPayment(string QRBarcode, string ReferenceId, decimal TotalAmount, eRazerChannel PaymentChannel, string componentCode)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Trace.WriteLineIf(SwcTraceLevel.TraceInfo, "RazerPayment starting...", TraceCategory);
            RazerPaymentResponse responseData = new RazerPaymentResponse();
            string result = string.Empty;

            try
            {
                string url = string.Format(@"{0}/API/MOLOPA/payment.php", "https://opa.merchant.razer.com/RMS");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                string param = string.Format("amount={0}", TotalAmount.ToString("#,0.00"));
                param += string.Format("&applicationCode={0}", GeneralVar.Razer_ApplicationCode.Trim());
                param += string.Format("&authorizationCode={0}", QRBarcode);
                param += string.Format("&authorizationCodeType={0}", eAuthCodeType.Barcode.ToString("d"));
                param += string.Format("&businessDate={0}", DateTime.Now.Date.ToString("yyyy-MM-dd"));
                param += string.Format("&channelId={0}", PaymentChannel == eRazerChannel.Any ? string.Empty : PaymentChannel.ToString("d"));
                param += string.Format("&currencyCode={0}", "MYR");
                param += string.Format("&description={0}", "LFFSSK");//GeneralVar.CurrentComponent.CinemaName);
                param += string.Format("&referenceId={0}", ReferenceId);
                param += string.Format("&storeId={0}", componentCode);//GeneralVar.EWallet_StoreId.Trim());
                param += string.Format("&terminalId={0}", componentCode);//GeneralVar.EWallet_TerminalId.Trim());
                param += string.Format("&version={0}", "v2");//GeneralVar.EWallet_Version);
                param += string.Format("&hashType={0}", "hmac-sha256");

                string dataVal = TotalAmount.ToString("#,0.00").Trim(); // Amount Value 
                dataVal += GeneralVar.Razer_ApplicationCode.Trim(); //application Code Value
                dataVal += QRBarcode.Trim(); // Authorization Code Value
                dataVal += eAuthCodeType.Barcode.ToString("d");// GeneralVar.EWalletAuthCodeType.ToString("d").Trim(); // Authorization Code Type Value
                dataVal += DateTime.Now.Date.ToString("yyyy-MM-dd").Trim(); //business date value
                dataVal += PaymentChannel == eRazerChannel.Any ? string.Empty : PaymentChannel.ToString("d").Trim(); // Channel Value
                dataVal += "MYR"; // Currency Code Value
                dataVal += "LFFSSK"; // Description Value
                dataVal += "hmac-sha256"; //hash type value
                dataVal += ReferenceId.Trim(); // Reference Id Value
                dataVal += componentCode;//GeneralVar.EWallet_StoreId.Trim(); // store Id value
                dataVal += componentCode;//GeneralVar.EWallet_TerminalId.Trim(); // Terminal Id value

                dataVal += "v2";//GeneralVar.EWallet_Version.Trim(); // Version Value

                string sign = GetHMACSHA256Hash(dataVal, GeneralVar.Razer_SecretKey.Trim());//GeneralVar.EWallet_SecretKey.Trim());

                param += string.Format("&signature={0}", sign);

                Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerPayment Url: {0}", url), TraceCategory);
                Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerPayment Encrypt Sign: {0}", dataVal), TraceCategory);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerPayment Request: {0}", param), TraceCategory);
                    streamWriter.Write(param);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    result = reader.ReadToEnd();
                    responseData = JsonConvert.DeserializeObject<RazerPaymentResponse>(result);
                }
                Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerPayment Response = {0}", result), TraceCategory);

                if (responseData.statusCode != "00" || !string.IsNullOrEmpty(responseData.errorCode))
                {
                    Trace.WriteLineIf(SwcTraceLevel.TraceError, string.Format("RazerPayment: Status code = {0}, Error Code = {1}", responseData.statusCode, responseData.errorCode), TraceCategory);
                }
            }
            catch (Exception ex)
            {

                Trace.WriteLineIf(SwcTraceLevel.TraceError, string.Format("RazerPayment failed [ErrorCode = {0}]", ex.Message), TraceCategory);
            }

            Trace.WriteLineIf(SwcTraceLevel.TraceInfo, "RazerPayment completed.", TraceCategory);
            return responseData;
        }

        public RazerPaymentResponse RazerInquiry(string ReferenceId)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Trace.WriteLineIf(SwcTraceLevel.TraceInfo, "RazerInquiry starting...", TraceCategory);
            RazerPaymentResponse responseData = new RazerPaymentResponse();
            string result = string.Empty;

            try
            {
                string url = string.Format(@"{0}/API/MOLOPA/inquiry.php", "https://api.merchant.razer.com/RMS");

                string param = string.Format("applicationCode={0}", GeneralVar.Razer_ApplicationCode.Trim());//GeneralVar.EWalletApplicationCode);
                param += string.Format("&version={0}", "v2");//GeneralVar.EWallet_Version);
                param += string.Format("&referenceId={0}", ReferenceId);
                param += string.Format("&hashType={0}", "hmac-sha256");

                string dataVal = GeneralVar.Razer_ApplicationCode.Trim();//GeneralVar.EWalletApplicationCode.Trim(); //application Code Value
                dataVal += "hmac-sha256"; //hash type value
                dataVal += ReferenceId.Trim(); // Reference Id Value
                dataVal += "v2";// GeneralVar.EWallet_Version.Trim(); // Version Value

                string sign = GetHMACSHA256Hash(dataVal, GeneralVar.Razer_SecretKey.Trim());//GeneralVar.EWallet_SecretKey.Trim());

                param += string.Format("&signature={0}", sign);

                url += "?" + param;

                Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerInquiry Url: {0}", url), TraceCategory);
                Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerInquiry Encrypt Sign: {0}", dataVal), TraceCategory);
                Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerInquiry Request: {0}", param), TraceCategory);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    result = reader.ReadToEnd();
                    responseData = JsonConvert.DeserializeObject<RazerPaymentResponse>(result);

                    Trace.WriteLineIf(SwcTraceLevel.TraceError, string.Format("RazerInquiry: Response = {0}", result), TraceCategory);
                }

                if (responseData.statusCode != "00" || !string.IsNullOrEmpty(responseData.errorCode))
                {
                    Trace.WriteLineIf(SwcTraceLevel.TraceError, string.Format("RazerInquiry: Status code = {0}, Error Code = {1}", responseData.statusCode, responseData.errorCode), TraceCategory);
                }
            }
            catch (Exception ex)
            {

                Trace.WriteLineIf(SwcTraceLevel.TraceError, string.Format("RazerInquiry failed [ErrorCode = {0}]", ex.Message), TraceCategory);
            }

            Trace.WriteLineIf(SwcTraceLevel.TraceInfo, "RazerInquiry completed.", TraceCategory);
            return responseData;
        }

        public RazerReversalVoidResponse RazerReversalVoid(string ReferenceId, string paymentReferenceId)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Trace.WriteLineIf(SwcTraceLevel.TraceInfo, "RazerReversalVoid starting...", TraceCategory);
            RazerReversalVoidResponse responseData = new RazerReversalVoidResponse();
            string result = string.Empty;

            try
            {
                string url = string.Format(@"{0}/API/MOLOPA/reversal.php", "https://api.merchant.razer.com/RMS");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                string param = string.Format("applicationCode={0}", GeneralVar.Razer_ApplicationCode.Trim());//GeneralVar.EWalletApplicationCode);
                param += string.Format("&businessDate={0}", DateTime.Now.Date.ToString("yyyy-MM-dd"));
                param += string.Format("&hashType={0}", "hmac-sha256");
                param += string.Format("&paymentReferenceId={0}", paymentReferenceId); // Payment Reference Id Value
                param += string.Format("&referenceId={0}", ReferenceId); // Reference Id
                param += string.Format("&version={0}", "v2");//GeneralVar.EWallet_Version);

                string dataVal = GeneralVar.Razer_ApplicationCode.Trim();//GeneralVar.EWalletApplicationCode.Trim(); //application Code Value
                dataVal += DateTime.Now.Date.ToString("yyyy-MM-dd").Trim(); //business date value
                dataVal += "hmac-sha256"; //hash type value
                dataVal += paymentReferenceId.Trim(); // Payment Reference Id Value
                dataVal += ReferenceId.Trim(); // Reference Id Value
                dataVal += "v2";// GeneralVar.EWallet_Version.Trim(); // Version Value

                string sign = GetHMACSHA256Hash(dataVal, GeneralVar.Razer_SecretKey.Trim());//GeneralVar.EWallet_SecretKey.Trim());

                param += string.Format("&signature={0}", sign);

                Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerReversalVoid Url: {0}", url), TraceCategory);
                Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerReversalVoid Encrypt Sign: {0}", dataVal), TraceCategory);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerReversalVoid Request: {0}", param), TraceCategory);
                    streamWriter.Write(param);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    result = reader.ReadToEnd();
                    responseData = JsonConvert.DeserializeObject<RazerReversalVoidResponse>(result);

                    Trace.WriteLineIf(SwcTraceLevel.TraceError, string.Format("RazerReversalVoid: Response = {0}", result), TraceCategory);
                }

                if (responseData.statusCode != "00" || !string.IsNullOrEmpty(responseData.errorCode))
                {
                    Trace.WriteLineIf(SwcTraceLevel.TraceError, string.Format("RazerReversalVoid: Status code = {0}, Error Code = {1}", responseData.statusCode, responseData.errorCode), TraceCategory);
                }
            }
            catch (Exception ex)
            {

                Trace.WriteLineIf(SwcTraceLevel.TraceError, string.Format("RazerReversalVoid failed [ErrorCode = {0}]", ex.Message), TraceCategory);
            }

            Trace.WriteLineIf(SwcTraceLevel.TraceInfo, "RazerReversalVoid completed.", TraceCategory);
            return responseData;
        }

        public RazerRefundResponse RazerRefund(string ReferenceId, string paymentReferenceId, decimal Amount, string BusinessDate)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Trace.WriteLineIf(SwcTraceLevel.TraceInfo, "RazerRefund starting...", TraceCategory);
            RazerRefundResponse responseData = new RazerRefundResponse();
            string result = string.Empty;

            try
            {
                string url = string.Format(@"{0}/API/MOLOPA/refund.php", "https://api.merchant.razer.com/RMS");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                string param = string.Format("amount={0}", Amount.ToString("#,0.00"));
                param += string.Format("&applicationCode={0}", GeneralVar.Razer_ApplicationCode.Trim());//GeneralVar.EWalletApplicationCode);
                param += string.Format("&businessDate={0}", BusinessDate); //business date value yyyy-MM-dd
                param += string.Format("&currencyCode={0}", "MYR");
                param += string.Format("&description={0}", "MS Cinema Kampar");//GeneralVar.CurrentComponent.CinemaName);
                param += string.Format("&hashType={0}", "hmac-sha256");
                param += string.Format("&paymentReferenceId={0}", paymentReferenceId); // Payment Reference Id Value
                param += string.Format("&referenceId={0}", ReferenceId); // Reference Id
                param += string.Format("&version={0}", "v2");//GeneralVar.EWallet_Version);

                string dataVal = string.Format("amount={0}", Amount.ToString("#,0.00"));
                dataVal += GeneralVar.Razer_ApplicationCode.Trim();//GeneralVar.EWalletApplicationCode.Trim(); //application Code Value
                dataVal += BusinessDate; //business date value yyyy-MM-dd
                dataVal += "MYR"; // Currency Code Value
                dataVal += "MS Cinema Kampar"; // Description Value
                dataVal += "hmac-sha256"; //hash type value
                dataVal += paymentReferenceId.Trim(); // Payment Reference Id Value
                dataVal += ReferenceId.Trim(); // Reference Id Value
                dataVal += "v2";// GeneralVar.EWallet_Version.Trim(); // Version Value

                string sign = GetHMACSHA256Hash(dataVal, "9de2c1e863888b6fd9993e1");//GeneralVar.EWallet_SecretKey.Trim());

                param += string.Format("&signature={0}", sign);

                Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerRefund Url: {0}", url), TraceCategory);
                Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerRefund Encrypt Sign: {0}", dataVal), TraceCategory);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerRefund Parameter: {0}", param), TraceCategory);
                    streamWriter.Write(param);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    responseData = JsonConvert.DeserializeObject<RazerRefundResponse>(reader.ReadToEnd());
                }

                if (responseData.statusCode != "00" || !string.IsNullOrEmpty(responseData.errorCode))
                {
                    Trace.WriteLineIf(SwcTraceLevel.TraceError, string.Format("RazerRefund: Status code = {0}, Error Code = {1}", responseData.statusCode, responseData.errorCode), TraceCategory);
                }
            }
            catch (Exception ex)
            {

                Trace.WriteLineIf(SwcTraceLevel.TraceError, string.Format("RazerRefund failed [ErrorCode = {0}]", ex.Message), TraceCategory);
            }

            Trace.WriteLineIf(SwcTraceLevel.TraceInfo, "RazerRefund completed.", TraceCategory);
            return responseData;
        }

        public RazerPreCreateQRResponse RazerPreCreateQRPayment(string ReferenceId, decimal TotalAmount, eRazerChannel PaymentChannel)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Trace.WriteLineIf(SwcTraceLevel.TraceInfo, "RazerPreCreateQRPayment starting...", TraceCategory);
            RazerPreCreateQRResponse responseData = new RazerPreCreateQRResponse();
            string result = string.Empty;

            try
            {
                string url = string.Format(@"{0}/API/MOLOPA/precreate.php", "https://opa.merchant.razer.com/RMS");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                string param = string.Format("amount={0}", TotalAmount.ToString("#,0.00"));
                param += string.Format("&applicationCode={0}", GeneralVar.Razer_ApplicationCode.Trim());//GeneralVar.EWalletApplicationCode);
                param += string.Format("&businessDate={0}", DateTime.Now.Date.ToString("yyyy-MM-dd").Trim());
                param += string.Format("&channelId={0}", PaymentChannel == eRazerChannel.Any ? string.Empty : PaymentChannel.ToString("d").Trim());
                param += string.Format("&currencyCode={0}", "MYR");
                param += string.Format("&description={0}", "AnW Malaysia");
                param += string.Format("&imageFormat={0}", "png");
                param += string.Format("&imageSize={0}", "");
                param += string.Format("&referenceId={0}", ReferenceId.Trim());
                param += string.Format("&storeId={0}", GeneralVar.ComponentCode);//GeneralVar.EWallet_StoreId.Trim());
                param += string.Format("&terminalId={0}", GeneralVar.ComponentCode);//GeneralVar.EWallet_TerminalId.Trim());
                param += string.Format("&version={0}", "v2");//GeneralVar.EWallet_Version);
                param += string.Format("&hashType={0}", "hmac-sha256");


                string dataVal = TotalAmount.ToString("#,0.00").Trim(); // Amount Value 
                dataVal += GeneralVar.Razer_ApplicationCode.Trim();
                dataVal += DateTime.Now.Date.ToString("yyyy-MM-dd").Trim(); //business date value
                dataVal += PaymentChannel == eRazerChannel.Any ? string.Empty : PaymentChannel.ToString("d").Trim(); // Channel Value
                dataVal += "MYR"; // Currency Code Value
                dataVal += "AnW Malaysia"; // Description Value
                dataVal += "hmac-sha256"; //hash type value
                dataVal += "png"; // image Format
                dataVal += ""; // image Size
                dataVal += ReferenceId.Trim(); // Reference Id Value
                dataVal += GeneralVar.ComponentCode;//GeneralVar.EWallet_StoreId.Trim(); // store Id value
                dataVal += GeneralVar.ComponentCode;//GeneralVar.EWallet_TerminalId.Trim(); // Terminal Id value

                dataVal += "v2";//GeneralVar.EWallet_Version.Trim(); // Version Value

                string sign = GetHMACSHA256Hash(dataVal, GeneralVar.Razer_SecretKey.Trim());//GeneralVar.EWallet_SecretKey.Trim());

                param += string.Format("&signature={0}", sign);

                Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerPreCreateQRPayment Url: {0}", url), TraceCategory);
                Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerPreCreateQRPayment Encrypt Sign: {0}", dataVal), TraceCategory);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerPreCreateQRPayment Parameter: {0}", param), TraceCategory);
                    streamWriter.Write(param);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    responseData = JsonConvert.DeserializeObject<RazerPreCreateQRResponse>(reader.ReadToEnd());
                    //result = reader.ReadToEnd();
                }
                Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("RazerPreCreateQRPayment response {0}", result), TraceCategory);

                if (responseData.statusCode != "00" || !string.IsNullOrEmpty(responseData.errorCode))
                {
                    Trace.WriteLineIf(SwcTraceLevel.TraceError, string.Format("RazerPreCreateQRPayment: Status code = {0}, Error Code = {1}", responseData.statusCode, responseData.errorCode), TraceCategory);
                }
            }
            catch (Exception ex)
            {

                Trace.WriteLineIf(SwcTraceLevel.TraceError, string.Format("RazerPreCreateQRPayment failed [ErrorCode = {0}]", ex.Message), TraceCategory);
            }

            Trace.WriteLineIf(SwcTraceLevel.TraceInfo, "RazerPreCreateQRPayment completed.", TraceCategory);
            return responseData;
        }

        private string GetHMACSHA256Hash(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return ToHexString(hashmessage);
            }
        }

        public string ToHexString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        public enum eRazerChannel
        {
            Any = 0,
            AliPay = 16,
            TouchNGoEWallet = 17,
            AlipayPreAuth = 18,
            Boost = 18,
            MaybankQRPay = 20,
            GrabPay = 21,
            UnionPay = 22,
            ShopeePay = 23,
            DuitNowQR = 24,
            AliPayPlus = 25,
            Atome = 26,
            WechatPayCN = 36,
            WechatPayMY = 37
        }

        public enum eAuthCodeType
        {
            Barcode = 1,
            QRCode = 2
        }

        public string AllProps(object o)
        {
            StringBuilder s = new StringBuilder("input=[");

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(o))
            {
                s.Append(descriptor.Name)
                    .Append(":")
                    .Append(descriptor.GetValue(o))
                    .Append(";");
            }
            return s.Append("]").ToString();
        }

        public string DownloadImage(string url, string referenceId)
        {
            string result = string.Empty;

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                using (var wc = new WebClient())
                {
                    using (var imgStream = new MemoryStream(wc.DownloadData(url)))
                    {
                        Bitmap bitmap = new Bitmap(imgStream);
                        string loc = @"D:\TempImage" + @"\" + referenceId + ".png";

                        bitmap.Save(loc, System.Drawing.Imaging.ImageFormat.Png);
                        bitmap.Dispose();
                        bitmap = null;

                        Trace.WriteLineIf(SwcTraceLevel.TraceInfo, string.Format("DownloadImage Location : {0}", loc), TraceCategory);
                        result = loc;
                    }
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(SwcTraceLevel.TraceError, string.Format("DownloadImage Failed Download : {0}", ex.ToString()), TraceCategory);
            }

            return result;
        }
    }

    public class RazerPaymentResponse
    {
        private string _applicationCode = string.Empty;
        private string _version = string.Empty;
        private string _referenceId = string.Empty;
        private int _authorizationCodeType = 0;
        private string _authorizationCode = string.Empty;
        private string _currencyCode = string.Empty;
        private decimal _amount = 0.00m;
        private string _molTransactionId = string.Empty;
        private string _payerId = string.Empty;
        private decimal _exchangeRate = 0.00m;
        private string _baseCurrencyCode = string.Empty;
        private string _channelId = string.Empty;
        private decimal _baseAmount = 0.00m;
        private string _statusCode = string.Empty;
        private string _errorCode = string.Empty;
        private DateTime _transactionDateTime;
        private string _hashType = string.Empty;
        private string _signature = string.Empty;

        [JsonProperty("applicationCode")]
        public string applicationCode
        {
            get { return _applicationCode; }
            set { _applicationCode = value; }
        }

        [JsonProperty("version")]
        public string version
        {
            get { return _version; }
            set { _version = value; }
        }

        [JsonProperty("referenceId")]
        public string referenceId
        {
            get { return _referenceId; }
            set { _referenceId = value; }
        }

        [JsonProperty("authorizationCodeType")]
        public int authorizationCodeType
        {
            get { return _authorizationCodeType; }
            set { _authorizationCodeType = value; }
        }

        [JsonProperty("authorizationCode")]
        public string authorizationCode
        {
            get { return _authorizationCode; }
            set { _authorizationCode = value; }
        }

        [JsonProperty("currencyCode")]
        public string currencyCode
        {
            get { return _currencyCode; }
            set { _currencyCode = value; }
        }

        [JsonProperty("amount")]
        public decimal amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        [JsonProperty("molTransactionId")]
        public string molTransactionId
        {
            get { return _molTransactionId; }
            set { _molTransactionId = value; }
        }

        [JsonProperty("payerId")]
        public string payerId
        {
            get { return _payerId; }
            set { _payerId = value; }
        }

        [JsonProperty("exchangeRate")]
        public decimal exchangeRate
        {
            get { return _exchangeRate; }
            set { _exchangeRate = value; }
        }

        [JsonProperty("baseCurrencyCode")]
        public string baseCurrencyCode
        {
            get { return _baseCurrencyCode; }
            set { _baseCurrencyCode = value; }
        }

        [JsonProperty("channelId")]
        public string channelId
        {
            get { return _channelId; }
            set { _channelId = value; }
        }

        [JsonProperty("baseAmount")]
        public decimal baseAmount
        {
            get { return _baseAmount; }
            set { _baseAmount = value; }
        }

        [JsonProperty("statusCode")]
        public string statusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        [JsonProperty("errorCode")]
        public string errorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }

        [JsonProperty("transactionDateTime")]
        public DateTime transactionDateTime
        {
            get { return _transactionDateTime; }
            set { _transactionDateTime = value; }
        }

        [JsonProperty("hashType")]
        public string hashType
        {
            get { return _hashType; }
            set { _hashType = value; }
        }

        [JsonProperty("signature")]
        public string signature
        {
            get { return _signature; }
            set { _signature = value; }
        }
    }

    public class RazerPreCreateQRResponse
    {
        private string _applicationCode = string.Empty;
        private string _version = string.Empty;
        private string _referenceId = string.Empty;
        private string _currencyCode = string.Empty;
        private decimal _amount = 0.00m;
        private string _molTransactionId = string.Empty;
        private string _channelId = string.Empty;
        private string _authorizationCode = string.Empty;
        private string _imageUrl = string.Empty;
        private string _imageUrlBig = string.Empty;
        private string _imageUrlSmall = string.Empty;
        private string _customImageUrl = string.Empty;
        private string _statusCode = string.Empty;
        private string _errorCode = string.Empty;
        private DateTime _transactionDateTime;
        private string _hashType = string.Empty;
        private string _signature = string.Empty;

        [JsonProperty("applicationCode")]
        public string applicationCode
        {
            get { return _applicationCode; }
            set { _applicationCode = value; }
        }

        [JsonProperty("version")]
        public string version
        {
            get { return _version; }
            set { _version = value; }
        }

        [JsonProperty("referenceId")]
        public string referenceId
        {
            get { return _referenceId; }
            set { _referenceId = value; }
        }

        [JsonProperty("authorizationCode")]
        public string authorizationCode
        {
            get { return _authorizationCode; }
            set { _authorizationCode = value; }
        }

        [JsonProperty("currencyCode")]
        public string currencyCode
        {
            get { return _currencyCode; }
            set { _currencyCode = value; }
        }

        [JsonProperty("amount")]
        public decimal amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        [JsonProperty("molTransactionId")]
        public string molTransactionId
        {
            get { return _molTransactionId; }
            set { _molTransactionId = value; }
        }

        [JsonProperty("imageUrl")]
        public string imageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }

        [JsonProperty("imageUrlBig")]
        public string imageUrlBig
        {
            get { return _imageUrlBig; }
            set { _imageUrlBig = value; }
        }

        [JsonProperty("imageUrlSmall")]
        public string imageUrlSmall
        {
            get { return _imageUrlSmall; }
            set { _imageUrlSmall = value; }
        }

        [JsonProperty("customImageUrl")]
        public string customImageUrl
        {
            get { return _customImageUrl; }
            set { _customImageUrl = value; }
        }

        [JsonProperty("channelId")]
        public string channelId
        {
            get { return _channelId; }
            set { _channelId = value; }
        }

        [JsonProperty("statusCode")]
        public string statusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        [JsonProperty("errorCode")]
        public string errorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }

        [JsonProperty("transactionDateTime")]
        public DateTime transactionDateTime
        {
            get { return _transactionDateTime; }
            set { _transactionDateTime = value; }
        }

        [JsonProperty("hashType")]
        public string hashType
        {
            get { return _hashType; }
            set { _hashType = value; }
        }

        [JsonProperty("signature")]
        public string signature
        {
            get { return _signature; }
            set { _signature = value; }
        }
    }

    public class RazerReversalVoidResponse
    {
        private string _applicationCode = string.Empty;
        private string _version = string.Empty;
        private string _referenceId = string.Empty;
        private string _paymentReferenceId = string.Empty;
        private string _channelId = string.Empty;
        private string _molTransactionId = string.Empty;
        private string _statusCode = string.Empty;
        private string _errorCode = string.Empty;
        private DateTime _transactionDateTime;
        private string _hashType = string.Empty;
        private string _signature = string.Empty;

        [JsonProperty("applicationCode")]
        public string applicationCode
        {
            get { return _applicationCode; }
            set { _applicationCode = value; }
        }

        [JsonProperty("version")]
        public string version
        {
            get { return _version; }
            set { _version = value; }
        }

        [JsonProperty("referenceId")]
        public string referenceId
        {
            get { return _referenceId; }
            set { _referenceId = value; }
        }

        [JsonProperty("paymentReferenceId")]
        public string paymentReferenceId
        {
            get { return _paymentReferenceId; }
            set { _paymentReferenceId = value; }
        }

        [JsonProperty("channelId")]
        public string channelId
        {
            get { return _channelId; }
            set { _channelId = value; }
        }

        [JsonProperty("molTransactionId")]
        public string molTransactionId
        {
            get { return _molTransactionId; }
            set { _molTransactionId = value; }
        }

        [JsonProperty("statusCode")]
        public string statusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        [JsonProperty("errorCode")]
        public string errorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }

        [JsonProperty("transactionDateTime")]
        public DateTime transactionDateTime
        {
            get { return _transactionDateTime; }
            set { _transactionDateTime = value; }
        }

        [JsonProperty("hashType")]
        public string hashType
        {
            get { return _hashType; }
            set { _hashType = value; }
        }

        [JsonProperty("signature")]
        public string signature
        {
            get { return _signature; }
            set { _signature = value; }
        }
    }

    public class RazerRefundResponse
    {
        private string _applicationCode = string.Empty;
        private string _version = string.Empty;
        private string _referenceId = string.Empty;
        private string _paymentReferenceId = string.Empty;
        private string _currencyCode = string.Empty;
        private decimal _amount = 0.00m;
        private string _channelId = string.Empty;
        private string _molTransactionId = string.Empty;
        private string _payerId = string.Empty;
        private decimal _exchangeRate = 0.00m;
        private string _baseCurrencyCode = string.Empty;
        private decimal _baseAmount = 0.00m;
        private string _statusCode = string.Empty;
        private string _errorCode = string.Empty;
        private DateTime _transactionDateTime;
        private string _hashType = string.Empty;
        private string _signature = string.Empty;

        [JsonProperty("applicationCode")]
        public string applicationCode
        {
            get { return _applicationCode; }
            set { _applicationCode = value; }
        }

        [JsonProperty("version")]
        public string version
        {
            get { return _version; }
            set { _version = value; }
        }

        [JsonProperty("referenceId")]
        public string referenceId
        {
            get { return _referenceId; }
            set { _referenceId = value; }
        }

        [JsonProperty("paymentReferenceId")]
        public string paymentReferenceId
        {
            get { return _paymentReferenceId; }
            set { _paymentReferenceId = value; }
        }

        [JsonProperty("currencyCode")]
        public string currencyCode
        {
            get { return _currencyCode; }
            set { _currencyCode = value; }
        }

        [JsonProperty("amount")]
        public decimal amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        [JsonProperty("molTransactionId")]
        public string molTransactionId
        {
            get { return _molTransactionId; }
            set { _molTransactionId = value; }
        }

        [JsonProperty("payerId")]
        public string payerId
        {
            get { return _payerId; }
            set { _payerId = value; }
        }

        [JsonProperty("exchangeRate")]
        public decimal exchangeRate
        {
            get { return _exchangeRate; }
            set { _exchangeRate = value; }
        }

        [JsonProperty("baseCurrencyCode")]
        public string baseCurrencyCode
        {
            get { return _baseCurrencyCode; }
            set { _baseCurrencyCode = value; }
        }

        [JsonProperty("channelId")]
        public string channelId
        {
            get { return _channelId; }
            set { _channelId = value; }
        }

        [JsonProperty("baseAmount")]
        public decimal baseAmount
        {
            get { return _baseAmount; }
            set { _baseAmount = value; }
        }

        [JsonProperty("statusCode")]
        public string statusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        [JsonProperty("errorCode")]
        public string errorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }

        [JsonProperty("transactionDateTime")]
        public DateTime transactionDateTime
        {
            get { return _transactionDateTime; }
            set { _transactionDateTime = value; }
        }

        [JsonProperty("hashType")]
        public string hashType
        {
            get { return _hashType; }
            set { _hashType = value; }
        }

        [JsonProperty("signature")]
        public string signature
        {
            get { return _signature; }
            set { _signature = value; }
        }
    }
}
