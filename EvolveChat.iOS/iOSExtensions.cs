using System;
using System.Linq;
using System.Collections.Generic;

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;

using UIKit;
using Foundation;

namespace EvolveChat {

	public static class iOSExtensions {

		public static IObservable<string> ObserveText (this UITextField textField)
		{
			return new UITextFieldObservable (textField);
		}
		class UITextFieldObservable : IObservable<string> {

			UITextField textField;
			NSObject subscription;
			List<IObserver<string>> observers = new List<IObserver<string>> ();

			public UITextFieldObservable (UITextField textField)
			{
				this.textField = textField;
			}

			public IDisposable Subscribe (IObserver<string> observer)
			{
				if (!observers.Contains (observer))
					observers.Add (observer);
				if (subscription == null) {
					subscription = NSNotificationCenter.DefaultCenter.AddObserver (
						UITextField.TextFieldTextDidChangeNotification,
						OnTextChanged,
						textField
					);
					textField.EditingDidEnd += OnEditingEnded;
				}
				return Disposable.Create (() => {
					observers.Remove (observer);
					if (!observers.Any ())
						Dispose ();
				});
			}

			void OnEditingEnded (object sender, EventArgs e)
			{
				foreach (var observer in observers)
					observer.OnCompleted ();
				observers.Clear ();
				Dispose ();
			}

			void OnTextChanged (NSNotification notif)
			{
				var text = textField.Text;
				foreach (var observer in observers)
					observer.OnNext (text);
			}

			void Dispose ()
			{
				if (subscription != null) {
					textField.EditingDidEnd -= OnEditingEnded;
					NSNotificationCenter.DefaultCenter.RemoveObserver (
						subscription,
						UITextField.TextFieldTextDidChangeNotification,
						textField
					);
					subscription = null;
				}
			}
		}
	}
}

