using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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
			// ...
		};

		static readonly Conversation [] dummyConversations = new[] {
			new Conversation {
				new Message (DateTime.UtcNow.AddMonths (-1), dummyContacts [0], "Hey!"),
				new Message (DateTime.UtcNow.AddMonths (-1), me, "Hey what's up?"),
				new Message (DateTime.UtcNow.AddMinutes (-1), me, "Hey again!"),
				new Message (DateTime.UtcNow.AddMinutes (-1), dummyContacts [0], "Sup"),
				new Message (DateTime.UtcNow.AddMinutes (-1), me, "Not much"),
				new Message (DateTime.UtcNow.AddMinutes (-1), dummyContacts [0], "Cool"),
			},
			new Conversation {
				new Message (DateTime.UtcNow.AddMonths (-1), dummyContacts [1], "Hey!"),
				new Message (DateTime.UtcNow.AddMonths (-1), me, "Hey what's up?"),
			},
		};

		public Contact Me => me;

		public IObservable<Contact> FindContacts (string searchTerm)
		{
			return new DummyContactObservable (searchTerm);
		}
		class DummyContactObservable : IObservable<Contact>, IDisposable {

			bool disposed;
			List<Contact> contacts;

			public DummyContactObservable (string searchTerm)
			{
				contacts = dummyContacts
					.Where (c => c.FirstName.Contains (searchTerm) || c.LastName.Contains (searchTerm))
					.ToList ();
			}

			public IDisposable Subscribe (IObserver<Contact> obs)
			{
				YieldData (obs);
				return this;
			}

			async void YieldData (IObserver<Contact> obs)
			{
				foreach (var contact in contacts.ToArray ()) {
					await Task.Delay (250);
					if (disposed)
						return;
					obs.OnNext (contact);
					contacts.Remove (contact);
				}
				obs.OnCompleted ();
			}

			public void Dispose ()
			{
				disposed = true;
			}
		}

		public IObservable<Conversation> GetMyConversations ()
		{
			return new DummyConversationObservable ();
		}
		class DummyConversationObservable : IObservable<Conversation>, IDisposable {

			bool disposed;
			List<Conversation> conversations;

			public DummyConversationObservable ()
			{
				conversations = dummyConversations.ToList ();
			}

			public IDisposable Subscribe (IObserver<Conversation> obs)
			{
				YieldData (obs);
				return this;
			}

			async void YieldData (IObserver<Conversation> obs)
			{
				foreach (var convo in conversations.ToArray ()) {
					await Task.Delay (250);
					if (disposed)
						return;
					obs.OnNext (convo);
					conversations.Remove (convo);
				}
				obs.OnCompleted ();
				
				// The following is just to test different UI bindings..
				await Task.Delay (2000);
				dummyConversations [0].Messages.Add (new Message (DateTime.UtcNow, dummyContacts [2], "Hey guys!")); 
			}

			public void Dispose ()
			{
				disposed = true;
			}
		}
	}
}

