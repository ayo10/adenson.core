using System;
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Adenson.Collections", Justification = "Consider another namespace, like?")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "Adenson.Collections.Hashtable`2")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Adenson.Collections.Hashtable`2.#System.Collections.Generic.ICollection`1<System.Collections.Generic.KeyValuePair`2<!0,!1>>.Contains(System.Collections.Generic.KeyValuePair`2<!0,!1>)", Justification = "Not supported")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Adenson.Collections.Hashtable`2.#System.Collections.Generic.ICollection`1<System.Collections.Generic.KeyValuePair`2<!0,!1>>.CopyTo(System.Collections.Generic.KeyValuePair`2<!0,!1>[],System.Int32)", Justification = "Not supported")]

[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Adenson.Configuration.LoggerSettings.#get_SeverityActual()")]
[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Adenson.Configuration.LoggerSettings.#get_TypeActual()")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Scope = "member", Target = "Adenson.Configuration.LoggerSettings.#Type")]

[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "Adenson.Cryptography.DES")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "Adenson.Cryptography.EncryptorType.#DES")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "member", Target = "Adenson.Cryptography.EncryptorType.#TripleDES")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "Adenson.Cryptography.TripleDES")]

[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "Adenson.Data.OleDbSqlHelper")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Scope = "type", Target = "Adenson.Data.SqlCeHelper")]

[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Adenson.FileUtil.#TryGetBytes(System.String,System.Byte[]&)")]

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Adenson.Log", Justification = "Consider another namespace, like?")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Scope = "member", Target = "Adenson.Log.LogEntry.#Type")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Scope = "member", Target = "Adenson.Log.Logger.#Type")]
[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Adenson.Log.Logger.#SaveToDatabase()")]
[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Adenson.Log.Logger.#SaveToEntryLog()")]
[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Adenson.Log.Logger.#SaveToFile()")]
[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Adenson.Log.Logger.#Write(Adenson.Log.LogSeverity,System.String,System.Object[])")]

[assembly: SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Adenson.Mailer.#Send(System.String,System.Net.Mail.MailMessage,System.Boolean)")]

[assembly: SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", Scope = "member", Target = "Adenson.StringUtil.#Format(System.String,System.Object[])")]

[assembly: SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Scope = "member", Target = "Adenson.TypeUtil.#TryConvert(System.Object,System.Type,System.Object&)")]