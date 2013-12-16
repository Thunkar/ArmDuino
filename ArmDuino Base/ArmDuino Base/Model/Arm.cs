using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmDuino_Base.Model
{
    class Arm : INotifyPropertyChanged
    {
        private int baseang;
        public int BaseAng
        {
            get
            {
                return baseang;
            }
            set
            {
                baseang = value;
                NotifyPropertyChanged("BaseAng");
            }
        }
        private int horizontal1ang;
        public int Horizontal1Ang
        {
            get
            {
                return horizontal1ang;
            }
            set
            {
                horizontal1ang = value;
                NotifyPropertyChanged("Horizontal1Ang");
            }
        }
        private int vertical1ang;
        public int Vertical1Ang
        {
            get
            {
                return vertical1ang;
            }
            set
            {
                vertical1ang = value;
                NotifyPropertyChanged("Vertical1Ang");
            }
        }
        private int horizontal2ang;
        public int Horizontal2Ang
        {
            get
            {
                return horizontal2ang;
            }
            set
            {
                horizontal2ang = value;
                NotifyPropertyChanged("Horizontal2Ang");
            }
        }
        private int vertical2ang;
        public int Vertical2Ang
        {
            get
            {
                return vertical2ang;
            }
            set
            {
                vertical2ang = value;
                NotifyPropertyChanged("Vertical2Ang");
            }
        }
        private int horizontal3ang;
        public int Horizontal3Ang
        {
            get
            {
                return horizontal3ang;
            }
            set
            {
                horizontal3ang = value;
                NotifyPropertyChanged("Horizontal3Ang");
            }
        }
        private int pinza;
        public int Pinza
        {
            get
            {
                return pinza;
            }
            set
            {
                pinza = value;
                NotifyPropertyChanged("Pinza");
            }
        }
        public int[] CurrentAngles;

        public Arm()
        {
            CurrentAngles = new int[7];
            BaseAng = 90;
            Horizontal1Ang = 90;
            Vertical1Ang = 90;
            Horizontal2Ang = 90;
            Vertical2Ang = 90;
            Horizontal3Ang = 90;
            Pinza = 170;
            CurrentAngles[0] = BaseAng;
            CurrentAngles[1] = Horizontal1Ang;
            CurrentAngles[2] = Vertical1Ang;
            CurrentAngles[3] = Horizontal2Ang;
            CurrentAngles[4] = Vertical2Ang;
            CurrentAngles[5] = Horizontal3Ang;
            CurrentAngles[6] = Pinza;
        }

        public void updateAngles()
        {
            CurrentAngles[0] = BaseAng;
            CurrentAngles[1] = Horizontal1Ang;
            CurrentAngles[2] = Vertical1Ang;
            CurrentAngles[3] = Horizontal2Ang;
            CurrentAngles[4] = Vertical2Ang;
            CurrentAngles[5] = Horizontal3Ang;
            CurrentAngles[6] = Pinza;
        }

        public void setAngles()
        {
            BaseAng = CurrentAngles[0];
            Horizontal1Ang = CurrentAngles[1];
            Vertical1Ang = CurrentAngles[2];
            Horizontal2Ang = CurrentAngles[3];
            Vertical2Ang = CurrentAngles[4];
            Horizontal3Ang = CurrentAngles[5];
            Pinza = CurrentAngles[6];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
