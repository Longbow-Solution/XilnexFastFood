using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LFFSSK
{
    public class IOHelper
    {
        #region Field

        const string traceCategory = "IOHelper";

        public Edam.Edam ioBoard;
        AutoResetEvent autoResetIOBoard = new AutoResetEvent(false);
        Thread thIOBoardProcess;


        bool isBlicking = false;

        public bool isWaitAborted = false;
        public bool alarmTrigger = false;
        public bool lightTrigger = false;
        private bool _doorOpen = false;
        public Action<bool> IOBoardCallBack;
        public bool DoorOpened
        {
            get { return _doorOpen; }
            set
            {
                _doorOpen = value;
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceVerbose, string.Format("[Error] DoorOpened: {0}", _doorOpen), traceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceVerbose, string.Format("[Error] IOBoardCallBack: {0}", IOBoardCallBack != null), traceCategory);
                if (IOBoardCallBack != null) IOBoardCallBack(value);
            }
        }
        public bool rpTrigger = false;
        public bool ccTrigger = false;
        public bool qrTrigger = false;

        #endregion

        #region Contructor

        public IOHelper()
        {
            ioBoard = new Edam.Edam();
        }

        #endregion

        #region  Method

        public bool OpenPort(string portName, int baudRate, Parity parity, int dataBit, StopBits stopBits)
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "OpenPort Starting...", traceCategory);

                ioBoard.OpenPort(portName, baudRate, parity, dataBit, stopBits);

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("IOBoard Port [{0}] Open Success [{1}]", portName, DateTime.Now), traceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "OpenPort Completed.", traceCategory);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] OpenPort: {0}", ex.ToString()), traceCategory);
                return false;
            }
        }

        public bool ClosePort()
        {
            try
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "ClosePort Starting...", traceCategory);
                ioBoard.ClosePort();

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, string.Format("IOBoard Port Close Success [{0}]", DateTime.Now), traceCategory);
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "ClosePort Completed.", traceCategory);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] ClosePort: {0}", ex.ToString()), traceCategory);
                return false;
            }
        }

        public bool Initialization()
        {
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "Initialization Starting...", traceCategory);
            try
            {
                //autoResetIOBoard.Set();

                Dictionary<Edam.Edam.eDigitalOutput, bool> digitalOutput = new Dictionary<Edam.Edam.eDigitalOutput, bool>();
                if (!ioBoard.DigitalDataOut(GeneralVar.IOBoard_ModuleAddress, digitalOutput))
                    throw new Exception("Unable to DigitalDataOut");


                if (thIOBoardProcess == null)
                {
                    thIOBoardProcess = new Thread(IOBoardProcess);
                    thIOBoardProcess.Name = "thIOBoardProcess";
                    thIOBoardProcess.Start();
                }

                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceInfo, "Initialization Completed", traceCategory);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceError, string.Format("[Error] Initialization: {0}", ex.ToString()), traceCategory);
                return false;
            }
        }

        bool isTri = false;
        private void IOBoardProcess()
        {
            isWaitAborted = true;

            alarmTrigger = false;
            lightTrigger = false;
            DateTime timefrom = DateTime.MinValue;

            while (isWaitAborted)
            {
                try
                {

                    #region Digital Input
                    Dictionary<Edam.Edam.eDigitalOutput, bool> digitalOutput;
                    Dictionary<Edam.Edam.eDigitalInput, bool> digitalInput;


                    if (!ioBoard.DigitalDataIn((byte)GeneralVar.IOBoard_ModuleAddress, out digitalOutput, out digitalInput))
                        throw new Exception("Unable to DigitalDataIn");


                    if (!digitalInput[(Edam.Edam.eDigitalInput)GeneralVar.IOBoard_DI_PanicButton_Channel])
                    {
                        Dictionary<Edam.Edam.eDigitalOutput, bool> _digitalOutput;
                        Dictionary<Edam.Edam.eDigitalInput, bool> _digitalInput;

                        if (lightTrigger)
                        {
                            Dictionary<Edam.Edam.eDigitalOutput, bool> lightOutput = new Dictionary<Edam.Edam.eDigitalOutput, bool>();

                            if (GeneralVar.IOBoard_DO_Light_Enabled)
                                lightOutput.Add((Edam.Edam.eDigitalOutput)GeneralVar.IOBoard_DO_Light_Channel, false);

                            ioBoard.DigitalDataOut((byte)GeneralVar.IOBoard_ModuleAddress, lightOutput);

                            bool istrigger = true;
                            lightTrigger = false;
                            while (istrigger)
                            {
                                if (!ioBoard.DigitalDataIn((byte)GeneralVar.IOBoard_ModuleAddress, out _digitalOutput, out _digitalInput))
                                    throw new Exception("Unable to DigitalDataIn");

                                istrigger = !_digitalInput[(Edam.Edam.eDigitalInput)GeneralVar.IOBoard_DI_PanicButton_Channel];
                                Thread.Sleep(100);
                            }


                        }
                        else
                            lightTrigger = true;
                    }

                    DoorOpened = digitalInput[(Edam.Edam.eDigitalInput)GeneralVar.IOBoard_DI_DoorSensor_Channel];

                    #endregion

                    #region Digital Output

                    Dictionary<Edam.Edam.eDigitalOutput, bool> DigitalOutput = new Dictionary<Edam.Edam.eDigitalOutput, bool>();

                    if (GeneralVar.IOBoard_DO_Alarm_Enabled)
                        DigitalOutput.Add((Edam.Edam.eDigitalOutput)GeneralVar.IOBoard_DO_Alarm_Channel, alarmTrigger);

                    if (GeneralVar.IOBoard_DO_Light_Enabled)
                        DigitalOutput.Add((Edam.Edam.eDigitalOutput)GeneralVar.IOBoard_DO_Light_Channel, lightTrigger);

                    DigitalOutput.Add((Edam.Edam.eDigitalOutput)GeneralVar.IOBoard_DO_QR_Channel, GeneralVar.IOBoard_DO_QR_Enabled && qrTrigger && isBlicking);

                    //if (!isTri && ccTrigger)
                    //{
                    //    isTri = true;
                    //    Thread.Sleep(5000);
                    //}
                    //else if (!ccTrigger)
                    //    isTri = false;

                    DigitalOutput.Add((Edam.Edam.eDigitalOutput)GeneralVar.IOBoard_DO_CC_Channel, GeneralVar.IOBoard_DO_CC_Enabled && ccTrigger && isBlicking);
                    DigitalOutput.Add((Edam.Edam.eDigitalOutput)GeneralVar.IOBoard_DO_RP_Channel, GeneralVar.IOBoard_DO_RP_Enabled && rpTrigger && isBlicking);

                    ioBoard.DigitalDataOut((byte)GeneralVar.IOBoard_ModuleAddress, DigitalOutput);

                    if (lightTrigger) 
                    {
                        bool isOffTrigger = true;
                        Dictionary<Edam.Edam.eDigitalOutput, bool> offOutput;
                        Dictionary<Edam.Edam.eDigitalInput, bool> offInput;
                        while (isOffTrigger)
                        {
                            if (!ioBoard.DigitalDataIn((byte)GeneralVar.IOBoard_ModuleAddress, out offOutput, out offInput))
                                throw new Exception("Unable to DigitalDataIn");

                            isOffTrigger = !offInput[(Edam.Edam.eDigitalInput)GeneralVar.IOBoard_DI_PanicButton_Channel];
                            Thread.Sleep(100);
                        }
                    }


                    isBlicking = !isBlicking;
                    #endregion

                    autoResetIOBoard.Reset();
                    autoResetIOBoard.WaitOne(100);
                }
                catch (Exception ex)
                {
                    Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceVerbose, string.Format("[Warning] IOBoardProcess: {0}", ex.ToString()), traceCategory);
                }
            }

            thIOBoardProcess = null;
            Trace.WriteLineIf(GeneralVar.SwcTraceLevel.TraceVerbose, string.Format("IOBoardProcess Completed."), traceCategory);
        }

        #endregion
    }
}
