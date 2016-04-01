using Foundation;
using System;
using UIKit;

namespace EvolveChat {

	public partial class ContactSearchBar : UITableViewHeaderFooterView {

		public UITextField TextField => textField;

		public ContactSearchBar (IntPtr handle) : base (handle)
		{
		}
	}
}