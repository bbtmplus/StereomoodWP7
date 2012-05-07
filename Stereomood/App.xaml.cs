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

            BugSenseHandler.Instance.Init(this, "7d1e25c4");
            BugSenseHandler.Instance.UnhandledException += OnUnhandledException;
        }

        private void OnUnhandledException(object sender, BugSenseUnhandledExceptionEventArgs e)
        {
            try
            {
                //Some code to execute
            }
            catch (Exception ex)
            {
                BugSenseHandler.HandleError(ex);
            }
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
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            DateTime firstLauchDate;
            if (settings.TryGetValue<DateTime>(FirstLauchDateKey, out firstLauchDate))
            {
                TimeSpan timeSinceFirstLauch = DateTime.UtcNow.Subtract(firstLauchDate);
                if (timeSinceFirstLauch > TrialPeriodLength)
                {
                    this.RootFrame.Navigated += new NavigatedEventHandler(RootFrame_Navigated);
                }
            }
            else
            {
                // if a value cannot be found for the first launch date
                // save the current date and time 
                settings.Add(FirstLauchDateKey, DateTime.UtcNow);
                settings.Save();
            }
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
            ReviewBugger.CheckNumOfRuns();
            FlurryWP7SDK.Api.StartSession(ApiKeyValue);
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

        // Код для выполнения при активации приложения (переводится в основной режим)
        // Этот код не будет выполняться при первом запуске приложения
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            CurrentItemCollections.Instance().LoadApplicationState();
            this.CheckTrialState();
        }

        // Код для выполнения при деактивации приложения (отправляется в фоновый режим)
        // Этот код не будет выполняться при закрытии приложения
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            CurrentItemCollections.Instance().SaveApplicationState();
        }

        // Код для выполнения при закрытии приложения (например, при нажатии пользователем кнопки "Назад")
        // Этот код не будет выполняться при деактивации приложения
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            CurrentItemCollections.Instance().SaveApplicationState();
        }

        // Код для выполнения в случае ошибки навигации
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Ошибка навигации; перейти в отладчик
                System.Diagnostics.Debugger.Break();
            }
        }

        // Код для выполнения на необработанных исключениях
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Произошло необработанное исключение; перейти в отладчик
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Инициализация приложения телефона

        // Избегайте двойной инициализации
        private bool phoneApplicationInitialized = false;

        // Не добавляйте в этот метод дополнительный код
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new RadPhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Не добавляйте в этот метод дополнительный код
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Задайте корневой визуальный элемент для визуализации приложения
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Удалите этот обработчик, т.к. он больше не нужен
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}