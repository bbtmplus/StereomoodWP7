using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Tasks;

namespace TuneYourMood
{
    public partial class BuyNowControl : UserControl
    {

        private readonly Popup popup = null;

        public BuyNowControl(Popup popup)
        {
            InitializeComponent();
            this.popup = popup;
        }

        private void buyNowClicked(object sender, RoutedEventArgs e)
        {
            MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.Show();
            this.ClosePopup();
        }

        private void continueTrialClicked(object sender, RoutedEventArgs e)
        {
            this.ClosePopup();
            this.ClosePopup();
        }

        private void ClosePopup()
        {
            if (this.popup != null)
            {
                this.popup.IsOpen = false;
            }
        }
    }
}
