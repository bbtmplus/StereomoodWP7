using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using BugSense;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;

namespace TuneYourMood
{
    public partial class App : Application
    {
        public const string ApiKeyValue = "Q2B2WXIGLCWYG7HS8CAH";

        public PhoneApplicationFrame RootFrame { get; private set; }

        public App()
        {
            UnhandledException += Application_UnhandledException;

            InitializeComponent();

            InitializePhoneApplication();

            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                          {
                                                              FlurryWP7SDK.Api.StartSession(ApiKeyValue);
                                                              BugSenseHandler.Instance.Init(Application.Current,
                                                                                            "7e54cf59");
                                                              if (ReviewBugger.IsTimeForReview())
                                                              {
                                                                  ReviewBugger.PromptUser();
                                                              }
                                                          });
        }

        public static bool IsTrial
        { get; private set; }

        private static readonly TimeSpan TrialPeriodLength = TimeSpan.FromDays(Constants.TRIAL_PERIOD);
        private const string FirstLauchDateKey = "FirstLaunchDate";

        private void DetermineIsTrail()
        {
#if TRIAL
            IsTrial = true;
#else
            var license = new Microsoft.Phone.Marketplace.LicenseInformation();
            IsTrial = license.IsTrial();
#endif
        }

        private void CheckTrialPeriodExpired()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                          {
                                                              IsolatedStorageSettings settings =
                                                                  IsolatedStorageSettings.ApplicationSettings;
                                                              DateTime firstLauchDate;
                                                              if (settings.TryGetValue<DateTime>(FirstLauchDateKey,
                                                                                                 out firstLauchDate))
                                                              {
                                                                  TimeSpan timeSinceFirstLauch =
                                                                      DateTime.UtcNow.Subtract(firstLauchDate);
                                                                  if (timeSinceFirstLauch > TrialPeriodLength)
                                                                  {
                                                                      this.RootFrame.Navigated +=
                                                                          new NavigatedEventHandler(RootFrame_Navigated);
                                                                  }
                                                              }
                                                              else
                                                              {
                                                                  // if a value cannot be found for the first launch date
                                                                  // save the current date and time 
                                                                  settings.Add(FirstLauchDateKey, DateTime.UtcNow);
                                                                  settings.Save();
                                                              }
                                                          });
        }

        void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            this.RootFrame.Navigated -= new NavigatedEventHandler(RootFrame_Navigated);

            Popup popup = new Popup();
            BuyNowControl content = new BuyNowControl(popup) { Width = Current.Host.Content.ActualWidth };
            popup.Child = content;
            // popup.VerticalOffset = 300;
            popup.IsOpen = true;
        }

        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            CurrentItemCollections.Instance().LoadApplicationState();
            DetermineIsTrail();

        }

        private void CheckTrialState()
        {
            // refresh the value of the IsTrial property 
            this.DetermineIsTrail();

            if (!IsTrial)
            {
                // do not execute further if app is full version
                return;
            }

            this.CheckTrialPeriodExpired();
        }

        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            CurrentItemCollections.Instance().LoadApplicationState();
            this.CheckTrialState();
        }

        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            CurrentItemCollections.Instance().SaveApplicationState();
        }

        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            CurrentItemCollections.Instance().SaveApplicationState();
        }

        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Инициализация приложения телефона

        private bool phoneApplicationInitialized = false;

        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            RootFrame = new RadPhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;
            phoneApplicationInitialized = true;
        }

        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}