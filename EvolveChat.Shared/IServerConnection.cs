using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EvolveChat {

	public interface IServerConnection {

		/// <summary>
		/// Gets the <c>Contact</c> associated with the currently authenticated user.
		/// </summary>
		Contact Me { get; }

		/// <summary>
		/// Searches contacts on the server in the address book of the currently authenticated user.
		/// </summary>
		/// <remarks>
		/// The returned <c>Contact</c> objects should be configured so that any
		///  changes on the server will be propagated to the local <c>Contact</c> object
		///  as long as the returned observable has subscriptions on it.
		/// </remarks>
		/// <returns>An observable that yields matching contacts. When it is first subscribed,
		///  the observable should yield all matching contacts. On each subsequent subscription,
		///  it should yield all contacts that were found since the last subscription.</returns>
		IObservable<Contact> FindContacts (string searchTerm);

		/// <summary>
		/// Gets all conversations on the server that include the currently authenticated user.
		/// </summary>
		/// <remarks>
		/// The returned <c>Conversation</c> objects should be configured so that any
		///  messages added to the <c>Messages</c> collection will be propagated
		///  to the server, and likewise, any new messages on the server will be
		///  propagated to the local object's <c>Messages</c> collection.
		/// </remarks>
		/// <returns>An observable that yields matching conversations. When it is first subscribed,
		///  the observable should yield all matching conversations. On each subsequent subscription,
		///  it should yield all conversations that were added since the last subscription.</returns>
		IObservable<Conversation> GetMyConversations ();
	}
}

