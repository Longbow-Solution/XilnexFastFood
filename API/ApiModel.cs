using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media.Imaging;
using System.IO;
using Newtonsoft.Json;

namespace LFFSSK.API
{
    public class ApiModel
    {

        public class GetMenu
        {
            
            public class Request
            {
                public string LocationID { get; set; }
                public int MenuID { get; set; }
                public int Source { get; set; }
            }
            public class Response
            {
                public bool ok { get; set; }
                public string status { get; set; }
                public object warning { get; set; }
                public object error { get; set; }
                public List<Category> data { get; set; }

                public class Category:Base
                {
                    string _TraceCategory = "LFFSSK.Model.ApiModel.Category";
                    public Category(int CategoryId, string CategoryName, string CategoryImageUrl, string ColorHex, int DisplayMode, List<object> department, string BinaryImageUrl, List<Product> Products)
                    {
                        categoryId = CategoryId;
                        categoryName = CategoryName;
                        categoryImageUrl = CategoryImageUrl;
                        colorHex = ColorHex;
                        displayMode = DisplayMode;
                        Department = department;
                        binaryImageUrl = BinaryImageUrl;
                        products = Products;
                    }

                    public int categoryId { get; set; }
                    public string categoryName { get; set; }
                    public string categoryImageUrl { get; set; }
                    public string colorHex { get; set; }
                    public int displayMode { get; set; }
                    public List<object> Department { get; set; }
                    public string binaryImageUrl { get; set; }
                    public List<Product> products { get; set; }
                    private bool _IsChecked;
                    public bool IsChecked
                    {
                        get { return _IsChecked; }
                        set
                        {
                            _IsChecked = value;
                            OnPropertyChanged(nameof(IsChecked));
                        }
                    }

                    private ICommand _BtnSortMenu;
                    public ICommand BtnSortMenu
                    {
                        get
                        {
                            if (_BtnSortMenu == null)
                                _BtnSortMenu = new RelayCommand<int>(RefreshProduct);
                            return _BtnSortMenu;
                        }
                    }

                    private void RefreshProduct(int categoryId)
                    {
                        try
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger RefreshProduct Starting ..."), _TraceCategory);

                            if (categoryId == GeneralVar.MainWindowVM.CateId)
                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Same category trigger..."), _TraceCategory);
                            else
                            {
                                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Getting Menu for category [ {0} ] ", this.categoryName), _TraceCategory);
                                GeneralVar.MainWindowVM.RefreshProduct(categoryId);
                            }

                            //if (GeneralVar.MainWindowVM.vMenuPage != null)
                            //    GeneralVar.MainWindowVM.vMenuPage.ResetMenuScroll();

                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger RefreshProduct Done ..."), _TraceCategory);

                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("[Error] RefreshProduct = {0}", ex.Message), _TraceCategory);
                        }
                    }
                }

                public class Product : Base
                {
                    private string _TraceCategory = "ApiModel.Product";
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

                    public Product(double doubleSalePrice, double doubleEmployeePrice, double doubleWholesalePrice, double doubleCustomPrice, double doubleManuSugRetailPrice, double doubleWebPrice, double doubleWebDealerPrice, string CustomField1, string CustomField2, string CustomField3, string CustomField4, string CustomField5, string CustomField6, string CustomField7, string CustomField8, string CustomField9, string CustomField10, string CustomField11, string CustomField12, string CustomField13, string CustomField14, string CustomField15, string CustomField16, string CustomField17, string CustomField18, string ItemName, string ColorHex, string DynamicHeaderLabel, List<DynamicModifier> DynamicModifiers, int ItemId, string ItemCode, string ItemType, double Price, List<string> ImageUrlList, string BinaryImageUrl, string ImageUrl, string Description, string Size, bool BoolOpenPrice, string StockType, bool isAssortment, bool hasStock, bool allowToSell, bool isActive, BitmapImage menuImagePath, string menuImage, int itemCurrentQty, double itemTotalPrice, double tempTotalPrice)
                    {
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

                    private int _ItemCurrentQty;
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
                        }
                    }
                    private double _ItemTotalPrice;

                    public double ItemTotalPrice
                    {
                        get { return _ItemTotalPrice; }
                        set
                        {
                            _ItemTotalPrice = value;
                            GeneralVar.MainWindowVM.MenuCurrentTotal = value;
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
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("{Error] Trigger RetrieveMenuDetails : {0}", ex.ToString()), _TraceCategory);
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

                    private void AddQty()
                    {
                        try
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger AddQty Starting ..."), _TraceCategory);

                            int currentQty = GeneralVar.MainWindowVM.MenuDetails.Where(x => x.itemId == itemId).FirstOrDefault().ItemCurrentQty;

                            //GeneralVar.MainWindowVM.MenuDetails.Where(x => x.itemId == itemId).FirstOrDefault().ItemCurrentQty += 1;

                            //this.ItemCurrentQty += 1;

                            foreach (var item in GeneralVar.MainWindowVM.MenuDetails.Where(x => x.itemId == itemId))
                            {
                                item.ItemCurrentQty++;
                                item.ItemTotalPrice = item.TempTotalPrice * item.ItemCurrentQty;
                            }

                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Adding qty for [ {0} ] from [ {1} ] to [ {2} ]", this.itemName, currentQty, this.ItemCurrentQty), _TraceCategory);
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger AddQty Done ..."), _TraceCategory);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("{Error] Trigger AddQty : {0}", ex.ToString()), _TraceCategory);
                        }
                    }

                    private void DeductQty()
                    {
                        try
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger RetrieveMenuDetails Starting ..."), _TraceCategory);

                            int currentQty = GeneralVar.MainWindowVM.MenuDetails.Where(x => x.itemId == this.itemId).FirstOrDefault().ItemCurrentQty;

                            if (currentQty > 1)
                            {
                                foreach (var item in GeneralVar.MainWindowVM.MenuDetails.Where(x => x.itemId == this.itemId))
                                {
                                    item.ItemCurrentQty--;
                                    item.ItemTotalPrice = item.TempTotalPrice * item.ItemCurrentQty;
                                }
                            }

                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Deduct qty for [ {0} ] from [ {1} ] to [ {2} ]", this.itemName, currentQty, this.ItemCurrentQty), _TraceCategory);
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger RetrieveMenuDetails Done ..."), _TraceCategory);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, String.Format("{Error] Trigger RetrieveMenuDetails : {0}", ex.ToString()), _TraceCategory);
                        }
                    }
                }

                public class DynamicModifier :Base
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

                public class Selection:Base
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

                    private void ModifierCheck()
                    {
                        try
                        {
                            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, String.Format("Trigger ModifierCheck Starting ..."), _TraceCategory);

                            int maxValue = GeneralVar.MainWindowVM.MenuDetails
                            .SelectMany(menuDetail => menuDetail.dynamicmodifiers)
                            .SelectMany(dynamicMod => dynamicMod.modifiers)// Flatten the list of lists into a single list
                            .Where(modifierType => modifierType.groupId == this.groupId) // Filter by the current modifierId
                            .FirstOrDefault().maxSelection;

                            int minValue = GeneralVar.MainWindowVM.MenuDetails
                            .SelectMany(menuDetail => menuDetail.dynamicmodifiers)
                            .SelectMany(dynamicMod => dynamicMod.modifiers)// Flatten the list of lists into a single list
                            .Where(modifierType => modifierType.groupId == this.groupId) // Filter by the current modifierId
                            .FirstOrDefault().minSelection;

                            int checkTotal = GeneralVar.MainWindowVM.MenuDetails
                            .SelectMany(menuDetail => menuDetail.dynamicmodifiers)
                            .SelectMany(dynamicMod => dynamicMod.modifiers)// Flatten the list of modifier types
                            .Where(mod => mod.groupId == this.groupId) // Filter by parentModifierId
                            .SelectMany(modifierType => modifierType.selections) // Flatten the list of ModifiersDetails
                            .Where(modifierDetail => modifierDetail.IsCheck == true)
                            .Count();

                            double tempPrice = GeneralVar.MainWindowVM.MenuDetails
                            .Where(x => x.itemId == this.parentItemId)
                            .FirstOrDefault().TempTotalPrice;

                            foreach (var modifier in GeneralVar.MainWindowVM.MenuDetails
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

                            foreach (var item in GeneralVar.MainWindowVM.MenuDetails.Where(x => x.itemId == this.parentItemId))
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
                }
            }
        }

        public class FnBOrders
        {
            public class Request
            {
                public Request(int salesNo, double totalAmount, double discountAmount, double roundingAmount, double mGstTaxAmount, double billTaxPercentange, double serviceChargeAmount, string customerId, string recipient, string salesOutlet, string salesType, string orderNo, int paxNo,string salesPerson,string remark, bool isPrint,string Datetime,string orderSource, string pickupTime, string recipientContact, string promoCode, double totalDiscountAmount, double totalAmountBeforeTax, string recipientContactTel, double billDiscountAmount, string customFieldValueThree, double billTaxAmount, double serviceChargeAmountAfterTax, bool postStatus, string externalRefId, List<SalesItem>salesItems)
                {
                    //this.SalesNo = salesNo;
                    //this.TotalAmount = totalAmount;
                    this.DiscountAmount = discountAmount;
                    //this.RoundingAmount = roundingAmount;
                    //this.MgstTaxAmount = mGstTaxAmount;
                    //this.BillTaxPercentage = billTaxPercentange;
                    //this.ServiceChargeAmount = serviceChargeAmount;
                    this.CustomerId = customerId;
                    this.Recipient = recipient;
                    this.SalesOutlet = salesOutlet;
                    this.SalesType = salesType;
                    this.OrderNo = orderNo;
                    //this.PaxNo = paxNo;
                    this.SalesPerson = salesPerson;
                    this.Remark = remark;
                    this.IsPrint = isPrint;
                    this.dateTime = Datetime;
                    this.OrderSource = orderSource;
                    this.PickupTime = pickupTime;
                    //this.RecipientContact = recipientContact;
                    this.PromoCode = promoCode;
                    //this.TotalDiscountAmount = totalDiscountAmount;
                    //this.TotalAmountBeforeTax = totalAmountBeforeTax;
                    //this.RecipientContactTel = recipientContactTel;
                    //this.BillDiscountAmount = billDiscountAmount;
                    this.CustomFieldValueThree = customFieldValueThree;
                    //this.BillTaxAmount = billTaxAmount;
                    //this.ServiceChargeAmountAfterTax = serviceChargeAmountAfterTax;
                    //this.PostStatus = postStatus;
                    this.ExternalRefId = externalRefId;
                    this.SalesItems = salesItems;
                }

                //public int SalesNo { get; set; }
                //public double TotalAmount { get; set; }
                public double DiscountAmount { get; set; }
                //public double RoundingAmount { get; set; }
                //public double MgstTaxAmount { get; set; }
                //public double BillTaxPercentage { get; set; }
                //public double ServiceChargeAmount { get; set; }
                public string CustomerId { get; set; }
                public string Recipient { get; set; }
                public string SalesOutlet { get; set; }
                public string SalesType { get; set; }
                public string OrderNo { get; set; }
                //public int PaxNo { get; set; }
                public string SalesPerson { get; set; }
                public string Remark { get; set; }
                public bool IsPrint { get; set; }
                public string dateTime { get; set; }
                public string OrderSource { get; set; }
                public string PickupTime { get; set; }
                //public string RecipientContact { get; set; }
                public string PromoCode { get; set; }
                //public double TotalDiscountAmount { get; set; }
                //public double TotalAmountBeforeTax { get; set; }
                //public string RecipientContactTel { get; set; }
                //public double BillDiscountAmount { get; set; }
                public string CustomFieldValueThree { get; set; }
                //public double BillTaxAmount { get; set; }
                //public double ServiceChargeAmountAfterTax { get; set; }
                //public bool PostStatus { get; set; }
                public string ExternalRefId { get; set; }
                public List<SalesItem> SalesItems { get; set; } = new List<SalesItem>();

                public class SalesItem
                {
                    public SalesItem(int salesItemId, int itemId, string itemCode,int quantity, int shippedQuantity, double price, double discountPercentage, double discountAmount, bool isPrint, double subTotal, double mgstTaxAmount, double totalTaxAmount, bool isInclusiveMgst, bool isServiceItem, double additionalTaxPercentage1, double additionalTaxPercentage2, double additionalTaxAmount1, double additionalTaxAmount2, bool isVoucherItem, bool isPromoDiscountItem, List<Modifier> modifiers)
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

        public class InitialOrderRequest 
        {
            public Xilnex.Request OrderDetails { get; set; }
            public int ComponentId { get; set; }
            public string ComponentCode { get; set; }
            public int OutletId { get; set; }
        }
        public class InitialOrderResponse
        {
            public string Code { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public string OrderNumber { get; set; }
            public string ReferenceNo { get; set; }
        }

        public class PostOrderRequest
        {
            public bool IsDelayOrder { get; set; }
            public string ReferenceNo { get; set; }
            public Xilnex.Request OrderDetails { get; set; }
            public int ComponentId { get; set; }
            public string ComponentCode { get; set; }
            public int OutletId { get; set; }
        }

        public class PostOrderResponse
        {
            public string Code { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public string OrderNumber { get; set; }
        }

    }
}
