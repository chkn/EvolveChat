using System;

namespace EvolveChat {

	public class Message {

		public DateTime TimeStamp { get; private set; }

		public Contact Sender { get; private set; }

		public string Text { get; private set; }

		public Message (DateTime timeStamp, Contact sender, string text)
		{
			TimeStamp = timeStamp;
			Sender = sender;
			Text = text;
		}
	}
}

