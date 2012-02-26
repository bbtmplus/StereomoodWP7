using Microsoft.Phone.Controls;

namespace Stereomood
{
    public partial class Settings : PhoneApplicationPage
    {
        public static bool isUserLoggedIn { get; set; }
    

        public Settings()
        {
            InitializeComponent();
        }
    }
}