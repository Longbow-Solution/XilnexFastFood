using LFFSSK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFFSSK.Model
{
    public class InitModel : Base
    {
        #region Field

        eComponent _Items;
        eInitStatus _InitialStatus;
        bool _IsEnabled;
        ObservableCollection<string> _Remarks = new ObservableCollection<string>();
        string _LastError;
        
        #endregion

        #region Properties

        public eComponent Items
        {
            get { return _Items; }
            set
            {
                if (value != _Items)
                {
                    _Items = value;
                    OnPropertyChanged("Items");
                }
            }
        }

        public eInitStatus InitialStatus
        {
            get { return _InitialStatus; }
            set
            {
                if (value != _InitialStatus)
                {
                    _InitialStatus = value;
                    OnPropertyChanged("InitialStatus");
                }
            }
        }

        public ObservableCollection<string> Remarks
        {
            get { return _Remarks; }
            private set
            {
                if (value != _Remarks)
                {
                    _Remarks = value;
                    OnPropertyChanged("Remarks");
                }
            }
        }

        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                if (value != _IsEnabled)
                {
                    _IsEnabled = value;
                    OnPropertyChanged("IsEnabled");
                }
            }
        }

        public string LastError
        {
            get { return _LastError; }
            set
            {
                _LastError = value;
                OnPropertyChanged("LastError");
            }
        }

        #endregion

        #region Contructor
        public InitModel() { }

        public InitModel(eComponent items, string displayName, bool isEnabled)
        {
            Items = items;
            DisplayName = displayName;
            InitialStatus = eInitStatus.Pending;
            IsEnabled = isEnabled;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _Remarks = null;  

            base.Dispose(disposing);
        }

        ~InitModel()
        {
            Dispose(false);
        }

        #endregion

        #region Function

        public void UpdateStatus(eInitStatus status, string message = null)
        {
            App.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(
            () =>
            {
                InitialStatus = status;
                if (!string.IsNullOrEmpty(message))
                {
                    Remarks.Add(string.Format("{0}", message));

                    if (status == eInitStatus.Error)
                        LastError = message;
                }
            }));
        }

        #endregion
    }
}
