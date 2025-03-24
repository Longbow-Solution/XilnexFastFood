using DFMonitoringClient;
using Helper;
using LFFSSK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
	public class ClientMonitoringStatus
	{
		const string traceCategory = "ClientMonitoringStatus";

		public static List<DFDeviceStatus> GetSystemStatusMonitoring()
		{
			List<DFDeviceStatus> param = new List<DFDeviceStatus>();
			param.Add(GetAppStatus());

			/*TO DO: Add or Remove Component*/
            param.Add(GetDeviceStatus("SYS", "General", true, GeneralVar.Checklist.Single(c => c.Items == eComponent.System_FNB).LastError, GeneralVar.Checklist.Single(c => c.Items == eComponent.System_FNB).LastError));
            param.Add(GetDeviceStatus("CC", "MPay Terminal", GeneralVar.CC_Enabled, GeneralVar.Checklist.Single(c => c.Items == eComponent.CreditTerminal).LastError, GeneralVar.Checklist.Single(c => c.Items == eComponent.CreditTerminal).LastError));
            param.Add(GetReceiptPrinterStatus());
            param.Add(GetIOBoardStatus());

			return param;
		}

		public static DFDeviceStatus GetAppStatus()
		{

            string status = string.Empty, details = string.Empty;
            DFSeverityLevel appSeverity = DFSeverityLevel.Info;
            try
            {
                if (GeneralVar.MainWindowVM.Stage == eStage.Initializing)
                    status = "Initializing";
                else if (GeneralVar.MainWindowVM.Stage == eStage.Initialized)
                    status = "Initialized";
                else if (GeneralVar.MainWindowVM.Stage == eStage.OffOperation)
                    status = "Offline";
                else if (GeneralVar.MainWindowVM.Stage == eStage.MaintenanceAction || GeneralVar.MainWindowVM.Stage == eStage.MaintenanceEDC || GeneralVar.MainWindowVM.Stage == eStage.MaintenanceLogin || GeneralVar.MainWindowVM.Stage == eStage.MaintenanceReprint || GeneralVar.MainWindowVM.Stage == eStage.MaintenanceSelection || GeneralVar.MainWindowVM.Stage == eStage.MaintenanceVoid)
                {
                    status = "Maintenance";
                    appSeverity = DFSeverityLevel.None;
                }
                else if (GeneralVar.MainWindowVM.Stage == eStage.OutOfOrder)
                {
                    status = "Out of Service";
                    appSeverity = DFSeverityLevel.Error;
                }
                else if (GeneralVar.MainWindowVM.Stage == eStage.CredentialVerificationTimeout)
                {
                    status = "Unauthorized Access";
                    appSeverity = DFSeverityLevel.Error;
                }
            }
            catch (Exception ex) 
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error]GetDeviceStatus = {0}", ex.ToString()), traceCategory);
            }

			return new DFDeviceStatus() { Code = "APP", Severity = appSeverity, Status = status, Details = details };
		}

		public static DFDeviceStatus GetDeviceStatus(string componentCode, string componentDisplayName, bool isEnabled, string lastError, string details)
		{

            string status = string.Empty;
            DFSeverityLevel severity = DFSeverityLevel.Info;
            try
            {
                if (!isEnabled)
                {
                    status = "Offline";
                    severity = DFSeverityLevel.None;
                }
                else if (!string.IsNullOrEmpty(lastError))
                {
                    status = lastError;
                    severity = DFSeverityLevel.Error;
                }
                else
                    status = "Online";

                status = string.Format("[{0}] {1}", componentDisplayName, status);
            }
            catch (Exception ex) 
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] [{0}]GetDeviceStatus = {1}", componentCode, ex.ToString()), traceCategory);
            }

            return new DFDeviceStatus() { Code = componentCode, Severity = severity, Status = status, Details = details };
		}

		//Customized Component Status
		public static DFDeviceStatus GetReceiptPrinterStatus()
		{
			string status = string.Empty;
			DFSeverityLevel severity = DFSeverityLevel.Info;

			if (!GeneralVar.Printer_Enabled)
			{
				status = "Offline";
				severity = DFSeverityLevel.None;
			}
            else if (!string.IsNullOrEmpty(GeneralVar.Checklist.Single(c => c.Items == eComponent.ReceiptPrinter).LastError))
			{
				status = GeneralVar.DocumentPrint.LastError;
                severity = GeneralVar.DocumentPrint.LastError == "PaperNearEnd" ? DFSeverityLevel.Warning : DFSeverityLevel.Error;
			}
			else
				status = "Online";

			status = string.Format("[{0}] {1}", GeneralVar.ReceiptPrinter_Port, status);
			return new DFDeviceStatus() { Code = "RP", Severity = severity, Status = status, Details = string.Empty };
		}

        //Note Acceptor Status

        public static DFDeviceStatus GetIOBoardStatus() 
        {
            string status = string.Empty, details = string.Empty;
            DFMonitoringClient.DFSeverityLevel severity = DFMonitoringClient.DFSeverityLevel.Info;
         
            try
            {
                if (!GeneralVar.IOBoard_Enabled)
                {
                    status = "Offline";
                    severity = DFMonitoringClient.DFSeverityLevel.None;
                }
                else
                    status = "Online";

                details = string.Format("Door Open", GeneralVar.IOBoard.DoorOpened);

                status = string.Format("[{0}] {1}", "Adam", status);
            }
            catch (Exception ex) 
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error]GetIOBoardStatus = {0}", ex.ToString()), traceCategory);
            }

            return new DFDeviceStatus() { Code = "IO", Severity = severity, Status = status, Details = details };
        }
	}
}
