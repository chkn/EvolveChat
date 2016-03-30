using System;
using System.Linq;

using UIKit;
using Foundation;

namespace EvolveChat {

    public partial class ConversationListViewController : ObserverTableViewController<Conversation> {

		IObservable<Conversation> dataSource = App.Backend.GetMyConversations ();

		public ConversationListViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.EstimatedRowHeight = 50;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			DisposeOnDisappear (dataSource.Subscribe (this));
		}

		protected override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath, Conversation data)
		{
			var cell = (ConversationCell)tableView.DequeueReusableCell (ConversationCell.Id, indexPath);
			cell.Conversation = data;
			return cell;
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);
			if (segue.Identifier == "Edit") {
				var dest = (ConversationViewController)segue.DestinationViewController;
				dest.PrepareToEdit (((ConversationCell)sender).Conversation);
			}
		}
    }
}