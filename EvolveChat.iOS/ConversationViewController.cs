using System;
using System.Linq;
using System.Collections.ObjectModel;

using System.Threading;

using System.Reactive;
using System.Reactive.Linq;

using UIKit;
using Foundation;

namespace EvolveChat {

	public partial class ConversationViewController : BoundTableViewController {

		static readonly NSString contactCellId = new NSString ("ContactCell");
		static readonly NSString contactHeaderId = new NSString ("ContactHeader");

		public Conversation Conversation {
			get { return conversation; }
			set {
				if (conversation != value) {
					conversation = value;
					if (titleSubscription != null) {
						titleSubscription.Dispose ();
						titleSubscription = null;
					}
					if (conversation != null) {
						Binding = conversation.Messages;
						// If someone joins the conversation, we'll update the title..
						titleSubscription = conversation.ObservePropertyValue (c => c.Participants)
							.ObserveOn (SynchronizationContext.Current)
							.Subscribe (participants =>
								Title = string.Join (", ", participants.Where (p => p != App.Backend.Me))
							);
					} else {
						Binding = null;
					}
				}
			}
		}
		Conversation conversation;
		IDisposable titleSubscription;

		public bool IsSelectingContacts => (Conversation == null);

		public ConversationViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.RegisterNibForHeaderFooterViewReuse (UINib.FromName ("ContactSearchBar", null), contactHeaderId);
			TableView.RowHeight = UITableView.AutomaticDimension;
			TableView.EstimatedRowHeight = 44;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			TableView.AllowsSelection = IsSelectingContacts;
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			Conversation = null;
		}

		public override nfloat GetHeightForHeader (UITableView tableView, nint section)
		{
			return IsSelectingContacts? 40 : 0;
		}

		public override UIView GetViewForHeader (UITableView tableView, nint section)
		{
			ContactSearchBar header = null;
			if (IsSelectingContacts) {
				header = (ContactSearchBar)tableView.DequeueReusableHeaderFooterView (contactHeaderId);
				Binding = App.Backend.FindContacts (header.TextField.ObserveText ());
				header.TextField.BecomeFirstResponder ();
			}
			return header;
		}

		protected override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath, object data)
		{
			var msg = data as Message;
			if (msg != null) {
				var cellId = msg.Sender == App.Backend.Me ? MessageCell.OutgoingId : MessageCell.IncomingId;
				var cell = (MessageCell)tableView.DequeueReusableCell (cellId, indexPath);
				cell.Message = msg;
				return cell;
			}

			var contact = data as Contact;
			if (contact != null) {
				var cell = tableView.DequeueReusableCell (contactCellId, indexPath);
				cell.TextLabel.Text = contact.FirstName + " " + contact.LastName;
				return cell;
			}
			throw new NotImplementedException (data.GetType ().FullName);
		}

		public override async void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// Resign first responder on the header so it can go away
			View.EndEditing (true);

			// Get the contact that was selected
			var contact = (Contact)GetData (indexPath);

			// Show a loading view controller and load the conversation
			var vc = Storyboard.InstantiateViewController ("LoadingViewController");
			PresentViewController (vc, false, null);

			// Create or resume the converstion
			Conversation = await App.Backend.StartConversation (contact);

			// Dismiss the loading view controller
			DismissViewController (false, null);
		}
	}
}