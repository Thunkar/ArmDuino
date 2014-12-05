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
using Microsoft.Kinect;
using System.Speech.AudioFormat;
using System.Threading;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Interaction;
using System.Globalization;


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
        SolidColorBrush activeBrush = new SolidColorBrush(Colors.Green);
        SolidColorBrush inactiveBrush = new SolidColorBrush(Colors.Red);



        public MainWindow()
        {
            InitializeComponent();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            Timer.Tick += Timer_Tick;
            this.Closed += MainWindow_Closed;
            synth.SetOutputToDefaultAudioDevice();
            synth.SelectVoice("Microsoft Zira Desktop");
            ConnectText.Text = "Connect";
            ArmCommander = new ArmCommander(MainViewModel.Current.Arm);
            ArmCommander.loadFromFile(recognizer);
            CommandPicker.ItemsSource = recognizer.Commands.Keys;
            COM.ItemsSource = MainViewModel.Current.COMHandler.COMPorts;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }



        void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                Timer.Stop();
                MainViewModel.Current.COMHandler.isConnected = false;
                MainViewModel.Current.Arm.CurrentAngles = new int[] { 90, 90, 90, 90, 90, 90, 170 };
                MainViewModel.Current.Arm.setAngles();
                byte[] stop = { 2, 0, 1, 0, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 0, 1, 7, 0 };
                COMHandler.Port.Write(stop, 0, 24);
                COMHandler.Port.Close();
                COMHandler.Port.Dispose();
            }
            catch
            {

            }

        }


        private void InitializeSpeechRecognition()
        {
            //Activation commands
            GrammarBuilder activateBuilder = new GrammarBuilder("Ok robot, activate voice control");
            activateBuilder.Culture = new System.Globalization.CultureInfo("en-US");
            Grammar activate = new Grammar(activateBuilder);
            activate.Name = "activate";

            GrammarBuilder deactivateBuilder = new GrammarBuilder("Ok robot, deactivate voice control");
            deactivateBuilder.Culture = new System.Globalization.CultureInfo("en-US");
            Grammar deactivate = new Grammar(deactivateBuilder);
            deactivate.Name = "deactivate";

            recognizer.LoadGrammar(activate);
            recognizer.LoadGrammar(deactivate);

            GrammarBuilder gestureBuilder = new GrammarBuilder("Ok robot, activate gesture control");
            gestureBuilder.Culture = new System.Globalization.CultureInfo("en-US");
            Grammar activateGesture = new Grammar(gestureBuilder);
            activate.Name = "activateGesture";

            GrammarBuilder deGestureBuilder = new GrammarBuilder("Ok robot, deactivate gesture control");
            deGestureBuilder.Culture = new System.Globalization.CultureInfo("en-US");
            Grammar deactivateGesture = new Grammar(deGestureBuilder);
            deactivate.Name = "deactivateGesture";

            recognizer.LoadGrammar(activateGesture);
            recognizer.LoadGrammar(deactivateGesture);
            recognizer.RequestRecognizerUpdate();
            recognizer.SpeechRecognized += _recognizer_SpeechRecognized;
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            Skeleton currentSkeleton = MainViewModel.Current.KinectHandler.closestSkeleton;
            if (MainViewModel.Current.KinectHandler.Sensor != null && MainViewModel.Current.KinectHandler.Sensor.SkeletonStream.IsEnabled
                && currentSkeleton != null)
            {
                SetEllipsePosition(ellipseHead, currentSkeleton.Joints[JointType.Head], false);
                SetEllipsePosition(ellipseLeftHand, currentSkeleton.Joints[JointType.HandLeft], false);
                SetEllipsePosition(ellipseRightHand, currentSkeleton.Joints[JointType.HandRight], true);
                SetEllipsePosition(ellipseRightElbow, currentSkeleton.Joints[JointType.ElbowRight], true);
                SetEllipsePosition(ellipseRightWrist, currentSkeleton.Joints[JointType.WristRight], true);
                SetEllipsePosition(ellipseRightShoulder, currentSkeleton.Joints[JointType.ShoulderRight], true);
                SetEllipsePosition(ellipseLeftShoulder, currentSkeleton.Joints[JointType.ShoulderLeft], true);
            }
        }


        private void StartSpeechRecognition()
        {
            InitializeSpeechRecognition();
            if (MainViewModel.Current.KinectHandler.Sensor != null)
            {
                var audioSource = MainViewModel.Current.KinectHandler.Sensor.AudioSource;
                audioSource.BeamAngleMode = BeamAngleMode.Adaptive;
                var kinectStream = audioSource.Start();
                recognizer.SetInputToAudioStream(
                        kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));

            }
            else
            {
                try
                {
                    recognizer.SetInputToDefaultAudioDevice();
                }
                catch
                {
                    MessageBoxResult error = MessageBox.Show("No input device found", "Le Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (error == MessageBoxResult.OK) return;
                }
            }
            recognizer.RecognizeAsync(RecognizeMode.Multiple);

        }



        void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "Ok robot, activate voice control" && e.Result.Confidence >= 0.5)
            {
                synth.SpeakAsync("Voice control activated");
                voiceControlActivated = true;
                recognizer.reset();
                ArmCommander.loadFromFile(recognizer);
            }
            if (e.Result.Text == "Ok robot, deactivate voice control" && e.Result.Confidence >= 0.5)
            {
                synth.SpeakAsync("Voice control deactivated");
                voiceControlActivated = false;
            }
            if (e.Result.Text == "Ok robot, activate gesture control" && e.Result.Confidence >= 0.5)
            {
                synth.SpeakAsync("Gesture control activated");
                MainViewModel.Current.KinectHandler.Tracking = true;
            }
            if (e.Result.Text == "Ok robot, deactivate gesture control" && e.Result.Confidence >= 0.5)
            {
                synth.SpeakAsync("Gesture control deactivated");
                MainViewModel.Current.KinectHandler.Tracking = false;
            }
            if (voiceControlActivated == true && e.Result.Confidence >= 0.5) executeBatchCommand(e.Result.Text);
        }

        public void executeBatchCommand(String command)
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
            MainViewModel.Current.KinectMapper();
            MainViewModel.Current.Arm.updateAngles();
            if (!MainViewModel.Current.COMHandler.writeDataBytes(MainViewModel.Current.Arm)) tryDisconnection();
        }


        void Port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            String data = COMHandler.Port.ReadExisting();
            System.Diagnostics.Debug.WriteLine(data);
        }

        //This method is used to position the ellipses on the canvas
        //according to correct movements of the tracked joints.
        public void SetEllipsePosition(Ellipse ellipse, Joint joint, bool isHighlighted)
        {
            if (isHighlighted)
            {
                ellipse.Width = 10;
                ellipse.Height = 10;
                ellipse.Fill = activeBrush;
            }
            else
            {
                ellipse.Width = 10;
                ellipse.Height = 10;
                ellipse.Fill = inactiveBrush;
            }

            CoordinateMapper mapper = MainViewModel.Current.KinectHandler.Sensor.CoordinateMapper;
            var point = mapper.MapSkeletonPointToColorPoint(joint.Position, MainViewModel.Current.KinectHandler.Sensor.ColorStream.Format);
            Canvas.SetLeft(ellipse, point.X - ellipse.ActualWidth / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.ActualHeight / 2);
        }

        private void tryConnection()
        {

            MainViewModel.Current.Arm.CurrentAngles = new int[] { 90, 90, 90, 90, 90, 90, 170 };
            MainViewModel.Current.Arm.setAngles();
            try
            {
                MainViewModel.Current.COMHandler.SelectedPort = COM.SelectedItem.ToString();
                MainViewModel.Current.COMHandler.Initialize();
                COMHandler.Port.DataReceived += Port_DataReceived;
                Timer.Start();
                ConnectText.Text = "Disconnect";
            }
            catch (Exception)
            {

            }
        }

        private void tryDisconnection()
        {
            Timer.Stop();
            try
            {
                byte[] stop = { 2, 0, 1, 0, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 0, 1, 7, 0 };
                COMHandler.Port.Write(stop, 0, 24);
                COMHandler.Port.Close();
            }
            catch (Exception)
            {

            }
            ConnectText.Text = "Connect";
            MainViewModel.Current.COMHandler.isConnected = false;
            MainViewModel.Current.Arm.CurrentAngles = new int[] { 90, 90, 90, 90, 90, 90, 170 };
            MainViewModel.Current.Arm.setAngles();

        }


        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (!MainViewModel.Current.COMHandler.isConnected) tryConnection();
            else tryDisconnection();
        }

        private void StartKinectButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Current.KinectHandler.initTask.Start();
        }


        private void StartSpeechRecog_Click(object sender, RoutedEventArgs e)
        {
            this.StartSpeechRecognition();
        }

        private void toogleGestureRecognition()
        {
            MainViewModel.Current.KinectHandler.Tracking = !MainViewModel.Current.KinectHandler.Tracking;
        }

        private void StartGestureRecog_Click(object sender, RoutedEventArgs e)
        {
            toogleGestureRecognition();
        }

        private void RefreshCommands_Click(object sender, RoutedEventArgs e)
        {
            CommandPicker.ItemsSource = null;
            recognizer.reset();
            ArmCommander.loadFromFile(recognizer);
            CommandPicker.ItemsSource = recognizer.Commands.Keys;
        }

        private void ExecuteCommands_Click(object sender, RoutedEventArgs e)
        {
            if (CommandPicker.SelectedItem != null)
                executeBatchCommand(CommandPicker.SelectedItem.ToString());
        }
    }
}
