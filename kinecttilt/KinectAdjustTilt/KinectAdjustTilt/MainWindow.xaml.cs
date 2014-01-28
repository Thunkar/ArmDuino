using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace KinectExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            setupKinect();
        }

        KinectSensor sensor;

        private void setupKinect()
        {
            if (KinectSensor.KinectSensors.Count == 0)
            {
                this.Title = "No Kinect Detected";
            }
            else
            {
                sensor = KinectSensor.KinectSensors[0];

                //Initialize(RuntimeOptions.UseColor | RuntimeOptions.UseDepth);
                sensor.Start();

                sensor.ColorFrameReady += sensor_ColorFrameReady;

                sensor.ColorStream.Enable();


                sliderAngle.Value = sensor.ElevationAngle;
            }
        }

        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            ColorImageFrame colorFrame = e.OpenColorImageFrame();
            if (colorFrame != null)
            {
                byte[] pixelData = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixelData);
                BitmapSource source = BitmapSource.Create(640,480,96,96,PixelFormats.Bgr32, null, pixelData, 640*4);
                imgKinect.Source = source;
            }
        }




        private void btnSetAngle_Click(object sender, RoutedEventArgs e)
        {
            sensor.ElevationAngle = (int)sliderAngle.Value;
        }

        private void sliderAngle_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblSliderValue.Content = (int)sliderAngle.Value;
        }

    }

}
