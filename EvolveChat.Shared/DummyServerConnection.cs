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

		public ObservableCollection<Contact> FindContacts (IObservable<string> searchTerm)
		{
			var results = new ObservableCollection<Contact> ();
			searchTerm.Subscribe (new ContactSearchObserver (results));
			return results;
		}
		class ContactSearchObserver : IObserver<string> {

			bool stop;
			int counter;
			ObservableCollection<Contact> results;

			public ContactSearchObserver (ObservableCollection<Contact> results)
			{
				this.results = results;
			}

			public async void OnNext (string searchTerm)
			{
				// This would update the query on the server to return matches for the new searchTerm...
				var oldItems = results.ToArray ();
				var newItems = dummyContacts
						.Where (c => c.FirstName.ToLowerInvariant ().Contains (searchTerm.ToLowerInvariant ()) || 
						             c.LastName.ToLowerInvariant ().Contains (searchTerm.ToLowerInvariant ()))
						.ToList ();

				// Remove items that no longer match the search term
				foreach (var item in oldItems) {
					if (!newItems.Contains (item))
						results.Remove (item);
				}

				// Add new items that now match the search term
				// Use a counter to bail if a new searchTerm comes in while we're yielded
				var lastCount = ++counter;
				foreach (var item in newItems) {
					await Task.Delay (1000);
					if (stop || lastCount != counter)
						return;
					oldItems = results.ToArray ();
					if (!oldItems.Contains (item))
						results.Add (item);
				}
			}

			public void Stop ()
			{
				// This would cancel the query on the server...
				stop = true;
			}

			public void OnCompleted ()
			{
				Stop ();
			}
			public void OnError (Exception error)
			{
				// IRL we'd prolly log the error.
				Stop ();
			}
		}

		// On a real IServerConnection, we'd prolly also want to keep this as a local cache of data.
		ObservableCollection<Conversation> myConversations;

		public ObservableCollection<Conversation> GetMyConversations ()
		{
			if (myConversations == null) {
				myConversations = new ObservableCollection<Conversation> ();
				YieldConversations ();
			}
			return myConversations;
		}
		async void YieldConversations ()
		{
			foreach (var convo in dummyConversations) {
				await Task.Delay (500);
				myConversations.Add (convo);
			}
		}

		public async Task<Conversation> StartConversation (params Contact [] contacts)
		{
			await Task.Delay (500);
			var convo = dummyConversations.FirstOrDefault (c => c.Participants.Where (p => p != Me).SequenceEqual (contacts));
			if (convo == null) {
				convo = new Conversation (contacts);
				myConversations.Add (convo);
			}
			return convo;
		}
	}
}

