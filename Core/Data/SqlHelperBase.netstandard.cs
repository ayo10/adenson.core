using System;
using System.Diagnostics;

namespace Adenson.Data
{
	/// <summary>
	/// Sql Helper base class
	/// </summary>
	public abstract partial class SqlHelperBase : IDisposable
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlHelperBase"/> class using specified connection string setting object.
		/// </summary>
		/// <param name="connectionKeyOrString">Either the connection key or the connection string to use.</param>
		/// <exception cref="ArgumentException">If specified connection string is invalid</exception>
		protected SqlHelperBase(string connectionKeyOrString)
		{
			this.ConnectionString = Arg.IsNotEmpty(connectionKeyOrString);
			this.CloseConnection = true;
		}

		#endregion
	}
}