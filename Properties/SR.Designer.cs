﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
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
        ///   Looks up a localized string similar to HH:mm:ss.
        /// </summary>
        internal static string LoggerDateFormat {
            get {
                return ResourceManager.GetString("LoggerDateFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The minumium value for LogBatchSize is 1..
        /// </summary>
        internal static string MsgExMinLogBatchSize {
            get {
                return ResourceManager.GetString("MsgExMinLogBatchSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Attempt to send using &apos;localhost&apos;, &apos;mail.Adenson.com&apos; and &apos;mail.Adenson.net&apos; all failed (in that specified order). 
        ///Override this behavior by setting System.Net.Mail.SmtpClient server properties in the configuration file settings, for example, an example configuration:
        ///
        ///&lt;configuration&gt;
        ///  &lt;system.net&gt;
        ///    &lt;mailSettings&gt;
        ///      &lt;smtp deliveryMethod=&quot;network&quot;&gt;
        ///        &lt;network host=&quot;localhost&quot; port=&quot;25&quot; defaultCredentials=&quot;true&quot; /&gt;
        ///      &lt;/smtp&gt;
        ///    &lt;/mailSettings&gt;
        ///  &lt;/system.net&gt;
        ///&lt;/configuration&gt;
        ///        /// [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string MsgMailerWarning {
            get {
                return ResourceManager.GetString("MsgMailerWarning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to eventlogger.log.
        /// </summary>
        internal static string VarEventLogFile {
            get {
                return ResourceManager.GetString("VarEventLogFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}	{1}	{2}	{3}	{4}.
        /// </summary>
        internal static string VarEventLoggerFileInsert {
            get {
                return ResourceManager.GetString("VarEventLoggerFileInsert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO [dbo].[EVENT_LOG] (severity, category, error_text, url, created_date) VALUES (&apos;{0}&apos;, &apos;{1}&apos;, &apos;{2}&apos;, &apos;{3}&apos;, &apos;{4}&apos;).
        /// </summary>
        internal static string VarEventLoggerSqlInsertStatement {
            get {
                return ResourceManager.GetString("VarEventLoggerSqlInsertStatement", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [{0}] {1} [{2}] - {3}.
        /// </summary>
        internal static string VarLoggerConsoleOutput {
            get {
                return ResourceManager.GetString("VarLoggerConsoleOutput", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {3}
        ///
        ///
        ///Date: {0}
        ///Type: {1}
        ///Path: {2}.
        /// </summary>
        internal static string VarLoggerEventLogMessage {
            get {
                return ResourceManager.GetString("VarLoggerEventLogMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}
        ///Source: {1}
        ///Site: {2}
        ///{3}
        ///.
        /// </summary>
        internal static string VarLoggerExceptionMessage {
            get {
                return ResourceManager.GetString("VarLoggerExceptionMessage", resourceCulture);
            }
        }
    }
}
