﻿#pragma checksum "C:\Users\Lonkly\documents\visual studio 2010\Projects\Stereomood\Stereomood\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B6CA59999B8B332F7455488367DDC374"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.17379
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
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


namespace TuneYourMood {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Microsoft.Phone.Shell.ProgressIndicator shellProgress;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Style customStyle;
        
        internal System.Windows.Controls.Image background;
        
        internal Microsoft.Phone.Controls.Panorama panorama;
        
        internal System.Windows.Media.ImageBrush panoramaBG;
        
        internal System.Windows.Controls.ListBox topTagsList;
        
        internal System.Windows.Controls.ListBox selectedTagsList;
        
        internal Microsoft.Phone.Controls.WebBrowser webBrowser1;
        
        internal Coding4Fun.Phone.Controls.ProgressOverlay customProgressOverlay;
        
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
            this.shellProgress = ((Microsoft.Phone.Shell.ProgressIndicator)(this.FindName("shellProgress")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.customStyle = ((System.Windows.Style)(this.FindName("customStyle")));
            this.background = ((System.Windows.Controls.Image)(this.FindName("background")));
            this.panorama = ((Microsoft.Phone.Controls.Panorama)(this.FindName("panorama")));
            this.panoramaBG = ((System.Windows.Media.ImageBrush)(this.FindName("panoramaBG")));
            this.topTagsList = ((System.Windows.Controls.ListBox)(this.FindName("topTagsList")));
            this.selectedTagsList = ((System.Windows.Controls.ListBox)(this.FindName("selectedTagsList")));
            this.webBrowser1 = ((Microsoft.Phone.Controls.WebBrowser)(this.FindName("webBrowser1")));
            this.customProgressOverlay = ((Coding4Fun.Phone.Controls.ProgressOverlay)(this.FindName("customProgressOverlay")));
        }
    }
}

