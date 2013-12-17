using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ArmDuino_Base.Model
{
    class ArmCommander
    {
        private DispatcherTimer CommandTimer = new DispatcherTimer();
        private ArmCommand currentCommand;
        private Arm currentArm;

        public ArmCommander(Arm arm)
        {
            this.currentArm = arm;
            CommandTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            CommandTimer.Tick += CommandTimer_Tick;
        }


        public void loadAndStart(ArmCommand command)
        {
            currentCommand = (ArmCommand)command.Clone();
            CommandTimer.Start();
        }

        void CommandTimer_Tick(object sender, EventArgs e)
        {
            if (currentCommand.MovementQueue.Count != 0)
            {
                Array.Copy(currentCommand.MovementQueue.Dequeue(), currentArm.CurrentAngles, 7);
                currentArm.setAngles();
            }

            else
            {
                currentCommand = null;
                CommandTimer.Stop();
            }
        }


    }
}
