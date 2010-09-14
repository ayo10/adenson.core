using System;

namespace Adenson.Log
{
	/// <summary>
	/// Enumeration of log types
	/// </summary>
	[Flags]
	public enum LogType
	{
		/// <summary>
		/// No logs
		/// </summary>
		None = 0,
		/// <summary>
		/// Spits logs into a database
		/// </summary>
		DataBase = 1,
		/// <summary>
		/// Spits logs into a physical file
		/// </summary>
		File = 2,
		/// <summary>
		/// Spits logs into Windows Event Log
		/// </summary>
		EventLog = 4,
		/// <summary>
		/// Spit logs via Console.WriteLine 
		/// </summary>
		Console = 8,
		/// <summary>
		/// Spits logs via System.Diagnostics.Debug.WriteLine
		/// </summary>
		Debug = 16,
		/// <summary>
		/// Console | DiagnosticsDebug
		/// </summary>
		ConsoleProjects = Console | Debug,
		/// <summary>
		/// DataBase | DiagnosticsDebug
		/// </summary>
		WebProjects = DataBase | Debug,
		/// <summary>
		/// File | Console | DiagnosticsDebug | EventLog
		/// </summary>
		WinFormProjects = File | Debug | EventLog,
		/// <summary>
		/// DataBase | File | EventLog | Console | DiagnosticsDebug
		/// </summary>
		All = DataBase | File | EventLog | Debug
	}
}