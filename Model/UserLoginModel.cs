using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFFSSK.Model
{
    public class UserLoginModel
    {
        #region Field

        Dictionary<string, string> _ModuleAccessRight;

        int _UserId;
        string _Username;
        string _DisplayName;

        int _MaintenaneId;
        DateTime _MaintenanceDate;
        string _MaintenanceReferenceNo;

        #endregion

        #region Contructor

        public UserLoginModel() { }

        #endregion

        #region Properties

        public Dictionary<string, string> ModuleAccessRight
        {
            get { return _ModuleAccessRight; }
            set { _ModuleAccessRight = value; }
        }

        public int UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        public string Username
        {
            get { return _Username; }
            set { _Username = value; }
        }

        public string DisplayName
        {
            get { return _DisplayName; }
            set { _DisplayName = value; }
        }

        public int MaintenanceId
        {
            get { return _MaintenaneId; }
            set
            {
                _MaintenaneId = value;
            }
        }

        public DateTime MaintenanceDate
        {
            get { return _MaintenanceDate; }
            set
            {
                _MaintenanceDate = value;
            }
        }

        public string MaintenanceReferenceNo
        {
            get { return _MaintenanceReferenceNo; }
            set
            {
                _MaintenanceReferenceNo = value;
            }
        }

        #endregion
    }
}
