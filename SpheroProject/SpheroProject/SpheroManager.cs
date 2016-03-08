using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Popups;
using RobotKit;
using System.ComponentModel;
namespace SpheroProject
{
    class SpheroManager : INotifyPropertyChanged
    {
        public Sphero m_robot = null;
        public string SpheroName
        {
            get { return spheroName; }
            set
            {
                spheroName = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SpheroName"));
                }
            }
        }

        private string spheroName = "";



        private string _GyroReading;
        public string GyroReading
        {
            get
            {
                return _GyroReading;
            }
            set
            {
                _GyroReading = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("GyroReading"));
                }
            }
        }


        private bool spheroConnected = false;

        private bool calibrationMode = false;

        public int calibrationAngle = 0;

        public bool SpheroConnected
        {
            get { return spheroConnected; }
            set
            {
                spheroConnected = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SpheroConnected"));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public SpheroManager()
        {
            //SetupRobotConnection();
        }



        private const string kNoSpheroConnected = "No Sphero Connected";

        //! @brief  the default string to show when connecting to a sphero ({0})
        private const string kConnectingToSphero = "Connecting to {0}";

        //! @brief  the default string to show when connected to a sphero ({0})
        private const string kSpheroConnected = "Connected to {0}";


        //! @brief  search for a robot to connect to
        public void SetupRobotConnection()
        {
            SpheroName = "No Sphero Connected";

            RobotProvider provider = RobotProvider.GetSharedProvider();
            provider.DiscoveredRobotEvent += OnRobotDiscovered;
            provider.NoRobotsEvent += OnNoRobotsEvent;
            provider.ConnectedRobotEvent += OnRobotConnected;
            provider.FindRobots();
        }
        public void OnRobotDiscovered(object sender, Robot robot)
        {
           
            if (m_robot == null)
            {
                RobotProvider provider = RobotProvider.GetSharedProvider();
                provider.ConnectRobot(robot);
        
                m_robot = (Sphero)robot;
          
                SpheroConnected = true;
                SpheroName = string.Format(kConnectingToSphero, robot.BluetoothName);
            }
        }




        private void OnNoRobotsEvent(object sender, EventArgs e)
        {
            SpheroName = "No Sphero Connected";
        }


        //! @brief  when a robot is connected, get ready to drive!
        private void OnRobotConnected(object sender, Robot robot)
        {
            //Debug.WriteLine(string.Format("Connected to {0}", robot));
            //ConnectionToggle.IsOn = true;
            // ConnectionToggle.OnContent = "Connected";
            SpheroConnected = true;
            m_robot.SetRGBLED(255, 255, 255);
            SpheroName = string.Format(kSpheroConnected, robot.BluetoothName);
            SpheroName = "Sphero Connected";

            m_robot.SensorControl.Hz = 10;

            m_robot.SensorControl.GyrometerUpdatedEvent += SensorControl_GyrometerUpdatedEvent;

            //m_robot.CollisionControl.StartDetectionForWallCollisions();
            //m_robot.CollisionControl.CollisionDetectedEvent += OnCollisionDetected;
        }

        private void SensorControl_GyrometerUpdatedEvent(object sender, GyrometerReading e)
        {
            GyroReading = string.Format("X:{0}" + Environment.NewLine + "Y:{1}" + Environment.NewLine + "Z:{2}", e.X, e.Y, e.Z);
        }

        public void ShutdownRobotConnection()
        {
            if (m_robot != null)
            {
                try
                {
                    m_robot.SensorControl.StopAll();
                    m_robot.Sleep();
                }
                catch { }
                // temporary while I work on Disconnect.
                //m_robot.Disconnect();
                //ConnectionToggle.OffContent = "Disconnected";
                SpheroConnected = false;
                SpheroName = kNoSpheroConnected;

                //m_robot.SensorControl.AccelerometerUpdatedEvent -= OnAccelerometerUpdated;
                //m_robot.SensorControl.GyrometerUpdatedEvent -= OnGyrometerUpdated;

                //m_robot.CollisionControl.StopDetection();
                //m_robot.CollisionControl.CollisionDetectedEvent -= OnCollisionDetected;

                RobotProvider provider = RobotProvider.GetSharedProvider();
                provider.DiscoveredRobotEvent -= OnRobotDiscovered;
                provider.NoRobotsEvent -= OnNoRobotsEvent;
                provider.ConnectedRobotEvent -= OnRobotConnected;
                SpheroName = "Sphero Disconnected";
            }
        }


        //public bool RobotNull()
        //{
        //    return m_robot == null;
            
        //}

        //public void Roll(int h, float f)
        //{
        //    if (m_robot != null)
        //    {
        //        m_robot.Roll(h, f);
        //    }
        //}
        //public void SetHeading(int h)
        //{
        //    if(m_robot!=null)
        //    {
        //       // calibrationAngle += h;
        //        m_robot.SetHeading(0);
        //    }
        //}

        //public void Calibrate(bool b)
        //{
        //    if (m_robot != null)
        //    {
        //        if (b)
        //            m_robot.SetBackLED(100f);
        //        else
        //            m_robot.SetBackLED(0f);
        //        calibrationMode = b;
        //    }
        //}
        //public bool inCalibration()
        //{
        //    return calibrationMode;
        //}
    }
}