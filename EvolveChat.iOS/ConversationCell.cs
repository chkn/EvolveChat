using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using System.Reactive;
using System.Reactive.Linq;

using UIKit;
using Foundation;

namespace EvolveChat {

    public partial class ConversationCell : UITableViewCell {

		public static readonly NSString Id = new NSString ("ConversationCell");

		IDisposable participantSubscription, messageSubscription, timeStampSubscription;

		public Conversation Conversation {
			get { return conversation; }
			set {
				if (conversation != value) {
					// Configure the cell for the new conversation...
					conversation = value;

					// Set the name(s), and update them if someone joins the conversation
					participantSubscription?.Dispose ();
					participantSubscription = conversation.ObservePropertyValue (c => c.Participants)
						.ObserveOn (SynchronizationContext.Current)
						.Subscribe (participants => {
							// Get the participants in the conversation who are not the current user
							var others = participants.Where (p => p != App.Backend.Me).ToList ();
							name.Text = string.Join (", ", others);
							badge.Text = (others.Count == 1)? (others [0].FirstName.Substring (0, 1) + others [0].LastName.Substring (0, 1)) : others.Count.ToString ();
						});

					// Set the last message preview and date, and update them if a new message is received
					messageSubscription?.Dispose ();
					messageSubscription = conversation.Messages.ObserveLastAndAddedItems ()
						.ObserveOn (SynchronizationContext.Current)
						.Subscribe (msg => {
							// Simply set the preview label to the entire message; UIKit will take care
							//  of line breaks and truncation.
							preview.Text = msg.Text;
	
							// Setup the date label to automatically update
							timeStampSubscription?.Dispose ();
							timeStampSubscription = msg.TimeStamp.ObservePrettyString ().Subscribe (str => date.Text = str);
						});
				}
			}
		}
		Conversation conversation;

        public ConversationCell (IntPtr handle) : base (handle)
        {
        }
    }
}