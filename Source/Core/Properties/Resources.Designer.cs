﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MAPE.Properties {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MAPE.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   厳密に型指定されたこのリソース クラスを使用して、すべての検索リソースに対し、
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Credential for {0} is required. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_AskCredential_Description {
            get {
                return ResourceManager.GetString("CLICommandBase_AskCredential_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Do you want to enable &quot;Basic Authentication Assumption Mode&quot;? The default value is &quot;Yes&quot;. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_AskCredential_EnableAssumptionMode_Description {
            get {
                return ResourceManager.GetString("CLICommandBase_AskCredential_EnableAssumptionMode_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Password:  に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_AskCredential_Password {
            get {
                return ResourceManager.GetString("CLICommandBase_AskCredential_Password", resourceCulture);
            }
        }
        
        /// <summary>
        ///   How save password? に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_AskCredential_Persistence_Description {
            get {
                return ResourceManager.GetString("CLICommandBase_AskCredential_Persistence_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   save the encrypted password in settings file に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_AskCredential_Persistence_Persistent {
            get {
                return ResourceManager.GetString("CLICommandBase_AskCredential_Persistence_Persistent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   only during running this process に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_AskCredential_Persistence_Process {
            get {
                return ResourceManager.GetString("CLICommandBase_AskCredential_Persistence_Process", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Selection:  に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_AskCredential_Persistence_Prompt {
            get {
                return ResourceManager.GetString("CLICommandBase_AskCredential_Persistence_Prompt", resourceCulture);
            }
        }
        
        /// <summary>
        ///   only during this http session に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_AskCredential_Persistence_Session {
            get {
                return ResourceManager.GetString("CLICommandBase_AskCredential_Persistence_Session", resourceCulture);
            }
        }
        
        /// <summary>
        ///   UserName:  に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_AskCredential_UserName {
            get {
                return ResourceManager.GetString("CLICommandBase_AskCredential_UserName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   MAPE (May Authentication Proxy Explode) に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_Logo_Command {
            get {
                return ResourceManager.GetString("CLICommandBase_Logo_Command", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Completed. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_Message_Completed {
            get {
                return ResourceManager.GetString("CLICommandBase_Message_Completed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   The settings cannot be saved because the settings file to which they are saved is not specified. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_Message_NoSettingsFile {
            get {
                return ResourceManager.GetString("CLICommandBase_Message_NoSettingsFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Cannot confirm its completion. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_Message_NotCompleted {
            get {
                return ResourceManager.GetString("CLICommandBase_Message_NotCompleted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Press Ctrl+C to quit. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_Message_StartingNote {
            get {
                return ResourceManager.GetString("CLICommandBase_Message_StartingNote", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Listening... に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_Message_StartListening {
            get {
                return ResourceManager.GetString("CLICommandBase_Message_StartListening", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Enter Y or N (Y: Yes, N: No): に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_Prompt_YesNo {
            get {
                return ResourceManager.GetString("CLICommandBase_Prompt_YesNo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Enter Y, N or C (Y: Yes, N: No, C: Cancel): に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_Prompt_YesNoCancel {
            get {
                return ResourceManager.GetString("CLICommandBase_Prompt_YesNoCancel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   The settings are saved to &apos;{0}&apos;. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CLICommandBase_SaveSettings_Completed {
            get {
                return ResourceManager.GetString("CLICommandBase_SaveSettings_Completed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Another MAPE instance is already running. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CommandBase_Message_AnotherInstanceIsRunning {
            get {
                return ResourceManager.GetString("CommandBase_Message_AnotherInstanceIsRunning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Failed to load the settings file &apos;{0}&apos;: {1} に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CommandBase_Message_FailToLoadSettingsFile {
            get {
                return ResourceManager.GetString("CommandBase_Message_FailToLoadSettingsFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Failed to save credentials: {0} に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CommandBase_Message_FailToSaveCredentials {
            get {
                return ResourceManager.GetString("CommandBase_Message_FailToSaveCredentials", resourceCulture);
            }
        }
        
        /// <summary>
        ///   An invalid argument &apos;{0}&apos; is specified. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CommandBase_Message_InvalidArgument {
            get {
                return ResourceManager.GetString("CommandBase_Message_InvalidArgument", resourceCulture);
            }
        }
        
        /// <summary>
        ///   An invalid option &apos;{0}&apos; is specified. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CommandBase_Message_InvalidOption {
            get {
                return ResourceManager.GetString("CommandBase_Message_InvalidOption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Tha actual proxy is not detected automatically. Specify it explicitly by settings file or command line. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CommandBase_Message_NoActualProxy {
            get {
                return ResourceManager.GetString("CommandBase_Message_NoActualProxy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   The settings file does not exist. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CommandBase_Message_NoSettingsFile {
            get {
                return ResourceManager.GetString("CommandBase_Message_NoSettingsFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   The system proxy settings which were switched at previous proxing seem not to be restored. Do you want to resotre it?
        ///The found backup is one at {0}. The settings set manually after the point will be overwritten. Be careful especially when the backup is old. 
        ///  Yes: Restore the backup
        ///  No: Do not restore the backup, but delete it
        ///  Cancel: Do nothing
        /// に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string CommandBase_Message_SystemSettingsAreNotRestored {
            get {
                return ResourceManager.GetString("CommandBase_Message_SystemSettingsAreNotRestored", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Failed to restore the previous system settings: {0}
        ///Please restore it manually.
        ///Or, it may be restored from the backup &apos;{1}&apos; if it exists when you restart MAPE. に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string RunningProxyState_Message_FailToRestoreSystemSettings {
            get {
                return ResourceManager.GetString("RunningProxyState_Message_FailToRestoreSystemSettings", resourceCulture);
            }
        }
    }
}
