﻿#pragma checksum "..\..\SubWindow1.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "13004E290760F0E61585D4824D3861FBA1EB93723DE0D34AE498B7CEB0653930"
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using voqui3;


namespace voqui3 {
    
    
    /// <summary>
    /// SubWindow1
    /// </summary>
    public partial class SubWindow1 : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\SubWindow1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TbLKey;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\SubWindow1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TbNendo;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\SubWindow1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TbKekka;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\SubWindow1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TbVersion;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\SubWindow1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CBoxNew;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\SubWindow1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonSet;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\SubWindow1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ButtonBack;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\SubWindow1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbHyou1;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\SubWindow1.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbHyou2;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/VOQ3AA;component/subwindow1.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\SubWindow1.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\SubWindow1.xaml"
            ((voqui3.SubWindow1)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            
            #line 8 "..\..\SubWindow1.xaml"
            ((voqui3.SubWindow1)(target)).Closed += new System.EventHandler(this.Window_Closed);
            
            #line default
            #line hidden
            return;
            case 2:
            this.TbLKey = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.TbNendo = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.TbKekka = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.TbVersion = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.CBoxNew = ((System.Windows.Controls.CheckBox)(target));
            
            #line 21 "..\..\SubWindow1.xaml"
            this.CBoxNew.Checked += new System.Windows.RoutedEventHandler(this.CBoxNew_Checked);
            
            #line default
            #line hidden
            
            #line 21 "..\..\SubWindow1.xaml"
            this.CBoxNew.Unchecked += new System.Windows.RoutedEventHandler(this.CBoxNew_Unchecked);
            
            #line default
            #line hidden
            return;
            case 7:
            this.ButtonSet = ((System.Windows.Controls.Button)(target));
            
            #line 23 "..\..\SubWindow1.xaml"
            this.ButtonSet.Click += new System.Windows.RoutedEventHandler(this.ButtonSet_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.ButtonBack = ((System.Windows.Controls.Button)(target));
            
            #line 24 "..\..\SubWindow1.xaml"
            this.ButtonBack.Click += new System.Windows.RoutedEventHandler(this.ButtonBack_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.tbHyou1 = ((System.Windows.Controls.TextBox)(target));
            return;
            case 10:
            this.tbHyou2 = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

