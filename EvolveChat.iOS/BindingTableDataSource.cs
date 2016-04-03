using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using UIKit;
using Foundation;
using System.Collections.Specialized;

namespace EvolveChat {

	public delegate UITableViewCell GetCellCallback (UITableView tableView, NSIndexPath indexPath, object data);

	/// <summary>
	/// A <c>UITableViewDataSource</c> that can be bound to an observable collection.
	/// </summary>
	public class BindingTableDataSource : UITableViewDataSource {

		public IList Binding {
			get { return binding; }
			set {
				if (binding != value) {
					var inpc = binding as INotifyCollectionChanged;
					if (inpc != null)
						inpc.CollectionChanged -= OnCollectionChanged;

					binding = value;
					inpc = binding as INotifyCollectionChanged;
					if (inpc != null)
						inpc.CollectionChanged += OnCollectionChanged;
					if (TableView.DataSource == this)
						TableView.ReloadData ();
				}
			}
		}
		IList binding;

		public UITableView TableView {
			get;
			private set;
		}

		GetCellCallback getCell;

		public BindingTableDataSource (UITableView tableView, GetCellCallback getCell)
		{
			this.TableView = tableView;
			this.getCell = getCell;
		}

		void OnCollectionChanged (object sender, NotifyCollectionChangedEventArgs e)
		{
			BeginInvokeOnMainThread (() => {
				NSIndexPath [] indexPaths;
				switch (e.Action) {

				case NotifyCollectionChangedAction.Add:
					indexPaths = Enumerable.Range (e.NewStartingIndex, e.NewItems.Count)
					                       .Select (i => NSIndexPath.FromRowSection (i, 0))
					                       .ToArray ();
					TableView.InsertRows (indexPaths, UITableViewRowAnimation.Automatic);
					return;

				case NotifyCollectionChangedAction.Remove:
					indexPaths = Enumerable.Range (e.OldStartingIndex, e.OldItems.Count)
					                       .Select (i => NSIndexPath.FromRowSection (i, 0))
					                       .ToArray ();
					TableView.DeleteRows (indexPaths, UITableViewRowAnimation.Automatic);
					return;

				case NotifyCollectionChangedAction.Reset:
					TableView.ReloadData ();
					return;
				}
				throw new NotImplementedException (e.Action.ToString ());
			});
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return Binding?.Count ?? 0;
		}

		public virtual object GetData (NSIndexPath indexPath)
		{
			return Binding [indexPath.Row];
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			return getCell (tableView, indexPath, GetData (indexPath));
		}
	}
}

