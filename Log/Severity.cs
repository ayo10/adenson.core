using System;

namespace Adenson.Log
{
	/// <summary>
	/// Log Severity Enums
	/// </summary>
	public enum Severity : int
	{
		/// <summary>
		/// No messages are logged.
		/// </summary>
		None = 0,

		/// <summary>
		/// All messages are logged.
		/// </summary>
		Debug = 1,

		/// <summary>
		/// Only messages called with <see cref="Profile"/>, <see cref="Warn"/>, <see cref="Error"/> and <see cref="Critical"/> are logged
		/// </summary>
		Profile = 2,

		/// <summary>
		/// Only messages called with <see cref="Warn"/>, <see cref="Warn"/>, <see cref="Error"/> and <see cref="Critical"/> are logged (in this case, all messages)
		/// </summary>
		Info = 3,

		/// <summary>
		/// Only messages called with <see cref="Warn"/>, <see cref="Error"/> and <see cref="Critical"/> are logged
		/// </summary>
		Warn = 4,

		/// <summary>
		/// Only messages called with <see cref="Error"/> and <see cref="Critical"/> are logged
		/// </summary>
		Error = 5,

		/// <summary>
		/// Only messages called with <see cref="Critical"/> are logged
		/// </summary>
		Critical = 6,
	}
}