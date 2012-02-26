using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace Stereomood
{
    public partial class MainPage : PhoneApplicationPage
    {
        private OauthCommunication oauthCommunication;
        private Dictionary<string, string> parameters;

        private bool isUserLoggedIn;

        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            oauthCommunication = OauthCommunication.getInstance();
            oauthCommunication.loadFinished += loadFinished;
            if (!Settings.isUserLoggedIn)
            {
                oauthCommunication.getRequestToken();
            }
        }

        private static void loadAppBackground()
        {

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
                        this.parameters = returnedParams;
                        webBrowser1.LoadCompleted += webbrowser_LoginPageLoaded;
                        webBrowser1.Navigating += webBrowser_Navigating;
                        webBrowser1.IsScriptEnabled = true;
                        webBrowser1.Navigate(new Uri(parameters["URL"]));
                        break;
                    }
                case Constants.METHOD_ACCESS_TOKEN:
                    {
                        oauthCommunication.searchSong("sting");
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
            } catch( SystemException ex)
            {
                webbrowser_FindPincode(null, null);
            }

        }

        private void webbrowser_FindPincode(object sender, NavigationEventArgs e)
        {
            string content = webBrowser1.SaveToString();
            int indexOf = content.IndexOf("PIN:", System.StringComparison.OrdinalIgnoreCase);

            string pinSubstring = content.Substring(indexOf);
            int endIndex = pinSubstring.IndexOf("</h1>", System.StringComparison.OrdinalIgnoreCase);
            string pinString = pinSubstring.Remove(endIndex);
            string pin = ExtractNumbers(pinString);

            OauthCommunication.getInstance().getAccessToken(pin);
        }

        private string ExtractNumbers(string expr)
        {
            return string.Join(null, Regex.Split(expr, "[^\\d]"));
        }
    }
}