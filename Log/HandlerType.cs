using System;

namespace Adenson.Log
{
	/// <summary>
	/// Enumeration of log types
	/// </summary>
	public enum HandlerType
	{
		/// <summary>
		/// No logs.
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
		/// Logs via Console.WriteLine .
		/// </summary>
		Console = 2,

		/// <summary>
		/// Logs via System.Diagnostics.Debug.WriteLine.
		/// </summary>
		Debug = 3,

		/// <summary>
		/// Logs via System.Diagnostics.Trace.WriteLine.
		/// </summary>
		Trace = 4,

		/// <summary>
		/// Logs via email (expects config setting adenson/logSettings/emailInfo[from, to] to be set.
		/// </summary>
		Email = 5,

		/// <summary>
		/// Logs into Windows Event Log.
		/// </summary>
		EventLog = 6,

		/// <summary>
		/// Logs into a database (expects config setting adenson/logSettings/databaseInfo[tableName,severityColumn,dateColumn,messageColumn,typeColumn] to be set.
		/// </summary>
		Database = 7,

		/// <summary>
		/// Custom log handler.
		/// </summary>
		Custom = 8
	}
}