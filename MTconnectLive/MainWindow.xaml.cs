using System.IO;
using System.Windows;

 



namespace MTconnectLive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {

        Setup InitialStart;

        public MainWindow()
        {
            InitializeComponent();

            //Set up MVVM model
            InitialStart = new Setup();

            //check whether the XML data already exist, if not, create a new one. 
            if( ! (File.Exists(@"C:\Users\Chris\Desktop\DeviceXML\xmlSchema.xml")) )
                InitialStart.CreateXMLSchema();

            //the MainWindows UI will have access to all public properties from InitialStart, which are bound to the UI
            this.DataContext = InitialStart;

        }

    }


}
