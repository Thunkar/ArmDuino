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
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;

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
            MainViewModel.Current.COMHandler.Initialize();
            COMHandler.Port.DataReceived += Port_DataReceived;
            synth.SetOutputToDefaultAudioDevice();
            InitializeSpeechRecognition();
            this.Closed += MainWindow_Closed;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            COMHandler.Port.Close();
            COMHandler.Port.Dispose();
        }

        public void KinectMapper()
        {
            if (MainViewModel.Current.KinectHandler.Sensor != null && MainViewModel.Current.KinectHandler.Sensor.SkeletonStream.IsEnabled
    && MainViewModel.Current.KinectHandler.closestSkeleton != null)
            {
                double horizontal1angle = MainViewModel.Current.KinectHandler.RightJointsAngle(MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.ShoulderLeft], MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.ShoulderRight], MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.ElbowRight], MainViewModel.Current.KinectHandler.closestSkeleton);
                double horizontal2angle = MainViewModel.Current.KinectHandler.RightJointsAngle(MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.ShoulderRight], MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.ElbowRight], MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.WristRight], MainViewModel.Current.KinectHandler.closestSkeleton);
                double horizontal3angle = MainViewModel.Current.KinectHandler.RightJointsAngle(MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.ElbowRight], MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.WristRight], MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.HandRight], MainViewModel.Current.KinectHandler.closestSkeleton);
                double pinzaAngle = MainViewModel.Current.KinectHandler.LeftJointsAngle(MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.ElbowLeft], MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.ShoulderLeft], MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.WristLeft], MainViewModel.Current.KinectHandler.closestSkeleton);
                MainViewModel.Arm.Horizontal1Ang = (int)horizontal1angle;
                MainViewModel.Arm.Horizontal2Ang = (int)horizontal2angle;
                MainViewModel.Arm.Horizontal3Ang = (int)horizontal3angle;
                MainViewModel.Arm.Pinza = (int)pinzaAngle;
                System.Diagnostics.Debug.WriteLine(horizontal2angle);
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
            recognizer.loadCommand("Ok llarvis, felicita a mi hermano por su cumple", MainViewModel.Current.Cumpleaños);
            recognizer.RequestRecognizerUpdate();
            recognizer.SpeechRecognized += _recognizer_SpeechRecognized;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (MainViewModel.Current.KinectHandler.Sensor != null && MainViewModel.Current.KinectHandler.Sensor.SkeletonStream.IsEnabled 
                && MainViewModel.Current.KinectHandler.closestSkeleton != null)
            {
                SetEllipsePosition(ellipseHead, MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.Head], false);
                SetEllipsePosition(ellipseLeftHand, MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.HandLeft], false);
                SetEllipsePosition(ellipseRightHand, MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.HandRight], true);
                SetEllipsePosition(ellipseRightElbow, MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.ElbowRight], true);
                SetEllipsePosition(ellipseRightWrist, MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.WristRight], true);
                SetEllipsePosition(ellipseRightShoulder, MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.ShoulderRight], true);
                SetEllipsePosition(ellipseLeftShoulder, MainViewModel.Current.KinectHandler.closestSkeleton.Joints[JointType.ShoulderLeft], true);
            } 
        }


        private void StartSpeechRecognition()
        {
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
                recognizer.SetInputToDefaultAudioDevice();
            }
            recognizer.RecognizeAsync(RecognizeMode.Multiple);

        }



        void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "Ok llarvis, activa el control por voz" && e.Result.Confidence >= 0.5)
            {
                synth.SpeakAsync("Control por voz activado");
                voiceControlActivated = true;
                ArmCommander = new ArmCommander(MainViewModel.Arm);
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
            MainViewModel.Arm.updateAngles();
            MainViewModel.Current.COMHandler.writeDataBytes();
        }


        void Port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            //String data = COMHandler.Port.ReadExisting();
            //System.Diagnostics.Debug.WriteLine(data);
            //if (data.Equals("Connected")) Connect.Content = "Connected!";
        }

        //This method is used to position the ellipses on the canvas
        //according to correct movements of the tracked joints.
        public void SetEllipsePosition(Ellipse ellipse, Joint joint, bool isHighlighted)
        {
            if (isHighlighted)
            {
                ellipse.Width = 30;
                ellipse.Height = 30;
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
            byte[] init = { 7, 7, 7, 7 };
            COMHandler.Port.Write(init, 0, 4);
            Timer.Start();
        }

        private void StartKinectButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Current.KinectHandler.InitializeSensor();
            this.StartSpeechRecognition();
        }
    }
}
