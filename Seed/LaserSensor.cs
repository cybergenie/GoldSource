using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using STIL_NET;

namespace Seed
{
    public class LaserSensor
    {
        private const float SEC_TO_MILLISEC = 1000.0f;

        private const float TIMEOUT_ACQUISITION_CONST = 500.0f;
        //number of buffers to acquire
        static uint number_of_buffers = 5;
        //number of points TRE to acquire
        static uint number_of_points_tre = 5;

        private dll_chr m_dll_chr = null;
        private sensor m_sensor = null;
        private cAcqParamMeasurement acqParamMeasurement = new cAcqParamMeasurement();
        private enSensorError sError = enSensorError.MCHR_ERROR_NONE;
        private sensorManager m_sensor_manager = new sensorManager();
        private cNamedEvent m_measurement_event = new cNamedEvent();
        private cNamedEvent m_exit_event = new cNamedEvent();
        private cNamedEvent m_exit_event_do = new cNamedEvent();
        public struct AcqData
        {
            public float Altitude;
            public float Counter;
        }

        private bool Init()
        {
            m_dll_chr = new dll_chr();

            if (m_dll_chr.Init() == false)
            {
                MessageBox.Show("传感器初始化失败.");
                return false;
            }
            //Display DLL(s) versions               
            Debug.WriteLine(string.Format("DLL_CHR.DLL :\t\t {0}", m_sensor_manager.DllChrVersion));
            Debug.WriteLine(string.Format("STILSensors.DLL :\t {0}", m_sensor_manager.DllSensorsVersion));
            return true;
        }

        private bool Release()
        {
            if (m_sensor != null)
            {
                m_sensor.Release();
            }
            m_dll_chr.Release();
            return (true);
        }

        private bool Open(enSensorType sensorType)
        {
            bool result = true;

            //open sensor
            m_sensor = m_sensor_manager.OpenUsbConnection("", sensorType, null, null);
            if (m_sensor != null)
            {
                m_sensor.OnError += new sensor.ErrorHandler(OnError);
                //get automatic parameters
                if (acqParamMeasurement.Init(m_sensor) == enSensorError.MCHR_ERROR_NONE)
                {
                    // Set number of points for TRE mode (should be > 0)
                    acqParamMeasurement.NumberPointsTRE = number_of_points_tre;
                    // Set buffer size (should be > 0)
                    acqParamMeasurement.BufferLength = number_of_points_tre;
                    // Set Number of acquisition buffers per data (should be > 1)
                    acqParamMeasurement.NumberOfBuffers = number_of_buffers;
                    //set altitude and counter buffering enabled
                    acqParamMeasurement.EnableBufferAltitude.Altitude = true;
                    acqParamMeasurement.EnableBufferAltitude.Counter = true;
                    //set timeout acquisition : should be at least = ((BufferLength * averaging) / rate) * 1000 + 100
                    acqParamMeasurement.Timeout = (uint)(TIMEOUT_ACQUISITION_CONST + ((SEC_TO_MILLISEC * Convert.ToSingle(m_sensor.Averaging) / Convert.ToSingle(acqParamMeasurement.Frequency)) * Convert.ToSingle(acqParamMeasurement.NumberPointsTRE)));
                    //trigger settings
                    acqParamMeasurement.Trigger.Enable = true;
                    acqParamMeasurement.Trigger.Type = enTriggerType.TRE;
                    acqParamMeasurement.Trigger.HighLevelOrRisingEdgeActivated = enLevelEdgeFlag.RISING_EDGE;
                    //event type (here end of measurements) and callback function
                    acqParamMeasurement.EnableEvent.EventEndBuffer = true;
                    m_sensor.OnEventMeasurement += new sensor.OnEventMeasurementHandler(FuncEventMeasurement);
                }
                else
                {
                    MessageBox.Show("驱动初始化失败。");
                    Close();
                    result = false;
                }
            }
            else
            {
                MessageBox.Show("传感器连接错误(没有连接传感器或者传感器连接不正确)。");
                result = false;
            }
            return (result);
        }

        void OnError(object sender, cErrorEventArgs e)
        {
            MessageBox.Show(String.Format("传感器错误: {0}-{1}{2}", e.Exception.ErrorType, e.Exception.FunctionName, e.Exception.ErrorDetail));
        }

        private bool Close()
        {
            bool result = true;

            if (m_sensor != null)
            {
                m_sensor.Close();
            }
            else
            {
                MessageBox.Show("连接已关闭 (没有连接传感器或者传感器连接不正确)");
                result = false;
            }
            return (result);

        }
        public bool StartAcquisition()
        {
            bool result = true;

            if (m_sensor != null)
            {
                sError = m_sensor.StartAcquisition_Measurement(acqParamMeasurement);
                if (sError != enSensorError.MCHR_ERROR_NONE)
                {
                    MessageBox.Show(string.Format("传感器获取数据错误 {0}", sError.ToString()));
                }
            }
            else
            {
                MessageBox.Show("传感器获取数据错误 (传感器没有连接或者连接错误)");
                result = false;
            }
            return (result);
        }
        public bool StopAcquisition()
        {
            bool result = true;

            if (m_sensor != null)
            {
                m_sensor.StopAcquisition_Measurement();
            }
            else
            {
                MessageBox.Show("传感器停止获取数据错误 (传感器没有连接或者连接错误)");
                result = false;
            }
            return (result);
        }

        public bool SetParameter()
        {
            //set 100hz acquisition frequency
            m_sensor.ScanRate = (enFixedScanRates)enFixedScanRates_CCS_PRIMA.CCS_PRIMA_100HZ;
            //set averaging = 1 for acquisition
            m_sensor.Averaging = 1;
            return (true);
        }

        public List<AcqData> Execute()
        {
            float[] Altitude = null;
            float[] Counter = null;
            float[] BufferNullFloat = null;
            uint Len = 0;
            List<AcqData> acqDatas = new List<AcqData>();

            while (m_exit_event.Wait(0) == false)
            {
                if (m_measurement_event.Wait(10))
                {
                    sError = m_sensor.GetAltitudeAcquisitionData(ref Altitude, ref BufferNullFloat, ref Counter, ref BufferNullFloat, ref BufferNullFloat, ref Len);
                    if (sError == enSensorError.MCHR_ERROR_NONE)
                    {
                        Console.WriteLine("");
                        for (uint idx = 0; idx < Len; idx++)
                        {
                            AcqData acqData;
                            acqData.Altitude = (float)Altitude[idx];
                            acqData.Counter = Counter[idx];
                            acqDatas.Add(acqData);
                            Debug.WriteLine(string.Format("[{0:D3}] 数值  = {1:F2} (强度 : {2})", idx, (float)Altitude[idx], Counter[idx]));
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format("FuncEventMeasurement : 错误 : GetAltitudeAcquisitionData : {0}", sError.ToString()));
                    }
                }
            }
            m_exit_event_do.Set();

            return acqDatas;
        }

        public bool EndExecute()
        {
            return (m_exit_event_do.Wait(10));
        }
        public void FuncEventMeasurement(sensor.enSensorAcquisitionEvent ev)
        {
            switch (ev)
            {
                case sensor.enSensorAcquisitionEvent.EV_END_ACQUIRE:
                    m_exit_event.Set();
                    Console.WriteLine(string.Format("Event : {0}", ev.ToString()));
                    break;
                case sensor.enSensorAcquisitionEvent.EV_END_BUFFER:
                    m_measurement_event.Set();
                    Console.WriteLine(string.Format("Event : {0}", ev.ToString()));
                    break;
                case sensor.enSensorAcquisitionEvent.EV_END_MEASUREMENT:
                    m_measurement_event.Set();
                    Console.WriteLine(string.Format("Event : {0}", ev.ToString()));
                    break;
                case sensor.enSensorAcquisitionEvent.EV_ACQUIRE_N_POINTS:
                    m_measurement_event.Set();
                    Console.WriteLine(string.Format("Event : {0}", ev.ToString()));
                    break;
                default:
                    if (ev == sensor.enSensorAcquisitionEvent.EV_END_ACQUIRE_TIMEOUT)
                    {
                        Console.Write(".");
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Event : {0}", ev.ToString()));
                    }
                    break;
            }


        }

        public async void GetLaserSensorData(List<AcqData> datas)
        {
            if (Init())
            {
                if (Open(enSensorType.CCS_PRIMA))
                {
                    SetParameter();
                    if (StartAcquisition())
                    {
                        await Task.Run(() =>
                        {
                            datas = Execute();
                        });
                        //Execute();
                    }
                    StopAcquisition();
                }
                Close();
            }
            Release();
        }
    }
}
