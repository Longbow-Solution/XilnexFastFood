using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Diagnostics;
using LFFSSK;
using LFFSSK.Model;

namespace Helper
{
    public class User
    {
        #region Field
        const string traceCategory = "User";

        UserStore<IdentityUser> UserStore;
        UserManager<IdentityUser> UserManager;
        #endregion

        #region Contructor

        public User()
        {

        }
        #endregion

        #region Function
        public bool UserAuthentication(string username, string password, out bool validuser, out bool validPassword, out bool validAccessRight, string accessRight = null)
        {
            bool success = false;
            validAccessRight = validPassword = validuser = false;
            try
            {
                IdentityUser user = UserStore.Users.Where(u => u.UserName == username).FirstOrDefault();

                if (user == null)
                    validuser = false;
                else
                {
                    validuser = true;
                    validPassword = UserManager.CheckPassword(user, password);
                }

                if (accessRight == null)
                    validAccessRight = true;
                else
                {
                    if (username == "sysadmin")
                    {
                        LFFSSK.Model.UserLoginModel u = new LFFSSK.Model.UserLoginModel();
                        u.DisplayName = "Admin";
                        u.UserId = -1;
                        u.Username = "sysadmin";
                        validAccessRight = true;
                        GeneralVar.User = u;
                    }
                }
            }
            catch (Exception ex)
            {
                //GeneralVar.Crud.TxAlarm_Insert(DateTime.Now, (char)eAlarmTxType.Error, GeneralVar.AlarmCategory[RCSSSK.AppCode.eAlarmCategory.GNR], ex.Message);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] UserAuthentication: {0}", ex.ToString()), traceCategory);
            }
            return success;
        }

        public string NewReferenceNo(string refType, int compId)
        {
            string ReferenceNo;
            int year = int.Parse(DateTime.Now.ToString("yy"));
            int sequence = 1;

            ReferenceNo = string.Format("{0}-{1}-{2}{3}", refType, compId.ToString("000"), year.ToString("00"), sequence.ToString("000000"));
            return ReferenceNo;
        }


        #endregion
    }
}
