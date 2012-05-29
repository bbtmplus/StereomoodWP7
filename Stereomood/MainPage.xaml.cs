using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using DeepForest.Phone.Assets.Tools;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using StereomoodPlaybackAgent;
using Telerik.Windows.Controls;
using TuneYourMood.Json;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace TuneYourMood
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly RestApiCommunication restCommunication = RestApiCommunication.getInstance();
        private EmailComposeTask emailComposeTask;
        private SearchResult searchResult;

        private bool uiBlocked;
        private bool isSearchBarVisible;
        public static ImageBrush backgroundBrush { get; set; }

        private readonly CurrentItemCollections itemCollections = CurrentItemCollections.Instance();

        private readonly Searchbar searchbar = new Searchbar();

        public MainPage()
        {
            Loaded += MainPage_Loaded;
            InitializeComponent();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                          {
                                                              searchbar.searchPressed += searchbar_searchPressed;
                                                              LayoutRoot.Children.Add(searchbar);

                                                              SetValue(RadTileAnimation.ContainerToAnimateProperty,
                                                                            selectedTagsList);
                                                              SetValue(RadTileAnimation.ContainerToAnimateProperty,
                                                                            topTagsList);

                                                              uiBlocked = true;
                                                          });
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(ReviewBugger.CheckNumOfRuns);

            if (NetworkInterface.GetIsNetworkAvailable())
            {
                searchbar.Visibility = Visibility.Collapsed;
                isSearchBarVisible = false;

                RestApiCommunication.ArrayEvent<Tag>.getArrayEventInstance().loadFinishedWithArray += loadFinishedWithArray;
                RestApiCommunication.LoadEvent<Song>.getArrayEventInstance().loadFinished += loadFinished;

                loadAppBackground();

                blockUI();
                loadTags();

                if (itemCollections.currentMood != null && itemCollections.songs != null)
                {
                    goToPlayerButton.IsEnabled = true;
                }
            }
            else
            {
                NotificationTool.Show("Offline",
                                      "Sorry, the network is not available at the moment",
                                      new NotificationAction("Okay :(", () => { }));
            }
        }

        private void loadAppBackground()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                          {
                                                              Dictionary<string, BitmapImage> backgroundBrushes =
                                                                  itemCollections.backgroundBrushes;
                                                              backgroundBrush = new ImageBrush
                                                                                    {
                                                                                        Opacity = 0.8d,
                                                                                        ImageSource =
                                                                                            backgroundBrushes[
                                                                                                itemCollections.
                                                                                                    currentBackgroundKey
                                                                                            ]
                                                                                    };
                                                              panorama.Background = backgroundBrush;
                                                          });
        }

        private void unblockUI()
        {
            customProgressOverlay.IsRunning = false;
            customProgressOverlay.Visibility = Visibility.Collapsed;
            uiBlocked = false;
        }

        private void blockUI()
        {
            customProgressOverlay.IsRunning = true;
            customProgressOverlay.Visibility = Visibility.Visible;
        }

        private void loadTags()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                               {
                                   if (itemCollections.topTags == null)
                                   {
                                       restCommunication.getTopTags();
                                   }
                                   else
                                   {
                                       List<Tag> refinedList = itemCollections.topTags.Where(
                                           tag => tag != null && tag.value != null && !tag.value.Equals("")).ToList();
                                       topTagsList.ItemsSource = refinedList;
                                       unblockUI();
                                   }
                                   if (itemCollections.selectedTags == null)
                                   {
                                       restCommunication.getSelectedTags();
                                   }
                                   else
                                   {
                                       List<Tag> refinedList = itemCollections.selectedTags.Where(
                                           tag => tag != null && tag.value != null && !tag.value.Equals("")).ToList();
                                       selectedTagsList.ItemsSource = refinedList;
                                       unblockUI();
                                   }


                                   if (itemCollections.favoritesDict == null)
                                   {
                                       itemCollections.favoritesDict = new Dictionary<string, Song>();
                                   }
                                   else
                                   {
                                       favoritesList.ItemsSource = itemCollections.favoritesDict.Values.ToList();

                                       unblockUI();
                                   }
                                   favoritesList.ItemsSource = itemCollections.favoritesDict.Values.ToList();
                               }
                           );
        }

        #region LoadFinished

        private void loadFinishedWithArray(int METHOD, Tag[] tags)
        {
            unblockUI();

            switch (METHOD)
            {
                case Constants.METHOD_SELECTED_TAGS:
                    {
                        if (tags != null)
                        {
                            selectedTagsList.ItemsSource = itemCollections.selectedTags = new List<Tag>(tags);
                        }
                        else
                        {
                            restCommunication.getSelectedTags();
                        }
                    }
                    break;
                case Constants.METHOD_TOP_TAGS:
                    {
                        if (tags != null)
                        {
                            topTagsList.ItemsSource = itemCollections.topTags = new List<Tag>(tags);
                        }
                        else
                        {
                            restCommunication.getTopTags();
                        }
                    }
                    break;
            }

        }

        private void loadFinished(int METHOD, Song[] songs, Dictionary<string, string> returnedParams)
        {
            unblockUI();

            switch (METHOD)
            {
                case Constants.METHOD_RESET:
                    {
                        break;
                    }
                case Constants.METHOD_QUITAPP:
                    {
                        break;
                    }
                case Constants.METHOD_SEARCH:
                    {

                        if (songs != null && songs.Length > 0)
                        {
                            if (returnedParams.ContainsKey("VALUE"))
                            {
                                searchResult = new SearchResult
                                                   {
                                                       trackList = songs,
                                                       tracksTotal = songs.Length
                                                   };
                                itemCollections.currentMood = new Tag
                                                                  {
                                                                      type = returnedParams["TYPE"],
                                                                      value = returnedParams["VALUE"]
                                                                  };
                            }

                            if (itemCollections.currentMood != null &&
                                !itemCollections.getSongsForTagDictionary().ContainsKey(itemCollections.currentMood.value))
                            {
                                itemCollections.getSongsForTagDictionary().Add(itemCollections.currentMood.value, songs);
                            }
                            itemCollections.songs = songs;
                            itemCollections.SaveApplicationState();
                            Uri songDetailsUri =
                                new Uri("/SongDetailsPage.xaml",
                                        UriKind.Relative);
                            NavigationService.Navigate(songDetailsUri);
                        }
                        break;
                    }
            }

        }
        #endregion

        #region ACTIONS

        private void settingsClicked(object sender, EventArgs e)
        {
            Uri uri = new Uri("/SettingsPage.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void sendEmailEvent(object sender, ManipulationCompletedEventArgs e)
        {
            if (!e.IsInertial)
            {
                emailComposeTask = new EmailComposeTask { To = "lonkly@knightswhocode.com", Subject = "Tune Your Mood" };
                emailComposeTask.Show();
            }
        }

        private void goToRina(object sender, ManipulationCompletedEventArgs e)
        {
            if (!e.IsInertial)
            {
                WebBrowserTask task = new WebBrowserTask { Uri = new Uri("http://www.flickr.com/photos/45837840@N07/", UriKind.Absolute) };
                task.Show();
            }
        }

        private void rateUs(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void goToPlayerClicked(object sender, EventArgs e)
        {
            string numberString = StorageUtility.readObjectFromFile<string>(IsolatedStorageFile.GetUserStoreForApplication(), "CurrentTrackNumber.txt");
            if (numberString != null)
                itemCollections.currentTrackNumber = Int16.Parse(numberString);

            Uri playerUri = new Uri("/SongDetailsPage.xaml", UriKind.Relative);
            NavigationService.Navigate(playerUri);
        }

        private void kwcClicked(object sender, RoutedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask { Uri = new Uri("http://knightswhocode.com/", UriKind.Absolute) };
            task.Show();
        }

        private void termsClicked(object sender, RoutedEventArgs e)
        {
            Uri playerUri = new Uri("/TermsPage.xaml", UriKind.Relative);
            NavigationService.Navigate(playerUri);
        }

        private void barItemSearchClicked(object sender, EventArgs e)
        {
            toggleSearchBar();
        }

        private void searchBarAnimationComplete(object sender, EventArgs e)
        {
            uiBlocked = false;
        }

        private void toggleSearchBar()
        {
            if (!uiBlocked)
            {
                uiBlocked = true;
                Storyboard sbMoveIn = new Storyboard();
                sbMoveIn.Completed += searchBarAnimationComplete;
                if (searchbar.Visibility == Visibility.Collapsed)
                {
                    searchbar.Visibility = Visibility.Visible;
                    MoveIn(searchbar, sbMoveIn, true);
                    isSearchBarVisible = true;
                }
                else
                {
                    isSearchBarVisible = isSearchBarVisible != true;
                    MoveIn(searchbar, sbMoveIn, isSearchBarVisible);
                }
            }
        }

        private void searchbar_searchPressed(string textToSearch)
        {
            if (stringNotEmpty(textToSearch))
            {
                blockUI();
                Deployment.Current.Dispatcher.BeginInvoke(() => restCommunication.searchSongs(Constants.TYPE_SITE, textToSearch));
            }
        }

        #endregion

        #region EVENTS

        private void topTagSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            blockUI();
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                Tag tag = (Tag)listBox.SelectedItem;
                if (tag != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => restCommunication.searchSongs(tag));
                    itemCollections.currentMood = tag;
                    itemCollections.currentTrackNumber = 0;
                }
            }

        }

        private void selectedTagsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            blockUI();
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                Tag tag = (Tag)listBox.SelectedItem;
                if (tag != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => restCommunication.searchSongs(tag));
                    itemCollections.currentMood = tag;
                    itemCollections.currentTrackNumber = 0;
                }
            }

        }

        private void favoritesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (favoritesList.SelectedIndex != -1)
            {
                Song[] songs =
                    itemCollections.favoritesDict.Values.ToArray();
                if (songs.Length > 0)
                {
                    searchResult = new SearchResult
                                       {
                                           trackList = songs,
                                           tracksTotal = songs.Length
                                       };
                    itemCollections.currentMood = new Tag
                                                      {
                                                          type =
                                                              Constants.
                                                              TYPE_ACTIVITY,
                                                          value =
                                                              "favorites!"
                                                      };
                    if (itemCollections.getSongsForTagDictionary().ContainsKey(itemCollections.currentMood.value))
                    {
                        itemCollections.getSongsForTagDictionary().Remove(itemCollections.currentMood.value);
                    }

                    itemCollections.getSongsForTagDictionary().Add(
                        itemCollections.currentMood.value, songs);

                    itemCollections.currentTrackNumber = favoritesList.SelectedIndex;
                    itemCollections.SaveApplicationState();
                    StorageUtility.writeStringToFile(IsolatedStorageFile.GetUserStoreForApplication(),
                "CurrentTrackNumber.txt", itemCollections.currentTrackNumber.ToString(CultureInfo.InvariantCulture));

                    Uri songDetailsUri = new Uri("/SongDetailsPage.xaml",
                                                 UriKind.Relative);
                    NavigationService.Navigate(songDetailsUri);
                }
            }

        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (isSearchBarVisible && !uiBlocked)
            {
                toggleSearchBar();
                e.Cancel = true;
            }
            base.OnBackKeyPress(e);
        }

        private void syncPressed(object sender, EventArgs eventArgs)
        {
            itemCollections.LoadApplicationState();
            if (itemCollections.favoritesDict != null)
            {
                favoritesList.ItemsSource = itemCollections.favoritesDict.Values.ToList();
            }
        }

        #endregion

        private void MoveIn(UIElement source, Storyboard sb, bool moveIn)
        {
            int startValue = moveIn ? -200 : 0;
            int endValue = moveIn ? 0 : -200;
            DoubleAnimationUsingKeyFrames animationFirstY = new DoubleAnimationUsingKeyFrames();
            source.RenderTransform = new CompositeTransform();
            Storyboard.SetTargetProperty(animationFirstY, new PropertyPath(CompositeTransform.TranslateYProperty));
            Storyboard.SetTarget(animationFirstY, source.RenderTransform);
            animationFirstY.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.Zero, Value = startValue });
            animationFirstY.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromSeconds(0.2), Value = endValue });
            sb.Children.Add(animationFirstY);
            sb.Begin();
        }

        private bool stringNotEmpty(string expr)
        {
            return Regex.IsMatch(expr, "[^ ]");
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            topTagsList.SelectedIndex = -1;
            selectedTagsList.SelectedIndex = -1;
            favoritesList.SelectedIndex = -1;
            base.OnNavigatedFrom(e);
        }

    }
}