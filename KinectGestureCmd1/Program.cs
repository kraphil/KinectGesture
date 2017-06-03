namespace KinectGestureCmd
{
    using System;
    using System.Linq;
    using Microsoft.Kinect;
    using System.IO;

    internal class Program
    {
        private static void Main(string[] args)
        {
            //Find Kinect sensor
            KinectSensor sensor = KinectSensor.KinectSensors.Where(s => s.Status == KinectStatus.Connected).FirstOrDefault();
            if (sensor == null)
            {
                Console.WriteLine("Kinect not found!");
                return;
            }
            else if (sensor != null)
            {
                Console.WriteLine(KinectSensor.KinectSensors.Count() + " Kinect Sensor(s) found");
            }

            // Skeleton-Tracker for sensor
            Tracker tracker = new Tracker(sensor);

            // Start sensor         
            sensor.Start();

            //Press 's' or 'S' to finish the program
            while (Char.ToLowerInvariant(Console.ReadKey().KeyChar) != 's') { }

            // Stop sensor
           sensor.Stop();
        }
    }

    internal class Tracker
    {
        private Skeleton[] skeletons = null;

        public Tracker(KinectSensor sensor)
        {
            // Start Skeleton-Tracking 
            sensor.SkeletonFrameReady += SensorSkeletonFrameReady;
            sensor.SkeletonStream.Enable();
        }

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Access to skeleton
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if (this.skeletons == null)
                    {
                        
                        this.skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    
                    skeletonFrame.CopySkeletonDataTo(this.skeletons);

                    
                    Skeleton skeleton = this.skeletons.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();

                    if (skeleton != null)
                    {
                        // Prints X, Y and Z coordinates to console if right hand is recognized
                        Joint RightHand = skeleton.Joints[JointType.HandRight];

                        if (RightHand.TrackingState == JointTrackingState.Tracked)
                        {
                            Console.WriteLine("Right Hand:  X:" + RightHand.Position.X + ", Y:" + RightHand.Position.Y + ", Z:" + RightHand.Position.Z);
                           
                        }
                    }
                }
            }
        }
    }
}
