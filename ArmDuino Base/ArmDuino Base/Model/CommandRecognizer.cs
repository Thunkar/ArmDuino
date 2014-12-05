using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace ArmDuino_Base.Model
{
    class CommandRecognizer : SpeechRecognitionEngine
    {
        public Dictionary<String, SpokenCommand> Commands;
 


        public CommandRecognizer() : base(new System.Globalization.CultureInfo("en-US"))
        {
            
            Commands = new Dictionary<String, SpokenCommand>();
        }

        public void reset()
        {
            Commands.Clear();
        }


        public void loadCommand(String command, SpokenCommand armcommand)
        {
            this.Commands.Add(command, armcommand);
            GrammarBuilder grBuilder = new GrammarBuilder(command);
            grBuilder.Culture =  new System.Globalization.CultureInfo("en-US");
            Grammar spokenCommandGr = new Grammar(grBuilder);
            spokenCommandGr.Name = command;
            this.LoadGrammar(spokenCommandGr);
        }

        public SpokenCommand requestCommand(String command)
        {
            SpokenCommand value;
            this.Commands.TryGetValue(command, out value);
            return value;
        }

    }
}
