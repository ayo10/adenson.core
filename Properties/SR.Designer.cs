﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18034
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Adenson {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SR {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SR() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Adenson.Properties.SR", typeof(SR).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
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
        ///   Looks up a localized string similar to An error occured when trying to create a Event Log with source &apos;{0}&apos;. Defaulting to &apos;Application&apos;. Error is &apos;{1}&apos;. See http://msdn.microsoft.com/en-us/library/6s7642se.aspx for a solution, the short of it being you will need to give the running user rights to be able to read the registry key HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Eventlog\Security..
        /// </summary>
        internal static string EventLogWarning {
            get {
                return ResourceManager.GetString("EventLogWarning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The minimum value for &apos;LogBatchSize&apos; is 1..
        /// </summary>
        internal static string MsgExMinLogBatchSize {
            get {
                return ResourceManager.GetString("MsgExMinLogBatchSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to START.
        /// </summary>
        internal static string ProfilerStart {
            get {
                return ResourceManager.GetString("ProfilerStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FINISH.
        /// </summary>
        internal static string ProfilerStop {
            get {
                return ResourceManager.GetString("ProfilerStop", resourceCulture);
            }
        }
    }
}
