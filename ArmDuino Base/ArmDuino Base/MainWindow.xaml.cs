using ArmDuino_Base.ViewModel;
using ArmDuino_Base.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Speech;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace ArmDuino_Base
{
    public partial class MainWindow : Window
    {

        public static DispatcherTimer Timer = new DispatcherTimer();
        public char[] buffer = new char[7];
        CommandRecognizer recognizer = new CommandRecognizer();
        SpeechSynthesizer synth = new SpeechSynthesizer();
        bool voiceControlActivated = false;
        ArmCommander ArmCommander;

        public MainWindow()
        {
            InitializeComponent();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            Timer.Tick += Timer_Tick;
            MainViewModel.Current.COMHandler.Initialize();
            COMHandler.Port.DataReceived += Port_DataReceived;
            synth.SetOutputToDefaultAudioDevice();
            Grammar activate = new Grammar(new GrammarBuilder("Ok robot, activa el control por voz"));
            activate.Name = "activate";
            Grammar deactivate = new Grammar(new GrammarBuilder("Ok robot, desactiva el control por voz"));
            deactivate.Name = "deactivate";
            recognizer.LoadGrammar(activate);
            recognizer.LoadGrammar(deactivate);
            recognizer.loadCommand("Ok robot, saluda", MainViewModel.Current.Salute);
            recognizer.loadCommand("Ok robot, recoge", MainViewModel.Current.Picker);
            recognizer.loadCommand("Ok robot, ponte recto", MainViewModel.Current.Rect);
            recognizer.loadCommand("Ok robot, hazme una paja", MainViewModel.Current.Paja);
            recognizer.loadCommand("Ok robot, felicita las navidades", MainViewModel.Current.Navidades);
            recognizer.loadCommand("Ok robot, para la música", MainViewModel.Current.ParalaMusica);
            recognizer.loadCommand("Ok robot, felicítame por mi cumple", MainViewModel.Current.Cumpleaños);
            recognizer.RequestRecognizerUpdate();
            recognizer.SpeechRecognized += _recognizer_SpeechRecognized;
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }



        void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "Ok robot, activa el control por voz")
            {
                synth.SpeakAsync("Control por voz activado");
                voiceControlActivated = true;
                ArmCommander = new ArmCommander(MainViewModel.Arm);
            }
            if (e.Result.Text == "Ok robot, desactiva el control por voz")
            {
                synth.SpeakAsync("Control por voz desactivado");
                voiceControlActivated = false;
                ArmCommander = null;
            }
            if (voiceControlActivated == true && e.Result.Confidence >= 0.7) voiceControlHandler(e.Result.Text);
        }

        public void voiceControlHandler(String command)
        {
            SpokenCommand result;
            String response;
            if (recognizer.Commands.TryGetValue(command, out  result))
            {
                ArmCommander.loadAndStart(result);
                result.executeFurtherActions(out response);
                if (response != null) synth.SpeakAsync(response);
            }
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            MainViewModel.Arm.updateAngles();
            MainViewModel.Current.COMHandler.writeDataBytes();
        }


        void Port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            //String data = COMHandler.Port.ReadExisting();
            //System.Diagnostics.Debug.WriteLine(data);
            //if (data.Equals("Connected")) Connect.Content = "Connected!";
        }


        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            byte[] init = { 7, 7, 7, 7 };
            COMHandler.Port.Write(init, 0, 4);
            Timer.Start();
        }
    }
}
