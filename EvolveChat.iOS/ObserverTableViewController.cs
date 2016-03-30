using System;
using System.Collections.Generic;

using UIKit;
using Foundation;

namespace EvolveChat {

	public abstract class ObserverTableViewController<TData> : UITableViewController, IObserver<TData> {

		protected List<TData> CachedData { get; private set; } = new List<TData> ();

		// This list contains a list of things to dispose when we disappear.
		List<IDisposable> disposables = new List<IDisposable> ();

		public ObserverTableViewController ()
		{
		}

		public ObserverTableViewController (IntPtr handle): base (handle)
		{
		}

		protected void DisposeOnDisappear (IDisposable disposable)
		{
			if (!disposables.Contains (disposable))
				disposables.Add (disposable);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
			foreach (var d in disposables)
				d.Dispose ();
			disposables.Clear ();
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return CachedData.Count;
		}

		public virtual TData GetData (NSIndexPath indexPath)
		{
			return CachedData [indexPath.Row];
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			return GetCell (tableView, indexPath, GetData (indexPath));
		}

		protected abstract UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath, TData data);

		protected virtual void OnNext (TData value)
		{
			var indexPath = NSIndexPath.FromRowSection (CachedData.Count, 0);
			CachedData.Add (value);
			if (IsViewLoaded)
				TableView.InsertRows (new[] { indexPath }, UITableViewRowAnimation.Automatic);
		}

		protected virtual void OnError (Exception err)
		{
		}

		protected virtual void OnCompleted ()
		{
		}

		#region IObserver<TData> implementation

		void IObserver<TData>.OnNext (TData value)
		{
			BeginInvokeOnMainThread (() => OnNext (value));
		}

		void IObserver<TData>.OnError (Exception err)
		{
			BeginInvokeOnMainThread (() => OnError (err));
		}

		void IObserver<TData>.OnCompleted ()
		{
			BeginInvokeOnMainThread (OnCompleted);
		}

		#endregion
	}
}

