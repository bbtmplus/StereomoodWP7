using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using DeepForest.Phone.Assets.Controls;
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

        private bool isUserLoggedIn;

        private readonly Searchbar searchbar = new Searchbar();
        private bool uiBlocked;

        public MainPage()
        {
            InitializeComponent();
            InitializeElements();

            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        private void InitializeElements()
        {
            searchbar.searchPressed += searchbar_searchPressed;
            searchbar.Visibility = Visibility.Collapsed;
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
            }else
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

            background.Source = bitmapImage;
            panorama.Background = new ImageBrush() { ImageSource = background.Source };
            background.Visibility = System.Windows.Visibility.Collapsed;
            background.Stretch = Stretch.Fill;
            background.Width = panorama.Width;
            background.Height = panorama.Height;
        }

        void sb_Completed(object sender, EventArgs e)
        {
            BitmapImage bitmapImage = new BitmapImage(new Uri(Constants.BACKGROUND_URL));
            ImageBrush imageBrush = new ImageBrush { ImageSource = bitmapImage };

            panorama.Background = imageBrush;
            Storyboard sbFadeOut = new Storyboard();

            FadeInOut(panorama.Background, sbFadeOut, false);
        }

        void webBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            oauthCommunication = OauthCommunication.getInstance();
        }



        private void loadFinished(int METHOD, Dictionary<string, string> returnedParams)
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
                        oauthCommunication.getTopTags();
                        oauthCommunication.getSelectedTags();
                        break;
                    }
                case Constants.METHOD_SEARCH:
                    {

                        break;
                    }
            }
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

        private string ExtractNumbers(string expr)
        {
            return string.Join(null, Regex.Split(expr, "[^\\d]"));
        }



        #region ACTIONS

        private void barItemSearchClicked(object sender, EventArgs e)
        {
            searchbar.Visibility = searchbar.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void searchbar_searchPressed(string textToSearch)
        {
            oauthCommunication.searchSong(textToSearch);
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

        #endregion
    }
}