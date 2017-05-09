using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
#if !NET35
using System.Dynamic;
#endif
using System.IO;
using System.Linq;
using Adenson.Log;

namespace Adenson.Data
{
	/// <summary>
	/// Sql Helper base class
	/// </summary>
	public abstract partial class SqlHelperBase : IDisposable
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the SqlHelperBase class using specified connection string setting object.
		/// </summary>
		/// <param name="keyOrConnectionString">Either the config connection key or the connection string to use.</param>
		/// <exception cref="ArgumentException">If specified connection string is invalid</exception>
		protected SqlHelperBase(string connectionString)
		{
			this.ConnectionString = Arg.IsNotEmpty(connectionString);
			this.CloseConnection = true;
		}

		#endregion
	}
}