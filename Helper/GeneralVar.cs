using Helper;
using LFFSSK.Model;
using LFFSSK.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using XFDevice.BarcodeScanner.BCTG321UVCP;

namespace LFFSSK
{
    public class GeneralVar
    {
        #region Trace

        public static TraceSwitch SwcTraceLevel = new TraceSwitch("SwcTraceLevel", "Trace Switch Level");
        
        #endregion

        #region Main View or ViewModel

        static MainWindow _MainWindowVW;
        public static MainWindow MainWindowVW
        {
            get { return _MainWindowVW; }
            set { _MainWindowVW = value; }
        }

        static MainWindowViewModel _MainWindowVM;
        public static MainWindowViewModel MainWindowVM
        {
            get { return _MainWindowVM; }
            set { _MainWindowVM = value; }
        }


        #endregion

        private static UserLoginModel _User;
        public static UserLoginModel User
        {
            get { return _User; }
            set { _User = value; }
        }

        public static User user;

        
        public static string MediaFileRepository = ConfigurationManager.AppSettings["MediaFileRepository"];
        public static string MenuRepository = ConfigurationManager.AppSettings["MenuImgRepository"];


        public static ePrinterModel PrinterModel = ePrinterModel.Fujitsu;

        //Kiosk Info
        public static string ComponentCode = ConfigurationManager.AppSettings["KioskCode"].ToString();

        public static string BranchId = ConfigurationManager.AppSettings["BranchId"].ToUpper();
        //Environment.MachineName; //"PRDM-SSK-01"; //"QSR-SSK-01"; //

        public static int ComponentId = Convert.ToInt32(ConfigurationManager.AppSettings["ComponentId"]);
        public static string ComponentUniqueId = ConfigurationManager.AppSettings["ComponentUniqueId"].ToLower();

        public static bool IsScreenMaximized = ConfigurationManager.AppSettings["Display_ScreenMaximized"].ToLower() == "true";
        public static bool Display_WindowStyle = ConfigurationManager.AppSettings["Display_WindowStyle"].ToLower() == "true";
        public static bool IsTopMost = ConfigurationManager.AppSettings["Display_TopMost"].ToLower() == "true";
        public static bool IsHideCursor = ConfigurationManager.AppSettings["Display_HideCursor"].ToLower() == "true";

        static string[] MainScreenResolution = ConfigurationManager.AppSettings["Display_ScreenResolution"].ToLower().Split('x');
        public static double MainWidth = Convert.ToDouble(MainScreenResolution[0]);
        public static double MainHeight = Convert.ToDouble(MainScreenResolution[1]);

        public static bool IsTableTopSSK;
        public static bool TestVoucher;

        public static string Poster_Path { get; set; }
        public static string ReceiptBackupPath = ConfigurationManager.AppSettings["ReceiptBackupPath"];
        public static string ApplicationMode { get; set; }

        public static string Cuscapi_APIAddress { get; set; }
        public static string IPOSPath = System.Configuration.ConfigurationManager.AppSettings["IPOSPath"].ToString();
        public static Dictionary<string, int> PaymentCategory;

        public static bool Monitoring_Enabled { get; set; }

        public static byte IOBoard_ModuleAddress = Convert.ToByte("1");

        public static string regionName;

        public static ComponentModel mComponent;
        public static List<BannerMedias> mBannerMedias;
        public static List<string> mVoucherPrefix;


        static ObservableCollection<InitModel> _Checklist;
        public static ObservableCollection<InitModel> Checklist
        {
            get { return _Checklist; }
            set { _Checklist = value; }
        }

        public static bool TemporaryMode = ConfigurationManager.AppSettings["TemporaryMode"].ToLower() == "true";
        public static bool TestingMode = ConfigurationManager.AppSettings["TestingMode"].ToLower() == "true";
        public static bool LiveAPI = ConfigurationManager.AppSettings["LiveAPI"].ToLower() == "true";
        public static bool TestPrinterMode = ConfigurationManager.AppSettings["TestPrinterMode"].ToLower() == "true";
        public static bool Printer_Enabled = ConfigurationManager.AppSettings["Printer_Enabled"].ToLower() == "true";
        public static bool CC_Enabled = ConfigurationManager.AppSettings["CC_Enabled"].ToLower() == "true";
        public static string CC_PortName = ConfigurationManager.AppSettings["CC_PortName"].ToUpper().Trim();
        //public static string CC_SettlementTime = ConfigurationManager.AppSettings["CC_SettlementTime"];
        public static string CC_SettlementTime { get; set; }
        public static string Razer_ApplicationCode = ConfigurationManager.AppSettings["Razer_ApplicationCode"].Trim();
        public static string Razer_SecretKey = ConfigurationManager.AppSettings["Razer_SecretKey"].Trim();

        public static DateTime OpeningTime = Convert.ToDateTime(System.Configuration.ConfigurationManager.AppSettings["OpeningTime"].ToString());
        public static DateTime ClosingTime = Convert.ToDateTime(System.Configuration.ConfigurationManager.AppSettings["ClosingTime"].ToString());
        
        //public static DateTime OpeningTime { get; set; }
        //public static DateTime ClosingTime { get; set; }

        public static bool GA_Enabled { get; set; }
        public static bool Email_Enabled { get; set; }
        public static bool ServeDeck_Enabled { get; set; }
        public static bool TakeAwayOnly { get; set; }
        public static bool CashOption_Enabled { get; set; }
        public static bool EVoucher_Enabled { get; set; }
        public static bool NewJson_Enabled { get; set; }
        public static bool SetBreakfast_NotAvailable { get; set; }
        public static bool NewIPOS_Enabled { get; set; }
        public static string Mpay_TID { get; set; }

        public static int AlohaTerminalId { get; set; }
        public static int AlohaStaffId { get; set; }
        public static int AlohaPOSQueueId { get; set; }

        public static int AlohaJobScope = 16;
        public static int AlohaGuestCount = 1;
        //public static int AlohaQueuId = 10;

        public static bool IOBoard_Enabled = ConfigurationManager.AppSettings["IOBoard_Enabled"].ToLower() == "true";
        public static string IOBoard_DisplayName = "IO Board";
        public static string IOBoard_PortName = ConfigurationManager.AppSettings["IOBoard_PortName"];
        public static int IOBoard_BaudRate = 9600;
        public static Parity IOBoard_Parity = Parity.None;
        public static StopBits IOBoard_StopBit = StopBits.One;
        public static int IOBoard_DataBit = 8;

        static string[] IOBoard_DI_DoorSensor = ConfigurationManager.AppSettings["IOBoard_DI_DoorSensor"].Split('|');
        public static bool IOBoard_DI_DoorSensor_Enabled = IOBoard_DI_DoorSensor[0].ToLower() == "true";
        public static int IOBoard_DI_DoorSensor_Channel = Convert.ToInt32(IOBoard_DI_DoorSensor[1]);

        static string[] IOBoard_DI_PanicButton = ConfigurationManager.AppSettings["IOBoard_DI_PanicButton"].Split('|');
        public static bool IOBoard_DI_PanicButton_Enabled = IOBoard_DI_PanicButton[0].ToLower() == "true";
        public static int IOBoard_DI_PanicButton_Channel = Convert.ToInt32(IOBoard_DI_PanicButton[1]);

        static string[] IOBoard_DO_Alarm = ConfigurationManager.AppSettings["IOBoard_DO_Alarm"].Split('|');
        public static bool IOBoard_DO_Alarm_Enabled = IOBoard_DO_Alarm[0].ToLower() == "true";
        public static int IOBoard_DO_Alarm_Channel = Convert.ToInt32(IOBoard_DO_Alarm[1]);

        static string[] IOBoard_DO_QR = ConfigurationManager.AppSettings["IOBoard_DO_QR"].Split('|');
        public static bool IOBoard_DO_QR_Enabled = IOBoard_DO_QR[0].ToLower() == "true";
        public static int IOBoard_DO_QR_Channel = Convert.ToInt32(IOBoard_DO_QR[1]);

        static string[] IOBoard_DO_CC = ConfigurationManager.AppSettings["IOBoard_DO_CC"].Split('|');
        public static bool IOBoard_DO_CC_Enabled = IOBoard_DO_CC[0].ToLower() == "true";
        public static int IOBoard_DO_CC_Channel = Convert.ToInt32(IOBoard_DO_CC[1]);

        static string[] IOBoard_DO_RP = ConfigurationManager.AppSettings["IOBoard_DO_RP"].Split('|');
        public static bool IOBoard_DO_RP_Enabled = IOBoard_DO_RP[0].ToLower() == "true";
        public static int IOBoard_DO_RP_Channel = Convert.ToInt32(IOBoard_DO_RP[1]);

        static string[] IOBoard_DO_Light = ConfigurationManager.AppSettings["IOBoard_DO_Light"].Split('|');
        public static bool IOBoard_DO_Light_Enabled = IOBoard_DO_Light[0].ToLower() == "true";
        public static int IOBoard_DO_Light_Channel = Convert.ToInt32(IOBoard_DO_Light[1]);

        public static IPAddress Monitoring_IPAddress { get; set; }
        public static int Monitoring_Port { get; set; }
        public static int Monitoring_SetStatusInterval { get; set; }
        public static int Monitoring_CheckCommandInterval { get; set; }

        public static ClientMonitoringController ClientMonitoringController = new ClientMonitoringController();

        public static bool NoOffOperation { get; set; }
        public static DateTime SettlementTime { get; set; }

        public static string ReceiptPrinter_Port = System.Configuration.ConfigurationManager.AppSettings["ReceiptPrinter_Port"].ToString();

        public static IOHelper IOBoard;
        public static DocumentPrintHelper DocumentPrint;

        public static string getWifi()
        {
            string wifi = string.Empty;
            string f1 = DateTime.Now.Month.ToString("D2");
            string f3 = DateTime.Now.Day.ToString("D2");
            int f2 = DateTime.Now.Day * DateTime.Now.Day + DateTime.Now.Month;
            wifi = string.Format("{0}{1}{2}{3}{4}", f1[1], f1[0], f2, f3[1], f3[0]);

            return wifi;
        }

        public static string getAlohaWifi(int queueNumber)
        {
            string wifi = string.Empty;
            int year = Convert.ToInt32(DateTime.Now.Year.ToString());
            int month = Convert.ToInt32(DateTime.Now.Month.ToString());
            int day = Convert.ToInt32(DateTime.Now.Day.ToString());
            int sumOfDate = year + month + (day * day);
            string sumOfDateHex = sumOfDate.ToString("X");

            int storeId = Convert.ToInt32(GeneralVar.mComponent.StoreId);
            string storeIdHex = storeId.ToString("X");

            string queueNumberHex = queueNumber.ToString("X3");
            wifi = string.Format("{0}-{1}-{2}", sumOfDateHex, storeIdHex, queueNumberHex);
            return wifi; 
        }

        public static string getSurveyCode(string queueNumber, string storeId)
        {
            string surveyCode, combinedStoreId, dateTimeString = string.Empty;
            
            if (!string.IsNullOrEmpty(storeId))
                combinedStoreId = "101" + storeId;
            else
                combinedStoreId = "1010000";

            dateTimeString = DateTime.Now.ToString("ddMMyyHHmm");
            surveyCode = String.Format("{0}-{1}{2}", combinedStoreId, dateTimeString, queueNumber);
       
            return surveyCode;
        }

        public static string generateSurveyUrl(string queueNumber, string storeId, eDiningMethod diningMethod)
        {
            string url = string.Empty;
            string storeCodeId = string.Empty;
            string connectionPath = "https://u.kfcvisit.com/MYS";
            try
            {
                List<string> specialStoreId = new List<string>() { "0064", "0038", "0072", "0031", "0013" };
                if (specialStoreId.Any(y => y == storeId))
                    storeCodeId = "100" + storeId;
                else
                    storeCodeId = "101" + storeId;

                Dictionary<string, object> surveyField = new Dictionary<string, object>();
                surveyField.Add("S", storeCodeId);
                surveyField.Add("D", DateTime.Now.ToString("ddMMyy"));
                surveyField.Add("T", DateTime.Now.ToString("HHmm"));
                surveyField.Add("U", queueNumber);
                surveyField.Add("V", diningMethod == eDiningMethod.DineIn ? "1" : "2");
                surveyField.Add("O", "3"); // 3 is kiosk, 2 is online
                surveyField.Add("Source", "QR");

                string param = "?";
                if (surveyField != null)
                {
                    foreach (KeyValuePair<string, object> parameter in surveyField)
                    {
                        param += String.Format("{0}={1}&", parameter.Key, parameter.Value);
                    }
                }
                param = param.Remove(param.Length - 1);

                url = connectionPath + param;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("generateSurveyUrl URL = {0}", url), "GeneralVar");
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceVerbose, string.Format("[Error] generateSurveyUrl = {0}", ex.ToString()), "GeneralVar");
            }
            return url;
        }

        #region API
        public static string ApiURL = ConfigurationManager.AppSettings["ApiURL"];
        public static string ApiKey = ConfigurationManager.AppSettings["x-api-key"];

        public static string LocationID = ConfigurationManager.AppSettings["LocationID"];
        public static string MenuID = ConfigurationManager.AppSettings["MenuID"];

        public static string AppID = ConfigurationManager.AppSettings["AppID"];
        public static string token = ConfigurationManager.AppSettings["token"];
        public static string auth = ConfigurationManager.AppSettings["auth"];
        public static string Source = ConfigurationManager.AppSettings["Source"];

        #endregion

        #region QR Info
        #region QR Reader
        public static BCTG321UVCPHelper EntryQRHelper;
        public static bool QR_Enabled = ConfigurationManager.AppSettings["QR_Enabled"].ToLower() == "true" ? true : false;
        public static string QR_PortName = ConfigurationManager.AppSettings["QR_PortName"].ToUpper().Trim();

        private static string _QRLastErrorMsg;

        public static string QRLastErrorMsg
        {
            get { return _QRLastErrorMsg; }
            set { _QRLastErrorMsg = value; }
        }
        #endregion
        #endregion

        #region SQL Connection

        private static Dictionary<string, Transaction> _lastTx;
        public static Dictionary<string, Transaction> LastTx
        {
            get { return _lastTx; }
            set { _lastTx = value; }
        }

        #endregion

        #region Air Selangor

        private static IEnumerable<PaymentType> _db_PaymentTypes;
        public static IEnumerable<PaymentType> DB_PaymentTypes
        {
            get { return _db_PaymentTypes; }
            set { _db_PaymentTypes = value; }
        }

        private static IEnumerable<BillType> _db_BillType;
        public static IEnumerable<BillType> DB_BillType
        {
            get { return _db_BillType; }
            set { _db_BillType = value; }
        }

        #endregion
    }
}
