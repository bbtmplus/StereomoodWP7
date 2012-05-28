using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace TuneYourMood
{
    public partial class SettingsPage
    {
        private readonly CurrentItemCollections currentItemCollections = CurrentItemCollections.Instance();
        List<string> namesList = new List<string> { "Dawn", "Daylight", "Blue Sky", "The Web", "Reflection", "Evening", "Clowdy Sea", "Dusk&Clowds", "Flower" };

        public SettingsPage()
        {
            InitializeComponent();
        }

        private List<string> updateNamesList()
        {
            List<string> tempList = new List<string> { CurrentItemCollections.Instance().currentBackgroundKey };
            namesList.Remove(CurrentItemCollections.Instance().currentBackgroundKey);
            tempList.AddRange(namesList);
            return tempList;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            namesList = updateNamesList();
            backgroundPicker.ItemsSource = namesList;
            base.OnNavigatedTo(e);
        }

        private void RadSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (backgroundPicker != null)
            {
                currentItemCollections.currentBackgroundKey = backgroundPicker.SelectedItem.ToString();
            }

        }

        private void favDownloadUnchecked(object sender, RoutedEventArgs e)
        {

        }

        private void favDownloadChecked(object sender, RoutedEventArgs e)
        {

        }
    }
}