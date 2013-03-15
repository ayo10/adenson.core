using System;

namespace Adenson.Log
{
	/// <summary>
	/// Log Severity Enums
	/// </summary>
	internal enum LogSeverity : int
	{
		/// <summary>
		/// Indicates no log is saved
		/// </summary>
		None = 0,

		/// <summary>
		/// Only messages called with Debug, Warning and Error are logged
		/// </summary>
		Debug = 1,

		/// <summary>
		/// Only messages called with Info, Debug and Error are logged (in this case, all messages)
		/// </summary>
		Info = 2,

		/// <summary>
		/// Only messages called with Profiler, Warning and Error are logged
		/// </summary>
		Profiler = 3,

		/// <summary>
		/// Only messages called with Warning and Error are logged
		/// </summary>
		Warn = 4,

		/// <summary>
		/// Only messages called with Error are logged
		/// </summary>
		Error = 5,
	}
}