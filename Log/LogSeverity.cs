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
		/// Only messages called with <see cref="Debug"/>, <see cref="Warn"/> and <see cref="Error"/> are logged
		/// </summary>
		Debug = 1,

		/// <summary>
		/// Only messages called with Info, <see cref="Debug"/> and <see cref="Error"/> are logged (in this case, all messages)
		/// </summary>
		Info = 2,

		/// <summary>
		/// Only messages called with <see cref="Profile"/>, <see cref="Warn"/> and <see cref="Error"/> are logged
		/// </summary>
		Profile = 3,

		/// <summary>
		/// Only messages called with <see cref="Warn"/> and <see cref="Error"/> are logged
		/// </summary>
		Warn = 4,

		/// <summary>
		/// Only messages called with <see cref="Error"/> are logged
		/// </summary>
		Error = 5,
	}
}