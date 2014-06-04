using System;
using System.Collections.Generic;
using System.IO;
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

       

        public void loadFromFile(CommandRecognizer recognizer)
        {
            string path = Directory.GetCurrentDirectory()+"//commander";
            string file = path + "//commands.conf";
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if(!File.Exists(file))
            {
                File.Create(file);
            }
            StreamReader configFile = new StreamReader(file);
            while(!configFile.EndOfStream)
            {
                string line = configFile.ReadLine();
                if(line.StartsWith("COMMAND"))
                {
                    char[] separator = { ';' };
                    String[] title = line.Split(separator);
                    string name = title[1].Replace(";", "");
                    name = name.Trim();
                    string response = title[2].Replace(";", "");
                    response = response.Trim();
                    string sound = null;
                    if(title.Length > 3)
                    {
                        sound = title[3].Replace(";", "");
                        sound = sound.Trim();
                    }
                    SpokenCommand command = new SpokenCommand(response, sound);
                    line = configFile.ReadLine();
                    while(line.StartsWith("KEYFRAME"))
                    {
                        line = line.Replace(" ", "");
                        String[] nums = line.Split(separator);
                        int[] data = new int[7];
                        for(int i = 1; i < data.Length+1; i++)
                        {
                            int servoCode = Int32.Parse(nums[i]);
                            data[i-1] = servoCode;
                        }
                        command.MovementQueue.Enqueue(data);
                        line = configFile.ReadLine();
                    }
                    recognizer.loadCommand(name, command);
                    line = configFile.ReadLine();
                }
            }
            configFile.Close();
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
