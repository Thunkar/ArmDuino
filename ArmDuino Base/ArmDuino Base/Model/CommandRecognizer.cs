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
        private string p;


        public CommandRecognizer()
        {
            Commands = new Dictionary<String, SpokenCommand>();
        }

        public CommandRecognizer(string p) : base(p)
        {
        }

        public void loadCommand(String command, SpokenCommand armcommand)
        {
            this.Commands.Add(command, armcommand);
            Grammar spokenCommandGr = new Grammar(new GrammarBuilder(command));
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
