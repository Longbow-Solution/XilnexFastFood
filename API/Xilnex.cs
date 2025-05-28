using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFFSSK.API
{
    public class Xilnex
    {
        public class Request
        {
            public Request(int salesNo, double totalAmount, double discountAmount, double roundingAmount, double mGstTaxAmount, double billTaxPercentange, double serviceChargeAmount, string customerId, string recipient, string salesOutlet, string salesType, string orderNo, int paxNo, string salesPerson, string remark, bool isPrint, string Datetime, string orderSource, string pickupTime, string recipientContact, string promoCode, double totalDiscountAmount, double totalAmountBeforeTax, string recipientContactTel, double billDiscountAmount, string customFieldValueThree, double billTaxAmount, double serviceChargeAmountAfterTax, bool postStatus, string externalRefId, List<SalesItem> salesItems)
            {
                this.SalesNo = salesNo;
                this.TotalAmount = totalAmount;
                this.DiscountAmount = discountAmount;
                this.RoundingAmount = roundingAmount;
                this.MgstTaxAmount = mGstTaxAmount;
                this.BillTaxPercentage = billTaxPercentange;
                this.ServiceChargeAmount = serviceChargeAmount;
                this.CustomerId = customerId;
                this.Recipient = recipient;
                this.SalesOutlet = salesOutlet;
                this.SalesType = salesType;
                this.OrderNo = orderNo;
                this.PaxNo = paxNo;
                this.SalesPerson = salesPerson;
                this.Remark = remark;
                this.IsPrint = isPrint;
                this.dateTime = Datetime;
                this.OrderSource = orderSource;
                this.PickupTime = pickupTime;
                this.RecipientContact = recipientContact;
                this.PromoCode = promoCode;
                this.TotalDiscountAmount = totalDiscountAmount;
                this.TotalAmountBeforeTax = totalAmountBeforeTax;
                this.RecipientContactTel = recipientContactTel;
                this.BillDiscountAmount = billDiscountAmount;
                this.CustomFieldValueThree = customFieldValueThree;
                this.BillTaxAmount = billTaxAmount;
                this.ServiceChargeAmountAfterTax = serviceChargeAmountAfterTax;
                this.PostStatus = postStatus;
                this.ExternalRefId = externalRefId;
                this.SalesItems = salesItems;
            }

            public int SalesNo { get; set; }
            public double TotalAmount { get; set; }
            public double DiscountAmount { get; set; }
            public double RoundingAmount { get; set; }
            public double MgstTaxAmount { get; set; }
            public double BillTaxPercentage { get; set; }
            public double ServiceChargeAmount { get; set; }
            public string CustomerId { get; set; }
            public string Recipient { get; set; }
            public string SalesOutlet { get; set; }
            public string SalesType { get; set; }
            public string OrderNo { get; set; }
            public int PaxNo { get; set; }
            public string SalesPerson { get; set; }
            public string Remark { get; set; }
            public bool IsPrint { get; set; }
            public string dateTime { get; set; }
            public string OrderSource { get; set; }
            public string PickupTime { get; set; }
            public string RecipientContact { get; set; }
            public string PromoCode { get; set; }
            public double TotalDiscountAmount { get; set; }
            public double TotalAmountBeforeTax { get; set; }
            public string RecipientContactTel { get; set; }
            public double BillDiscountAmount { get; set; }
            public string CustomFieldValueThree { get; set; }
            public double BillTaxAmount { get; set; }
            public double ServiceChargeAmountAfterTax { get; set; }
            public bool PostStatus { get; set; }
            public string ExternalRefId { get; set; }
            public List<SalesItem> SalesItems { get; set; } = new List<SalesItem>();

            public class SalesItem
            {
                public SalesItem(int salesItemId, int itemId, string itemCode, int quantity, int shippedQuantity, double price, double discountPercentage, double discountAmount, bool isPrint, double subTotal, double mgstTaxAmount, double totalTaxAmount, bool isInclusiveMgst, bool isServiceItem, double additionalTaxPercentage1, double additionalTaxPercentage2, double additionalTaxAmount1, double additionalTaxAmount2, bool isVoucherItem, bool isPromoDiscountItem, List<Modifier> modifiers)
                {
                    SalesItemId = salesItemId;
                    ItemId = itemId;
                    ItemCode = itemCode;
                    Quantity = quantity;
                    ShippedQuantity = shippedQuantity;
                    Price = price;
                    DiscountPercentage = discountPercentage;
                    DiscountAmount = discountAmount;
                    IsPrint = isPrint;
                    SubTotal = subTotal;
                    MgstTaxAmount = mgstTaxAmount;
                    TotalTaxAmount = totalTaxAmount;
                    IsInclusiveMgst = isInclusiveMgst;
                    IsServiceItem = isServiceItem;
                    AdditionalTaxPercentage1 = additionalTaxPercentage1;
                    AdditionalTaxPercentage2 = additionalTaxPercentage2;
                    AdditionalTaxAmount1 = additionalTaxAmount1;
                    AdditionalTaxAmount2 = additionalTaxAmount2;
                    IsVoucherItem = isVoucherItem;
                    IsPromoDiscountItem = isPromoDiscountItem;
                    Modifiers = modifiers;
                }

                public int SalesItemId { get; set; }
                public int ItemId { get; set; }
                public string ItemCode { get; set; }
                public int Quantity { get; set; }
                public int ShippedQuantity { get; set; }
                public double Price { get; set; }
                public double DiscountPercentage { get; set; }
                public double DiscountAmount { get; set; }
                public bool IsPrint { get; set; }
                public double SubTotal { get; set; }
                public double MgstTaxAmount { get; set; }
                public double TotalTaxAmount { get; set; }
                public bool IsInclusiveMgst { get; set; }
                public bool IsServiceItem { get; set; }
                public double AdditionalTaxPercentage1 { get; set; }
                public double AdditionalTaxPercentage2 { get; set; }
                public double AdditionalTaxAmount1 { get; set; }
                public double AdditionalTaxAmount2 { get; set; }
                public bool IsVoucherItem { get; set; }
                public bool IsPromoDiscountItem { get; set; }
                public List<Modifier> Modifiers { get; set; } = new List<Modifier>();
            }

            public class Modifier
            {
                public double Subtotal { get; set; }
                public int SalesItemId { get; set; }
                public int ItemId { get; set; }
                public string ItemCode { get; set; }
                public int Quantity { get; set; }
                public int ShippedQuantity { get; set; }
                public double Price { get; set; }
                public double DiscountPercentage { get; set; }
                public double DiscountAmount { get; set; }
                public bool IsPrint { get; set; }
                public double SubTotal { get; set; }
                public double MgstTaxAmount { get; set; }
                public double TotalTaxAmount { get; set; }
                public bool IsInclusiveMgst { get; set; }
                public bool IsServiceItem { get; set; }
                public double AdditionalTaxPercentage1 { get; set; }
                public double AdditionalTaxPercentage2 { get; set; }
                public double AdditionalTaxAmount1 { get; set; }
                public double AdditionalTaxAmount2 { get; set; }
                public bool IsVoucherItem { get; set; }
                public bool IsPromoDiscountItem { get; set; }
            }

        }
        public class Response
        {
            public string Ok { get; set; }
            public string Status { get; set; }
            public string Warning { get; set; }
            public string Error { get; set; }

            public Data data { get; set; }
            public class Data
            {
                public int SalesNo { get; set; }
                public string SalesDate { get; set; }
                public string SalesTime { get; set; }
                public double TotalAmount { get; set; }
                public double RoundingAmount { get; set; }
                public double MgstTaxAmount { get; set; }
                public double BillTaxPercentage { get; set; }
                public double ServiceChargeAmount { get; set; }
                public string CustomerId { get; set; }
                public string CustomerName { get; set; }
                public string Recipient { get; set; }
                public string SalesStatus { get; set; }
                public string SalesOutlet { get; set; }
                public string SalesType { get; set; }
                public string OrderNo { get; set; }
                public int PaxNo { get; set; }
                public string SalesPerson { get; set; }
                public string Remark { get; set; }
                public bool IsPrint { get; set; }
                public string OrderSource { get; set; }
                public string OrderTerminalId { get; set; }
                public string OrderStatus { get; set; }
                public DateTime PickupTime { get; set; }
                public DateTime SalesDateTime { get; set; }
                public string RecipientContact { get; set; }
                public string PromoCode { get; set; }
                public string[] PromoCodeCollection { get; set; }
                public double TotalDiscountAmount { get; set; }
                public double TotalAmountBeforeTax { get; set; }
                public ShippingAddress ShippingAddress { get; set; }
                public string ShippingRemark { get; set; }
                public string RecipientContactTel { get; set; }
                public string ShipmentProvider { get; set; }
                public string TrackingLink { get; set; }
                public string TrackingNumber { get; set; }
                public string DeliveryType { get; set; }
                public string OrderId { get; set; }
                public double BillDiscountAmount { get; set; }
                public double BillTaxAmount { get; set; }
                public string PromoIdentifier { get; set; }
                public string ShipmentDateTime { get; set; }
                public string BusinessDate { get; set; }
                public string CustomFieldValueThree { get; set; }
                public string CustomFieldValueFour { get; set; }
                public string CustomFieldValueFive { get; set; }
                public double ServiceChargeAmountAfterTax { get; set; }
                public string SubSalesType { get; set; }
                public string BillingRemark { get; set; }
                public string ExternalDocumentId { get; set; }
                public bool PostStatus { get; set; }
                public string IntegrationModule { get; set; }
                public string DocumentType { get; set; }
                public string ExternalRefId { get; set; }
                public string ExternalRefId2 { get; set; }
                public string ExternalRefId3 { get; set; }
                public List<SalesItem> SalesItems { get; set; }
                public List<Collections> Collections { get; set; }
                public string PaymentFlowType { get; set; }
                public PaymentPromo PaymentForPromo { get; set; }
                public Shipments Shipments { get; set; }
                public string IntegrationCustomField1 { get; set; }
                public string IntegrationCustomField2 { get; set; }
                public string IntegrationCustomField3 { get; set; }
                public string IntegrationCustomField4 { get; set; }
                public string IntegrationCustomField5 { get; set; }
                public string IntegrationCustomField6 { get; set; }
                public string IntegrationCustomField7 { get; set; }
                public string IntegrationCustomField8 { get; set; }
                public string IntegrationCustomField9 { get; set; }
                public string IntegrationCustomField10 { get; set; }
                public PromoCodeStatus ListPromoCodeStatus { get; set; }
                public string PickUpBarcode { get; set; }
                public string KdsEstimatedWaitingTime { get; set; }
                public string ReturnSalesID { get; set; }
            }

            public class ShippingAddress
            {
                public string Street { get; set; }
                public string City { get; set; }
                public string State { get; set; }
                public string Zipcode { get; set; }
                public string Country { get; set; }
            }
            public class SalesItem
            {
                public int SalesItemId { get; set; }
                public int ItemId { get; set; }
                public string ItemCode { get; set; }
                public string ItemName { get; set; }
                public double Quantity { get; set; }
                public double ShippedQuantity { get; set; }
                public double Price { get; set; }
                public double DiscountPercentage { get; set; }
                public double DiscountAmount { get; set; }
                public string Remark { get; set; }
                public bool IsPrint { get; set; }
                public string SalesPerson { get; set; }
                public double SubTotal { get; set; }
                public double MgstTaxAmount { get; set; }
                public double TotalTaxAmount { get; set; }
                public bool IsInclusiveMgst { get; set; }
                public string OrderSource { get; set; }
                public double MgstTaxPercentage { get; set; }
                public bool IsServiceItem { get; set; }
                public string DeliveryType { get; set; }
                public string DiscountRemark { get; set; }
                public string Brand { get; set; }
                public string ItemType { get; set; }
                public string RuleName { get; set; }
                public string CustomField1 { get; set; }
                public string CustomField2 { get; set; }
                public string CustomField3 { get; set; }
                public string CustomField4 { get; set; }
                public string CustomField5 { get; set; }
                public string CustomField6 { get; set; }
                public string CustomField7 { get; set; }
                public string CustomField8 { get; set; }
                public string CustomField9 { get; set; }
                public string CustomField10 { get; set; }
                public string CustomField11 { get; set; }
                public string CustomField12 { get; set; }
                public string CustomField13 { get; set; }
                public string CustomField14 { get; set; }
                public string CustomField15 { get; set; }
                public string CustomField16 { get; set; }
                public string CustomField17 { get; set; }
                public string CustomField18 { get; set; }
                public double AdditionalTaxPercentage1 { get; set; }
                public double AdditionalTaxPercentage2 { get; set; }
                public double AdditionalTaxAmount1 { get; set; }
                public double AdditionalTaxAmount2 { get; set; }
                public string[] ItemImageURL { get; set; }
                public string ScanCode { get; set; }
                public bool IsVoucherItem { get; set; }
                public string MatrixBarcode { get; set; }
                public string PromoCode { get; set; }
                public bool IsPromoDiscountItem { get; set; }
                public List<Modifier> Modifiers { get; set; }
            }

            public class Modifier
            {
                public double Subtotal { get; set; }
                public int SalesItemId { get; set; }
                public int ItemId { get; set; }
                public string ItemCode { get; set; }
                public string ItemName { get; set; }
                public double Quantity { get; set; }
                public double ShippedQuantity { get; set; }
                public double Price { get; set; }
                public double DiscountPercentage { get; set; }
                public double DiscountAmount { get; set; }
                public bool IsPrint { get; set; }
                public string SalesPerson { get; set; }
                public double SubTotal { get; set; }
                public double MgstTaxAmount { get; set; }
                public double TotalTaxAmount { get; set; }
                public bool IsInclusiveMgst { get; set; }
                public string OrderSource { get; set; }
                public double MgstTaxPercentage { get; set; }
                public bool IsServiceItem { get; set; }
                public string DeliveryType { get; set; }
                public string DiscountRemark { get; set; }
                public string Brand { get; set; }
                public string ItemType { get; set; }
                public string RuleName { get; set; }
                public string CustomField1 { get; set; }
                public string CustomField2 { get; set; }
                public string CustomField3 { get; set; }
                public string CustomField4 { get; set; }
                public string CustomField5 { get; set; }
                public string CustomField6 { get; set; }
                public string CustomField7 { get; set; }
                public string CustomField8 { get; set; }
                public string CustomField9 { get; set; }
                public string CustomField10 { get; set; }
                public string CustomField11 { get; set; }
                public string CustomField12 { get; set; }
                public string CustomField13 { get; set; }
                public string CustomField14 { get; set; }
                public string CustomField15 { get; set; }
                public string CustomField16 { get; set; }
                public string CustomField17 { get; set; }
                public string CustomField18 { get; set; }
                public double AdditionalTaxPercentage1 { get; set; }
                public double AdditionalTaxPercentage2 { get; set; }
                public double AdditionalTaxAmount1 { get; set; }
                public double AdditionalTaxAmount2 { get; set; }
                public string[] ItemImageURL { get; set; }
                public string ScanCode { get; set; }
                public bool IsVoucherItem { get; set; }
                public string MatrixBarcode { get; set; }
                public string PromoCode { get; set; }
                public bool IsPromoDiscountItem { get; set; }
            }

            public class Collections
            {
                public int Id { get; set; }
                public string ClientId { get; set; }
                public string InvoiceId { get; set; }
                public double Amount { get; set; }
                public string Method { get; set; }
                public string Reference { get; set; }
                public string OutletId { get; set; }
                public string PaymentDate { get; set; }
                public bool IsVoid { get; set; }
                public int CreditCardRate { get; set; }
                public int SiteId { get; set; }
                public string CardAppCode { get; set; }
                public string CardType { get; set; }
                public string Status { get; set; }
                public string ReceiveBy { get; set; }
                public string CardExpiry { get; set; }
                public string TraceNumber { get; set; }
                public string Remark { get; set; }
                public double TenderAmount { get; set; }
                public double Change { get; set; }
                public int DeclarationSessionId { get; set; }
                public int EodLogId { get; set; }
                public bool IsDeposit { get; set; }
                public string SalesOrderId { get; set; }
                public string CardType2 { get; set; }
                public string CardType3 { get; set; }
                public string BusinessDate { get; set; }
                public string InternalReferenceId { get; set; }
                public double AvailableBalance { get; set; }
                public string UsedData { get; set; }
                public string PrepaidCardNumber { get; set; }
                public string PrepaidReferenceNumber { get; set; }
                public double ExchangeRate { get; set; }
                public string CurrencyCode { get; set; }
                public double ForeignAmount { get; set; }
                public string ForeignGain { get; set; }
                public string CardLookup { get; set; }
                public string ReceiveByCashierName { get; set; }
                public string DeviceName { get; set; }
                public string ExternalRefID { get; set; }
            }

            public class PaymentPromo
            {
                public string Method { get; set; }
                public string Type { get; set; }
            }
            public class Shipments
            {
                public string TrackingCode { get; set; }
            }
            public class PromoCodeStatus
            {
                public string PromoCode { get; set; }
                public string Status { get; set; }
                public string Message { get; set; }
            }
        }
    }
}
