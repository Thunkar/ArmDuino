using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ArmDuino_Base.Model
{
    class COMHandler : INotifyPropertyChanged
    {
        public static SerialPort Port;
        public bool isConnected = false;
        public ObservableCollection<string> COMPorts;
        private string selectedPort;
        public string SelectedPort
        {
            get
            {
                return selectedPort;
            }
            set
            {
                selectedPort = value;
                NotifyPropertyChanged("SelectedPort");
            }
        }


        public COMHandler()
        {
            COMPorts = new ObservableCollection<string>();
            refresh();
        }

        public void refresh()
        {
            COMPorts.Clear();
            string[] portnames = SerialPort.GetPortNames();
            foreach (string name in portnames)
            {
                COMPorts.Add(name);
            }
        }

        public void Initialize()
        {

            if(COMPorts.Count == 0)
            {
                MessageBoxResult error = MessageBox.Show("No COM Port found", "Le Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                this.isConnected = false;
                if (error == MessageBoxResult.OK) return;
            }
            Port = new SerialPort(selectedPort, 115200);
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

        public bool writeDataBytes(Arm currentArm)
        {
            try
            {
                byte[] buffer = dataToBytes(currentArm);
                Port.Write(buffer, 0, buffer.Length);
                return true;
            }
            catch(Exception e)
            {
                isConnected = false;
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
