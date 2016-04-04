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
    [Register ("ConversationViewController")]
    partial class ConversationViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView messageEntry { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint messageEntryBottom { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField messageText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton sendButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView TableView { get; set; }

        [Action ("UIButton809_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnSendMessage (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (messageEntry != null) {
                messageEntry.Dispose ();
                messageEntry = null;
            }

            if (messageEntryBottom != null) {
                messageEntryBottom.Dispose ();
                messageEntryBottom = null;
            }

            if (messageText != null) {
                messageText.Dispose ();
                messageText = null;
            }

            if (sendButton != null) {
                sendButton.Dispose ();
                sendButton = null;
            }

            if (TableView != null) {
                TableView.Dispose ();
                TableView = null;
            }
        }
    }
}