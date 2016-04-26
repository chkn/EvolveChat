using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EvolveChat {

	public class DummyServerConnection : IServerConnection {

		static readonly Contact me = new Contact {
			FirstName = "Me",
			LastName = "User"
		};

		static readonly Contact [] dummyContacts = new[] {
			new Contact { FirstName = "Kate", LastName = "Bell" },
			new Contact { FirstName = "Johnny", LastName = "Appleseed" },
			new Contact { FirstName = "John", LastName = "Doe" },
			new Contact { FirstName = "Steve", LastName = "Jobs" },
			new Contact { FirstName = "Bill", LastName = "Gates" },
		};

		static readonly Conversation [] dummyConversations = new[] {
			new Conversation {
				new Message (DateTime.UtcNow.AddMonths (-1), dummyContacts [0], "Hey!"),
				new Message (DateTime.UtcNow.AddMonths (-1), me, "Hey what's up?"),
				new Message (DateTime.UtcNow.AddMinutes (-1), me, "Hey again!"),
				new Message (DateTime.UtcNow.AddMinutes (-1), dummyContacts [0], "Sup"),
				new Message (DateTime.UtcNow.AddMinutes (-1), me, "Not much"),
				new Message (DateTime.UtcNow.AddMinutes (-1), dummyContacts [0], "Cool"),
				new Message (DateTime.UtcNow.AddMinutes (-1), me, "This is a really long message to test that the preview and message bubbles wrap as expected. Hooray!")
			},
			new Conversation {
				new Message (DateTime.UtcNow.AddMonths (-1), dummyContacts [1], "Hey!"),
				new Message (DateTime.UtcNow.AddMonths (-1), me, "Hey what's up?"),
			},
		};

		public Contact Me => me;

		// On a real IServerConnection, we'd prolly also want to keep these as a local cache of data.
		ObservableCollection<Contact> contacts;
		ObservableCollection<Conversation> conversations;

		public ObservableCollection<Contact> GetContacts ()
		{
			if (contacts == null) {
				contacts = new ObservableCollection<Contact> ();
				YieldContacts ();
			}
			return contacts;
		}
		async void YieldContacts ()
		{
			foreach (var contact in dummyContacts) {
				await Task.Delay (500);
				contacts.Add (contact);
			}
		}

		public ObservableCollection<Conversation> GetConversations ()
		{
			if (conversations == null) {
				conversations = new ObservableCollection<Conversation> ();
				YieldConversations ();
			}
			return conversations;
		}
		async void YieldConversations ()
		{
			foreach (var convo in dummyConversations) {
				await Task.Delay (500);
				conversations.Add (convo);
			}
		}

		public async Task<Conversation> StartConversation (params Contact [] contacts)
		{
			await Task.Delay (500);
			var convo = dummyConversations.FirstOrDefault (c => c.Participants.Where (p => p != Me).SequenceEqual (contacts));
			if (convo == null) {
				convo = new Conversation (contacts);
				conversations.Add (convo);
			}
			return convo;
		}
	}
}

