using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmDuino_Base.Model
{
    class ArmCommand : ICloneable
    {
        public Queue<int[]> MovementQueue;

        public ArmCommand()
        {
            MovementQueue = new Queue<int[]>();
        }

        public ArmCommand(ArmCommand other)
        {
            MovementQueue = other.MovementQueue;
        }

        public object Clone()
        {
            ArmCommand newCommand = new ArmCommand();
            newCommand.MovementQueue = new Queue<int[]>(this.MovementQueue);
            return newCommand;
        }
    }
}
