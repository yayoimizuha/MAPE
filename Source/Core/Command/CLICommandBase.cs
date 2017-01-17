﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using MAPE.Utils;
using MAPE.Properties;


namespace MAPE.Command {
    public abstract class CLICommandBase: CommandBase {
		#region types

		public static new class OptionNames {
			#region constants

			public const string Save = "Save";

			public const string NoLogo = SettingNames.NoLogo;

			#endregion
		}

		public static new class SettingNames {
			#region constants

			public const string NoLogo = "NoLogo";

			#endregion
		}

		public new class ExecutionKind: CommandBase.ExecutionKind {
			#region constants

			public const string SaveSettings = "SaveSettings";

			#endregion
		}

		#endregion


		#region data
		#endregion


		#region creation and disposal

		public CLICommandBase(ComponentFactory componentFactory): base(componentFactory) {
		}

		#endregion


		#region methods

		protected void OutputStandardLogo(Assembly assembly) {
			Console.WriteLine(Resources.CLICommandBase_Logo_Command);
			if (assembly != null) {
				string version = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
				if (string.IsNullOrEmpty(version) == false) {
					Console.WriteLine("version " + version);
				}
				string copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
				if (string.IsNullOrEmpty(copyright) == false) {
					Console.WriteLine(copyright);
				}
			}
			Console.WriteLine();

			return;
		}

		#endregion


		#region overrides/overridables - argument processing

		protected override bool HandleOption(string name, string value, Settings settings) {
			// handle option
			bool handled = true;
			if (AreSameOptionNames(name, OptionNames.Save)) {
				this.Kind = ExecutionKind.SaveSettings;
			} else if (AreSameOptionNames(name, OptionNames.NoLogo)) {
				settings.SetBooleanValue(SettingNames.NoLogo, true);
			} else {
				handled = base.HandleOption(name, value, settings);
			}

			return handled;
		}

		#endregion


		#region overrides/overridables - execution

		public override void Run(string[] args) {
			// connect a ColorConsoleTraceListener during its execution to show color-coded log in the console
			ColorConsoleTraceListener traceListener = new ColorConsoleTraceListener(true);
			Logger.Source.Listeners.Add(traceListener);
			try {
				base.Run(args);
			} finally {
				Logger.Source.Listeners.Remove(traceListener);
			}
		}

		public override void Execute(string commandKind, Settings settings) {
			// argument checks
			Debug.Assert(commandKind != null);

			// show logo
			if (settings.GetBooleanValue(SettingNames.NoLogo, false) == false) {
				OutputLogo();
			}

			// execute command according to the command kind 
			switch (commandKind) {
				case ExecutionKind.SaveSettings:
					SaveSettings(settings);
					break;
				default:
					base.Execute(commandKind, settings);
					break;
			}

			return;
		}

		protected override void RunProxy(Settings settings) {
			// argument checks
			Debug.Assert(settings.IsNull == false);

			// run the proxy
			bool completed = false;
			using (RunningProxyState runningProxyState = StartProxy(settings, this)) {
				// wait for Ctrl+C
				Console.WriteLine(Resources.CLICommandBase_Message_StartListening);
				Console.WriteLine(Resources.CLICommandBase_Message_StartingNote);
				using (ManualResetEvent quitEvent = new ManualResetEvent(false)) {
					// setup Ctrl+C handler
					ConsoleCancelEventHandler ctrlCHandler = (o, e) => {
						e.Cancel = true;
						quitEvent.Set();
					};
					Console.CancelKeyPress += ctrlCHandler;

					// wait for Ctrl+C
					quitEvent.WaitOne();

					// cleanup Ctrl+C handler
					Console.CancelKeyPress -= ctrlCHandler;
				}

				// stop the proxy
				completed = runningProxyState.Stop(5000);
			}
			Console.WriteLine(completed ? Resources.CLICommandBase_Message_Completed : Resources.CLICommandBase_Message_NotCompleted);

			return;
		}

		protected override CredentialInfo UpdateCredential(string endPoint, string realm, CredentialInfo oldCredential) {
			// argument checks
			Debug.Assert(endPoint != null);
			Debug.Assert(realm != null);    // may be empty

			return AskCredentialInfo(endPoint, realm, canSave: this.HasSettingsFile);
		}

		protected virtual void SaveSettings(Settings settings) {
			// argument checks
			Debug.Assert(settings.IsNull == false);

			// state checks
			if (this.HasSettingsFile == false) {
				throw new Exception(Resources.CLICommandBase_Message_NoSettingsFile);
			}

			// save the settings
			SaveSettingsToFile(settings);

			string message = string.Format(Resources.CLICommandBase_SaveSettings_Completed, this.SettingsFilePath);
			Console.WriteLine(message);
		}

		protected virtual void OutputLogo() {
			OutputStandardLogo(null);
		}

		#endregion


		#region overrides/overridables - misc

		protected override void ShowErrorMessage(string message) {
			Console.Error.WriteLine(message);
		}

		#endregion


		#region privates

		private static CredentialInfo AskCredentialInfo(string endPoint, string realm, bool canSave) {
			// argument checks
			Debug.Assert(realm != null);

			// read information from the console
			Console.WriteLine(Resources.CLICommandBase_AskCredential_Description, endPoint);
			Console.WriteLine($"Realm: {endPoint}");
			Console.Write(Resources.CLICommandBase_AskCredential_UserName);
			string userName = Console.ReadLine();
			Console.Write(Resources.CLICommandBase_AskCredential_Password);
			string password = ReadPassword();
			CredentialPersistence persistence = AskCredentialPersistence(canSave);

			return new CredentialInfo(endPoint, userName, password, persistence);
		}

		private static CredentialPersistence AskCredentialPersistence(bool canSave) {
			// read user preference from the console
			do {
				Console.WriteLine(Resources.CLICommandBase_AskCredential_Persistence_Description);
				Console.WriteLine($"  1: {Resources.CLICommandBase_AskCredential_Persistence_Session}");
				Console.WriteLine($"  2: {Resources.CLICommandBase_AskCredential_Persistence_Process}");
				if (canSave) {
					Console.WriteLine($"  3: {Resources.CLICommandBase_AskCredential_Persistence_Persistent}");
				}
				Console.Write(Resources.CLICommandBase_AskCredential_Persistence_Prompt);
				string answer = Console.ReadLine();

				int number;
				if (int.TryParse(answer, out number)) {
					switch (number) {
						case 1:
							return CredentialPersistence.Session;
						case 2:
							return CredentialPersistence.Process;
						case 3:
							if (canSave) {
								return CredentialPersistence.Persistent;
							}
							break;
					}
				}
			} while (true);
		}

		// thanks to http://stackoverflow.com/questions/3404421/password-masking-console-application
		private static string ReadPassword() {
			var buf = new StringBuilder();
			do {
				var keyInfo = Console.ReadKey(intercept: true);
				switch (keyInfo.Key) {
					case ConsoleKey.Enter:
						Console.WriteLine();
						return buf.ToString();
					case ConsoleKey.Backspace:
						if (0 < buf.Length) {
							buf = buf.Remove(buf.Length - 1, 1);
							Console.Write("\b \b");
						}
						break;
					default:
						buf.Append(keyInfo.KeyChar);
						Console.Write("*");
						break;
				}
			} while (true);
		}

		#endregion
	}
}
