﻿#pragma checksum "C:\Users\Lonkly\documents\visual studio 2010\Projects\Stereomood\Stereomood\SongDetailsPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D2423FDB4BE3FB8E1293700225C13D2C"
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


namespace TuneYourMood {
    
    
    public partial class SongDetailsPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock ApplicationTitle;
        
        internal System.Windows.Controls.Grid songPanel;
        
        internal System.Windows.Controls.MediaElement mediaPlayer;
        
        internal System.Windows.Controls.TextBlock songTextBox;
        
        internal System.Windows.Controls.Image artImage;
        
        internal System.Windows.Controls.TextBlock authorTextBox;
        
        internal Phone7.Fx.Controls.BindableApplicationBarIconButton prevButton;
        
        internal Phone7.Fx.Controls.BindableApplicationBarIconButton playButton;
        
        internal Phone7.Fx.Controls.BindableApplicationBarIconButton nextButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/TuneYourMood;component/SongDetailsPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ApplicationTitle = ((System.Windows.Controls.TextBlock)(this.FindName("ApplicationTitle")));
            this.songPanel = ((System.Windows.Controls.Grid)(this.FindName("songPanel")));
            this.mediaPlayer = ((System.Windows.Controls.MediaElement)(this.FindName("mediaPlayer")));
            this.songTextBox = ((System.Windows.Controls.TextBlock)(this.FindName("songTextBox")));
            this.artImage = ((System.Windows.Controls.Image)(this.FindName("artImage")));
            this.authorTextBox = ((System.Windows.Controls.TextBlock)(this.FindName("authorTextBox")));
            this.prevButton = ((Phone7.Fx.Controls.BindableApplicationBarIconButton)(this.FindName("prevButton")));
            this.playButton = ((Phone7.Fx.Controls.BindableApplicationBarIconButton)(this.FindName("playButton")));
            this.nextButton = ((Phone7.Fx.Controls.BindableApplicationBarIconButton)(this.FindName("nextButton")));
        }
    }
}
