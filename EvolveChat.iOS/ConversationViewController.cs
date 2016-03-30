using System;
using System.Linq;

using System.Threading;

using System.Reactive;
using System.Reactive.Linq;

using UIKit;
using Foundation;

namespace EvolveChat {

    public partial class ConversationViewController : ObserverTableViewController<Message> {

		IObservable<Message> dataSource;

		public ConversationViewController (IntPtr handle) : base (handle)
		{
		}

		// Only to be called right before the view controller is shown
		public void PrepareToEdit (Conversation convo)
		{
			dataSource = convo.Messages.ObserveAllItems ();

			// If someone joins the conversation, we'll update the title..
			DisposeOnDisappear (
				convo.ObservePropertyValue (c => c.Participants)
					.ObserveOn (SynchronizationContext.Current)
					.Subscribe (participants =>
						Title = string.Join (", ", participants.Where (p => p != App.Backend.Me))
					)
			);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.RowHeight = UITableView.AutomaticDimension;
			TableView.EstimatedRowHeight = 44;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (dataSource != null)
				DisposeOnDisappear (dataSource.Subscribe (this));
		}

		protected override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath, Message data)
		{
			var cellId = data.Sender == App.Backend.Me ? MessageCell.OutgoingId : MessageCell.IncomingId;
			var cell = (MessageCell)tableView.DequeueReusableCell (cellId, indexPath);
			cell.Message = data;
			return cell;
		}
    }
}