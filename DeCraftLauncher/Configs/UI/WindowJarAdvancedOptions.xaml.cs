﻿using DeCraftLauncher.Configs;
using DeCraftLauncher.Utils;
using SourceChord.FluentWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DeCraftLauncher.Configs.UI
{
    /// <summary>
    /// Logika interakcji dla klasy JarAdvancedOptions.xaml
    /// </summary>
    public partial class WindowJarAdvancedOptions : AcrylicWindow
    {
        JarConfig targetConfig;

        public WindowJarAdvancedOptions(JarConfig target)
        {
            this.targetConfig = target;
            InitializeComponent();
            label_title.Content = $"Advanced options: {target.jarFileName}";
            LoadConfig();
            Util.UpdateAcrylicWindowBackground(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SaveConfig();
        }

        public void LoadConfig()
        {
            tbox_sessionid.Text = targetConfig.sessionID;
            tbox_gameargs.Text = targetConfig.gameArgs;
            checkbox_cwdisdotminecraft.IsChecked = targetConfig.cwdIsDotMinecraft;
            checkbox_emulatehttp.IsChecked = targetConfig.appletEmulateHTTP;
            tbox_appletdocumenturl.Text = targetConfig.documentBaseUrl;
            checkbox_redirecttolocalskins.IsChecked = targetConfig.appletRedirectSkins;
            tbox_skinredirectpath.Text = targetConfig.appletSkinRedirectPath;
        }

        public void SaveConfig()
        {
            targetConfig.sessionID = tbox_sessionid.Text;
            targetConfig.gameArgs = tbox_gameargs.Text;
            targetConfig.cwdIsDotMinecraft = checkbox_cwdisdotminecraft.IsChecked == true;
            targetConfig.appletEmulateHTTP = checkbox_emulatehttp.IsChecked == true;
            targetConfig.documentBaseUrl = tbox_appletdocumenturl.Text;
            targetConfig.appletRedirectSkins = checkbox_redirecttolocalskins.IsChecked == true;
            targetConfig.appletSkinRedirectPath = tbox_skinredirectpath.Text;
            targetConfig.SaveToXML(MainWindow.configDir + "/" + targetConfig.jarFileName + ".xml");
        }
    }
}