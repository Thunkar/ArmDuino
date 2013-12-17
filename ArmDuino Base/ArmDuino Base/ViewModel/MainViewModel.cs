using ArmDuino_Base.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmDuino_Base;

namespace ArmDuino_Base.ViewModel
{
    class MainViewModel
    {
        public static MainViewModel Current;
        public COMHandler COMHandler { get; set; }
        public static Arm Arm { get; set; }
        public ArmCommand Picker { get; set; }
        public ArmCommand Rect { get; set; }
        public ArmCommand Salute { get; set; }
        public MainViewModel()
        {
            Current = this;
            Arm = new Arm();
            COMHandler = new COMHandler(Arm);
            loadCommands();
        }

        public void loadCommands()
        {
            //Picker
            Picker = new ArmCommand();
            Picker.MovementQueue.Enqueue(new int[] { 90, 50, 90, 10, 90, 30, 160 });
            Picker.MovementQueue.Enqueue(new int[] { 90, 50, 90, 10, 90, 30, 50 });
            Picker.MovementQueue.Enqueue(new int[] { 90, 90, 90, 30, 90, 50, 50 });
            Picker.MovementQueue.Enqueue(new int[] { 90, 90, 90, 30, 90, 30, 50 });
            Picker.MovementQueue.Enqueue(new int[] { 0, 90, 90, 30, 90, 30, 50 });
            Picker.MovementQueue.Enqueue(new int[] { 0, 90, 90, 30, 90, 30, 160 });
            //Rect
            Rect = new ArmCommand();
            Rect.MovementQueue.Enqueue(new int[] { 90, 90, 90, 90, 90, 90, 170 });
            //Salute
            Salute = new ArmCommand();
            Salute.MovementQueue.Enqueue(new int[] { 90, 130, 90, 30, 90, 30, 170 });
            Salute.MovementQueue.Enqueue(new int[] { 90, 130, 50, 30, 90, 30, 50 });
            Salute.MovementQueue.Enqueue(new int[] { 90, 130, 130, 30, 90, 30, 170 });
            Salute.MovementQueue.Enqueue(new int[] { 90, 130, 50, 30, 90, 30, 50 });
            Salute.MovementQueue.Enqueue(new int[] { 90, 130, 130, 30, 90, 30, 170 });
            Salute.MovementQueue.Enqueue(new int[] { 90, 90, 90, 90, 90, 90, 170 });
        }
    }
}
