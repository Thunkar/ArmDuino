using ArmDuino_Base.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmDuino_Base.ViewModel
{
    class MainViewModel
    {
        public static MainViewModel Current;
        public COMHandler COMHandler { get; set; }
        public Arm Arm { get; set; }
        public MainViewModel()
        {
            Current = this;
            Arm = new Arm();
            COMHandler = new COMHandler();
        }
    }
}
