﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using MAPE.Utils;
using MAPE.Server;


namespace MAPE.Command {
	public class SystemSettingsSwitcherForWindows: SystemSettingsSwitcher {
		#region types

		public static new class SettingNames {
			#region constants

			public const string ProxyOverride = "ProxyOverride";

			#endregion
		}

		public static class RegistryNames {
			#region constants

			public const string AutoConfigURL = "AutoConfigURL";

			public const string ProxyEnable = "ProxyEnable";

			public const string ProxyServer = "ProxyServer";

			public const string ProxyOverride = "ProxyOverride";

			#endregion
		}

		public static class EnvironmentNames {
			#region constants

			public const string HttpProxy = "http_proxy";

			public const string HttpsProxy = "https_proxy";

			#endregion
		}

		#endregion


		#region data

		public string AutoConfigURL { get; protected set; } = null;

		public int? ProxyEnable { get; protected set; } = null;

		// ex. http=proxy.example.org:8080;https=proxy.example.org:8080
		public string ProxyServer { get; protected set; } = null;

		// ex. *.example.org;*.example.jp;<local>
		public string ProxyOverride { get; protected set; } = null;

		public string HttpProxyEnvironmentVariable { get; protected set; } = null;

		public string HttpsProxyEnvironmentVariable { get; protected set; } = null;

		#endregion


		#region creation and disposal

		public SystemSettingsSwitcherForWindows(CommandBase owner, Settings settings, Proxy proxy) : base(owner, settings, proxy) {
			// argument checks
			// settings can contain null

			// initialize members
			if (settings.IsNull == false) {
				Debug.Assert(proxy != null);
				string proxyEndPoint = proxy.MainListenerEndPoint.ToString();

				Debug.Assert(this.AutoConfigURL == null);
				this.ProxyEnable = 1;
				this.ProxyServer = $"http={proxyEndPoint};https={proxyEndPoint}";
				this.ProxyOverride = settings.GetStringValue(SettingNames.ProxyOverride, null);
				this.HttpProxyEnvironmentVariable = $"http://{proxyEndPoint}";
				this.HttpsProxyEnvironmentVariable = $"http://{proxyEndPoint}";
			}

			return;
		}

		#endregion


		#region overridables

		protected override void LoadCurrentSettings() {
			// load the base class level settings
			base.LoadCurrentSettings();

			// load this class level settings

			// read Internet Options from the registry 
			using (RegistryKey key = GetInternetSettingKey(writable: false)) {
				// AutoConfigURL
				this.AutoConfigURL = (string)key.GetValue(RegistryNames.AutoConfigURL, defaultValue: null);

				// ProxyEnable
				this.ProxyEnable = (int?)key.GetValue(RegistryNames.ProxyEnable, defaultValue: null);

				// ProxyServer
				this.ProxyServer = (string)key.GetValue(RegistryNames.ProxyServer, defaultValue: null);

				// ProxyOverride
				this.ProxyOverride = (string)key.GetValue(RegistryNames.ProxyOverride, defaultValue: null);
			}

			// read User Environment Variables from the registry
			using (RegistryKey key = GetEnvironmentKey(writable: false)) {
				// HttpProxyEnvironmentVariable
				this.HttpProxyEnvironmentVariable = (string)key.GetValue(EnvironmentNames.HttpProxy, defaultValue: null);

				// HttpsProxyEnvironmentVariable
				this.HttpsProxyEnvironmentVariable = (string)key.GetValue(EnvironmentNames.HttpsProxy, defaultValue: null);
			}

			return;
		}

		protected override bool Switch() {
			// set Internet Options in the registry 
			using (RegistryKey key = GetInternetSettingKey(writable: true)) {
				// AutoConfigURL
				SetValue(key, RegistryNames.AutoConfigURL, this.AutoConfigURL);

				// ProxyEnable
				SetValue(key, RegistryNames.ProxyEnable, this.ProxyEnable);

				// ProxyServer
				SetValue(key, RegistryNames.ProxyServer, this.ProxyServer);

				// ProxyOverride
				SetValue(key, RegistryNames.ProxyOverride, this.ProxyOverride);
			}

			// set User Environment Variables in the registry
			using (RegistryKey key = GetEnvironmentKey(writable: true)) {
				// HttpProxyEnvironmentVariable
				SetValue(key, EnvironmentNames.HttpProxy, this.HttpProxyEnvironmentVariable);

				// HttpsProxyEnvironmentVariable
				SetValue(key, EnvironmentNames.HttpsProxy, this.HttpsProxyEnvironmentVariable);
			}

			return true;
		}

		protected override void NotifySwitched() {
			// notify the internet settings are changed
			InternetSetOption(
				IntPtr.Zero,    // NULL
				39,             // INTERNET_OPTION_SETTINGS_CHANGED
				IntPtr.Zero,    // NULL
				0
			);

			// notify that environment variables are changed
			UIntPtr dummy;
			SendMessageTimeout(
				(IntPtr)0xffff,     // HWND_BROADCAST
				0x001A,             // WM_SETTINGCHANGE
				UIntPtr.Zero,
				"Environment",
				0x0002,             // SMTO_ABORTIFHUNG,
				5000,
				out dummy
			);

			return;
		}

		#endregion


		#region private

		private static RegistryKey GetInternetSettingKey(bool writable) {
			return Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings", writable);
		}

		private static RegistryKey GetEnvironmentKey(bool writable) {
			return Registry.CurrentUser.OpenSubKey(@"Environment", writable);
		}

		private static void SetValue(RegistryKey key, string name, string value) {
			// argument checks
			Debug.Assert(key != null);
			// name can be null or empty (that means the default value)
			// value can be null

			if (value == null) {
				key.DeleteValue(name, throwOnMissingValue: false);
			} else {
				key.SetValue(name, value, RegistryValueKind.String);
			}
		}

		private static void SetValue(RegistryKey key, string name, int? value) {
			// argument checks
			Debug.Assert(key != null);
			// name can be null or empty (that means the default value)
			// value can be null

			if (value == null) {
				key.DeleteValue(name, throwOnMissingValue: false);
			} else {
				key.SetValue(name, value.Value, RegistryValueKind.DWord);
			}
		}

		#endregion


		#region interop entries

		// in our use, lParam is used as string.
		[DllImport("User32.dll", SetLastError = true)]
		public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, UIntPtr wParam, string lParam, uint fuFlags, uint uTimeout, out UIntPtr lpdwResult);

		[DllImport("wininet.dll", SetLastError = true)]
		private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

		#endregion
	}
}