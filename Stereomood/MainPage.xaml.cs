using System.Windows;
using Microsoft.Phone.Controls;

namespace Stereomood
{
    public partial class MainPage : PhoneApplicationPage
    {
        private OauthCommunication oauthCommunication;

        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            oauthCommunication = OauthCommunication.getInstance();
            oauthCommunication.getRequestToken();
        }

        private static void loadAppBackground()
        {
            
        }
    }
}