using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using DeepForest.Phone.Assets.Tools;
using Microsoft.Phone.Controls;
using TuneYourMood.Json;
using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;

namespace TuneYourMood
{
    public partial class MainPage : PhoneApplicationPage
    {
        private OauthCommunication oauthCommunication;
        private Dictionary<string, string> parameters;

        private SearchResult searchResult;

        private bool uiBlocked;
        private bool isUserLoggedIn;
        private bool isSearchBarVisible;
        public static ImageBrush backgroundBrush { get; set; }

        private readonly Searchbar searchbar = new Searchbar();


        public MainPage()
        {
            InitializeComponent();
            InitializeElements();

            LayoutRoot.Children.Add(searchbar);

            uiBlocked = true;
        }

        private void InitializeElements()
        {
            searchbar.searchPressed += searchbar_searchPressed;
        }

        private void loadAppBackground()
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri(Constants.BACKGROUND_URL));
            //  backgroundBrush = new ImageBrush
            //  {
            //     Opacity = 0.8d,
            //      ImageSource = bitmapImage
            //  };
            // panorama.Background = backgroundBrush;
        }

        private void loadTags()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                               {
                                   if (CurrentItemCollections.Instance().topTags == null)
                                   {
                                       oauthCommunication.getTopTags();
                                   }
                                   else
                                   {
                                       topTagsList.ItemsSource = CurrentItemCollections.Instance().topTags;
                                       customProgressOverlay.Hide();
                                       uiBlocked = false;
                                   }
                                   if (CurrentItemCollections.Instance().selectedTags == null)
                                   {
                                       oauthCommunication.getSelectedTags();
                                   }
                                   else
                                   {
                                       selectedTagsList.ItemsSource = CurrentItemCollections.Instance().selectedTags;
                                       customProgressOverlay.Hide();
                                       uiBlocked = false;
                                   }
                               }
                           );
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                searchbar.Visibility = Visibility.Collapsed;
                isSearchBarVisible = false;

                oauthCommunication = OauthCommunication.getInstance();
                oauthCommunication.loadFinished += loadFinished;
                OauthCommunication.ArrayEvent<Tag>.getArrayEventInstance().loadFinishedWithArray +=
                    loadFinishedWithArray;

                InitializeElements();

                if (!oauthCommunication.isConnected())
                {
                    loadAppBackground();
                    oauthCommunication.getRequestToken();
                    customProgressOverlay.Visibility = Visibility.Visible;
                    customProgressOverlay.Show();
                }
            }
            else
            {
                NotificationTool.Show("Offline",
                                      "Sorry, the network is not available at the moment",
                                      new NotificationAction("Okay :(", () => { throw new Exception("ExitApp"); }));
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SongListPage songListPage = (e.Content as SongListPage);
            if (songListPage != null)
            {
                songListPage.searchResult = searchResult;
                songListPage.currentTag = CurrentItemCollections.Instance().currentMood;
            }
            base.OnNavigatedFrom(e);
        }

        #region LoadFinished

        private void loadFinishedWithArray(int METHOD, Tag[] tags)
        {
            customProgressOverlay.Hide();
            shellProgress.IsVisible = false;
            switch (METHOD)
            {
                case Constants.METHOD_SELECTED_TAGS:
                    {
                        if (tags != null)
                        {
                            CurrentItemCollections.Instance().selectedTags = new List<Tag>(tags);
                            selectedTagsList.ItemsSource = CurrentItemCollections.Instance().selectedTags;
                            customProgressOverlay.Hide();
                            uiBlocked = false;
                        }
                        else
                        {
                            oauthCommunication.getSelectedTags();
                        }
                    }
                    break;
                case Constants.METHOD_TOP_TAGS:
                    {
                        if (tags != null)
                        {
                            CurrentItemCollections.Instance().topTags = new List<Tag>(tags);
                            topTagsList.ItemsSource = CurrentItemCollections.Instance().topTags;
                            customProgressOverlay.Hide();
                            uiBlocked = false;
                        }
                        else
                        {
                            oauthCommunication.getTopTags();
                        }
                    }
                    break;
            }

        }

        private void loadFinished(int METHOD, Dictionary<string, string> returnedParams, JsonObject jsonObject)
        {

            shellProgress.IsVisible = false;
            switch (METHOD)
            {
                case Constants.METHOD_AUTHORIZATION:
                    {
                        parameters = returnedParams;
                        webBrowser1.LoadCompleted += webbrowser_LoginPageLoaded;
                        webBrowser1.IsScriptEnabled = true;
                        webBrowser1.Navigate(new Uri(parameters["URL"]));
                        break;
                    }
                case Constants.METHOD_ACCESS_TOKEN:
                    {
                        loadTags();
                        break;
                    }
                case Constants.METHOD_SEARCH:
                    {
                        customProgressOverlay.Hide();
                        searchResult = ((SearchResult)jsonObject);
                        if (searchResult.total > 0)
                        {
                            Uri songListUri = new Uri("/SongListPage.xaml", UriKind.Relative);
                            if (returnedParams.ContainsKey("VALUE"))
                            {
                                CurrentItemCollections.Instance().currentMood = new Tag { type = returnedParams["TYPE"], value = returnedParams["VALUE"] };
                            }
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
        #endregion

        #region OauthLogin Methods

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

        #endregion

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
                customProgressOverlay.Show();
                Deployment.Current.Dispatcher.BeginInvoke(() => oauthCommunication.searchSongs(Constants.TYPE_SITE, textToSearch));
            }
        }

        private void topTagSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            customProgressOverlay.Show();
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                Tag tag = ((Tag)listBox.SelectedItems[0]);
                if (tag != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => oauthCommunication.searchSongs(tag));
                    CurrentItemCollections.Instance().currentMood = tag;
                }
            }
        }

        private void selectedTagsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            customProgressOverlay.Show();
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                Tag tag = ((Tag)listBox.SelectedItems[0]);
                if (tag != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => oauthCommunication.searchSongs(tag));
                    CurrentItemCollections.Instance().currentMood = tag;
                }
            }

        }
        /* TODO: To be implemented
        private void favoritesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            customProgressOverlay.Show();
            var listBox = sender as ListBox;
            if (listBox != null)
            {
                Tag tag = ((Tag)listBox.SelectedItems[0]);
                if (tag != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() => oauthCommunication.searchSongs(tag));
                    CurrentItemCollections.Instance().currentMood = tag;
                }
            }
        }
        */
        protected override void OnBackKeyPress(CancelEventArgs e)
        {

            if (isSearchBarVisible && !uiBlocked)
            {
                toggleSearchBar();
                e.Cancel = true;
            }
            base.OnBackKeyPress(e);
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