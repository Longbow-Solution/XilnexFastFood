using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using LFFSSK.API;
using LFFSSK;
using System.Windows;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.IO;

namespace LFFSSK.Model
{
    public class CartModel : Base
    {
        public List<Product> orderMenuList { get; set; }

        public class Product : Base
        {
            string _TraceCategory = "LFFSSK.Model.CartModel.Product";
            public int cartMenuNo { get; set; }
            public double DOUBLE_Sale_Price { get; set; }
            public double DOUBLE_Employee_Price { get; set; }
            public double DOUBLE_Wholesale_Price { get; set; }
            public double DOUBLE_Custom_Price { get; set; }
            public double DOUBLE_Manufacturer_Suggested_Retail_Price { get; set; }
            public double DOUBLE_Web_Price { get; set; }
            public double DOUBLE_Web_Dealer_Price { get; set; }
            public string customField1 { get; set; }
            public string customField2 { get; set; }
            public string customField3 { get; set; }
            public string customField4 { get; set; }
            public string customField5 { get; set; }
            public string customField6 { get; set; }
            public string customField7 { get; set; }
            public string customField8 { get; set; }
            public string customField9 { get; set; }
            public string customField10 { get; set; }
            public string customField11 { get; set; }
            public string customField12 { get; set; }
            public string customField13 { get; set; }
            public string customField14 { get; set; }
            public string customField15 { get; set; }
            public string customField16 { get; set; }
            public string customField17 { get; set; }
            public string customField18 { get; set; }
            public string itemName { get; set; }
            public string colorHex { get; set; }
            public string dynamicHeaderLabel { get; set; }
            public List<DynamicModifier> dynamicmodifiers { get; set; }
            public int itemId { get; set; }
            public string itemCode { get; set; }
            public string itemType { get; set; }
            public double price { get; set; }
            public List<string> imageUrlList { get; set; }
            public string binaryImageUrl { get; set; }
            public string imageUrl { get; set; }
            public string description { get; set; }
            public string size { get; set; }
            public bool boolOpenPrice { get; set; }
            public string stockType { get; set; }
            public bool IsAssortment { get; set; }
            public bool HasStock { get; set; }
            public bool AllowToSell { get; set; }
            public bool IsActive { get; set; }
            public Product(int CartMenuNo, ObservableCollection<Modifier> modifierlist,double doubleSalePrice, double doubleEmployeePrice, double doubleWholesalePrice, double doubleCustomPrice, double doubleManuSugRetailPrice, double doubleWebPrice, double doubleWebDealerPrice, string CustomField1, string CustomField2, string CustomField3, string CustomField4, string CustomField5, string CustomField6, string CustomField7, string CustomField8, string CustomField9, string CustomField10, string CustomField11, string CustomField12, string CustomField13, string CustomField14, string CustomField15, string CustomField16, string CustomField17, string CustomField18, string ItemName, string ColorHex, string DynamicHeaderLabel, List<DynamicModifier> DynamicModifiers, int ItemId, string ItemCode, string ItemType, double Price, List<string> ImageUrlList, string BinaryImageUrl, string ImageUrl, string Description, string Size, bool BoolOpenPrice, string StockType, bool isAssortment, bool hasStock, bool allowToSell, bool isActive, BitmapImage menuImagePath, string menuImage, int itemCurrentQty, double itemTotalPrice, double tempTotalPrice)
            {
                cartMenuNo = CartMenuNo;
                ModifierList = modifierlist;
                DOUBLE_Sale_Price = doubleSalePrice;
                DOUBLE_Employee_Price = doubleEmployeePrice;
                DOUBLE_Wholesale_Price = doubleWholesalePrice;
                DOUBLE_Custom_Price = doubleCustomPrice;
                DOUBLE_Manufacturer_Suggested_Retail_Price = doubleManuSugRetailPrice;
                DOUBLE_Web_Price = doubleWebPrice;
                DOUBLE_Web_Dealer_Price = doubleWebDealerPrice;
                customField1 = CustomField1;
                customField2 = CustomField2;
                customField3 = CustomField3;
                customField4 = CustomField4;
                customField5 = CustomField5;
                customField6 = CustomField6;
                customField7 = CustomField7;
                customField8 = CustomField8;
                customField9 = CustomField9;
                customField10 = CustomField10;
                customField11 = CustomField11;
                customField12 = CustomField12;
                customField13 = CustomField13;
                customField14 = CustomField14;
                customField15 = CustomField15;
                customField16 = CustomField16;
                customField17 = CustomField17;
                customField18 = CustomField18;
                itemName = ItemName;
                colorHex = ColorHex;
                dynamicHeaderLabel = DynamicHeaderLabel;
                dynamicmodifiers = DynamicModifiers;
                itemId = ItemId;
                itemCode = ItemCode;
                itemType = ItemType;
                price = Price;
                imageUrlList = ImageUrlList;
                binaryImageUrl = BinaryImageUrl;
                imageUrl = ImageUrl;
                description = Description;
                size = Size;
                boolOpenPrice = BoolOpenPrice;
                stockType = StockType;
                IsAssortment = isAssortment;
                HasStock = hasStock;
                AllowToSell = allowToSell;
                IsActive = isActive;
                MenuImagePath = menuImagePath;
                MenuImage = menuImage;
                ItemCurrentQty = itemCurrentQty;
                ItemTotalPrice = itemTotalPrice;
                TempTotalPrice = tempTotalPrice;
            }
            public BitmapImage MenuImagePath { get; set; }

            private string _MenuImage;
            public string MenuImage
            {
                get { return _MenuImage; }
                set
                {
                    _MenuImage = value;
                    OnPropertyChanged(nameof(MenuImage));
                }
            }

            private Visibility _ExpVisibility;

            public Visibility ExpVisibility
            {
                get 
                {
                    if (ModifierList.Count() == 0 || ModifierList == null)
                        _ExpVisibility = Visibility.Collapsed;
                    else
                        _ExpVisibility = Visibility.Visible;
                    return _ExpVisibility; 
                }
                set 
                { 
                    _ExpVisibility = value;
                    OnPropertyChanged(nameof(ExpVisibility));
                    OnPropertyChanged(nameof(ModifierList));
                }
            }



            private int _ItemCurrentQty;
            public string Trash => "\\Resource\\LFFImages\\trash.png";
            public string EditImg => "\\Resource\\LFFImages\\Edit.png";
            public int ItemCurrentQty
            {
                get
                {
                    if (_ItemCurrentQty == 1)
                        ButtonEnable = false;
                    else
                        ButtonEnable = true;
                    return _ItemCurrentQty;
                }
                set
                {
                    _ItemCurrentQty = value;
                    OnPropertyChanged(nameof(ItemCurrentQty));
                    OnPropertyChanged(nameof(ItemTotalPrice));
                    GeneralVar.MainWindowVM.CalculateTotalItem();
                }
            }
            private double _ItemTotalPrice;

            public double ItemTotalPrice
            {
                get { return ItemCurrentQty * TempTotalPrice; }
                set
                {
                    _ItemTotalPrice = value;
                    OnPropertyChanged(nameof(ItemTotalPrice));
                }
            }

            private double _TempTotalPrice;

            public double TempTotalPrice
            {
                get { return _TempTotalPrice; }
                set
                {
                    _TempTotalPrice = value;
                    OnPropertyChanged(nameof(TempTotalPrice));
                }
            }

            private bool _ButtonEnable;

            public bool ButtonEnable
            {
                get { return _ButtonEnable; }
                set
                {
                    _ButtonEnable = value;
                    OnPropertyChanged(nameof(ButtonEnable));
                }
            }

            private ObservableCollection<Modifier> _ModifierList;

            public ObservableCollection<Modifier> ModifierList
            {
                get { return _ModifierList; }
                set 
                { 
                    _ModifierList = value;
                    OnPropertyChanged(nameof(ModifierList));
                }
            }


            public string EditLabel => GeneralVar.MainWindowVM.Lbl_Edit;

            private ICommand _BtnGetMenuDetails;
            public ICommand BtnGetMenuDetails
            {
                get
                {
                    if (_BtnGetMenuDetails == null)
                        _BtnGetMenuDetails = new RelayCommand<int>(RetrieveMenuDetails);
                    return _BtnGetMenuDetails;
                }
            }

            private ICommand _BtnAddQty;
            public ICommand BtnAddQty
            {
                get
                {
                    if (_BtnAddQty == null)
                        _BtnAddQty = new RelayCommand(AddQty);
                    return _BtnAddQty;
                }
            }
            
            private ICommand _BtnDeleteItem;
            public ICommand BtnDeleteItem
            {
                get
                {
                    if (_BtnDeleteItem == null)
                        _BtnDeleteItem = new RelayCommand(DeleteItem);
                    return _BtnDeleteItem;
                }
            }
            private ICommand _BtnEditItem;
            public ICommand BtnEditItem
            {
                get
                {
                    if (_BtnEditItem == null)
                        _BtnEditItem = new RelayCommand<int>(EditItemAction);
                    return _BtnEditItem;
                }
            }

            
            private ICommand _BtnDeductQty;
            public ICommand BtnDeductQty
            {
                get
                {
                    if (_BtnDeductQty == null)
                        _BtnDeductQty = new RelayCommand(DeductQty);
                    return _BtnDeductQty;
                }
            }

            private void RetrieveMenuDetails(int itemId)
            {
                try
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger RetrieveMenuDetails Starting ..."), _TraceCategory);
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Getting MenuDetails for [ {0} ]", this.itemName), _TraceCategory);

                    GeneralVar.MainWindowVM.RetrieveMenuDetails(itemId);

                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger RetrieveMenuDetails Done ..."), _TraceCategory);
                }
                catch (Exception ex)
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger RetrieveMenuDetails : {0}", ex.ToString()), _TraceCategory);
                }
            }

            private void AddQty()
            {
                try
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger AddQty Starting ..."), _TraceCategory);

                    ItemCurrentQty++;
                    OnPropertyChanged("ItemTotalPrice");
                    //int currentQty = GeneralVar.MainWindowVM.CartList.Where(x => x.itemId == itemId).FirstOrDefault().ItemCurrentQty;

                    ////GeneralVar.MainWindowVM.MenuDetails.Where(x => x.itemId == itemId).FirstOrDefault().ItemCurrentQty += 1;

                    ////this.ItemCurrentQty += 1;

                    //foreach (var item in GeneralVar.MainWindowVM.CartList.Where(x => x.itemId == itemId))
                    //{
                    //    item.ItemCurrentQty++;
                    //    item.ItemTotalPrice = item.TempTotalPrice * item.ItemCurrentQty;
                    //}

                    //Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Adding qty for [ {0} ] from [ {1} ] to [ {2} ]", this.itemName, currentQty, this.ItemCurrentQty), _TraceCategory);
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger AddQty Done ..."), _TraceCategory);
                }
                catch (Exception ex)
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] Trigger AddQty : {0}", ex.ToString()), _TraceCategory);
                }
            }

            private void DeductQty()
            {
                try
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger RetrieveMenuDetails Starting ..."), _TraceCategory);

                    if(this.ItemCurrentQty>1)
                    {
                        ItemCurrentQty--;
                        OnPropertyChanged("ItemTotalPrice");

                        //if (GeneralVar.MainWindowVM.CustomerID != 0)
                        //    GeneralVar.MainWindowVM.VoucherCheck();
                        //else
                        //    GeneralVar.MainWindowVM.CalculateTotalItem();
                    }

                    //int currentQty = GeneralVar.MainWindowVM.CartList.Where(x => x.itemId == this.itemId).FirstOrDefault().ItemCurrentQty;

                    //if (currentQty > 1)
                    //{
                    //    foreach (var item in GeneralVar.MainWindowVM.CartList.Where(x => x.itemId == this.itemId))
                    //    {
                    //        item.ItemCurrentQty--;
                    //        item.ItemTotalPrice = item.TempTotalPrice * item.ItemCurrentQty;
                    //    }
                    //}

                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Deduct qty for [ {0} ] from [ {1} ] to [ {2} ]", this.itemName, this.ItemCurrentQty, this.ItemCurrentQty), _TraceCategory);
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger RetrieveMenuDetails Done ..."), _TraceCategory);
                }
                catch (Exception ex)
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("{Error] Trigger RetrieveMenuDetails : {0}", ex.ToString()), _TraceCategory);
                }
            }

            public void DeleteItem()
            {
                try
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger DeleteItem Starting ..."), _TraceCategory);

                    int itemID = GeneralVar.MainWindowVM.CartList.Where(x => x.cartMenuNo == this.cartMenuNo).Select(y=>y.itemId).FirstOrDefault();
                    //int redeemId = GeneralVar.MainWindowVM.CartList.Where(x => x.cartMenuNo == this.cartMenuNo).Select(y => y.redeemId).FirstOrDefault();

                    //if (redeemId != 0)
                    //{
                    //    GeneralVar.MainWindowVM.Voucher.Remove(GeneralVar.MainWindowVM.Voucher.Single(y => y.rewardSuperId == redeemId));
                    //    //GeneralVar.MainWindowVM.VoucherList.Where(y => Convert.ToInt32(y.RedeemId) == redeemId).Select(z => z.IsCheck = false).ToList();
                    //}

                    GeneralVar.MainWindowVM.CartList.Remove(GeneralVar.MainWindowVM.CartList.Single(x => x.cartMenuNo == this.cartMenuNo));

                    //if (GeneralVar.MainWindowVM.AddOnSum.Where(x => x.itemId == itemID).Count() > 0)
                        //GeneralVar.MainWindowVM.AddOnSum.Where(x => x.itemId == itemID).Select(y => y.IsCheck = false).ToList();

                    //if (GeneralVar.MainWindowVM.CustomerID != 0)
                    //    GeneralVar.MainWindowVM.VoucherCheck();
                    //else
                        GeneralVar.MainWindowVM.CalculateTotalItem();

                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger DeleteItem Done ..."), _TraceCategory);
                }
                catch (Exception ex)
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("{Error] Trigger DeleteItem : {0}", ex.ToString()), _TraceCategory);
                }
            }

            private void EditItemAction(int queueNo)
            {
                try
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger EditItem Starting ..."), _TraceCategory);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        GeneralVar.MainWindowVM.EditOrderCart(queueNo);

                        //if (GeneralVar.MainWindowVM.CustomerID != 0)
                        //    GeneralVar.MainWindowVM.VoucherCheck();
                    }));

                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger EditItem Done ..."), _TraceCategory);
                }
                catch (Exception ex)
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("{Error] EditItem DeleteItem : {0}", ex.ToString()), _TraceCategory);
                }
            }
        }
        public class DynamicModifier : Base
        {
            public double DOUBLE_Sale_Price { get; set; }
            public double DOUBLE_Employee_Price { get; set; }
            public double DOUBLE_Wholesale_Price { get; set; }
            public double DOUBLE_Custom_Price { get; set; }
            public double DOUBLE_Manufacturer_Suggested_Retail_Price { get; set; }
            public double DOUBLE_Web_Price { get; set; }
            public double DOUBLE_Web_Dealer_Price { get; set; }
            public string customField1 { get; set; }
            public string customField2 { get; set; }
            public string customField3 { get; set; }
            public string customField4 { get; set; }
            public string customField5 { get; set; }
            public string customField6 { get; set; }
            public string customField7 { get; set; }
            public string customField8 { get; set; }
            public string customField9 { get; set; }
            public string customField10 { get; set; }
            public string customField11 { get; set; }
            public string customField12 { get; set; }
            public string customField13 { get; set; }
            public string customField14 { get; set; }
            public string customField15 { get; set; }
            public string customField16 { get; set; }
            public string customField17 { get; set; }
            public string customField18 { get; set; }
            public string itemName { get; set; }
            public string itemShortName { get; set; }
            public int defaultSelected { get; set; }
            public List<Modifier> modifiers { get; set; }
            public int itemId { get; set; }
            public string itemCode { get; set; }
            public string itemType { get; set; }
            public double price { get; set; }
            public List<string> imageUrlList { get; set; }
            public string binaryImageUrl { get; set; }
            public string imageUrl { get; set; }
            public string description { get; set; }
            public string size { get; set; }
            public bool boolOpenPrice { get; set; }
            public string stockType { get; set; }
            public bool IsAssortment { get; set; }
            public bool HasStock { get; set; }
            public bool AllowToSell { get; set; }
            public bool IsActive { get; set; }
            public BitmapImage MenuImagePath { get; set; }

            private string _MenuImage;
            public string MenuImage
            {
                get { return _MenuImage; }
                set
                {
                    _MenuImage = value;
                    OnPropertyChanged(nameof(MenuImage));
                }
            }

            public void RefreshMenuImage()
            {
                if (MenuImage == null)
                {
                    MenuImagePath = new BitmapImage();
                    return;
                }

                string posterPath = "";
                try
                {
                    string poster = Path.Combine(GeneralVar.MenuRepository, MenuImage);

                    MenuImagePath = new BitmapImage();
                    MenuImagePath.BeginInit();
                    MenuImagePath.CacheOption = BitmapCacheOption.OnLoad;
                    MenuImagePath.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    MenuImagePath.UriSource = new Uri(poster, UriKind.Absolute);

                    MenuImagePath.EndInit();

                    if (MenuImagePath.CanFreeze)
                        MenuImagePath.Freeze();
                }
                catch (Exception)
                {
                    MenuImagePath = new BitmapImage();
                    MenuImagePath.BeginInit();
                    MenuImagePath.CacheOption = BitmapCacheOption.OnLoad;
                    MenuImagePath.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    MenuImagePath.UriSource = new Uri(posterPath, UriKind.Absolute);

                    MenuImagePath.EndInit();

                    if (MenuImagePath.CanFreeze)
                        MenuImagePath.Freeze();
                }
                GC.Collect();
            }
        }

        public class Modifier
        {
            public string type { get; set; }
            public int minSelection { get; set; }
            public int maxSelection { get; set; }
            public string groupId { get; set; }
            public string groupName { get; set; }
            public List<Selection> selections { get; set; }
        }

        public class Selection : Base
        {
            string _TraceCategory = "LFFSSK.Model.ApiModel.Product.DynamicModifier.Modifier";
            public int minSelection { get; set; }
            public int maxSelection { get; set; }
            public string groupId { get; set; }
            public string name { get; set; }
            public int defaultSelected { get; set; }
            public int itemId { get; set; }
            public string itemCode { get; set; }
            public string itemType { get; set; }
            public double price { get; set; }
            public double DOUBLE_Sale_Price { get; set; }
            public double DOUBLE_Employee_Price { get; set; }
            public double DOUBLE_Wholesale_Price { get; set; }
            public double DOUBLE_Custom_Price { get; set; }
            public double DOUBLE_Manufacturer_Suggested_Retail_Price { get; set; }
            public double DOUBLE_Web_Price { get; set; }
            public double DOUBLE_Web_Dealer_Price { get; set; }
            public List<string> imageUrlList { get; set; }
            public string binaryImageUrl { get; set; }
            public string imageUrl { get; set; }
            public string description { get; set; }
            public string customField1 { get; set; }
            public string customField2 { get; set; }
            public string customField3 { get; set; }
            public string customField4 { get; set; }
            public string customField5 { get; set; }
            public string customField6 { get; set; }
            public string customField7 { get; set; }
            public string customField8 { get; set; }
            public string customField9 { get; set; }
            public string customField10 { get; set; }
            public string customField11 { get; set; }
            public string customField12 { get; set; }
            public string customField13 { get; set; }
            public string customField14 { get; set; }
            public string customField15 { get; set; }
            public string customField16 { get; set; }
            public string customField17 { get; set; }
            public string customField18 { get; set; }
            public string size { get; set; }
            public bool boolOpenPrice { get; set; }
            public string stockType { get; set; }
            public bool IsAssortment { get; set; }
            public bool HasStock { get; set; }
            public bool AllowToSell { get; set; }
            public bool IsActive { get; set; }
            public BitmapImage MenuImagePath { get; set; }
            public int parentItemId { get; set; }
            public bool IsEnable { get; set; }

            private string _MenuImage;
            public string MenuImage
            {
                get { return _MenuImage; }
                set
                {
                    _MenuImage = value;
                    OnPropertyChanged(nameof(MenuImage));
                }
            }

            public void RefreshMenuImage()
            {
                if (MenuImage == null)
                {
                    MenuImagePath = new BitmapImage();
                    return;
                }

                string posterPath = "";
                try
                {
                    string poster = Path.Combine(GeneralVar.MenuRepository, MenuImage);

                    MenuImagePath = new BitmapImage();
                    MenuImagePath.BeginInit();
                    MenuImagePath.CacheOption = BitmapCacheOption.OnLoad;
                    MenuImagePath.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    MenuImagePath.UriSource = new Uri(poster, UriKind.Absolute);

                    MenuImagePath.EndInit();

                    if (MenuImagePath.CanFreeze)
                        MenuImagePath.Freeze();
                }
                catch (Exception)
                {
                    MenuImagePath = new BitmapImage();
                    MenuImagePath.BeginInit();
                    MenuImagePath.CacheOption = BitmapCacheOption.OnLoad;
                    MenuImagePath.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    MenuImagePath.UriSource = new Uri(posterPath, UriKind.Absolute);

                    MenuImagePath.EndInit();

                    if (MenuImagePath.CanFreeze)
                        MenuImagePath.Freeze();
                }
                GC.Collect();
            }

            private bool _IsCheck;
            public bool IsCheck
            {
                get { return _IsCheck; }
                set
                {
                    _IsCheck = value;
                    OnPropertyChanged(nameof(IsCheck));
                }
            }

            private ICommand _BtnModifierCheckAction;
            public ICommand BtnModifierCheckAction
            {
                get
                {
                    if (_BtnModifierCheckAction == null)
                        _BtnModifierCheckAction = new RelayCommand(ModifierCheck);
                    return _BtnModifierCheckAction;
                }
            }

            private ICommand _BtnModifierCheckTempAction;
            public ICommand BtnModifierCheckTempAction
            {
                get
                {
                    if (_BtnModifierCheckTempAction == null)
                        _BtnModifierCheckTempAction = new RelayCommand(ModifierCheckTemp);
                    return _BtnModifierCheckTempAction;
                }
            }

            private void ModifierCheck()
            {
                try
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger ModifierCheck Starting ..."), _TraceCategory);

                    int maxValue = GeneralVar.MainWindowVM.CartList
                    .SelectMany(menuDetail => menuDetail.dynamicmodifiers)
                    .SelectMany(dynamicMod => dynamicMod.modifiers)// Flatten the list of lists into a single list
                    .Where(modifierType => modifierType.groupId == this.groupId) // Filter by the current modifierId
                    .FirstOrDefault().maxSelection;

                    int minValue = GeneralVar.MainWindowVM.CartList
                    .SelectMany(menuDetail => menuDetail.dynamicmodifiers)
                    .SelectMany(dynamicMod => dynamicMod.modifiers)// Flatten the list of lists into a single list
                    .Where(modifierType => modifierType.groupId == this.groupId) // Filter by the current modifierId
                    .FirstOrDefault().minSelection;

                    int checkTotal = GeneralVar.MainWindowVM.CartList
                    .SelectMany(menuDetail => menuDetail.dynamicmodifiers)
                    .SelectMany(dynamicMod => dynamicMod.modifiers)// Flatten the list of modifier types
                    .Where(mod => mod.groupId == this.groupId) // Filter by parentModifierId
                    .SelectMany(modifierType => modifierType.selections) // Flatten the list of ModifiersDetails
                    .Where(modifierDetail => modifierDetail.IsCheck == true)
                    .Count();

                    double tempPrice = GeneralVar.MainWindowVM.CartList
                    .Where(x => x.itemId == this.parentItemId)
                    .FirstOrDefault().TempTotalPrice;

                    foreach (var modifier in GeneralVar.MainWindowVM.CartList
                    .SelectMany(menuDetail => menuDetail.dynamicmodifiers)
                    .SelectMany(dynamicMod => dynamicMod.modifiers)// Flatten the list of modifier types
                    .Where(modifierType => modifierType.groupId == this.groupId) // Filter by parentModifierId
                    .SelectMany(modifierType => modifierType.selections) // Flatten the list of ModifiersDetails
                    .Where(modifierDetail => modifierDetail.itemId == this.itemId))
                    {
                        if (modifier.IsCheck != false)
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("AddOn [ {0} ] is select : [ {1} ]", this.name, this.IsCheck), _TraceCategory);

                            if (checkTotal > maxValue)
                            {
                                foreach (var mod in GeneralVar.MainWindowVM.MenuDetails
                                .SelectMany(menuDetail => menuDetail.dynamicmodifiers)
                                .SelectMany(dynamicMod => dynamicMod.modifiers)// Flatten the list of modifier types
                                .Where(modifierType => modifierType.groupId == this.groupId) // Filter by parentModifierId
                                .SelectMany(modifierType => modifierType.selections))
                                {
                                    if (mod.IsCheck == true && mod.itemId != this.itemId)
                                    {
                                        tempPrice -= mod.price;
                                        mod.IsCheck = false;
                                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Untick {0} - {1}", mod.name, mod.IsCheck), _TraceCategory);
                                        break;
                                    }
                                }
                                //Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("CheckTotal [ {0} ] is bigger than MaxValue [ {1} ], item [ {2} ] is not able to select", checkTotal, maxValue, this.name), _TraceCategory);

                                //modifier.IsCheck = false;
                            }

                            //else
                            //{
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("CheckTotal [ {0} ] is smaller than MaxValue [ {1} ], item [ {2} ] is selected", checkTotal, maxValue, this.name), _TraceCategory);

                            modifier.IsCheck = true;
                            tempPrice += modifier.price;

                            //}
                        }
                        else
                        {
                            if (checkTotal > minValue || minValue == 0)
                            {
                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("AddOn [ {0} ] is select : [ {1} ]", this.name, this.IsCheck), _TraceCategory);
                                tempPrice -= modifier.price;
                            }
                            else
                            {
                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("AddOn [ {0} ] is select : [ {1} ] to remove but fail due to min item select {2}", this.name, this.IsCheck, minValue), _TraceCategory);
                                modifier.IsCheck = true;
                            }
                        }
                    }

                    foreach (var item in GeneralVar.MainWindowVM.CartList.Where(x => x.itemId == this.parentItemId))
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("New total price : [ {0} ] , Before AddOn [ {1} ] , AddOnPrice [ {2} ]", tempPrice, item.TempTotalPrice, this.price), _TraceCategory);

                        item.TempTotalPrice = tempPrice;
                        item.ItemTotalPrice = tempPrice * item.ItemCurrentQty;
                    }

                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger ModifierCheck Done ..."), _TraceCategory);
                }
                catch (Exception ex)
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("{Error] Trigger ModifierCheck : {0}", ex.ToString()), _TraceCategory);
                }

            }

            private void ModifierCheckTemp()
            {
                try
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger ModifierCheck Starting ..."), _TraceCategory);

                    int maxValue = GeneralVar.MainWindowVM.TempEditItem
                    .SelectMany(menuDetail => menuDetail.dynamicmodifiers)
                    .SelectMany(dynamicMod => dynamicMod.modifiers)// Flatten the list of lists into a single list
                    .Where(modifierType => modifierType.groupId == this.groupId) // Filter by the current modifierId
                    .FirstOrDefault().maxSelection;

                    int minValue = GeneralVar.MainWindowVM.TempEditItem
                    .SelectMany(menuDetail => menuDetail.dynamicmodifiers)
                    .SelectMany(dynamicMod => dynamicMod.modifiers)// Flatten the list of lists into a single list
                    .Where(modifierType => modifierType.groupId == this.groupId) // Filter by the current modifierId
                    .FirstOrDefault().minSelection;

                    int checkTotal = GeneralVar.MainWindowVM.TempEditItem
                    .SelectMany(menuDetail => menuDetail.dynamicmodifiers)
                    .SelectMany(dynamicMod => dynamicMod.modifiers)// Flatten the list of modifier types
                    .Where(mod => mod.groupId == this.groupId) // Filter by parentModifierId
                    .SelectMany(modifierType => modifierType.selections) // Flatten the list of ModifiersDetails
                    .Where(modifierDetail => modifierDetail.IsCheck == true)
                    .Count();

                    double tempPrice = GeneralVar.MainWindowVM.TempEditItem
                    .Where(x => x.itemId == this.parentItemId)
                    .FirstOrDefault().TempTotalPrice;

                    foreach (var modifier in GeneralVar.MainWindowVM.TempEditItem
                    .SelectMany(menuDetail => menuDetail.dynamicmodifiers)
                    .SelectMany(dynamicMod => dynamicMod.modifiers)// Flatten the list of modifier types
                    .Where(modifierType => modifierType.groupId == this.groupId) // Filter by parentModifierId
                    .SelectMany(modifierType => modifierType.selections) // Flatten the list of ModifiersDetails
                    .Where(modifierDetail => modifierDetail.itemId == this.itemId))
                    //{
                    //    if (modifier.IsCheck != false)
                    //    {
                    //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("AddOn [ {0} ] is select : [ {1} ]", this.name, this.IsCheck), _TraceCategory);

                    //        if (checkTotal > maxValue)
                    //        {
                    //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("CheckTotal [ {0} ] is bigger than MaxValue [ {1} ], item [ {2} ] is not able to select", checkTotal, maxValue, this.name), _TraceCategory);

                    //            modifier.IsCheck = false;
                    //        }
                    //        else
                    //        {
                    //            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("CheckTotal [ {0} ] is smaller than MaxValue [ {1} ], item [ {2} ] is selected", checkTotal, maxValue, this.name), _TraceCategory);

                    //            modifier.IsCheck = true;
                    //            tempPrice += modifier.price;

                    //        }
                    //    }
                    //    else
                    //    {
                    //        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("AddOn [ {0} ] is select : [ {1} ]", this.name, this.IsCheck), _TraceCategory);
                    //        tempPrice -= modifier.price;
                    //    }
                    //}
                    {
                        if (modifier.IsCheck != false)
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("AddOn [ {0} ] is select : [ {1} ]", this.name, this.IsCheck), _TraceCategory);

                            if (checkTotal > maxValue)
                            {
                                foreach (var mod in GeneralVar.MainWindowVM.TempEditItem
                                .SelectMany(menuDetail => menuDetail.dynamicmodifiers)
                                .SelectMany(dynamicMod => dynamicMod.modifiers)// Flatten the list of modifier types
                                .Where(modifierType => modifierType.groupId == this.groupId) // Filter by parentModifierId
                                .SelectMany(modifierType => modifierType.selections))
                                {
                                    if (mod.IsCheck == true && mod.itemId != this.itemId)
                                    {
                                        tempPrice -= mod.price;
                                        mod.IsCheck = false;
                                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Untick {0} - {1}", mod.name, mod.IsCheck), _TraceCategory);
                                        break;
                                    }
                                }
                                //Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("CheckTotal [ {0} ] is bigger than MaxValue [ {1} ], item [ {2} ] is not able to select", checkTotal, maxValue, this.name), _TraceCategory);

                                //modifier.IsCheck = false;
                            }

                            //else
                            //{
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("CheckTotal [ {0} ] is smaller than MaxValue [ {1} ], item [ {2} ] is selected", checkTotal, maxValue, this.name), _TraceCategory);

                            modifier.IsCheck = true;
                            tempPrice += modifier.price;

                            //}
                        }
                        else
                        {
                            if (checkTotal > minValue || minValue == 0)
                            {
                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("AddOn [ {0} ] is select : [ {1} ]", this.name, this.IsCheck), _TraceCategory);
                                tempPrice -= modifier.price;
                            }
                            else
                            {
                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("AddOn [ {0} ] is select : [ {1} ] to remove but fail due to min item select {2}", this.name, this.IsCheck, minValue), _TraceCategory);
                                modifier.IsCheck = true;
                            }
                        }
                    }

                    foreach (var item in GeneralVar.MainWindowVM.TempEditItem.Where(x => x.itemId == this.parentItemId))
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("New total price : [ {0} ] , Before AddOn [ {1} ] , AddOnPrice [ {2} ]", tempPrice, item.TempTotalPrice, this.price), _TraceCategory);

                        item.TempTotalPrice = tempPrice;
                        item.ItemTotalPrice = tempPrice * item.ItemCurrentQty;
                    }


                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger ModifierCheck Done ..."), _TraceCategory);
                }
                catch (Exception ex)
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("{Error] Trigger ModifierCheck : {0}", ex.ToString()), _TraceCategory);
                }

            }

            private Visibility _TextVisibility;

            public Visibility TextVisibility
            {
                get
                {
                    if (price == 0)
                        _TextVisibility = Visibility.Collapsed;
                    else
                        _TextVisibility = Visibility.Visible;
                    return _TextVisibility;
                }
                set
                {
                    _TextVisibility = value;
                    OnPropertyChanged(nameof(TextVisibility));
                    OnPropertyChanged(nameof(price));
                }
            }
        }

    }
}
    