using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArmDuino_Base.Model
{
    class KinectHandler : INotifyPropertyChanged
    {
        public KinectSensor Sensor;
        byte[] colorBytes;
        Skeleton[] skeletons;
        private ImageSource imageFromKinect;
        public ImageSource ImageFromKinect
        {
            get
            {
                return imageFromKinect;
            }
            set
            {
                imageFromKinect = value;
                NotifyPropertyChanged("ImageFromKinect");
            }
        }
        public Skeleton closestSkeleton;
        public Thread initTask;
        private bool busy;
        public bool Busy
        {
            get
            {
                return busy;
            }
            set
            {
                busy = value;
                NotifyPropertyChanged("Busy");
            }
        }

        public KinectHandler()
        {
            initTask = new Thread(new ThreadStart(InitializeSensor));
            Busy = false;
        }

        public void InitializeSensor()
        {
            Busy = true;
            Sensor = KinectSensor.KinectSensors.FirstOrDefault();
            if (Sensor == null)
            {
                MessageBox.Show("This application requires a Kinect sensor.");
                return;
            }

            Sensor.Start();

            Sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            Sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);

            Sensor.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);

            Sensor.SkeletonStream.Enable();
            Sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);

            //sensor.ElevationAngle = 10;

            Busy = false;
        }

        void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (var image = e.OpenColorImageFrame())
            {
                if (image == null)
                    return;

                if (colorBytes == null || colorBytes.Length != image.PixelDataLength)
                {
                    colorBytes = new byte[image.PixelDataLength];
                }

                image.CopyPixelDataTo(colorBytes);

                //You could use PixelFormats.Bgr32 below to ignore the alpha,
                //or if you need to set the alpha you would loop through the bytes 
                //as in this loop below
                int length = colorBytes.Length;
                for (int i = 0; i < length; i += 4)
                {
                    colorBytes[i + 3] = 255;
                }

                BitmapSource source = BitmapSource.Create(image.Width,
                    image.Height,
                    96,
                    96,
                    PixelFormats.Bgra32,
                    null,
                    colorBytes,
                    image.Width * image.BytesPerPixel);
                this.ImageFromKinect = source;
            }
        }

        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                    return;

                if (skeletons == null || skeletons.Length != skeletonFrame.SkeletonArrayLength)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                }

                skeletonFrame.CopySkeletonDataTo(skeletons);
            }

            this.closestSkeleton = skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked)
                                                .OrderBy(s => s.Position.Z * Math.Abs(s.Position.X))
                                                .FirstOrDefault();

            if (this.closestSkeleton == null)
                return;

            Joint head = closestSkeleton.Joints[JointType.Head];
            Joint rightHand = closestSkeleton.Joints[JointType.HandRight];
            Joint leftHand = closestSkeleton.Joints[JointType.HandLeft];


            if (head.TrackingState == JointTrackingState.NotTracked ||
                rightHand.TrackingState == JointTrackingState.NotTracked ||
                leftHand.TrackingState == JointTrackingState.NotTracked)
            {
                //Don't have a good read on the joints so we cannot process gestures
                return;
            }
        }

        public double RightJointsAngle(Joint leftRef, Joint rightRef, Joint joint, Skeleton skeleton)
        {
            double leftRefX = (double)(skeleton.Joints[leftRef.JointType].Position.X);
            double rightRefX = (double)(skeleton.Joints[rightRef.JointType].Position.X);
            double leftRefY = (double)(skeleton.Joints[leftRef.JointType].Position.Y);
            double rightRefY = (double)(skeleton.Joints[rightRef.JointType].Position.Y);
            double refDistance = Math.Sqrt(Math.Pow(rightRefX - leftRefX, 2) + Math.Pow(rightRefY - leftRefY, 2));
            double jointX = (double)(skeleton.Joints[joint.JointType].Position.X);
            double jointY = (double)(skeleton.Joints[joint.JointType].Position.Y);
            double leftRefToJoint = Math.Sqrt(Math.Pow(jointX - leftRefX, 2) + Math.Pow(jointY - leftRefY, 2));
            double rightRefToJoint = Math.Sqrt(Math.Pow(jointX - rightRefX, 2) + Math.Pow(jointY - rightRefY, 2));
            double angle = Math.Acos(((Math.Pow(refDistance, 2) + Math.Pow(rightRefToJoint, 2) - Math.Pow(leftRefToJoint, 2))/(2*refDistance*rightRefToJoint)));
            angle = (angle * 360) / (2 * Math.PI);
            angle -= 90;
            if (jointY > rightRefY)
            {
                angle = 180 - angle;
            }
            if (angle < 0) angle = -angle;
            if (angle > 180) angle = 180;
            return angle;
        }

        public double LeftJointsAngle(Joint leftRef, Joint rightRef, Joint joint, Skeleton skeleton)
        {
            double leftRefX = (double)(skeleton.Joints[leftRef.JointType].Position.X);
            double rightRefX = (double)(skeleton.Joints[rightRef.JointType].Position.X);
            double leftRefY = (double)(skeleton.Joints[leftRef.JointType].Position.Y);
            double rightRefY = (double)(skeleton.Joints[rightRef.JointType].Position.Y);
            double refDistance = Math.Sqrt(Math.Pow(rightRefX - leftRefX, 2) + Math.Pow(rightRefY - leftRefY, 2));
            double jointX = (double)(skeleton.Joints[joint.JointType].Position.X);
            double jointY = (double)(skeleton.Joints[joint.JointType].Position.Y);
            double leftRefToJoint = Math.Sqrt(Math.Pow(jointX - leftRefX, 2) + Math.Pow(jointY - leftRefY, 2));
            double rightRefToJoint = Math.Sqrt(Math.Pow(jointX - rightRefX, 2) + Math.Pow(jointY - rightRefY, 2));
            double angle = Math.Acos(((Math.Pow(leftRefToJoint, 2) + Math.Pow(refDistance, 2) - Math.Pow(rightRefToJoint, 2)) / (2 * leftRefToJoint * refDistance)));
            angle = (angle * 360) / (2 * Math.PI);
            angle -= 90;
            if (jointY > rightRefY)
            {
                angle = 180 - angle;
            }
            if (angle < 0) angle = -angle;
            if (angle > 180) angle = 180;
            return angle;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
