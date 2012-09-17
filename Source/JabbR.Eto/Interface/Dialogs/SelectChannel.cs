using System;
using Eto.Forms;
using System.Runtime.Remoting;
using System.Linq;
using JabbR.Eto.Model;
using Eto.Drawing;

namespace JabbR.Eto.Interface.Dialogs
{
	public class SelectChannel : Dialog
	{
		GridView grid;
		
		public ChannelInfo SelectedChannel {
			get {
				return grid.SelectedItems.FirstOrDefault () as ChannelInfo;
			}
		}
		
		public SelectChannel (Server server)
		{
			this.ClientSize = new Size (600, 400);
			this.Resizable = true;
			this.Title = "Channel List";
			
			grid = new GridView {
				AllowMultipleSelection = false
			};
			grid.MouseDoubleClick += HandleMouseDoubleClick;
			grid.Columns.Add (new GridColumn { DataCell = new TextBoxCell ("Name"), HeaderText = "Channel", Width = 150, AutoSize = false });
			grid.Columns.Add (new GridColumn { DataCell = new TextBoxCell ("UserCount"), HeaderText = "Users", Width = 60, AutoSize = false });
			grid.Columns.Add (new GridColumn { DataCell = new TextBoxCell ("Topic"), HeaderText = "Topic", Width = 350, AutoSize = false });
			
			var layout = new DynamicLayout (this);
			
			layout.Add (grid, yscale: true);
			layout.BeginVertical ();
			layout.AddRow (null, CancelButton (), JoinButton ());
			layout.EndVertical ();
			
			var channelTask = server.GetChannelList ();
			channelTask.ContinueWith (task => {
				Application.Instance.AsyncInvoke (delegate {
					grid.DataStore = new GridItemCollection (task.Result.OrderBy (r => r.Name).OrderByDescending (r => r.UserCount));
				});
			}, System.Threading.Tasks.TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		void HandleMouseDoubleClick (object sender, MouseEventArgs e)
		{
			TriggerJoin ();
		}
		
		void TriggerJoin ()
		{
			if (SelectedChannel != null) {
				DialogResult = DialogResult.Ok;
				Close ();
			}
		}
		
		Control CancelButton ()
		{
			var button = new Button {
				Text = "Cancel"
			};
			AbortButton = button;
			
			button.Click += (sender, e) => {
				DialogResult = DialogResult.Cancel;
				Close ();
			};
			return button;
		}

		Control JoinButton ()
		{
			var button = new Button {
				Text = "Join Channel"
			};
			DefaultButton = button;
			
			button.Click += (sender, e) => {
				TriggerJoin ();
			};
			return button;
		}
	}
}

