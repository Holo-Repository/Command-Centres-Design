﻿#pragma checksum "C:\code\Command-Centres-Design\WindowManager\WindowManager\DownloadConfigPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "11022E1768816AD9BFAA06565988FDB0B21500A55ED3D30DD3F166AB1173D200"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WindowManager
{
    partial class DownloadConfigPage : 
        global::Microsoft.UI.Xaml.Controls.Page, 
        global::Microsoft.UI.Xaml.Markup.IComponentConnector
    {

        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2306")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // DownloadConfigPage.xaml line 12
                {
                    this.stackpanel = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.StackPanel>(target);
                }
                break;
            case 3: // DownloadConfigPage.xaml line 15
                {
                    this.HelpTitle = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 4: // DownloadConfigPage.xaml line 49
                {
                    this.PickAFileButton = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.PickAFileButton).Click += this.PickAFileButton_Click;
                }
                break;
            case 5: // DownloadConfigPage.xaml line 62
                {
                    this.PickAFileOutputTextBlock = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            case 6: // DownloadConfigPage.xaml line 93
                {
                    this.ShareAFileButton = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.Button>(target);
                    ((global::Microsoft.UI.Xaml.Controls.Button)this.ShareAFileButton).Click += this.ShareAFileButton_Click;
                }
                break;
            case 7: // DownloadConfigPage.xaml line 101
                {
                    this.ShareAFileOutputTextBlock = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2306")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Microsoft.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

