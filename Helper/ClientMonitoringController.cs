using DFMonitoringClient;
using LFFSSK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class ClientMonitoringController
    {
        #region Variable
        const string traceCategory = "ClientMonitoringHandler";
        const string clientType = "SSK";

        private DFSocketClientHandler ClientStatusHandler;
        private DFSocketClientHandler ClientCommandHandler;
        #endregion

        public void StartMonitoring()
        {
            IPEndPoint serverEP = new IPEndPoint(GeneralVar.Monitoring_IPAddress, GeneralVar.Monitoring_Port);

            if (ClientStatusHandler != null)
            {
                ClientStatusHandler.StopHandle();
                ClientStatusHandler = null;
            }

            if (ClientCommandHandler != null)
            {
                ClientCommandHandler.StopHandle();
                ClientCommandHandler = null;
            }

            ClientStatusHandler = new DFSocketClientHandler(GeneralVar.ComponentCode, clientType, serverEP, GeneralVar.Monitoring_SetStatusInterval);
            ClientCommandHandler = new DFSocketClientHandler(GeneralVar.ComponentCode, clientType, serverEP, GeneralVar.Monitoring_CheckCommandInterval);

            ClientStatusHandler.ConnectSuccess += ConnectSuccess;
            ClientStatusHandler.ConnectFailed += ConnectFailed;
            ClientStatusHandler.SetStatus += SetStatus;
            ClientStatusHandler.SetStatusFailed += SetStatusFailed;
            ClientStatusHandler.SetResponseFailed += SetStatusResponseFailed;
            ClientStatusHandler.StartHandle();

            ClientCommandHandler.GetCommandSuccess += GetCommandSuccess;
            ClientCommandHandler.GetCommandFailed += GetCommandFailed;
            ClientCommandHandler.SetResponseFailed += SetCommandResponseFailed;
            ClientCommandHandler.StartHandle();
        }

        public void StopMonitoring()
        {
            if (ClientStatusHandler != null)
            {
                ClientStatusHandler.StopHandle();

                ClientStatusHandler.ConnectSuccess -= ConnectSuccess;
                ClientStatusHandler.ConnectFailed -= ConnectFailed;
                ClientStatusHandler.SetStatus -= SetStatus;
                ClientStatusHandler.SetStatusFailed -= SetStatusFailed;
                ClientStatusHandler.SetResponseFailed -= SetStatusResponseFailed;

                ClientStatusHandler = null;
            }

            if (ClientCommandHandler != null)
            {
                ClientCommandHandler.StopHandle();

                ClientCommandHandler.GetCommandSuccess -= GetCommandSuccess;
                ClientCommandHandler.GetCommandFailed -= GetCommandFailed;
                ClientCommandHandler.SetResponseFailed -= SetCommandResponseFailed;

                ClientCommandHandler = null;
            }
        }

        private void ConnectSuccess(object sender, ConnectSuccessEventArgs e)
        {
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceVerbose, string.Format("ConnectSuccess: {0}", e.RemoteEP), traceCategory);
        }

        private void ConnectFailed(object sender, ConnectFailedEventArgs e)
        {
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("ConnectFailed: {0}", e.Error.ToString()), traceCategory);
        }

        #region Set Status
        private void SetStatus(object sender, SetStatusEventArgs e)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceVerbose, "SetStatus...", traceCategory);
                e.ClientStatus.DeviceStatus = ClientMonitoringStatus.GetSystemStatusMonitoring();
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] SetStatus: {0}", ex.ToString()), traceCategory);
            }
        }

        private void SetStatusFailed(object sender, SetStatusFailedEventArgs e)
        {
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("SetStatusFailed: {0}", e.Error.ToString()), traceCategory);
        }

        public void SetStatusResponseFailed(object sender, SetResponseFailedEventArgs e)
        {
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("SetStatusResponseFailed: {0}", e.Error.ToString()), traceCategory);
        }
        #endregion

        #region Get Command
        private void GetCommandSuccess(object sender, GetCommandSuccessEventArgs e)
        {
            try
            {
                string commandLog = string.Format("GetCommandSuccess... Client Id: {0}\r\nToken Id: {1}\r\nConsoleId: {2}\r\nCommand: {3}", e.Message.ClientId, e.Message.TokenId, e.Message.ConsoleId, e.Message.Command);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, commandLog, traceCategory);

                switch (e.Message.Command.ToUpper())
                {
                    case "GETSTATUS":
                        //string status = DFSocketClientHandler.Serialize(GeneralFunc.GetSystemStatusMonitoring());
                        //Trace.WriteLineIf(GeneralVar.swcTraceLevel.TraceInfo, string.Format("GetCommandSuccess: Status: {0}", status), TraceCategory);
                        //e.Message.Response = status;
                        break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] GetCommandSuccess: {0}", ex.ToString()), traceCategory);
            }
        }

        private void GetCommandFailed(object sender, GetCommandFailedEventArgs e)
        {
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("GetCommandFailed: {0}", e.Error.ToString()), traceCategory);
        }

        public void SetCommandResponseFailed(object sender, SetResponseFailedEventArgs e)
        {
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("SetCommandResponseFailed: {0}", e.Error.ToString()), traceCategory);
        }
        #endregion
    }
}
