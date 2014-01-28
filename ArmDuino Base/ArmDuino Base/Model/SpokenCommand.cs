using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArmDuino_Base.Model
{
    class SpokenCommand : ArmCommand
    {
        private String response;
        private String soundname;
        public static MediaPlayer sound = new MediaPlayer();

        public SpokenCommand() { }
        public SpokenCommand(String response, String soundname)
        {
            this.response = response;
            this.soundname = soundname;  
        }
        public SpokenCommand(String response)
        {
            this.response = response;
        }

        public void executeFurtherActions(out String response)
        {
            response = this.response;
            sound.Stop();
            if (this.soundname != null)
            {
                Uri source = new Uri("C:\\Users\\Gregorio\\Documents\\Curso 2013-2014\\Brazo Robótico\\ArmDuino\\ArmDuino Base\\ArmDuino Base\\Assets\\" + this.soundname +".wav");
                sound.Open(source);
                sound.Position = TimeSpan.Zero;
                sound.Play();
            }
        }
    }
}
