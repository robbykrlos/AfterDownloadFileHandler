﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AfterDownloadFileHandler.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.4.0.0")]
    internal sealed partial class Params : global::System.Configuration.ApplicationSettingsBase {
        
        private static Params defaultInstance = ((Params)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Params())));
        
        public static Params Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("m:/")]
        public string REMOTE_MOVIES_PATH {
            get {
                return ((string)(this["REMOTE_MOVIES_PATH"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("i:/")]
        public string REMOTE_SERIES_PATH {
            get {
                return ((string)(this["REMOTE_SERIES_PATH"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Copy2NAS")]
        public string LABEL_TRIGGER_VALUE {
            get {
                return ((string)(this["LABEL_TRIGGER_VALUE"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AUTO_SUBTITLE_DOWNLOAD {
            get {
                return ((bool)(this["AUTO_SUBTITLE_DOWNLOAD"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("m:/ZZZ/")]
        public string REMOTE_UNKNOWN_PATH {
            get {
                return ((string)(this["REMOTE_UNKNOWN_PATH"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("dat,Dat,DAT,nfo,Nfo,NFO")]
        public string SKIP_FILE_EXTENSIONS {
            get {
                return ((string)(this["SKIP_FILE_EXTENSIONS"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("str,srt,sub")]
        public string SUB_EXTENSIONS {
            get {
                return ((string)(this["SUB_EXTENSIONS"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("rum,eng")]
        public string ASD_LANG {
            get {
                return ((string)(this["ASD_LANG"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("sample,Sample,SAMPLE,samples,Samples,SAMPLES")]
        public string SKIP_SAMPLES {
            get {
                return ((string)(this["SKIP_SAMPLES"]));
            }
        }
    }
}
