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
    [Register ("ConversationCell")]
    partial class ConversationCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel badge { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel date { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel name { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel preview { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (badge != null) {
                badge.Dispose ();
                badge = null;
            }

            if (date != null) {
                date.Dispose ();
                date = null;
            }

            if (name != null) {
                name.Dispose ();
                name = null;
            }

            if (preview != null) {
                preview.Dispose ();
                preview = null;
            }
        }
    }
}