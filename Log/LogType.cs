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
		/// Logs into a database
		/// </summary>
		Database = 1,
		/// <summary>
		/// Logs into a physical file.
		/// </summary>
		/// <remarks>
		/// <para>Files are written to the directory that contains the executing assembly, except for ASP.NET projects, which are written in the directory above the bin folder.</para>
		/// <para>The user application is running under should have file create, read and write access to the specified location stated above</para>
		/// <para>If <see cref="Adenson.Configuration.LoggerSettings.FileName"/> is prepended with a directory name, the directory should exit before hand</para>
		/// </remarks>
		File = 2,
		/// <summary>
		/// Logs into Windows Event Log
		/// </summary>
		EventLog = 4,
		/// <summary>
		/// Spit logs via Console.WriteLine 
		/// </summary>
		Console = 8,
		/// <summary>
		/// Logs via System.Diagnostics.Debug.WriteLine
		/// </summary>
		Debug = 16,
		/// <summary>
		/// DataBase | File | EventLog | Console | DiagnosticsDebug
		/// </summary>
		All = Database | File | EventLog | Debug
	}
}