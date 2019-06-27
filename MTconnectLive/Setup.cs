using System;
using System.Linq;
using MTConnectDevices = MTConnect.MTConnectDevices;
using MTConnectStreams = MTConnect.MTConnectStreams;
using MTConnect.Clients;
using System.ComponentModel;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MTconnectLive
{
    /// <summary>
    /// Setup class by using MVVM Model
    /// </summary>
    public class Setup : INotifyPropertyChanged
    {

        #region private Properties 
        /// <summary>
        /// "now" has the access to the function of class "Current" 
        /// </summary>
        private Current now;
        /// <summary>
        /// it receives data, which is sented from "now" 
        /// </summary>
        private MTConnectStreams.Document current;
        /// <summary>
        /// it has access to the function of "MTConnectClient" class
        /// </summary>
        MTConnectClient client;
        /// <summary>
        /// path that save XML data
        /// </summary>
        private string path;

        /// <summary>
        /// set the start reading index from xml data
        /// </summary>
        private int _indexXml = 0;

        private ObservableCollection<string> Avail;
        private ObservableCollection<string> RotaryA;
        private ObservableCollection<string> RotaryC;
        private ObservableCollection<string> LinearX;
        private ObservableCollection<string> LinearY;
        private ObservableCollection<string> LinearZ;


        private ObservableCollection<string> maxRotaryA;
        private ObservableCollection<string> maxRotaryC;
        private ObservableCollection<string> maxLinearX;
        private ObservableCollection<string> maxLinearY;
        private ObservableCollection<string> maxLinearZ;

        private XDocument FirstDevice;
        #endregion

        #region Constructor for Setup
        public Setup()
        {
            //Avail for 2 simulated devices
            Avail = new ObservableCollection<string>(new string[2]);

            RotaryA = new ObservableCollection<string>(new string[3]);
            RotaryC = new ObservableCollection<string>(new string[3]);
            LinearX = new ObservableCollection<string>(new string[3]);
            LinearY = new ObservableCollection<string>(new string[3]);
            LinearZ = new ObservableCollection<string>(new string[3]);

            maxRotaryA = new ObservableCollection<string>(new string[3]);
            maxRotaryC = new ObservableCollection<string>(new string[3]);
            maxLinearX = new ObservableCollection<string>(new string[3]);
            maxLinearY = new ObservableCollection<string>(new string[3]);
            maxLinearZ = new ObservableCollection<string>(new string[3]);

            FirstDevice = XDocument.Load(@"C:\Users\Chris\Desktop\DeviceXML\xmlSchema.xml");

            //fire the Start Function
            Start();
            //Set the XML saving Position manually
            path = @"C:\Users\Chris\Desktop\DeviceXML\xmlSchema.xml"; 
        }


        #endregion

        #region Start function including first data setup and Timer-setup
        /// <summary>
        /// it setup the MTConnectClient and get data from CurrentStream
        /// it also setup the timer, for data update and add XML data
        /// </summary>
        public void Start()
        {
            // The base address for the MTConnect Agent
            string baseUrl = "https://smstestbed.nist.gov/vds/";

            // Create a new MTConnectClient using the baseUrl
            client = new MTConnectClient(baseUrl);

            // Subscribe to the Event handlers to receive the MTConnect documents
            client.ProbeReceived += DevicesSuccessful;
            client.CurrentReceived += StreamsSuccessful;
            client.SampleReceived += StreamsSuccessful;

            // Start the MTConnectClient
            client.Start();

            now = new Current(baseUrl);

            //make sure _current has data document to avoid the error from finding DataItems.
            while (current == null) { current = now.Execute(); }
            
            //set the first data, which can be showed to UI and later compared with new data.
            RotaryA[0] = current.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-A_2").CDATA;
            _maxRotaryA[0] = current.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-A_2").CDATA;

            RotaryC[0] = current.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-C_2").CDATA;
            _maxRotaryC[0] = current.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-C_2").CDATA;

            LinearX[0] = current.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-X_2").CDATA;
            _maxLinearX[0] = current.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-X_2").CDATA;

            LinearY[0] = current.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-Y_2").CDATA;
            _maxLinearY[0] = current.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-Y_2").CDATA;

            LinearZ[0] = current.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-Z_2").CDATA;
            _maxLinearZ[0] = current.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-Z_2").CDATA;

            //for the simulated device , set the first data
            Avail[0] = FirstDevice
                       .Element("DeviceStream")
                       .Elements("Events")
                       .Where(x => x.Attribute("name").Value == "Availability").Descendants().ElementAt(_indexXml).Value;

            RotaryA[1] = FirstDevice
                           .Element("DeviceStream")
                           .Elements("ComponentStream")
                           .Where(x => x.Attribute("name").Value == "Rotary(A)").Descendants().ElementAt(_indexXml).Value;
            _maxRotaryA[1] = FirstDevice
                           .Element("DeviceStream")
                           .Elements("ComponentStream")
                           .Where(x => x.Attribute("name").Value == "Rotary(A)").Descendants().ElementAt(_indexXml).Value;

            RotaryC[1] = FirstDevice
                           .Element("DeviceStream")
                           .Elements("ComponentStream")
                           .Where(x => x.Attribute("name").Value == "Rotary(C)").Descendants().ElementAt(_indexXml).Value;
            _maxRotaryC[1] = FirstDevice
                           .Element("DeviceStream")
                           .Elements("ComponentStream")
                           .Where(x => x.Attribute("name").Value == "Rotary(C)").Descendants().ElementAt(_indexXml).Value;

            LinearX[1] = FirstDevice
                           .Element("DeviceStream")
                           .Elements("ComponentStream")
                           .Where(x => x.Attribute("name").Value == "Linear(X)").Descendants().ElementAt(_indexXml).Value;
            LinearY[1] = FirstDevice
                           .Element("DeviceStream")
                           .Elements("ComponentStream")
                           .Where(x => x.Attribute("name").Value == "Linear(Y)").Descendants().ElementAt(_indexXml).Value;
            LinearZ[1] = FirstDevice
                           .Element("DeviceStream")
                           .Elements("ComponentStream")
                           .Where(x => x.Attribute("name").Value == "Linear(Z)").Descendants().ElementAt(_indexXml).Value;
            _maxLinearX[1] = FirstDevice
                           .Element("DeviceStream")
                           .Elements("ComponentStream")
                           .Where(x => x.Attribute("name").Value == "Linear(X)").Descendants().ElementAt(_indexXml).Value;
            _maxLinearY[1] = FirstDevice
                           .Element("DeviceStream")
                           .Elements("ComponentStream")
                           .Where(x => x.Attribute("name").Value == "Linear(Y)").Descendants().ElementAt(_indexXml).Value;
            _maxLinearZ[1] = FirstDevice
                           .Element("DeviceStream")
                           .Elements("ComponentStream")
                           .Where(x => x.Attribute("name").Value == "Linear(Z)").Descendants().ElementAt(_indexXml).Value;

            //set the first data as Maximum Data, bound to UI

            //_maxRotaryA = RotaryA;
            //_maxRotaryC = RotaryC;
            //_maxLinearX = LinearX;
            //_maxLinearY = LinearY;
            //_maxLinearZ = LinearZ;

            //for simulated device, bound to UI
            _Avail = Avail;

            //request new data by 3 sec by using DispatcherTimer
            //it will update the current data
            //it contains functions, which will set the Maximum Date
            //it will add the newest data to XML-data
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
            dispatcherTimer.Start();

        }
        #endregion

        #region Timer for request new Data and adding to XML Schema
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            MTConnectStreams.Document Newcurrent = null;
            //make sure "current" has data document to avoid error 
            while (Newcurrent == null) { Newcurrent = now.Execute(); }
            //give new Current Data to current-variable
            current = Newcurrent;

            //get new Axis Data, A,C-Angle and X,Y,Z-Position 
            var NewRotaryA = current.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-A_2").CDATA;
            var NewRotaryC = current.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-C_2").CDATA;
            var NewLinearX = current.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-X_2").CDATA;
            var NewLinearY = current.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-Y_2").CDATA;
            var NewLinearZ = current.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-Z_2").CDATA;


            //Set new current data to _current and it will fire the OnPropertyChanged Event, so update the UI
            _current = current;

            //compare the New Data with older one, when new data is larger, it will replace the old data 
            //and it will fire the OnPropertyChanged Event, so update the UI
            //New Data shouldn't equal to UNAVAILABLE when executing compare function
            if (Math.Abs(double.Parse(NewRotaryA)) > Math.Abs(double.Parse(_maxRotaryA[0])) && NewRotaryA != "UNAVAILABLE")
                _maxRotaryA[0] = NewRotaryA;
            if (Math.Abs(double.Parse(NewRotaryC)) > Math.Abs(double.Parse(_maxRotaryC[0])) && NewRotaryC != "UNAVAILABLE")
                _maxRotaryC[0] = NewRotaryC;
            if (Math.Abs(double.Parse(NewLinearX)) > Math.Abs(double.Parse(_maxLinearX[0])) && NewLinearX != "UNAVAILABLE")
                _maxLinearX[0] = NewLinearX;
            if (Math.Abs(double.Parse(NewLinearY)) > Math.Abs(double.Parse(_maxLinearY[0])) && NewLinearY != "UNAVAILABLE")
                _maxLinearY[0] = NewLinearY;
            if (Math.Abs(double.Parse(NewLinearZ)) > Math.Abs(double.Parse(_maxLinearZ[0])) && NewLinearZ != "UNAVAILABLE")
                _maxLinearZ[0] = NewLinearZ;

            //Add new data as XML data from path
            AddXmlData(_current, path);

            //read data from XML and show to UI
            ReadXML(path);

        }
        #endregion

        #region Public Properties, which are bound to UI
        /// <summary>
        /// it save the current data
        /// </summary>
        public MTConnectStreams.Document _current
        {
            get {return current; }
            set
            {
                //Update _current data to UI
                if (current != value)
                    current = value;
                OnPropertyChanged("_current");
            }
        }
        /// <summary>
        /// Maximun Value from Rotary A
        /// </summary>
        public ObservableCollection<string> _maxRotaryA
        {

            get
            {
                return maxRotaryA;
            }
            set
            {
                ////Update RotaryA data to UI
                //if (RotaryA != value)
                //    RotaryA = value;
                //OnPropertyChanged("_maxRotaryA");
               
            }
        }
        /// <summary>
        /// Maximum Value from Rotary C
        /// </summary>
        public ObservableCollection<string> _maxRotaryC
        {

            get
            {
                return maxRotaryC;
            }
            set
            {
                //Update RotaryA data to UI
                //if (RotaryC != value)
                //    RotaryC = value;
                //OnPropertyChanged("_maxRotaryC");

            }
        }
        /// <summary>
        /// Maximum Value from Linear X
        /// </summary>
        public ObservableCollection<string> _maxLinearX
        {

            get
            {
                return maxLinearX;
            }
            set
            {
                ////Update LinearX data to UI
                // when the older value of LinearX isn't equal to new value, it will update to the new one.

                //if (LinearX != value)
                //    LinearX = value;
                //OnPropertyChanged("_maxLinearX");

            }
        }
        /// <summary>
        /// Maximum Value from Linear Y
        /// </summary>
        public ObservableCollection<string> _maxLinearY
        {

            get
            {
                return maxLinearY;
            }
            set
            {
                //////Update LinearY data to UI
                //// when the older value of LinearX isn't equal to new value, it will update to the new one.
                //if (LinearY != value)
                //    LinearY = value;
                //OnPropertyChanged("_maxLinearY");

            }
        }
        /// <summary>
        /// Maximum Value from Linear Z
        /// </summary>
        public ObservableCollection<string> _maxLinearZ
        {

            get
            {
                return maxLinearZ;
            }
            set
            {
                ////Update LinearZ data to UI
                // when the older value of LinearX isn't equal to new value, it will update to the new one.
                //if (LinearZ != value)
                //    LinearZ = value;
                //OnPropertyChanged("_maxLinearZ");

            }
        }

        public ObservableCollection<string> _RotaryA { get { return RotaryA; } set { } }
        public ObservableCollection<string> _RotaryC { get { return RotaryC; } set { } }
        public ObservableCollection<string> _LinearX { get { return LinearX; } set { } }
        public ObservableCollection<string> _LinearY { get { return LinearY; } set { } }
        public ObservableCollection<string> _LinearZ { get { return LinearZ; } set { } }
        public ObservableCollection<string> _Avail { get { return Avail; } set { } }
        #endregion

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
             
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region create and update XML Data function
        /// <summary>
        /// function for XML Schema creation
        /// </summary>
        public void CreateXMLSchema()
        {
            
            XDocument xmlSchema = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement("DeviceStream", new XAttribute("name", "GFAgie02"),
                                             new XAttribute("model", "HPM600U"),
                                             new XAttribute("manufacturer", "Agie Charmilles"),
                                             new XAttribute("Betreiber", "(SMS) Test Bed"),
                        new XElement("Events", new XAttribute("name", "Availability")),
                        new XElement("ComponentStream", new XAttribute("name", "Rotary(A)")),
                        new XElement("ComponentStream", new XAttribute("name", "Rotary(C)")),
                        new XElement("ComponentStream", new XAttribute("name", "Linear(X)")),
                        new XElement("ComponentStream", new XAttribute("name", "Linear(Y)")),
                        new XElement("ComponentStream", new XAttribute("name", "Linear(Z)"))
                         ));

            xmlSchema.Save(@"C:\Users\Chris\Desktop\DeviceXML\xmlSchema.xml");
        }

        /// <summary>
        /// Add new XML data to the existed XML-Schema
        /// </summary>
        /// <param name="document">it contains the newest data</param>
        /// <param name="Xmlpath">the position where XML Data will be saved</param>
        public void AddXmlData (MTConnectStreams.Document document ,string Xmlpath )
        {
            //find and save DataItem from document as variable

            var avail = document.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-dtop_1");
            var RotaryA = document.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-A_2");
            var RotaryC = document.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-C_2");
            var LinearX = document.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-X_2");
            var LinearY = document.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-Y_2");
            var LinearZ = document.DeviceStreams[0].DataItems.Find(o => o.DataItemId == "GFAgie01-Z_2");

            //Load XML datai from path
            XDocument XmlData = XDocument.Load(Xmlpath);

            //Add "Availability" Live Data in XML-Schema
            XmlData.Element("DeviceStream")
                   .Elements("Events")
                   .Where(x => x.Attribute("name").Value == "Availability").LastOrDefault()
                   .Add(
                        new XElement("Availability", new XAttribute("timestamp", avail.Timestamp),new XAttribute("name", avail.Name),
                                    
                                avail.CDATA));

            //Add "Angle(A)" Live Data in XML-Schema
            XmlData.Element("DeviceStream")
                   .Elements("ComponentStream")
                   .Where(x => x.Attribute("name").Value == "Rotary(A)").LastOrDefault()
                   .Add(
                        new XElement("Angle", new XAttribute("timestamp", RotaryA.Timestamp), new XAttribute("name", RotaryA.Name),

                    RotaryA.CDATA));

            //Add "Angle(C)" Live Data in XML-Schema
            XmlData.Element("DeviceStream")
                   .Elements("ComponentStream")
                   .Where(x => x.Attribute("name").Value == "Rotary(C)").LastOrDefault()
                   .Add(
                        new XElement("Angle", new XAttribute("timestamp", RotaryC.Timestamp), new XAttribute("name", RotaryC.Name),

                    RotaryC.CDATA));

            //Add "Position(X)" Live Data in XML-Schema
            XmlData.Element("DeviceStream")
                   .Elements("ComponentStream")
                   .Where(x => x.Attribute("name").Value == "Linear(X)").LastOrDefault()
                   .Add(
                        new XElement("Position", new XAttribute("timestamp", LinearX.Timestamp), new XAttribute("name", LinearX.Name),

                    LinearX.CDATA));

            //Add "Position(Y)" Live Data in XML-Schema
            XmlData.Element("DeviceStream")
                   .Elements("ComponentStream")
                   .Where(x => x.Attribute("name").Value == "Linear(Y)").LastOrDefault()
                   .Add(
                        new XElement("Position", new XAttribute("timestamp", LinearY.Timestamp), new XAttribute("name", LinearY.Name),

                    LinearY.CDATA));

            //Add "Position(Z)" Live Data in XML-Schema
            XmlData.Element("DeviceStream")
                   .Elements("ComponentStream")
                   .Where(x => x.Attribute("name").Value == "Linear(Z)").LastOrDefault()
                   .Add(
                        new XElement("Position", new XAttribute("timestamp", LinearZ.Timestamp), new XAttribute("name", LinearZ.Name),

                    LinearZ.CDATA));

            //save XML data to XML path
            XmlData.Save(Xmlpath);
        }

        public void ReadXML (string Xmlpath1)
        {
            
            //read next element in XML data
            _indexXml++;
            //the first simulated device data update
            Avail[0] = FirstDevice
                       .Element("DeviceStream")
                       .Elements("Events")
                       .Where(x => x.Attribute("name").Value == "Availability").Descendants().ElementAt(_indexXml).Value;

            RotaryA[1] = FirstDevice
                           .Element("DeviceStream")
                           .Elements("ComponentStream")
                           .Where(x => x.Attribute("name").Value == "Rotary(A)").Descendants().ElementAt(_indexXml).Value;
            RotaryC[1] = FirstDevice
                           .Element("DeviceStream")
                           .Elements("ComponentStream")
                           .Where(x => x.Attribute("name").Value == "Rotary(C)").Descendants().ElementAt(_indexXml).Value;
            LinearX[1] = FirstDevice
                           .Element("DeviceStream")
                           .Elements("ComponentStream")
                           .Where(x => x.Attribute("name").Value == "Linear(X)").Descendants().ElementAt(_indexXml).Value;
            LinearY[1] = FirstDevice
                           .Element("DeviceStream")
                           .Elements("ComponentStream")
                           .Where(x => x.Attribute("name").Value == "Linear(Y)").Descendants().ElementAt(_indexXml).Value;
            LinearZ[1] = FirstDevice
                           .Element("DeviceStream")
                           .Elements("ComponentStream")
                           .Where(x => x.Attribute("name").Value == "Linear(Z)").Descendants().ElementAt(_indexXml).Value;

            //compare the New Data with older one, when new data is larger, it will replace the old data 
            //and it will fire the OnPropertyChanged Event, so update the UI
            //New Data shouldn't equal to UNAVAILABLE when executing compare function
            if ((Math.Abs(double.Parse(RotaryA[1])) > Math.Abs(double.Parse(_maxRotaryA[1]))) && RotaryA[1] != "UNAVAILABLE")
                _maxRotaryA[1] = RotaryA[1];
            if ((Math.Abs(double.Parse(RotaryC[1])) > Math.Abs(double.Parse(_maxRotaryC[1]))) && RotaryC[1] != "UNAVAILABLE")
                _maxRotaryC[1] = RotaryC[1];
            if ((Math.Abs(double.Parse(LinearX[1])) > Math.Abs(double.Parse(_maxLinearX[1]))) && LinearX[1] != "UNAVAILABLE")
                _maxLinearX[1] = LinearX[1];
            if ((Math.Abs(double.Parse(LinearY[1])) > Math.Abs(double.Parse(_maxLinearY[1]))) && LinearY[1] != "UNAVAILABLE")
                _maxLinearY[1] = LinearY[1];
            if ((Math.Abs(double.Parse(LinearZ[1])) > Math.Abs(double.Parse(_maxLinearZ[1]))) && LinearZ[1] != "UNAVAILABLE")
                _maxLinearZ[1] = LinearZ[1];


        }


        #endregion

        #region other functions
        public void Stop()
            {
                client.Stop();
            }

        // --- Event Handlers ---

        public void DevicesSuccessful(MTConnectDevices.Document document)
        {
            foreach (var device in document.Devices)
            {
                var dataItems = device.GetDataItems();
                foreach (var dataItem in dataItems) Console.WriteLine(dataItem.Id + " : " + dataItem.Name);
            }
        }

        public void StreamsSuccessful(MTConnectStreams.Document document)
        {
            foreach (var deviceStream in document.DeviceStreams)
            {
                foreach (var dataItem in deviceStream.DataItems) Console.WriteLine(dataItem.DataItemId + " = " + dataItem.CDATA);
            }
        }
        #endregion


    }
}
