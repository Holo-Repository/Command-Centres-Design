﻿#pragma checksum "C:\code\Command-Centres-Design\WindowManager\WindowManager\UserControls\MenuBar.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "060087B73D31320BCA7E3126A9E38FBE7FB19C2A7C2955ACB51072EF39906ADF"
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
    partial class MenuBar : global::Microsoft.UI.Xaml.Controls.UserControl
    {


#pragma warning disable 0169    //  Proactively suppress unused/uninitialized field warning in case they aren't used, for things like x:Name
#pragma warning disable 0649
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2306")]
        private global::Microsoft.UI.Xaml.Controls.TextBlock NumWindowsTextBlock; 
#pragma warning restore 0649
#pragma warning restore 0169
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2306")]
        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2306")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;

            global::System.Uri resourceLocator = new global::System.Uri("ms-appx:///UserControls/MenuBar.xaml");
            global::Microsoft.UI.Xaml.Application.LoadComponent(this, resourceLocator, global::Microsoft.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
        }

        partial void UnloadObject(global::Microsoft.UI.Xaml.DependencyObject unloadableObject);

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2306")]
        private interface IMenuBar_Bindings
        {
            void Initialize();
            void Update();
            void StopTracking();
            void DisconnectUnloadedObject(int connectionId);
        }

        private interface IMenuBar_BindingsScopeConnector
        {
            global::System.WeakReference Parent { get; set; }
            bool ContainsElement(int connectionId);
            void RegisterForElementConnection(int connectionId, global::Microsoft.UI.Xaml.Markup.IComponentConnector connector);
        }
#pragma warning disable 0169    //  Proactively suppress unused field warning in case Bindings is not used.
#pragma warning disable 0649
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler"," 3.0.0.2306")]
        private IMenuBar_Bindings Bindings;
#pragma warning restore 0649
#pragma warning restore 0169
    }
}


