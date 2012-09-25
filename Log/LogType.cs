using System;

namespace Adenson.Log
{
	/// <summary>
	/// Enumeration of log types
	/// </summary>
	[Flags]
	public enum LogTypes
	{
		/// <summary>
		/// No logs
		/// </summary>
		None = 0,

		/// <summary>
		/// Logs into a physical file.
		/// </summary>
		/// <remarks>
		/// <para>Files are written to the directory that contains the executing assembly, except for ASP.NET projects, which are written in the directory above the bin folder.</para>
		/// <para>The user application is running under should have file create, read and write access to the specified location stated above</para>
		/// <para>If config setting adenson/loggerSettings.fileName is prepended with a directory name, the directory should exit before hand</para>
		/// </remarks>
		File = 1,

		/// <summary>
		/// Logs via Console.WriteLine 
		/// </summary>
		Console = 2,

		/// <summary>
		/// Logs via System.Diagnostics.Debug.WriteLine
		/// </summary>
		Debug = 4,

		/// <summary>
		/// Logs via System.Diagnostics.Trace.WriteLine
		/// </summary>
		Trace = 8,

		/// <summary>
		/// Logs via email (expects config setting adenson/loggerSettings/emailInfo[from, to] to be set
		/// </summary>
		Email = 16,

		/// <summary>
		/// Logs into Windows Event Log
		/// </summary>
		EventLog = 32,

		/// <summary>
		/// Logs into a database (expects config setting adenson/loggerSettings/databaseInfo[tableName,severityColumn,dateColumn,messageColumn,typeColumn] to be set
		/// </summary>
		Database = 64,

		/// <summary>
		/// DataBase | File | EventLog | Console | Debug | Email
		/// </summary>
		All = Database | File | EventLog | Trace | Email
	}
}