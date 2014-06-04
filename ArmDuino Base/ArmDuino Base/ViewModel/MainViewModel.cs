using ArmDuino_Base.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmDuino_Base;
using Microsoft.Kinect;

namespace ArmDuino_Base.ViewModel
{
    class MainViewModel
    {
        public static MainViewModel Current;
        public COMHandler COMHandler { get; set; }
        public KinectHandler KinectHandler { get; set; }
        public Arm Arm { get; set; }
        public SpokenCommand Rect { get; set; }
        public SpokenCommand Salute { get; set; }
        public SpokenCommand Navidades { get; set; }
        public SpokenCommand ParalaMusica { get; set; }
        public SpokenCommand Cumpleaños { get; set; }

        public MainViewModel()
        {
            Current = this;
            Arm = new Arm();
            COMHandler = new COMHandler();
            KinectHandler = new KinectHandler();
        }

        public void KinectMapper()
        {
            if (!KinectHandler.Tracking) return;
            Skeleton currentSkeleton = KinectHandler.closestSkeleton;
            if (KinectHandler.Sensor != null && KinectHandler.Sensor.SkeletonStream.IsEnabled
    && currentSkeleton != null)
            {
                double horizontal1angle = KinectHandler.RightJointsAngle(currentSkeleton.Joints[JointType.ShoulderLeft], currentSkeleton.Joints[JointType.ShoulderRight], currentSkeleton.Joints[JointType.ElbowRight], currentSkeleton);
                double horizontal2angle = KinectHandler.RightJointsAngle(currentSkeleton.Joints[JointType.ShoulderRight], currentSkeleton.Joints[JointType.ElbowRight], currentSkeleton.Joints[JointType.WristRight], currentSkeleton);
                double horizontal3angle = KinectHandler.RightJointsAngle(currentSkeleton.Joints[JointType.ElbowRight], currentSkeleton.Joints[JointType.WristRight], currentSkeleton.Joints[JointType.HandRight], currentSkeleton);
                Arm.Horizontal1Ang = (int)horizontal1angle;
                Arm.Horizontal2Ang = (int)horizontal2angle;
                Arm.Horizontal3Ang = (int)horizontal3angle;
                if (KinectHandler.Grip) Arm.Pinza = 90;
                else Arm.Pinza = 180;
                Arm.BaseAng = KinectHandler.GetVerticalAngle(currentSkeleton);
                Arm.Vertical1Ang = KinectHandler.GetVerticalAngle(currentSkeleton);
                Arm.Vertical2Ang = KinectHandler.GetVerticalAngle(currentSkeleton);
            }
        }
    }
}
