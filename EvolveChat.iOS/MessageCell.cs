using Foundation;
using System;
using UIKit;

namespace EvolveChat {

	public partial class MessageCell : UITableViewCell {

		public static readonly NSString IncomingId = new NSString ("IncomingMessageCell");
		public static readonly NSString OutgoingId = new NSString ("OutgoingMessageCell");

		public Message Message {
			get { return message; }
			set {
				if (message != value) {
					message = value;
					textView.Text = message.Text;
				}
			}
		}
		Message message;

		public MessageCell (IntPtr handle) : base (handle)
		{
		}
	}
}