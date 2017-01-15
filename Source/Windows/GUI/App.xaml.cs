﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using MAPE.Utils;
using AssemblyResources = MAPE.Windows.GUI.Properties.Resources;


namespace MAPE.Windows.GUI {
	public partial class App: Application {
		#region types

		[Flags]
		public enum UIState {
			ExitEnabled = 0x01,
			StartEnabled = 0x02,
			StopEnabled = 0x04,
			SettingsEnabled = 0x08,
			VersionInfoEnabled = 0x10,

			InitialState = ExitEnabled | StartEnabled | SettingsEnabled | VersionInfoEnabled,
		}

		#endregion


		#region data

		private Command command;

		private UIState uiState;

		private bool runningProxy;

		private NotifyIconComponent notifyIcon;

		private Window mainWindow;

		private Window settingsWindow;

		private Window versionInfoWindow;

		#endregion


		#region creation and disposal

		internal App(Command command) : base() {
			// argument checks
			if (command == null) {
				throw new ArgumentNullException(nameof(command));
			}

			// initialize members
			this.command = command;
			this.uiState = UIState.InitialState;
			this.runningProxy = false;
			this.notifyIcon = null;
			this.mainWindow = null;

			return;
		}

		#endregion


		#region methods

		internal void StartProxy() {
			// state checks
			Command command = this.command;
			if (command == null) {
				throw new InvalidOperationException();
			}
			Debug.Assert((this.uiState & UIState.StartEnabled) != 0);

			try {
				// update UI not to be selected 'Start' menu
				this.runningProxy = true;
				UpdateUIState();

				// start the proxy
				command.StartProxy();
				this.notifyIcon.Icon = AssemblyResources.OnIcon;
			} catch (Exception exception) {
				// restore UI 
				this.runningProxy = false;
				UpdateUIState();

				throw;
			}

			return;
		}

		internal void StopProxy() {
			// state checks
			Command command = this.command;
			if (command == null) {
				throw new InvalidOperationException();
			}
			Debug.Assert((this.uiState & UIState.StopEnabled) != 0);

			try {
				// stop the proxy
				this.notifyIcon.Icon = AssemblyResources.OffIcon;
				command.StopProxy();
			} finally {
				// update UI not to be selected 'Stop' menu
				this.runningProxy = false;
				UpdateUIState();
			}

			return;
		}

		internal void ErrorMessage(string message) {
			// ToDo: the way to show error
			MessageBox.Show(message, this.command.ComponentName, MessageBoxButton.OK, MessageBoxImage.Error);
		}

		#endregion


		#region overrides

		protected override void OnStartup(StartupEventArgs e) {
			// process the base class level tasks
			base.OnStartup(e);

			// process this class level tasks
			this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

			NotifyIconComponent notifyIcon = new NotifyIconComponent();
			notifyIcon.StartMenuItem.Click += this.StartMenuItem_Click;
			notifyIcon.StopMenuItem.Click += this.StopMenuItem_Click;
			notifyIcon.OpenMenuItem.Click += this.OpenMenuItem_Click;
			notifyIcon.SettingsMenuItem.Click += this.SettingsMenuItem_Click;
			notifyIcon.VersionInfoMenuItem.Click += this.VersionInfoMenuItem_Click;
			notifyIcon.ExitMenuItem.Click += this.ExitMenuItem_Click;
			this.notifyIcon = notifyIcon;

			// process this class level tasks
			OnUIStateChanged(this.uiState);

			return;
		}

		protected override void OnExit(ExitEventArgs e) {
			// process this class level tasks
			Util.DisposeWithoutFail(ref this.notifyIcon);

			// process the base class level tasks
			base.OnExit(e);
		}

		protected override void OnLoadCompleted(NavigationEventArgs e) {
			// process the base class level tasks
			base.OnLoadCompleted(e);

			// process this class level tasks
			OnUIStateChanged(this.uiState);

			return;
		}

		#endregion


		#region privates

		private UIState GetUIState() {
			UIState state = UIState.ExitEnabled | UIState.VersionInfoEnabled;
			if (this.runningProxy) {
				state |= UIState.StopEnabled;
			} else {
				state |= UIState.StartEnabled;
				state |= UIState.SettingsEnabled;
			}

			return state;
		}

		private void UpdateUIState() {
			UIState newState = GetUIState();
			if (newState != this.uiState) {
				this.uiState = newState;
				OnUIStateChanged(newState);
			}

			return;
		}

		private void OnUIStateChanged(UIState newState) {
			NotifyIconComponent notifyIcon = this.notifyIcon;
			if (notifyIcon != null) {
				notifyIcon.ExitMenuItem.Enabled = ((newState & UIState.ExitEnabled) != 0);
				notifyIcon.StartMenuItem.Enabled = ((newState & UIState.StartEnabled) != 0);
				notifyIcon.StopMenuItem.Enabled = ((newState & UIState.StopEnabled) != 0);
				notifyIcon.SettingsMenuItem.Enabled = ((newState & UIState.SettingsEnabled) != 0);
				notifyIcon.VersionInfoMenuItem.Enabled = ((newState & UIState.VersionInfoEnabled) != 0);
			}

			return;
		}

		#endregion


		#region event handlers

		private void StartMenuItem_Click(object sender, EventArgs e) {
			try {
				StartProxy();
			} catch (Exception exception) {
				ErrorMessage(exception.Message);
			}
		}

		private void StopMenuItem_Click(object sender, EventArgs e) {
			try {
				StopProxy();
			} catch (Exception exception) {
				ErrorMessage(exception.Message);
			}
		}

		private void OpenMenuItem_Click(object sender, EventArgs e) {
			try {
				Window window = this.mainWindow;
				if (window != null) {
					window.Activate();
				} else {
					window = new MainWindow();
					window.Closed += mainWindow_Closed;
					this.mainWindow = window;
					window.Show();
				}
			} catch (Exception exception) {
				ErrorMessage(exception.Message);
			}
		}

		private void SettingsMenuItem_Click(object sender, EventArgs e) {
			try {
				Window window = this.settingsWindow;
				if (window != null) {
					window.Activate();
				} else {
					window = new SettingsWindow();
					window.Closed += settingsWindow_Closed;
					this.settingsWindow = window;
					window.Show();
				}
			} catch (Exception exception) {
				ErrorMessage(exception.Message);
			}
		}

		private void VersionInfoMenuItem_Click(object sender, EventArgs e) {
			try {
				Window window = this.versionInfoWindow;
				if (window != null) {
					window.Activate();
				} else {
					window = new VersionInfoWindow();
					window.Closed += versionInfoWindow_Closed;
					this.versionInfoWindow = window;
					window.Show();
				}
			} catch (Exception exception) {
				ErrorMessage(exception.Message);
			}
		}

		private void ExitMenuItem_Click(object sender, EventArgs e) {
			Shutdown();
		}

		private void mainWindow_Closed(object sender, EventArgs e) {
			this.mainWindow = null;
		}

		private void settingsWindow_Closed(object sender, EventArgs e) {
			this.settingsWindow = null;
		}

		private void versionInfoWindow_Closed(object sender, EventArgs e) {
			this.versionInfoWindow = null;
		}

		#endregion
	}
}
