﻿#pragma checksum "C:\Users\Lonkly\documents\visual studio 2010\Projects\Stereomood\Stereomood\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "396894B05ABF5472DB459BB7D4B5F3EA"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.17379
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Phone7.Fx.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;
using Telerik.Windows.Controls;


namespace TuneYourMood {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Style customStyle;
        
        internal Microsoft.Phone.Controls.Panorama panorama;
        
        internal System.Windows.Controls.ListBox topTagsList;
        
        internal System.Windows.Controls.ListBox selectedTagsList;
        
        internal System.Windows.Controls.ListBox favoritesList;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.HyperlinkButton textBlock1;
        
        internal System.Windows.Controls.TextBlock textBlock3;
        
        internal System.Windows.Controls.TextBlock textBlock4;
        
        internal System.Windows.Controls.TextBlock emailUrlButton;
        
        internal System.Windows.Controls.TextBlock textBlock5;
        
        internal System.Windows.Controls.Button button1;
        
        internal System.Windows.Controls.TextBlock textBlock6;
        
        internal System.Windows.Controls.Button button2;
        
        internal Microsoft.Phone.Controls.WebBrowser webBrowser1;
        
        internal Telerik.Windows.Controls.RadBusyIndicator customProgressOverlay;
        
        internal Phone7.Fx.Controls.BindableApplicationBarIconButton goToPlayerButton;
        
        internal Phone7.Fx.Controls.BindableApplicationBarIconButton syncButton;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/TuneYourMood;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.customStyle = ((System.Windows.Style)(this.FindName("customStyle")));
            this.panorama = ((Microsoft.Phone.Controls.Panorama)(this.FindName("panorama")));
            this.topTagsList = ((System.Windows.Controls.ListBox)(this.FindName("topTagsList")));
            this.selectedTagsList = ((System.Windows.Controls.ListBox)(this.FindName("selectedTagsList")));
            this.favoritesList = ((System.Windows.Controls.ListBox)(this.FindName("favoritesList")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.textBlock1 = ((System.Windows.Controls.HyperlinkButton)(this.FindName("textBlock1")));
            this.textBlock3 = ((System.Windows.Controls.TextBlock)(this.FindName("textBlock3")));
            this.textBlock4 = ((System.Windows.Controls.TextBlock)(this.FindName("textBlock4")));
            this.emailUrlButton = ((System.Windows.Controls.TextBlock)(this.FindName("emailUrlButton")));
            this.textBlock5 = ((System.Windows.Controls.TextBlock)(this.FindName("textBlock5")));
            this.button1 = ((System.Windows.Controls.Button)(this.FindName("button1")));
            this.textBlock6 = ((System.Windows.Controls.TextBlock)(this.FindName("textBlock6")));
            this.button2 = ((System.Windows.Controls.Button)(this.FindName("button2")));
            this.webBrowser1 = ((Microsoft.Phone.Controls.WebBrowser)(this.FindName("webBrowser1")));
            this.customProgressOverlay = ((Telerik.Windows.Controls.RadBusyIndicator)(this.FindName("customProgressOverlay")));
            this.goToPlayerButton = ((Phone7.Fx.Controls.BindableApplicationBarIconButton)(this.FindName("goToPlayerButton")));
            this.syncButton = ((Phone7.Fx.Controls.BindableApplicationBarIconButton)(this.FindName("syncButton")));
        }
    }
}

