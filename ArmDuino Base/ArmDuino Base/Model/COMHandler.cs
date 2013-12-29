using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows;

namespace ArmDuino_Base.Model
{
    class COMHandler
    {
        public static SerialPort Port;
        public bool isConnected = false;
        public Arm currentArm;

        public COMHandler(Arm currentArm)
        {
            this.currentArm = currentArm;
        }

        public void Initialize()
        {
            string[] portnames = SerialPort.GetPortNames();

            while (portnames.Length == 0)
            {
                MessageBoxResult error = MessageBox.Show("No COM Port found", "Le Fail", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                if (error == MessageBoxResult.OK)
                {
                    portnames = SerialPort.GetPortNames();
                }
                else
                {
                    Application.Current.Shutdown();
                    throw new Exception("PUM");
                }
            }

            Port = new SerialPort(portnames[0], 115200);
            while (!Port.IsOpen)
            {
                try
                {
                    Port.Open();
                }
                catch (Exception)
                {
                    MessageBoxResult error = MessageBox.Show("No COM Port found", "Le Fail", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    if (error == MessageBoxResult.OK)
                    {
                        continue;
                    }
                    else
                    {
                        Application.Current.Shutdown();
                        throw new Exception("PUM");
                    }
                }
            }
        }



        public String dataToString()
        {
            String result = "";
            for (int i = 0; i < currentArm.CurrentAngles.Length; i++)
            {
                if (currentArm.CurrentAngles[i] < 10)
                {
                    result += "00" + currentArm.CurrentAngles[i];
                }
                else if (currentArm.CurrentAngles[i] < 100)
                {
                    result += "0" + currentArm.CurrentAngles[i];
                }
                else result += currentArm.CurrentAngles[i];
            }
            return result;
        }

        public byte[] dataToBytes()
        {
            char[] result = dataToString().ToCharArray();
            byte[] buffer = new byte[21];
            for (int i = 0; i < result.Length; i++)
            {
                buffer[i] = (byte)Int32.Parse(""+result[i]);
            }
            return buffer;
        }

        public void writeDataBytes()
        {
            try
            {
                byte[] buffer = dataToBytes();
                Port.Write(buffer, 0, buffer.Length);
            }
            catch(Exception)
            {
                Application.Current.Shutdown();
                throw new Exception("PUM");
            }
        }

        public void writeDataString()
        {
            Port.WriteLine(dataToString());
        }
    }
}
