﻿#pragma checksum "C:\code\Command-Centres-Design\WindowManager\WindowManager\UserControls\MenuBar.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "2F10C27C42767C4F007448EC8E10D48968CEBFECFEE6A80DEC45EF9094757DBB"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WindowManager.UserControls
{
    partial class MenuBar : 
        global::Microsoft.UI.Xaml.Controls.UserControl, 
        global::Microsoft.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2306")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private static class XamlBindingSetters
        {
            public static void Set_Microsoft_UI_Xaml_Controls_TextBlock_Text(global::Microsoft.UI.Xaml.Controls.TextBlock obj, global::System.String value, string targetNullValue)
            {
                if (value == null && targetNullValue != null)
                {
                    value = targetNullValue;
                }
                obj.Text = value ?? global::System.String.Empty;
            }
        };

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2306")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        private class MenuBar_obj1_Bindings :
            global::Microsoft.UI.Xaml.Markup.IDataTemplateComponent,
            global::Microsoft.UI.Xaml.Markup.IXamlBindScopeDiagnostics,
            global::Microsoft.UI.Xaml.Markup.IComponentConnector,
            IMenuBar_Bindings
        {
            private global::WindowManager.UserControls.MenuBar dataRoot;
            private bool initialized = false;
            private const int NOT_PHASED = (1 << 31);
            private const int DATA_CHANGED = (1 << 30);

            // Fields for each control that has bindings.
            private global::Microsoft.UI.Xaml.Controls.TextBlock obj3;

            // Static fields for each binding's enabled/disabled state
            private static bool isobj3TextDisabled = false;

            public MenuBar_obj1_Bindings()
            {
            }

            public void Disable(int lineNumber, int columnNumber)
            {
                if (lineNumber == 87 && columnNumber == 20)
                {
                    isobj3TextDisabled = true;
                }
            }

            // IComponentConnector

            public void Connect(int connectionId, global::System.Object target)
            {
                switch(connectionId)
                {
                    case 3: // UserControls\MenuBar.xaml line 87
                        this.obj3 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.TextBlock>(target);
                        break;
                    default:
                        break;
                }
            }
                        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2306")]
                        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
                        public global::Microsoft.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target) 
                        {
                            return null;
                        }

            // IDataTemplateComponent

            public void ProcessBindings(global::System.Object item, int itemIndex, int phase, out int nextPhase)
            {
                nextPhase = -1;
            }

            public void Recycle()
            {
                return;
            }

            // IMenuBar_Bindings

            public void Initialize()
            {
                if (!this.initialized)
                {
                    this.Update();
                }
            }
            
            public void Update()
            {
                this.Update_(this.dataRoot, NOT_PHASED);
                this.initialized = true;
            }

            public void StopTracking()
            {
            }

            public void DisconnectUnloadedObject(int connectionId)
            {
                throw new global::System.ArgumentException("No unloadable elements to disconnect.");
            }

            public bool SetDataRoot(global::System.Object newDataRoot)
            {
                if (newDataRoot != null)
                {
                    this.dataRoot = global::WinRT.CastExtensions.As<global::WindowManager.UserControls.MenuBar>(newDataRoot);
                    return true;
                }
                return false;
            }

            public void Activated(object obj, global::Microsoft.UI.Xaml.WindowActivatedEventArgs data)
            {
                this.Initialize();
            }

            public void Loading(global::Microsoft.UI.Xaml.FrameworkElement src, object data)
            {
                this.Initialize();
            }

            // Update methods for each path node used in binding steps.
            private void Update_(global::WindowManager.UserControls.MenuBar obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_ViewModel(obj.ViewModel, phase);
                    }
                }
            }
            private void Update_ViewModel(global::WindowManager.NumWindowsViewModel obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_ViewModel_DefaultNumWindows(obj.DefaultNumWindows, phase);
                    }
                }
            }
            private void Update_ViewModel_DefaultNumWindows(global::WindowManager.NumWindows obj, int phase)
            {
                if (obj != null)
                {
                    if ((phase & (NOT_PHASED | (1 << 0))) != 0)
                    {
                        this.Update_ViewModel_DefaultNumWindows_NumWindowsString(obj.NumWindowsString, phase);
                    }
                }
            }
            private void Update_ViewModel_DefaultNumWindows_NumWindowsString(global::System.String obj, int phase)
            {
                if ((phase & ((1 << 0) | NOT_PHASED )) != 0)
                {
                    // UserControls\MenuBar.xaml line 87
                    if (!isobj3TextDisabled)
                    {
                        XamlBindingSetters.Set_Microsoft_UI_Xaml_Controls_TextBlock_Text(this.obj3, obj, null);
                    }
                }
            }
        }

        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2306")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // UserControls\MenuBar.xaml line 84
                {
                    global::Microsoft.UI.Xaml.Controls.ToggleSwitch element2 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.ToggleSwitch>(target);
                    ((global::Microsoft.UI.Xaml.Controls.ToggleSwitch)element2).Toggled += this.ToggleSwitch_Toggled;
                }
                break;
            case 4: // UserControls\MenuBar.xaml line 64
                {
                    global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem element4 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem>(target);
                    ((global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem)element4).Click += this.ChangeURLItem_Click;
                }
                break;
            case 5: // UserControls\MenuBar.xaml line 72
                {
                    global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem element5 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem>(target);
                    ((global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem)element5).Click += this.ChangeURLItem_Click;
                }
                break;
            case 6: // UserControls\MenuBar.xaml line 45
                {
                    global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem element6 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem>(target);
                    ((global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem)element6).Click += this.MenuFlyoutItem_Click;
                }
                break;
            case 7: // UserControls\MenuBar.xaml line 53
                {
                    global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem element7 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem>(target);
                    ((global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem)element7).Click += this.MenuFlyoutItem_Click;
                }
                break;
            case 8: // UserControls\MenuBar.xaml line 36
                {
                    global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem element8 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem>(target);
                    ((global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem)element8).Click += this.MenuFlyoutItem_Click_1;
                }
                break;
            case 9: // UserControls\MenuBar.xaml line 17
                {
                    global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem element9 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem>(target);
                    ((global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem)element9).Click += this.MenuFlyoutItem_Click;
                }
                break;
            case 10: // UserControls\MenuBar.xaml line 23
                {
                    global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem element10 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem>(target);
                    ((global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem)element10).Click += this.MenuFlyoutItem_Click;
                }
                break;
            case 11: // UserControls\MenuBar.xaml line 29
                {
                    global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem element11 = global::WinRT.CastExtensions.As<global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem>(target);
                    ((global::Microsoft.UI.Xaml.Controls.MenuFlyoutItem)element11).Click += this.MenuFlyoutItem_Click;
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
            switch(connectionId)
            {
            case 1: // UserControls\MenuBar.xaml line 2
                {                    
                    global::Microsoft.UI.Xaml.Controls.UserControl element1 = (global::Microsoft.UI.Xaml.Controls.UserControl)target;
                    MenuBar_obj1_Bindings bindings = new MenuBar_obj1_Bindings();
                    returnValue = bindings;
                    bindings.SetDataRoot(this);
                    this.Bindings = bindings;
                    element1.Loading += bindings.Loading;
                    global::Microsoft.UI.Xaml.Markup.XamlBindingHelper.SetDataTemplateComponent(element1, bindings);
                }
                break;
            }
            return returnValue;
        }
    }
}

