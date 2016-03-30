using System;
using System.Reflection;

using System.Linq;
using System.Linq.Expressions;

using System.Threading;
using System.Threading.Tasks;

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;

using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace EvolveChat {

	public static class Extensions {

		/// <summary>
		/// Returns an <c>IObservable</c> that yields values like "just now" or "an hour ago"
		///  for the given <c>DateTime</c>.
		/// </summary>
		public static IObservable<string> ObservePrettyString (this DateTime dt)
		{
			return Observable.Create (async (IObserver<string> obs, CancellationToken cancelToken) => {
				while (!cancelToken.IsCancellationRequested) {
					var delta = DateTime.UtcNow - dt;
	
					var seconds = delta.TotalSeconds;
					if (seconds < 45) {
						obs.OnNext ("just now");
						await Task.Delay (TimeSpan.FromSeconds (45 - seconds), cancelToken);
						continue;
					}

					if (delta.TotalHours < 1) {
						var minutes = Math.Round (delta.TotalMinutes);
						obs.OnNext ((minutes <= 1)? "a minute ago" : string.Format ("{0} minutes ago", minutes));
						await Task.Delay (TimeSpan.FromMinutes (1), cancelToken);
						continue;
					}
	
					if (delta.TotalDays < 1) {
						var hours = Math.Round (delta.TotalHours);
						obs.OnNext ((hours <= 1)? "an hour ago" : string.Format ("{0} hours ago", hours));
						await Task.Delay (TimeSpan.FromMinutes (60 - delta.Minutes), cancelToken);
						continue;
					}
		
					obs.OnNext (dt.ToString ("d"));
					break;
				}
			});
		}

		/// <summary>
		/// Creates an <c>IObservable</c> that yields all items added to the given collection.
		/// </summary>
		public static IObservable<TData> ObserveAddedItems<TData> (this ObservableCollection<TData> collection)
		{
			return Observable.FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs> (
				h => (_, e) => h (e),
				h => collection.CollectionChanged += h,
				h => collection.CollectionChanged -= h
			).SelectMany (e => {
				if (e.Action != NotifyCollectionChangedAction.Add)
					throw new NotSupportedException (e.Action.ToString ());
				// We cannot safely cast to IEnumerable<TData> here because
				//  `NewItems` is typed `IList` and may be a non-generic `IList` implementation.
				return ((IEnumerable)e.NewItems).Cast<TData> ();
			});
		}

		/// <summary>
		/// Creates an <c>IObservable</c> that yields the last item in the given collection and any items that are subsequently added.
		/// </summary>
		public static IObservable<TData> ObserveLastAndAddedItems<TData> (this ObservableCollection<TData> collection)
		{
			var obs = collection.ObserveAddedItems ();
			return collection.Any ()? obs.StartWith (collection.Last ()) : obs;
		}

		/// <summary>
		/// Creates an <c>IObservable</c> that yields all existing items in the given collection and any items that are subsequently added.
		/// </summary>
		public static IObservable<TData> ObserveAllItems<TData> (this ObservableCollection<TData> collection)
		{
			return collection.ToObservable ().Concat (collection.ObserveAddedItems ());
		}

		public static IObservable<PropertyChangedEventArgs> ObservePropertyChanges<TSubj> (this TSubj subject)
			where TSubj : INotifyPropertyChanged
		{
			return Observable.FromEvent<PropertyChangedEventHandler,PropertyChangedEventArgs> (
		            h => (_, e) => h (e),
		            h => subject.PropertyChanged += h,
		            h => subject.PropertyChanged -= h
			);
		}

		public static IObservable<TValue> ObservePropertyValue<TSubj,TValue> (this TSubj subject, Expression<Func<TSubj,TValue>> property)
			where TSubj : INotifyPropertyChanged
		{
			// Extract the property from the expression tree
			var propInfo = (property.Body as MemberExpression)?.Member as PropertyInfo;
			if (propInfo == null)
				throw new ArgumentException ("Expression must reference a property", nameof (property));

			return subject.ObservePropertyChanges ()
			              .Where  (p => p.PropertyName == propInfo.Name)
			              .Select (_ => (TValue)propInfo.GetValue (subject))
			              .StartWith ((TValue)propInfo.GetValue (subject));
		}
	}
}

