﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace LFFSSK
{
    public class Base : DependencyObject, IDisposable, INotifyPropertyChanged
	{
	  #region Constructor

        protected Base()
        {
        }

        #endregion // Constructor

        #region DisplayName

        public virtual string DisplayName { get; set; }

        #endregion // DisplayName

        #region Debugging Aides

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property displayName: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion // INotifyPropertyChanged Members

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // get rid of managed resources
            }
            // get rid of unmanaged resources
        }

#if DEBUG
        // only if you use unmanaged resources directly in B
        ~Base()
        {
            Dispose();

            //string msg = string.Format("{0} ({1}) ({2}) Finalized", this.GetType().Name, this.DisplayName, this.GetHashCode());
            //System.Diagnostics.Debug.WriteLine(msg);
        }
#endif

        //public void Dispose()
        //{
        //    OnDispose();
        //    GC.SuppressFinalize(this);
        //}

        //protected virtual void OnDispose()
        //{
        //}

        //#if DEBUG

        //~ViewModelBase()
        //{
        //    OnDispose();

        //    string msg = string.Format("{0} ({1}) ({2}) Finalized", this.GetType().Name, this.DisplayName, this.GetHashCode());
        //    System.Diagnostics.Debug.WriteLine(msg);
        //}

        //#endif

        #endregion // IDisposable Members
	}
}
