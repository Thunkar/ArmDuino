using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Interaction;


namespace ArmDuino_Base.Model
{
     public class DummyInteractionClient : IInteractionClient
    {
        public InteractionInfo GetInteractionInfoAtLocation(
            int skeletonTrackingId, 
            InteractionHandType handType, 
            double x, 
            double y)
        {
            return new InteractionInfo
            {
                IsPressTarget = false,
                IsGripTarget = true,
            };
        }
    }
}
