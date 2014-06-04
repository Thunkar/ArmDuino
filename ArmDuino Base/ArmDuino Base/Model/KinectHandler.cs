using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;
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
using System.Windows.Media.Media3D;
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
        private bool tracking;
        public bool Tracking
        {
            get
            {
                return tracking;
            }
            set
            {
                tracking = value;
                NotifyPropertyChanged("Traking");
            }
        }
        public int Tilt
        {
            get
            {
                if (Sensor != null)
                {
                    return Sensor.ElevationAngle;
                }
                else return 0;
            }
            set
            {
                if(Sensor != null)
                {
                    try
                    {
                        Sensor.ElevationAngle = value;
                        NotifyPropertyChanged("Tilt");
                    }
                    catch(Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Too fast!");
                    }
                }
            }
        }
        public InteractionStream interactionStream;
        private bool grip;
        public bool Grip
        {
            get
            {
                return grip;
            }
            set
            {
                grip = value;
                NotifyPropertyChanged("Grip");
            }
        }
        private UserInfo[] userInfos;

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
                MessageBoxResult error = MessageBox.Show("This feature requires a Kinect for Windows sensor. Note that if you have a Kinect for Xbox360 sensor, you can still use it from Visual Studio (get the source code!)", "Le Fail", MessageBoxButton.OK, MessageBoxImage.Error);
                if (error == MessageBoxResult.OK)
                {
                    Busy = false;
                    return;
                }
            }

            Sensor.Start();
            Sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            Sensor.SkeletonStream.Enable();
            Sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

            Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
                    Sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
                    Sensor.DepthFrameReady += Sensor_DepthFrameReady;
                    this.interactionStream = new InteractionStream(Sensor, new DummyInteractionClient());
                    this.interactionStream.InteractionFrameReady += interactionStream_InteractionFrameReady;
                }
                ));
            Busy = false;
        }

        void Sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null)
                    return;
                interactionStream.ProcessDepth(depthFrame.GetRawPixelData(), depthFrame.Timestamp);
            }
        }

        void interactionStream_InteractionFrameReady(object sender, InteractionFrameReadyEventArgs e)
        {
            using (InteractionFrame frame = e.OpenInteractionFrame())
            {
                if (frame != null)
                {
                    if (this.userInfos == null)
                    {
                        this.userInfos = new UserInfo[InteractionFrame.UserInfoArrayLength];
                    }

                    frame.CopyInteractionDataTo(this.userInfos);
                }
                else
                {
                    return;
                }
            }



            foreach (UserInfo userInfo in this.userInfos)
            {
                foreach (InteractionHandPointer handPointer in userInfo.HandPointers)
                {
                    string action = null;

                    switch (handPointer.HandEventType)
                    {
                        case InteractionHandEventType.Grip:
                            action = "gripped";
                            break;

                        case InteractionHandEventType.GripRelease:
                            action = "released";

                            break;
                    }

                    if (action != null)
                    {
                        string handSide = "unknown";

                        switch (handPointer.HandType)
                        {
                            case InteractionHandType.Left:
                                handSide = "left";
                                break;

                            case InteractionHandType.Right:
                                handSide = "right";
                                break;
                        }

                        if (handSide == "left")
                        {
                            if (action == "released")
                            {
                                Grip = false;
                            }
                            else Grip = true;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
        }


        public void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
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

        public void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
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

                interactionStream.ProcessSkeleton(skeletons, Sensor.AccelerometerGetCurrentReading(), skeletonFrame.Timestamp);


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
            double argAcos = ((Math.Pow(refDistance, 2) + Math.Pow(rightRefToJoint, 2) - Math.Pow(leftRefToJoint, 2)) / (2 * refDistance * rightRefToJoint));
            if (argAcos > 1) argAcos = 1;
            if (argAcos < -1) argAcos = -1;
            double angle = Math.Acos(argAcos);
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

        public int GetVerticalAngle(Skeleton skeleton)
        {
            float shoulderZ = skeleton.Joints[JointType.ShoulderLeft].Position.Z;
            float handZ = skeleton.Joints[JointType.HandLeft].Position.Z;
            float angle = (float)Math.Sqrt(Math.Abs(shoulderZ * shoulderZ - handZ * handZ));
            angle *= 100;
            if (angle > 180f) return 180;
            if (angle < 0f) return 0;
            angle = (angle * 180f) / 130f;
            System.Diagnostics.Debug.WriteLine(angle);
            return (int)angle;
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
            double argAcos = ((Math.Pow(leftRefToJoint, 2) + Math.Pow(refDistance, 2) - Math.Pow(rightRefToJoint, 2)) / (2f * leftRefToJoint * refDistance));
            if (argAcos > 1) argAcos = 1;
            if (argAcos < -1) argAcos = -1;
            double angle = Math.Acos(argAcos);
            angle = (angle * 360f) / (2f * Math.PI);
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
