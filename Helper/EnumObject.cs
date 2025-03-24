using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFFSSK
{
    public enum eStage
    {

        //Default
        None = 0,
        Home,
        Offline,
        Initializing,
        Initialized,
        OutOfOrder,
        OffOperation,

        //project KFC FNB
        CredentialVerificationTimeout,
        LandingPage,
        MenuCategory,
        MenuItem,
        MenuCustomize,
        OrderSummary,
        PaymentMethodSelection,
        PaymentProcessing,
        CashProcessing,
        QRProcessing,
        PaymentByCard,
        PaymentByQR,
        PaymentByCash,
        PaymentByApplePay,
        FinalPage,
        ComingSoon,
        EditItem,
        DoneAddCart,
        Voucher,

        //wilson@2019/03/13 - Maintenance
        MaintenanceLogin,
        MaintenanceSelection,
        MaintenanceAction,
        MaintenanceEDC,
        MaintenanceReprint,
        MaintenanceVoid,
        MenuExclusion,        
    }

    public enum eMaintenanceTask
    {
        Logout,
        ShutdownApp,
        OnOffApp,
        RestartApp,
        CheckCard,
        NoteCollection,
        PrinterReplenishment,
        NoteReplenishment,
        //ChequeCollection,
        MenuExclusion,
        BankSettlement,
        AlarmLog,
        TestPrintTransaction,
        ReprintTransaction,
        VoidTransaction,
        CancelOrder,
        OverridingOrder,
        TestingMode,
    }

    public enum eErrorMessage 
    {
        PaymentIssue,
        PaymentSuccessCuscapiIssue,
        UnableToPrint,
        CuscapiIssue,

    }

    public enum eComponent 
    {
        ReceiptPrinter = 0,
        CreditTerminal,
        QRCodeReader,
        IOBoard,
        System_FNB,
        Testing
    }

    public enum ePaymentMethod 
    {
        None = 0,
        CreditDebit_Card,
        KASH_Card,
        Maybank_QR,
        AliPay_QR,
        WeChat_QR,
        Boost_QR,
        TNG_QR,
        GRAB_QR,
        RAZER_QR,
        SARAWAKPAY_QR,
        Cash,
        VoucherRM5,
        VoucherRM10,
        VoucherRM20,
        ShopeePay
    }

    public enum eInitStatus
    {
        Pending, Checking, Success, Warning, Error, Information
    }

    public enum ePrinterModel 
    {
        None = 0,
        Custom,
        Fujitsu
    }

    public enum eDiningMethod
    { 
        DineIn,
        TakeAway,
    }

    public enum eLanguage 
    {
        English,
        Bahasa
    }

    public enum eMenuStatus 
    {
        None = 'A',
        NoStok = 'O',
        Exclusion = 'C'
    }

    public enum eExclusionTier 
    {
        Schedule = 'A', 
        Category = 'B',
        Group = 'C',
        Menu = 'D',
        Package = 'E',
        PackageGroup = 'F',
        PackageDetail = 'G',

    }

    public enum ePOS 
    {
        Aloha = 'A',
        Cuscapi = 'C',
        DirectAloha = 'D'
    }

    public enum eGoLargeType
    {
        Package = 'A',
        PackageDetail = 'B',
        None = 'C'
    }

    public enum eVoidStatus
    {
        Paid = 'S',
        CashierCancelled = 'T',
        SystemCancelled = 'U',
        SystemVoided = 'V'
    }

    public enum eBillType
    {

    }

    public enum eStatus
    {
        Error = 0,
        Warning,
        Information
    }

    public enum ePaymentTypeUsed
    {
        None = 0,
        Card,
        EWallet
    }

    public enum ePaymentStage
    {
        None = 0,
        PaymentSelection,
        InsertNote,
        Dispense,
        Loading,
        InsertCard,
        EnterPin,
        ReadingCard,
        ApprovedCard,
        RemoveCard,
        TakeOutCard,
        ScanQR,
        QRProcessingPayment,
        PaymentFailed,
        PaymentSuccess,
        EWalletLoading
    }
}
