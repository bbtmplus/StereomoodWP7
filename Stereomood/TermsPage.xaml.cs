using System;
using System.IO;
using System.Windows;
using Microsoft.Phone.Controls;

namespace TuneYourMood
{
    public partial class TermsPage : PhoneApplicationPage
    {
        public TermsPage()
        {
            InitializeComponent();
            var resource = Application.GetResourceStream(new Uri(@"/TuneYourMood;component/OtherResources/TermsOfUse.txt", UriKind.Relative));

            StreamReader streamReader = new StreamReader(resource.Stream);
            string x = streamReader.ReadToEnd();
            termsOfUse.Text = x;
        }
    }
}