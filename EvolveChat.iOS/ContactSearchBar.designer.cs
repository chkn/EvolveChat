// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace EvolveChat
{
    [Register ("ContactSearchBar")]
    partial class ContactSearchBar
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField textField { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (textField != null) {
                textField.Dispose ();
                textField = null;
            }
        }
    }
}