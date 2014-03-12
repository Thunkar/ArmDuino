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
            ConnectText.Text = "Connect";
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

        public void KinectMapper()
        {
            Skeleton currentSkeleton = MainViewModel.Current.KinectHandler.closestSkeleton;
            if (MainViewModel.Current.KinectHandler.Sensor != null && MainViewModel.Current.KinectHandler.Sensor.SkeletonStream.IsEnabled
    && currentSkeleton != null)
            {
                double horizontal1angle = MainViewModel.Current.KinectHandler.RightJointsAngle(currentSkeleton.Joints[JointType.ShoulderLeft], currentSkeleton.Joints[JointType.ShoulderRight], currentSkeleton.Joints[JointType.ElbowRight], currentSkeleton);
                double horizontal2angle = MainViewModel.Current.KinectHandler.RightJointsAngle(currentSkeleton.Joints[JointType.ShoulderRight], currentSkeleton.Joints[JointType.ElbowRight], currentSkeleton.Joints[JointType.WristRight], currentSkeleton);
                double horizontal3angle = MainViewModel.Current.KinectHandler.RightJointsAngle(currentSkeleton.Joints[JointType.ElbowRight], currentSkeleton.Joints[JointType.WristRight], currentSkeleton.Joints[JointType.HandRight], currentSkeleton);
                double pinzaAngle = MainViewModel.Current.KinectHandler.LeftJointsAngle(currentSkeleton.Joints[JointType.ElbowLeft], currentSkeleton.Joints[JointType.ShoulderLeft], currentSkeleton.Joints[JointType.WristLeft], currentSkeleton);
                MainViewModel.Current.Arm.Horizontal1Ang = (int)horizontal1angle;
                MainViewModel.Current.Arm.Horizontal2Ang = (int)horizontal2angle;
                MainViewModel.Current.Arm.Horizontal3Ang = (int)horizontal3angle;
                MainViewModel.Current.Arm.Pinza = (int)pinzaAngle;
                MainViewModel.Current.Arm.BaseAng = (int)MainViewModel.Current.KinectHandler.computeRotation(currentSkeleton);
            }
        }


        private void InitializeSpeechRecognition()
        {
            //Activation commands
            Grammar activate = new Grammar(new GrammarBuilder("Ok llarvis, activa el control por voz"));
            activate.Name = "activate";
            Grammar deactivate = new Grammar(new GrammarBuilder("Ok llarvis, desactiva el control por voz"));
            deactivate.Name = "deactivate";
            recognizer.LoadGrammar(activate);
            recognizer.LoadGrammar(deactivate);
            //Robotic arm commands
            recognizer.loadCommand("Ok llarvis, saluda", MainViewModel.Current.Salute);
            recognizer.loadCommand("Ok llarvis, recoge", MainViewModel.Current.Picker);
            recognizer.loadCommand("Ok llarvis, ponte recto", MainViewModel.Current.Rect);
            recognizer.loadCommand("Ok robot, hazme una paja", MainViewModel.Current.Paja);
            recognizer.loadCommand("Ok llarvis, felicita las navidades", MainViewModel.Current.Navidades);
            recognizer.loadCommand("Ok llarvis, para la música", MainViewModel.Current.ParalaMusica);
            recognizer.loadCommand("Ok llarvis, felicítame por mi cumpleaños", MainViewModel.Current.Cumpleaños);
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
            if (e.Result.Text == "Ok llarvis, activa el control por voz" && e.Result.Confidence >= 0.5)
            {
                synth.SpeakAsync("Control por voz activado");
                voiceControlActivated = true;
                ArmCommander = new ArmCommander(MainViewModel.Current.Arm);
            }
            if (e.Result.Text == "Ok llarvis, desactiva el control por voz" && e.Result.Confidence >= 0.5)
            {
                synth.SpeakAsync("Control por voz desactivado");
                voiceControlActivated = false;
                ArmCommander = null;
            }
            if (voiceControlActivated == true && e.Result.Confidence >= 0.6) voiceControlHandler(e.Result.Text);
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
            KinectMapper();
            MainViewModel.Current.Arm.updateAngles();
            MainViewModel.Current.COMHandler.writeDataBytes(MainViewModel.Current.Arm);
        }


        void Port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            String data = COMHandler.Port.ReadExisting();
            System.Diagnostics.Debug.WriteLine(data);
            //if (data.Equals("Connected")) Connect.Content = "Connected!";
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


        private void Connect_Click(object sender, RoutedEventArgs e)
        {

            if (!MainViewModel.Current.COMHandler.isConnected)
            {
                MainViewModel.Current.Arm.CurrentAngles = new int[] { 90, 90, 90, 90, 90, 90, 170 };
                MainViewModel.Current.Arm.setAngles();
                MainViewModel.Current.COMHandler.Initialize();
                COMHandler.Port.DataReceived += Port_DataReceived;
                Timer.Start();
                ConnectText.Text = "Disconnect";
            }
            else
            {
                Timer.Stop();
                MainViewModel.Current.COMHandler.isConnected = false;
                MainViewModel.Current.Arm.CurrentAngles = new int[] {90,90,90,90,90,90,170};
                MainViewModel.Current.Arm.setAngles();
                byte[] stop = { 2, 0, 1, 0, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 0, 0, 9, 0, 1, 7, 0 };
                COMHandler.Port.Write(stop, 0, 24);
                COMHandler.Port.Close();
                ConnectText.Text = "Connect";
            }

        }

        private void StartKinectButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Current.KinectHandler.initTask.Start();
        }


        private void StartSpeechRecog_Click(object sender, RoutedEventArgs e)
        {
            this.StartSpeechRecognition();
        }
    }
}
