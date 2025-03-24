using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LFFSSK.Model
{
    public class LFFSSKComponentModel
    {
        string _TraceCategory = "LFFSSKComponentModel";

        #region Field

        int _ComponentId;
        int _LocationId;
        DateTime _ClosingTime, _OpeningTime;
        List<DateTime> _EDCSettlementTime;

        string _ComponentCode;
        string _ComponentName;
        string _LocationCode;
        string _LocationName;
        string _ReferenceCode;
        string _OperationStatus;
        string _ComponentDescription;

        #endregion

        #region Contructor

        public LFFSSKComponentModel()
        {
        }

        public LFFSSKComponentModel(int componentId, string componentCode, string componentName, int locationId, string locationCode, string locationName, string componentConfig, string referenceCode, string componentDescription)
        {
            this.ComponentId = componentId;
            this.ComponentCode = componentCode;
            this.ComponentName = componentName;
            this.LocationId = locationId;
            this.LocationCode = locationCode;
            this.LocationName = locationName;
            this.ReferenceCode = referenceCode;
            this.ComponentDescription = componentDescription;
            this.EDCSettlementTime = new List<DateTime>();

            if (componentConfig != null)
            {
                string reg = @"(\w+)=([^\x7C]*)";
                MatchCollection matches = Regex.Matches(componentConfig, reg);
                string edcSettlementTime = string.Empty;

                foreach (Match m in matches)
                {
                    if (m.Groups[1].Value == "NOT")
                        OpeningTime = Convert.ToDateTime(m.Groups[2].Value);
                    else if (m.Groups[1].Value == "NCT")
                        ClosingTime = Convert.ToDateTime(m.Groups[2].Value);
                    else if (m.Groups[1].Value == "EDC")
                        edcSettlementTime = m.Groups[2].Value;
                }

                if (!string.IsNullOrEmpty(edcSettlementTime))
                {
                    try
                    {
                        foreach (string time in edcSettlementTime.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                            this.EDCSettlementTime.Add(Convert.ToDateTime(time));
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] LFFSSKComponentModel = {0}", ex.ToString()), _TraceCategory);
                    }
                }
            }
            else
            {
                DateTime d = DateTime.Now;
                OpeningTime = d;
                ClosingTime = d;
            }

        }

        #endregion

        #region Properties

        public string ComponentCode
        {
            get { return _ComponentCode; }
            set { _ComponentCode = value; }
        }

        public string ComponentName
        {
            get { return _ComponentName; }
            set { _ComponentName = value; }
        }

        public string LocationCode
        {
            get { return _LocationCode; }
            set { _LocationCode = value; }
        }

        public string ReferenceCode
        {
            get { return _ReferenceCode; }
            set { _ReferenceCode = value; }
        }

        public int ComponentId
        {
            get { return _ComponentId; }
            set { _ComponentId = value; }
        }

        public int LocationId
        {
            get { return _LocationId; }
            set { _LocationId = value; }
        }

        public string LocationName
        {
            get { return _LocationName; }
            set { _LocationName = value; }
        }

        public DateTime ClosingTime
        {
            get { return _ClosingTime; }
            set { _ClosingTime = value; }
        }

        public DateTime OpeningTime
        {
            get { return _OpeningTime; }
            set { _OpeningTime = value; }
        }

        public List<DateTime> EDCSettlementTime
        {
            get { return _EDCSettlementTime; }
            set { _EDCSettlementTime = value; }
        }

        public string OperationStatus
        {
            get { return _OperationStatus; }
            set { _OperationStatus = value; }
        }

        public string ComponentDescription
        {
            get { return _ComponentDescription; }
            set { _ComponentDescription = value; }
        }

        #endregion
    }
}
