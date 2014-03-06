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


        public COMHandler()
        {

        }

        public void Initialize()
        {
            string[] portnames = SerialPort.GetPortNames();

            if(portnames.Length == 0)
            {
                MessageBoxResult error = MessageBox.Show("No COM Port found", "Le Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                this.isConnected = false;
                if (error == MessageBoxResult.OK) return;
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
                    MessageBoxResult error = MessageBox.Show("Can't open serial port", "Le Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.isConnected = false;
                    if (error == MessageBoxResult.OK) return;
                }
            }
            this.isConnected = true;
        }




        public String dataToString(Arm currentArm)
        {
            String result = "200";
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

        public byte[] dataToBytes(Arm currentArm)
        {
            char[] result = dataToString(currentArm).ToCharArray();
            byte[] buffer = new byte[24];
            for (int i = 0; i < result.Length; i++)
            {
                buffer[i] = (byte)Int32.Parse(""+result[i]);
            }
            return buffer;
        }

        public void writeDataBytes(Arm currentArm)
        {
            try
            {
                byte[] buffer = dataToBytes(currentArm);
                //for (int i = 0; i < buffer.Length; i++)
                //{
                //    System.Diagnostics.Debug.Write(buffer[i]);
                //}
                //System.Diagnostics.Debug.WriteLine("");
                    Port.Write(buffer, 0, buffer.Length);
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }
        }
    }
}
