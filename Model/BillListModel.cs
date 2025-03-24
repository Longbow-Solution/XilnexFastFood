using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LFFSSK.Model
{
    public class BillListModel : Base
    {
        public BillListModel()
        {

        }

        public BillListModel(int index, string billNo, string accountId, string billId, string billName, string billType, decimal billAmount, string referenceNo = null)
        {
            this.BillListAccountId = accountId;
            this.BillListIndex = index;
            this.BillListNo = billNo;
            this.BillListBillId = billId;
            this.BillListName = billName;
            this.BillListType = billType;
            this.BillListAmountPaid = billAmount;
            this.BillListReferenceNo = referenceNo;
        }

        string _BillListNo;
        string _BillListBillId;
        string _BillListAccountId;
        string _BillListName;
        string _BillListType;
        string _BillListTypeDB;
        decimal _BillListAmountPaid;
        decimal _BillListAPIAmount;
        ICommand _BillListDeleteCommand;
        int _BillListIndex;

        public int BillListIndex
        {
            get { return _BillListIndex; }
            set
            {
                _BillListIndex = value;
                OnPropertyChanged("BillListIndex");
                OnPropertyChanged("sIndex");
            }
        }

        public string BillListReferenceNo { get; set; }

        public string sIndex
        {
            get { return _BillListIndex.ToString(); }
        }

        public string BillListNo
        {
            get { return _BillListNo; }
            set
            {
                _BillListNo = value;
                OnPropertyChanged("BillListNo");
            }
        }

        public string BillListBillId
        {
            get { return _BillListBillId; }
            set
            {
                _BillListBillId = value;
                OnPropertyChanged("BillListBillId");
            }
        }

        public string BillListAccountId
        {
            get { return _BillListAccountId; }
            set
            {
                _BillListAccountId = value;
                OnPropertyChanged("BillListAccountId");
            }
        }

        public string BillListName
        {
            get { return _BillListName; }
            set
            {
                _BillListName = value;
                OnPropertyChanged("BillListName");
            }
        }

        public string BillListType
        {
            get { return _BillListType; }
            set
            {
                _BillListType = value;
                OnPropertyChanged("BillListType");
            }
        }

        public string BillListTypeDB
        {
            get { return _BillListTypeDB; }
            set
            {
                _BillListTypeDB = value;
                OnPropertyChanged("BillListTypeDB");
            }
        }

        public decimal BillListAmountPaid
        {
            get { return _BillListAmountPaid; }
            set
            {
                _BillListAmountPaid = value;
                OnPropertyChanged("BillListAmount");
            }
        }

        public decimal BillListAPIAmount
        {
            get { return _BillListAPIAmount; }
            set
            {
                _BillListAPIAmount = value;
                OnPropertyChanged("BillListAPIAmount");
            }
        }

        string _UpdateAPIStatus;
        public string UpdateAPIStatus
        {
            get { return _UpdateAPIStatus; }
            set
            {
                _UpdateAPIStatus = value;
                OnPropertyChanged("UpdateAPIStatus");
            }
        }

        string _LabelDelete;
        public string LabelDelete
        {
            get { return _LabelDelete; }
            set
            {
                _LabelDelete = value;
                OnPropertyChanged("LabelDelete");
            }
        }

        public ICommand BillListDeleteCommand
        {
            get
            {
                if (_BillListDeleteCommand == null)
                    _BillListDeleteCommand = new RelayCommand<string>(BillListDelete);
                return _BillListDeleteCommand;
            }
        }

        void BillListDelete(string param)
        {
            GeneralVar.MainWindowVM.Delete(param);
        }
    }
}
