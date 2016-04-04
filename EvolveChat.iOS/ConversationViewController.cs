using System;
using System.Linq;
using System.Threading;
using System.Reactive.Linq;

using UIKit;
using Foundation;
using CoreGraphics;

namespace EvolveChat {

	public partial class ConversationViewController : UIViewController, IUITableViewDelegate {

		static readonly NSString contactCellId = new NSString ("ContactCell");
		static readonly NSString contactHeaderId = new NSString ("ContactHeader");

		public Conversation Conversation {
			get { return conversation; }
			set {
				if (conversation != value) {
					conversation = value;
					SetTitleBinding ();
					SetDataBinding ();
				}
			}
		}

		public bool IsSelectingContacts => (Conversation == null);
		public BindingTableDataSource DataSource => TableView?.DataSource as BindingTableDataSource;

		IDisposable titleBinding;
		Conversation conversation;
		NSObject keyboardObserver;

		public ConversationViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.Delegate = this;
			TableView.DataSource = new BindingTableDataSource (TableView, GetCell);
			SetDataBinding ();

			TableView.RegisterNibForHeaderFooterViewReuse (UINib.FromName ("ContactSearchBar", null), contactHeaderId);
			TableView.RowHeight = UITableView.AutomaticDimension;
			TableView.EstimatedRowHeight = 44;

			var ci = TableView.ContentInset;
			ci.Bottom += messageEntry.Frame.Height;
			TableView.ContentInset = ci;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			TableView.AllowsSelection = IsSelectingContacts;
			sendButton.Enabled = !IsSelectingContacts;
			keyboardObserver = NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillChangeFrameNotification, OnKeyboardChangeFrame);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			NSNotificationCenter.DefaultCenter.RemoveObserver (keyboardObserver, UIKeyboard.WillChangeFrameNotification, null);
		}

		void OnKeyboardChangeFrame (NSNotification obj)
		{
			var frame = View.ConvertRectFromView (UIKeyboard.FrameEndFromNotification (obj), null);
			var offset = View.Bounds.Height - frame.Y;
			messageEntryBottom.Constant = offset - BottomLayoutGuide.Length;

			var curve = UIKeyboard.AnimationCurveFromNotification (obj);
			UIView.Animate (
				duration: UIKeyboard.AnimationDurationFromNotification (obj),
				delay: 0,
				options: (UIViewAnimationOptions)(curve << 16),
				animation: () => {
					View.LayoutIfNeeded ();
					var ci = TableView.ContentInset;
					ci.Bottom = offset + messageEntry.Frame.Height;
					TableView.ContentInset = ci;
				},
				completion: null
			);
		}

		partial void OnSendMessage (UIButton sender)
		{
			Conversation.Messages.Add (new Message (DateTime.UtcNow, App.Backend.Me, messageText.Text));
			messageText.Text = "";
		}

		void SetDataBinding ()
		{
			var ds = DataSource;
			if (ds != null)
				ds.Binding = Conversation?.Messages;
		}

		void SetTitleBinding ()
		{
			// If someone joins the conversation, we'll update the title..
			titleBinding?.Dispose ();
			titleBinding = Conversation?.ObservePropertyValue (c => c.Participants)
				.ObserveOn (SynchronizationContext.Current)
				.Subscribe (participants =>
					Title = string.Join (", ", participants.Where (p => p != App.Backend.Me))
				);
		}

		[Export ("tableView:heightForHeaderInSection:")]
		public nfloat GetHeightForHeader (UITableView tableView, nint section)
		{
			return IsSelectingContacts? 40 : 0;
		}

		[Export ("tableView:viewForHeaderInSection:")]
		public UIView GetViewForHeader (UITableView tableView, nint section)
		{
			ContactSearchBar header = null;
			if (IsSelectingContacts) {
				header = (ContactSearchBar)tableView.DequeueReusableHeaderFooterView (contactHeaderId);
				DataSource.Binding = App.Backend.FindContacts (header.TextField.ObserveText ());
				header.TextField.BecomeFirstResponder ();
			}
			return header;
		}

		protected UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath, object data)
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

		[Export ("tableView:didSelectRowAtIndexPath:")]
		public async void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// Resign first responder on the header so it can go away
			View.EndEditing (true);

			// Get the contact that was selected
			var contact = (Contact)DataSource.GetData (indexPath);

			// Show a loading view controller and load the conversation
			var vc = Storyboard.InstantiateViewController ("LoadingViewController");
			PresentViewController (vc, false, null);

			// Create or resume the converstion
			Conversation = await App.Backend.StartConversation (contact);

			// Enable send button
			sendButton.Enabled = true;

			// Dismiss the loading view controller
			DismissViewController (false, null);
		}
	}
}