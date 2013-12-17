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
        SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();
        SpeechSynthesizer synth = new SpeechSynthesizer();
        bool voiceControlActivated = false;
        ArmCommander ArmCommander;

        public MainWindow()
        {
            InitializeComponent();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            Timer.Tick += Timer_Tick;
            COMHandler.Port.DataReceived += Port_DataReceived;
            synth.SetOutputToDefaultAudioDevice();
            Grammar activate = new Grammar(new GrammarBuilder("Ok robot, activa el control por voz"));
            activate.Name = "activate";
            Grammar deactivate = new Grammar(new GrammarBuilder("Ok robot, desactiva el control por voz"));
            deactivate.Name = "deactivate";
            Grammar rect = new Grammar(new GrammarBuilder("Ponte recto"));
            rect.Name = "rect";
            Grammar recoge = new Grammar(new GrammarBuilder("Recoge"));
            rect.Name = "recoge";
            Grammar salute = new Grammar(new GrammarBuilder("Saluda"));
            salute.Name = "saluda";
            recognizer.LoadGrammar(recoge);
            recognizer.LoadGrammar(activate);
            recognizer.LoadGrammar(deactivate);
            recognizer.LoadGrammar(rect);
            recognizer.LoadGrammar(salute);
            recognizer.RequestRecognizerUpdate();
            recognizer.SpeechRecognized += _recognizer_SpeechRecognized; 
            recognizer.SetInputToDefaultAudioDevice(); // set the input of the speech recognizer to the default audio device
            recognizer.RecognizeAsync(RecognizeMode.Multiple); // recognize speech asynchronous
        }

        void SpeechTimer_Tick(object sender, EventArgs e)
        {
            
        }

        void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "Ok robot, activa el control por voz" && e.Result.Confidence >= 0.7) // e.Result.Text contains the recognized text
            {
                synth.SpeakAsync("Control por voz activado");
                voiceControlActivated = true;
                ArmCommander = new ArmCommander(MainViewModel.Arm);
            }
            if (e.Result.Text == "Ok robot, desactiva el control por voz" && e.Result.Confidence >= 0.7)
            {
                synth.SpeakAsync("Control por voz desactivado");
                voiceControlActivated = false;
                ArmCommander = null;
            }
            if (voiceControlActivated == true && e.Result.Confidence >= 0.7) voiceControlHandler(e.Result.Text);
        }

        public void voiceControlHandler(String command)
        {
            switch (command)
            {
                case "Ponte recto":
                    {
                        synth.SpeakAsync("Ejecutando comando");
                        ArmCommander.loadAndStart(MainViewModel.Current.Rect);
                        break;
                    }
                case "Recoge":
                    {
                        synth.SpeakAsync("Ejecutando comando");
                        ArmCommander.loadAndStart(MainViewModel.Current.Picker);
                        break;
                    }
                case "Saluda":
                    {
                        synth.SpeakAsync("Hola");
                        ArmCommander.loadAndStart(MainViewModel.Current.Salute);
                        break;
                    }
            }
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            MainViewModel.Arm.updateAngles()
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
