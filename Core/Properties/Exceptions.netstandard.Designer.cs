﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Adenson {
    using System;
    using System.Reflection;
    
    
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
    internal class Exceptions {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Exceptions() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Adenson.Core.Properties.Exceptions", typeof(Exceptions).GetTypeInfo().Assembly);
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
        ///   Looks up a localized string similar to The Algorithm property is used for encryption and decryption, and cannot be null..
        /// </summary>
        internal static string AlgorithmNull {
            get {
                return ResourceManager.GetString("AlgorithmNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified argument &apos;{0}&apos; is invalid, must be greater than zero..
        /// </summary>
        internal static string ArgLessOrEqualToZero {
            get {
                return ResourceManager.GetString("ArgLessOrEqualToZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to None of the argument in the list can be null..
        /// </summary>
        internal static string ArgumentInListNull {
            get {
                return ResourceManager.GetString("ArgumentInListNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are no items in the specified argument list..
        /// </summary>
        internal static string ArgumentsEmpty {
            get {
                return ResourceManager.GetString("ArgumentsEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are {0} transactions still open and need to be closed first. Never call BeginTransaction without calling EndTransaction!!!!!..
        /// </summary>
        internal static string CannotDisposeWithArgOpenTransactions {
            get {
                return ResourceManager.GetString("CannotDisposeWithArgOpenTransactions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No connection string with specified key &apos;{0}&apos; could be found. Check to make sure you have a connection string section in your configuration file with the specified key..
        /// </summary>
        internal static string ConnectionStringWithKeyArgNotFound {
            get {
                return ResourceManager.GetString("ConnectionStringWithKeyArgNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type must be set for custom log handler..
        /// </summary>
        internal static string CustomHandlerTypeTypeMustBeSpecified {
            get {
                return ResourceManager.GetString("CustomHandlerTypeTypeMustBeSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to To email address must be set, and cannot be empty..
        /// </summary>
        internal static string EmailToAddressInvalid {
            get {
                return ResourceManager.GetString("EmailToAddressInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Errant command:
        ///{0}.
        /// </summary>
        internal static string ErrantCommandArg {
            get {
                return ResourceManager.GetString("ErrantCommandArg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Source cannot be null or empty if LogType contains LogType.EventLog..
        /// </summary>
        internal static string EventLogTypeWithSourceNull {
            get {
                return ResourceManager.GetString("EventLogTypeWithSourceNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Getting a {0} hash using this method is unsupported. Use a salt supported version, i.e. GetHash(HashType,byte[],byte[]) instead..
        /// </summary>
        internal static string GetHashArgNotSupported {
            get {
                return ResourceManager.GetString("GetHashArgNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No encryptor with that key exists..
        /// </summary>
        internal static string NoEncryptorExists {
            get {
                return ResourceManager.GetString("NoEncryptorExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There are no current open transactions..
        /// </summary>
        internal static string NoOpenTransactions {
            get {
                return ResourceManager.GetString("NoOpenTransactions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Retrieving property values using reflection for more than one arrayed object is not supported, yet!.
        /// </summary>
        internal static string NoSupportNonSingleParameterArrayedObjects {
            get {
                return ResourceManager.GetString("NoSupportNonSingleParameterArrayedObjects", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No types implementing &apos;{0}&apos; could be found. It is possible the assembly required might be loaded. Use TypeUtil.LoadAssembly to try and load it..
        /// </summary>
        internal static string NoTypeImplementingArgFound {
            get {
                return ResourceManager.GetString("NoTypeImplementingArgFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The property or argument cannot be null or empty..
        /// </summary>
        internal static string NullOrEmptyArgument {
            get {
                return ResourceManager.GetString("NullOrEmptyArgument", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A field or property with the name &apos;{0}&apos; was not found on the provided data item..
        /// </summary>
        internal static string PropertyOrFieldArgNotFound {
            get {
                return ResourceManager.GetString("PropertyOrFieldArgNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A field or property with the name &apos;{0}&apos; was not found on the provided data item..
        /// </summary>
        internal static string PropertyOrFieldNotFound {
            get {
                return ResourceManager.GetString("PropertyOrFieldNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This instance is read only..
        /// </summary>
        internal static string ReadOnlyInstance {
            get {
                return ResourceManager.GetString("ReadOnlyInstance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The section is not of DictionarySectionHandler..
        /// </summary>
        internal static string SectionNotDictionarySection {
            get {
                return ResourceManager.GetString("SectionNotDictionarySection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type &apos;{0}&apos; could not be loaded..
        /// </summary>
        internal static string TypeArgCouldNotBeLoaded {
            get {
                return ResourceManager.GetString("TypeArgCouldNotBeLoaded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified command text argument could not be parsed. 
        ///Use formattable strings (e.g. &apos;UPDATE [Table] SET [Column1] = {0}, [Column2] = {1}&apos;)	
        ///-OR-
        ///Use sql parameterized strings (e.g.&apos;UPDATE [Table] SET [Column1] = @parameter1, [Column2] = @parameter2&apos;) and parameter arguments that MUST be one of &apos;Adenson.Data.Parameter&apos;, &apos;System.Data.IDataParameter&apos;, System.Collections.Generic.KeyValuePair&lt;string,object&gt; or System.Collections.IDictionary.	 .
        /// </summary>
        internal static string UnableToParseCommandText {
            get {
                return ResourceManager.GetString("UnableToParseCommandText", resourceCulture);
            }
        }
    }
}
