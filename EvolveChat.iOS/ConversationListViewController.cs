using System;
using System.Linq;
using System.Collections.ObjectModel;

using UIKit;
using Foundation;

namespace EvolveChat {

    public partial class ConversationListViewController : BoundTableViewController {

		public ConversationListViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.EstimatedRowHeight = 50;
			Binding = App.Backend.GetMyConversations ();
		}

		protected override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath, object data)
		{
			var cell = (ConversationCell)tableView.DequeueReusableCell (ConversationCell.Id, indexPath);
			cell.Conversation = (Conversation)data;
			return cell;
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier == "Edit") {
				var dest = (ConversationViewController)segue.DestinationViewController;
				dest.Conversation = ((ConversationCell)sender).Conversation;
			}
		}
    }
}