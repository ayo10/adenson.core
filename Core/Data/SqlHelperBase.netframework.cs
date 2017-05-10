using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
#if !NET35
#endif

namespace Adenson.Data
{
	/// <summary>
	/// Sql Helper base class
	/// </summary>
	public abstract partial class SqlHelperBase : IDisposable
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the SqlHelperBase class using <see cref="Configuration.ConnectionStrings.Default"/>.
		/// </summary>
		protected SqlHelperBase() : this(Configuration.ConnectionStrings.Default)
		{
			this.CloseConnection = true;
		}

		/// <summary>
		/// Initializes a new instance of the SqlHelperBase class using specified connection string setting object.
		/// </summary>
		/// <param name="connectionStringSettings">The connection string settings object to use.</param>
		/// <exception cref="ArgumentNullException">If specified connection string null</exception>
		/// <exception cref="ArgumentException">If specified connection string object has an invalid connection string</exception>
		protected SqlHelperBase(ConnectionStringSettings connectionStringSettings)
		{
			Arg.IsNotNull(connectionStringSettings);
			this.ConnectionString = connectionStringSettings.ConnectionString;
			this.CloseConnection = true;
		}

		/// <summary>
		/// Initializes a new instance of the SqlHelperBase class using specified connection string setting object.
		/// </summary>
		/// <param name="keyOrConnectionString">Either the config connection key or the connection string to use.</param>
		/// <exception cref="ArgumentException">If specified connection string is invalid</exception>
		protected SqlHelperBase(string keyOrConnectionString)
		{
			Arg.IsNotEmpty(keyOrConnectionString);
			var cs = ConfigurationManager.ConnectionStrings[keyOrConnectionString];
			this.ConnectionString = cs == null ? keyOrConnectionString : cs.ConnectionString;
			this.CloseConnection = true;
		}

		#endregion
		#region Methods
		#region Abstract Methods

		/// <summary>
		/// Creates a new data adapter object for use by the helper methods.
		/// </summary>
		/// <param name="command">The command to use to construct the adapter.</param>
		/// <returns>New <see cref="IDbDataAdapter"/> adapter</returns>
		public abstract IDbDataAdapter CreateAdapter(IDbCommand command);

		#endregion
		#region ExecuteDataSet

		/// <summary>
		/// Executes and returns a new DataSet from specified command text using specified transaction.
		/// </summary>
		/// <param name="commandText">The command to execute.</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all).</param>
		/// <returns>A new DataSet object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty</exception>
		/// <exception cref="ArgumentNullException">If <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual DataSet ExecuteDataSet(string commandText, params object[] parameterValues)
		{
			return this.ExecuteDataSet(this.CreateCommand(CommandType.Text, commandText, parameterValues));
		}

		/// <summary>
		/// Executes and returns a new DataSet from specified command text.
		/// </summary>
		/// <param name="type">The command type.</param>
		/// <param name="commandText">The command to execute.</param>
		/// <param name="parameterValues">Zero or more parameter values (could be of tye System.Data.IDataParameter, Adenson.Data.Parameter, any IConvertible object or a combination of all).</param>
		/// <returns>A new DataSet object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="commandText"/> is null or empty, OR, <paramref name="parameterValues"/> is not empty but any item in it is null</exception>
		public virtual DataSet ExecuteDataSet(CommandType type, string commandText, params object[] parameterValues)
		{
			return this.ExecuteDataSet(this.CreateCommand(type, commandText, parameterValues));
		}

		/// <summary>
		/// Executes and returns a new DataSet from specified command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns>A new DataSet object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="command"/> is null</exception>
		[SuppressMessage("Microsoft.Reliability", "CA2000", Justification = "Object being returned.")]
		public virtual DataSet ExecuteDataSet(IDbCommand command)
		{
			Arg.IsNotNull(command, "command");
			this.PrepCommand(command);

			IDbDataAdapter dataAdapter = this.CreateAdapter(command);
			DataSet dataset = new DataSet();
			dataset.Locale = System.Globalization.CultureInfo.CurrentCulture;
			dataAdapter.Fill(dataset);

			IDisposable d = dataAdapter as IDisposable;
			if (d != null)
			{
				d.Dispose();
			}

			return dataset;
		}

		/// <summary>
		/// Executes the commands in a batched mode with a transaction.
		/// </summary>
		/// <param name="commands">1 or more commands.</param>
		/// <returns>The result of each ExecuteDataSet run on each command</returns>
		/// <exception cref="ArgumentException">If <paramref name="commands"/> is empty</exception>
		/// <exception cref="ArgumentNullException">If any of the items in <paramref name="commands"/> is null</exception>
		public virtual DataSet[] ExecuteDataSets(params IDbCommand[] commands)
		{
			return this.Execute<DataSet>(ExecuteType.Dataset, commands);
		}

		/// <summary>
		/// Executes the command texts in a batched mode with a transaction.
		/// </summary>
		/// <param name="commandTexts">1 or more command texts.</param>
		/// <returns>The result of each ExecuteDataSet run on each command text</returns>
		/// <exception cref="ArgumentException">If <paramref name="commandTexts"/> is empty</exception>
		/// <exception cref="ArgumentNullException">If any of the items in <paramref name="commandTexts"/> is null</exception>
		public virtual DataSet[] ExecuteDataSets(params string[] commandTexts)
		{
			return this.Execute<DataSet>(ExecuteType.Dataset, commandTexts);
		}

		#endregion

		#endregion
	}
}