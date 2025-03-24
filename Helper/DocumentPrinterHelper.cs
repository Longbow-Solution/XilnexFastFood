using LFFSSK;
using LFFSSK.Model;
using MPAY.Model.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XFDevice.Custom.Printer;
using XFUtility.DocumentPrint;
using XFDevice.FujitsuPrinter;
using System.Windows.Forms;
using System.Windows;
using XFUtility.Barcode;
using MpayIntegration;
using LFFSSK.Helper;
using System.Collections.ObjectModel;
using EDC_Serial_Lib.MessageResp;

namespace LFFSSK
{
    public class DocumentPrintHelper : DocumentPrint
    {
        const string traceCategory = "DocumentPrintController";

        private FujitsuPrinter _printerFHandler;
        private string _printerName, _LastError;
        private bool _isLandscape, _isError;
        private ManualResetEvent _printedEvent = new ManualResetEvent(false);
        private ManualResetEvent _requestEvent = new ManualResetEvent(false);
        private Queue<PrintRequest> _requestQueue = new Queue<PrintRequest>();
        public XFDevice.Custom.Printer.PrinterHandler printer;
        private bool _IsPaperEnd = false;
        public bool IsWarning = false;
        private FujitsuPrinter FujitsuCtrl;

        #region Properties
        private Dictionary<string, string> _TokenList;
        public Dictionary<string, string> TokenList
        {
            get { return _TokenList; }
            set
            {
                if (_TokenList != value)
                    _TokenList = value;
            }
        }

        private Dictionary<string, Bitmap> _ImageList;
        public Dictionary<string, Bitmap> ImageList
        {
            get { return _ImageList; }
            set
            {
                if (_ImageList != value)
                    _ImageList = value;
            }
        }

        private List<TableFormat> _TableList;
        public List<TableFormat> TableList
        {
            get { return _TableList; }
            set
            {
                if (_TableList != value)
                    _TableList = value;
            }
        }

        public bool IsPaperEnd
        {
            get { return _IsPaperEnd; }
            set
            {
                _IsPaperEnd = value;
            }
        }

        public string LastError
        {
            get { return _LastError; }
            set
            {
                _LastError = value;
            }
        }
        #endregion

        #region Constructor

        public DocumentPrintHelper(string printerName)
        {

            GeneralFunc.SetLicense();

            //_printerHandler = new PrinterHandler();
            //_printerHandler.
            if (FujitsuCtrl == null)
                FujitsuCtrl = new FujitsuPrinter();
            _printerName = printerName;
        }

        #endregion

        //public XFDevice.Custom.Printer.Printer.eErrorCode InitializePrinter()
        //{

        //    InitializeDocuments();
        //    StartProcessingQueue();

        //    return _printerHandler.Initialise();
        //}

        public bool InitializePrinterFujitsu()
        {
            bool IsSuccess = false;
            try
            {
                InitializeDocuments();
                StartProcessingQueue();

                bool getStatus = FujitsuCtrl.GetStatus();

                if (!FujitsuCtrl.GetStatus())
                    throw new Exception("[FujitsuCtrl]Offline");
                    //throw new Exception("[FujitsuCtrl]Unable to GetStatus");

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("CoverOpen = {0}", FujitsuCtrl.LastPrinterStatus[ePrinterStatus.CoverOpen]), traceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PaperEnd = {0}", FujitsuCtrl.LastPrinterStatus[ePrinterStatus.PaperEnd]), traceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("PaperNearEnd = {0}", FujitsuCtrl.LastPrinterStatus[ePrinterStatus.PaperNearEnd]), traceCategory);

                if (FujitsuCtrl.LastPrinterStatus[ePrinterStatus.CoverOpen])
                    throw new Exception("[FujitsuCtrl] Error");

                if (FujitsuCtrl.LastPrinterStatus[ePrinterStatus.PaperEnd])
                {
                    //updated 13/11/2020
                    GeneralVar.MainWindowVM.ShowPaperEnd = Visibility.Visible;
                    throw new Exception("[FujitsuCtrl] PaperEnd");
                }
                else
                    GeneralVar.MainWindowVM.ShowPaperEnd = Visibility.Collapsed;

                IsSuccess = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] InitializePrinterFujitsu = {0}", ex.ToString()), traceCategory);
                LastError = ex.Message;
            }

            return IsSuccess;

        }

        public void InitializeDocuments()
        {
            PrintDocument printDoc;
            TokenList = new Dictionary<string, string>();
            TableList = new List<TableFormat>();
            ImageList = null;
            PaperSize paperSize = new PaperSize("default", 330, 300);

            /*TO DO: Load All Document*/
            string printerTemplateFolder = Environment.CurrentDirectory + @"\Resource\Template\";
            List<string> printerTemplateFiles = new List<string>();
            printerTemplateFiles.Add(@"AnWOrderReceiptCC.docx");
            printerTemplateFiles.Add(@"AnWOrderReceiptCC_Failed.docx");

            foreach (string file in printerTemplateFiles)
                SendAsPrintDocument(printerTemplateFolder + file, out printDoc, TokenList, ImageList, TableList, _isLandscape, paperSize, paperSize);
        }

        public bool FujitsuReceiptPrintingChecking()
        {
            bool IsSuccess = false;
            int i = 0;
            while (i < 3)
            {
                try
                {
                    LastError = string.Empty;
                    if (!FujitsuCtrl.GetStatus())
                        throw new Exception("Unable to GetStatus");

                    _IsPaperEnd = FujitsuCtrl.LastPrinterStatus[ePrinterStatus.PaperEnd];

                    IsWarning = FujitsuCtrl.LastPrinterStatus[ePrinterStatus.PaperNearEnd];

                    if (FujitsuCtrl.LastPrinterStatus[ePrinterStatus.CoverOpen])
                        throw new Exception("Cover Open");

                    if (FujitsuCtrl.LastPrinterStatus[ePrinterStatus.PaperEnd])
                        throw new Exception("Paper End");

                    IsSuccess = true;
                }
                catch (Exception ex)
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] FujitsuReceiptPrintingChecking: {0}", ex.ToString()), traceCategory);
                    LastError = ex.Message;
                }
                if (IsSuccess)
                    i = 3;
                else
                {
                    Thread.Sleep(500);
                    i++;
                }
            }
            return IsSuccess;
        }
        bool ThreadStart = false;

        public void StartProcessingQueue()
        {
            try
            {
                if (!ThreadStart)
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(o => ProcessPrinterQueue());
                    ThreadStart = true;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] StartProcessingQueue:{0}", ex.ToString()), traceCategory);
            }
        }



        #region Project Based Functions

        public bool Reprint(string check)
        {

            PrintDocument doc = new PrintDocument();
            //doc.DocumentName = @"C:\TempReceipt\108.docx";
            SendAsPrintDocument(check, out doc, null, null, null, false);
            doc.Print();
            //EnqueueRequest(new PrintRequest(doc, 3000));

            return true;
        }

        public List<string> GetFileList()
        {
            try
            {
                return Directory.GetFiles(@"D:\\TempReceipt\", "*.docx",
                                                 SearchOption.TopDirectoryOnly).Where(c => new FileInfo(c).LastWriteTime.Date == DateTime.Now.Date).OrderByDescending(d => new FileInfo(d).LastWriteTime).ToList();

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] GetFileList = {0}", ex.ToString()), traceCategory);
                return new List<string>();
            }
        }

        public bool Print_CardReceipt(bool isOrderSuccess, decimal subTotal, string componentCode, string queueNo, string dineMethod, ObservableCollection<CartModel.Product> cartItem, Sales_Resp response, decimal total, double tax, double rounding, double voucherAmount, double giftAmount,string storeName)
        {
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Print_CardReceipt Starting..."), traceCategory);
            bool isSuccess = false;
            try
            {
                int height = 1000;
                _isError = false;
                _isLandscape = false;
                string payMethod = string.Empty;


                ImageList = new Dictionary<string, Bitmap>();
                if (string.IsNullOrEmpty(dineMethod))
                    dineMethod = "-";

                if (string.IsNullOrEmpty(GeneralVar.MainWindowVM._PaymentCategoryName))
                    payMethod = "-";
                else
                    payMethod = GeneralVar.MainWindowVM._PaymentCategoryName;


                #region TokenList

                TokenList = new Dictionary<string, string>()
                    {
                        {"[@Date]", DateTime.Now.ToString("dd MMM yy HH:mm:ss")},
                        {"[@KioskCode]", componentCode},
                        {"[@QueueNo]", queueNo},
                        {"[@DineMethod]", dineMethod},                       
                        {"[@StoreName]", "A&W"}
                    };

                #endregion

                TableList = new List<TableFormat>();

                #region menu summary
                TableFormat detailTable = new TableFormat();
                detailTable.includeHeader = true;
                detailTable.tableName = "SummaryList";
                detailTable.data = new DataTable(detailTable.tableName);
                detailTable.data.Columns.Add();
                detailTable.data.Columns.Add();
                detailTable.data.Columns.Add();

                if (cartItem != null && cartItem.Count() > 0)
                {
                    foreach (CartModel.Product order in cartItem)
                    {
                        height += 20;
                        DataRow row = detailTable.data.NewRow();
                        row[0] = order.ItemCurrentQty;
                        row[1] = order.itemName;
                        row[2] = order.ItemTotalPrice.ToString("N2");
                        detailTable.data.Rows.Add(row);

                        foreach (var orderDetails in order.dynamicmodifiers.SelectMany(modifier=>modifier.modifiers).ToList())
                        {
                            foreach (var containDetails in orderDetails.selections)
                            {
                                if (containDetails.IsCheck)
                                {
                                    height += 20;
                                    row = detailTable.data.NewRow();
                                    row[1] = containDetails.name;
                                    if (containDetails.price > 0)
                                        row[2] = containDetails.price.ToString("N2");
                                    else
                                        row[2] = "";
                                    detailTable.data.Rows.Add(row);
                                }
                            }
                        }
                    }

                }

                TableList.Add(detailTable);
                height += 15;

                #endregion

                #region trx amount
                TableFormat trxDetailsTable = new TableFormat();
                trxDetailsTable.includeHeader = false;
                trxDetailsTable.tableName = "TransactionDetails";
                trxDetailsTable.data = new DataTable(trxDetailsTable.tableName);
                trxDetailsTable.data.Columns.Add();
                trxDetailsTable.data.Columns.Add();

                DataRow paymentType = trxDetailsTable.data.NewRow();
                paymentType[0] = "Payment Type:";
                paymentType[1] = payMethod;
                trxDetailsTable.data.Rows.Add(paymentType);
                height += 15;

                DataRow Subtt = trxDetailsTable.data.NewRow();
                Subtt[0] = "Subtotal (Inc tax and fees):";
                Subtt[1] = string.Format("RM {0}", subTotal.ToString("N2"));
                trxDetailsTable.data.Rows.Add(Subtt);
                height += 15;

                DataRow Sst = trxDetailsTable.data.NewRow();
                Sst[0] = "SST (6%):";
                Sst[1] = string.Format("RM {0}", tax.ToString("N2"));
                trxDetailsTable.data.Rows.Add(Sst);
                height += 15;

                if (voucherAmount > 0)
                {
                    DataRow VcAmount = trxDetailsTable.data.NewRow();
                    VcAmount[0] = "Voucher Amount:";
                    VcAmount[1] = string.Format("RM {0}", voucherAmount.ToString("N2"));
                    trxDetailsTable.data.Rows.Add(VcAmount);
                    height += 15;
                }

                if (giftAmount > 0)
                {
                    DataRow GfAmount = trxDetailsTable.data.NewRow();
                    GfAmount[0] = "Gift Voucher:";
                    GfAmount[1] = string.Format("RM {0}", giftAmount.ToString("N2"));
                    trxDetailsTable.data.Rows.Add(GfAmount);
                    height += 15;
                }

                DataRow totalAmount = trxDetailsTable.data.NewRow();
                totalAmount[0] = "Total:";
                totalAmount[1] = string.Format("RM {0}", total.ToString("N2"));
                trxDetailsTable.data.Rows.Add(totalAmount);
                height += 15;

                DataRow roundUp = trxDetailsTable.data.NewRow();
                roundUp[0] = "Rounding:";
                roundUp[1] = string.Format("RM {0}", rounding.ToString("N2"));
                trxDetailsTable.data.Rows.Add(roundUp);
                height += 15;

                DataRow gTotal = trxDetailsTable.data.NewRow();
                gTotal[0] = "Grand Total:";
                gTotal[1] = string.Format("RM {0}", total.ToString("N2"));
                trxDetailsTable.data.Rows.Add(gTotal);
                height += 15;

                trxDetailsTable.prefixName = "";
                TableList.Add(trxDetailsTable);
                height += 15;

                #endregion

                #region credit summary
                TableFormat ccdetailTable = new TableFormat();
                ccdetailTable.tableName = "CCSummary";
                ccdetailTable.data = new DataTable(ccdetailTable.tableName);
                ccdetailTable.data.Columns.Add();
                ccdetailTable.data.Columns.Add();

                if (GeneralVar.MainWindowVM.CustomerID != 0)
                {
                    DataRow cId = ccdetailTable.data.NewRow();
                    cId[0] = "Customer Id:";
                    cId[1] = GeneralVar.MainWindowVM.CustomerPhoneNo;
                    ccdetailTable.data.Rows.Add(cId);
                    height += 15;

                    DataRow cRP = ccdetailTable.data.NewRow();
                    cRP[0] = "Rooty Points:";
                    cRP[1] = GeneralVar.MainWindowVM.EarnPoint;
                    ccdetailTable.data.Rows.Add(cRP);
                    height += 15;
                }


                if (response != null)
                {                    
                    DataRow cCT = ccdetailTable.data.NewRow();
                    cCT[0] = "CARD TYPE:";
                    cCT[1] = response.CardIssuerName;
                    ccdetailTable.data.Rows.Add(cCT);
                    height += 15;

                    DataRow cTID = ccdetailTable.data.NewRow();
                    cTID[0] = "TID:";
                    cTID[1] = response.TerminalId;
                    ccdetailTable.data.Rows.Add(cTID);
                    height += 15;

                    DataRow cMID = ccdetailTable.data.NewRow();
                    cMID[0] = "MID:";
                    cMID[1] = response.MerchantNo;
                    ccdetailTable.data.Rows.Add(cMID);
                    height += 15;

                    DataRow cDT = ccdetailTable.data.NewRow();
                    cDT[0] = "DATE:";
                    cDT[1] = response.TransactionDate;
                    ccdetailTable.data.Rows.Add(cDT);
                    height += 15;

                    DataRow cTM = ccdetailTable.data.NewRow();
                    cTM[0] = "TIME:";
                    cTM[1] = response.TransactionTime;
                    ccdetailTable.data.Rows.Add(cTM);
                    height += 15;

                    DataRow cCN = ccdetailTable.data.NewRow();
                    cCN[0] = "CARD NUM:";
                    cCN[1] = response.CardNo;
                    ccdetailTable.data.Rows.Add(cCN);
                    height += 15;

                    DataRow cXD = ccdetailTable.data.NewRow();
                    cXD[0] = "EXPIRY DATE:";
                    cXD[1] = response.ExpiryDate;
                    ccdetailTable.data.Rows.Add(cXD);
                    height += 15;

                    DataRow cAPR = ccdetailTable.data.NewRow();
                    cAPR[0] = "APPR CODE:";
                    cAPR[1] = response.ApprovalCode;
                    ccdetailTable.data.Rows.Add(cAPR);
                    height += 15;

                    DataRow cRRE = ccdetailTable.data.NewRow();
                    cRRE[0] = "RREF NUM:";
                    cRRE[1] = response.RetrievalReferenceNo;
                    ccdetailTable.data.Rows.Add(cRRE);
                    height += 15;

                    DataRow cBTC = ccdetailTable.data.NewRow();
                    cBTC[0] = "BATCH NUM:";
                    cBTC[1] = response.BatchNo;
                    ccdetailTable.data.Rows.Add(cBTC);
                    height += 15;

                    DataRow cINV = ccdetailTable.data.NewRow();
                    cINV[0] = "INV NUM:";
                    cINV[1] = response.InvoiceNumber;
                    ccdetailTable.data.Rows.Add(cINV);
                    height += 15;                  

                }

                ccdetailTable.includeHeader = true;
                ccdetailTable.prefixName = "";
                TableList.Add(ccdetailTable);
                height += 15;
                #endregion

                PaperSize paperSize = new PaperSize("default", 300, height);

                PrintDocument printDoc = null;
                printDoc = new PrintDocument();

                if (!Directory.Exists(GeneralVar.ReceiptBackupPath))
                {
                    Directory.CreateDirectory(GeneralVar.ReceiptBackupPath);
                }

                if (isOrderSuccess)
                {
                    var saveStatus = SaveAsFile(Environment.CurrentDirectory + @"\Resource\Template\" + "AnWOrderReceiptCC.docx", @GeneralVar.ReceiptBackupPath + "\\" + queueNo + ".docx", WordFileFormat.Docx, TokenList, ImageList, TableList, paperSize);
                    var status = SendAsPrintDocument(Environment.CurrentDirectory + @"\Resource\Template\" + "AnWOrderReceiptCC.docx", out printDoc, TokenList, ImageList, TableList, _isLandscape, paperSize, paperSize);
                }
                else
                {
                    var saveStatus = SaveAsFile(Environment.CurrentDirectory + @"\Resource\Template\" + "AnWOrderReceiptCC_Failed.docx", @GeneralVar.ReceiptBackupPath + "\\" + queueNo + ".docx", WordFileFormat.Docx, TokenList, ImageList, TableList, paperSize);
                    var status = SendAsPrintDocument(Environment.CurrentDirectory + @"\Resource\Template\" + "AnWOrderReceiptCC_Failed.docx", out printDoc, TokenList, ImageList, TableList, _isLandscape, paperSize, paperSize);
                }

                printDoc.DocumentName = queueNo;

                printDoc.Print();
                _isError = !FujitsuReceiptPrintingChecking();


                //Thread.Sleep(1000);
                //Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Print_CardReceipt ReferenceNo = {0}", referenceNo), traceCategory);
                //Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Print_CardReceipt PrintDoc Name = {0}", printDoc.DocumentName), traceCategory);

                //if (status != DocumentProcessError.Success)
                //    throw new Exception(status.ToString());
                //EnqueueRequest(new PrintRequest(printDoc, 3000));

                //_printedEvent.Reset();
                //_printedEvent.WaitOne(30000);



                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Print_CardReceipt Completed."), traceCategory);
                isSuccess = true;
                return !_isError;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] Print_CardReceipt = {0}", ex.ToString()), traceCategory);
            }
            return isSuccess;
        }

        public bool Print_WalletReceipt(bool isOrderSuccess, decimal subTotal, string componentCode, string queueNo, string dineMethod, ObservableCollection<CartModel.Product> cartItem, RazerPaymentResponse response, decimal total, double tax, double rounding,double voucherAmount,double giftAmount,string storeName)
        {
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Print_WalletReceipt Starting..."), traceCategory);

            bool isSuccess = false;
            try
            {
                int height = 1000;
                _isError = false;
                _isLandscape = false;

                ImageList = new Dictionary<string, Bitmap>();
                if (string.IsNullOrEmpty(dineMethod))
                    dineMethod = "-";

                #region TokenList

                TokenList = new Dictionary<string, string>()
                    {
                        {"[@Date]", DateTime.Now.ToString("dd MMM yy HH:mm:ss")},
                        {"[@KioskCode]", componentCode},
                        {"[@QueueNo]", queueNo},
                        {"[@DineMethod]", dineMethod},
                        {"[@Subtotal]",subTotal.ToString("N2") },
                        {"[@SST]",tax.ToString("N2")},
                        {"[@Rounding]",rounding.ToString("N2")},
                        {"[@GrandT]",total.ToString("N2") },
                        {"[@Total]",total.ToString("N2") },
                        {"[@StoreName]", storeName},
                        {"[@VoucherAmount]", voucherAmount.ToString("N2")},
                        {"[@GiftAmount]", giftAmount.ToString("N2")},
                        {"[@PaymentType]", GeneralVar.MainWindowVM._PaymentCategoryName},
                    };

                TableList = new List<TableFormat>();
                TableFormat trxDetailsTable = new TableFormat();
                trxDetailsTable.includeHeader = false;
                trxDetailsTable.tableName = "TransactionDetails";
                trxDetailsTable.data = new DataTable(trxDetailsTable.tableName);
                trxDetailsTable.data.Columns.Add();
                trxDetailsTable.data.Columns.Add();

                DataRow paymentType = trxDetailsTable.data.NewRow();
                paymentType[0] = "Payment Type:";
                paymentType[1] = GeneralVar.MainWindowVM._PaymentCategoryName;
                trxDetailsTable.data.Rows.Add(paymentType);
                height += 15;

                DataRow Subtt = trxDetailsTable.data.NewRow();
                Subtt[0] = "Subtotal (Inc tax and fees):";
                Subtt[1] = string.Format("RM {0}", subTotal.ToString("N2"));
                trxDetailsTable.data.Rows.Add(Subtt);
                height += 15;

                DataRow Sst = trxDetailsTable.data.NewRow();
                Sst[0] = "SST (6%):";
                Sst[1] = string.Format("RM {0}", tax.ToString("N2"));
                trxDetailsTable.data.Rows.Add(Sst);
                height += 15;

                if (voucherAmount > 0)
                {
                    DataRow VcAmount = trxDetailsTable.data.NewRow();
                    VcAmount[0] = "Voucher Amount:";
                    VcAmount[1] = string.Format("RM {0}", voucherAmount.ToString("N2"));
                    trxDetailsTable.data.Rows.Add(VcAmount);
                    height += 15;
                }

                if (giftAmount > 0)
                {
                    DataRow GfAmount = trxDetailsTable.data.NewRow();
                    GfAmount[0] = "Gift Voucher:";
                    GfAmount[1] = string.Format("RM {0}", giftAmount.ToString("N2"));
                    trxDetailsTable.data.Rows.Add(GfAmount);
                    height += 15;
                }

                DataRow totalAmount = trxDetailsTable.data.NewRow();
                totalAmount[0] = "Total:";
                totalAmount[1] = string.Format("RM {0}", total.ToString("N2"));
                trxDetailsTable.data.Rows.Add(totalAmount);
                height += 15;

                DataRow roundUp = trxDetailsTable.data.NewRow();
                roundUp[0] = "Rounding:";
                roundUp[1] = string.Format("RM {0}", rounding.ToString("N2"));
                trxDetailsTable.data.Rows.Add(roundUp);
                height += 15;

                DataRow gTotal = trxDetailsTable.data.NewRow();
                gTotal[0] = "Grand Total:";
                gTotal[1] = string.Format("RM {0}", total.ToString("N2"));
                trxDetailsTable.data.Rows.Add(gTotal);
                height += 15;

                trxDetailsTable.prefixName = "";
                TableList.Add(trxDetailsTable);
                height += 15;

                #endregion

                TableFormat detailTable = new TableFormat();
                detailTable.includeHeader = false;
                detailTable.tableName = "SummaryList";
                detailTable.data = new DataTable(detailTable.tableName);
                detailTable.data.Columns.Add();
                detailTable.data.Columns.Add();
                detailTable.data.Columns.Add();

                if (cartItem != null && cartItem.Count() > 0)
                {
                    foreach (CartModel.Product order in cartItem)
                    {
                        height += 20;
                        DataRow row = detailTable.data.NewRow();
                        row[0] = order.ItemCurrentQty;
                        row[1] = order.itemName;
                        row[2] = order.ItemTotalPrice.ToString("N2");
                        detailTable.data.Rows.Add(row);

                        foreach (var orderDetails in order.dynamicmodifiers.SelectMany(modifier=>modifier.modifiers).ToList())
                        {
                            foreach (var containDetails in orderDetails.selections)
                            {
                                if (containDetails.IsCheck)
                                {
                                    height += 20;
                                    row = detailTable.data.NewRow();
                                    row[1] = containDetails.name;
                                    if (containDetails.price > 0)
                                        row[2] = containDetails.price.ToString("N2");
                                    else
                                        row[2] = "";
                                    detailTable.data.Rows.Add(row);
                                }
                            }
                        }
                    }
                }

                TableList.Add(detailTable);
                height += 15;

                TableFormat ccdetailTable = new TableFormat();
                ccdetailTable.tableName = "CCSummary";
                ccdetailTable.data = new DataTable(ccdetailTable.tableName);
                ccdetailTable.data.Columns.Add();
                ccdetailTable.data.Columns.Add();

                if(GeneralVar.MainWindowVM.CustomerID!=0)
                {
                    DataRow cId = ccdetailTable.data.NewRow();
                    cId[0] = "Customer Id:";
                    cId[1] = GeneralVar.MainWindowVM.CustomerPhoneNo;
                    ccdetailTable.data.Rows.Add(cId);
                    height += 15;

                    DataRow cRP = ccdetailTable.data.NewRow();
                    cRP[0] = "Rooty Points:";
                    cRP[1] = GeneralVar.MainWindowVM.EarnPoint;
                    ccdetailTable.data.Rows.Add(cRP);
                    height += 15;
                }

                if (response != null)
                {
                    DataRow cCT = ccdetailTable.data.NewRow();
                    cCT[0] = "Status Code:";
                    cCT[1] = response.statusCode;
                    ccdetailTable.data.Rows.Add(cCT);
                    height += 15;

                    DataRow cTID = ccdetailTable.data.NewRow();
                    cTID[0] = "Mol Transaction Id:";
                    cTID[1] = response.molTransactionId;
                    ccdetailTable.data.Rows.Add(cTID);
                    height += 15;

                    DataRow cMID = ccdetailTable.data.NewRow();
                    cMID[0] = "Reference Id:";
                    cMID[1] = response.referenceId;
                    ccdetailTable.data.Rows.Add(cMID);
                    height += 15;

                    DataRow cDT = ccdetailTable.data.NewRow();
                    cDT[0] = "Channel Id:";
                    cDT[1] = response.channelId;
                    ccdetailTable.data.Rows.Add(cDT);
                    height += 15;

                    DataRow cTM = ccdetailTable.data.NewRow();
                    cTM[0] = "Amount:";
                    cTM[1] = response.amount.ToString("#.00");
                    ccdetailTable.data.Rows.Add(cTM);
                    height += 15;

                    DataRow cCN = ccdetailTable.data.NewRow();
                    cCN[0] = "QR Authorization Code:";
                    cCN[1] = response.authorizationCode;
                    ccdetailTable.data.Rows.Add(cCN);
                    height += 15;

                    DataRow cXD = ccdetailTable.data.NewRow();
                    cXD[0] = "Currency Code";
                    cXD[1] = response.currencyCode;
                    ccdetailTable.data.Rows.Add(cXD);
                    height += 15;

                    DataRow cAPR = ccdetailTable.data.NewRow();
                    cAPR[0] = "Payer Id:";
                    cAPR[1] = response.payerId;
                    ccdetailTable.data.Rows.Add(cAPR);
                    height += 15;
                    
                }
                ccdetailTable.includeHeader = true;
                ccdetailTable.prefixName = "";
                TableList.Add(ccdetailTable);
                height += 15;

                PaperSize paperSize = new PaperSize("default", 300, height);

                PrintDocument printDoc;

                if (!Directory.Exists(GeneralVar.ReceiptBackupPath))
                {
                    Directory.CreateDirectory(GeneralVar.ReceiptBackupPath);
                }


                if (isOrderSuccess)
                {
                    var saveStatus = SaveAsFile(Environment.CurrentDirectory + @"\Resource\Template\" + "AnWOrderReceiptCC.docx", @GeneralVar.ReceiptBackupPath + "\\" + queueNo + ".docx", WordFileFormat.Docx, TokenList, ImageList, TableList, paperSize);
                    var status = SendAsPrintDocument(Environment.CurrentDirectory + @"\Resource\Template\" + "AnWOrderReceiptCC.docx", out printDoc, TokenList, ImageList, TableList, _isLandscape, paperSize, paperSize);
                }
                else
                {
                    var saveStatus = SaveAsFile(Environment.CurrentDirectory + @"\Resource\Template\" + "AnWOrderReceiptCC_Failed.docx", @GeneralVar.ReceiptBackupPath + "\\" + queueNo + ".docx", WordFileFormat.Docx, TokenList, ImageList, TableList, paperSize);
                    var status = SendAsPrintDocument(Environment.CurrentDirectory + @"\Resource\Template\" + "AnWOrderReceiptCC_Failed.docx", out printDoc, TokenList, ImageList, TableList, _isLandscape, paperSize, paperSize);
                }             

                printDoc.Print();
                _isError = !FujitsuReceiptPrintingChecking();

                //if (status != DocumentProcessError.Success)
                //    throw new Exception(status.ToString());
                //EnqueueRequest(new PrintRequest(printDoc, 3000));

                //_printedEvent.Reset();
                //_printedEvent.WaitOne(30000);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("Print_WalletReceipt Completed."), traceCategory);

                return !_isError;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] Print_WalletReceipt = {0}", ex.ToString()), traceCategory);
            }
            return isSuccess;
        }

        #endregion

        #region queue
        private void ProcessPrinterQueue()
        {
            while (true)
            {
                _requestEvent.Reset();
                _requestEvent.WaitOne();
                Thread.Sleep(250);
                //trace

                if (_requestQueue.Count() > 0)
                {
                    try
                    {
                        PrintRequest request = _requestQueue.Dequeue();
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("ProcessPrinterQueue DocumentName = {0}", request.Document.DocumentName), traceCategory);

                        //request.Document.DocumentName
                        //PrintDialog pdi = new PrintDialog();
                        //pdi.Document = request.Document;
                        //if (pdi.ShowDialog() == DialogResult.OK)
                        //{
                        //    //request.Document.PrinterSettings.PrinterName = _printerName;

                        //    request.Document.Print();
                        //}

                        request.Document.PrinterSettings.PrinterName = _printerName;

                        request.Document.Print();
                        Thread.Sleep(500);

                        if (GeneralVar.PrinterModel == ePrinterModel.Fujitsu)
                            _isError = !FujitsuReceiptPrintingChecking();

                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] ProcessPrinterQueue = {0}", ex.ToString()), traceCategory);
                        LastError = ex.Message;
                        _isError = true;
                    }
                }
                _printedEvent.Set();

                //if (_requestQueue.Count > 0)
                //{
                //    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("ProcessPrinterQueue step:Dequeue"), traceCategory);
                //    PrintRequest request = _requestQueue.Dequeue();
                //    request.Document.PrinterSettings.PrinterName = _printerName;
                //    if (_isError)
                //        continue;

                //    try
                //    {
                //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("ProcessPrinterQueue step:Print Job"), traceCategory);
                //        request.Document.Print();
                //        _printedLength += request.PaperHeight;
                //    }
                //    catch (Exception ex)
                //    {
                //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error]Print:{0}", ex.ToString()), traceCategory);
                //        _isError = true;
                //        _printedEvent.Set();
                //    }
                //    System.Threading.Thread.Sleep(100);
                //}
                //else
                //{
                //    if (!_isQueued)
                //    {
                //        System.Threading.Thread.Sleep(50);
                //        continue;
                //    }

                //    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("ProcessPrinterQueue Custom Printer Check Status"), traceCategory);
                //    XFDevice.Custom.Printer.PrinterHandler.FullPrinterStatus status;
                //    var result = _printerHandler.GetPrinterStatus(out status);
                //    if (result == XFDevice.Custom.Printer.Printer.eErrorCode.NO_ERR)
                //    {

                //        if (status.paperStatus.PaperEnd)
                //        {
                //            IsPaperEnd = true;
                //            LastError = "Printer: PaperStatus.PaperEnd.";
                //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, "[Error] PrintDocument: PaperStatus.PaperEnd", traceCategory);

                //            _printedEvent.Set();

                //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("ProcessPrinterQueue Wait"), traceCategory);
                //            _requestEvent.Reset();
                //            _requestEvent.WaitOne();
                //        }
                //        else if (!status.userError.PrinterQueued)
                //        {
                //            _printedEvent.Set();

                //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("ProcessPrinterQueue Wait"), traceCategory);
                //            _requestEvent.Reset();
                //            _requestEvent.WaitOne();
                //        }
                //        else
                //            System.Threading.Thread.Sleep(100);
                //    }
                //    else
                //    {
                //        _isError = true;
                //        System.Threading.Thread.Sleep(100);
                //    }
                //}
            }
        }



        private void EnqueueRequest(PrintRequest request)
        {
            _requestQueue.Enqueue(request);
            _requestEvent.Set();
        }

        #endregion
    }

    public class PrintRequest
    {
        public PrintDocument Document;
        public int PaperHeight;

        public PrintRequest(PrintDocument document, int paperHeight)
        {

            Document = document;
            PaperHeight = paperHeight;
        }
    }

}
