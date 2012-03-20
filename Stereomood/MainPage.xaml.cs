using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using DeepForest.Phone.Assets.Tools;
using Microsoft.Phone.Controls;
using Stereomood.Json;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace Stereomood
{
    public partial class MainPage : PhoneApplicationPage
    {
        private OauthCommunication oauthCommunication;
        private Dictionary<string, string> parameters;

        private List<Tag> topTags;
        private List<Tag> selectedTags;

        private SearchResult searchResult;

        private bool uiBlocked;
        private bool isUserLoggedIn;
        private bool isSearchBarVisible;

        private readonly Searchbar searchbar = new Searchbar();


        public MainPage()
        {
            InitializeComponent();
            InitializeElements();

            uiBlocked = true;
            Loaded += MainPage_Loaded;
        }

        private void InitializeElements()
        {
            searchbar.searchPressed += searchbar_searchPressed;
            searchbar.Visibility = Visibility.Collapsed;
            isSearchBarVisible = false;
            LayoutRoot.Children.Add(searchbar);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                oauthCommunication = OauthCommunication.getInstance();
                oauthCommunication.loadFinished += loadFinished;
                OauthCommunication.ArrayEvent<Tag>.getArrayEventInstance().loadFinishedWithArray +=
                    loadFinishedWithArray;

                if (!Settings.isUserLoggedIn)
                {
                    loadAppBackground();
                    oauthCommunication.getRequestToken();
                }
                else
                {

                }
            }
            else
            {
                NotificationTool.Show("Offline",
                                      "Sorry, the network is not available at the moment",
                                      new NotificationAction("Okay :(", () => { throw new Exception("ExitApp"); }));
            }
        }

        private void loadFinishedWithArray(int METHOD, Tag[] tags)
        {
            switch (METHOD)
            {
                case Constants.METHOD_SELECTED_TAGS:
                    {
                        selectedTags = new List<Tag>();
                        selectedTags.AddRange(tags);
                        selectedTagsList.ItemsSource = selectedTags;
                        progressOverlay.Visibility = Visibility.Collapsed;
                        uiBlocked = false;
                    }
                    break;
                case Constants.METHOD_TOP_TAGS:
                    {
                        topTags = new List<Tag>();
                        topTags.AddRange(tags);
                        topTagsList.ItemsSource = topTags;
                        progressOverlay.Visibility = Visibility.Collapsed;
                        uiBlocked = false;
                    }
                    break;
            }

        }

        private void loadAppBackground()
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(Constants.BACKGROUND_URL));
            ImageBrush brush = new ImageBrush
                                   {
                                       Opacity = 0.8d,
                                       ImageSource = bitmapImage
                                   };
            panorama.Background = brush;
        }

        void webBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            oauthCommunication = OauthCommunication.getInstance();
        }



        private void loadFinished(int METHOD, Dictionary<string, string> returnedParams, JsonObject jsonObject)
        {
            switch (METHOD)
            {
                case Constants.METHOD_AUTHORIZATION:
                    {
                        parameters = returnedParams;
                        webBrowser1.LoadCompleted += webbrowser_LoginPageLoaded;
                        webBrowser1.Navigating += webBrowser_Navigating;
                        webBrowser1.IsScriptEnabled = true;
                        webBrowser1.Navigate(new Uri(parameters["URL"]));
                        break;
                    }
                case Constants.METHOD_ACCESS_TOKEN:
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                                      {
                                                                          oauthCommunication.getTopTags();
                                                                          oauthCommunication.getSelectedTags();
                                                                      }
                            );
                        break;
                    }
                case Constants.METHOD_SEARCH:
                    {
                        searchResult = ((SearchResult)jsonObject);
                        if (searchResult.total > 0)
                        {
                            Uri songListUri = new Uri("/SongListPage.xaml", UriKind.Relative);
                            NavigationService.Navigate(songListUri);
                        }
                        else
                        {
                            NotificationTool.Show("Sorry",
                                                  "Couldn't find anything. Could you be more prescice?",
                                                  new NotificationAction("Okay :(", () => { }));
                        }

                        break;
                    }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SongListPage songListPage = (e.Content as SongListPage);
            if (songListPage != null)
                songListPage.searchResult = searchResult;
            base.OnNavigatedFrom(e);
        }

        private void webbrowser_LoginPageLoaded(object sender, NavigationEventArgs e)
        {
            if (e.Uri.Equals(parameters["URL"]) && !isUserLoggedIn)
            {
                isUserLoggedIn = true;
                webBrowser1.InvokeScript("eval", "document.getElementById('username').value='lonkly';");
                webBrowser1.InvokeScript("eval", "document.getElementById('user_pass').value='Lonkly303';");

                webBrowser1.InvokeScript("eval", "getLogin = function() { var links = document.getElementsByTagName(\"a\");for(var i = 0; i < links.length; i++) {" +
                                                 "if (links[i].className==\"submit_login submit_action\"){" +
                                                 "return links[i]}" +
                                                 "}" +
                                                 "}");
                webBrowser1.InvokeScript("eval", "var button = getLogin(); button.click();");
                webBrowser1.LoadCompleted -= webbrowser_LoginPageLoaded;
                webBrowser1.LoadCompleted += webbrowser_FilledLoginCredentials;
            }
        }

        private void webbrowser_FilledLoginCredentials(object sender, NavigationEventArgs e)
        {
            webBrowser1.LoadCompleted -= webbrowser_FilledLoginCredentials;
            webBrowser1.LoadCompleted += webbrowser_FindPincode;
            webBrowser1.InvokeScript("eval", "allowButton = function() { var links = document.getElementsByTagName(\"a\");for(var i = 0; i < links.length; i++) {" +
                                  "if (links[i].className==\"linkSubmitReq\"){" +
                                  "return links[i]}" +
                                  "}" +
                                  "}");
            try
            {
                webBrowser1.InvokeScript("eval", "var button = allowButton(); button.click();");
            }
            catch (SystemException ex)
            {
                webbrowser_FindPincode(null, null);
            }

        }

        private void webbrowser_FindPincode(object sender, NavigationEventArgs e)
        {
            string content = webBrowser1.SaveToString();
            int indexOf = content.IndexOf("PIN:", StringComparison.OrdinalIgnoreCase);

            string pinSubstring = content.Substring(indexOf);
            int endIndex = pinSubstring.IndexOf("</h1>", StringComparison.OrdinalIgnoreCase);
            string pinString = pinSubstring.Remove(endIndex);
            string pin = ExtractNumbers(pinString);

            OauthCommunication.getInstance().getAccessToken(pin);
        }


        #region ACTIONS

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
                sbMoveIn.Completed += new EventHandler(searchBarAnimationComplete);
                if (searchbar.Visibility == Visibility.Collapsed)
                {
                    searchbar.Visibility = Visibility.Visible;
                    MoveIn(searchbar, sbMoveIn, true);
                }
                else
                {
                    MoveIn(searchbar, sbMoveIn, isSearchBarVisible);
                    isSearchBarVisible = isSearchBarVisible != true;
                }
            }

        }

        private void searchbar_searchPressed(string textToSearch)
        {
            if (stringNotEmpty(textToSearch))
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => oauthCommunication.searchSong(Constants.TYPE_SITE, textToSearch));
            }
        }

        private void topTagSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var itemContainerGenerator = this.topTagsList.ItemContainerGenerator;
            if (itemContainerGenerator != null)
            {
                ListBoxItem selectedItem = itemContainerGenerator.ContainerFromIndex(2) as ListBoxItem;
                if (selectedItem != null)
                {
                    Tag data = selectedItem.DataContext as Tag;
                    if (data != null)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                                  oauthCommunication.searchSong(
                                                                      data.type,
                                                                      data.value));
                    }
                }

            }
        }

        private void selectedTagsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var itemContainerGenerator = this.selectedTagsList.ItemContainerGenerator;
            if (itemContainerGenerator != null)
            {
                ListBoxItem selectedItem = itemContainerGenerator.ContainerFromIndex(2) as ListBoxItem;
                if (selectedItem != null)
                {
                    Tag data = selectedItem.DataContext as Tag;
                    if (data != null)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                                  oauthCommunication.searchSong(data.type, data.value));
                    }
                }

            }

        }

        private void favoritesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var itemContainerGenerator = this.selectedTagsList.ItemContainerGenerator;
            if (itemContainerGenerator != null)
            {
                ListBoxItem selectedItem = itemContainerGenerator.ContainerFromIndex(2) as ListBoxItem;
                if (selectedItem != null)
                {
                    Tag data = selectedItem.DataContext as Tag;
                    if (data != null)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(
                            () => oauthCommunication.searchSong(data.type,
                                                                data.value));
                    }
                }
            }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (!isSearchBarVisible)
            {
                toggleSearchBar();
            }
            e.Cancel = true;
        }

        #endregion


        #region UTILS

        private void FadeInOut(DependencyObject target, Storyboard sb, bool isFadeIn)
        {
            Duration d = new Duration(TimeSpan.FromSeconds(1));
            DoubleAnimation daFade = new DoubleAnimation { Duration = d };
            if (isFadeIn)
            {
                daFade.From = 1.00;
                daFade.To = 0.00;
            }
            else
            {
                daFade.From = 0.00;
                daFade.To = 1.00;
            }

            sb.Duration = d;
            sb.Children.Add(daFade);
            Storyboard.SetTarget(daFade, target);
            Storyboard.SetTargetProperty(daFade, new PropertyPath("Opacity"));

            sb.Begin();
        }

        private void MoveIn(UIElement source, Storyboard sb, bool moveIn)
        {
            int startValue = moveIn == true ? -170 : 0;
            int endValue = moveIn == true ? 0 : -170;
            DoubleAnimationUsingKeyFrames animationFirstY = new DoubleAnimationUsingKeyFrames();
            source.RenderTransform = new CompositeTransform();
            Storyboard.SetTargetProperty(animationFirstY, new PropertyPath(CompositeTransform.TranslateYProperty));
            Storyboard.SetTarget(animationFirstY, source.RenderTransform);
            animationFirstY.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = TimeSpan.Zero, Value = startValue });
            animationFirstY.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromSeconds(0.2), Value = endValue });
            sb.Children.Add(animationFirstY);
            sb.Begin();
        }

        private string ExtractNumbers(string expr)
        {
            return string.Join(null, Regex.Split(expr, "[^\\d]"));
        }

        private bool stringNotEmpty(string expr)
        {
            return Regex.IsMatch(expr, "[^ ]");
        }

        #endregion


    }
}