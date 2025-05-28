using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using LFFSSK.View;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.ObjectModel;
using LFFSSK.Model;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Xml;
using MPAY;
using MPAY.Model.Response;
using System.Net;
using System.Reflection;
using System.Net.Mail;
using System.Threading.Tasks;
using GoogleAnalytic;
using System.Windows.Media;
using System.Configuration;
using System.Net.Sockets;
using System.Globalization;
using MpayIntegration;
using System.Windows.Media.Animation;
using LFFSSK.Helper;
using System.Text.RegularExpressions;
using Helper;
using LFFSSK.API;
using EDC_Serial_Lib.MessageResp;
using EDC_Serial_Lib.MessageType;
using static LFFSSK.Helper.RazerPayIntegration;
using XFUtility.Barcode;

namespace LFFSSK.ViewModel
{
    public class MainWindowViewModel : Base
    {
        #region Field

        //test
        string _TraceCategory = "LFFSSK";
        bool _TestingMode = false;
        ObservableCollection<TimePicker> _TimeList;
        TimePicker _SelectedTime;
        int maintenanceClickTimeout = 0;
        int maintenanceClickAttempt = 0;
        int InitClickTimeout = 0;
        int InitClickAttempt = 0;
        int RefreshClickTimeout = 0;
        int RefreshClickAttempt = 0;
        int OfflineClickTimeout = 0;
        int OfflineClickAttempt = 0;
        int OffClickTimeout = 0;
        int OffClickAttempt = 0;
        public int comboCount = 0;
        public int alaCarteCount = 0;
        decimal _TaxAmount;
        int _TimeOutSecond = 180;
        string _ComponentCode;
        ObservableCollection<InitModel> _Checklist;
        ObservableCollection<ReprintSelectionHelper> _ReprintCollection;
        MResponse _mResponse;
        MResponse mpayResponseForPrinting;
        string _BarcodeInfo;
        BackgroundWorker bwSystemCheck;
        public bool isSystemChecking = false;
        Visibility _ShowCancel = Visibility.Collapsed;
        Visibility _ShowBack = Visibility.Collapsed;
        Visibility _ShowLoading = Visibility.Collapsed;
        Visibility _ShowCancelQR = Visibility.Collapsed;
        Visibility _ShowOOOErrorMessage = Visibility.Collapsed;
        Visibility _ShowTimeOut = Visibility.Collapsed;
        Visibility _ShowMaintenance = Visibility.Visible;
        Visibility _ShowPaperEnd = Visibility.Collapsed;
        Visibility _ShowUAT = Visibility.Collapsed;
        Visibility _ShowMorning = Visibility.Visible;
        Visibility _ShowNormalHour = Visibility.Visible;
        Visibility _ShowMidnight = Visibility.Visible;
        Visibility _ShowSaveChange = Visibility.Collapsed;
        Visibility _ShowUpdateStatus = Visibility.Collapsed;
        Visibility _ShowBannerMedias = Visibility.Collapsed;
        bool _isMenuExclusion = false;
        bool _UnderMaintenance = false;
        public string QRCodeReceipt = string.Empty;

        string _CheckNumber;
        ePaymentMethod _PaymentMethod;

        string _PaymentCategoryMessage = string.Empty;
        string packageId = string.Empty;

        string _ReferenceNo;
        int txid = 0;
        string uniqueTranNumber;
        public int todayDay = 0;

        UserControl _DesignView;
        UserControl _BannerView;
        UserControl _DetailView;
        bool _IsEN;
        bool _IsBM;
        int _Label_NumberOfOrder;

        int _CurrentIndex = 0;
        bool _Label_Completed_Enabled = false;
        bool _EnableAddBag = false;
        //public bool _IsGoLarged = false;
        TimeSpan bfStart = new TimeSpan(4, 0, 0);
        TimeSpan bfEnd = new TimeSpan(9, 59, 59);
        //public List<int> cluster1Store = new List<int>() { 145, 102, 31, 20, 7, 13, 12, 11, 6 };

        string _SystemError;
        string _UpdateStatus;

        DateTime _StartTime = DateTime.Now;
        DateTime _EndTime = DateTime.Now;
        DateTime requestTime = DateTime.MinValue;
        DateTime responseTime = DateTime.MinValue;
        DateTime paymentRequestTime = DateTime.MinValue;
        DateTime paymentResponseTime = DateTime.MinValue;

        ICommand _LanguageCommand, _PaymentSelectionCommand, _TimeOutCommand, _ReprintCommand, _EasyMaintenanceCommand;


        AutoResetEvent _WaitDone = new AutoResetEvent(false);
        AutoResetEvent _WaitDoneVoucher = new AutoResetEvent(false);
        AutoResetEvent _WaitFailed = new AutoResetEvent(false);



        AutoResetEvent waitProceed = new AutoResetEvent(false);

        decimal _TotalAmount = 0.00m;
        decimal _TotalAmountDeductDonation = 0.00m;
        decimal _TotalAmountBeforeRounding = 0.00m;
        decimal _TotalAmountBeforeVoucher = 0.00m;
        decimal _TotalVoucherAmount = 0.00m;
        decimal _VoucherAmount = 0.00m;
        decimal _AmountBeforeTax = 0.00m;
        decimal _SubAmount = 0.00m;
        public int totalDonationCount = 0;
        string _UiCulture;

        TimeSpan StartCheckHour = new TimeSpan(4, 0, 0);
        TimeSpan EndCheckHour = new TimeSpan(10, 0, 0);
        TimeSpan CheatHour = new TimeSpan(10, 30, 0);

        ApiFunc _ApiFunc = null;

        bool readyToScan = false;
        #endregion

        #region Properties

        public bool TestingMode
        {
            get { return _TestingMode; }
            set
            {
                _TestingMode = value;
                OnPropertyChanged("TestingMode");
            }
        }

        public ObservableCollection<TimePicker> TimeList
        {
            get { return _TimeList; }
            set
            {
                _TimeList = value;
                OnPropertyChanged("TimeList");
            }
        }

        public TimePicker SelectedTime
        {
            get { return _SelectedTime; }
            set
            {
                _SelectedTime = value;
                OnPropertyChanged("SelectedTime");
            }
        }

        public List<TimePicker> TimeListl = new List<TimePicker>()
        {
            new TimePicker() { sTime = "00:00", dTime = new DateTime(2020, 02, 11, 00, 00, 00) },
            new TimePicker() { sTime = "01:00", dTime = new DateTime(2020, 02, 11, 01, 00, 00) },
            new TimePicker() { sTime = "02:00", dTime = new DateTime(2020, 02, 11, 02, 00, 00) },
            new TimePicker() { sTime = "03:00", dTime = new DateTime(2020, 02, 11, 03, 00, 00) },
            new TimePicker() { sTime = "04:00", dTime = new DateTime(2020, 02, 11, 04, 00, 00) },
            new TimePicker() { sTime = "05:00", dTime = new DateTime(2020, 02, 11, 05, 00, 00) },
            new TimePicker() { sTime = "06:00", dTime = new DateTime(2020, 02, 11, 06, 00, 00) },
            new TimePicker() { sTime = "07:00", dTime = new DateTime(2020, 02, 11, 07, 00, 00) },
            new TimePicker() { sTime = "08:00", dTime = new DateTime(2020, 02, 11, 08, 00, 00) },
            new TimePicker() { sTime = "09:00", dTime = new DateTime(2020, 02, 11, 09, 00, 00) },
            new TimePicker() { sTime = "10:00", dTime = new DateTime(2020, 02, 11, 10, 00, 00) },
            new TimePicker() { sTime = "11:00", dTime = new DateTime(2020, 02, 11, 11, 00, 00) },
            new TimePicker() { sTime = "12:00", dTime = new DateTime(2020, 02, 11, 12, 00, 00) },
            new TimePicker() { sTime = "13:00", dTime = new DateTime(2020, 02, 11, 13, 00, 00) },
            new TimePicker() { sTime = "14:00", dTime = new DateTime(2020, 02, 11, 14, 00, 00) },
            new TimePicker() { sTime = "15:00", dTime = new DateTime(2020, 02, 11, 15, 00, 00) },
            new TimePicker() { sTime = "16:00", dTime = new DateTime(2020, 02, 11, 16, 00, 00) },
            new TimePicker() { sTime = "17:00", dTime = new DateTime(2020, 02, 11, 17, 00, 00) },
            new TimePicker() { sTime = "18:00", dTime = new DateTime(2020, 02, 11, 18, 00, 00) },
            new TimePicker() { sTime = "19:00", dTime = new DateTime(2020, 02, 11, 19, 00, 00) },
            new TimePicker() { sTime = "20:00", dTime = new DateTime(2020, 02, 11, 20, 00, 00) },
            new TimePicker() { sTime = "21:00", dTime = new DateTime(2020, 02, 11, 21, 00, 00) },
            new TimePicker() { sTime = "22:00", dTime = new DateTime(2020, 02, 11, 22, 00, 00) },
            new TimePicker() { sTime = "23:00", dTime = new DateTime(2020, 02, 11, 23, 00, 00) }
        };

        public bool UnderMaintenance
        {
            get { return _UnderMaintenance; }
            set
            {
                _UnderMaintenance = value;
                OnPropertyChanged("UnderMaintenance");
                OnPropertyChanged(nameof(OperationModeColour));

            }
        }

        public decimal TaxAmount
        {
            get { return _TaxAmount; }
            set
            {
                _TaxAmount = value;
                OnPropertyChanged("TaxAmount");
            }
        }

        public string ComponentCode
        {
            get { return _ComponentCode; }
            set
            {
                _ComponentCode = value;
                OnPropertyChanged("ComponentCode");
            }

        }

        public string SystemError
        {
            get { return _SystemError; }
            set
            {
                _SystemError = value;
                OnPropertyChanged("SystemError");
            }
        }

        public string UpdateStatus
        {
            get { return _UpdateStatus; }
            set
            {
                _UpdateStatus = value;
                OnPropertyChanged("UpdateStatus");
            }
        }

        public ePaymentMethod PaymentMethod // For UI Usage purpose
        {
            get { return _PaymentMethod; }
            set
            {
                _PaymentMethod = value;
                OnPropertyChanged("PaymentMethod");
            }
        }

        public string CheckNumber
        {
            get { return _CheckNumber; }
            set
            {
                _CheckNumber = value;
            }
        }

        public int CurrentIndex
        {
            get { return _CurrentIndex; }
            set
            {
                _CurrentIndex = value;
                OnPropertyChanged("CurrentIndex");
            }
        }

        public decimal TotalAmount
        {
            get { return Math.Round(_TotalAmount * 20) / 20; }
            set
            {
                _TotalAmount = value;
                OnPropertyChanged("TotalAmount");
            }
        }

        public decimal TotalAmountDeductDonation
        {
            get { return _TotalAmountDeductDonation; }
            set { _TotalAmountDeductDonation = value; }
        }

        public decimal TotalAmountBeforeRounding
        {
            get { return _TotalAmountBeforeRounding; }
            set { _TotalAmountBeforeRounding = value; }
        }

        public decimal TotalAmountBeforeVoucher
        {
            get { return Math.Round(_TotalAmountBeforeVoucher * 20) / 20; }
            set { _TotalAmountBeforeVoucher = value; }
        }

        public decimal TotalVoucherAmount
        {
            get
            {
                return _TotalVoucherAmount;
            }
            set { _TotalVoucherAmount = value; }
        }

        public decimal VoucherAmount
        {
            get { return _VoucherAmount; }
            set
            {
                _VoucherAmount = value;
                OnPropertyChanged("VoucherAmount");
            }
        }

        public decimal SubAmount
        {
            get { return _SubAmount; }
            set
            {
                _SubAmount = value;
                OnPropertyChanged("SubAmount");
            }
        }

        public decimal AmountBeforeTax
        {
            get { return _AmountBeforeTax; }
            set { _AmountBeforeTax = value; }
        }

        public ObservableCollection<InitModel> Checklist
        {
            get { return _Checklist; }
            private set
            {
                if (value != _Checklist)
                {
                    _Checklist = value;
                    OnPropertyChanged("Checklist");
                }
            }
        }

        public ObservableCollection<ReprintSelectionHelper> ReprintCollection
        {
            get { return _ReprintCollection; }
            private set
            {
                if (value != _ReprintCollection)
                {
                    _ReprintCollection = value;
                    OnPropertyChanged("ReprintCollection");
                }
            }
        }

        eStage _Stage = eStage.None;
        public eStage Stage
        {
            get { return _Stage; }
            set
            {
                _Stage = value;
                OnPropertyChanged("Stage");
                OnPropertyChanged(nameof(BackgroundPath));
            }
        }

        public Dictionary<string, string> TerminalResult = new Dictionary<string, string>()
        {
            {"0", "-"},
            {"1", "Pin Verified Success, No Signature Required"},
            {"2", "Signature Required"},
            {"3", "No Signature Required"}
        };

        public UserControl DesignView
        {
            get { return _DesignView; }
            set
            {
                if (value == _DesignView)
                    return;
                _DesignView = value;
                OnPropertyChanged("DesignView");
            }
        }

        public UserControl DetailView
        {
            get { return _DetailView; }
            set
            {
                if (value == _DetailView)
                    return;
                _DetailView = value;
                OnPropertyChanged("DetailView");
            }
        }

        #region Visibility

        public Visibility ShowMaintenance
        {
            get { return _ShowMaintenance; }
            set
            {
                _ShowMaintenance = value;
                OnPropertyChanged("ShowMaintenance");
            }
        }

        public Visibility ShowPaperEnd
        {
            get { return _ShowPaperEnd; }
            set
            {
                _ShowPaperEnd = value;
                OnPropertyChanged("ShowPaperEnd");
            }
        }

        public Visibility ShowTimeOut
        {
            get { return _ShowTimeOut; }
            set
            {
                _ShowTimeOut = value;
                OnPropertyChanged("ShowTimeOut");
            }
        }

        public Visibility ShowOOOErrorMessage
        {
            get { return _ShowOOOErrorMessage; }
            set
            {
                _ShowOOOErrorMessage = value;
                OnPropertyChanged("ShowOOOErrorMessage");
            }
        }

        public Visibility ShowUpdateStatus
        {
            get { return _ShowUpdateStatus; }
            set
            {
                _ShowUpdateStatus = value;
                OnPropertyChanged("ShowUpdateStatus");
            }
        }

        public Visibility ShowCancelQR
        {
            get { return _ShowCancelQR; }
            set
            {
                _ShowCancelQR = value;
                OnPropertyChanged("ShowCancelQR");
            }
        }

        public Visibility ShowBack
        {
            get { return _ShowBack; }
            set
            {
                _ShowBack = value;
                OnPropertyChanged("ShowBack");
            }
        }

        public Visibility ShowLoading
        {
            get { return _ShowLoading; }
            set
            {
                _ShowLoading = value;
                OnPropertyChanged("ShowLoading");
            }
        }
        public Visibility ShowUAT
        {
            get { return _ShowUAT; }
            set
            {
                _ShowUAT = value;
                OnPropertyChanged("ShowUAT");
            }
        }

        public Visibility ShowBannerMedias
        {
            get { return _ShowBannerMedias; }
            set
            {
                _ShowBannerMedias = value;
                OnPropertyChanged("ShowBannerMedias");
            }
        }

        private Visibility _DetailsViewVisbility;

        public Visibility DetailsViewVisbility
        {
            get { return _DetailsViewVisbility; }
            set
            {
                _DetailsViewVisbility = value;
                OnPropertyChanged(nameof(DetailsViewVisbility));
            }
        }

        #endregion

        #region Label

        #endregion

        private Brush _BackgroundColor = new SolidColorBrush(Colors.White);
        public Brush BackgroundColor
        {
            get
            {
                return _BackgroundColor;
            }
            set
            {
                if (value == _BackgroundColor)
                    return;

                _BackgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }

        public bool EnableAddBag
        {
            get { return _EnableAddBag; }
            set
            {
                _EnableAddBag = value;
                OnPropertyChanged("EnableAddBag");
            }
        }

        public bool IsEN
        {
            get { return _IsEN; }
            set
            {
                _IsEN = value;
                OnPropertyChanged("IsEN");
            }
        }

        public bool IsBM
        {
            get { return _IsBM; }
            set
            {
                _IsBM = value;
                OnPropertyChanged("IsBM");
            }
        }

        public string BarcodeInfo
        {
            get { return _BarcodeInfo; }
            set
            {
                _BarcodeInfo = value;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("BarcodeInfo = {0}", _BarcodeInfo), _TraceCategory);
                if (_BarcodeInfo.EndsWith("\r\n"))
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("BarcodeInfo = {0}", _BarcodeInfo), _TraceCategory);

                    _BarcodeInfo = string.Empty;
                }

                OnPropertyChanged("BarcodeInfo");
            }
        }


        public int Label_NumberOfOrder
        {
            get { return _Label_NumberOfOrder; }
            set
            {
                _Label_NumberOfOrder = value;
                OnPropertyChanged("Label_NumberOfOrder");
            }
        }

        //private List<Kiosk.KioskPaymentType> _KioskPaymentType;

        //public List<Kiosk.KioskPaymentType> KioskPaymentType
        //{
        //    get { return _KioskPaymentType; }
        //    set
        //    {
        //        _KioskPaymentType = value;
        //        OnPropertyChanged(nameof(PaymentType));
        //    }
        //}

        private ObservableCollection<CartModel.Product> _CartList;

        public ObservableCollection<CartModel.Product> CartList
        {
            get { return _CartList; }
            set
            {
                _CartList = value;
                OnPropertyChanged(nameof(CartList));
            }
        }

        private ObservableCollection<CartModel.Product> _TempCartList;

        public ObservableCollection<CartModel.Product> TempCartList
        {
            get { return _TempCartList; }
            set
            {
                _TempCartList = value;
                OnPropertyChanged(nameof(TempCartList));
            }
        }


        private ObservableCollection<CartModel.Product> _TempEditItem;

        public ObservableCollection<CartModel.Product> TempEditItem
        {
            get { return _TempEditItem; }
            set
            {
                _TempEditItem = value;
                OnPropertyChanged(nameof(TempEditItem));
            }
        }

        private ObservableCollection<ApiModel.GetMenu.Response.Category> _CateDetails;

        public ObservableCollection<ApiModel.GetMenu.Response.Category> CateDetails
        {
            get { return _CateDetails; }
            set
            {
                _CateDetails = value;
                OnPropertyChanged(nameof(CateDetails));
            }
        }

        private ObservableCollection<ApiModel.GetMenu.Response.Product> _Menu;

        public ObservableCollection<ApiModel.GetMenu.Response.Product> Menu
        {
            get { return _Menu; }
            set
            {
                _Menu = value;
                OnPropertyChanged(nameof(Menu));
            }
        }

        private int _CartItem;

        public int CartItem
        {
            get { return _CartItem; }
            set
            {
                _CartItem = value;
                OnPropertyChanged(nameof(CartItem));
                OnPropertyChanged(nameof(CartImage));
                OnPropertyChanged(nameof(CartAmountColor));
                OnPropertyChanged(nameof(CheckOutEnable));
                OnPropertyChanged(nameof(ViewOrderEnable));
            }
        }

        private SolidColorBrush _CartAmountColor;

        public SolidColorBrush CartAmountColor
        {
            get
            {
                if (CartItem > 0)
                    _CartAmountColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FBF7EB"));
                else
                    _CartAmountColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3D160E"));

                return _CartAmountColor;
            }
            set
            {
                _CartAmountColor = value;
                OnPropertyChanged(nameof(CartAmountColor));
            }
        }

        private bool _CheckOutEnable;

        public bool CheckOutEnable
        {
            get
            {
                if (CartItem > 0)
                    _CheckOutEnable = true;
                else
                    _CheckOutEnable = false;
                return _CheckOutEnable;
            }
            set
            {
                _CheckOutEnable = value;
                OnPropertyChanged(nameof(CheckOutEnable));
            }
        }

        private string _UVAlign;

        public string UVAlign
        {
            get { return _UVAlign; }
            set
            {
                _UVAlign = value;
                OnPropertyChanged(nameof(UVAlign));
            }
        }

        private ObservableCollection<ApiModel.GetMenu.Response.Category> _MasterMenuCategory;

        public ObservableCollection<ApiModel.GetMenu.Response.Category> MasterMenuCategory
        {
            get { return _MasterMenuCategory; }
            set
            {
                _MasterMenuCategory = value;
                OnPropertyChanged(nameof(MasterMenuCategory));
            }
        }

        private ObservableCollection<ApiModel.GetMenu.Response.Category> _MasterMenuProduct;

        public ObservableCollection<ApiModel.GetMenu.Response.Category> MasterMenuProduct
        {
            get { return _MasterMenuProduct; }
            set
            {
                _MasterMenuProduct = value;
                OnPropertyChanged(nameof(MasterMenuProduct));
            }
        }

        private ObservableCollection<ApiModel.GetMenu.Response.Product> _MenuDetails;

        public ObservableCollection<ApiModel.GetMenu.Response.Product> MenuDetails
        {
            get { return _MenuDetails; }
            set
            {
                _MenuDetails = value;
                OnPropertyChanged(nameof(MenuDetails));
            }
        }

        #endregion

        #region ImageAssign
        private string _BackgroundPath;

        public string BackgroundPath
        {
            get
            {
                //if (Stage == eStage.Home)
                //{
                //    _BackgroundPath = "\\Resource\\AnwImages\\bg-1.png";
                //}
                //else if (Stage == eStage.MenuCustomize || Stage == eStage.EditItem)
                //{
                //    _BackgroundPath = "\\Resource\\AnwImages\\bg-4.png";
                //}
                if (Stage == eStage.Initializing)
                    _BackgroundPath = "\\Resource\\LFFImages\\bg-2.png";
                else
                {
                    _BackgroundPath = "\\Resource\\LFFImages\\whitebg.png";
                }
                return _BackgroundPath;
            }
            set
            {
                _BackgroundPath = value;
                OnPropertyChanged(nameof(BackgroundPath));
            }
        }

        //private string _CartImage;

        //public string CartImage
        //{
        //    get
        //    {
        //        if (CartItem == 0)
        //        {
        //            _CartImage = "\\Resource\\LFFImages\\CartWithoutItem.png";
        //        }
        //        else if (CartItem > 0)
        //        {
        //            _CartImage = "\\Resource\\LFFImages\\CartWithItems.png";
        //        }
        //        return _CartImage;
        //    }
        //    set
        //    {
        //        _CartImage = value;
        //        OnPropertyChanged(nameof(CartImage));
        //    }
        //}

        public string LandingBanner => "\\Resource\\LFFImages\\BannerAnW-1.png";
        public string CartImage => "\\Resource\\LFFImages\\Cart.png";

        public string DineInIcon => "\\Resource\\LFFImages\\dine-in.png";
        public string DineInIcon2 => "\\Resource\\LFFImages\\di.png";

        public string TakeAwayIcon => "\\Resource\\LFFImages\\takeaway.png";
        public string TakeAwayIcon2 => "\\Resource\\LFFImages\\ta.png";

        public string PaymentType => "\\Resource\\LFFImages\\payment-type.png";
        public string AnWLogo => "\\Resource\\LFFImages\\LFF_Logo.png";
        public string AnWPhone => "\\Resource\\LFFImages\\phone.png";
        public string Grab => "\\Resource\\LFFImages\\grab.png";
        public string Boost => "\\Resource\\LFFImages\\boost.png";
        public string Tng => "\\Resource\\LFFImages\\tng.png";
        public string DuitNow => "\\Resource\\LFFImages\\duitnow.png";
        public string Card => "\\Resource\\LFFImages\\MyDebit.png";
        public string Visa => "\\Resource\\LFFImages\\visa.png";
        public string Master => "\\Resource\\LFFImages\\master.png";
        public string Cash => "\\Resource\\LFFImages\\cash.png";
        public string Android => "\\Resource\\LFFImages\\AnWAndroid.png";
        public string Apple => "\\Resource\\LFFImages\\AnWAppleQr.png";
        public string QrCode => "\\Resource\\LFFImages\\QrCode.png";
        public string RootyBuddy => "\\Resource\\LFFImages\\RootyBuddy.png";
        public string Logout => "\\Resource\\LFFImages\\logout.png";
        public string VoucherIcon => "\\Resource\\LFFImages\\voucherIcon.png";
        public string ErrorImg => "\\Resource\\LFFImages\\error.png";
        public string Home => "\\Resource\\LFFImages\\Home.png";
        public string Resit => "\\Resource\\LFFImages\\resit.png";
        #endregion

        #region LabelAssign
        public string Lbl_DineIn { get { return LFFSSK.Properties.Resources.Lbl_DineIn; } }
        public string Lbl_Takeaway => LFFSSK.Properties.Resources.Lbl_Takeaway;
        public string Lbl_Ads => LFFSSK.Properties.Resources.Lbl_Ads;
        public string Lbl_CancelHeader => LFFSSK.Properties.Resources.Lbl_CancelHeader;
        public string Lbl_CancelContent => LFFSSK.Properties.Resources.Lbl_CancelContent;
        public string Lbl_BtnYes => LFFSSK.Properties.Resources.Lbl_BtnYes;
        public string Lbl_BtnCancel => LFFSSK.Properties.Resources.Lbl_BtnClose;
        public string Lbl_ViewOrder => LFFSSK.Properties.Resources.Lbl_ViewOrder;
        public string Lbl_Back => LFFSSK.Properties.Resources.Lbl_Back;
        public string Lbl_AddToCart => LFFSSK.Properties.Resources.Lbl_AddToCart;
        public string Lbl_Checkout => LFFSSK.Properties.Resources.Lbl_Checkout;
        public string Lbl_YourOrder => LFFSSK.Properties.Resources.Lbl_YourOrder;
        public string Lbl_ContinueOrder => LFFSSK.Properties.Resources.Lbl_ContinueOrder;
        public string Lbl_CancelOrder => LFFSSK.Properties.Resources.Lbl_CancelOrder;
        public string Lbl_Upsize => LFFSSK.Properties.Resources.Lbl_Upsize;
        public string Lbl_Downsize => LFFSSK.Properties.Resources.Lbl_Downsize;
        public string Lbl_UpgradeCombo => LFFSSK.Properties.Resources.Lbl_UpgradeCombo;
        public string Lbl_BtnUpCombo => LFFSSK.Properties.Resources.Lbl_BtnUpCombo;
        public string Lbl_BtnUpsize => LFFSSK.Properties.Resources.Lbl_BtnUpsize;
        public string Lbl_BtnDownsize => LFFSSK.Properties.Resources.Lbl_BtnDownsize;
        public string Lbl_ScanAndLogin => LFFSSK.Properties.Resources.Lbl_ScanAndLogin;
        public string Lbl_SelectOrderType => LFFSSK.Properties.Resources.Lbl_SelectOrderType;
        public string Lbl_Download => LFFSSK.Properties.Resources.Lbl_Download;
        public string Lbl_Or => LFFSSK.Properties.Resources.Lbl_Or;
        public string Lbl_Skip => LFFSSK.Properties.Resources.Lbl_Skip;
        public string Lbl_AddOn => LFFSSK.Properties.Resources.Lbl_AddOn;
        public string Lbl_TextVoucher => LFFSSK.Properties.Resources.Lbl_TextVoucher;
        public string Lbl_MemberLogin => LFFSSK.Properties.Resources.Lbl_MemberLogin;
        public string Lbl_Login => LFFSSK.Properties.Resources.Lbl_Login;
        public string Lbl_ItemAdded => LFFSSK.Properties.Resources.Lbl_ItemAdded;
        public string Lbl_ScanQr => LFFSSK.Properties.Resources.Lbl_ScanQr;
        public string Lbl_BtnClose => LFFSSK.Properties.Resources.Lbl_BtnClose;
        public string Lbl_Welcome => LFFSSK.Properties.Resources.Lbl_Welcome;
        public string Lbl_CollectPoints => LFFSSK.Properties.Resources.Lbl_CollectPoints;
        public string Lbl_CC => LFFSSK.Properties.Resources.Lbl_CC;
        public string Lbl_Ewallet => LFFSSK.Properties.Resources.Lbl_Ewallet;
        public string Lbl_Cash => LFFSSK.Properties.Resources.Lbl_Cash;
        public string Lbl_YourAmount => LFFSSK.Properties.Resources.Lbl_YourAmount;
        public string Lbl_BackOnly => LFFSSK.Properties.Resources.Lbl_BackOnly;
        public string Lbl_PaymentMethod => LFFSSK.Properties.Resources.Lbl_PaymentMethod;
        public string Lbl_OrderNumber => LFFSSK.Properties.Resources.Lbl_OrderNumber;
        public string Lbl_PickupOrder => LFFSSK.Properties.Resources.Lbl_PickupOrder;
        public string Lbl_DoneOrder => LFFSSK.Properties.Resources.Lbl_DoneOrder;
        public string Lbl_SuccessOrder => LFFSSK.Properties.Resources.Lbl_SuccessOrder;
        public string Lbl_ShowVoucher => LFFSSK.Properties.Resources.Lbl_ShowVoucher;
        public string Lbl_Home => LFFSSK.Properties.Resources.Lbl_Home;
        public string Lbl_HomeHow => LFFSSK.Properties.Resources.Lbl_HomeHow;
        public string Lbl_TimeOut => LFFSSK.Properties.Resources.Lbl_TimeOut;
        public string Lbl_TimeoutMsj => LFFSSK.Properties.Resources.Lbl_TimeoutMsj;
        public string Lbl_Error => LFFSSK.Properties.Resources.Lbl_Error;
        public string Lbl_Confirmation => LFFSSK.Properties.Resources.Lbl_Confirmation;
        public string Lbl_Info => LFFSSK.Properties.Resources.Lbl_Info;
        public string Lbl_InvalidQr => LFFSSK.Properties.Resources.Lbl_InvalidQr;
        public string Lbl_InvalidPromoCode => LFFSSK.Properties.Resources.Lbl_InvalidPromoCode;
        public string Lbl_EnterPromoCode => LFFSSK.Properties.Resources.Lbl_EnterPromoCode;
        public string Lbl_ErrorAssist => LFFSSK.Properties.Resources.Lbl_ErrorAssist;
        public string Lbl_ErrorAssist2 => LFFSSK.Properties.Resources.Lbl_ErrorAssist2;
        public string Lbl_ErrorTryAgain => LFFSSK.Properties.Resources.Lbl_ErrorTryAgain;
        public string Lbl_ErrorTryAgain2 => LFFSSK.Properties.Resources.Lbl_ErrorTryAgain2;
        public string Lbl_ContainPromoCode => LFFSSK.Properties.Resources.Lbl_ContainPromoCode;
        public string Lbl_BtnPromoCode => LFFSSK.Properties.Resources.Lbl_BtnPromoCode;
        public string Lbl_PromoCode => LFFSSK.Properties.Resources.Lbl_PromoCode;
        public string Lbl_PopUpPromoCode => LFFSSK.Properties.Resources.Lbl_PopUpPromoCode;
        public string Lbl_PaymentFail => LFFSSK.Properties.Resources.Lbl_PaymentFail;
        public string Lbl_DownloadHere => LFFSSK.Properties.Resources.Lbl_DownloadHere;
        public string Lbl_Recommend => LFFSSK.Properties.Resources.Lbl_Recommend;
        public string Lbl_Subtotal => LFFSSK.Properties.Resources.Lbl_Subtotal;
        public string Lbl_VoucherAmount => LFFSSK.Properties.Resources.Lbl_VoucherAmount;
        public string Lbl_GiftVoucher => LFFSSK.Properties.Resources.Lbl_GiftVoucher;
        public string Lbl_Rounding => LFFSSK.Properties.Resources.Lbl_Rounding;
        public string Lbl_GTotal => LFFSSK.Properties.Resources.Lbl_GTotal;
        public string Lbl_ValidUntil => LFFSSK.Properties.Resources.Lbl_ValidUntil;
        public string Lbl_Edit => LFFSSK.Properties.Resources.Lbl_Edit;
        public string Lbl_InsertPin => LFFSSK.Properties.Resources.Lbl_InsertPin;
        public string Lbl_EwalletPayment => LFFSSK.Properties.Resources.Lbl_EwalletPayment;
        public string Lbl_EwalletMethod1 => LFFSSK.Properties.Resources.Lbl_EwalletMethod1;
        public string Lbl_EwalletMethod2 => LFFSSK.Properties.Resources.Lbl_EwalletMethod2;
        public string Lbl_EwalletMethod3 => LFFSSK.Properties.Resources.Lbl_EwalletMethod3;
        public string Lbl_EwalletMethod4 => LFFSSK.Properties.Resources.Lbl_EwalletMethod4;
        public string Lbl_Voucher => LFFSSK.Properties.Resources.Lbl_Voucher;
        public string Lbl_UseNow => LFFSSK.Properties.Resources.Lbl_UseNow;
        public string Lbl_Logout => LFFSSK.Properties.Resources.Lbl_Logout;
        public string Lbl_CheckVoucher => LFFSSK.Properties.Resources.Lbl_CheckVoucher;
        public string Lbl_InvalidMenu => LFFSSK.Properties.Resources.Lbl_InvalidMenu;
        public string Lbl_KadCC => LFFSSK.Properties.Resources.Lbl_KadCC;
        public string Lbl_DoneOnly => LFFSSK.Properties.Resources.Lbl_DoneOnly;
        public string Lbl_UpgradeComboR => LFFSSK.Properties.Resources.Lbl_UpgradeComboR;
        public string Lbl_UpgradeComboL => LFFSSK.Properties.Resources.Lbl_UpgradeComboL;
        public string Lbl_SelectLanguage => LFFSSK.Properties.Resources.Lbl_SelectLanguage;
        public string Lbl_InputTableTent => LFFSSK.Properties.Resources.Lbl_InputTableTent;
        public string Lbl_Next => LFFSSK.Properties.Resources.Lbl_Next;
        public string Lbl_No => LFFSSK.Properties.Resources.Lbl_No;
        public string Lbl_Yes => LFFSSK.Properties.Resources.Lbl_Yes;
        public string Lbl_DoYouHaveSeat => LFFSSK.Properties.Resources.Lbl_DoYouHaveSeat;
        #endregion

        #region ICommand

        public ICommand EasyMaintenanceCommand
        {
            get
            {
                if (_EasyMaintenanceCommand == null)
                    _EasyMaintenanceCommand = new RelayCommand<string>(EasyMaintenance);
                return _EasyMaintenanceCommand;
            }
        }

        public ICommand TimeOutCommand
        {
            get
            {
                if (_TimeOutCommand == null)
                    _TimeOutCommand = new RelayCommand<string>(TimeOut);
                return _TimeOutCommand;
            }
        }

        public ICommand ReprintCommand
        {
            get
            {
                if (_ReprintCommand == null)
                    _ReprintCommand = new RelayCommand<string>(Reprint);
                return _ReprintCommand;
            }
        }

        public ICommand LanguageCommand
        {
            get
            {
                if (_LanguageCommand == null)
                    _LanguageCommand = new RelayCommand<string>(Language);

                return _LanguageCommand;
            }

        }

        #endregion

        #region Method

        void PreLoad(bool OnStart)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PreLoad Starting..."), _TraceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PreLoad OnStart = {0}", OnStart), _TraceCategory);

                //if (OnStart) 
                //{
                //    if (GeneralVar.Monitoring_Enabled)
                //    {
                //        GeneralVar.ClientMonitoringController.StopMonitoring();
                //        GeneralVar.ClientMonitoringController.StartMonitoring();
                //    }
                //}

                GeneralVar.Checklist = new ObservableCollection<InitModel>();

                GeneralVar.Checklist.Add(new InitModel(eComponent.System_FNB, "Core System", true));
                GeneralVar.Checklist.Add(new InitModel(eComponent.CreditTerminal, "Credit Card Terminal", GeneralVar.CC_Enabled));
                GeneralVar.Checklist.Add(new InitModel(eComponent.IOBoard, "IO Board", OnStart ? GeneralVar.IOBoard_Enabled : false));
                GeneralVar.Checklist.Add(new InitModel(eComponent.QRCodeReader, "QR Code Reader", true));
                GeneralVar.Checklist.Add(new InitModel(eComponent.ReceiptPrinter, "Receipt Printer", GeneralVar.Printer_Enabled));


                StartSystemCheck();

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PreLoad Completed."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error]PreLoad = {0}", ex.ToString()), _TraceCategory);
            }

        }

        public void StartSystemCheck()
        {
            if (bwSystemCheck == null)
            {
                bwSystemCheck = new BackgroundWorker();
                bwSystemCheck.DoWork += bwSystemCheck_DoWork;
                bwSystemCheck.RunWorkerCompleted += bwSystemCheck_RunWorkerCompleted;
            }
            bwSystemCheck.RunWorkerAsync();
        }

        void bwSystemCheck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (GeneralVar.IOBoard_Enabled)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Warning] DoorOpened: {0}", GeneralVar.IOBoard.DoorOpened), _TraceCategory);
                if (GeneralVar.IOBoard.DoorOpened)
                {
                    ChangeToMaintenanceMode();
                }
                else
                {
                    if (Checklist.Count(c => c.InitialStatus == eInitStatus.Error) > 0)
                    {

                        SystemError = Checklist.Where(c => c.InitialStatus == eInitStatus.Error).FirstOrDefault().LastError;
                        ShowOOOErrorMessage = Visibility.Visible;
                        SetStage(eStage.OutOfOrder);
                    }
                    else
                    {
                        SetStage(eStage.Home);
                    }

                    SetIOBoardCallback();
                }
            }
            else
            {
                if (Checklist.Count(c => c.InitialStatus == eInitStatus.Error) > 0)
                {
                    SystemError = Checklist.Where(c => c.InitialStatus == eInitStatus.Error).FirstOrDefault().LastError;
                    ShowOOOErrorMessage = Visibility.Visible;
                    SetStage(eStage.OutOfOrder);
                    //SetStage(eStage.Offline);
                }
                else
                {
                    SetStage(eStage.Home);
                }
                //ChangeToMaintenanceMode();
                SetIOBoardCallback();
            }
        }

        void bwSystemCheck_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("bwSystemCheck_DoWork Starting..."), _TraceCategory);

                if (Checklist != null)
                    Checklist = null;

                Checklist = new ObservableCollection<InitModel>();

                foreach (InitModel row in GeneralVar.Checklist)
                {
                    Checklist.Add(new InitModel(row.Items, row.DisplayName, row.IsEnabled));
                }

                for (int i = 0; i < Checklist.Count(); i++)
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("bwSystemCheck_DoWork CheckList[{0}]", Checklist[i].DisplayName), _TraceCategory);
                    GeneralVar.Checklist[i].LastError = string.Empty;
                    Checklist[i].UpdateStatus(eInitStatus.Checking);
                    Thread.Sleep(200);

                    if (!Checklist[i].IsEnabled)
                    {
                        Checklist[i].UpdateStatus(eInitStatus.Information, "Unit Disabled"); ;
                        continue;
                    }

                    /* TO DO: Check Component Here */

                    #region System

                    if (Checklist[i].Items == eComponent.System_FNB)
                        LFFSSK_SystemChecking(i);

                    #endregion

                    #region CreditTerminal

                    else if (Checklist[i].Items == eComponent.CreditTerminal)
                    {
                        Init_CardTerminal(i);
                        // need to check with Cardbiz
                    }

                    #endregion

                    #region IOBoard

                    else if (Checklist[i].Items == eComponent.IOBoard)
                    {
                        InitIO(i);
                    }

                    #endregion

                    #region QRCodeReader

                    else if (Checklist[i].Items == eComponent.QRCodeReader)
                    {
                        InitQR(i);
                    }

                    #endregion

                    #region ReceiptPrinter

                    else if (Checklist[i].Items == eComponent.ReceiptPrinter)
                    {
                        InitPrinter(i);
                    }

                    #endregion

                    /* TO DO: Check Component Done */
                }

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("bwSystemCheck_DoWork Completed."), _TraceCategory);

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error]bwSystemCheck_DoWork = {0}.", ex.ToString()), _TraceCategory);
            }

        }

        List<int> eastMalaysia = new List<int>() { 91, 83, 84, 79 };

        string mssg = "";
        void LFFSSK_SystemChecking(int i)
        {
            try
            {
                mssg = "";

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("ComponentCode = {0}", GeneralVar.ComponentCode), _TraceCategory);

                Checklist[i].UpdateStatus(eInitStatus.Checking, "Retrieving Menu");

                List<ApiModel.GetMenu.Response.Category> tempCategory = new List<ApiModel.GetMenu.Response.Category>();
                List<ApiModel.GetMenu.Response.Category> tempMenu = new List<ApiModel.GetMenu.Response.Category>();

                KioskId = GeneralVar.ComponentCode;
                StoreName = GeneralVar.ComponentCode;

                if (_ApiFunc.GetMenu(out ApiModel.GetMenu.Response res))
                {
                    if (res != null)
                    {
                        if (res.ok)
                        {
                            if (res.data != null)
                            {
                                if (res.data.Count() > 0)
                                {
                                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                    {
                                        foreach (var category in res.data)
                                        {
                                            ApiModel.GetMenu.Response.Category newCategory = new ApiModel.GetMenu.Response.Category(category.categoryId, category.categoryName, category.categoryImageUrl, null, 0, null, null, null);

                                            tempCategory.Add(newCategory);
                                        };
                                        MasterMenuCategory = new ObservableCollection<ApiModel.GetMenu.Response.Category>(tempCategory);


                                        foreach (var menuCate in res.data)
                                        {
                                            ApiModel.GetMenu.Response.Category Data = null;
                                            Data = new ApiModel.GetMenu.Response.Category(menuCate.categoryId, menuCate.categoryName, menuCate.categoryImageUrl, menuCate.colorHex, menuCate.displayMode, menuCate.Department, menuCate.binaryImageUrl, new List<ApiModel.GetMenu.Response.Product>());
                                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Category : {0}", menuCate.categoryName), _TraceCategory);


                                            foreach (var menu in menuCate.products)
                                            {
                                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Product : {0}", menu.itemName), _TraceCategory);
                                                string menuFileName = string.Empty;

                                                if (menu.imageUrlList != null && menu.imageUrlList.Count() > 0)
                                                {
                                                    if (!string.IsNullOrEmpty(menu.imageUrlList.First()))
                                                        menuFileName = menu.imageUrlList.First().Substring(menu.imageUrlList.First().LastIndexOf('/') + 1);

                                                    foreach (var item in menuCate.products.Where(y => y.itemId == menu.itemId))
                                                    {
                                                        item.MenuImage = menuFileName;
                                                    }

                                                    GeneralFunc.DownloadMedia(GeneralVar.MenuRepository, menuFileName, menu.imageUrlList.First());

                                                    foreach (var item in menuCate.products.Where(y => y.itemId == menu.itemId))
                                                    {
                                                        item.RefreshMenuImage();
                                                    }
                                                }

                                                ApiModel.GetMenu.Response.Product newMenu = new ApiModel.GetMenu.Response.Product(menu.DOUBLE_Sale_Price, menu.DOUBLE_Employee_Price, menu.DOUBLE_Wholesale_Price, menu.DOUBLE_Custom_Price, menu.DOUBLE_Manufacturer_Suggested_Retail_Price, menu.DOUBLE_Web_Price, menu.DOUBLE_Web_Dealer_Price, menu.customField1, menu.customField2, menu.customField3, menu.customField4, menu.customField5, menu.customField6, menu.customField7, menu.customField8, menu.customField9, menu.customField10, menu.customField11, menu.customField12, menu.customField13, menu.customField14, menu.customField15, menu.customField16, menu.customField17, menu.customField18, menu.itemName, menu.colorHex, menu.dynamicHeaderLabel, new List<ApiModel.GetMenu.Response.DynamicModifier>(), menu.itemId, menu.itemCode, menu.itemType, menu.price, menu.imageUrlList, menu.binaryImageUrl, menu.imageUrl, menu.description, menu.size, menu.boolOpenPrice, menu.stockType, menu.IsAssortment, menu.HasStock, menu.AllowToSell, menu.IsActive, menu.MenuImagePath, menu.MenuImage, 1, menu.DOUBLE_Sale_Price, menu.DOUBLE_Sale_Price);

                                                foreach (var modifier in menu.dynamicmodifiers)
                                                {
                                                    if (modifier.imageUrlList != null && modifier.imageUrlList.Count() > 0)
                                                    {
                                                        if (!string.IsNullOrEmpty(modifier.imageUrlList.First()))
                                                            menuFileName = modifier.imageUrlList.First().Substring(modifier.imageUrlList.First().LastIndexOf('/') + 1);

                                                        foreach (var item in menu.dynamicmodifiers.Where(y => y.itemId == modifier.itemId))
                                                        {
                                                            item.MenuImage = menuFileName;
                                                        }

                                                        GeneralFunc.DownloadMedia(GeneralVar.MenuRepository, menuFileName, modifier.imageUrlList.First());

                                                        foreach (var item in menu.dynamicmodifiers.Where(y => y.itemId == modifier.itemId))
                                                        {
                                                            item.RefreshMenuImage();
                                                        }
                                                    }

                                                    ApiModel.GetMenu.Response.DynamicModifier tempDynamicMod = new ApiModel.GetMenu.Response.DynamicModifier
                                                    {
                                                        DOUBLE_Sale_Price = modifier.DOUBLE_Sale_Price,
                                                        DOUBLE_Employee_Price = modifier.DOUBLE_Employee_Price,
                                                        DOUBLE_Wholesale_Price = modifier.DOUBLE_Wholesale_Price,
                                                        DOUBLE_Custom_Price = modifier.DOUBLE_Custom_Price,
                                                        DOUBLE_Manufacturer_Suggested_Retail_Price = modifier.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                                        DOUBLE_Web_Price = modifier.DOUBLE_Web_Price,
                                                        DOUBLE_Web_Dealer_Price = modifier.DOUBLE_Web_Dealer_Price,
                                                        customField1 = modifier.customField1,
                                                        customField2 = modifier.customField2,
                                                        customField3 = modifier.customField3,
                                                        customField4 = modifier.customField4,
                                                        customField5 = modifier.customField5,
                                                        customField6 = modifier.customField6,
                                                        customField7 = modifier.customField7,
                                                        customField8 = modifier.customField8,
                                                        customField9 = modifier.customField9,
                                                        customField10 = modifier.customField10,
                                                        customField11 = modifier.customField11,
                                                        customField12 = modifier.customField12,
                                                        customField13 = modifier.customField13,
                                                        customField14 = modifier.customField14,
                                                        customField15 = modifier.customField15,
                                                        customField16 = modifier.customField16,
                                                        customField17 = modifier.customField17,
                                                        customField18 = modifier.customField18,
                                                        itemName = modifier.itemName,
                                                        itemShortName = modifier.itemShortName,
                                                        defaultSelected = modifier.defaultSelected,
                                                        modifiers = new List<ApiModel.GetMenu.Response.Modifier>(),
                                                        itemId = modifier.itemId,
                                                        itemCode = modifier.itemCode,
                                                        itemType = modifier.itemType,
                                                        price = modifier.price,
                                                        imageUrlList = modifier.imageUrlList,
                                                        binaryImageUrl = modifier.binaryImageUrl,
                                                        imageUrl = modifier.imageUrl,
                                                        description = modifier.description,
                                                        size = modifier.size,
                                                        boolOpenPrice = modifier.boolOpenPrice,
                                                        stockType = modifier.stockType,
                                                        IsAssortment = modifier.IsAssortment,
                                                        HasStock = modifier.HasStock,
                                                        AllowToSell = modifier.AllowToSell,
                                                        IsActive = modifier.IsActive,
                                                        MenuImage = modifier.MenuImage,
                                                        MenuImagePath = modifier.MenuImagePath
                                                    };

                                                    foreach (var modifierDetails in modifier.modifiers)
                                                    {
                                                        ApiModel.GetMenu.Response.Modifier tempMod = new ApiModel.GetMenu.Response.Modifier
                                                        {
                                                            type = modifierDetails.type,
                                                            minSelection = modifierDetails.minSelection,
                                                            maxSelection = modifierDetails.maxSelection,
                                                            groupId = modifierDetails.groupId,
                                                            groupName = modifierDetails.groupName,
                                                            selections = new List<ApiModel.GetMenu.Response.Selection>()
                                                        };

                                                        foreach (var selection in modifierDetails.selections)
                                                        {
                                                            if (selection.imageUrlList != null && selection.imageUrlList.Count() > 0)
                                                            {
                                                                if (!string.IsNullOrEmpty(selection.imageUrlList.First()))
                                                                    menuFileName = selection.imageUrlList.First().Substring(selection.imageUrlList.First().LastIndexOf('/') + 1);

                                                                foreach (var item in modifierDetails.selections.Where(y => y.itemId == selection.itemId))
                                                                {
                                                                    item.MenuImage = menuFileName;
                                                                }

                                                                GeneralFunc.DownloadMedia(GeneralVar.MenuRepository, menuFileName, selection.imageUrlList.First());

                                                                foreach (var item in modifierDetails.selections.Where(y => y.itemId == selection.itemId))
                                                                {
                                                                    item.RefreshMenuImage();
                                                                }
                                                            }

                                                            ApiModel.GetMenu.Response.Selection tempSelection = new ApiModel.GetMenu.Response.Selection
                                                            {
                                                                name = selection.name,
                                                                defaultSelected = selection.defaultSelected,
                                                                itemId = selection.itemId,
                                                                itemCode = selection.itemCode,
                                                                itemType = selection.itemType,
                                                                price = selection.price,
                                                                DOUBLE_Sale_Price = selection.DOUBLE_Sale_Price,
                                                                DOUBLE_Employee_Price = selection.DOUBLE_Employee_Price,
                                                                DOUBLE_Wholesale_Price = selection.DOUBLE_Wholesale_Price,
                                                                DOUBLE_Custom_Price = selection.DOUBLE_Custom_Price,
                                                                DOUBLE_Manufacturer_Suggested_Retail_Price = selection.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                                                DOUBLE_Web_Price = selection.DOUBLE_Web_Price,
                                                                DOUBLE_Web_Dealer_Price = selection.DOUBLE_Web_Dealer_Price,
                                                                imageUrlList = selection.imageUrlList,
                                                                binaryImageUrl = selection.binaryImageUrl,
                                                                imageUrl = selection.imageUrl,
                                                                description = selection.description,
                                                                customField1 = selection.customField1,
                                                                customField2 = selection.customField2,
                                                                customField3 = selection.customField3,
                                                                customField4 = selection.customField4,
                                                                customField5 = selection.customField5,
                                                                customField6 = selection.customField6,
                                                                customField7 = selection.customField7,
                                                                customField8 = selection.customField8,
                                                                customField9 = selection.customField9,
                                                                customField10 = selection.customField10,
                                                                customField11 = selection.customField11,
                                                                customField12 = selection.customField12,
                                                                customField13 = selection.customField13,
                                                                customField14 = selection.customField14,
                                                                customField15 = selection.customField15,
                                                                customField16 = selection.customField16,
                                                                customField17 = selection.customField17,
                                                                customField18 = selection.customField18,
                                                                size = selection.size,
                                                                boolOpenPrice = selection.boolOpenPrice,
                                                                stockType = selection.stockType,
                                                                IsAssortment = selection.IsAssortment,
                                                                HasStock = selection.HasStock,
                                                                AllowToSell = selection.AllowToSell,
                                                                IsActive = selection.IsActive,
                                                                parentItemId = newMenu.itemId,
                                                                minSelection = tempMod.minSelection,
                                                                maxSelection = tempMod.maxSelection,
                                                                groupId = tempMod.groupId,
                                                                MenuImage = selection.MenuImage,
                                                                MenuImagePath = selection.MenuImagePath,
                                                                IsEnable = true

                                                            };

                                                            tempMod.selections.Add(tempSelection);
                                                        }

                                                        tempDynamicMod.modifiers.Add(tempMod);
                                                    }
                                                    newMenu.dynamicmodifiers.Add(tempDynamicMod);
                                                }
                                                Data.products.Add(newMenu);
                                            }
                                            tempMenu.Add(Data);
                                        }


                                        if (MasterMenuProduct != null)
                                            MasterMenuProduct = null;
                                        MasterMenuProduct = new ObservableCollection<ApiModel.GetMenu.Response.Category>(tempMenu);

                                    }));

                                }

                            }
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(res.status))
                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("[Error] API GetMenu - {0}", res.status), _TraceCategory);
                            else
                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("[Error] API GetMenu - OK response false"), _TraceCategory);
                        }
                    }
                }

                Checklist[i].UpdateStatus(eInitStatus.Success, "Retrieve Menu Success");

                BannerList = new ObservableCollection<string>() { "FFbanner1.png" };

                if (GeneralVar.IOBoard == null)
                    GeneralVar.IOBoard = new IOHelper();


                if (GeneralVar.DocumentPrint != null)
                    GeneralVar.DocumentPrint = null;
                GeneralVar.DocumentPrint = new DocumentPrintHelper(GeneralVar.ReceiptPrinter_Port);

                //GeneralVar.user = new User();
                //bool valid1, valid3, valid2;
                //GeneralVar.user.UserAuthentication("sysadmin", "123456", out valid1, out valid2, out valid3);

                Checklist[i].UpdateStatus(eInitStatus.Success, "Success");
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] LFFSSK_SystemChecking = {0}", ex.ToString()), _TraceCategory);

                Checklist[i].UpdateStatus(eInitStatus.Error, mssg);
                //SetStage(eStage.OutOfOrder);
            }
        }

        void Init_CardTerminal(int i)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Init_CardTerminal Starting..."), _TraceCategory);

                Checklist[i].UpdateStatus(eInitStatus.Checking, "Init_CardTerminal Start");

                KeepAlive _CardBizLastAliveRequest;
                KeepAlive_Resp _CardBizLastAliveResponse = new KeepAlive_Resp();

                try
                {
                    _CardBizLastAliveRequest = new KeepAlive(GeneralVar.CC_PortName, true);
                    _CardBizLastAliveResponse = _CardBizLastAliveRequest.SendRequest();
                }
                catch (Exception ex)
                {
                    string description = string.Format("[CC] Unable to open port: {0}", ex.Message);
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, description, _TraceCategory);
                    throw new Exception("Terminal Offline");
                }

                try
                {
                    if (_CardBizLastAliveResponse.StatusCode != 0 || _CardBizLastAliveResponse.ResponseCode != "00")
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[CC] Terminal Status: StatusCode = {0} - {1}, ResponseCode = {2}", _CardBizLastAliveResponse.StatusCode, _CardBizLastAliveResponse.StatusMessage, _CardBizLastAliveResponse.ResponseCode), _TraceCategory);
                        throw new Exception("Terminal Error");
                    }
                }
                catch (Exception ex)
                {
                    string description = string.Format("[CC] CheckCreditCardTerminal: {0}", ex.Message);
                    throw new Exception(description);
                }

                Checklist[i].UpdateStatus(eInitStatus.Success, string.Format("Card Terminal OK"));

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Init_CardTerminal Comleted."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] Init_CardTerminal = {0}", ex.ToString()), _TraceCategory);
                Checklist[i].UpdateStatus(eInitStatus.Error, ex.Message);
            }
        }

        void InitPrinter(int i)
        {
            GeneralVar.DocumentPrint.LastError = string.Empty;
            Checklist[i].UpdateStatus(eInitStatus.Checking);

            if (GeneralVar.PrinterModel == ePrinterModel.Fujitsu)
            {
                if (GeneralVar.DocumentPrint.InitializePrinterFujitsu())
                    Checklist[i].UpdateStatus(string.IsNullOrEmpty(GeneralVar.DocumentPrint.LastError) ? eInitStatus.Success : eInitStatus.Warning, GeneralVar.DocumentPrint.LastError);
                else
                    Checklist[i].UpdateStatus(eInitStatus.Error, GeneralVar.DocumentPrint.LastError);
            }
        }

        void InitIO(int i)
        {
            Checklist[i].UpdateStatus(eInitStatus.Checking);
            if (GeneralVar.IOBoard_Enabled && !GeneralVar.IOBoard.OpenPort(GeneralVar.IOBoard_PortName, GeneralVar.IOBoard_BaudRate, GeneralVar.IOBoard_Parity, GeneralVar.IOBoard_DataBit, GeneralVar.IOBoard_StopBit))
                Checklist[i].UpdateStatus(eInitStatus.Error, "[I/O Board] " + "");
            else if (!GeneralVar.IOBoard.Initialization())
                Checklist[i].UpdateStatus(eInitStatus.Error, "Unable Initialize");
            else
                Checklist[i].UpdateStatus(eInitStatus.Success);
        }

        #region QR Init
        private string _QRValue;
        public string QRValue
        {
            get { return _QRValue; }
            set { _QRValue = value; OnPropertyChanged(nameof(QRValue)); }
        }

        AutoResetEvent waitforQRScan = new AutoResetEvent(false);

        private void QRResponse(string content)
        {
            waitforQRScan.Set();

            content = content.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Trim();

            if (!String.IsNullOrEmpty(content))
            {
                if (((Stage == eStage.OrderSummary && QRPopUpVisbility == Visibility.Visible) || (Stage == eStage.MenuItem && LoginVisbility == Visibility.Visible && LoginPopUp)) && readyToScan)
                {
                    Thread threadLogIn = new Thread(() =>
                    {
                        try
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] thLogin Starting..."), _TraceCategory);
                            ShowLoading = Visibility.Visible;
                            QRValue = content;
                            if (CheckQR())
                            {
                                BackgroundVisibility = Visibility.Collapsed;
                                LogInBorderVisibility = Visibility.Collapsed;
                                LogOutBorderVisibility = Visibility.Visible;
                                PointGridVisibility = Visibility.Visible;
                                BorderHeight = 1200;

                                readyToScan = false;
                            }
                            else
                            {
                                WarningMessageBoxVisibility = Visibility.Visible;
                            }

                            LoginPopUp = false;
                            QRPopUpVisbility = Visibility.Collapsed;
                            LoginVisbility = Visibility.Collapsed;
                            ShowLoading = Visibility.Collapsed;
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] thLogin Done..."), _TraceCategory);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("{Error] Trigger threadLogIn : {0}", ex.ToString()), _TraceCategory);
                        }
                    });
                    threadLogIn.Start();
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] Barcode Reader Value = {0}", content), _TraceCategory);
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] readyToScan Value = {0}", readyToScan), _TraceCategory);
                }
                else if (Stage == eStage.PaymentMethodSelection)
                {
                    //Need to fix double scan
                    //if (ShowEWallet == Visibility.Visible)
                    //{
                    //    Thread thEwallet = new Thread(() =>
                    //    {
                    //        try
                    //        {
                    //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] thEwallet Starting..."), _TraceCategory);
                    //            //ShowEWallet = Visibility.Collapsed;
                    //            ShowPaymentProcessing = Visibility.Visible;
                    //            StartAcceptingEWallet(content);
                    //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] thEwallet Done..."), _TraceCategory);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Error] thEwallet error = {0}", ex.ToString()), _TraceCategory);
                    //        }

                    //    });
                    //    thEwallet.Start();
                    //}
                }
                else
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] readyToScan Value = {0}", readyToScan), _TraceCategory);
                }
            }
        }
        void InitQR(int i)
        {
            try
            {
                Checklist[i].UpdateStatus(eInitStatus.Checking);
                if (GeneralVar.QR_Enabled)
                {
                    if (GeneralVar.EntryQRHelper != null)
                        GeneralVar.EntryQRHelper.ClosePort();

                    GeneralVar.EntryQRHelper = new XFDevice.BarcodeScanner.BCTG321UVCP.BCTG321UVCPHelper(QRResponse);
                    GeneralVar.EntryQRHelper.OpenPort(GeneralVar.QR_PortName, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);

                }
                Checklist[i].UpdateStatus(eInitStatus.Checking, "Success");
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] InitQR = {0}", ex.Message), _TraceCategory);
                GeneralVar.QRLastErrorMsg = String.Format("[Error] InitQR = {0}", "QR Reader Offline");
                //GeneralVar.QRLastErrorMsg = String.Format("[Error] InitQR = {0}", ex.Message);
                //Checklist[i].UpdateStatus(eInitStatus.Error, ex.Message);
                Checklist[i].UpdateStatus(eInitStatus.Error, "QR Reader Offline");
            }
        }
        #endregion

        #endregion
        void Language(string language)
        {
            try
            {
                switch (language)
                {
                    case "BM":
                        _UiCulture = "ms-my";
                        IsEN = false;
                        IsBM = true;
                        break;
                    case "EN":
                        _UiCulture = "en-us";
                        IsEN = true;
                        IsBM = false;
                        break;
                    case "ZH":
                        _UiCulture = "zh-cn";
                        break;
                    default:
                        IsEN = true;
                        IsBM = false;
                        _UiCulture = "en-us";
                        break;
                }

                LFFSSK.Properties.Resources.Culture = new System.Globalization.CultureInfo(_UiCulture);
                TriggerLFFSSKLanguage();
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] Language = {0}", ex.ToString()), _TraceCategory);

            }
        }
        void TimeOut(string status)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("TimeOut Starting..."), _TraceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("TimeOut Status = {0}", status), _TraceCategory);

                ShowTimeOut = BackgroundVisibility = Visibility.Collapsed;
                if (status.ToUpper() == "Y")
                {
                    isStop = false;
                    timeRemainingTimeoutTimer.Stop();
                    TimeoutCountdown = new TimeSpan(0, 0, 15);

                    timeRemainingTimer.Start();
                    IsTimeRemainingVisible = true;
                }
                else
                {
                    timeRemainingTimeoutTimer.Stop();
                    autoResetTimeout.Set();
                }

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("TimeOut Completed."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] TimeOut = {0}", ex.ToString()), _TraceCategory);
            }
        }

        void PrepareReceiptList()
        {
            try
            {
                List<string> path = GeneralVar.DocumentPrint.GetFileList();
                if (ReprintCollection != null)
                    ReprintCollection = null;

                ReprintCollection = new ObservableCollection<ReprintSelectionHelper>();
                if (path.Count() > 0)
                {
                    if (path.Count() > 10)
                        path = path.Take(10).ToList();
                    foreach (string sp in path)
                    {
                        string sname = Path.GetFileNameWithoutExtension(sp);
                        ReprintCollection.Add(new ReprintSelectionHelper(sname, ReprintCommand, sname, sp));
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] PrepareReceiptList = {0}", ex.ToString()), _TraceCategory);
            }
        }

        void Reprint(string param)
        {
            try
            {
                if (param == "B")
                {
                    SetStage(eStage.MaintenanceSelection);
                    return;
                }

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Reprint Starting..."), _TraceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Reprint param = {0}", param), _TraceCategory);
                string path = ReprintCollection.Single(c => c.CommandParameter == param).rPath;

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Reprint path = {0}", path), _TraceCategory);

                GeneralVar.DocumentPrint.Reprint(path);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Reprint Completed."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] Reprint = {0}", ex.ToString()), _TraceCategory);
            }

        }

        void EasyMaintenance(string action)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("EasyMaintenance Starting..."), _TraceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("EasyMaintenance action = {0}", action), _TraceCategory);

                if (action == "OFF")
                {
                    OffClickAttempt++;
                    if (OffClickAttempt >= 5)
                    {
                        OffClickAttempt = 0;
                        if (Stage == eStage.LandingPage || Stage == eStage.OutOfOrder || Stage == eStage.OffOperation || Stage == eStage.MenuCategory)
                        {
                            UnderMaintenance = !UnderMaintenance;
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("EasyMaintenance action = Offline ({0})", UnderMaintenance), _TraceCategory);
                        }
                    }
                }

                if (action == "M")
                {
                    maintenanceClickAttempt++;
                    if (maintenanceClickAttempt >= 5)
                    {
                        maintenanceClickAttempt = 0;
                        if (Stage == eStage.Home || Stage == eStage.Offline || Stage == eStage.OffOperation || Stage == eStage.OutOfOrder)
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("EasyMaintenance action = Maintenance Mode"), _TraceCategory);
                            ChangeToMaintenanceMode();
                        }
                    }
                }

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("EasyMaintenance Completed."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] EasyMaintenance = {0}", ex.ToString()), _TraceCategory);
            }
        }


        #region Contructor

        public MainWindowViewModel()
        {

            //string nref = string.Empty;
            //int orderId, dataId;
            //GeneralVar.getLatestUniqTranNo("", false, out nref, out orderId, out dataId);
            AdsVisibility = Visibility.Collapsed;
            BackgroundVisibility = Visibility.Collapsed;
            CancelVisibility = Visibility.Collapsed;
            BannerAdsVisibility = Visibility.Collapsed;
            DetailsViewVisbility = Visibility.Collapsed;
            QRPopUpVisbility = Visibility.Collapsed;
            LoginVisbility = Visibility.Collapsed;
            PointGridVisibility = Visibility.Collapsed;
            VoucherPopupVisibility = Visibility.Collapsed;
            WarningMessageBoxVisibility = Visibility.Collapsed;
            MenuBannerVisibility = Visibility.Collapsed;
            CheckOutContentVisibility = Visibility.Collapsed;

            //handle delay order
            DelayOrderPopupVisibility = Visibility.Collapsed;
            TableNoVisibility = Visibility.Collapsed;
            IsDelaySentOrder = false;

            if (GeneralVar.TestingMode)
                TestLoginVisibility = Visibility.Visible;
            else
                TestLoginVisibility = Visibility.Collapsed;

            MenuCurrentTotal = 0;
            BorderHeight = 1300;
            VoucherQty = 0;

            TimeList = new ObservableCollection<TimePicker>(TimeListl);
            SelectedTime = TimeList[0];
            if (bwMaintenanceTimer == null)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Initial Maintenance Timer Starting..."), _TraceCategory);

                bwMaintenanceTimer = new BackgroundWorker();
                bwMaintenanceTimer.WorkerReportsProgress = true;
                bwMaintenanceTimer.WorkerSupportsCancellation = true;
                bwMaintenanceTimer.ProgressChanged += bwMaintenanceTimer_ProgressChanged;
                bwMaintenanceTimer.DoWork += bwMaintenanceTimer_DoWork;
                bwMaintenanceTimer.RunWorkerCompleted += bwMaintenanceTimer_RunWorkerCompleted;

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Initial Maintenance Timer Completed."), _TraceCategory);
            }

            SetStage(eStage.Initializing);
            PreLoad(true);
            _ApiFunc = new ApiFunc();
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = true;
            timer.Start();

            if (timeRemainingTimer != null)
                timeRemainingTimer = null;

            timeRemainingTimer = new System.Timers.Timer(1000);
            timeRemainingTimer.Elapsed += timeRemainingTimer_Elapsed;

            if (timeRemainingTimeoutTimer != null)
                timeRemainingTimeoutTimer = null;
            timeRemainingTimeoutTimer = new System.Timers.Timer(1000);
            timeRemainingTimeoutTimer.Elapsed += timeRemainingTimeoutTimer_Elapsed;

            if (refreshMenuTime != null)
                refreshMenuTime = null;
            refreshMenuTime = new System.Timers.Timer(3600000);
            //refreshMenuTime = new System.Timers.Timer(300000);
            refreshMenuTime.Elapsed += RefreshMenuTime_Elapsed;
            refreshMenuTime.Start();
        }

        private void RefreshMenuTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] RefreshMenuTime_Elapsed starting..."), _TraceCategory);
                if (Stage == eStage.Home)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] RefreshMenuTime_Elapsed Refreshing..."), _TraceCategory);
                        ShowLoading = Visibility.Visible;


                        ShowLoading = Visibility.Collapsed;
                    }));
                }
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] RefreshMenuTime_Elapsed Done..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] RefreshMenuTime_Elapsed - {0}", ex.ToString()), _TraceCategory);
            }
        }

        bool isRun = false;
        DateTime settlementDate = DateTime.MinValue;
        DateTime secondsettlementDate = DateTime.MinValue;
        DateTime thirdsettlementDate = DateTime.MinValue;
        int batchCount = 0;
        int tempbatchAmount = 0;
        decimal batchamount = 0.00m;
        string batchNumber = "-";
        bool SettlementRun = false;

        DateTime checkNow = DateTime.MinValue;
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("attmp={0}, timeout = {1}", maintenanceClickAttempt, maintenanceClickTimeout), _TraceCategory);
                CurrentDate = DateTime.Now;
                if (OffClickAttempt > 0)
                {
                    if (OffClickTimeout <= 1)
                    {
                        OffClickTimeout++;
                    }
                    else
                    {
                        OffClickTimeout = 0;
                        OffClickAttempt = 0;
                    }
                }
                if (maintenanceClickAttempt > 0)
                {
                    if (maintenanceClickTimeout <= 1)
                    {
                        maintenanceClickTimeout++;
                    }
                    else
                    {
                        maintenanceClickTimeout = 0;
                        maintenanceClickAttempt = 0;
                    }
                }
                if (InitClickAttempt > 0)
                {
                    if (InitClickTimeout <= 1)
                    {
                        InitClickTimeout++;
                    }
                    else
                    {
                        InitClickTimeout = 0;
                        InitClickAttempt = 0;
                    }
                }

                if (RefreshClickAttempt > 0)
                {
                    if (RefreshClickTimeout <= 1)
                    {
                        RefreshClickTimeout++;
                    }
                    else
                    {
                        RefreshClickTimeout = 0;
                        RefreshClickAttempt = 0;
                    }
                }

                if (OfflineClickAttempt > 0)
                {
                    if (OfflineClickTimeout <= 1)
                    {
                        OfflineClickTimeout++;
                    }
                    else
                    {
                        OfflineClickTimeout = 0;
                        OfflineClickAttempt = 0;
                    }
                }


                if (!isRun)
                {
                    isRun = true;

                    //if (GeneralVar.LFFSSKComponent != null)
                    //{
                    //TimeSpan operationStart = GeneralVar.LFFSSKComponent.OpeningTime.TimeOfDay;
                    //TimeSpan operationEnd = GeneralVar.LFFSSKComponent.ClosingTime.TimeOfDay;

                    TimeSpan operationStart = GeneralVar.OpeningTime.TimeOfDay;
                    TimeSpan operationEnd = GeneralVar.ClosingTime.TimeOfDay;

                    if (GeneralVar.OpeningTime != null && GeneralVar.ClosingTime != null && !UnderMaintenance)
                    {
                        if (!TimeBetween(DateTime.Now, operationStart, operationEnd))
                        {
                            if (Stage != eStage.Offline && Stage == eStage.Home)
                            {
                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] Closing Time..."), _TraceCategory);
                                SystemError = "[Operation] Off Operation";
                                SetStage(eStage.Offline);
                            }
                        }
                        else if (Stage == eStage.Offline)
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] Opening Time..."), _TraceCategory);
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            {
                                SetStage(eStage.Initializing);
                                PreLoad(false);
                            }));
                        }
                    }

                    if (GeneralVar.CC_SettlementTime != null)
                    {
                        //if (GeneralVar.LFFSSKComponent.EDCSettlementTime.Exists(x => x.ToString("HH:mm") == DateTime.Now.ToString("HH:mm")) && settlementDate.Date != DateTime.Now.Date)
                        if (GeneralVar.CC_SettlementTime == DateTime.Now.ToString("HH:mm") && settlementDate.Date != DateTime.Now.Date)
                        {
                            if (Stage == eStage.Offline || Stage == eStage.Home || Stage == eStage.OffOperation)
                                SettlementRun = true;

                            if (SettlementRun)
                            {
                                settlementDate = DateTime.Now;
                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("SettlementTime {0}", DateTime.Now), _TraceCategory);

                                PerformCardbizSettlement();

                                SettlementRun = false;
                            }
                        }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error]timer_Elapsed = {0}", ex.ToString()), _TraceCategory);
            }
            isRun = false;
        }

        bool TimeBetween(DateTime datetime, TimeSpan start, TimeSpan end)
        {
            // convert datetime to a TimeSpan
            TimeSpan now = datetime.TimeOfDay;
            // see if start comes before end
            if (start < end)
                return start <= now && now <= end;
            // start is after end, so do the inverse comparison
            return !(end < now && now < start);
        }

        #endregion

        #region IO Board

        public bool SetIOBoardCallback()
        {
            bool result = false;
            try
            {
                GeneralVar.IOBoard.IOBoardCallBack = Callback_IOBoard;

                result = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] SetIOBoardCallback :{0}", ex.ToString()), _TraceCategory);
            }

            return result;
        }

        bool? CurrentDoorStage = null;
        private void Callback_IOBoard(bool doorOpened)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceVerbose, "OnSensorDetection Starting...", _TraceCategory);

                if (CurrentDoorStage == null)
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceVerbose, string.Format("OnSensorDetection {0}", doorOpened), _TraceCategory);
                else if (CurrentDoorStage == doorOpened)
                    return;

                CurrentDoorStage = doorOpened;

                MaintenanceDoorOpened = doorOpened;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceVerbose, "OnSensorDetection Completed", _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] OnSensorDetection: {0}", ex.ToString()), _TraceCategory);
            }
        }

        bool? _MaintenanceDoorOpened = false;
        public bool? MaintenanceDoorOpened
        {
            get { return _MaintenanceDoorOpened; }
            set
            {
                if (value == _MaintenanceDoorOpened)
                    return;
                _MaintenanceDoorOpened = value;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("MaintenanceDoorOpened:{0}", _MaintenanceDoorOpened), _TraceCategory);
                if (_MaintenanceDoorOpened == true)
                    ChangeToMaintenanceMode();
            }
        }

        public void ChangeToMaintenanceMode()
        {
            try
            {
                //try
                //{
                //}
                //catch (Exception) { }

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "Maintenance Starting...", _TraceCategory);

                if (_Stage != eStage.MaintenanceSelection && _Stage != eStage.MaintenanceSelection && _Stage != eStage.MaintenanceLogin)
                {
                    Username = "";
                    Password = "";
                    StartMaintenancebwTimer(new Action(BackToInitializing));
                    SetStage(eStage.MaintenanceLogin);
                }

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "Maintenance Completed", _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] ChangeToMaintenanceMode :{0}", ex.ToString()), _TraceCategory);
            }
        }

        #endregion

        #region Module Timeout

        public void StartTimer()
        {
            Thread t = new Thread(() => StartCountDown(_TimeOutSecond));
            t.Start();
        }

        void TimeOutTrigger()
        {
            StopCountDown();

            if (Stage == eStage.Home && AdsVisibility == Visibility.Visible)
            {
                AdsVisibility = Visibility.Collapsed;
            }
            else
            {
                ShowLoading = Visibility.Visible;
                Thread thAutoTimeOutLogout = new Thread(() =>
                {
                    if (CustomerID != 0)
                    {

                    }

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        ShowCancelQR = ShowSaveChange = ShowLoading = ShowTimeOut = BackgroundVisibility = ShowPromptMessage = ShowCardPayment = ShowFailPayment = ShowEWallet =
                        ShowPaymentProcessing = ShowSendingKitchen = ShowQRPayment = Visibility.Collapsed;
                        GeneralVar.IOBoard.qrTrigger = GeneralVar.IOBoard.rpTrigger = GeneralVar.IOBoard.ccTrigger = false;
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("OrderStart Cancel = {0}", DateTime.Now.ToString("yy/MM/dd HH:mm:ss")), _TraceCategory);
                        SetStage(eStage.Home);
                    }));
                    ShowLoading = Visibility.Collapsed;
                });
                thAutoTimeOutLogout.Start();
            }

        }

        AutoResetEvent autoResetTimeout = new AutoResetEvent(false);
        public bool isStop = false;
        System.Timers.Timer timeRemainingTimer, timeRemainingTimeoutTimer, refreshMenuTime;
        public void StartCountDown(int second)
        {

            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "StartCountDown Starting...", _TraceCategory);
            isStop = false;
            TimeoutCountdown = new TimeSpan(0, 0, second);

            timeRemainingTimer.Start();
            IsTimeRemainingVisible = true;

            autoResetTimeout.WaitOne();
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "StartCountDown Completed.", _TraceCategory);

            //if (Stage != eStage.Home)
            TimeOutTrigger();
        }

        public void StopCountDown()
        {
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "StopCountDown Starting...", _TraceCategory);
            timeRemainingTimer.Stop();
            isStop = true;
            IsTimeRemainingVisible = false;
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "StopCountDown Completed.", _TraceCategory);
        }

        private void timeRemainingTimer_Elapsed(object sender, EventArgs e)
        {
            TimeoutCountdown = TimeoutCountdown.Subtract(new TimeSpan(0, 0, 1));
            //ShowTimeOut = Visibility.Collapsed;
            if (TimeoutCountdown.TotalSeconds > 0)
                return;
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "timeRemainingTimer_Elapsed Starting...", _TraceCategory);
            timeRemainingTimer.Stop();

            isStop = true;
            IsTimeRemainingVisible = false;
            ShowCancelQR = ShowSaveChange = ShowLoading = ShowTimeOut = BackgroundVisibility = ShowPromptMessage = ShowCardPayment = ShowFailPayment = ShowEWallet =
                ShowPaymentProcessing = ShowSendingKitchen = ShowQRPayment = Visibility.Collapsed;
            if (Stage != eStage.Home)
                ShowTimeOut = BackgroundVisibility = Visibility.Visible;
            TimeoutRenewalCountdown = new TimeSpan(0, 0, 10);
            timeRemainingTimeoutTimer.Start();

            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "timeRemainingTimer_Elapsed Completed.", _TraceCategory);
        }

        private void timeRemainingTimeoutTimer_Elapsed(object sender, EventArgs e)
        {
            TimeoutRenewalCountdown = TimeoutRenewalCountdown.Subtract(new TimeSpan(0, 0, 1));

            if (TimeoutRenewalCountdown.TotalSeconds > 0)
                return;

            timeRemainingTimeoutTimer.Stop();
            ShowTimeOut = BackgroundVisibility = Visibility.Collapsed;
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "timeRemainingTimeoutTimer_Elapsed Starting...", _TraceCategory);
            autoResetTimeout.Set();
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "timeRemainingTimeoutTimer_Elapsed Completed.", _TraceCategory);
        }

        public void ResetTimer()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "ResetTimer Starting...", _TraceCategory);
                TimeoutCountdown = new TimeSpan(0, 0, _TimeOutSecond);
                timeRemainingTimeoutTimer.Stop();
                if (isStop)
                {
                    StartTimer();
                }
                ShowTimeOut = BackgroundVisibility = Visibility.Collapsed;
                IsTimeRemainingVisible = true;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "ResetTimer Completed.", _TraceCategory);

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] ResetTimeOut:{0}", ex.ToString()), _TraceCategory);
            }
        }

        public void ResetTimer(int second)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "ResetTimer Starting...", _TraceCategory);
                TimeoutCountdown = new TimeSpan(0, 0, second);
                timeRemainingTimeoutTimer.Stop();
                if (isStop)
                {
                    StartTimer();
                }
                ShowTimeOut = BackgroundVisibility = Visibility.Collapsed;
                IsTimeRemainingVisible = true;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "ResetTimer Completed.", _TraceCategory);

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] ResetTimeOut:{0}", ex.ToString()), _TraceCategory);
            }
        }

        #endregion

        #region Maintenance

        #region Maintenance Timeout

        BackgroundWorker bwMaintenanceTimer;
        int idleDuration;
        bool _stopWithAction = false;
        Action bwMaintenanceAction;

        TimeSpan _TimeRemaining, _TimeRemainingRenewal;
        string _TimeRemainingLanguage;
        bool _IsTimeRemainingVisible = false;
        public TimeSpan TimeoutCountdown
        {
            get { return _TimeRemaining; }
            set { _TimeRemaining = value; OnPropertyChanged("TimeoutCountdown"); }
        }

        public string TimeRemainingLanguage
        {
            get { return _TimeRemainingLanguage; }
            set { _TimeRemainingLanguage = value; OnPropertyChanged("TimeRemainingLanguage"); }
        }

        public TimeSpan TimeoutRenewalCountdown
        {
            get { return _TimeRemainingRenewal; }
            set { _TimeRemainingRenewal = value; OnPropertyChanged("TimeoutRenewalCountdown"); }
        }

        public bool IsTimeRemainingVisible
        {
            get { return _IsTimeRemainingVisible; }
            set { _IsTimeRemainingVisible = value; OnPropertyChanged("IsTimeRemainingVisible"); }
        }

        private string _sTimeLeft;
        public string sTimeLeft
        {
            get { return _sTimeLeft; }
            set
            {
                _sTimeLeft = value;
                OnPropertyChanged("sTimeLeft");
            }
        }

        public string ProcessingFlow
        {
            get { return _ProcessingFlow; }
            set
            {
                _ProcessingFlow = value;
                OnPropertyChanged("ProcessingFlow");
            }
        }

        public bool IsBusy
        {
            get
            {
                if (bwMaintenanceTimer != null)
                    return bwMaintenanceTimer.IsBusy;
                return false;
            }
        }

        public void StartMaintenancebwTimer(Action action)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("StartMaintenancebwTimer Starting..."), _TraceCategory);

                this._stopWithAction = true;

                idleDuration = 60000 / 1000;
                //idleDuration += 10;
                if (this.bwMaintenanceAction != null)
                    this.bwMaintenanceAction = null;

                if (this.bwMaintenanceAction != action)
                    this.bwMaintenanceAction = action;

                if (!bwMaintenanceTimer.IsBusy)
                    bwMaintenanceTimer.RunWorkerAsync();

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("StartMaintenancebwTimer Completed."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] StartMaintenancebwTimer = {0}", ex.ToString()), _TraceCategory);
            }
        }

        public void ResetMaintenancebwTimer(Action action)
        {
            try
            {
                //Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("ResetTimer Starting..."), traceCategory);


                idleDuration = 180000 / 1000;
                idleDuration += 10;
                _stopWithAction = true;
                if (this.bwMaintenanceAction != null)

                    if (this.bwMaintenanceAction != action)
                    {
                        this.bwMaintenanceAction = null;
                        this.bwMaintenanceAction = action;
                    }

                if (bwMaintenanceTimer != null && !bwMaintenanceTimer.IsBusy)
                    bwMaintenanceTimer.RunWorkerAsync();

                //Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("ResetTimer Completed."), traceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] ResetMaintenancebwTimer = {0}", ex.ToString()), _TraceCategory);
            }
        }

        public void CancelMaintenancebwTimer(bool stopWithAction)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("CancelMaintenancebwTimer Starting..."), _TraceCategory);

                this._stopWithAction = stopWithAction;
                idleDuration = 60000 / 1000;

                if (bwMaintenanceTimer.IsBusy && bwMaintenanceTimer.WorkerSupportsCancellation)
                    bwMaintenanceTimer.CancelAsync();

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("CancelMaintenancebwTimer Completed."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] CancelMaintenancebwTimer = {0}", ex.ToString()), _TraceCategory);
            }
        }

        void bwMaintenanceTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "bwMaintenanceTimer_DoWork Starting...", _TraceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("bwMaintenanceTimer_DoWork [Duration]= {0}", 180000), _TraceCategory);


                BackgroundWorker bw = sender as BackgroundWorker;

                while (!bw.CancellationPending && idleDuration > 1)
                {
                    idleDuration--;
                    if (idleDuration < 10)
                        bw.ReportProgress(idleDuration, "p");
                    else
                        bw.ReportProgress(idleDuration, "v");
                    Thread.Sleep(1000);
                }
                bw.ReportProgress(idleDuration, "c");
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error]bwMaintenanceTimer_DoWork = {0}", ex.ToString()), _TraceCategory);
            }
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "bwMaintenanceTimer_DoWork Starting...", _TraceCategory);
        }

        void bwMaintenanceTimer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ShowTimeOut = BackgroundVisibility = Visibility.Collapsed;
            IsTimeRemainingVisible = true;
            TimeoutCountdown = new TimeSpan(0, 0, e.ProgressPercentage);
            TimeRemainingLanguage = "Time Remaining";
            sTimeLeft = TimeRemainingLanguage + TimeoutCountdown.ToString();

        }

        void bwMaintenanceTimer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("bwTimer_RunWorkerCompleted Starting..."), _TraceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("bwTimer_RunWorkerCompleted [_stopWithAction = {0}]", _stopWithAction), _TraceCategory);
                if (bwMaintenanceAction != null && _stopWithAction)
                    bwMaintenanceAction();

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("bwTimer_RunWorkerCompleted Completed"), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] bwTimer_RunWorkerCompleted {0}", ex.ToString()), _TraceCategory);
            }
        }

        public void BackToInitializing()
        {
            try
            {
                if (Stage == eStage.MaintenanceLogin)
                {
                    if (!UnderMaintenance)
                    {
                        GeneralVar.MainWindowVM.SetStage(eStage.Initializing);
                        PreLoad(false);
                    }
                    else
                    {
                        SetStage(eStage.Offline, "Manually Set Offline");
                    }
                    //GeneralVar.MainWindowVM.SetStage(eStage.Initializing);
                    //PreLoad(false);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] BackToInitializing = {0}", ex.ToString()), _TraceCategory);
            }
        }

        public void TriggerAlarm()
        {
            try
            {
                if (GeneralVar.IOBoard_Enabled)
                {
                    GeneralVar.IOBoard.alarmTrigger = true;
                }
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region LoginCommand

        private string _Password;
        private string _Username;

        private bool _IsError = false;
        private string _ErrorMessage = string.Empty;
        private bool _ValidUser = true;
        private bool _ValidPassword = true;
        private string _ProcessingFlow = string.Empty;

        public string Username
        {
            get { return _Username; }
            set
            {
                _Username = value;
                OnPropertyChanged("Username");
            }
        }

        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                OnPropertyChanged("Password");
                OnPropertyChanged("PasswordIsEmpty");
            }
        }

        private string _IntructionDescription = "";
        public string IntructionDescription
        {
            get { return _IntructionDescription; }
            set
            {
                _IntructionDescription = value;
                OnPropertyChanged("IntructionDescription");
            }
        }

        private string _OnOffStatus = "On Line";
        public string OnOffStatus
        {
            get { return _OnOffStatus; }
            set
            {
                _OnOffStatus = value;
                OnPropertyChanged("OnOffStatus");
            }
        }

        public bool PasswordIsEmpty
        {
            get { return string.IsNullOrEmpty(Password); }
        }

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set
            {
                _ErrorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

        public bool ValidUser
        {
            get { return _ValidUser; }
            set
            {
                _ValidUser = value;
                OnPropertyChanged("ValidUser");
            }
        }

        public bool ValidPassword
        {
            get { return _ValidPassword; }
            set
            {
                _ValidPassword = value;
                OnPropertyChanged("ValidPassword");

            }
        }

        public bool IsError
        {
            get { return _IsError; }
            set
            {
                _IsError = value;
                OnPropertyChanged("IsError");
            }
        }

        ICommand _LoginCommand;
        public ICommand LoginCommand
        {
            get
            {
                if (_LoginCommand == null)
                    _LoginCommand = new RelayCommand(Login);

                return _LoginCommand;
            }
        }

        void Login()
        {
            try
            {
                Thread t = new Thread(() =>
                {
                    try
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Thread t starting..."), _TraceCategory);
                        ShowLoading = Visibility.Visible;
                        bool vUser, vPassword, vAccessRight;
                        User user = new User();


                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Login Starting..."), _TraceCategory);

                        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                        {
                            setErrorValue(!string.IsNullOrEmpty(Username), !string.IsNullOrEmpty(Password), "Username and Password cannot be empty!");
                            ShowLoading = Visibility.Collapsed;
                            return;
                        }
                        //bool ValidAccessRight = false;

                        //if (user.UserAuthentication(Username, Password, out vUser, out vPassword, out vAccessRight, "Kiosk_Access")) // use this when CMS deploy
                        if (Username == "sysadmin" && Password == "sysadmin")
                        {

                            if (GeneralVar.IOBoard_Enabled)
                            {
                                GeneralVar.IOBoard.alarmTrigger = false;
                            }

                            CancelMaintenancebwTimer(false);
                            DisplayMaintenanceList();
                            SetStage(eStage.MaintenanceSelection);
                        }
                        //else if (!vUser)
                        else if (Password != "sysadmin")
                            setErrorValue(_ValidUser, _ValidPassword, "Invalid Username");
                        //else if (!vPassword)
                        else if (Username != "sysadmin")
                            setErrorValue(_ValidUser, _ValidPassword, "Invalid Password");
                        else
                            ErrorMessage = "Access Denied";
                        //ValidUser = vUser;
                        //ValidPassword = vPassword;

                        ShowLoading = Visibility.Collapsed;
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Thread t done..."), _TraceCategory);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Thread t error - {0}", ex.ToString()), _TraceCategory);
                    }

                });
                t.Start();

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Login Completed."), _TraceCategory);
            }
            catch (Exception ex)
            {
                ShowLoading = Visibility.Collapsed;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] Login = {0}", ex.ToString()), _TraceCategory);

            }
        }

        void setErrorValue(bool vUser, bool vPassword, string error)
        {
            ValidUser = vUser;
            ValidPassword = vPassword;
            IsError = !(vUser && vPassword);
            ErrorMessage = error;
        }

        #endregion

        public class MaintenanceList : Base
        {
            public string DisplayCode { get; set; }
            public ICommand Command { get; set; }
            public eMaintenanceTask CommandParameter { get; set; }
            public bool IsEnabled { get; set; }

            public MaintenanceList(string displayCode, string displayName, ICommand command, eMaintenanceTask commandParameter)
            {
                DisplayCode = displayCode;
                DisplayName = displayName;
                Command = command;
                CommandParameter = commandParameter;
            }

            public MaintenanceList(string displayCode, string displayName, ICommand command, eMaintenanceTask commandParameter, bool isEnabled)
            {
                DisplayCode = displayCode;
                DisplayName = displayName;
                Command = command;
                CommandParameter = commandParameter;
                IsEnabled = isEnabled;
            }
        }

        #region MaintenanceCommand

        ICommand _MaintenanceCommand;
        public ICommand MaintenanceCommand
        {
            get
            {
                if (_MaintenanceCommand == null)
                    _MaintenanceCommand = new RelayCommand<eMaintenanceTask>(MaintenanceSetting);

                return _MaintenanceCommand;
            }
        }

        ObservableCollection<MaintenanceList> _MaintenanceListCollection;
        public ObservableCollection<MaintenanceList> MaintenanceListCollection
        {
            get { return _MaintenanceListCollection; }
            set
            {
                _MaintenanceListCollection = value;
                OnPropertyChanged("MaintenanceListCollection");
            }
        }

        void MaintenanceSetting(eMaintenanceTask command)
        {
            try
            {
                ErrorMessage = string.Empty;
                ProcessingFlow = string.Empty;
                TimeSpan startOperation = GeneralVar.OpeningTime.TimeOfDay;
                DateTime endOperation = GeneralVar.ClosingTime;
                TimeSpan morningSession = new TimeSpan(9, 59, 59);
                TimeSpan midnightStartSession = new TimeSpan(00, 00, 01);
                TimeSpan midnightEndSession = new TimeSpan(3, 00, 01);
                //MaintenanceBackButtonLabel = "Cancel";
                //ShowFunctional = Visibility.Visible;
                switch (command)
                {
                    /*TO DO: ADD Maintenance Setting*/
                    case eMaintenanceTask.RestartApp:
                        RestartApp();
                        break;
                    case eMaintenanceTask.ShutdownApp:
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Shut Down Application."), _TraceCategory);
                        ShutDownApp();
                        break;
                    case eMaintenanceTask.Logout:
                        if (!UnderMaintenance)
                        {
                            GeneralVar.MainWindowVM.SetStage(eStage.Initializing);
                            PreLoad(false);
                        }
                        else
                        {
                            SetStage(eStage.Offline, "Manually Set Offline");
                        }
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Logout from Maintenance Mode"), _TraceCategory);
                        break;
                    case eMaintenanceTask.ReprintTransaction:
                        PrepareReceiptList();
                        SetStage(eStage.MaintenanceReprint);
                        break;
                    case eMaintenanceTask.OnOffApp:
                        if (UnderMaintenance)
                        {
                            UnderMaintenance = false;
                            OnOffStatus = "On Line";
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Change Offline to Online"), _TraceCategory);
                        }
                        else
                        {
                            UnderMaintenance = true;
                            OnOffStatus = "Off Line";
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Change Online to Offline"), _TraceCategory);
                        }
                        break;
                    case eMaintenanceTask.TestingMode:
                        TestingMode = !TestingMode;
                        if (TestingMode)
                        {
                        }
                        break;
                    default:
                        actionTask = command;
                        DisplayMaintenanceFunction(command);
                        SetStage(eStage.MaintenanceAction);
                        //ContructMaintenanceView(eMaintenanceTask.SettlementTime);
                        break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] MaintenanceSetting:{0}", ex.ToString()), _TraceCategory);
            }

        }

        void RestartApp()
        {
            try
            {
                System.Windows.Forms.Application.Restart();
                App.Current.Shutdown(1);
            }
            catch (Exception)
            {

            }
        }

        void ShutDownApp()
        {
            try
            {
                App.Current.Shutdown(1);
            }
            catch (Exception)
            {

            }
        }

        void DisplayMaintenanceList()
        {
            try
            {
                IntructionDescription = string.Format(@"
                    Hi,
                      Please the maintenance module to operate.

                    Module 1. Test Print
                    To reprint receipt for last 5 transaction.
  
                      Module 2. Reprint
                    To reprint receipt for last 5 transaction.

                      Module 3. Void
                    To void payment for last 5 transaction.

                      Module 4. EDC Settlement
                    To do manual Credit Card Terminal Settlement with Bank Host

                      Module 5. Cancel Order
                    To cancel order for last 5 cash transaction.

                      Module 6. Overriding Order
                    To overriding  order for last 5 cash transaction.

                      Module 7. Restart Application
                    To Restart 'PBSSSK' application

                      Module 8. Shutdown Application
                    To Shutdown 'PBSSSK' application

                      Module 0. Logout
                    To Logout Maintenance mode and close the door to continue operation.
");


                if (MaintenanceListCollection != null)
                    MaintenanceListCollection = null;

                if (_isMenuExclusion)
                {
                    _isMenuExclusion = false;
                    MaintenanceListCollection = new ObservableCollection<MaintenanceList>();
                    MaintenanceListCollection.Add(new MaintenanceList("1", "Menu Exclusion", MaintenanceCommand, eMaintenanceTask.MenuExclusion, true));
                }
                else
                {
                    MaintenanceListCollection = new ObservableCollection<MaintenanceList>();
                    MaintenanceListCollection.Add(new MaintenanceList("1", "Test Print", MaintenanceCommand, eMaintenanceTask.TestPrintTransaction, true));
                    MaintenanceListCollection.Add(new MaintenanceList("2", "Reprint", MaintenanceCommand, eMaintenanceTask.ReprintTransaction, true));
                    MaintenanceListCollection.Add(new MaintenanceList("3", "Bank Settlement", MaintenanceCommand, eMaintenanceTask.BankSettlement, true));
                    MaintenanceListCollection.Add(new MaintenanceList("4", "Shutdown Application", MaintenanceCommand, eMaintenanceTask.ShutdownApp, true));
                    MaintenanceListCollection.Add(new MaintenanceList("5", "[On-Off] Operation", MaintenanceCommand, eMaintenanceTask.OnOffApp, true));
                    //MaintenanceListCollection.Add(new MaintenanceList("6", "[On-Off] Testing Mode", MaintenanceCommand, eMaintenanceTask.TestingMode, true));
                    //MaintenanceListCollection.Add(new MaintenanceList("6", "Menu Exclusion", MaintenanceCommand, eMaintenanceTask.MenuExclusion, true));
                }
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region MaintenanceActionCommand

        public class MaintenanceActionList : Base
        {
            public string CheckNo { get; set; }
            public string Amount { get; set; }
            public string sDateTime { get; set; }
            public string SalesType { get; set; }
            public string PaymentMethod { get; set; }
            public string PaymentNo { get; set; }
            public string sDetail { get; set; }
            public ICommand Command { get; set; }

            public MaintenanceActionList(string displayCode, string displayName, ICommand command)
            {
                CheckNo = displayCode;
                DisplayName = displayName;
                Command = command;
            }
        }

        eMaintenanceTask actionTask = eMaintenanceTask.OverridingOrder;

        private string _MaintenanceModule = "";
        public string MaintenanceModule
        {
            get { return _MaintenanceModule; }
            set
            {
                _MaintenanceModule = value;
                OnPropertyChanged("MaintenanceModule");
            }
        }

        private string _sActionButton = "";
        public string sActionButton
        {
            get { return _sActionButton; }
            set
            {
                _sActionButton = value;
                OnPropertyChanged("sActionButton");
            }
        }

        private Visibility _vSalesMenu = Visibility.Collapsed;
        public Visibility vSalesMenu
        {
            get { return _vSalesMenu; }
            set
            {
                _vSalesMenu = value;
                OnPropertyChanged("vSalesMenu");
            }
        }

        ICommand _ActionBackCommand;
        public ICommand ActionBackCommand
        {
            get
            {
                if (_ActionBackCommand == null)
                    _ActionBackCommand = new RelayCommand(BackToMaintenanceMain);

                return _ActionBackCommand;
            }
        }

        ICommand _ActionCommand;
        public ICommand ActionCommand
        {
            get
            {
                if (_ActionCommand == null)
                    _ActionCommand = new RelayCommand(ActionMaintenance);

                return _ActionCommand;
            }
        }

        private string _sActionSuccessMsg = "";
        public string sActionSuccessMsg
        {
            get { return _sActionSuccessMsg; }
            set
            {
                _sActionSuccessMsg = value;
                OnPropertyChanged("sActionSuccessMsg");
            }
        }

        private string _sActionFailMsg = "";
        public string sActionFailMsg
        {
            get { return _sActionFailMsg; }
            set
            {
                _sActionFailMsg = value;
                OnPropertyChanged("sActionFailMsg");
            }
        }

        public Visibility ShowMorning
        {
            get { return _ShowMorning; }
            set
            {
                _ShowMorning = value;
                OnPropertyChanged("ShowMorning");
            }
        }

        public Visibility ShowNormalHour
        {
            get { return _ShowNormalHour; }
            set
            {
                _ShowNormalHour = value;
                OnPropertyChanged("ShowNormalHour");
            }
        }

        public Visibility ShowMidnight
        {
            get { return _ShowMidnight; }
            set
            {
                _ShowMidnight = value;
                OnPropertyChanged("ShowMidnight");
            }
        }

        public Visibility ShowSaveChange
        {
            get { return _ShowSaveChange; }
            set
            {
                _ShowSaveChange = value;
                OnPropertyChanged("ShowSaveChange");
            }
        }

        public string Label_SaveChangeMessage
        {
            get
            {
                return "DO YOU WANT TO SAVE THE CHANGES?";
            }
        }

        ObservableCollection<MaintenanceList> _MaintenanceActionCollection;
        public ObservableCollection<MaintenanceList> MaintenanceActionCollection
        {
            get { return _MaintenanceActionCollection; }
            set
            {
                _MaintenanceActionCollection = value;
                OnPropertyChanged("MaintenanceActionCollection");
            }
        }

        void DisplayMaintenanceFunction(eMaintenanceTask task)
        {
            try
            {
                ShowLoading = Visibility.Visible;
                MaintenanceModule = task.ToString();
                sActionButton = task.ToString();
                sActionFailMsg = "";
                sActionSuccessMsg = "";

                switch (task)
                {
                    case eMaintenanceTask.ReprintTransaction:
                        vSalesMenu = Visibility.Visible;
                        break;
                    case eMaintenanceTask.VoidTransaction:
                        //MaintenanceActionCollection = LoadLastTransaction(1);
                        vSalesMenu = Visibility.Visible;
                        break;
                    case eMaintenanceTask.BankSettlement:
                        MaintenanceModule = "Bank Settlement";
                        sActionButton = "Bank Settlement";
                        vSalesMenu = Visibility.Collapsed;
                        break;
                    case eMaintenanceTask.TestPrintTransaction:
                        MaintenanceModule = "Test Print";
                        sActionButton = "Test Print";
                        vSalesMenu = Visibility.Collapsed;
                        break;
                    case eMaintenanceTask.CancelOrder:
                        //MaintenanceActionCollection = LoadLastTransaction(2);
                        vSalesMenu = Visibility.Visible;
                        break;
                    case eMaintenanceTask.OverridingOrder:
                        //MaintenanceActionCollection = LoadLastTransaction(1);
                        vSalesMenu = Visibility.Visible;
                        break;
                }

                ShowLoading = Visibility.Collapsed;
            }
            catch (Exception)
            {

            }
        }

        Visibility _ShowSettlementResult = Visibility.Collapsed;
        public Visibility ShowSettlementResult
        {
            get { return _ShowSettlementResult; }
            set
            {
                _ShowSettlementResult = value;
                OnPropertyChanged("ShowSettlementResult");
            }
        }

        string _SettlementResult;
        public string SettlementResult
        {
            get { return _SettlementResult; }
            set
            {
                _SettlementResult = value;
                OnPropertyChanged("SettlementResult");
            }
        }

        void ActionMaintenance()
        {
            try
            {
                sActionFailMsg = "";
                sActionSuccessMsg = "";
                ShowLoading = Visibility.Visible;
                string errMsg = "";
                bool result = false;

                switch (actionTask)
                {
                    case eMaintenanceTask.ReprintTransaction:
                        //result = Reprint(out errMsg);
                        vSalesMenu = Visibility.Visible;
                        break;
                    case eMaintenanceTask.VoidTransaction:
                        break;
                    case eMaintenanceTask.BankSettlement:
                        SettlementResult = string.Empty;
                        ShowSettlementResult = Visibility.Visible;
                        result = EDCSettlement(out errMsg);
                        break;
                    case eMaintenanceTask.TestPrintTransaction:
                        result = TestPrint(out errMsg);
                        vSalesMenu = Visibility.Collapsed;
                        break;
                    case eMaintenanceTask.CancelOrder:
                        //MaintenanceActionCollection = LoadLastTransaction(2);
                        vSalesMenu = Visibility.Visible;
                        break;
                    case eMaintenanceTask.OverridingOrder:
                        //MaintenanceActionCollection = LoadLastTransaction(1);
                        vSalesMenu = Visibility.Visible;
                        break;
                }

                if (result)
                {
                    sActionSuccessMsg = errMsg;
                }
                else
                {
                    sActionFailMsg = errMsg;
                }

                ShowLoading = Visibility.Collapsed;
            }
            catch (Exception)
            {

            }
        }

        void BackToMaintenanceMain()
        {
            ShowSettlementResult = Visibility.Collapsed;
            SetStage(eStage.MaintenanceSelection);
        }

        public bool EDCSettlement(out string err)
        {
            bool result = false;
            err = "";

            try
            {
                Thread th = new Thread(() =>
                {
                    try
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            ShowLoading = Visibility.Visible;
                        }));

                        PerformCardbizSettlement();

                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            ShowLoading = Visibility.Collapsed;
                        }));
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] Thread EDCSettlement = {0}", ex.ToString()), _TraceCategory);
                    }
                });
                th.Start();
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] EDCSettlement = {0}", ex.ToString()), _TraceCategory);
            }
            return result;
        }

        bool PerformCardbizSettlement()
        {
            bool success = false;
            try
            {
                KeepAlive _CardBizLastAliveRequest;
                KeepAlive_Resp _CardBizLastAliveResponse = new KeepAlive_Resp();
                Settlement_Resp _CardBizLastSettlementResponse = new Settlement_Resp();
                Settlement _CardBizLastSettlementRequest = new Settlement(GeneralVar.CC_PortName, true);

                _CardBizLastSettlementResponse = _CardBizLastSettlementRequest.SendRequest("", "", "0", 300);

                do
                {
                    _CardBizLastAliveRequest = new KeepAlive(GeneralVar.CC_PortName, false);
                    _CardBizLastAliveResponse = _CardBizLastAliveRequest.SendRequest();

                    if (_CardBizLastAliveResponse.ResponseCode == "CP")
                    {
                        System.Threading.Thread.Sleep(1000);
                    }

                } while (_CardBizLastAliveResponse.ResponseCode == "CP");

                foreach (var a in _CardBizLastSettlementResponse.SettlementInfoList)
                {
                    SettlementResult += string.Format("Settlement {0} Starting... \r\n", a.HostName);
                    string settlementSummary = string.Format(@"
**********************************
{0} Settlement Completed
**********************************
Status Code: {1}
Host Name: {2}
Batch No: {3}
Batch Count: {4}
Batch Amount: {5}
**********************************",
                    a.HostName, a.ResponseCode + "-" + a.ResponseText, a.HostName, a.BatchNo, a.SalesCnt, a.SalesAmt);
                    SettlementResult += settlementSummary + "\r\n";
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, settlementSummary, _TraceCategory);
                }
                success = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] PerformCardbizSettlement = {0}", ex.ToString()), _TraceCategory);
            }
            return success;
        }

        public bool TestPrint(out string err)
        {
            bool result = false;
            err = "";
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("TestPrint Starting..."), _TraceCategory);

                Sales_Resp response = new Sales_Resp();

                ObservableCollection<CartModel.Product> testBill = new ObservableCollection<CartModel.Product>();
                result = GeneralVar.DocumentPrint.Print_CardReceipt(true, 0m, KioskId, "TEST PRINT", "TEST", testBill, response, 0m, 0, 0, 0, 0, StoreName);
                err = GeneralVar.DocumentPrint.LastError;

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("TestPrint Completed."), _TraceCategory);
            }
            catch (Exception ex)
            {
                err = ex.Message;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] TestPrint = {0}", ex.ToString()), _TraceCategory);
            }

            return result;
        }

        #endregion

        #endregion

        #region LFFSSK Page View

        OffilneView vOffline = new OffilneView();
        SystemInitView vSystemInit;
        HomeView vHome = new HomeView();
        AuthenticationView vMaintenanceAuthorization = new AuthenticationView();
        SystemMaintenanceModuleListView vSystemMaintenance = new SystemMaintenanceModuleListView();
        SystemMaintenanceActionView vSystemMaintenanceAction = new SystemMaintenanceActionView();
        SystemMaintenanceReprintView vSystemMaintenanceReprint = new SystemMaintenanceReprintView();

        public MenuPage vMenuPage = null;
        public MenuDetails vMenuDetails = null;
        OrderSummary vOrderSummary = null;
        EditItem vEditItem = null;
        AddedItemPage vAddedItem = null;
        PaymentMethodPage vPayMethod = null;
        OrderNumPage vOrderNum = null;
        VoucherPage vVoucher = null;
        PaymentByCardView vCardView = null;
        PaymentByQRView vQrView = null;

        MediaPlayerView vMediaPlayerView;
        MediaPlayerView2 vMediaPlayerView2;

        public void SetStage(eStage stage, string param = null)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("SetStage Starting..."), _TraceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("SetStage Current Stage = {0}", this.Stage), _TraceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("SetStage New Stage = {0}", stage), _TraceCategory);

                if (param != null)
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("SetStage param = {0}", param), _TraceCategory);

                this.Stage = stage;

                switch (Stage)
                {
                    case eStage.Offline:
                    case eStage.OffOperation:
                    case eStage.OutOfOrder:
                        StopCountDown();
                        if (UnderMaintenance)
                            SystemError = "Manually set OFFLINE MODE";

                        LogoBarHeight = 80;
                        LogoBarWidth = 1080;
                        MenuBannerVisibility = Visibility.Visible;
                        if (vOffline == null)
                            vOffline = new OffilneView();
                        DesignView = vOffline;
                        break;

                    case eStage.Initializing:
                        if (vSystemInit == null)
                            vSystemInit = new SystemInitView();
                        DesignView = vSystemInit;

                        UVAlign = "Center";
                        break;

                    case eStage.Home:
                        StartTimer();

                        //UVAlign = "Top";
                        LogoBarHeight = 80;
                        LogoBarWidth = 1080;

                        if (vMediaPlayerView != null)
                            vMediaPlayerView = null;
                        vMediaPlayerView = new MediaPlayerView();
                        vMediaPlayerView.StartBannerHome();
                        HomeBannerView = vMediaPlayerView;

                        if (vMediaPlayerView2 != null)
                            vMediaPlayerView2.StopBannerMenu();

                        Language("EN");
                        CollapseAllTrigger();
                        Refresh();

                        if (vHome == null)
                            vHome = new HomeView();
                        DesignView = vHome;
                        break;

                    case eStage.MaintenanceLogin:
                        StopCountDown();
                        if (vMaintenanceAuthorization == null)
                            vMaintenanceAuthorization = new AuthenticationView();
                        DesignView = vMaintenanceAuthorization;
                        break;

                    case eStage.MaintenanceSelection:
                        StopCountDown();
                        if (vSystemMaintenance == null)
                            vSystemMaintenance = new SystemMaintenanceModuleListView();
                        DesignView = vSystemMaintenance;
                        break;

                    case eStage.MaintenanceAction:
                        StopCountDown();
                        if (vSystemMaintenanceAction == null)
                            vSystemMaintenanceAction = new SystemMaintenanceActionView();
                        DesignView = vSystemMaintenanceAction;
                        break;

                    case eStage.MaintenanceReprint:
                        StopCountDown();
                        if (vSystemMaintenanceReprint == null)
                            vSystemMaintenanceReprint = new SystemMaintenanceReprintView();
                        DesignView = vSystemMaintenanceReprint;
                        break;

                    case eStage.MaintenanceVoid:
                        StopCountDown();
                        if (vMaintenanceAuthorization == null)
                            vMaintenanceAuthorization = new AuthenticationView();
                        DesignView = vMaintenanceAuthorization;
                        break;

                    case eStage.MenuItem:
                        ResetTimer(180);

                        LogoBarHeight = 300;
                        LogoBarWidth = 200;

                        if (vMediaPlayerView2 != null)
                            vMediaPlayerView2 = null;
                        vMediaPlayerView2 = new MediaPlayerView2();
                        vMediaPlayerView2.StartBannerMenu();
                        MenuBannerView = vMediaPlayerView2;

                        if (vMediaPlayerView != null)
                            vMediaPlayerView.StopBannerHome();

                        //UVAlign = "Center";
                        BannerAdsVisibility = Visibility.Collapsed;
                        DetailsViewVisbility = Visibility.Collapsed;
                        MenuBannerVisibility = Visibility.Collapsed;
                        if (LoginPopUp)
                        {
                            LoginVisbility = Visibility.Visible;
                            BackgroundVisibility = Visibility.Visible;
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Login Popup Visible..."), _TraceCategory);
                            readyToScan = true;
                        }

                        if (vMenuPage == null)
                            vMenuPage = new MenuPage();
                        vMenuPage.InitializeSlider();
                        DesignView = vMenuPage;
                        break;
                    case eStage.MenuCustomize:
                        ResetTimer(180);

                        if (vMenuDetails == null)
                            vMenuDetails = new MenuDetails();
                        vMenuDetails.InitializeSlider();
                        BackgroundVisibility = DetailsViewVisbility = Visibility.Visible;
                        DetailView = vMenuDetails;
                        break;
                    case eStage.OrderSummary:
                        ResetTimer(180);
                        //BannerAdsVisibility = Visibility.Collapsed;

                        if (vOrderSummary == null)
                            vOrderSummary = new OrderSummary();
                        vOrderSummary.InitializeSlider();
                        DesignView = vOrderSummary;

                        break;
                    case eStage.EditItem:
                        StopCountDown();
                        if (vEditItem == null)
                            vEditItem = new EditItem();
                        BackgroundVisibility = DetailsViewVisbility = Visibility.Visible;
                        vEditItem.InitializeSlider();
                        DetailView = vEditItem;
                        break;
                    case eStage.DoneAddCart:
                        ResetTimer(180);
                        if (vAddedItem == null)
                            vAddedItem = new AddedItemPage();
                        DetailView = vAddedItem;
                        break;
                    case eStage.PaymentMethodSelection:
                        ResetTimer(180);
                        if (vPayMethod == null)
                            vPayMethod = new PaymentMethodPage();


                        BackgroundVisibility = DetailsViewVisbility = Visibility.Visible;
                        DetailView = vPayMethod;
                        //DesignView = vPayMethod;
                        break;
                    case eStage.FinalPage:
                        StopCountDown();
                        DetailsViewVisbility = Visibility.Collapsed;
                        if (vOrderNum == null)
                            vOrderNum = new OrderNumPage();
                        DesignView = vOrderNum;

                        break;
                    case eStage.Voucher:
                        ResetTimer(180);
                        if (vVoucher == null)
                            vVoucher = new VoucherPage();
                        DetailView = vVoucher;
                        break;
                    case eStage.PaymentByCard:
                        StopCountDown();
                        if (vCardView == null)
                            vCardView = new PaymentByCardView();
                        DetailView = vCardView;
                        break;
                    case eStage.PaymentByQR:
                        StopCountDown();
                        if (vQrView == null)
                            vQrView = new PaymentByQRView();
                        DetailView = vQrView;
                        break;
                    default:
                        throw new Exception("Invalid Stage");
                }
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("SetStage Completed."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] SetStage = {0}", ex.ToString()), _TraceCategory);
            }
        }

        #endregion

        #region LFFSSK Common Function

        //public List<Order.OrderRequest> orderRequests;
        public List<BillListModel> mBillList;
        string printingRemarks = string.Empty;
        bool isCardPayment = false;
        public int CustomerID;
        public string AccessToken;
        public int eatMethod;
        Xilnex.Request orderDetail;
        ApiModel.InitialOrderRequest orderRequest;
        //Order.OrderPayment payment;
        string paymentCode = string.Empty;
        int paymentId;

        private string _TxtErrorHeader;
        public string TxtErrorHeader
        {
            get { return _TxtErrorHeader; }
            set
            {
                _TxtErrorHeader = value;
                OnPropertyChanged(nameof(TxtErrorHeader));
            }
        }

        private string _TxtErrorMessage;
        public string TxtErrorMessage
        {
            get { return _TxtErrorMessage; }
            set
            {
                _TxtErrorMessage = value;
                OnPropertyChanged(nameof(TxtErrorMessage));
            }
        }

        public string AppVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        string _KioskId;
        public string KioskId
        {
            get { return _KioskId; }
            set
            {
                _KioskId = value;
                OnPropertyChanged("KioskId");
            }
        }

        private string _StoreName;

        public string StoreName
        {
            get { return _StoreName; }
            set
            {
                _StoreName = value;
                OnPropertyChanged(nameof(StoreName));
            }
        }


        DateTime _CurrentDate;
        public DateTime CurrentDate
        {
            get { return _CurrentDate; }
            set
            {
                _CurrentDate = value;
                OnPropertyChanged("CurrentDate");
                OnPropertyChanged("sDate");
                OnPropertyChanged("sTime");
            }
        }
        public string sDate
        {
            get { return _CurrentDate.ToString("dd MMM yyyy"); }
        }

        public string sTime
        {
            get { return _CurrentDate.ToString("hh:mm tt"); }
        }

        private double _LogoBarWidth;

        public double LogoBarWidth
        {
            get { return _LogoBarWidth; }
            set
            {
                _LogoBarWidth = value;
                OnPropertyChanged(nameof(LogoBarWidth));
            }
        }

        private double _LogoBarHeight;

        public double LogoBarHeight
        {
            get { return _LogoBarHeight; }
            set
            {
                _LogoBarHeight = value;
                OnPropertyChanged(nameof(LogoBarHeight));
            }
        }

        private Visibility _BannerAdsVisibility;

        public Visibility BannerAdsVisibility
        {
            get { return _BannerAdsVisibility; }
            set
            {
                _BannerAdsVisibility = value;
                OnPropertyChanged(nameof(BannerAdsVisibility));
            }
        }

        private Visibility _WarningMessageBoxVisibility;

        public Visibility WarningMessageBoxVisibility
        {
            get { return _WarningMessageBoxVisibility; }
            set
            {
                _WarningMessageBoxVisibility = value;
                OnPropertyChanged(nameof(WarningMessageBoxVisibility));
            }
        }

        private Visibility _TestLoginVisibility;

        public Visibility TestLoginVisibility
        {
            get { return _TestLoginVisibility; }
            set
            {
                _TestLoginVisibility = value;
                OnPropertyChanged(nameof(TestLoginVisibility));
            }
        }



        private string _CateName;

        public string CateName
        {
            get { return _CateName; }
            set
            {
                _CateName = value;
                OnPropertyChanged(nameof(CateName));
            }
        }

        private int _CateId;

        public int CateId
        {
            get { return _CateId; }
            set
            {
                _CateId = value;
                OnPropertyChanged(nameof(CateId));
            }
        }


        private string _VoucherCode;

        public string VoucherCode
        {
            get { return _VoucherCode; }
            set
            {
                _VoucherCode = value;
                OnPropertyChanged(nameof(VoucherCode));
            }
        }

        private List<Xilnex.Request.SalesItem> _Order;

        public List<Xilnex.Request.SalesItem> Order
        {
            get { return _Order; }
            set
            {
                _Order = value;
                OnPropertyChanged(nameof(Order));
            }
        }
        public void Refresh()
        {
            try
            {
                BarcodeInfo = string.Empty;
                CartItem = 0;
                TotalAmount = 0;
                CustomerPhoneNo = string.Empty;
                //CustomerDetails = null;
                BackgroundVisibility = Visibility.Collapsed;
                CancelVisibility = Visibility.Collapsed;
                ShowTimeOut = Visibility.Collapsed;
                QRPopUpVisbility = Visibility.Collapsed;
                LogOutBorderVisibility = Visibility.Collapsed;
                LogInBorderVisibility = Visibility.Visible;
                PointGridVisibility = Visibility.Collapsed;
                DetailsViewVisbility = Visibility.Collapsed;
                LoginVisbility = Visibility.Collapsed;
                VoucherPopupVisibility = Visibility.Collapsed;
                WarningMessageBoxVisibility = Visibility.Collapsed;
                CheckOutContentVisibility = Visibility.Collapsed;
                SelectedOption = "EN";
                //disable login, comment loginpopup
                LoginPopUp = true;
                VoucherCode = string.Empty;
                //VoucherList = null;
                AnWVoucherAmount = 0;
                AnWGiftVoucher = 0;
                AnWRounding = 0;
                CustomerID = 0;
                BorderHeight = 1300;
                VoucherQty = 0;

                EarnPoint = 0;
                OrderNum = string.Empty;

                //reset delay order
                DelayOrderPopupVisibility = Visibility.Collapsed;
                IsDelaySentOrder = false;

                if (printCardResponse != null)
                    printCardResponse = null;

                _PaymentCategoryId = "";
                _PaymentCategoryName = "";

                if (customerLoginMenu)
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] Clearing Customer Category Menu"), _TraceCategory);

                }
                else
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] Customer Category Menu No exist"), _TraceCategory);

                customerLoginMenu = false;
                //clear all parameter
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Refresh = {0}", ex.Message), _TraceCategory);
            }
        }

        public void CollapseAllTrigger()
        {
            try
            {
                AdsVisibility = ShowTimeOut = BackgroundVisibility = Visibility.Collapsed;
                ShowLoading = Visibility.Collapsed;
                ShowPromptMessage = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] CollapseAllTrigger = {0}", ex.Message), _TraceCategory);
            }
        }

        ICommand _TimeoutYesCommand;
        public ICommand TimeoutYesCommand
        {
            get
            {
                if (_TimeoutYesCommand == null)
                    _TimeoutYesCommand = new RelayCommand<string>(TimeoutYes);
                return _TimeoutYesCommand;
            }
        }

        ICommand _TimeoutNoCommand;
        public ICommand TimeoutNoCommand
        {
            get
            {
                if (_TimeoutNoCommand == null)
                    _TimeoutNoCommand = new RelayCommand<string>(TimeoutNo);
                return _TimeoutNoCommand;
            }
        }

        private ICommand _VoucherPopupClose;

        public ICommand VoucherPopupClose
        {
            get
            {
                if (_VoucherPopupClose == null)
                    _VoucherPopupClose = new RelayCommand(VoucherPopupCloseAction);
                return _VoucherPopupClose;
            }
        }

        private ICommand _WarningPopupClose;

        public ICommand WarningPopupClose
        {
            get
            {
                if (_WarningPopupClose == null)
                    _WarningPopupClose = new RelayCommand(WarningPopupCloseAction);
                return _WarningPopupClose;
            }
        }

        private ICommand _TestLogin;

        public ICommand TestLogin
        {
            get
            {
                if (_TestLogin == null)
                    _TestLogin = new RelayCommand(TestingModeLogin);
                return _TestLogin;
            }
        }

        public void TimeoutYes(string triggerType)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger BtnTimeOutYesAction Starting ..."), _TraceCategory);

                switch (triggerType)
                {
                    case "M":
                        {
                            BackgroundVisibility = Visibility.Collapsed;
                            CancelVisibility = Visibility.Collapsed;

                            timeRemainingTimeoutTimer.Stop();
                            TimeoutCountdown = new TimeSpan(0, 0, 90);

                            timeRemainingTimer.Start();
                            IsTimeRemainingVisible = true;
                        }
                        break;
                    case "A":
                        {
                            BackgroundVisibility = Visibility.Collapsed;
                            ShowTimeOut = Visibility.Collapsed;

                            timeRemainingTimeoutTimer.Stop();
                            TimeoutCountdown = new TimeSpan(0, 0, 90);

                            timeRemainingTimer.Start();
                            IsTimeRemainingVisible = true;
                        }
                        break;
                    default:
                        break;
                }

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger BtnTimeOutYesAction Done ..."), _TraceCategory);
                isStop = false;

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] BtnTimeOutYesAction = {0}", ex.Message), _TraceCategory);
            }
        }

        public void TimeoutNo(string triggerType)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger BtnTimeOutNoAction Starting ..."), _TraceCategory);
                ShowLoading = Visibility.Visible;
                Thread timeOutLogout = new Thread(() =>
                {
                    if (CustomerID != 0)
                    {

                    }
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        switch (triggerType)
                        {
                            case "M":
                                {
                                    BackgroundVisibility = Visibility.Collapsed;
                                    CancelVisibility = Visibility.Collapsed;
                                    timeRemainingTimeoutTimer.Stop();
                                    autoResetTimeout.Set();
                                }
                                break;
                            case "A":
                                {
                                    BackgroundVisibility = Visibility.Collapsed;
                                    ShowTimeOut = Visibility.Collapsed;
                                    timeRemainingTimeoutTimer.Stop();
                                    autoResetTimeout.Set();
                                }
                                break;
                            default:
                                break;
                        }
                        SetStage(eStage.Home);
                        ShowLoading = Visibility.Collapsed;
                    }));
                });
                timeOutLogout.Start();

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger BtnTimeOutNoAction Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] BtnTimeOutNoAction = {0}", ex.Message), _TraceCategory);
            }
        }

        public void VoucherPopupCloseAction()
        {
            try
            {

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger VoucherPopupCloseAction Starting ..."), _TraceCategory);

                VoucherPopupVisibility = Visibility.Collapsed;
                BackgroundVisibility = Visibility.Collapsed;

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger VoucherPopupCloseAction Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] VoucherPopupCloseAction = {0}", ex.Message), _TraceCategory);
            }
        }

        public void WarningPopupCloseAction()
        {
            try
            {

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger WarningPopupCloseAction Starting ..."), _TraceCategory);

                WarningMessageBoxVisibility = Visibility.Collapsed;
                BackgroundVisibility = Visibility.Collapsed;

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger WarningPopupCloseAction Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] WarningPopupCloseAction = {0}", ex.Message), _TraceCategory);
            }
        }

        decimal _TotalAmountLFFSSK;
        public decimal TotalAmountLFFSSK
        {
            get
            {
                return _TotalAmountLFFSSK;
            }
            set
            {
                _TotalAmountLFFSSK = value;
                OnPropertyChanged("TotalAmountLFFSSK");
            }
        }

        public void CountTotalAmount()
        {
            try
            {
                //if (mBillList != null)
                //{
                //    if (mBillList.Count()>0)
                //        TotalAmountLFFSSK = Math.Round(mBillList.Sum(y => y.BillListAmountPaid), 2);
                //    else
                //        TotalAmountLFFSSK = 0.00m;
                //}
                //else
                TotalAmountLFFSSK = 0.00m;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] CountTotalAmount = {0}", ex.ToString()), _TraceCategory);
            }
        }

        public void Delete(string index)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Delete Starting..."), _TraceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Index = {0}", index), _TraceCategory);
                int iIndex = Convert.ToInt32(index);


                CountTotalAmount();
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] Delete = {0}", ex.ToString()), _TraceCategory);
            }
        }

        Visibility _ShowPromptMessage = Visibility.Collapsed;
        public Visibility ShowPromptMessage
        {
            get { return _ShowPromptMessage; }
            set
            {
                _ShowPromptMessage = value;
                OnPropertyChanged("ShowPromptMessage");
            }
        }

        public void TestingModeLogin()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("TestingMode Login Starting..."), _TraceCategory);
                //QRResponse("6066;iaayr4HF7cCzlCaKc+5kTA==");
                //QRResponse("6570;jTM8pWgM+Z3rUDB5Q+mdzw=="); 
                QRResponse("6313;/snPwCMWpzFyr0+zuL25iw==");
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("TestingMode Login Done..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("TestingMode error - {0}", ex.ToString()), _TraceCategory);
            }
        }

        #endregion

        #region LFFSSK Label

        public void TriggerLFFSSKLanguage()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger Language Change Starting ..."), _TraceCategory);

                OnPropertyChanged(nameof(Lbl_DineIn));
                OnPropertyChanged(nameof(Lbl_Takeaway));
                OnPropertyChanged(nameof(Lbl_CancelHeader));
                OnPropertyChanged(nameof(Lbl_CancelContent));
                OnPropertyChanged(nameof(Lbl_BtnYes));
                OnPropertyChanged(nameof(Lbl_BtnCancel));
                OnPropertyChanged(nameof(Lbl_Ads));
                OnPropertyChanged(nameof(Lbl_ViewOrder));
                OnPropertyChanged(nameof(Lbl_ScanAndLogin));
                OnPropertyChanged(nameof(Lbl_SelectOrderType));
                OnPropertyChanged(nameof(Lbl_Download));
                OnPropertyChanged(nameof(Lbl_Or));
                OnPropertyChanged(nameof(Lbl_Skip));
                OnPropertyChanged(nameof(Lbl_BtnDownsize));
                OnPropertyChanged(nameof(Lbl_BtnUpsize));
                OnPropertyChanged(nameof(Lbl_ContinueOrder));
                OnPropertyChanged(nameof(Lbl_Upsize));
                OnPropertyChanged(nameof(Lbl_Downsize));
                OnPropertyChanged(nameof(Lbl_AddOn));
                OnPropertyChanged(nameof(Lbl_TextVoucher));
                OnPropertyChanged(nameof(Lbl_MemberLogin));
                OnPropertyChanged(nameof(Lbl_Login));
                OnPropertyChanged(nameof(Lbl_ItemAdded));
                OnPropertyChanged(nameof(Lbl_ScanQr));
                OnPropertyChanged(nameof(Lbl_BtnClose));
                OnPropertyChanged(nameof(Lbl_Welcome));
                OnPropertyChanged(nameof(Lbl_CollectPoints));
                OnPropertyChanged(nameof(Lbl_YourAmount));
                OnPropertyChanged(nameof(Lbl_CC));
                OnPropertyChanged(nameof(Lbl_Ewallet));
                OnPropertyChanged(nameof(Lbl_Cash));
                OnPropertyChanged(nameof(Lbl_BackOnly));
                OnPropertyChanged(nameof(Lbl_OrderNumber));
                OnPropertyChanged(nameof(Lbl_PickupOrder));
                OnPropertyChanged(nameof(Lbl_DoneOrder));
                OnPropertyChanged(nameof(Lbl_SuccessOrder));
                OnPropertyChanged(nameof(Lbl_ShowVoucher));
                OnPropertyChanged(nameof(Lbl_Home));
                OnPropertyChanged(nameof(Lbl_HomeHow));
                OnPropertyChanged(nameof(Lbl_UpgradeCombo));
                OnPropertyChanged(nameof(Lbl_BtnUpCombo));
                OnPropertyChanged(nameof(Lbl_AddToCart));
                OnPropertyChanged(nameof(Lbl_TimeoutMsj));
                OnPropertyChanged(nameof(Lbl_TimeOut));
                OnPropertyChanged(nameof(Lbl_Error));
                OnPropertyChanged(nameof(Lbl_Confirmation));
                OnPropertyChanged(nameof(Lbl_Info));
                OnPropertyChanged(nameof(Lbl_InvalidQr));
                OnPropertyChanged(nameof(Lbl_InvalidPromoCode));
                OnPropertyChanged(nameof(Lbl_EnterPromoCode));
                OnPropertyChanged(nameof(Lbl_ErrorAssist));
                OnPropertyChanged(nameof(Lbl_ErrorAssist2));
                OnPropertyChanged(nameof(Lbl_ErrorTryAgain));
                OnPropertyChanged(nameof(Lbl_ErrorTryAgain2));
                OnPropertyChanged(nameof(Lbl_ContainPromoCode));
                OnPropertyChanged(nameof(Lbl_BtnPromoCode));
                OnPropertyChanged(nameof(Lbl_PromoCode));
                OnPropertyChanged(nameof(Lbl_PopUpPromoCode));
                OnPropertyChanged(nameof(Lbl_PaymentFail));
                OnPropertyChanged(nameof(Lbl_DownloadHere));
                OnPropertyChanged(nameof(Lbl_Recommend));
                OnPropertyChanged(nameof(Lbl_Subtotal));
                OnPropertyChanged(nameof(Lbl_VoucherAmount));
                OnPropertyChanged(nameof(Lbl_GiftVoucher));
                OnPropertyChanged(nameof(Lbl_Rounding));
                OnPropertyChanged(nameof(Lbl_GTotal));
                OnPropertyChanged(nameof(Lbl_ValidUntil));
                OnPropertyChanged(nameof(Lbl_Edit));
                OnPropertyChanged(nameof(Lbl_InsertPin));
                OnPropertyChanged(nameof(Lbl_EwalletPayment));
                OnPropertyChanged(nameof(Lbl_EwalletMethod1));
                OnPropertyChanged(nameof(Lbl_EwalletMethod2));
                OnPropertyChanged(nameof(Lbl_EwalletMethod3));
                OnPropertyChanged(nameof(Lbl_EwalletMethod4));
                OnPropertyChanged(nameof(Lbl_Voucher));
                OnPropertyChanged(nameof(Lbl_UseNow));
                OnPropertyChanged(nameof(Lbl_Logout));
                OnPropertyChanged(nameof(Lbl_CheckVoucher));
                OnPropertyChanged(nameof(Lbl_InvalidMenu));
                OnPropertyChanged(nameof(Lbl_KadCC));
                OnPropertyChanged(nameof(Lbl_DoneOnly));
                OnPropertyChanged(nameof(Lbl_UpgradeComboR));
                OnPropertyChanged(nameof(Lbl_UpgradeComboL));
                OnPropertyChanged(nameof(Lbl_SelectLanguage));
                OnPropertyChanged(nameof(Lbl_InputTableTent));
                OnPropertyChanged(nameof(Lbl_Next));
                OnPropertyChanged(nameof(Lbl_No));
                OnPropertyChanged(nameof(Lbl_Yes));
                OnPropertyChanged(nameof(Lbl_DoYouHaveSeat));

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger Language Change Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] TriggerLanguageChanged = {0}", ex.Message), _TraceCategory);
            }
        }

        string _PromptMessage;
        public string PromptMessage
        {
            get { return _PromptMessage; }
            set
            {
                _PromptMessage = value;
                OnPropertyChanged("PromptMessage");
            }
        }

        string _Label_PaymentMethodMessage;
        public string Label_PaymentMethodMessage
        {
            get { return _Label_PaymentMethodMessage; }
            set
            {
                _Label_PaymentMethodMessage = value;
                OnPropertyChanged("Label_PaymentMethodMessage");
            }
        }

        string _Label_PaymentChoosen;
        public string Label_PaymentChoosen
        {
            get { return _Label_PaymentChoosen; }
            set
            {
                _Label_PaymentChoosen = value;
                OnPropertyChanged("Label_PaymentChoosen");
            }
        }

        string _Label_PaymentInstruction;
        public string Label_PaymentInstruction
        {
            get { return _Label_PaymentInstruction; }
            set
            {
                _Label_PaymentInstruction = value;
                OnPropertyChanged("Label_PaymentInstruction");
            }
        }

        string _Label_ConfirmSentence;
        public string Label_ConfirmSentence
        {
            get { return _Label_ConfirmSentence; }
            set
            {
                _Label_ConfirmSentence = value;
                OnPropertyChanged("Label_ConfirmSentence");
            }
        }

        #endregion

        #region HomeView

        #region property

        UserControl _HomeBannerView;
        public UserControl HomeBannerView
        {
            get { return _HomeBannerView; }
            set
            {
                if (value == _HomeBannerView)
                    return;
                _HomeBannerView = value;
                OnPropertyChanged("HomeBannerView");
            }
        }

        private Visibility _AdsVisibility;
        public Visibility AdsVisibility
        {
            get { return _AdsVisibility; }
            set
            {
                _AdsVisibility = value;
                OnPropertyChanged(nameof(AdsVisibility));
            }
        }

        private Visibility _PopUpVisibility;
        public Visibility PopUpVisibility
        {
            get { return _PopUpVisibility; }
            set
            {
                _PopUpVisibility = value;
                OnPropertyChanged(nameof(PopUpVisibility));
            }
        }

        private Visibility _MenuBannerVisibility;
        public Visibility MenuBannerVisibility
        {
            get { return _MenuBannerVisibility; }
            set
            {
                _MenuBannerVisibility = value;
                OnPropertyChanged(nameof(MenuBannerVisibility));
            }
        }

        private string _SelectedOption;

        public string SelectedOption
        {
            get { return _SelectedOption; }
            set
            {
                _SelectedOption = value;
                OnPropertyChanged(nameof(SelectedOption));
            }
        }

        private ObservableCollection<string> _BannerList;

        public ObservableCollection<string> BannerList
        {
            get { return _BannerList; }
            set
            {
                _BannerList = value;
                OnPropertyChanged(nameof(BannerList));
            }
        }


        #endregion

        #region Command
        ICommand _LFFSSKLanguageCommand;
        public ICommand LFFSSKLanguageCommand
        {
            get
            {
                if (_LFFSSKLanguageCommand == null)
                    _LFFSSKLanguageCommand = new RelayCommand<string>(LFFSSKLanguage);
                return _LFFSSKLanguageCommand;
            }
        }

        private ICommand _BtnCloseAds;

        public ICommand BtnCloseAds
        {
            get
            {
                if (_BtnCloseAds == null)
                    _BtnCloseAds = new RelayCommand(BtnCloseAdsAction);
                return _BtnCloseAds;
            }
        }

        private ICommand _BtnClosePopUp;

        public ICommand BtnClosePopUp
        {
            get
            {
                if (_BtnClosePopUp == null)
                    _BtnClosePopUp = new RelayCommand(BtnClosePopUpAction);
                return _BtnClosePopUp;
            }
        }

        private ICommand _BtnEng;

        public ICommand BtnEng
        {
            get
            {
                if (_BtnEng == null)
                    _BtnEng = new RelayCommand(ChangeEN);
                return _BtnEng;
            }
        }

        private ICommand _BtnBM;

        public ICommand BtnBM
        {
            get
            {
                if (_BtnBM == null)
                    _BtnBM = new RelayCommand(ChangeBM);
                return _BtnBM;
            }
        }

        private ICommand _BtnGetMenu;
        public ICommand BtnGetMenu
        {
            get
            {
                if (_BtnGetMenu == null)
                    _BtnGetMenu = new RelayCommand<string>(RetrieveCategory);
                return _BtnGetMenu;
            }
        }
        #endregion

        #region Function
        public void ChangeEN()
        {
            try
            {
                LFFSSKLanguage("EN");
            }
            catch (Exception ex)
            {

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] ChangeEN = {0}", ex.Message), _TraceCategory);
            }
        }

        public void ChangeBM()
        {
            try
            {
                LFFSSKLanguage("BM");
            }
            catch (Exception ex)
            {

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] ChangeBM = {0}", ex.Message), _TraceCategory);
            }
        }
        void LFFSSKLanguage(string param)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Selected Language : {0}", param), _TraceCategory);

                if (param == "EN")
                {
                    _UiCulture = "en-us";
                    IsEN = true;
                    IsBM = false;
                }
                else
                {
                    _UiCulture = "ms-my";
                    IsEN = false;
                    IsBM = true;
                }

                LFFSSK.Properties.Resources.Culture = new System.Globalization.CultureInfo(_UiCulture);
                TriggerLFFSSKLanguage();
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] LFFSSKLanguage = Done"), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] LFFSSKLanguage = {0}", ex.Message), _TraceCategory);
            }
        }

        private void BtnCloseAdsAction()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] BtnCloseAdsAction = Trigger"), _TraceCategory);
                AdsVisibility = Visibility.Visible;
                ResetTimer(15);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] BtnCloseAdsAction = {0}", ex.Message), _TraceCategory);
            }
        }

        private void BtnClosePopUpAction()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] BtnCloseAdsAction = Trigger"), _TraceCategory);
                AdsVisibility = Visibility.Collapsed;
                StopCountDown();
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] BtnCloseAdsAction = {0}", ex.Message), _TraceCategory);
            }
        }

        string dineMethod;
        string orderNum;
        string referenceNo;
        public void RetrieveCategory(string command)
        {
            Thread thRetrieveCategory = new Thread(() =>
            {
                try
                {
                    ShowLoading = Visibility.Visible;
                    StopCountDown();
                    dineMethod = string.Empty;
                    orderNum = string.Empty;
                    TableNo = string.Empty;
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] RetrieveCategory , Thread thRetrieveCategory Starting... "), _TraceCategory);

                    CartList = null;
                    CartList = new ObservableCollection<CartModel.Product>();
                    //Voucher = new List<Order.OrderReward>();

                    try
                    {
                        switch (command)
                        {
                            //dine in
                            case "1":
                                {
                                    dineMethod = "Dine In";
                                    //TableNoVisibility = Visibility.Visible;
                                    if (GeneralVar.EnableDelayOrder)
                                    {
                                        DelayOrderPopupVisibility = Visibility.Visible;
                                    }
                                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] RetrieveCategory [ Option : Dine-In ] "), _TraceCategory);
                                }
                                break;
                            //take away
                            case "2":
                                {
                                    dineMethod = "Take Away";
                                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] RetrieveCategory [ Option : Takeaway ] "), _TraceCategory);
                                }
                                break;
                            default:
                                break;
                        }


                        eatMethod = int.Parse(command);
                        
                        CateDetails = new ObservableCollection<ApiModel.GetMenu.Response.Category>();
                        foreach (var cate in MasterMenuCategory)
                        {
                            CateDetails.Add(new ApiModel.GetMenu.Response.Category(cate.categoryId, cate.categoryName, cate.categoryImageUrl, null, 0, null, null, null));
                        }

                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            CateDetails[0].IsChecked = true;
                            RefreshProduct(CateDetails[0].categoryId);
                            SetStage(eStage.MenuItem);
                        }));

                        ShowLoading = Visibility.Collapsed;

                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] RetrieveCategory = {0}", ex.Message), _TraceCategory);
                    }

                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] RetrieveCategory , Thread thRetrieveCategory Done... "), _TraceCategory);
                }
                catch (Exception ex)
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] RetrieveCategory, Thread thRetrieveCategory = {0}", ex.Message), _TraceCategory);
                }
            });
            thRetrieveCategory.Start();

        }

        #endregion
        #endregion

        #region Menu Page
        #region Property

        UserControl _MenuBannerView;
        public UserControl MenuBannerView
        {
            get { return _MenuBannerView; }
            set
            {
                if (value == _MenuBannerView)
                    return;
                _MenuBannerView = value;
                OnPropertyChanged("MenuBannerView");
            }
        }
        //change to disable login = false, enable = true
        bool LoginPopUp = true;
        private Visibility _CancelVisibility;

        public Visibility CancelVisibility
        {
            get { return _CancelVisibility; }
            set
            {
                _CancelVisibility = value;
                OnPropertyChanged(nameof(CancelVisibility));
            }
        }

        private Visibility _BackgroundVisibility;

        public Visibility BackgroundVisibility
        {
            get { return _BackgroundVisibility; }
            set
            {
                _BackgroundVisibility = value;
                OnPropertyChanged(nameof(BackgroundVisibility));
            }
        }

        private Visibility _VoucherPopupVisibility;

        public Visibility VoucherPopupVisibility
        {
            get { return _VoucherPopupVisibility; }
            set
            {
                _VoucherPopupVisibility = value;
                OnPropertyChanged(nameof(VoucherPopupVisibility));
            }
        }

        private Visibility _CheckOutContentVisibility;

        public Visibility CheckOutContentVisibility
        {
            get { return _CheckOutContentVisibility; }
            set
            {
                _CheckOutContentVisibility = value;
                OnPropertyChanged(nameof(CheckOutContentVisibility));
            }
        }


        private bool _ViewOrderEnable;
        public bool ViewOrderEnable
        {
            get
            {
                if (CartItem == 0)
                {
                    _ViewOrderEnable = false;
                    CheckOutContentVisibility = Visibility.Collapsed;
                }
                else
                {
                    _ViewOrderEnable = true;
                    CheckOutContentVisibility = Visibility.Visible;
                }
                return _ViewOrderEnable;
            }
            set
            {
                _ViewOrderEnable = value;
                OnPropertyChanged(nameof(ViewOrderEnable));
                OnPropertyChanged(nameof(CheckOutContentVisibility));
            }
        }

        private Visibility _PointGridVisibility;

        public Visibility PointGridVisibility
        {
            get { return _PointGridVisibility; }
            set
            {
                _PointGridVisibility = value;
                OnPropertyChanged(nameof(PointGridVisibility));
            }
        }

        private int _VoucherQty;

        public int VoucherQty
        {
            get { return _VoucherQty; }
            set
            {
                _VoucherQty = value;
                OnPropertyChanged(nameof(VoucherQty));
            }
        }

        private int _SvMenuHeight;

        public int SvMenuHeight
        {
            get { return _SvMenuHeight; }
            set
            {
                _SvMenuHeight = value;
                OnPropertyChanged(nameof(SvMenuHeight));
            }
        }

        private string _TableNo;

        public string TableNo
        {
            get 
            {
                if (!String.IsNullOrEmpty(_TableNo))
                {
                    if (_TableNo.Contains("."))
                        _TableNo=_TableNo.Replace(".", "");
                }                
                return _TableNo; 
            }
            set 
            { 
                _TableNo = value;
                OnPropertyChanged(nameof(TableNo));
                OnPropertyChanged(nameof(NextBtnEnable));
            }
        }

        private bool _NextBtnEnable;

        public bool NextBtnEnable
        {
            get 
            {
                if (String.IsNullOrEmpty(TableNo))
                    _NextBtnEnable = false;
                else
                    _NextBtnEnable = true;
                return _NextBtnEnable; 
            }
            set 
            { 
                _NextBtnEnable = value;
                OnPropertyChanged(nameof(NextBtnEnable));
            }
        }

        private Visibility _TableNoVisibility;

        public Visibility TableNoVisibility
        {
            get { return _TableNoVisibility; }
            set 
            { 
                _TableNoVisibility = value;
                OnPropertyChanged(nameof(TableNoVisibility));
            }
        }

        private bool _IsDelaySentOrder;

        public bool IsDelaySentOrder
        {
            get { return _IsDelaySentOrder; }
            set 
            { 
                _IsDelaySentOrder = value;
                OnPropertyChanged(nameof(IsDelaySentOrder));
            }
        }

        private Visibility _DelayOrderPopupVisibility;

        public Visibility DelayOrderPopupVisibility
        {
            get { return _DelayOrderPopupVisibility; }
            set 
            { 
                _DelayOrderPopupVisibility = value;
                OnPropertyChanged(nameof(DelayOrderPopupVisibility));
            }
        }


        #endregion

        #region Command
        private ICommand _BtnReturnLanding;
        public ICommand BtnReturnLanding
        {
            get
            {
                if (_BtnReturnLanding == null)
                    _BtnReturnLanding = new RelayCommand(ReturnLanding);
                return _BtnReturnLanding;
            }
        }

        private ICommand _BtnViewOrder;
        public ICommand BtnViewOrder
        {
            get
            {
                if (_BtnViewOrder == null)
                    _BtnViewOrder = new RelayCommand(ViewOrderAction);
                return _BtnViewOrder;
            }
        }

        private ICommand _BtnCloseTableNoPopup;
        public ICommand BtnCloseTableNoPopup
        {
            get
            {
                if (_BtnCloseTableNoPopup == null)
                    _BtnCloseTableNoPopup = new RelayCommand(CloseTableNoPopUp);
                return _BtnCloseTableNoPopup;
            }
        }

        private ICommand _BtnNextTableNoPopup;
        public ICommand BtnNextTableNoPopup
        {
            get
            {
                if (_BtnNextTableNoPopup == null)
                    _BtnNextTableNoPopup = new RelayCommand(NextTableNoPopUp);
                return _BtnNextTableNoPopup;
            }
        }

        private ICommand _BtnProceedDelayOrder;
        public ICommand BtnProceedDelayOrder
        {
            get
            {
                if (_BtnProceedDelayOrder == null)
                    _BtnProceedDelayOrder = new RelayCommand(DelaySentOrder);
                return _BtnProceedDelayOrder;
            }
        }

        private ICommand _BtnProceedSentOrder;
        public ICommand BtnProceedSentOrder
        {
            get
            {
                if (_BtnProceedSentOrder == null)
                    _BtnProceedSentOrder = new RelayCommand(ProceedSentOrder);
                return _BtnProceedSentOrder;
            }
        }

        private int _CustomerPoints;

        public int CustomerPoints
        {
            get { return _CustomerPoints; }
            set
            {
                _CustomerPoints = value;
                OnPropertyChanged(nameof(CustomerPoints));
            }
        }

        private string _CustomerName;

        public string CustomerName
        {
            get { return _CustomerName; }
            set
            {
                _CustomerName = value;
                OnPropertyChanged(nameof(CustomerName));
            }
        }

        Visibility _ShowMBAMenu = Visibility.Collapsed;
        public Visibility ShowMBAMenu
        {
            get
            {
                if (_ShowMBAMenu == Visibility.Visible)
                    SvMenuHeight = 1100;
                else
                    SvMenuHeight = 1500;
                return _ShowMBAMenu;
            }
            set
            {
                _ShowMBAMenu = value;
                OnPropertyChanged("ShowMBAMenu");
                OnPropertyChanged(nameof(SvMenuHeight));
            }
        }

        #endregion

        #region Function
        public void ReturnLanding()
        {
            if (Stage != eStage.FinalPage)
            {
                BackgroundVisibility = Visibility.Visible;
                CancelVisibility = Visibility.Visible;
            }
            else
            {
                SetStage(eStage.Home);
            }
        }

        //use to skip table service, will generate reference no
        public void CloseTableNoPopUp()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("User Skip Entering Table Tent Number - Trigger CloseTableNoPopUp Starting ..."), _TraceCategory);
                TableNoVisibility = Visibility.Collapsed;
                TableNo = string.Empty;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("User Skip Entering Table Tent Number - Trigger CloseTableNoPopUp Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] CloseTableNoPopUp = {0}", ex.Message), _TraceCategory);
            }
        }

        //use if user have input table tent number 
        public void NextTableNoPopUp()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("User have Entering Table Tent Number - Trigger NextTableNoPopUp Starting ..."), _TraceCategory);
                TableNoVisibility = Visibility.Collapsed;
                //DelayOrderPopupVisibility = Visibility.Visible;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("User have Entering Table Tent Number - Trigger NextTableNoPopUp Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] NextTableNoPopUp = {0}", ex.Message), _TraceCategory);
            }
        }

        //User have place, direct send order
        public void ProceedSentOrder()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("User have seat - Trigger ProceedSentOrder Starting ..."), _TraceCategory);
                DelayOrderPopupVisibility = Visibility.Collapsed;
                IsDelaySentOrder = false;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("User have seat - Trigger ProceedSentOrder Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] ProceedSentOrder = {0}", ex.Message), _TraceCategory);
            }
        }

        //user agree to delay order, order details will save at lb end
        public void DelaySentOrder()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("User dont have seat - Trigger DelaySentOrder Starting ..."), _TraceCategory);
                DelayOrderPopupVisibility = Visibility.Collapsed;
                TableNoVisibility = Visibility.Visible;
                IsDelaySentOrder = true;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("User dont have seat - Trigger DelaySentOrder Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] DelaySentOrder = {0}", ex.Message), _TraceCategory);
            }
        }

        public void RetrieveMenuDetails(int itemId)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger RetrieveMenuDetails Starting ..."), _TraceCategory);

                MenuDetails = null;
                MenuDetails = new ObservableCollection<ApiModel.GetMenu.Response.Product>();

                var tempItem = MasterMenuProduct
                .SelectMany(category => category.products) // Flatten all products into a single list
                .FirstOrDefault(product => product.itemId == itemId);

                if (tempItem != null)
                {
                    MenuDetails.Add(new ApiModel.GetMenu.Response.Product
                        (
                            tempItem.DOUBLE_Sale_Price,
                            tempItem.DOUBLE_Employee_Price,
                            tempItem.DOUBLE_Wholesale_Price,
                            tempItem.DOUBLE_Custom_Price,
                            tempItem.DOUBLE_Manufacturer_Suggested_Retail_Price,
                            tempItem.DOUBLE_Web_Price,
                            tempItem.DOUBLE_Web_Dealer_Price,
                            tempItem.customField1,
                            tempItem.customField2,
                            tempItem.customField3,
                            tempItem.customField4,
                            tempItem.customField5,
                            tempItem.customField6,
                            tempItem.customField7,
                            tempItem.customField8,
                            tempItem.customField9,
                            tempItem.customField10,
                            tempItem.customField11,
                            tempItem.customField12,
                            tempItem.customField13,
                            tempItem.customField14,
                            tempItem.customField15,
                            tempItem.customField16,
                            tempItem.customField17,
                            tempItem.customField18,
                            tempItem.itemName,
                            tempItem.colorHex,
                            tempItem.dynamicHeaderLabel,
                            tempItem.dynamicmodifiers.Select(a => new ApiModel.GetMenu.Response.DynamicModifier
                            {
                                DOUBLE_Sale_Price = a.DOUBLE_Sale_Price,
                                DOUBLE_Employee_Price = a.DOUBLE_Employee_Price,
                                DOUBLE_Wholesale_Price = a.DOUBLE_Wholesale_Price,
                                DOUBLE_Custom_Price = a.DOUBLE_Custom_Price,
                                DOUBLE_Manufacturer_Suggested_Retail_Price = a.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                DOUBLE_Web_Price = a.DOUBLE_Web_Price,
                                DOUBLE_Web_Dealer_Price = a.DOUBLE_Web_Dealer_Price,
                                customField1 = a.customField1,
                                customField2 = a.customField2,
                                customField3 = a.customField3,
                                customField4 = a.customField4,
                                customField5 = a.customField5,
                                customField6 = a.customField6,
                                customField7 = a.customField7,
                                customField8 = a.customField8,
                                customField9 = a.customField9,
                                customField10 = a.customField10,
                                customField11 = a.customField11,
                                customField12 = a.customField12,
                                customField13 = a.customField13,
                                customField14 = a.customField14,
                                customField15 = a.customField15,
                                customField16 = a.customField16,
                                customField17 = a.customField17,
                                customField18 = a.customField18,
                                itemName = a.itemName,
                                itemShortName = a.itemShortName,
                                defaultSelected = a.defaultSelected,
                                modifiers = a.modifiers.Select(b => new ApiModel.GetMenu.Response.Modifier
                                {
                                    type = b.type,
                                    minSelection = b.minSelection,
                                    maxSelection = b.maxSelection,
                                    groupId = b.groupId,
                                    groupName = b.groupName,
                                    selections = b.selections.Select(c => new ApiModel.GetMenu.Response.Selection
                                    {
                                        name = c.name,
                                        defaultSelected = c.defaultSelected,
                                        itemId = c.itemId,
                                        itemCode = c.itemCode,
                                        itemType = c.itemType,
                                        price = c.price,
                                        DOUBLE_Sale_Price = c.DOUBLE_Sale_Price,
                                        DOUBLE_Employee_Price = c.DOUBLE_Employee_Price,
                                        DOUBLE_Wholesale_Price = c.DOUBLE_Wholesale_Price,
                                        DOUBLE_Custom_Price = c.DOUBLE_Custom_Price,
                                        DOUBLE_Manufacturer_Suggested_Retail_Price = c.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                        DOUBLE_Web_Price = c.DOUBLE_Web_Price,
                                        DOUBLE_Web_Dealer_Price = c.DOUBLE_Web_Dealer_Price,
                                        imageUrlList = c.imageUrlList,
                                        binaryImageUrl = c.binaryImageUrl,
                                        imageUrl = c.imageUrl,
                                        description = c.description,
                                        customField1 = c.customField1,
                                        customField2 = c.customField2,
                                        customField3 = c.customField3,
                                        customField4 = c.customField4,
                                        customField5 = c.customField5,
                                        customField6 = c.customField6,
                                        customField7 = c.customField7,
                                        customField8 = c.customField8,
                                        customField9 = c.customField9,
                                        customField10 = c.customField10,
                                        customField11 = c.customField11,
                                        customField12 = c.customField12,
                                        customField13 = c.customField13,
                                        customField14 = c.customField14,
                                        customField15 = c.customField15,
                                        customField16 = c.customField16,
                                        customField17 = c.customField17,
                                        customField18 = c.customField18,
                                        size = c.size,
                                        boolOpenPrice = c.boolOpenPrice,
                                        stockType = c.stockType,
                                        IsAssortment = c.IsAssortment,
                                        HasStock = c.HasStock,
                                        AllowToSell = c.AllowToSell,
                                        IsActive = c.IsActive,
                                        groupId = c.groupId,
                                        minSelection = c.minSelection,
                                        maxSelection = c.maxSelection,
                                        parentItemId = c.parentItemId,
                                        MenuImage = c.MenuImage,
                                        MenuImagePath = c.MenuImagePath,
                                        IsEnable = c.IsEnable

                                    }).ToList()
                                }).ToList(),
                                itemId = a.itemId,
                                itemCode = a.itemCode,
                                itemType = a.itemType,
                                price = a.price,
                                imageUrlList = a.imageUrlList,
                                binaryImageUrl = a.binaryImageUrl,
                                imageUrl = a.imageUrl,
                                description = a.description,
                                size = a.size,
                                boolOpenPrice = a.boolOpenPrice,
                                stockType = a.stockType,
                                IsAssortment = a.IsAssortment,
                                HasStock = a.HasStock,
                                AllowToSell = a.AllowToSell,
                                IsActive = a.IsActive,
                                MenuImage = a.MenuImage,
                                MenuImagePath = a.MenuImagePath

                            }).ToList(),
                            tempItem.itemId,
                            tempItem.itemCode,
                            tempItem.itemType,
                            tempItem.price,
                            tempItem.imageUrlList,
                            tempItem.binaryImageUrl,
                            tempItem.imageUrl,
                            tempItem.description,
                            tempItem.size,
                            tempItem.boolOpenPrice,
                            tempItem.stockType,
                            tempItem.IsAssortment,
                            tempItem.HasStock,
                            tempItem.AllowToSell,
                            tempItem.IsActive,
                            tempItem.MenuImagePath,
                            tempItem.MenuImage,
                            tempItem.ItemCurrentQty,
                            tempItem.ItemTotalPrice,
                            tempItem.TempTotalPrice
                        ));
                }

                if (MenuDetails != null)
                {
                    if (MenuDetails.Count() > 0)
                    {
                        foreach (var modifiertype in MenuDetails.SelectMany(modifier => modifier.dynamicmodifiers).ToList())
                        {
                            foreach (var modifierDet in modifiertype.modifiers)
                            {
                                //to set item default and be checked
                                if (modifierDet.minSelection == 1 && modifierDet.selections.Count() == 1)
                                {
                                    modifierDet.selections[0].IsEnable = false;

                                    if(modifierDet.selections[0].defaultSelected == 1)
                                    {
                                        modifierDet.selections[0].IsCheck = true;
                                    }
                                }

                                //to tick default item
                                if (modifierDet.minSelection >= 1)
                                {
                                    if (modifierDet.selections.Where(x => x.defaultSelected == 1).Count() > 0)
                                    {

                                        modifierDet.selections.ForEach(y =>
                                        {
                                            if (y.defaultSelected == 1)
                                            {
                                                y.IsCheck = true;
                                            }
                                        });
                                    }
                                    else
                                        modifierDet.selections[0].IsCheck = true;
                                }
                            }
                        }
                        MenuCurrentTotal = MenuDetails.Select(x => x.ItemTotalPrice).FirstOrDefault();
                    }
                    else
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Menu Details is less than 0"), _TraceCategory);
                }
                else
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Menu Details is null"), _TraceCategory);


                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger RetrieveMenuDetails Done ..."), _TraceCategory);

                SetStage(eStage.MenuCustomize);

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] RetrieveMenuDetails = {0}", ex.Message), _TraceCategory);
            }
        }

        public void RefreshProduct(int categoryId)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger RefreshProduct Starting ..."), _TraceCategory);

                if (Menu != null)
                    Menu = null;
                Menu = new ObservableCollection<ApiModel.GetMenu.Response.Product>();

                MasterMenuProduct.Where(x => x.categoryId == categoryId).ToList().ForEach(y =>
                {
                    if (y.products != null)
                    {
                        y.products.ForEach(f =>
                        {
                            Menu.Add(new ApiModel.GetMenu.Response.Product
                                (
                                    f.DOUBLE_Sale_Price,
                                    f.DOUBLE_Employee_Price,
                                    f.DOUBLE_Wholesale_Price,
                                    f.DOUBLE_Custom_Price,
                                    f.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                    f.DOUBLE_Web_Price,
                                    f.DOUBLE_Web_Dealer_Price,
                                    f.customField1,
                                    f.customField2,
                                    f.customField3,
                                    f.customField4,
                                    f.customField5,
                                    f.customField6,
                                    f.customField7,
                                    f.customField8,
                                    f.customField9,
                                    f.customField10,
                                    f.customField11,
                                    f.customField12,
                                    f.customField13,
                                    f.customField14,
                                    f.customField15,
                                    f.customField16,
                                    f.customField17,
                                    f.customField18,
                                    f.itemName,
                                    f.colorHex,
                                    f.dynamicHeaderLabel,
                                    f.dynamicmodifiers.Select(a => new ApiModel.GetMenu.Response.DynamicModifier
                                    {
                                        DOUBLE_Sale_Price = a.DOUBLE_Sale_Price,
                                        DOUBLE_Employee_Price = a.DOUBLE_Employee_Price,
                                        DOUBLE_Wholesale_Price = a.DOUBLE_Wholesale_Price,
                                        DOUBLE_Custom_Price = a.DOUBLE_Custom_Price,
                                        DOUBLE_Manufacturer_Suggested_Retail_Price = a.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                        DOUBLE_Web_Price = a.DOUBLE_Web_Price,
                                        DOUBLE_Web_Dealer_Price = a.DOUBLE_Web_Dealer_Price,
                                        customField1 = a.customField1,
                                        customField2 = a.customField2,
                                        customField3 = a.customField3,
                                        customField4 = a.customField4,
                                        customField5 = a.customField5,
                                        customField6 = a.customField6,
                                        customField7 = a.customField7,
                                        customField8 = a.customField8,
                                        customField9 = a.customField9,
                                        customField10 = a.customField10,
                                        customField11 = a.customField11,
                                        customField12 = a.customField12,
                                        customField13 = a.customField13,
                                        customField14 = a.customField14,
                                        customField15 = a.customField15,
                                        customField16 = a.customField16,
                                        customField17 = a.customField17,
                                        customField18 = a.customField18,
                                        itemName = a.itemName,
                                        itemShortName = a.itemShortName,
                                        defaultSelected = a.defaultSelected,
                                        modifiers = a.modifiers.Select(b => new ApiModel.GetMenu.Response.Modifier
                                        {
                                            type = b.type,
                                            minSelection = b.minSelection,
                                            maxSelection = b.maxSelection,
                                            groupId = b.groupId,
                                            groupName = b.groupName,
                                            selections = b.selections.Select(c => new ApiModel.GetMenu.Response.Selection
                                            {
                                                name = c.name,
                                                defaultSelected = c.defaultSelected,
                                                itemId = c.itemId,
                                                itemCode = c.itemCode,
                                                itemType = c.itemType,
                                                price = c.price,
                                                DOUBLE_Sale_Price = c.DOUBLE_Sale_Price,
                                                DOUBLE_Employee_Price = c.DOUBLE_Employee_Price,
                                                DOUBLE_Wholesale_Price = c.DOUBLE_Wholesale_Price,
                                                DOUBLE_Custom_Price = c.DOUBLE_Custom_Price,
                                                DOUBLE_Manufacturer_Suggested_Retail_Price = c.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                                DOUBLE_Web_Price = c.DOUBLE_Web_Price,
                                                DOUBLE_Web_Dealer_Price = c.DOUBLE_Web_Dealer_Price,
                                                imageUrlList = c.imageUrlList,
                                                binaryImageUrl = c.binaryImageUrl,
                                                imageUrl = c.imageUrl,
                                                description = c.description,
                                                customField1 = c.customField1,
                                                customField2 = c.customField2,
                                                customField3 = c.customField3,
                                                customField4 = c.customField4,
                                                customField5 = c.customField5,
                                                customField6 = c.customField6,
                                                customField7 = c.customField7,
                                                customField8 = c.customField8,
                                                customField9 = c.customField9,
                                                customField10 = c.customField10,
                                                customField11 = c.customField11,
                                                customField12 = c.customField12,
                                                customField13 = c.customField13,
                                                customField14 = c.customField14,
                                                customField15 = c.customField15,
                                                customField16 = c.customField16,
                                                customField17 = c.customField17,
                                                customField18 = c.customField18,
                                                size = c.size,
                                                boolOpenPrice = c.boolOpenPrice,
                                                stockType = c.stockType,
                                                IsAssortment = c.IsAssortment,
                                                HasStock = c.HasStock,
                                                AllowToSell = c.AllowToSell,
                                                IsActive = c.IsActive,
                                                groupId = c.groupId,
                                                minSelection = c.minSelection,
                                                maxSelection = c.maxSelection,
                                                parentItemId = c.parentItemId,
                                                MenuImage = c.MenuImage,
                                                MenuImagePath = c.MenuImagePath,
                                                IsEnable = c.IsEnable

                                            }).ToList()
                                        }).ToList(),
                                        itemId = a.itemId,
                                        itemCode = a.itemCode,
                                        itemType = a.itemType,
                                        price = a.price,
                                        imageUrlList = a.imageUrlList,
                                        binaryImageUrl = a.binaryImageUrl,
                                        imageUrl = a.imageUrl,
                                        description = a.description,
                                        size = a.size,
                                        boolOpenPrice = a.boolOpenPrice,
                                        stockType = a.stockType,
                                        IsAssortment = a.IsAssortment,
                                        HasStock = a.HasStock,
                                        AllowToSell = a.AllowToSell,
                                        IsActive = a.IsActive,
                                        MenuImage = a.MenuImage,
                                        MenuImagePath = a.MenuImagePath

                                    }).ToList(),
                                    f.itemId,
                                    f.itemCode,
                                    f.itemType,
                                    f.price,
                                    f.imageUrlList,
                                    f.binaryImageUrl,
                                    f.imageUrl,
                                    f.description,
                                    f.size,
                                    f.boolOpenPrice,
                                    f.stockType,
                                    f.IsAssortment,
                                    f.HasStock,
                                    f.AllowToSell,
                                    f.IsActive,
                                    f.MenuImagePath,
                                    f.MenuImage,
                                    f.ItemCurrentQty,
                                    f.ItemTotalPrice,
                                    f.TempTotalPrice
                                ));
                        });
                    }

                });

                CateId = categoryId;
                CateName = CateDetails.Where(x => x.categoryId == categoryId).Select(y => y.categoryName).FirstOrDefault();

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger RefreshProduct Done ..."), _TraceCategory);

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] RefreshProduct = {0}", ex.Message), _TraceCategory);
            }
        }

        public void CalculateTotalItem()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger CalculateTotalItem Starting ..."), _TraceCategory);

                DateTime timenow = DateTime.UtcNow;
                DateTime? dateTime = null;

                GetLastestOrderList();

                orderDetail = new Xilnex.Request
                    (
                        1,
                        CartList.Sum(x => x.ItemTotalPrice),
                        0,
                        0,
                        CartList.Sum(x => x.ItemTotalPrice) * 0.06,
                        6,
                        0,
                        "9002",
                        "9002",
                        GeneralVar.LocationID,
                        dineMethod,
                        orderNum,
                        1,
                        GeneralVar.ComponentCode,
                        string.Empty,
                        false,
                        DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fffZ"),
                        "XILNEXLIVESALES",
                        DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fffZ"),
                        string.Empty,
                        string.Empty,
                        0,
                        CartList.Sum(x => x.ItemTotalPrice) - (CartList.Sum(y => y.ItemTotalPrice) * 0.06),
                        string.Empty,
                        0,
                        "-",
                        CartList.Sum(x => x.ItemTotalPrice) * 0.06,
                        0,
                        false,
                        orderNum,
                        Order
                    );

                orderRequest = new ApiModel.InitialOrderRequest
                {
                    OrderDetails = orderDetail,
                    ComponentId = GeneralVar.ComponentId,
                    ComponentCode =GeneralVar.ComponentCode,
                    OutletId = 1,
                };

                //CartItem = CartList.Select(x => x.ItemCurrentQty).Sum();

                CartItem = Order.Sum(x => x.Quantity);
                TotalAmount = Convert.ToDecimal(Order.Sum(x => x.SubTotal));

                //CartItem = Order.Sum(x => x.quantity);
                //TotalAmount = orderRequest.subTotal;
                AnWTotalAmount = Order.Sum(x => x.SubTotal);
                AnWTax = Convert.ToDouble(TotalAmount) * 0.06;

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger CalculateTotalItem Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("{Error] Trigger CalculateTotalItem : {0}", ex.ToString()), _TraceCategory);
            }
        }

        public void ViewOrderAction()
        {
            //if (vOrderSummary != null)
            //    vOrderSummary.SvReset();

            SetStage(eStage.OrderSummary);
        }

        #endregion
        #endregion

        #region MenuDetails Page
        #region Property
        private double _MenuCurrentTotal;

        public double MenuCurrentTotal
        {
            get { return _MenuCurrentTotal; }
            set
            {
                _MenuCurrentTotal = value;
                OnPropertyChanged(nameof(MenuCurrentTotal));
            }
        }

        #endregion

        #region Command
        private ICommand _BtnReturnMenu;
        public ICommand BtnReturnMenu
        {
            get
            {
                if (_BtnReturnMenu == null)
                    _BtnReturnMenu = new RelayCommand(ReturnMenu);
                return _BtnReturnMenu;
            }
        }

        private ICommand _BtnAddToCart;
        public ICommand BtnAddToCart
        {
            get
            {
                if (_BtnAddToCart == null)
                    _BtnAddToCart = new RelayCommand(AddToCartAction);
                return _BtnAddToCart;
            }
        }
        #endregion

        #region Function
        public void ReturnMenu()
        {
            //if (vMenuPage != null)
            //    vMenuPage.ResetMenuScroll();

            if (DetailsViewVisbility == Visibility.Visible && Stage == eStage.EditItem)
            {
                BackgroundVisibility = DetailsViewVisbility = Visibility.Collapsed;
                SetStage(eStage.OrderSummary);
            }
            else if (DetailsViewVisbility == Visibility.Visible && Stage == eStage.MenuCustomize)
            {
                BackgroundVisibility = DetailsViewVisbility = Visibility.Collapsed;
                SetStage(eStage.MenuItem);
            }
            else if (DetailsViewVisbility == Visibility.Visible && Stage == eStage.Voucher)
            {
                BackgroundVisibility = DetailsViewVisbility = Visibility.Collapsed;
                SetStage(eStage.OrderSummary);
            }
            else
                SetStage(eStage.MenuItem);

        }
        public void AddToCartAction()

        {
            try
            {
                if (CheckSimilarity(null, out int itemCurrentQueue))
                {
                    int menuQty = MenuDetails.Select(x => x.ItemCurrentQty).FirstOrDefault();
                    CartList.Where(x => x.cartMenuNo == itemCurrentQueue).Select(z => z.ItemCurrentQty += menuQty).ToList();
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Similarity - true"), _TraceCategory);
                }
                else
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Similarity - false"), _TraceCategory);
                    int itemQueue;
                    if (CartList.Count() == 0)
                    {
                        itemQueue = 1;
                    }
                    else
                        itemQueue = CartList.Max(x => x.cartMenuNo) + 1;

                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger AddToCart Starting ..."), _TraceCategory);

                    MenuDetails.ToList().ForEach(x =>
                    {

                        ObservableCollection<CartModel.Modifier> ModifierDetails = new ObservableCollection<CartModel.Modifier>();

                        x.dynamicmodifiers.SelectMany(modifier => modifier.modifiers).ToList().ForEach(a =>
                          {
                              var checkModifiers = a.selections.Where(b => b.IsCheck).ToList();

                              if (checkModifiers.Any())
                              {
                                  ModifierDetails.Add(new CartModel.Modifier
                                  {
                                      type = a.type,
                                      minSelection = a.minSelection,
                                      maxSelection = a.maxSelection,
                                      groupId = a.groupId,
                                      groupName = a.groupName,
                                      selections = checkModifiers.Select(c => new CartModel.Selection
                                      {
                                          name = c.name,
                                          defaultSelected = c.defaultSelected,
                                          itemId = c.itemId,
                                          itemCode = c.itemCode,
                                          itemType = c.itemType,
                                          price = c.price,
                                          DOUBLE_Sale_Price = c.DOUBLE_Sale_Price,
                                          DOUBLE_Employee_Price = c.DOUBLE_Employee_Price,
                                          DOUBLE_Wholesale_Price = c.DOUBLE_Wholesale_Price,
                                          DOUBLE_Custom_Price = c.DOUBLE_Custom_Price,
                                          DOUBLE_Manufacturer_Suggested_Retail_Price = c.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                          DOUBLE_Web_Price = c.DOUBLE_Web_Price,
                                          DOUBLE_Web_Dealer_Price = c.DOUBLE_Web_Dealer_Price,
                                          imageUrlList = c.imageUrlList,
                                          binaryImageUrl = c.binaryImageUrl,
                                          imageUrl = c.imageUrl,
                                          description = c.description,
                                          customField1 = c.customField1,
                                          customField2 = c.customField2,
                                          customField3 = c.customField3,
                                          customField4 = c.customField4,
                                          customField5 = c.customField5,
                                          customField6 = c.customField6,
                                          customField7 = c.customField7,
                                          customField8 = c.customField8,
                                          customField9 = c.customField9,
                                          customField10 = c.customField10,
                                          customField11 = c.customField11,
                                          customField12 = c.customField12,
                                          customField13 = c.customField13,
                                          customField14 = c.customField14,
                                          customField15 = c.customField15,
                                          customField16 = c.customField16,
                                          customField17 = c.customField17,
                                          customField18 = c.customField18,
                                          size = c.size,
                                          boolOpenPrice = c.boolOpenPrice,
                                          stockType = c.stockType,
                                          IsAssortment = c.IsAssortment,
                                          HasStock = c.HasStock,
                                          AllowToSell = c.AllowToSell,
                                          IsActive = c.IsActive,
                                          groupId = c.groupId,
                                          minSelection = c.minSelection,
                                          maxSelection = c.maxSelection,
                                          parentItemId = c.parentItemId,
                                          MenuImage = c.MenuImage,
                                          MenuImagePath = c.MenuImagePath,
                                          IsCheck = c.IsCheck,
                                          IsEnable = c.IsEnable

                                      }).ToList()
                                  });
                              }
                          });

                        CartList.Add(new CartModel.Product
                        (
                            itemQueue,
                            ModifierDetails,
                            x.DOUBLE_Sale_Price,
                            x.DOUBLE_Employee_Price,
                            x.DOUBLE_Wholesale_Price,
                            x.DOUBLE_Custom_Price,
                            x.DOUBLE_Manufacturer_Suggested_Retail_Price,
                            x.DOUBLE_Web_Price,
                            x.DOUBLE_Web_Dealer_Price,
                            x.customField1,
                            x.customField2,
                            x.customField3,
                            x.customField4,
                            x.customField5,
                            x.customField6,
                            x.customField7,
                            x.customField8,
                            x.customField9,
                            x.customField10,
                            x.customField11,
                            x.customField12,
                            x.customField13,
                            x.customField14,
                            x.customField15,
                            x.customField16,
                            x.customField17,
                            x.customField18,
                            x.itemName,
                            x.colorHex,
                            x.dynamicHeaderLabel,
                            x.dynamicmodifiers.Select(a => new CartModel.DynamicModifier
                            {
                                DOUBLE_Sale_Price = a.DOUBLE_Sale_Price,
                                DOUBLE_Employee_Price = a.DOUBLE_Employee_Price,
                                DOUBLE_Wholesale_Price = a.DOUBLE_Wholesale_Price,
                                DOUBLE_Custom_Price = a.DOUBLE_Custom_Price,
                                DOUBLE_Manufacturer_Suggested_Retail_Price = a.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                DOUBLE_Web_Price = a.DOUBLE_Web_Price,
                                DOUBLE_Web_Dealer_Price = a.DOUBLE_Web_Dealer_Price,
                                customField1 = a.customField1,
                                customField2 = a.customField2,
                                customField3 = a.customField3,
                                customField4 = a.customField4,
                                customField5 = a.customField5,
                                customField6 = a.customField6,
                                customField7 = a.customField7,
                                customField8 = a.customField8,
                                customField9 = a.customField9,
                                customField10 = a.customField10,
                                customField11 = a.customField11,
                                customField12 = a.customField12,
                                customField13 = a.customField13,
                                customField14 = a.customField14,
                                customField15 = a.customField15,
                                customField16 = a.customField16,
                                customField17 = a.customField17,
                                customField18 = a.customField18,
                                itemName = a.itemName,
                                itemShortName = a.itemShortName,
                                defaultSelected = a.defaultSelected,
                                modifiers = a.modifiers.Select(b => new CartModel.Modifier
                                {
                                    type = b.type,
                                    minSelection = b.minSelection,
                                    maxSelection = b.maxSelection,
                                    groupId = b.groupId,
                                    groupName = b.groupName,
                                    selections = b.selections.Select(c => new CartModel.Selection
                                    {
                                        name = c.name,
                                        defaultSelected = c.defaultSelected,
                                        itemId = c.itemId,
                                        itemCode = c.itemCode,
                                        itemType = c.itemType,
                                        price = c.price,
                                        DOUBLE_Sale_Price = c.DOUBLE_Sale_Price,
                                        DOUBLE_Employee_Price = c.DOUBLE_Employee_Price,
                                        DOUBLE_Wholesale_Price = c.DOUBLE_Wholesale_Price,
                                        DOUBLE_Custom_Price = c.DOUBLE_Custom_Price,
                                        DOUBLE_Manufacturer_Suggested_Retail_Price = c.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                        DOUBLE_Web_Price = c.DOUBLE_Web_Price,
                                        DOUBLE_Web_Dealer_Price = c.DOUBLE_Web_Dealer_Price,
                                        imageUrlList = c.imageUrlList,
                                        binaryImageUrl = c.binaryImageUrl,
                                        imageUrl = c.imageUrl,
                                        description = c.description,
                                        customField1 = c.customField1,
                                        customField2 = c.customField2,
                                        customField3 = c.customField3,
                                        customField4 = c.customField4,
                                        customField5 = c.customField5,
                                        customField6 = c.customField6,
                                        customField7 = c.customField7,
                                        customField8 = c.customField8,
                                        customField9 = c.customField9,
                                        customField10 = c.customField10,
                                        customField11 = c.customField11,
                                        customField12 = c.customField12,
                                        customField13 = c.customField13,
                                        customField14 = c.customField14,
                                        customField15 = c.customField15,
                                        customField16 = c.customField16,
                                        customField17 = c.customField17,
                                        customField18 = c.customField18,
                                        size = c.size,
                                        boolOpenPrice = c.boolOpenPrice,
                                        stockType = c.stockType,
                                        IsAssortment = c.IsAssortment,
                                        HasStock = c.HasStock,
                                        AllowToSell = c.AllowToSell,
                                        IsActive = c.IsActive,
                                        groupId = c.groupId,
                                        minSelection = c.minSelection,
                                        maxSelection = c.maxSelection,
                                        parentItemId = c.parentItemId,
                                        MenuImage = c.MenuImage,
                                        MenuImagePath = c.MenuImagePath,
                                        IsCheck = c.IsCheck,
                                        IsEnable = c.IsEnable
                                    }).ToList()
                                }).ToList(),
                                itemId = a.itemId,
                                itemCode = a.itemCode,
                                itemType = a.itemType,
                                price = a.price,
                                imageUrlList = a.imageUrlList,
                                binaryImageUrl = a.binaryImageUrl,
                                imageUrl = a.imageUrl,
                                description = a.description,
                                size = a.size,
                                boolOpenPrice = a.boolOpenPrice,
                                stockType = a.stockType,
                                IsAssortment = a.IsAssortment,
                                HasStock = a.HasStock,
                                AllowToSell = a.AllowToSell,
                                IsActive = a.IsActive,
                                MenuImage = a.MenuImage,
                                MenuImagePath = a.MenuImagePath

                            }).ToList(),
                            x.itemId,
                            x.itemCode,
                            x.itemType,
                            x.price,
                            x.imageUrlList,
                            x.binaryImageUrl,
                            x.imageUrl,
                            x.description,
                            x.size,
                            x.boolOpenPrice,
                            x.stockType,
                            x.IsAssortment,
                            x.HasStock,
                            x.AllowToSell,
                            x.IsActive,
                            x.MenuImagePath,
                            x.MenuImage,
                            x.ItemCurrentQty,
                            x.ItemTotalPrice,
                            x.TempTotalPrice
                        ));
                    });

                }

                CalculateTotalItem();

                Thread changeView = new Thread(() =>
                {
                    try
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            SetStage(eStage.DoneAddCart);
                        }));

                        Thread.Sleep(1800);

                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            SetStage(eStage.MenuItem);
                        }));
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("AddToCartAction changeView = {0}", ex.Message), _TraceCategory);
                    }
                });
                changeView.Start();

                //SetStage(eStage.MenuItem);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger AddToCart Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] AddToCart = {0}", ex.Message), _TraceCategory);
            }
        }
        public void
            AddOnToCartAction(int itemID)
        {
            try
            {
                if (CheckSimilarity(itemID, out int itemCurrentQueue))
                {
                    CartList.Where(x => x.cartMenuNo == itemCurrentQueue).Select(z => z.ItemCurrentQty += 1).ToList();
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Similarity - true"), _TraceCategory);
                }
                else
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Similarity - false"), _TraceCategory);
                    int itemQueue;
                    if (CartList.Count() == 0)
                    {
                        itemQueue = 1;
                    }
                    else
                        itemQueue = CartList.Max(x => x.cartMenuNo) + 1;

                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger AddToCart Starting ..."), _TraceCategory);
                    //AddOnSum.Where(a => a.itemId == itemID).ToList().ForEach(
                    //    x =>
                    //    {
                    //        CartModel.SizingDetails sizingDetail = x.menuSizing != null ? new CartModel.SizingDetails(x.menuSizing.seqOption,x.menuSizing.type,x.menuSizing.upgradeName ,x.menuSizing.canUpsize, x.menuSizing.canDownSize, x.menuSizing.fromItemId, x.menuSizing.toItemId, x.menuSizing.ChangeMenuId, x.menuSizing.LabelContent, x.menuSizing.LabelBtn, x.menuSizing.UpsizeVisibility, x.menuSizing.toItemImg, x.menuSizing.toItemPrice, x.menuSizing.toItemDiffPrice) : null;

                    //        ObservableCollection<CartModel.ModifierTypeDetails> ModifierDetails = new ObservableCollection<CartModel.ModifierTypeDetails>();
                    //        List<CartModel.SizingDetails> sizings = new List<CartModel.SizingDetails>();
                    //        if (x.menuSizings != null)
                    //        {
                    //            x.menuSizings.ForEach(f =>
                    //            {
                    //                sizings.Add(new CartModel.SizingDetails(f.seqOption,f.type, f.upgradeName,f.canUpsize, f.canDownSize, f.fromItemId, f.toItemImg, f.ChangeMenuId, f.LabelContent, f.LabelBtn, f.UpsizeVisibility, f.toItemImg, f.toItemPrice, f.toItemDiffPrice));

                    //            });
                    //        }

                    //        x.modifiertypes.ToList().ForEach(a =>
                    //        {
                    //            var checkedModifiers = a.modifiers.Where(b => b.IsCheck).ToList();

                    //            if (checkedModifiers.Any())
                    //            {
                    //                ModifierDetails.Add(new CartModel.ModifierTypeDetails
                    //                {
                    //                    modifierId = a.modifierId,
                    //                    name = a.name,
                    //                    type = a.type,
                    //                    isSet = a.isSet,
                    //                    minNumber = a.minNumber,
                    //                    maxNumber = a.maxNumber,
                    //                    mustDefault = a.mustDefault,
                    //                    status = a.status,
                    //                    ordermodifiers = checkedModifiers.Select(b => new CartModel.ModifiersDetails
                    //                    {
                    //                        modifierId = b.modifierId,
                    //                        type = b.type,
                    //                        price = b.price,
                    //                        priceWOTax = b.priceWOTax,
                    //                        tax = b.tax,
                    //                        name = b.name,
                    //                        itemId = b.itemId,
                    //                        isDefault = b.isDefault,
                    //                        isSet = b.isSet,
                    //                        qty = b.qty,
                    //                        img = b.img,
                    //                        ModiImg = b.ModiImg,
                    //                        mustDefault = b.mustDefault,
                    //                        status = b.status,
                    //                        parentItemId = b.parentItemId,
                    //                        parentModifierId = b.parentModifierId,
                    //                        IsEnable = b.IsEnable,
                    //                        IsCheck = b.IsCheck
                    //                    }).ToList()
                    //                });
                    //            }
                    //        });

                    //        CartList.Add(new CartModel.MenuDetails
                    //            (
                    //                itemQueue,
                    //                x.groupId,
                    //                x.catId,
                    //                x.catName,
                    //                x.itemId,
                    //                x.itemTemplateId,
                    //                x.itemCategoryId,
                    //                x.itemType,
                    //                x.itemName,
                    //                x.ItemDesc,
                    //                x.img,
                    //                x.MenuImagePath,
                    //                x.originalPrice,
                    //                x.price,
                    //                x.priceWOTax,
                    //                x.tax,
                    //                x.hasModifier,
                    //                x.itemCode,
                    //                x.status,
                    //                x.hasStock,
                    //                x.IsInactive,
                    //                x.MenuImage,
                    //                ModifierDetails,
                    //                x.ItemCurrentQty,
                    //                x.ItemTotalPrice,
                    //                x.TempTotalPrice,
                    //                x.modifiertypes.Select(y => new CartModel.ModifierTypeDetails
                    //                {
                    //                    modifierId = y.modifierId,
                    //                    name = y.name,
                    //                    type = y.type,
                    //                    isSet = y.isSet,
                    //                    minNumber = y.minNumber,
                    //                    maxNumber = y.maxNumber,
                    //                    mustDefault = y.mustDefault,
                    //                    status = y.status,
                    //                    ordermodifiers = y.modifiers.Select(z => new CartModel.ModifiersDetails
                    //                    {
                    //                        modifierId = z.modifierId,
                    //                        type = z.type,
                    //                        price = z.price,
                    //                        priceWOTax = z.priceWOTax,
                    //                        tax = z.tax,
                    //                        name = z.name,
                    //                        itemId = z.itemId,
                    //                        isDefault = z.isDefault,
                    //                        isSet = z.isSet,
                    //                        qty = z.qty,
                    //                        img = z.img,
                    //                        ModiImg = z.ModiImg,
                    //                        mustDefault = z.mustDefault,
                    //                        status = z.status,
                    //                        parentItemId = z.parentItemId,
                    //                        parentModifierId = z.parentModifierId,
                    //                        IsEnable = z.IsEnable,
                    //                        IsCheck = z.IsCheck
                    //                    }).ToList()
                    //                }).ToList(),
                    //                sizingDetail,
                    //                sizings,
                    //                0,
                    //                0,
                    //                false,
                    //                false,
                    //                "U"
                    //             ));

                    //    });
                }
                CalculateTotalItem();

                //Thread changeView = new Thread(() =>
                //{
                //    try
                //    {
                //        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                //        {
                //            SetStage(eStage.DoneAddCart);
                //        }));

                //        Thread.Sleep(1800);

                //        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                //        {
                //            SetStage(eStage.MenuItem);
                //        }));
                //    }
                //    catch (Exception ex)
                //    {
                //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("AddToCartAction changeView = {0}", ex.Message), _TraceCategory);
                //    }
                //});
                //changeView.Start();

                //SetStage(eStage.MenuItem);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger AddToCart Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] AddToCart = {0}", ex.Message), _TraceCategory);
            }
        }

        public bool CheckSimilarity(int? itemID, out int itemQueue)
        {
            bool similar = true;
            int tempItemId = 0;
            int foundSimilarItem = 0;
            int itemCheck = 0;
            itemQueue = 0;

            ObservableCollection<ApiModel.GetMenu.Response.Product> TempMenu = null;

            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger CheckSimilarity Starting ..."), _TraceCategory);

                if (itemID.HasValue)
                {
                    tempItemId = (int)itemID;
                    //TempMenu = new ObservableCollection<ApiModel.GetMenu.Response.Product>();
                }
                else
                {
                    if (Stage == eStage.MenuCustomize)
                    {
                        tempItemId = MenuDetails.Select(x => x.itemId).FirstOrDefault();
                        TempMenu = new ObservableCollection<ApiModel.GetMenu.Response.Product>(MenuDetails.Where(x => x.itemId == tempItemId).ToList());
                    }
                    else if (Stage == eStage.EditItem)
                    {
                        tempItemId = TempEditItem.Select(x => x.itemId).FirstOrDefault();
                        TempMenu = new ObservableCollection<ApiModel.GetMenu.Response.Product>();

                        TempEditItem.ToList().ForEach(y =>
                        {
                            TempMenu.Add(new ApiModel.GetMenu.Response.Product
                                (
                                y.DOUBLE_Sale_Price,
                                y.DOUBLE_Employee_Price,
                                y.DOUBLE_Wholesale_Price,
                                y.DOUBLE_Custom_Price,
                                y.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                y.DOUBLE_Web_Price,
                                y.DOUBLE_Web_Dealer_Price,
                                y.customField1,
                                y.customField2,
                                y.customField3,
                                y.customField4,
                                y.customField5,
                                y.customField6,
                                y.customField7,
                                y.customField8,
                                y.customField9,
                                y.customField10,
                                y.customField11,
                                y.customField12,
                                y.customField13,
                                y.customField14,
                                y.customField15,
                                y.customField16,
                                y.customField17,
                                y.customField18,
                                y.itemName,
                                y.colorHex,
                                y.dynamicHeaderLabel,
                                y.dynamicmodifiers.Select(a => new ApiModel.GetMenu.Response.DynamicModifier
                                {
                                    DOUBLE_Sale_Price = a.DOUBLE_Sale_Price,
                                    DOUBLE_Employee_Price = a.DOUBLE_Employee_Price,
                                    DOUBLE_Wholesale_Price = a.DOUBLE_Wholesale_Price,
                                    DOUBLE_Custom_Price = a.DOUBLE_Custom_Price,
                                    DOUBLE_Manufacturer_Suggested_Retail_Price = a.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                    DOUBLE_Web_Price = a.DOUBLE_Web_Price,
                                    DOUBLE_Web_Dealer_Price = a.DOUBLE_Web_Dealer_Price,
                                    customField1 = a.customField1,
                                    customField2 = a.customField2,
                                    customField3 = a.customField3,
                                    customField4 = a.customField4,
                                    customField5 = a.customField5,
                                    customField6 = a.customField6,
                                    customField7 = a.customField7,
                                    customField8 = a.customField8,
                                    customField9 = a.customField9,
                                    customField10 = a.customField10,
                                    customField11 = a.customField11,
                                    customField12 = a.customField12,
                                    customField13 = a.customField13,
                                    customField14 = a.customField14,
                                    customField15 = a.customField15,
                                    customField16 = a.customField16,
                                    customField17 = a.customField17,
                                    customField18 = a.customField18,
                                    itemName = a.itemName,
                                    itemShortName = a.itemShortName,
                                    defaultSelected = a.defaultSelected,
                                    modifiers = a.modifiers.Select(b => new ApiModel.GetMenu.Response.Modifier
                                    {
                                        type = b.type,
                                        minSelection = b.minSelection,
                                        maxSelection = b.maxSelection,
                                        groupId = b.groupId,
                                        groupName = b.groupName,
                                        selections = b.selections.Select(c => new ApiModel.GetMenu.Response.Selection
                                        {
                                            name = c.name,
                                            defaultSelected = c.defaultSelected,
                                            itemId = c.itemId,
                                            itemCode = c.itemCode,
                                            itemType = c.itemType,
                                            price = c.price,
                                            DOUBLE_Sale_Price = c.DOUBLE_Sale_Price,
                                            DOUBLE_Employee_Price = c.DOUBLE_Employee_Price,
                                            DOUBLE_Wholesale_Price = c.DOUBLE_Wholesale_Price,
                                            DOUBLE_Custom_Price = c.DOUBLE_Custom_Price,
                                            DOUBLE_Manufacturer_Suggested_Retail_Price = c.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                            DOUBLE_Web_Price = c.DOUBLE_Web_Price,
                                            DOUBLE_Web_Dealer_Price = c.DOUBLE_Web_Dealer_Price,
                                            imageUrlList = c.imageUrlList,
                                            binaryImageUrl = c.binaryImageUrl,
                                            imageUrl = c.imageUrl,
                                            description = c.description,
                                            customField1 = c.customField1,
                                            customField2 = c.customField2,
                                            customField3 = c.customField3,
                                            customField4 = c.customField4,
                                            customField5 = c.customField5,
                                            customField6 = c.customField6,
                                            customField7 = c.customField7,
                                            customField8 = c.customField8,
                                            customField9 = c.customField9,
                                            customField10 = c.customField10,
                                            customField11 = c.customField11,
                                            customField12 = c.customField12,
                                            customField13 = c.customField13,
                                            customField14 = c.customField14,
                                            customField15 = c.customField15,
                                            customField16 = c.customField16,
                                            customField17 = c.customField17,
                                            customField18 = c.customField18,
                                            size = c.size,
                                            boolOpenPrice = c.boolOpenPrice,
                                            stockType = c.stockType,
                                            IsAssortment = c.IsAssortment,
                                            HasStock = c.HasStock,
                                            AllowToSell = c.AllowToSell,
                                            IsActive = c.IsActive,
                                            groupId = c.groupId,
                                            minSelection = c.minSelection,
                                            maxSelection = c.maxSelection,
                                            parentItemId = c.parentItemId,
                                            MenuImage = c.MenuImage,
                                            MenuImagePath = c.MenuImagePath,
                                            IsEnable = c.IsEnable,
                                            IsCheck = c.IsCheck

                                        }).ToList()
                                    }).ToList()
                                }).ToList(),
                                y.itemId,
                                y.itemCode,
                                y.itemType,
                                y.price,
                                y.imageUrlList,
                                y.binaryImageUrl,
                                y.imageUrl,
                                y.description,
                                y.size,
                                y.boolOpenPrice,
                                y.stockType,
                                y.IsAssortment,
                                y.HasStock,
                                y.AllowToSell,
                                y.IsActive,
                                y.MenuImagePath,
                                y.MenuImage,
                                y.ItemCurrentQty,
                                y.ItemTotalPrice,
                                y.TempTotalPrice
                                ));
                        });
                    }
                }

                foundSimilarItem = CartList.Where(x => x.itemId == tempItemId).Count();

                if (foundSimilarItem > 0)
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Found similar item : {0} ", foundSimilarItem), _TraceCategory);

                    TempCartList = new ObservableCollection<CartModel.Product>(CartList.Where(x => x.itemId == tempItemId).ToList());
                    foreach (var item in TempMenu)
                    {
                        foreach (var cartItem in TempCartList)
                        {
                            itemCheck++;
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Item [{0}] ", itemCheck), _TraceCategory);
                            foreach (var modifier in item.dynamicmodifiers.SelectMany(modifier => modifier.modifiers).ToList())
                            {
                                foreach (var modifierCart in cartItem.dynamicmodifiers.SelectMany(cartModifier => cartModifier.modifiers).ToList())
                                {
                                    if (modifier.groupId == modifierCart.groupId)
                                    {
                                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Comparing {0} with {1} ", modifier.groupName, modifierCart.groupName), _TraceCategory);

                                        foreach (var modifierDetails in modifier.selections)
                                        {
                                            foreach (var CartModifierDetails in modifierCart.selections)
                                            {
                                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Comparing {0} with {1} ", modifierDetails.name, CartModifierDetails.name), _TraceCategory);

                                                if (modifierDetails.itemId == CartModifierDetails.itemId)
                                                {
                                                    if (modifierDetails.IsCheck == CartModifierDetails.IsCheck)
                                                    {
                                                        similar = true;
                                                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Comparing {0} {2} with {1} {3} : {4}", modifierDetails.name, CartModifierDetails.name, modifierDetails.IsCheck, CartModifierDetails.IsCheck, similar), _TraceCategory);
                                                    }
                                                    else
                                                    {
                                                        similar = false;
                                                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Comparing {0} {2} with {1} {3} : {4}", modifierDetails.name, CartModifierDetails.name, modifierDetails.IsCheck, CartModifierDetails.IsCheck, similar), _TraceCategory);

                                                        if (itemCheck == foundSimilarItem)
                                                        {
                                                            itemQueue = cartItem.cartMenuNo;
                                                            return similar;
                                                        }
                                                    }

                                                    break;
                                                }
                                            }
                                            if (!similar)
                                                break;
                                        }

                                    }
                                    if (!similar)
                                        break;
                                }
                                if (!similar)
                                    break;
                            }
                            if (similar)
                            {
                                itemQueue = cartItem.cartMenuNo;
                                return similar;
                            }
                        }


                    }
                }
                else
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("No similar item found..."), _TraceCategory);
                    similar = false;
                }

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger CheckSimilarity Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] AddToCart = {0}", ex.Message), _TraceCategory);
            }
            return similar;
        }
        #endregion


        #endregion

        #region Summary Page
        #region property
        int index;
        int queueNoTemp;

        private int _BorderHeight;

        public int BorderHeight
        {
            get { return _BorderHeight; }
            set
            {
                _BorderHeight = value;
                OnPropertyChanged(nameof(BorderHeight));
            }
        }


        private string _CustomerPhoneNo;

        public string CustomerPhoneNo
        {
            get { return _CustomerPhoneNo; }
            set
            {
                _CustomerPhoneNo = value;
                OnPropertyChanged(nameof(CustomerPhoneNo));
            }
        }

        private Visibility _QRPopUpVisbility;

        public Visibility QRPopUpVisbility
        {
            get { return _QRPopUpVisbility; }
            set
            {
                _QRPopUpVisbility = value;
                OnPropertyChanged(nameof(QRPopUpVisbility));
            }
        }

        private Visibility _LoginVisbility;

        public Visibility LoginVisbility
        {
            get { return _LoginVisbility; }
            set
            {
                _LoginVisbility = value;
                OnPropertyChanged(nameof(LoginVisbility));
            }
        }

        private Visibility _LogOutBorderVisibility;

        public Visibility LogOutBorderVisibility
        {
            get { return _LogOutBorderVisibility; }
            set
            {
                _LogOutBorderVisibility = value;
                OnPropertyChanged(nameof(LogOutBorderVisibility));
            }
        }

        private Visibility _LogInBorderVisibility;

        public Visibility LogInBorderVisibility
        {
            get { return _LogInBorderVisibility; }
            set
            {
                _LogInBorderVisibility = value;
                OnPropertyChanged(nameof(LogInBorderVisibility));
            }
        }

        private double _AnWVoucherAmount;

        public double AnWVoucherAmount
        {
            get { return _AnWVoucherAmount; }
            set
            {
                _AnWVoucherAmount = value;
                OnPropertyChanged(nameof(VoucherAmountVisiblity));
                OnPropertyChanged(nameof(AnWVoucherAmount));
            }
        }

        private Visibility _VoucherAmountVisiblity;

        public Visibility VoucherAmountVisiblity
        {
            get
            {
                if (AnWVoucherAmount > 0)
                    _VoucherAmountVisiblity = Visibility.Visible;
                else
                    _VoucherAmountVisiblity = Visibility.Collapsed;
                return _VoucherAmountVisiblity;
            }
            set
            {
                _VoucherAmountVisiblity = value;
                OnPropertyChanged(nameof(VoucherAmountVisiblity));
            }
        }


        private double _AnWGiftVoucher;

        public double AnWGiftVoucher
        {
            get { return _AnWGiftVoucher; }
            set
            {
                _AnWGiftVoucher = value;
                OnPropertyChanged(nameof(GiftAmountVisiblity));
                OnPropertyChanged(nameof(AnWGiftVoucher));
            }
        }

        private Visibility _GiftAmountVisiblity;

        public Visibility GiftAmountVisiblity
        {
            get
            {
                if (AnWGiftVoucher > 0)
                    _GiftAmountVisiblity = Visibility.Visible;
                else
                    _GiftAmountVisiblity = Visibility.Collapsed;
                return _GiftAmountVisiblity;
            }
            set
            {
                _GiftAmountVisiblity = value;
                OnPropertyChanged(nameof(GiftAmountVisiblity));
            }
        }

        private double _AnWRounding;

        public double AnWRounding
        {
            get { return _AnWRounding; }
            set
            {
                _AnWRounding = value;
                OnPropertyChanged(nameof(AnWRounding));
            }
        }

        private double _AnWTotalAmount;

        public double AnWTotalAmount
        {
            get { return _AnWTotalAmount; }
            set
            {
                _AnWTotalAmount = value;
                OnPropertyChanged(nameof(AnWTotalAmount));
                OnPropertyChanged(nameof(AnWRootyPoints));
            }
        }

        private double _AnWRootyPoints;

        public double AnWRootyPoints
        {
            get
            {
                _AnWRootyPoints = AnWTotalAmount * 10;
                return _AnWRootyPoints;
            }
            set
            {
                _AnWRootyPoints = value;
                OnPropertyChanged(nameof(AnWRootyPoints));
            }
        }


        private double _AnWTax;

        public double AnWTax
        {
            get { return _AnWTax; }
            set
            {
                _AnWTax = value;
                OnPropertyChanged(nameof(AnWTax));
            }
        }

        #endregion

        #region Command
        private ICommand _BtnLogIn;
        public ICommand BtnLogIn
        {
            get
            {
                if (_BtnLogIn == null)
                    _BtnLogIn = new RelayCommand(LogInPopUp);
                return _BtnLogIn;
            }
        }

        //private ICommand _BtnLogOut;
        //public ICommand BtnLogOut
        //{
        //    get
        //    {
        //        if (_BtnLogOut == null)
        //            _BtnLogOut = new RelayCommand(LogOut);
        //        return _BtnLogOut;
        //    }
        //}

        private ICommand _BtnCloseLogIn;
        public ICommand BtnCloseLogIn
        {
            get
            {
                if (_BtnCloseLogIn == null)
                    _BtnCloseLogIn = new RelayCommand(CloseLogInPopUp);
                return _BtnCloseLogIn;
            }
        }

        private ICommand _BtnProceedPayment;
        public ICommand BtnProceedPayment
        {
            get
            {
                if (_BtnProceedPayment == null)
                    _BtnProceedPayment = new RelayCommand(ProceedPayment);
                return _BtnProceedPayment;
            }
        }

        private ICommand _ShowVoucher;
        public ICommand ShowVoucher
        {
            get
            {
                if (_ShowVoucher == null)
                    _ShowVoucher = new RelayCommand(ShowVoucherAction);
                return _ShowVoucher;
            }
        }

        private ICommand _ShowVoucherPopup;
        public ICommand ShowVoucherPopup
        {
            get
            {
                if (_ShowVoucherPopup == null)
                    _ShowVoucherPopup = new RelayCommand(ShowVoucherPopupAction);
                return _ShowVoucherPopup;
            }
        }

        private ICommand _BtnPromoCode;
        public ICommand BtnPromoCode
        {
            get
            {
                if (_BtnPromoCode == null)
                    _BtnPromoCode = new RelayCommand(CheckPromoCode);
                return _BtnPromoCode;
            }
        }
        #endregion

        #region Function
        public void EditOrderCart(int queueNo)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger EditOrderCart Starting ..."), _TraceCategory);
                queueNoTemp = queueNo;
                index = 0;

                index = CartList.IndexOf(CartList.Single(x => x.cartMenuNo == queueNo));

                TempEditItem = null;
                TempEditItem = new ObservableCollection<CartModel.Product>();

                CartList.Where(t => t.cartMenuNo == queueNo).ToList().ForEach(x =>
                {
                    ObservableCollection<CartModel.Modifier> ModifierDetails = new ObservableCollection<CartModel.Modifier>();

                    x.dynamicmodifiers.SelectMany(modifier => modifier.modifiers).ToList().ForEach(a =>
                    {
                        var checkModifiers = a.selections.Where(b => b.IsCheck).ToList();

                        if (checkModifiers.Any())
                        {
                            ModifierDetails.Add(new CartModel.Modifier
                            {
                                type = a.type,
                                minSelection = a.minSelection,
                                maxSelection = a.maxSelection,
                                groupId = a.groupId,
                                groupName = a.groupName,
                                selections = checkModifiers.Select(c => new CartModel.Selection
                                {
                                    name = c.name,
                                    defaultSelected = c.defaultSelected,
                                    itemId = c.itemId,
                                    itemCode = c.itemCode,
                                    itemType = c.itemType,
                                    price = c.price,
                                    DOUBLE_Sale_Price = c.DOUBLE_Sale_Price,
                                    DOUBLE_Employee_Price = c.DOUBLE_Employee_Price,
                                    DOUBLE_Wholesale_Price = c.DOUBLE_Wholesale_Price,
                                    DOUBLE_Custom_Price = c.DOUBLE_Custom_Price,
                                    DOUBLE_Manufacturer_Suggested_Retail_Price = c.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                    DOUBLE_Web_Price = c.DOUBLE_Web_Price,
                                    DOUBLE_Web_Dealer_Price = c.DOUBLE_Web_Dealer_Price,
                                    imageUrlList = c.imageUrlList,
                                    binaryImageUrl = c.binaryImageUrl,
                                    imageUrl = c.imageUrl,
                                    description = c.description,
                                    customField1 = c.customField1,
                                    customField2 = c.customField2,
                                    customField3 = c.customField3,
                                    customField4 = c.customField4,
                                    customField5 = c.customField5,
                                    customField6 = c.customField6,
                                    customField7 = c.customField7,
                                    customField8 = c.customField8,
                                    customField9 = c.customField9,
                                    customField10 = c.customField10,
                                    customField11 = c.customField11,
                                    customField12 = c.customField12,
                                    customField13 = c.customField13,
                                    customField14 = c.customField14,
                                    customField15 = c.customField15,
                                    customField16 = c.customField16,
                                    customField17 = c.customField17,
                                    customField18 = c.customField18,
                                    size = c.size,
                                    boolOpenPrice = c.boolOpenPrice,
                                    stockType = c.stockType,
                                    IsAssortment = c.IsAssortment,
                                    HasStock = c.HasStock,
                                    AllowToSell = c.AllowToSell,
                                    IsActive = c.IsActive,
                                    groupId = c.groupId,
                                    minSelection = c.minSelection,
                                    maxSelection = c.maxSelection,
                                    parentItemId = c.parentItemId,
                                    MenuImage = c.MenuImage,
                                    MenuImagePath = c.MenuImagePath,
                                    IsCheck = c.IsCheck,
                                    IsEnable = c.IsEnable

                                }).ToList()
                            });
                        }
                    });

                    TempEditItem.Add(new CartModel.Product
                      (
                          queueNo,
                          ModifierDetails,
                          x.DOUBLE_Sale_Price,
                            x.DOUBLE_Employee_Price,
                            x.DOUBLE_Wholesale_Price,
                            x.DOUBLE_Custom_Price,
                            x.DOUBLE_Manufacturer_Suggested_Retail_Price,
                            x.DOUBLE_Web_Price,
                            x.DOUBLE_Web_Dealer_Price,
                            x.customField1,
                            x.customField2,
                            x.customField3,
                            x.customField4,
                            x.customField5,
                            x.customField6,
                            x.customField7,
                            x.customField8,
                            x.customField9,
                            x.customField10,
                            x.customField11,
                            x.customField12,
                            x.customField13,
                            x.customField14,
                            x.customField15,
                            x.customField16,
                            x.customField17,
                            x.customField18,
                            x.itemName,
                            x.colorHex,
                            x.dynamicHeaderLabel,
                            x.dynamicmodifiers.Select(a => new CartModel.DynamicModifier
                            {
                                DOUBLE_Sale_Price = a.DOUBLE_Sale_Price,
                                DOUBLE_Employee_Price = a.DOUBLE_Employee_Price,
                                DOUBLE_Wholesale_Price = a.DOUBLE_Wholesale_Price,
                                DOUBLE_Custom_Price = a.DOUBLE_Custom_Price,
                                DOUBLE_Manufacturer_Suggested_Retail_Price = a.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                DOUBLE_Web_Price = a.DOUBLE_Web_Price,
                                DOUBLE_Web_Dealer_Price = a.DOUBLE_Web_Dealer_Price,
                                customField1 = a.customField1,
                                customField2 = a.customField2,
                                customField3 = a.customField3,
                                customField4 = a.customField4,
                                customField5 = a.customField5,
                                customField6 = a.customField6,
                                customField7 = a.customField7,
                                customField8 = a.customField8,
                                customField9 = a.customField9,
                                customField10 = a.customField10,
                                customField11 = a.customField11,
                                customField12 = a.customField12,
                                customField13 = a.customField13,
                                customField14 = a.customField14,
                                customField15 = a.customField15,
                                customField16 = a.customField16,
                                customField17 = a.customField17,
                                customField18 = a.customField18,
                                itemName = a.itemName,
                                itemShortName = a.itemShortName,
                                defaultSelected = a.defaultSelected,
                                modifiers = a.modifiers.Select(b => new CartModel.Modifier
                                {
                                    type = b.type,
                                    minSelection = b.minSelection,
                                    maxSelection = b.maxSelection,
                                    groupId = b.groupId,
                                    groupName = b.groupName,
                                    selections = b.selections.Select(c => new CartModel.Selection
                                    {
                                        name = c.name,
                                        defaultSelected = c.defaultSelected,
                                        itemId = c.itemId,
                                        itemCode = c.itemCode,
                                        itemType = c.itemType,
                                        price = c.price,
                                        DOUBLE_Sale_Price = c.DOUBLE_Sale_Price,
                                        DOUBLE_Employee_Price = c.DOUBLE_Employee_Price,
                                        DOUBLE_Wholesale_Price = c.DOUBLE_Wholesale_Price,
                                        DOUBLE_Custom_Price = c.DOUBLE_Custom_Price,
                                        DOUBLE_Manufacturer_Suggested_Retail_Price = c.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                        DOUBLE_Web_Price = c.DOUBLE_Web_Price,
                                        DOUBLE_Web_Dealer_Price = c.DOUBLE_Web_Dealer_Price,
                                        imageUrlList = c.imageUrlList,
                                        binaryImageUrl = c.binaryImageUrl,
                                        imageUrl = c.imageUrl,
                                        description = c.description,
                                        customField1 = c.customField1,
                                        customField2 = c.customField2,
                                        customField3 = c.customField3,
                                        customField4 = c.customField4,
                                        customField5 = c.customField5,
                                        customField6 = c.customField6,
                                        customField7 = c.customField7,
                                        customField8 = c.customField8,
                                        customField9 = c.customField9,
                                        customField10 = c.customField10,
                                        customField11 = c.customField11,
                                        customField12 = c.customField12,
                                        customField13 = c.customField13,
                                        customField14 = c.customField14,
                                        customField15 = c.customField15,
                                        customField16 = c.customField16,
                                        customField17 = c.customField17,
                                        customField18 = c.customField18,
                                        size = c.size,
                                        boolOpenPrice = c.boolOpenPrice,
                                        stockType = c.stockType,
                                        IsAssortment = c.IsAssortment,
                                        HasStock = c.HasStock,
                                        AllowToSell = c.AllowToSell,
                                        IsActive = c.IsActive,
                                        groupId = c.groupId,
                                        minSelection = c.minSelection,
                                        maxSelection = c.maxSelection,
                                        parentItemId = c.parentItemId,
                                        MenuImage = c.MenuImage,
                                        MenuImagePath = c.MenuImagePath,
                                        IsCheck = c.IsCheck,
                                        IsEnable = c.IsEnable
                                    }).ToList()
                                }).ToList(),
                                itemId = a.itemId,
                                itemCode = a.itemCode,
                                itemType = a.itemType,
                                price = a.price,
                                imageUrlList = a.imageUrlList,
                                binaryImageUrl = a.binaryImageUrl,
                                imageUrl = a.imageUrl,
                                description = a.description,
                                size = a.size,
                                boolOpenPrice = a.boolOpenPrice,
                                stockType = a.stockType,
                                IsAssortment = a.IsAssortment,
                                HasStock = a.HasStock,
                                AllowToSell = a.AllowToSell,
                                IsActive = a.IsActive,
                                MenuImage = a.MenuImage,
                                MenuImagePath = a.MenuImagePath

                            }).ToList(),
                            x.itemId,
                            x.itemCode,
                            x.itemType,
                            x.price,
                            x.imageUrlList,
                            x.binaryImageUrl,
                            x.imageUrl,
                            x.description,
                            x.size,
                            x.boolOpenPrice,
                            x.stockType,
                            x.IsAssortment,
                            x.HasStock,
                            x.AllowToSell,
                            x.IsActive,
                            x.MenuImagePath,
                            x.MenuImage,
                            x.ItemCurrentQty,
                            x.ItemTotalPrice,
                            x.TempTotalPrice
                       ));
                });

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    SetStage(eStage.EditItem);
                }));


                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger EditOrderCart Done ..."), _TraceCategory);
            }

            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger EditOrderCart : {0}", ex.ToString()), _TraceCategory);
            }
        }

        public void EditUpgradeMenu(int itemID)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger EditOrderCart Starting ..."), _TraceCategory);

                int tempPreviousItemQty = TempEditItem.Select(x => x.ItemCurrentQty).FirstOrDefault();

                TempEditItem = null;
                TempEditItem = new ObservableCollection<CartModel.Product>();


                if (TempEditItem != null)
                {
                    if (TempEditItem.Count() > 0)
                    {
                        foreach (var modifiertype in TempEditItem.SelectMany(dynamicMod => dynamicMod.dynamicmodifiers).ToList())
                        {
                            foreach (var modifier in modifiertype.modifiers)
                            {
                                if (modifier.minSelection == 1 && modifier.selections.Count() == 1)
                                {
                                    modifier.selections[0].IsEnable = false;
                                }

                                if (modifier.minSelection == 1)
                                {
                                    modifier.selections[0].IsCheck = true;
                                }
                            }
                        }
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("TempEditMenu less than 0"), _TraceCategory);
                    }
                }
                else
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("TempEditMenu is null"), _TraceCategory);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger EditOrderCart Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger EditOrderCart : {0}", ex.ToString()), _TraceCategory);
            }
        }

        bool customerLoginMenu;
        public bool CheckQR()
        {
            bool success = false;
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger CheckQR Starting ..."), _TraceCategory);

                string[] QrData = QRValue.Split(new Char[] { ';' }, 2);
                if (QrData.Length != 2)
                {
                    TxtErrorHeader = Lbl_Error;
                    TxtErrorMessage = Lbl_InvalidQr;

                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("QrData Length - {0}", QrData.Length), _TraceCategory);
                    throw new Exception("[Error] Trigger CheckQR : Invalid Qr Code");
                }

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("QrData(1) - {0}", QrData[0]), _TraceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("QrData(2) - {0}", QrData[1]), _TraceCategory);


                CustomerPhoneNo = string.Empty;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger threadLogIn Starting ..."), _TraceCategory);

                ShowLoading = Visibility.Collapsed;
                QRPopUpVisbility = Visibility.Collapsed;
                BackgroundVisibility = Visibility.Collapsed;

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger threadLogIn Done ..."), _TraceCategory);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger CheckQR Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger CheckQR : {0}", ex.ToString()), _TraceCategory);
            }
            return success;
        }

        public void LogInPopUp()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger LogInPopUp Starting ..."), _TraceCategory);
                QRPopUpVisbility = Visibility.Visible;
                BackgroundVisibility = Visibility.Visible;
                readyToScan = true;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger LogInPopUp Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger LogInPopUp : {0}", ex.ToString()), _TraceCategory);
            }
        }

        //public void LogOut()
        //{
        //    try
        //    {
        //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger LogOut Starting ..."), _TraceCategory);
        //        ShowLoading = Visibility.Visible;
        //        Thread thLogout = new Thread(() =>
        //        {


        //            LogInBorderVisibility = Visibility.Visible;
        //            LogOutBorderVisibility = Visibility.Collapsed;
        //            PointGridVisibility = Visibility.Collapsed;
        //            CustomerPhoneNo = string.Empty;
        //            //CustomerDetails = null;
        //            BarcodeInfo = string.Empty;
        //            AnWVoucherAmount = 0;
        //            AnWGiftVoucher = 0;
        //            //VoucherList = null;
        //            CustomerID = 0;
        //            AnWRounding = 0;
        //            BorderHeight = 1300;
        //            Voucher = new List<Order.OrderReward>();
        //            VoucherQty = 0;

        //            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
        //            {
        //                var rewardMenu = CartList.Where(x => x.isReward == true).ToList();
        //                foreach (var reward in rewardMenu)
        //                {
        //                    CartList.Remove(reward);
        //                }

        //                if (customerLoginMenu)
        //                {
        //                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] Clearing Customer Category Menu"), _TraceCategory);
        //                    //int groupId = CateDetails.First().groupId;

        //                    //var cateToRemove = CateDetails.Where(x => x.groupId == groupId).ToList();
        //                    //foreach (var item in cateToRemove)
        //                    //{
        //                    //    CateDetails.Remove(item);
        //                    //}

        //                    //var menuToRemove = TempListMenuDetails.Where(x => x.groupId == groupId).ToList();
        //                    //foreach (var menu in menuToRemove)
        //                    //{
        //                    //    TempListMenuDetails.Remove(menu);
        //                    //}                            

        //                    //CateDetails[0].IsChecked = true;
        //                    //RefreshProduct(CateDetails[0].groupId);
        //                    customerLoginMenu = false;
        //                }
        //                else
        //                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Info] Customer Category Menu No exist"), _TraceCategory);

        //            }));

        //            CalculateTotalItem();
        //            ShowLoading = Visibility.Collapsed;
        //        });
        //        thLogout.Start();


        //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger LogOut Done ..."), _TraceCategory);
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] LogOut LogInPopUp : {0}", ex.ToString()), _TraceCategory);
        //    }
        //}

        public void CloseLogInPopUp()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger CloseLogInPopUp Starting ..."), _TraceCategory);
                if (QRPopUpVisbility == Visibility.Visible)
                {
                    QRPopUpVisbility = Visibility.Collapsed;
                    BackgroundVisibility = Visibility.Collapsed;
                }
                else if (LoginVisbility == Visibility.Visible)
                {
                    LoginVisbility = Visibility.Collapsed;
                    BackgroundVisibility = Visibility.Collapsed;
                    LoginPopUp = false;
                }
                readyToScan = false;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Closing PopUp at Stage {0}", Stage), _TraceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger CloseLogInPopUp Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger CloseLogInPopUp : {0}", ex.ToString()), _TraceCategory);
            }
        }

        public void ProceedPayment()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger ProceedPayment Starting ..."), _TraceCategory);
                //if (AnWTotalAmount > 0)
                    SetStage(eStage.PaymentMethodSelection);
                //else
                //{
                //    try
                //    {
                //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Thread thDirectOrder Starting ..."), _TraceCategory);
                //        ShowLoading = Visibility.Visible;
                //        Thread thDirectOrder = new Thread(() =>
                //        {
                //            DirectOrder();
                //        });
                //        thDirectOrder.Start();
                //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Thread thDirectOrder Starting ..."), _TraceCategory);
                //    }
                //    catch (Exception ex)
                //    {
                //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Thread thDirectOrder : {0}", ex.ToString()), _TraceCategory);
                //    }
                //}

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger ProceedPayment Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger ProceedPayment : {0}", ex.ToString()), _TraceCategory);
            }
        }

        //direct order if amount = 0
        public void DirectOrder()
        {
            try
            {
                _PaymentCategoryName = "";
                _PaymentCategoryName = "NA";
                //if (InitialOrder("PF"))
                //{
                //    ShowLoading = Visibility.Collapsed;
                //    //if (initialOrderResponse != null)
                //    //{
                //    //    intialOrderRequest.orderId = Convert.ToInt32(initialOrderResponse.OrderId);
                //    //    if (PostOrder(intialOrderRequest,"0" ,initialOrderResponse))
                //    //    {
                //    //        PrintReceipt(true, true, null, null);
                //    //    }
                //    //    else
                //    //    {
                //    //        PrintReceipt(true, false, null, null);
                //    //    }
                //    //}
                //}
                //else
                //{
                //    ShowLoading = Visibility.Collapsed;
                //    TxtErrorHeader = Lbl_Error;
                //    TxtErrorMessage = Lbl_ErrorTryAgain;
                //    WarningMessageBoxVisibility = Visibility.Visible;
                //}
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger DirectOrder : {0}", ex.ToString()), _TraceCategory);
            }

        }

        public void ShowVoucherAction()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger ShowVoucher Starting ..."), _TraceCategory);
                #region hide
                //Thread thShowVoucher = new Thread(() =>
                //{
                //    try
                //    {
                //StopCountDown();

                //ShowLoading = Visibility.Visible;

                //string[] QrData = QRValue.Split(new Char[] { ';' }, 2);

                //Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger thShowVoucher Starting ..."), _TraceCategory);

                //ApiModel.GetPromotions.Request req = null;
                //TempVoucherList = new ObservableCollection<ApiModel.GetPromotions.Response.Voucher>();
                //VoucherList = new ObservableCollection<ApiModel.GetPromotions.Response.Voucher>();

                //req = new ApiModel.GetPromotions.Request(int.Parse(QrData[0]), QrData[1].ToString(), 2, "8cd5704b-0edd-41e3-a45d-07140e3d29bc");

                //if(_ApiFunc.GetPromotions(req,out ApiModel.GetPromotions.Response res))
                //{
                //    if(res!=null)
                //    {
                //        if (res.Code.Equals("00"))
                //        {
                //            foreach(var voucher in res.VoucherList)
                //            {
                //                ApiModel.GetPromotions.Response.Voucher newVoucherList = new ApiModel.GetPromotions.Response.Voucher
                //                (
                //                    voucher.Code,
                //                    voucher.Type,
                //                    voucher.ItemType,
                //                    voucher.RedeemTxId,
                //                    voucher.RedeemId,
                //                    voucher.Amount,
                //                    voucher.MaxAmount,
                //                    voucher.CatId,
                //                    voucher.ItemId,
                //                    voucher.ItemCode,
                //                    voucher.ItemName,
                //                    voucher.ItemImg,
                //                    voucher.Points,
                //                    voucher.IsAvailable,
                //                    voucher.ExpiredDate,
                //                    voucher.Qty,
                //                    voucher.TermsAndConditions,
                //                   voucher.HasExpired,
                //                   voucher.IsVoucher,
                //                    voucher.IsPromoCode,
                //                    voucher.IsDiscount,
                //                    voucher.IsItem,
                //                    voucher.ShowTerms,
                //                    voucher.ShowQty,
                //                    voucher.EncodedUniqueId,
                //                    voucher.IsCheck
                //                );

                //                TempVoucherList.Add(newVoucherList);
                //            }

                //            TempVoucherList.Where(y => y.Type == "GiftVoucher").ToList().ForEach(z => VoucherList.Add(new ApiModel.GetPromotions.Response.Voucher(
                //                z.Code,
                //                z.Type,
                //                z.ItemType,
                //                z.RedeemTxId,
                //                z.RedeemId,
                //                z.Amount,
                //                z.MaxAmount,
                //                z.CatId,
                //                z.ItemId,
                //                z.ItemCode,
                //                z.ItemName,
                //                z.ItemImg,
                //                z.Points,
                //                z.IsAvailable,
                //                z.ExpiredDate,
                //                z.Qty,
                //                z.TermsAndConditions,
                //                z.HasExpired,
                //                z.IsVoucher,
                //                z.IsPromoCode,
                //                z.IsDiscount,
                //                z.IsItem,
                //                z.ShowTerms,
                //                z.ShowQty,
                //                z.EncodedUniqueId,
                //                z.IsCheck
                //                )));

                //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                //{
                //    DetailsViewVisbility = Visibility.Visible;
                //    SetStage(eStage.Voucher);
                //}));
                //        }
                //        //invalid
                //        else if (res.Code.Equals("01") || res.Code.Equals("10"))
                //        {
                //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("{Error] Login - {0}", res.Message), _TraceCategory);
                //        }
                //        //error
                //        else if (res.Code.Equals("99"))
                //        {
                //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("{Error] Login - {0}", res.Message), _TraceCategory);
                //        }
                //    }
                //}
                //else
                //{
                //    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Login, fail to retrieve voucher list"), _TraceCategory);
                //}
                //ShowLoading = Visibility.Collapsed;
                //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger thShowVoucher Done ..."), _TraceCategory);
                //    }
                //    catch (Exception ex)
                //    {
                //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("{Error] Trigger thShowVoucher : {0}", ex.ToString()), _TraceCategory);
                //    }
                //});
                //thShowVoucher.Start();
                #endregion 

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    BackgroundVisibility = DetailsViewVisbility = Visibility.Visible;
                    SetStage(eStage.Voucher);
                }));

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger ShowVoucher Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger ShowVoucher : {0}", ex.ToString()), _TraceCategory);
            }
        }

        public void ShowVoucherPopupAction()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger ShowVoucher Starting ..."), _TraceCategory);

                VoucherPopupVisibility = Visibility.Visible;
                BackgroundVisibility = Visibility.Visible;

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger ShowVoucher Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger ShowVoucher : {0}", ex.ToString()), _TraceCategory);
            }
        }

        //public bool VoucherApiAction(bool isPromoCode, string voucherId, string promoCodeValue, int orderTypeId, string branchId, List<Order.OrderReward> voucherList, List<Order.OrderMenuList> orderList, int customerId, string accessToken, int componentId, string componentUniqueId)
        //{
        //    bool success = false;
        //    try
        //    {
        //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger VoucherApiAction Starting ..."), _TraceCategory);

        //        //ApiModel.ReadPromotion.Request req = null;
        //        //req = new ApiModel.ReadPromotion.Request(isPromoCode, voucherId, promoCodeValue, orderTypeId, branchId, voucherList, orderList, customerId, accessToken, componentId, componentUniqueId);

        //        //if (_ApiFunc.ReadPromotion(req, out ApiModel.ReadPromotion.Response res))
        //        //{
        //        //    if (res != null)
        //        //    {
        //        //        if (res.Code.Equals("00"))
        //        //        {
        //        //            //if promotion wont return menu item promotion
        //        //            //if item voucher will return both
        //        //            if (res.VoucherItem != null)
        //        //            {
        //        //                if(res.VoucherItem.rewardItemType.ToUpper()== "EmployeeVoucher".ToUpper())
        //        //                {
        //        //                    if(String.IsNullOrEmpty(res.VoucherItem.itemCode))
        //        //                    {
        //        //                        Voucher.Add(new Order.OrderReward
        //        //                        {
        //        //                            rewardTxId = res.VoucherItem.rewardTxId,
        //        //                            rewardSuperId = res.VoucherItem.rewardSuperId,
        //        //                            rewardType = res.VoucherItem.rewardType,
        //        //                            rewardItemType = res.VoucherItem.rewardItemType,
        //        //                            qty = res.VoucherItem.qty,
        //        //                            rewardName = res.VoucherItem.rewardName,
        //        //                            rewardCode = res.VoucherItem.rewardCode,
        //        //                            promoCode = res.VoucherItem.promoCode,
        //        //                            uniqueCode = res.VoucherItem.uniqueCode,
        //        //                            amount = res.VoucherItem.amount,
        //        //                            percentage = res.VoucherItem.percentage,
        //        //                            maxAmount = res.VoucherItem.maxAmount,
        //        //                            itemTxId = res.VoucherItem.itemTxId,
        //        //                            itemCatId = res.VoucherItem.itemCatId,
        //        //                            itemId = res.VoucherItem.itemId,
        //        //                            itemCode = res.VoucherItem.rewardCode,
        //        //                            itemName = res.VoucherItem.itemName,
        //        //                            itemPrice = res.VoucherItem.itemPrice,
        //        //                            itemTax = res.VoucherItem.itemTax,
        //        //                            isVoucher = res.VoucherItem.isVoucher,
        //        //                            isPromoCode = res.VoucherItem.isPromoCode,
        //        //                            isDiscount = res.VoucherItem.isDiscount,
        //        //                            isItem = res.VoucherItem.isItem
        //        //                        });
        //        //                    }
        //        //                    else
        //        //                    {
        //        //                        Voucher.Add(new Order.OrderReward
        //        //                        {
        //        //                            rewardTxId = res.VoucherItem.rewardTxId,
        //        //                            rewardSuperId = res.VoucherItem.rewardSuperId,
        //        //                            rewardType = res.VoucherItem.rewardType,
        //        //                            rewardItemType = res.VoucherItem.rewardItemType,
        //        //                            qty = res.VoucherItem.qty,
        //        //                            rewardName = res.VoucherItem.rewardName,
        //        //                            rewardCode = res.VoucherItem.rewardCode,
        //        //                            promoCode = res.VoucherItem.promoCode,
        //        //                            uniqueCode = res.VoucherItem.uniqueCode,
        //        //                            amount = res.VoucherItem.amount,
        //        //                            percentage = res.VoucherItem.percentage,
        //        //                            maxAmount = res.VoucherItem.maxAmount,
        //        //                            itemTxId = res.VoucherItem.itemTxId,
        //        //                            itemCatId = res.VoucherItem.itemCatId,
        //        //                            itemId = res.VoucherItem.itemId,
        //        //                            itemCode = res.VoucherItem.itemCode,
        //        //                            itemName = res.VoucherItem.itemName,
        //        //                            itemPrice = res.VoucherItem.itemPrice,
        //        //                            itemTax = res.VoucherItem.itemTax,
        //        //                            isVoucher = res.VoucherItem.isVoucher,
        //        //                            isPromoCode = res.VoucherItem.isPromoCode,
        //        //                            isDiscount = res.VoucherItem.isDiscount,
        //        //                            isItem = res.VoucherItem.isItem
        //        //                        });
        //        //                    }
        //        //                }
        //        //                else
        //        //                {
        //        //                    Voucher.Add(new Order.OrderReward
        //        //                    {
        //        //                        rewardTxId = res.VoucherItem.rewardTxId,
        //        //                        rewardSuperId = res.VoucherItem.rewardSuperId,
        //        //                        rewardType = res.VoucherItem.rewardType,
        //        //                        rewardItemType = res.VoucherItem.rewardItemType,
        //        //                        qty = res.VoucherItem.qty,
        //        //                        rewardName = res.VoucherItem.rewardName,
        //        //                        rewardCode = res.VoucherItem.rewardCode,
        //        //                        promoCode = res.VoucherItem.promoCode,
        //        //                        uniqueCode = res.VoucherItem.uniqueCode,
        //        //                        amount = res.VoucherItem.amount,
        //        //                        percentage = res.VoucherItem.percentage,
        //        //                        maxAmount = res.VoucherItem.maxAmount,
        //        //                        itemTxId = res.VoucherItem.itemTxId,
        //        //                        itemCatId = res.VoucherItem.itemCatId,
        //        //                        itemId = res.VoucherItem.itemId,
        //        //                        itemCode = res.VoucherItem.itemCode,
        //        //                        itemName = res.VoucherItem.itemName,
        //        //                        itemPrice = res.VoucherItem.itemPrice,
        //        //                        itemTax = res.VoucherItem.itemTax,
        //        //                        isVoucher = res.VoucherItem.isVoucher,
        //        //                        isPromoCode = res.VoucherItem.isPromoCode,
        //        //                        isDiscount = res.VoucherItem.isDiscount,
        //        //                        isItem = res.VoucherItem.isItem
        //        //                    });
        //        //                }
        //        //            }

        //        //            if (res.MenuItemPromotion != null)
        //        //            {
        //        //                int itemQueue;
        //        //                if (CartList.Count() == 0)
        //        //                {
        //        //                    itemQueue = 1;
        //        //                }
        //        //                else
        //        //                    itemQueue = CartList.Max(x => x.cartMenuNo) + 1;

        //        //                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
        //        //                {
        //        //                    string menuFileName = string.Empty;
        //        //                    BitmapImage menuImagePath = new BitmapImage();
        //        //                    if (!string.IsNullOrEmpty(res.MenuItemPromotion.img))
        //        //                    {
        //        //                        try
        //        //                        {
        //        //                            menuFileName = res.MenuItemPromotion.img.Substring(res.MenuItemPromotion.img.LastIndexOf('/') + 1);
        //        //                            if (GeneralFunc.DownloadMedia(GeneralVar.MenuRepository, menuFileName, res.MenuItemPromotion.img))
        //        //                            {
        //        //                                string loc = Path.Combine(GeneralVar.MenuRepository, menuFileName);
        //        //                                menuImagePath = FilePathToBitmapImage(loc);
        //        //                            }
        //        //                            else
        //        //                            {
        //        //                                menuImagePath.BeginInit();
        //        //                                menuImagePath.UriSource = new Uri("pack://application:,,,/Resource/LFFImages/AnW_Logo.png", UriKind.RelativeOrAbsolute);
        //        //                                menuImagePath.EndInit();
        //        //                            }
        //        //                        }
        //        //                        catch (Exception ex)
        //        //                        {
        //        //                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Download image voucher menu : {0}", ex.ToString()), _TraceCategory);
        //        //                        }

        //        //                    }

        //        //                    ObservableCollection<CartModel.ModifierTypeDetails> tempDetail = new ObservableCollection<CartModel.ModifierTypeDetails>();

        //        //                    CartList.Add(new CartModel.MenuDetails
        //        //                            (itemQueue,
        //        //                            0,
        //        //                            null,
        //        //                            null,
        //        //                            Convert.ToInt32(res.MenuItemPromotion.itemId),
        //        //                            res.MenuItemPromotion.itemTemplateId,
        //        //                            res.MenuItemPromotion.itemCategoryId,
        //        //                            res.MenuItemPromotion.itemType,
        //        //                            res.MenuItemPromotion.itemName,
        //        //                            res.MenuItemPromotion.itemDesc,
        //        //                            res.MenuItemPromotion.img,
        //        //                            menuImagePath,//need to fix
        //        //                            Convert.ToDouble(res.MenuItemPromotion.originalPrice),
        //        //                            Convert.ToDouble(res.MenuItemPromotion.price),
        //        //                            Convert.ToDouble(res.MenuItemPromotion.priceWOTax),
        //        //                            Convert.ToDouble(res.MenuItemPromotion.tax),
        //        //                            0,
        //        //                            res.MenuItemPromotion.itemCode,
        //        //                            "",
        //        //                            true,
        //        //                            false,
        //        //                            menuFileName,
        //        //                            tempDetail,
        //        //                            res.MenuItemPromotion.quantity,
        //        //                            Convert.ToDouble(res.MenuItemPromotion.price),
        //        //                            Convert.ToDouble(res.MenuItemPromotion.price),
        //        //                            res.MenuItemPromotion.orderModifiertypes.Select(x => new CartModel.ModifierTypeDetails
        //        //                            {
        //        //                                modifierId = x.modifierId,
        //        //                                name = x.name,
        //        //                                qty = 1,
        //        //                                price = Convert.ToDouble(x.orderModifiers.Where(a => a.modifierId == x.modifierId).Select(b => b.price).FirstOrDefault()),
        //        //                                ordermodifiers = x.orderModifiers.Select(y => new CartModel.ModifiersDetails
        //        //                                {
        //        //                                    modifierId = y.modifierId,
        //        //                                    name = y.name,
        //        //                                    qty = y.qty,
        //        //                                    price = Convert.ToDouble(y.price),
        //        //                                    priceWOTax = Convert.ToDouble(y.priceWOTax),
        //        //                                    tax = Convert.ToDouble(y.tax),
        //        //                                    isFoc = y.isFoc,
        //        //                                    ModiImg = Path.Combine(GeneralVar.MenuRepository, string.Format("{0}", y.modifierId))
        //        //                                }).ToList()
        //        //                            }).ToList(),
        //        //                            null,
        //        //                            null,
        //        //                            res.MenuItemPromotion.redeemId,
        //        //                            res.MenuItemPromotion.redeemTxId,
        //        //                            res.MenuItemPromotion.isFixed,
        //        //                            res.MenuItemPromotion.isReward,
        //        //                            string.Empty
        //        //                            ));
        //        //                }));

        //        //            }

        //        //            success = true;

        //        //            if (res.Message != null)
        //        //                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger VoucherApiAction : {0}", res.Message.ToString()), _TraceCategory);
        //        //        }
        //        //        else if (res.Code.Equals("01") || res.Code.Equals("10"))
        //        //        {
        //        //            TxtErrorHeader = Lbl_Info;

        //        //            if (res.Message != null)
        //        //            {
        //        //                TxtErrorMessage = res.Message;
        //        //                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger VoucherApiAction : {0}", res.Message.ToString()), _TraceCategory);
        //        //            }
        //        //            else
        //        //            {
        //        //                TxtErrorMessage = Lbl_ErrorTryAgain;
        //        //                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger VoucherApiAction : Unknown Error"), _TraceCategory);
        //        //            }

        //        //        }
        //        //        else if (res.Code.Equals("99"))
        //        //        {
        //        //            TxtErrorHeader = Lbl_Info;
        //        //            if (res.Message != null)
        //        //            {
        //        //                TxtErrorMessage = res.Message;
        //        //                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[rror] Trigger VoucherApiAction : {0}", res.Message.ToString()), _TraceCategory);
        //        //            }
        //        //            else
        //        //            {
        //        //                TxtErrorMessage = Lbl_ErrorTryAgain2;
        //        //                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger VoucherApiAction : Unknown Error"), _TraceCategory);
        //        //            }

        //        //        }
        //        //        else
        //        //        {
        //        //            TxtErrorHeader = Lbl_Info;
        //        //            if (res.Message != null)
        //        //            {
        //        //                TxtErrorMessage = res.Message;
        //        //                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[rror] Trigger VoucherApiAction : {0}", res.Message.ToString()), _TraceCategory);
        //        //            }
        //        //            else
        //        //            {
        //        //                TxtErrorMessage = Lbl_ErrorTryAgain2;
        //        //                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger VoucherApiAction : Unknown Error"), _TraceCategory);
        //        //            }
        //        //        }

        //        //    }
        //        //    else
        //        //    {
        //        //        TxtErrorHeader = Lbl_Error;
        //        //        TxtErrorMessage = Lbl_ErrorTryAgain2;
        //        //        VoucherPopupVisibility = Visibility.Collapsed;
        //        //        WarningMessageBoxVisibility = BackgroundVisibility = Visibility.Visible;
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    TxtErrorHeader = Lbl_Error;
        //        //    TxtErrorMessage = Lbl_ErrorTryAgain;
        //        //    VoucherPopupVisibility = Visibility.Collapsed;
        //        //    WarningMessageBoxVisibility = BackgroundVisibility = Visibility.Visible;

        //        //    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger VoucherApiAction : Fail to get detail from API"), _TraceCategory);
        //        //}

        //        GeneralVar.MainWindowVM.CalculateTotalItem();

        //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger VoucherApiAction Done ..."), _TraceCategory);
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger VoucherApiAction : {0}", ex.ToString()), _TraceCategory);
        //    }
        //    return success;
        //}

        public bool GetLastestOrderList()
        {
            bool success = false;
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger GetLastestOrderList Starting ..."), _TraceCategory);

                Order = new List<Xilnex.Request.SalesItem>();

                CartList.ToList().ForEach(x =>
                {
                    int salesItem = x.itemId;
                    List<Xilnex.Request.Modifier> modifier = new List<Xilnex.Request.Modifier>();

                    if (x.dynamicmodifiers.SelectMany(modifierzz => modifierzz.modifiers).Count() > 0)
                    {
                        x.dynamicmodifiers.SelectMany(modifierss => modifierss.modifiers).ToList().ForEach(z =>
                        {
                            if (z.selections.Count > 0)
                            {
                                z.selections.Where(f => f.IsCheck).ToList().ForEach(r =>
                                {
                                    Xilnex.Request.Modifier tempMod = new Xilnex.Request.Modifier
                                    {
                                        Subtotal = r.DOUBLE_Sale_Price,
                                        SalesItemId = salesItem,
                                        ItemId = r.itemId,
                                        ItemCode = r.itemCode,
                                        Quantity = 1,
                                        ShippedQuantity = 1,
                                        Price = r.DOUBLE_Sale_Price,
                                        DiscountPercentage = 0,
                                        DiscountAmount = 0,
                                        IsPrint = false,
                                        SubTotal = r.DOUBLE_Sale_Price,
                                        MgstTaxAmount = r.DOUBLE_Sale_Price * 0.06,
                                        TotalTaxAmount = r.DOUBLE_Sale_Price * 0.06,
                                        IsInclusiveMgst = true,
                                        IsServiceItem = false,
                                        AdditionalTaxPercentage1 = 0,
                                        AdditionalTaxPercentage2 = 0,
                                        AdditionalTaxAmount1 = 0,
                                        AdditionalTaxAmount2 = 0,
                                        IsVoucherItem = false,
                                        IsPromoDiscountItem = false
                                    };

                                    modifier.Add(tempMod);
                                });
                            }
                        });
                    }

                    Order.Add(new Xilnex.Request.SalesItem(
                                x.itemId,
                                x.itemId,
                                x.itemCode,
                                x.ItemCurrentQty,
                                x.ItemCurrentQty,
                                x.price,
                                0,
                                0,
                                false,
                                x.ItemTotalPrice,
                                x.price * 0.06,
                                x.ItemTotalPrice * 0.06,
                                true,
                                true,
                                0,
                                0,
                                0,
                                0,
                                false,
                                false,
                                modifier
                                ));
                });

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger GetLastestOrderList Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger GetLastestOrderList : {0}", ex.ToString()), _TraceCategory);
            }
            return success;
        }

        public void CheckPromoCode()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger CheckPromoCode Starting ..."), _TraceCategory);

                Thread thCheckPromo = new Thread(() =>
                {
                    try
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger thCheckPromo Starting ..."), _TraceCategory);

                        GeneralVar.MainWindowVM.ShowLoading = GeneralVar.MainWindowVM.BackgroundVisibility = Visibility.Visible;

                        if (!String.IsNullOrEmpty(VoucherCode))
                        {
                            //if (TempVoucherList.Where(x => x.Code.ToUpper() == VoucherCode.ToUpper()).ToList().Count() > 0)
                            //{
                            //    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Promo Code found ..."), _TraceCategory);

                            //    GetLastestOrderList();

                            //    if (VoucherApiAction(true, string.Empty, VoucherCode.ToUpper(), eatMethod, GeneralVar.BranchId, Voucher, Order, CustomerID, AccessToken, GeneralVar.ComponentId, GeneralVar.ComponentUniqueId))
                            //    {
                            //        ShowLoading = VoucherPopupVisibility = BackgroundVisibility = Visibility.Collapsed;
                            //    }
                            //    else
                            //    {
                            //        VoucherPopupVisibility = Visibility.Collapsed;
                            //        WarningMessageBoxVisibility = BackgroundVisibility = Visibility.Visible;
                            //    }
                            //}
                            //else
                            //{
                            //    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Promo Code not found ..."), _TraceCategory);
                            //    TxtErrorHeader = Lbl_Error;
                            //    TxtErrorMessage = Lbl_InvalidPromoCode;
                            //    VoucherPopupVisibility = Visibility.Collapsed;
                            //    WarningMessageBoxVisibility = BackgroundVisibility = Visibility.Visible;
                            //}


                            VoucherCode = string.Empty;
                        }
                        else
                        {
                            TxtErrorHeader = Lbl_Error;
                            TxtErrorMessage = Lbl_EnterPromoCode;
                            VoucherPopupVisibility = Visibility.Collapsed;
                            WarningMessageBoxVisibility = BackgroundVisibility = Visibility.Visible;

                        }

                        ShowLoading = Visibility.Collapsed;


                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger thCheckPromo Done ..."), _TraceCategory);

                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger thCheckPromo : {0}", ex.ToString()), _TraceCategory);
                    }
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger thCheckPromo Starting ..."), _TraceCategory);
                });
                thCheckPromo.Start();
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger CheckPromoCode Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger CheckPromoCode : {0}", ex.ToString()), _TraceCategory);
            }
        }

        //public bool VoucherCheck()
        //{
        //    bool success = false;
        //    try
        //    {
        //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger VoucherCheck Starting ..."), _TraceCategory);

        //        GetLastestOrderList();
        //        CalculateTotalItem();

        //        orderRequest.payment = new Order.OrderPayment(19, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, orderRequest.paymentTotal, string.Empty);

        //        //ApiModel.AutoReviewOrder.Request req = new ApiModel.AutoReviewOrder.Request(CustomerID, AccessToken, orderRequest, GeneralVar.ComponentId, GeneralVar.ComponentUniqueId);

        //        //if (_ApiFunc.AutoReviewOrder(req, out ApiModel.AutoReviewOrder.Response res))
        //        //{
        //        //    if (res.Code.Equals("00"))
        //        //    {
        //        //        success = true;
        //        //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger VoucherCheck : Voucher remain the same"), _TraceCategory);

        //        //        if (res.Message != null)
        //        //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger VoucherCheck : {0}", res.Message.ToString()), _TraceCategory);
        //        //    }
        //        //    else if (res.Code.Equals("01"))
        //        //    {
        //        //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Info] Trigger VoucherCheck : Voucher have changes"), _TraceCategory);
        //        //        if (res.PromotionToRemove != null)
        //        //        {
        //        //            if (res.PromotionToRemove.Count() > 0)
        //        //            {
        //        //                var voucherToRemove = res.PromotionToRemove.ToList();

        //        //                foreach (var voucher in voucherToRemove)
        //        //                {
        //        //                    //VoucherList.Where(x => int.Parse(x.RedeemTxId) == voucher.rewardTxId && int.Parse(x.RedeemId) == voucher.rewardSuperId).Select(y => y.IsCheck = false).ToList();
        //        //                    Voucher.Remove(Voucher.Single(x => x.rewardTxId == Convert.ToInt32(voucher.rewardTxId) && x.rewardSuperId == voucher.rewardSuperId));
        //        //                }

        //        //                CalculateTotalItem();
        //        //                TxtErrorHeader = Lbl_Info;
        //        //                TxtErrorMessage = Lbl_CheckVoucher;
        //        //                WarningMessageBoxVisibility = Visibility.Visible;
        //        //            }
        //        //        }
        //        //        success = true;
        //        //        if (res.Message != null)
        //        //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Info] Trigger VoucherCheck : {0}", res.Message.ToString()), _TraceCategory);
        //        //        else
        //        //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Info] Trigger VoucherCheck : Menu have some changes"), _TraceCategory);
        //        //    }
        //        //    else if (res.Code.Equals("99"))
        //        //    {
        //        //        if (res.Message != null)
        //        //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger VoucherCheck : {0}", res.Message.ToString()), _TraceCategory);
        //        //        else
        //        //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger VoucherCheck : Unknown Error"), _TraceCategory);
        //        //    }
        //        //    else
        //        //    {
        //        //        if (res.Message != null)
        //        //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger VoucherCheck : {0}", res.Message.ToString()), _TraceCategory);
        //        //        else
        //        //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger VoucherCheck : Unknown Error"), _TraceCategory);
        //        //    }
        //        //}

        //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger VoucherCheck Done ..."), _TraceCategory);
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger VoucherCheck : {0}", ex.ToString()), _TraceCategory);
        //    }
        //    return success;
        //}
        #endregion
        #endregion

        #region EditItem
        #region Property
        #endregion

        #region Command
        private ICommand _BtnDoneChange;
        public ICommand BtnDoneChange
        {
            get
            {
                if (_BtnDoneChange == null)
                    _BtnDoneChange = new RelayCommand(DoneChangeAction);
                return _BtnDoneChange;
            }
        }
        #endregion

        #region Function
        public void DoneChangeAction()
        {
            try
            {
                ShowLoading = Visibility.Visible;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger DoneChangeAction Starting ..."), _TraceCategory);

                if (index != -1)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        int previousItemQty = TempEditItem.Select(x => x.ItemCurrentQty).FirstOrDefault();
                        CartList.RemoveAt(index);

                        if (CheckSimilarity(null, out int itemueue))
                        {
                            CartList.Where(x => x.cartMenuNo == itemueue).Select(z => z.ItemCurrentQty += previousItemQty).ToList();
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Similarity - true"), _TraceCategory);
                        }
                        else
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Similarity - false"), _TraceCategory);
                            ObservableCollection<CartModel.Modifier> ModifierDetails = new ObservableCollection<CartModel.Modifier>();


                            TempEditItem.SelectMany(x => x.dynamicmodifiers).SelectMany(y => y.modifiers).ToList().ForEach(a =>
                              {
                                  var checkedModifiers = a.selections.Where(b => b.IsCheck).ToList();

                                  if (checkedModifiers.Any())
                                  {
                                      ModifierDetails.Add(new CartModel.Modifier
                                      {
                                          type = a.type,
                                          minSelection = a.minSelection,
                                          maxSelection = a.maxSelection,
                                          groupId = a.groupId,
                                          groupName = a.groupName,
                                          selections = checkedModifiers.Select(c => new CartModel.Selection
                                          {
                                              name = c.name,
                                              defaultSelected = c.defaultSelected,
                                              itemId = c.itemId,
                                              itemCode = c.itemCode,
                                              itemType = c.itemType,
                                              price = c.price,
                                              DOUBLE_Sale_Price = c.DOUBLE_Sale_Price,
                                              DOUBLE_Employee_Price = c.DOUBLE_Employee_Price,
                                              DOUBLE_Wholesale_Price = c.DOUBLE_Wholesale_Price,
                                              DOUBLE_Custom_Price = c.DOUBLE_Custom_Price,
                                              DOUBLE_Manufacturer_Suggested_Retail_Price = c.DOUBLE_Manufacturer_Suggested_Retail_Price,
                                              DOUBLE_Web_Price = c.DOUBLE_Web_Price,
                                              DOUBLE_Web_Dealer_Price = c.DOUBLE_Web_Dealer_Price,
                                              imageUrlList = c.imageUrlList,
                                              binaryImageUrl = c.binaryImageUrl,
                                              imageUrl = c.imageUrl,
                                              description = c.description,
                                              customField1 = c.customField1,
                                              customField2 = c.customField2,
                                              customField3 = c.customField3,
                                              customField4 = c.customField4,
                                              customField5 = c.customField5,
                                              customField6 = c.customField6,
                                              customField7 = c.customField7,
                                              customField8 = c.customField8,
                                              customField9 = c.customField9,
                                              customField10 = c.customField10,
                                              customField11 = c.customField11,
                                              customField12 = c.customField12,
                                              customField13 = c.customField13,
                                              customField14 = c.customField14,
                                              customField15 = c.customField15,
                                              customField16 = c.customField16,
                                              customField17 = c.customField17,
                                              customField18 = c.customField18,
                                              size = c.size,
                                              boolOpenPrice = c.boolOpenPrice,
                                              stockType = c.stockType,
                                              IsAssortment = c.IsAssortment,
                                              HasStock = c.HasStock,
                                              AllowToSell = c.AllowToSell,
                                              IsActive = c.IsActive,
                                              groupId = c.groupId,
                                              minSelection = c.minSelection,
                                              maxSelection = c.maxSelection,
                                              parentItemId = c.parentItemId,
                                              MenuImage = c.MenuImage,
                                              MenuImagePath = c.MenuImagePath,
                                              IsCheck = c.IsCheck,
                                              IsEnable = c.IsEnable

                                          }).ToList()
                                      });
                                  }
                              });
                            TempEditItem.Select(x => x.ModifierList = ModifierDetails).ToList();
                            CartList.Insert(index, TempEditItem.Single());
                        }

                        CalculateTotalItem();
                        SetStage(eStage.OrderSummary);
                        BackgroundVisibility = DetailsViewVisbility = Visibility.Collapsed;
                    }));

                }
                ShowLoading = Visibility.Collapsed;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger DoneChangeAction Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger DoneChangeAction : {0}", ex.ToString()), _TraceCategory);
            }
        }
        #endregion

        #endregion

        #region Payment Selection Page

        #region Field
        bool isStopAccepting = false;
        string ccStatusCode = string.Empty;
        Sales cardBizLastSalesRequest;
        Sales_Resp cardBizLastSalesResponse = new Sales_Resp();
        Sales_Resp printCardResponse = new Sales_Resp();
        bool logEnable = false;
        KeepAlive _CardBizLastAliveRequest;
        KeepAlive_Resp _CardBizLastAliveResponse = new KeepAlive_Resp();
        string _CardBizCardProcessState = string.Empty;

        ApiModel.PostOrderResponse fnbRes = null;

        ApiModel.InitialOrderResponse initialOrderResponse = null;
        ApiModel.InitialOrderRequest intialOrderRequest;

        RazerPayIntegration razerPay = new RazerPayIntegration();
        RazerPaymentResponse printRazerResponse = null;

        RazerPaymentResponse qrRazerResponse = null;
        bool isStopQRPayment = false;

        eRazerChannel eWalletSelected = eRazerChannel.Any;

        string _PaymentCategoryId = string.Empty;
        public string _PaymentCategoryName = string.Empty;

        #endregion

        #region Property

        string initialOrderNumber = string.Empty;
        string _Lbl_QRTimeRemaining;
        public string Lbl_QRTimeRemaining
        {
            get
            {
                return _Lbl_QRTimeRemaining;
            }
            set
            {
                _Lbl_QRTimeRemaining = value;
                OnPropertyChanged("Lbl_QRTimeRemaining");
            }
        }

        Visibility _ShowCardPayment = Visibility.Collapsed;
        public Visibility ShowCardPayment
        {
            get { return _ShowCardPayment; }
            set
            {
                _ShowCardPayment = value;
                OnPropertyChanged("ShowCardPayment");
            }
        }

        Visibility _ShowEWallet = Visibility.Collapsed;
        public Visibility ShowEWallet
        {
            get { return _ShowEWallet; }
            set
            {
                _ShowEWallet = value;
                OnPropertyChanged("ShowEWallet");
            }
        }

        Visibility _ShowPaymentProcessing = Visibility.Collapsed;
        public Visibility ShowPaymentProcessing
        {
            get { return _ShowPaymentProcessing; }
            set
            {
                _ShowPaymentProcessing = value;
                OnPropertyChanged("ShowPaymentProcessing");
            }
        }

        Visibility _ShowSendingKitchen = Visibility.Collapsed;
        public Visibility ShowSendingKitchen
        {
            get { return _ShowSendingKitchen; }
            set
            {
                _ShowSendingKitchen = value;
                OnPropertyChanged("ShowSendingKitchen");
            }
        }

        Visibility _ShowFailPayment = Visibility.Collapsed;
        public Visibility ShowFailPayment
        {
            get { return _ShowFailPayment; }
            set
            {
                _ShowFailPayment = value;
                OnPropertyChanged("ShowFailPayment");
            }
        }

        Visibility _ShowQRPayment = Visibility.Collapsed;
        public Visibility ShowQRPayment
        {
            get { return _ShowQRPayment; }
            set
            {
                _ShowQRPayment = value;
                OnPropertyChanged(nameof(ShowQRPayment));
                //OnPropertyChanged("ShowQRPayment");
            }
        }

        BitmapImage _QRPaymentImage;
        public BitmapImage QRPaymentImage
        {
            get { return _QRPaymentImage; }
            set
            {
                _QRPaymentImage = value;
                OnPropertyChanged("QRPaymentImage");
            }

        }

        #endregion

        #region Command


        private ICommand _BtnCloseQRPayment;
        public ICommand BtnCloseQRPayment
        {
            get
            {
                if (_BtnCloseQRPayment == null)
                    _BtnCloseQRPayment = new RelayCommand(CloseQRPayment);
                return _BtnCloseQRPayment;
            }
        }

        private ICommand _BtnFailPayment;
        public ICommand BtnFailPayment
        {
            get
            {
                if (_BtnFailPayment == null)
                    _BtnFailPayment = new RelayCommand(FailPayment);
                return _BtnFailPayment;
            }
        }

        private ICommand _BtnReturnSummary;
        public ICommand BtnReturnSummary
        {
            get
            {
                if (_BtnReturnSummary == null)
                    _BtnReturnSummary = new RelayCommand(ReturnOrderSummary);
                return _BtnReturnSummary;
            }
        }

        private ICommand _BtnPayment;
        public ICommand BtnPayment
        {
            get
            {
                if (_BtnPayment == null)
                    _BtnPayment = new RelayCommand<string>(Payment);
                return _BtnPayment;
            }
        }

        private ICommand _BtnCloseEwallet;
        public ICommand BtnCloseEwallet
        {
            get
            {
                if (_BtnCloseEwallet == null)
                    _BtnCloseEwallet = new RelayCommand(CloseEwallet);
                return _BtnCloseEwallet;
            }
        }

        #endregion

        #region Function
        public void ReturnOrderSummary()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger ReturnOrderSummary Starting ..."), _TraceCategory);

                BackgroundVisibility = DetailsViewVisbility = Visibility.Collapsed;
                SetStage(eStage.OrderSummary);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger ReturnOrderSummary Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger ReturnOrderSummary : {0}", ex.ToString()), _TraceCategory);
            }
        }

        public void FailPayment()
        {
            try
            {
                ShowFailPayment = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] FailPayment = {0}", ex.ToString()), _TraceCategory);
            }
        }

        public void CloseEwallet()
        {
            try
            {
                ShowEWallet = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] FailPayment = {0}", ex.ToString()), _TraceCategory);
            }
        }

        Dictionary<string, eRazerChannel> paymentMethodMapping = new Dictionary<string, eRazerChannel>
        {
            { "DUITNOW", eRazerChannel.DuitNowQR },
            { "UNION", eRazerChannel.UnionPay },
            { "ALIPAY", eRazerChannel.AliPay },
            { "ALIPAY+", eRazerChannel.AliPayPlus },
            { "WECHAT", eRazerChannel.WechatPayCN }
        };

        public bool InitialOrder(string paymentMethod)
        {
            bool success = false;

            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger InitialOrder Starting ..."), _TraceCategory);
                CalculateTotalItem();

                if(!_ApiFunc.InitOrder(orderRequest, out initialOrderResponse))
                {
                    throw new Exception("Initial Order Fail");
                }
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger InitialOrder Done ..."), _TraceCategory);
                success = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger InitialOrder Error - {0}", ex.ToString()), _TraceCategory);
            }
            return success;
        }

        public void Payment(string paymentMethod)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger Payment Starting ..."), _TraceCategory);
                StopCountDown();
                ShowLoading = Visibility.Visible;

                if (string.IsNullOrEmpty(TableNo))
                {
                    orderNum = string.Format("{0}-{1}", GeneralVar.ComponentCode, DateTime.Now.ToString("ddMMyyHHmmss"));
                    orderRequest.OrderDetails.OrderNo = orderNum;
                    
                }
                else
                    orderRequest.OrderDetails.OrderNo = "TT"+TableNo;

                Thread payment = new Thread(() =>
                {
                    try
                    {
                        QRCodeReceipt = string.Empty;
                        bool initialOrderResult = InitialOrder(paymentMethod);

                        if (initialOrderResponse != null)
                        {
                            if (!string.IsNullOrEmpty(initialOrderResponse.OrderNumber))
                            {
                                initialOrderNumber = initialOrderResponse.OrderNumber;
                                orderRequest.OrderDetails.OrderNo = initialOrderNumber;
                            }
                            if (!string.IsNullOrEmpty(initialOrderResponse.ReferenceNo))
                            {
                                referenceNo = initialOrderResponse.ReferenceNo;
                                QRCodeReceipt = initialOrderResponse.ReferenceNo;
                            }
                        }

                        ShowLoading = Visibility.Collapsed;

                        if (!initialOrderResult)
                        {
                            TxtErrorHeader = Lbl_Error;
                            TxtErrorMessage = Lbl_ErrorAssist;
                            WarningMessageBoxVisibility = Visibility.Visible;
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, "Initial Order failed.", _TraceCategory);
                            return;
                        }

                        switch (paymentMethod)
                        {
                            case "KCC":
                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger Payment - Card"), _TraceCategory);
                                StartAcceptingCard();
                                break;

                            case "EW":
                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger Payment - E-Wallet"), _TraceCategory);
                                ShowEWallet = Visibility.Visible;
                                break;

                            case "DUITNOW":
                            case "UNION":
                            case "ALIPAY":
                            case "ALIPAY+":
                            case "WECHAT":
                                if (paymentMethodMapping.TryGetValue(paymentMethod, out eRazerChannel selectedChannel))
                                {
                                    ShowLoading = Visibility.Visible;
                                    eWalletSelected = selectedChannel;
                                    StartPreCreateQREWallet();
                                }
                                break;

                            case "CASH":
                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger Payment - Cash"), _TraceCategory);
                                
                                    if (PostOrder("Test123"))
                                    {

                                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                        {
                                            SetStage(eStage.FinalPage);
                                        }));

                                        Sales_Resp cardResponse = null;
                                        if (!GeneralVar.DocumentPrint.Print_CardReceipt(true, TotalAmount, KioskId, fnbRes.OrderNumber.ToString(), dineMethod, CartList, cardResponse, Convert.ToDecimal(AnWTotalAmount), AnWTax, AnWRounding, AnWVoucherAmount, AnWGiftVoucher, StoreName, IsDelaySentOrder))
                                        {
                                            _WaitDone.Reset();
                                            _WaitDone.WaitOne(10000);

                                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                            {
                                                SetStage(eStage.Offline);
                                            }));
                                        }
                                        else
                                        {
                                            _WaitDone.Reset();
                                            _WaitDone.WaitOne(10000);

                                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                            {
                                                SetStage(eStage.Home);
                                            }));
                                        }
                                        //Thread.Sleep(2000);
                                        //    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                        //    {
                                        //        SetStage(eStage.FinalPage);
                                        //    }));

                                        //    _WaitDone.Reset();
                                        //    _WaitDone.WaitOne(10000);

                                        //    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                        //    {
                                        //        SetStage(eStage.Home);
                                        //    }));
                                    }
                                    else
                                    {

                                    }
                                break;
                                

                            default:
                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger Payment - Default"), _TraceCategory);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Thread payment : {0}", ex.ToString()), _TraceCategory);
                    }
                });
                payment.Start();

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger Payment Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("{Error] Trigger Payment : {0}", ex.ToString()), _TraceCategory);
            }
        }

        private void StartAcceptingEWallet(string qrCodeData) //check avoid double scan
        {
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("StartAcceptingEWallet Starting..."), _TraceCategory);

            try
            {
                string QRData = qrCodeData.Trim();
                RazerPayIntegration.eRazerChannel channel = RazerPayIntegration.eRazerChannel.Any;
                RazerPaymentResponse payResponse = null;

                //if (initialOrderResponse != null)
                //{
                //    _ReferenceNo = initialOrderResponse.OrderId;
                //}
                //else
                //    _ReferenceNo = DateTime.Now.ToString("dMMyyyyHHMMff");

                if (GeneralVar.TestingMode)
                    payResponse = razerPay.RazerPayment(QRData, _ReferenceNo + "-T", 0.10m, channel, GeneralVar.ComponentCode);
                else
                    payResponse = razerPay.RazerPayment(QRData, _ReferenceNo + "-S", Convert.ToDecimal(AnWTotalAmount), channel, GeneralVar.ComponentCode);

                ShowEWallet = Visibility.Collapsed;
                ShowPaymentProcessing = Visibility.Visible;

                if (payResponse != null)
                {
                    if (printRazerResponse != null)
                        printRazerResponse = null;
                    printRazerResponse = new RazerPaymentResponse();
                    printRazerResponse = payResponse;

                    Trace.WriteLine(razerPay.AllProps(payResponse));
                    Trace.WriteLine(" StatusCode      : " + payResponse.statusCode);
                    Trace.WriteLine(" ErrorCode     : " + payResponse.errorCode);
                    Trace.WriteLine(" MolTransactionId     : " + payResponse.molTransactionId);
                    Trace.WriteLine(" ReferenceId       : " + payResponse.referenceId);
                    Trace.WriteLine(" ChannelId       : " + payResponse.channelId);
                    Trace.WriteLine(" Amount      : " + payResponse.amount.ToString("#.00"));
                    Trace.WriteLine(" QRAuthorizationCode     : " + payResponse.authorizationCode);
                    Trace.WriteLine(" CurrencyCode   : " + payResponse.currencyCode);
                    Trace.WriteLine(" PayerId   : " + payResponse.payerId);

                    if (payResponse.statusCode != "00" || !string.IsNullOrEmpty(payResponse.errorCode))
                    {
                        if (payResponse.statusCode == "01" || payResponse.statusCode == "11" || payResponse.statusCode == "99") // go inquiry
                        {
                            RazerPaymentResponse retryResponse = null;

                            if (GeneralVar.TestingMode)
                                retryResponse = razerPay.RazerInquiry(_ReferenceNo + "-T");
                            else
                                retryResponse = razerPay.RazerInquiry(_ReferenceNo + "-S");

                            if (retryResponse != null)
                            {
                                if (printRazerResponse != null)
                                    printRazerResponse = null;
                                printRazerResponse = new RazerPaymentResponse();
                                printRazerResponse = retryResponse;

                                Trace.WriteLine(razerPay.AllProps(retryResponse));
                                Trace.WriteLine(" StatusCode      : " + retryResponse.statusCode);
                                Trace.WriteLine(" ErrorCode     : " + retryResponse.errorCode);
                                Trace.WriteLine(" MolTransactionId     : " + retryResponse.molTransactionId);
                                Trace.WriteLine(" ReferenceId       : " + retryResponse.referenceId);
                                Trace.WriteLine(" ChannelId       : " + retryResponse.channelId);
                                Trace.WriteLine(" Amount      : " + retryResponse.amount.ToString("#.00"));
                                Trace.WriteLine(" QRAuthorizationCode     : " + retryResponse.authorizationCode);
                                Trace.WriteLine(" CurrencyCode   : " + retryResponse.currencyCode);
                                Trace.WriteLine(" PayerId   : " + retryResponse.payerId);

                                if (retryResponse.statusCode != "00" || !string.IsNullOrEmpty(retryResponse.errorCode))
                                {
                                    throw new Exception("Failed Payment");
                                }
                                else
                                {
                                    PrepareSuccessEwalletTrx(retryResponse);
                                }
                            }
                            else
                                throw new Exception("retryResponse is null");
                        }
                        else
                            throw new Exception("Failed Payment");
                    }
                    else
                    {
                        //Success payment
                        PrepareSuccessEwalletTrx(payResponse);
                    }
                }
                else
                {
                    throw new Exception("payResponse is null");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] StartAcceptingEWallet = {0}", ex.ToString()), _TraceCategory);

                ShowEWallet = Visibility.Collapsed;
                ShowPaymentProcessing = Visibility.Collapsed;
                ShowFailPayment = Visibility.Visible;
                SetStage(eStage.PaymentMethodSelection);
            }
        }

        private void StartAcceptingCard()
        {
            try
            {
                isStopAccepting = false;
                ccStatusCode = string.Empty;

                //ShowCardPayment = Visibility.Visible;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    SetStage(eStage.PaymentByCard);
                }));

                if (cardBizLastSalesRequest != null)
                    cardBizLastSalesRequest = null;
                cardBizLastSalesRequest = new Sales(GeneralVar.CC_PortName, true);

                cardBizLastSalesResponse = new Sales_Resp();

                if (GeneralVar.TestingMode)
                    cardBizLastSalesResponse = cardBizLastSalesRequest.SendRequest("", "", Convert.ToDouble(0.01m), 0.00, "01", "0", "", "0", 90);
                else
                    cardBizLastSalesResponse = cardBizLastSalesRequest.SendRequest("", "", Convert.ToDouble(AnWTotalAmount), 0.00, "01", "0", "", "0", 90);

                //ShowCardPayment = Visibility.Collapsed;
                ShowPaymentProcessing = Visibility.Visible;

                if (printCardResponse != null)
                    printCardResponse = null;
                printCardResponse = cardBizLastSalesResponse;

                //Logs
                #region Trace

                string CardBizTransactionLog = string.Format(@"
CreditCardApprovedDetails ****************************************
CardBiz Status: {0} - {1}
CardBiz Response: {2} - {3}
PayAccId: {4}
Amount: {5}
CashBack Amount: {6}
Approval Code: {7}
Invoice No: {8}
Trace No: {9}
Encrypted Card No: {10}
Card No: {11}
Card Label: {12}
Card Holder Name: {13}
Expiry Date: {14}
Card Issue Date: {15}
Member Expiry Date: {16}
TID: {17}
Merchant No: {18} - {19}
Batch No: {20}
RREF No: {21}
AID: {22}
Application Profile EMV: {23}
CID EMV: {24}
TC: {25}					
Transaction Status Information: {26}
Terminal Verification Result: {27}
Card Entry: {28}
Transaction Date: {29}
Transaction Time: {30}					
Account Balance: {31}
Card Issuer Name: {32}				
END - CreditCardApprovedDetails ****************************************",
                cardBizLastSalesResponse.StatusCode,
                string.IsNullOrEmpty(cardBizLastSalesResponse.StatusMessage) ? string.Empty : cardBizLastSalesResponse.StatusMessage,
                string.IsNullOrEmpty(cardBizLastSalesResponse.ResponseCode) ? string.Empty : cardBizLastSalesResponse.ResponseCode,
                string.IsNullOrEmpty(cardBizLastSalesResponse.ResponseText) ? string.Empty : cardBizLastSalesResponse.ResponseText,
                string.IsNullOrEmpty(cardBizLastSalesResponse.PayAccId) ? string.Empty : cardBizLastSalesResponse.PayAccId,
                string.IsNullOrEmpty(cardBizLastSalesResponse.Amount) ? string.Empty : cardBizLastSalesResponse.Amount,
                string.IsNullOrEmpty(cardBizLastSalesResponse.CashbackAmount) ? string.Empty : cardBizLastSalesResponse.CashbackAmount,
                string.IsNullOrEmpty(cardBizLastSalesResponse.ApprovalCode) ? string.Empty : cardBizLastSalesResponse.ApprovalCode,
                string.IsNullOrEmpty(cardBizLastSalesResponse.InvoiceNumber) ? string.Empty : cardBizLastSalesResponse.InvoiceNumber,
                string.IsNullOrEmpty(cardBizLastSalesResponse.TraceNumber) ? string.Empty : cardBizLastSalesResponse.TraceNumber,
                string.IsNullOrEmpty(cardBizLastSalesResponse.EncryptedCardNo) ? string.Empty : cardBizLastSalesResponse.EncryptedCardNo,
                string.IsNullOrEmpty(cardBizLastSalesResponse.CardNo) ? string.Empty : cardBizLastSalesResponse.CardNo,
                string.IsNullOrEmpty(cardBizLastSalesResponse.CardLabel) ? string.Empty : cardBizLastSalesResponse.CardLabel,
                string.IsNullOrEmpty(cardBizLastSalesResponse.CardholderName) ? string.Empty : cardBizLastSalesResponse.CardholderName,
                string.IsNullOrEmpty(cardBizLastSalesResponse.ExpiryDate) ? string.Empty : cardBizLastSalesResponse.ExpiryDate,
                string.IsNullOrEmpty(cardBizLastSalesResponse.CardIssueDate) ? string.Empty : cardBizLastSalesResponse.CardIssueDate,
                string.IsNullOrEmpty(cardBizLastSalesResponse.MemberExpiryDate) ? string.Empty : cardBizLastSalesResponse.MemberExpiryDate,
                string.IsNullOrEmpty(cardBizLastSalesResponse.TerminalId) ? string.Empty : cardBizLastSalesResponse.TerminalId,
                string.IsNullOrEmpty(cardBizLastSalesResponse.MerchantNo) ? string.Empty : cardBizLastSalesResponse.MerchantNo,
                string.IsNullOrEmpty(cardBizLastSalesResponse.MerchantName) ? string.Empty : cardBizLastSalesResponse.MerchantName,
                string.IsNullOrEmpty(cardBizLastSalesResponse.BatchNo) ? string.Empty : cardBizLastSalesResponse.BatchNo,
                string.IsNullOrEmpty(cardBizLastSalesResponse.RetrievalReferenceNo) ? string.Empty : cardBizLastSalesResponse.RetrievalReferenceNo,
                string.IsNullOrEmpty(cardBizLastSalesResponse.AID_EMV) ? string.Empty : cardBizLastSalesResponse.AID_EMV,
                string.IsNullOrEmpty(cardBizLastSalesResponse.ApplicationProfile_EMV) ? string.Empty : cardBizLastSalesResponse.ApplicationProfile_EMV,
                string.IsNullOrEmpty(cardBizLastSalesResponse.CID_EMV) ? string.Empty : cardBizLastSalesResponse.CID_EMV,
                string.IsNullOrEmpty(cardBizLastSalesResponse.ApplicationCryptogram_EMV) ? string.Empty : cardBizLastSalesResponse.ApplicationCryptogram_EMV,
                string.IsNullOrEmpty(cardBizLastSalesResponse.TSI_EMV) ? string.Empty : cardBizLastSalesResponse.TSI_EMV,
                string.IsNullOrEmpty(cardBizLastSalesResponse.TVR_EMV) ? string.Empty : cardBizLastSalesResponse.TVR_EMV,
                string.IsNullOrEmpty(cardBizLastSalesResponse.CardEntryMode) ? string.Empty : cardBizLastSalesResponse.CardEntryMode,
                string.IsNullOrEmpty(cardBizLastSalesResponse.TransactionDate) ? string.Empty : cardBizLastSalesResponse.TransactionDate,
                string.IsNullOrEmpty(cardBizLastSalesResponse.TransactionTime) ? string.Empty : cardBizLastSalesResponse.TransactionTime,
                string.IsNullOrEmpty(cardBizLastSalesResponse.AccountBalance) ? string.Empty : cardBizLastSalesResponse.AccountBalance,
                string.IsNullOrEmpty(cardBizLastSalesResponse.CardIssuerName) ? string.Empty : cardBizLastSalesResponse.CardIssuerName);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("CardBizTransactionResponse:- {0}", CardBizTransactionLog), _TraceCategory);

                #endregion

                //ShowLoading

                do
                {
                    _CardBizLastAliveRequest = new KeepAlive(GeneralVar.CC_PortName, logEnable);
                    _CardBizLastAliveResponse = _CardBizLastAliveRequest.SendRequest();

                    if (_CardBizLastAliveResponse.ResponseCode == "CP")
                    {
                        System.Threading.Thread.Sleep(1000);
                    }

                } while (_CardBizLastAliveResponse.ResponseCode == "CP");

                if (cardBizLastSalesResponse.StatusCode == 0 && cardBizLastSalesResponse.ResponseCode == "00")
                {
                    PrepareSuccessCardTrx(printCardResponse);
                }
                else
                {
                    PrepareFailCardTrx();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] StartAcceptingCard = {0}", ex.ToString()), _TraceCategory);

                //ShowCardPayment = Visibility.Collapsed;
                ShowPaymentProcessing = Visibility.Collapsed;
                ShowFailPayment = Visibility.Visible;
                SetStage(eStage.PaymentMethodSelection);
            }
        }

        BackgroundWorker bwPreCreateQREWallet = new BackgroundWorker();
        private void StartPreCreateQREWallet()
        {
            bwPreCreateQREWallet = new BackgroundWorker();

            if (!bwPreCreateQREWallet.IsBusy)
            {
                bwPreCreateQREWallet.WorkerSupportsCancellation = true;
                bwPreCreateQREWallet.DoWork += bwPreCreateQREWallet_DoWork;
                bwPreCreateQREWallet.RunWorkerCompleted += bwPreCreateQREWallet_RunWorkerCompleted;
                bwPreCreateQREWallet.RunWorkerAsync();
            }
        }

        private BitmapImage GetImageFromUrlAsync(string url)
        {
            BitmapImage bitmapImage = new BitmapImage();

            try
            {
                using (var webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = stream;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze(); // To make it cross-thread accessible
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] GetImageFromUrlAsync = {0}", ex.ToString()), _TraceCategory);
            }

            return bitmapImage;
        }

        void bwPreCreateQREWallet_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                decimal paymentAmount = 0;
                string paymentReferenceName = null;
                int paymentTypeId = 0;
                isStopQRPayment = false;
                _ReferenceNo = string.Empty;
                _ReferenceNo = referenceNo;

                //if (initialOrderResponse != null)
                //{
                //    if (string.IsNullOrEmpty(initialOrderResponse.OrderId))
                //        _ReferenceNo = DateTime.Now.ToString("dMMyyyyHHMMff");
                //    else
                //        _ReferenceNo = initialOrderResponse.OrderId.Trim();
                //}
                //else
                //    _ReferenceNo = DateTime.Now.ToString("dMMyyyyHHMMff");

                RazerPreCreateQRResponse PreCreateQRResponse = new RazerPreCreateQRResponse();

                if (GeneralVar.TestingMode)
                    PreCreateQRResponse = razerPay.RazerPreCreateQRPayment(_ReferenceNo, 0.1m, eWalletSelected);
                else
                    PreCreateQRResponse = razerPay.RazerPreCreateQRPayment(_ReferenceNo, Convert.ToDecimal(AnWTotalAmount), eWalletSelected);

                string RazerPreCreateLog = string.Format(@"
	                PreCreateQRResponse ****************************************
	                applicationCode : {0}
	                version : {1}
	                referenceId : {2}
                    currencyCode : {3}
	                amount : {4}
	                molTransactionId : {5}
                    channelId : {6}
	                authorizationCode : {7}
	                imageUrl : {8}
                    imageUrlBig : {9}
                    imageUrlSmall : {10}
                    customImageUrl : {11}
                    statusCode : {12}
                    errorCode : {13}
					transactionDateTime : {14}
                    hashType : {15}
					signature : {16}
	                END - PreCreateQRResponse ****************************************",
                              PreCreateQRResponse.applicationCode, PreCreateQRResponse.version,
                              PreCreateQRResponse.referenceId, PreCreateQRResponse.currencyCode,
                              PreCreateQRResponse.amount, PreCreateQRResponse.molTransactionId,
                              PreCreateQRResponse.channelId, PreCreateQRResponse.authorizationCode,
                              PreCreateQRResponse.imageUrl, PreCreateQRResponse.imageUrlBig,
                              PreCreateQRResponse.imageUrlSmall, PreCreateQRResponse.customImageUrl,
                              PreCreateQRResponse.statusCode, PreCreateQRResponse.errorCode,
                              PreCreateQRResponse.transactionDateTime, PreCreateQRResponse.hashType,
                              PreCreateQRResponse.signature);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("bwPreCreateQREWallet_DoWork PreCreateQRResponse:- {0}", RazerPreCreateLog), _TraceCategory);

                if (PreCreateQRResponse.statusCode == "00")
                {
                    //QRPaymentImage = GetImageFromUrlAsync(PreCreateQRResponse.imageUrl);

                    string paymentQRImagePath = DownloadPaymentImage(PreCreateQRResponse.imageUrl, _ReferenceNo);
                    if (string.IsNullOrEmpty(paymentQRImagePath))
                    {
                        Barcode.SetBarcodeSize(240, 240);
                        MemoryStream paymentBarcode = new MemoryStream();
                        paymentBarcode = Barcode.SaveBarcodeToStream(BarcodeFormat.QRCode, PreCreateQRResponse.authorizationCode, SaveOptions.Jpg);
                        System.Drawing.Bitmap bmEpaymentBarcode = new System.Drawing.Bitmap(paymentBarcode);
                        QRPaymentImage = ToBitmapImage(bmEpaymentBarcode);
                    }
                    else
                    {
                        QRPaymentImage = FilePathToBitmapImage(paymentQRImagePath);
                    }

                    ShowLoading = Visibility.Collapsed;
                    //ShowQRPayment = Visibility.Visible;


                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        SetStage(eStage.PaymentByQR);

                        Lbl_QRTimeRemaining = "";
                        if (timerIsRunning)
                            duitNowTimer.Stop();
                        remainingTime = TimeSpan.FromMinutes(1).Add(TimeSpan.FromSeconds(30));
                        if (duitNowTimer != null)
                            duitNowTimer = null;
                        duitNowTimer = new DispatcherTimer();
                        duitNowTimer.Interval = TimeSpan.FromSeconds(1);
                        duitNowTimer.Tick += duitNowTimer_Tick;
                        duitNowTimer.Start();
                    }));

                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("bwPreCreateQREWallet_DoWork url:- {0}", PreCreateQRResponse.imageUrl), _TraceCategory);

                    DateTime requestTime = DateTime.Now;
                    double duration = 0;
                    do
                    {
                        DateTime eTime = DateTime.Now;
                        TimeSpan ts = eTime - requestTime;
                        duration = Math.Round(ts.TotalSeconds, 0);

                        if (duration > 90)
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Timeout reaached"), _TraceCategory);
                            break; // Timeout reached
                        }

                        System.Threading.Thread.Sleep(2000);

                        string MolPayChannelId = string.Empty;
                        //MolPayChannelId = qrRazerResponse.channelId;
                        qrRazerResponse = new RazerPaymentResponse();
                        qrRazerResponse = razerPay.RazerInquiry(_ReferenceNo);

                    } while ((qrRazerResponse.statusCode == "01" || qrRazerResponse.statusCode == "11") && !isStopQRPayment);

                    if (isStopQRPayment)
                    {
                        ShowLoading = Visibility.Collapsed;
                        ShowQRPayment = Visibility.Collapsed;
                        ShowFailPayment = Visibility.Visible;
                        SetStage(eStage.PaymentMethodSelection);
                    }
                    else
                    {
                        if (qrRazerResponse.statusCode == "00")
                        {
                            string MolPayLog = string.Format(@"
	                EwalletResponse ****************************************
	                applicationCode : {0}
	                version : {1}
	                referenceId : {2}
	                authorizationCodeType : {3}
	                authorizationCode : {4}
	                currencyCode : {5}
	                amount : {6}
	                molTransactionId : {7}
					payerId : {8}
					exchangeRate : {9}
					baseCurrencyCode : {10}
					baseAmount : {11}
					statusCode : {12}
					errorCode : {13}
					transactionDateTime : {14}
					channelId : {17}
					hashType : {15}
					signature : {16}
	                END - EwalletResponse ****************************************",
                              qrRazerResponse.applicationCode, qrRazerResponse.version,
                              qrRazerResponse.referenceId, qrRazerResponse.authorizationCodeType,
                              qrRazerResponse.authorizationCode, qrRazerResponse.currencyCode,
                              qrRazerResponse.amount, qrRazerResponse.molTransactionId,
                              qrRazerResponse.payerId, qrRazerResponse.exchangeRate,
                              qrRazerResponse.baseCurrencyCode, qrRazerResponse.baseAmount,
                              qrRazerResponse.statusCode, qrRazerResponse.errorCode,
                              qrRazerResponse.transactionDateTime, qrRazerResponse.hashType,
                              qrRazerResponse.signature,
                              qrRazerResponse.channelId);

                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("bwPreCreateQREWallet_DoWork EwalletResponse:- {0}", MolPayLog), _TraceCategory);

                            PrepareSuccessQRTrx(qrRazerResponse);
                        }
                        else
                        {
                            PrepareFailEwalletTrx();
                        }
                    }
                }
                else
                {
                    PrepareFailEwalletTrx();
                }
            }
            catch (Exception ex)
            {
                ShowLoading = Visibility.Collapsed;
                ShowQRPayment = Visibility.Collapsed;
                ShowFailPayment = Visibility.Visible;
                SetStage(eStage.PaymentMethodSelection);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] bwPreCreateQREWallet_DoWork:{0}", ex.ToString()), _TraceCategory);
            }
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("bwPreCreateQREWallet_DoWork Completed."), _TraceCategory);
        }

        void bwPreCreateQREWallet_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                bwPreCreateQREWallet.CancelAsync();
                bwPreCreateQREWallet.Dispose();
                bwPreCreateQREWallet = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] bwPreCreateQREWallet_RunWorkerCompleted:{0}", ex.ToString()), _TraceCategory);
            }
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("bwPreCreateQREWallet_RunWorkerCompleted Completed."), _TraceCategory);
        }

        public BitmapImage ToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            var bitmapImage = new BitmapImage();
            try
            {

                using (var memory = new MemoryStream())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;


                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    //return bitmapImage;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] ToBitmapImage:{0}", ex.ToString()), _TraceCategory);
            }
            return bitmapImage;
        }

        public BitmapImage FilePathToBitmapImage(string filePath)
        {
            BitmapImage bitmapImage = new BitmapImage();

            try
            {
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(filePath, UriKind.Absolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // To make it cross-thread accessible
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] FilePathToBitmapImage:{0}", ex.ToString()), _TraceCategory);
            }

            return bitmapImage;
        }

        public string DownloadPaymentImage(string url, string referenceId)
        {
            string result = string.Empty;

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                using (var wc = new WebClient())
                {
                    byte[] imageBytes = wc.DownloadData(url);

                    // Check if the downloaded data is not empty
                    if (imageBytes.Length > 0)
                    {
                        // Temporarily save the downloaded data to a file for inspection
                        string tempFilePath = Path.Combine(Path.GetTempPath(), referenceId + "_temp_download");
                        File.WriteAllBytes(tempFilePath, imageBytes);
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Temporary downloaded file saved at : {0}", tempFilePath), _TraceCategory);

                        using (var imgStream = new MemoryStream(imageBytes))
                        {
                            // Validate if the stream can be read as an image
                            if (IsImage(imgStream))
                            {
                                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(imgStream);
                                string loc = Path.Combine(GeneralVar.MenuRepository, referenceId + ".png");

                                bitmap.Save(loc, System.Drawing.Imaging.ImageFormat.Png);
                                bitmap.Dispose();

                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("DownloadImage Location : {0}", loc), _TraceCategory);
                                result = loc;
                            }
                            else
                            {
                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, "DownloadImage Failed: Invalid image format.", _TraceCategory);
                            }
                        }
                    }
                    else
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, "DownloadImage Failed: Empty image data.", _TraceCategory);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("DownloadImage Failed Download : {0}", ex.ToString()), _TraceCategory);
            }

            return result;
        }

        private bool IsImage(Stream stream)
        {
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                stream.Position = 0; // Reset stream position for further use
            }
        }

        TimeSpan remainingTime;
        bool timerIsRunning = false;
        DispatcherTimer duitNowTimer;
        void duitNowTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (remainingTime.TotalSeconds > 0)
                {
                    if (IsEN)
                        Lbl_QRTimeRemaining = string.Format("Time Remaining {0:mm\\:ss}", remainingTime);
                    else
                        Lbl_QRTimeRemaining = string.Format("Baki Masa {0:mm\\:ss}", remainingTime);

                    remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));
                    timerIsRunning = true;
                }
                else
                {
                    timerIsRunning = false;
                    duitNowTimer.Stop();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] DuitNowTimer_Tick = {0}", ex.ToString()), _TraceCategory);
            }
        }

        //public string DownloadPaymentImage(string url, string referenceId)
        //{
        //    string result = string.Empty;

        //    try
        //    {
        //        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

        //        using (var wc = new WebClient())
        //        {
        //            using (var imgStream = new MemoryStream(wc.DownloadData(url)))
        //            {
        //                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(imgStream);
        //                string loc = GeneralVar.MenuRepository + @"\" + referenceId + ".png";

        //                bitmap.Save(loc, System.Drawing.Imaging.ImageFormat.Png);
        //                bitmap.Dispose();
        //                bitmap = null;

        //                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("DownloadImage Location : {0}", loc), _TraceCategory);
        //                result = loc;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("DownloadImage Failed Download : {0}", ex.ToString()), _TraceCategory);
        //    }

        //    return result;
        //}

        bool PrepareSuccessCardTrx(Sales_Resp cardResponses)
        {
            bool success = false;
            try
            {
                string code = cardResponses.InvoiceNumber;
                PreparePaymentInformationCard(cardResponses.CardIssuerName);
                ShowPaymentProcessing = Visibility.Collapsed;
                ShowCardPayment = Visibility.Collapsed;

                
                    if (PostOrder(code))
                    {
                        PrintReceipt(true, true, cardResponses, null);
                    }
                    else
                    {
                        PrintReceipt(true, false, cardResponses, null);
                    }
                



            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] PrepareSuccessCardTrx = {0}", ex.ToString()), _TraceCategory);
            }
            return success;
        }

        bool PrepareFailCardTrx()
        {
            bool success = false;
            try
            {
                ShowCardPayment = Visibility.Collapsed;
                ShowPaymentProcessing = Visibility.Collapsed;
                ShowFailPayment = Visibility.Visible;
                SetStage(eStage.PaymentMethodSelection);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] PrepareFailCardTrx = {0}", ex.ToString()), _TraceCategory);
            }
            return success;
        }

        void PreparePaymentInformationEWallet(string param)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PreparePaymentInformationEWallet Starting..."), _TraceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PreparePaymentInformationEWallet param = {0}", param), _TraceCategory);

                if (param.Trim() == "19")
                {
                    _PaymentCategoryId = "19";
                    _PaymentCategoryName = "BOOST";
                }
                else if (param.Trim() == "21")
                {
                    _PaymentCategoryId = "21";
                    _PaymentCategoryName = "GRABPAY";
                }
                else if (param.Trim() == "20")
                {
                    _PaymentCategoryId = "20";
                    _PaymentCategoryName = "MAE";
                }
                else if (param.Trim() == "17")
                {
                    _PaymentCategoryId = "17";
                    _PaymentCategoryName = "TOUCHNGO";
                }
                else if (param.Trim() == "23")
                {
                    _PaymentCategoryId = "23";
                    _PaymentCategoryName = "SHOPEEPAY";
                }
                else if (param.Trim() == "24")
                {
                    _PaymentCategoryId = "24";
                    _PaymentCategoryName = "DUITNOW";
                }
                else if (param.Trim() == "16")
                {
                    _PaymentCategoryId = "16";
                    _PaymentCategoryName = "ALIPAY";
                }
                else if (param.Trim() == "25")
                {
                    _PaymentCategoryId = "25";
                    _PaymentCategoryName = "ALIPAY+";
                }
                else if (param.Trim() == "22")
                {
                    _PaymentCategoryId = "22";
                    _PaymentCategoryName = "UNIONPAYQR";
                }
                else if (param.Trim() == "36")
                {
                    _PaymentCategoryId = "36";
                    _PaymentCategoryName = "WECHATPAY";
                }
                else
                {
                    _PaymentCategoryId = "24";
                    _PaymentCategoryName = "DUITNOW";
                }

                //orderRequest.paymentType = KioskPaymentType.Where(x => x.PaymentName.Replace(" ", "").ToUpper() == _PaymentCategoryName.Trim().ToUpper()).Select(y => y.PaymentId).FirstOrDefault().ToString();
                //orderRequest.payment.paymentTypeId = KioskPaymentType.Where(x => x.PaymentName.Replace(" ", "").ToUpper() == _PaymentCategoryName.Trim().ToUpper()).Select(y => y.PaymentId).FirstOrDefault();

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PreparePaymentInformationEWallet _PaymentCategoryId = {0}", _PaymentCategoryId), _TraceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PreparePaymentInformationEWallet _PaymentCategoryName = {0}", _PaymentCategoryName), _TraceCategory);
                //Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PreparePaymentInformationEWallet PaymentId = {0}", orderRequest.paymentType), _TraceCategory);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PreparePaymentInformationEWallet Completed."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] PreparePaymentInformationEWallet = {0}", ex.ToString()), _TraceCategory);
            }
        }

        void PreparePaymentInformationCard(string param)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PreparePaymentInformationCard Starting..."), _TraceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PreparePaymentInformationCard param = {0}", param), _TraceCategory);

                _PaymentCategoryId = "0";



                switch (param.Trim().ToUpper())
                {
                    case "VISA":
                        _PaymentCategoryName = "VISA";
                        break;
                    case "MASTERCARD":
                        _PaymentCategoryName = "MASTERCARD";
                        break;
                    case "DINERS":
                        _PaymentCategoryName = "DINERS";
                        break;
                    case "AMEX":
                        _PaymentCategoryName = "AMEX";
                        break;
                    case "JCB":
                        _PaymentCategoryName = "JCB";
                        break;
                    case "MYDEBIT":
                        _PaymentCategoryName = "DEBIT";
                        break;
                    case "UNIONPAY":
                        _PaymentCategoryName = "UNIONPAYCC";
                        break;
                    default:
                        _PaymentCategoryName = "VISA";
                        break;
                }

                //orderRequest.paymentType = KioskPaymentType.Where(x => x.PaymentName.Replace(" ", "").ToUpper() == _PaymentCategoryName.Trim().ToUpper()).Select(y => y.PaymentId).FirstOrDefault().ToString();
                //orderRequest.payment.paymentTypeId = KioskPaymentType.Where(x => x.PaymentName.Replace(" ", "").ToUpper() == _PaymentCategoryName.Trim().ToUpper()).Select(y => y.PaymentId).FirstOrDefault();


                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PreparePaymentInformationCard _PaymentCategoryId = {0}", _PaymentCategoryId), _TraceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PreparePaymentInformationCard _PaymentCategoryName = {0}", _PaymentCategoryName), _TraceCategory);
                //Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PreparePaymentInformationCard PaymentId = {0}", orderRequest.paymentType), _TraceCategory);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PreparePaymentInformationCard Completed."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] PreparePaymentInformationCard = {0}", ex.ToString()), _TraceCategory);
            }
        }

        bool PrepareSuccessEwalletTrx(RazerPaymentResponse eWalletResponse)
        {
            bool success = false;
            try
            {
                string code = eWalletResponse.referenceId;
                PreparePaymentInformationEWallet(eWalletResponse.channelId);
                ShowEWallet = Visibility.Collapsed;
                ShowPaymentProcessing = Visibility.Collapsed;

                //if (initialOrderResponse != null)
                //{
                //    if (string.IsNullOrEmpty(initialOrderResponse.OrderId))
                //        intialOrderRequest.orderId = initialOrderId;
                //    else
                //        intialOrderRequest.orderId = Convert.ToInt32(initialOrderResponse.OrderId);

                //    if (PostOrder(intialOrderRequest, code,initialOrderResponse))
                //    {
                //        PrintReceipt(false, true, null, eWalletResponse);
                //    }
                //    else
                //    {
                //        PrintReceipt(false, false, null, eWalletResponse);
                //    }
                //}
                //else
                //{
                //    PrintReceipt(false, false, null, eWalletResponse);
                //}
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] PrepareSuccessEwalletTrx = {0}", ex.ToString()), _TraceCategory);
            }
            return success;
        }

        bool PrepareSuccessQRTrx(RazerPaymentResponse qrResponse)
        {
            bool success = false;
            try
            {
                string code = qrResponse.referenceId;
                PreparePaymentInformationEWallet(qrResponse.channelId);
                ShowQRPayment = Visibility.Collapsed;
                ShowPaymentProcessing = Visibility.Collapsed;
                
                if (PostOrder(code))
                {
                    PrintReceipt(false, true, null, qrResponse);
                }
                else
                {
                    PrintReceipt(false, false, null, qrResponse);
                }
                

                //if (initialOrderResponse != null)
                //{
                //    if (string.IsNullOrEmpty(initialOrderResponse.OrderId))
                //        intialOrderRequest.orderId = initialOrderId;
                //    else
                //        intialOrderRequest.orderId = Convert.ToInt32(initialOrderResponse.OrderId);

                //    if (PostOrder(intialOrderRequest,code ,initialOrderResponse))
                //    {
                //        PrintReceipt(false, true, null, qrResponse);
                //    }
                //    else
                //    {
                //        PrintReceipt(false, false, null, qrResponse);
                //    }
                //}
                //else
                //{
                //    PrintReceipt(false, false, null, qrResponse);
                //}
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] PrepareSuccessEwalletTrx = {0}", ex.ToString()), _TraceCategory);
            }
            return success;
        }

        bool PrepareFailEwalletTrx()
        {
            bool success = false;
            try
            {
                ShowLoading = Visibility.Collapsed;
                ShowQRPayment = Visibility.Collapsed;
                ShowFailPayment = Visibility.Visible;
                SetStage(eStage.PaymentMethodSelection);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] PrepareFailEwalletTrx = {0}", ex.ToString()), _TraceCategory);
            }
            return success;
        }

        void CloseQRPayment()
        {
            try
            {
                timerIsRunning = false;
                // Timer reached 0:00, stop the timer or perform an action
                duitNowTimer.Stop();

                ShowLoading = Visibility.Visible;
                isStopQRPayment = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] CloseQRPayment = {0}", ex.ToString()), _TraceCategory);
            }
        }

        //bool InitialOrder(string paymentMethod)
        //{
        //    bool success = false;
        //    try
        //    {
        //        if (orderRequest.paymentTotal == 0)
        //            paymentId = KioskPaymentType.Where(x => x.PaymentGroup.ToUpper() == "PF").Select(y => y.PaymentId).FirstOrDefault();
        //        else
        //        {
        //            switch (paymentMethod)
        //            {
        //                case "DUITNOW":
        //                case "UNION":
        //                case "ALIPAY":
        //                case "ALIPAY+":
        //                case "WECHAT":
        //                    paymentId = KioskPaymentType.Where(x => x.PaymentGroup.ToUpper() == "KEW").Select(y => y.PaymentId).FirstOrDefault();
        //                    break;
        //                case "KCC":
        //                    paymentId = KioskPaymentType.Where(x => x.PaymentGroup.ToUpper() == paymentMethod).Select(y => y.PaymentId).FirstOrDefault();
        //                    break;
        //            }
        //        }


        //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger thPayment Starting ..."), _TraceCategory);
        //        payment = new Order.OrderPayment(paymentId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, orderRequest.paymentTotal, string.Empty);

        //        CalculateTotalItem();

        //        intialOrderRequest = new Order.OrderRequest
        //        (
        //            orderRequest.txId,
        //            orderRequest.orderId,
        //            orderRequest.clientId,
        //            orderRequest.mobileNo,
        //            orderRequest.branchId,
        //            orderRequest.addressId,
        //            orderRequest.channelType,
        //            orderRequest.orderDateTime,
        //            orderRequest.collectionDateTime,
        //            orderRequest.deliveryDateTime,
        //            orderRequest.carPlateNo,
        //            orderRequest.cutlery,
        //            orderRequest.orderRemark,
        //            paymentId.ToString(),//payment Type
        //            orderRequest.subTotal,
        //            orderRequest.tax,
        //            orderRequest.serviceCharge,
        //            orderRequest.deliveryFee,
        //            orderRequest.deliveryDiscount,
        //            orderRequest.discount,
        //            orderRequest.total,
        //            orderRequest.rounding,
        //            orderRequest.paymentTotal,
        //            orderRequest.tenderTotal,
        //            orderRequest.EntityCId,
        //            orderRequest.Ogi,
        //            orderRequest.noteToRider,
        //            orderRequest.displayNumber,
        //            orderRequest.promoTxId,
        //            orderRequest.voucherTxId,
        //            orderRequest.orderMenuList,
        //            orderRequest.payment,
        //            orderRequest.vouchers
        //            );



        //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger thPayment Done ..."), _TraceCategory);
        //        success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger thPayment Error - {0}", ex.ToString()), _TraceCategory);
        //    }

        //    return success;
        //}

        bool PostOrder(string paymentTxId)
        {
            bool success = false;
            try
            {
                fnbRes = null;
                ShowSendingKitchen = Visibility.Visible;
                #region Not Needed

                //Thread thFnbOrder = new Thread(() =>
                //{
                //    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger thFnbOrder Starting ..."), _TraceCategory);
                //    try
                //    {
                //        if (_ApiFunc.FnBOrder(orderRequest, out fnbRes))
                //        {
                //            if (fnbRes != null)
                //            {
                //                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Post Order Success..."), _TraceCategory);
                //                OrderNum = fnbRes.data.SalesNo.ToString();
                //                //EarnPoint = postOrderRes.EarnPoints;
                //                ShowSendingKitchen = Visibility.Collapsed;
                //            }
                //            else
                //            {
                //                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Error] Trigger thFnbOrder Fail ..."), _TraceCategory);
                //                throw new Exception("Post Order Fail");
                //            }
                //        }
                //        ShowSendingKitchen = Visibility.Collapsed;
                //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger thFnbOrder Done ..."), _TraceCategory);

                //    }
                //    catch (Exception ex)
                //    {
                //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Error] thFnBOrder fail - {0}",ex.ToString()), _TraceCategory);
                //    }
                //});

                //thFnbOrder.Start();

                #endregion

                ApiModel.PostOrderRequest postOrderRequest = new ApiModel.PostOrderRequest
                {
                    IsDelayOrder = IsDelaySentOrder,
                    ReferenceNo = initialOrderResponse.ReferenceNo,
                    OrderDetails = orderDetail,
                    ComponentId = GeneralVar.ComponentId,
                    ComponentCode = GeneralVar.ComponentCode,
                    OutletId = 1                    
                };

                if (_ApiFunc.PostOrder(postOrderRequest, out fnbRes))
                {
                    if (fnbRes != null)
                    {
                        if(fnbRes.Code=="00")
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Post Order Success..."), _TraceCategory);
                            OrderNum = fnbRes.OrderNumber.ToString();
                            //EarnPoint = postOrderRes.EarnPoints;
                            ShowSendingKitchen = Visibility.Collapsed;
                        }
                        else
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Error] Trigger thFnbOrder Fail ..."), _TraceCategory);
                            throw new Exception("Post Order Fail");
                        }
                        
                    }
                    else
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Error] Trigger thFnbOrder Fail ..."), _TraceCategory);
                        throw new Exception("Post Order Fail");
                    }
                    success = true;
                }
                else
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Error] Trigger thFnbOrder Fail ..."), _TraceCategory);
                    throw new Exception("Post Order Fail");
                    //}
                    ShowSendingKitchen = Visibility.Collapsed;
                }
                
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger thFnbOrder Done ..."), _TraceCategory);

                //waitforQRScan.Reset();
                //waitforQRScan.WaitOne();

                //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                //{
                //    SetStage(eStage.FinalPage);
                //}));

                //GeneralVar.DocumentPrint.Print_CardReceipt(Convert.ToDecimal(AnWTotalAmount), GeneralVar.ComponentCode, "--", dineMethod, CartList, null, 0m, 0, 0);
                                
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("[Error] PostOrder = {0}", ex.ToString()), _TraceCategory);
            }

            return success;
        }

        #endregion

        #endregion

        #region OrderNum Page

        #region property
        private string _OrderNum;

        public string OrderNum
        {
            get { return _OrderNum; }
            set
            {
                _OrderNum = value;
                OnPropertyChanged(nameof(OrderNum));
            }
        }

        private int _EarnPoint;

        public int EarnPoint
        {
            get { return _EarnPoint; }
            set
            {
                _EarnPoint = value;
                OnPropertyChanged(nameof(EarnPoint));
            }
        }

        #endregion

        #region Command

        private ICommand _BtnDone;
        public ICommand BtnDone
        {
            get
            {
                if (_BtnDone == null)
                    _BtnDone = new RelayCommand
                        (
                            execute: () =>
                            {
                                _WaitDone.Set(); // This will be executed when the command is invoked.
                            },
                            canExecute: () => true // Optional: You can provide a method to determine if the command can execute.);
                        );
                return _BtnDone;
            }
        }

        #endregion

        #region Function

        bool PrintReceipt(bool isCardPayment, bool isOrderSuccess, Sales_Resp cardResponse = null, RazerPaymentResponse walletResponse = null)
        {
            bool success = false;
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    SetStage(eStage.FinalPage);
                }));

                if (isCardPayment)
                {
                    if (!GeneralVar.DocumentPrint.Print_CardReceipt(isOrderSuccess, TotalAmount, KioskId, fnbRes.OrderNumber.ToString(), dineMethod, CartList, cardResponse, Convert.ToDecimal(AnWTotalAmount), AnWTax, AnWRounding, AnWVoucherAmount, AnWGiftVoucher, StoreName, IsDelaySentOrder))
                    {
                        _WaitDone.Reset();
                        _WaitDone.WaitOne(10000);

                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            SetStage(eStage.Offline);
                        }));
                    }
                    else
                    {
                        _WaitDone.Reset();
                        _WaitDone.WaitOne(10000);

                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            SetStage(eStage.Home);
                        }));
                    }
                }
                else
                {
                    if (!GeneralVar.DocumentPrint.Print_WalletReceipt(isOrderSuccess, TotalAmount, KioskId, fnbRes.OrderNumber.ToString(), dineMethod, CartList, walletResponse, Convert.ToDecimal(AnWTotalAmount), AnWTax, AnWRounding, AnWVoucherAmount, AnWGiftVoucher, StoreName))
                    {
                        _WaitDone.Reset();
                        _WaitDone.WaitOne(10000);

                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            SetStage(eStage.Offline);
                        }));
                    }
                    else
                    {
                        _WaitDone.Reset();
                        _WaitDone.WaitOne(10000);

                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            SetStage(eStage.Home);
                        }));
                    }
                }

                success = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] PrintReceipt = {0}", ex.ToString()), _TraceCategory);
            }
            return success;
        }

        #endregion

        #endregion

        #region Maintenance Mode
        #region Command
        private ICommand _BtnReturnInit;
        public ICommand BtnReturnInit
        {
            get
            {
                if (_BtnReturnInit == null)
                    _BtnReturnInit = new RelayCommand(BtnBackToInitialize);
                return _BtnReturnInit;
            }
        }
        #endregion
        #region property
        private System.Windows.Media.Brush _OperationModeColour;

        public System.Windows.Media.Brush OperationModeColour
        {
            get { return _OperationModeColour = !UnderMaintenance ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red; }
        }
        #endregion
        #region function

        public void BtnBackToInitialize()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger BtnBackToInitialize Starting ..."), _TraceCategory);

                if (!UnderMaintenance)
                {
                    GeneralVar.MainWindowVM.SetStage(eStage.Initializing);
                    PreLoad(false);
                }
                else
                {
                    SetStage(eStage.Offline, "Manually Set Offline");
                }

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger BtnBackToInitialize Done ..."), _TraceCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] BtnBackToInitialize = {0}", ex.ToString()), _TraceCategory);
            }
        }

        #endregion
        #endregion
    }

    public class TimePicker
    {
        public TimePicker() { }

        public string sTime { get; set; }
        public DateTime dTime { get; set; }
    }

    public class ReprintSelectionHelper : Base
    {
        public ICommand Command { get; set; }

        public string CommandParameter { get; set; }

        public string rPath { get; set; }

        public ReprintSelectionHelper(string displayName, ICommand command, string commandParameter, string rp)
        {
            DisplayName = displayName;
            Command = command;
            CommandParameter = commandParameter;
            rPath = rp;
        }
    }
}