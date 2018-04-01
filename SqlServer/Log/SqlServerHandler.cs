using Adenson.Data;

namespace Adenson.Log
{
	/// <summary>
	/// Handler for inserting logs into sql server.
	/// </summary>
	public class SqlServerHandler : DatabaseHandler
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlServerHandler"/> class extending <see cref="DatabaseHandler"/> by using an instance of <see cref="SqlServerHelper"/> with connection key 'logger'.
		/// </summary>
		/// <param name="tableName">The database table name.</param>
		/// <param name="dateColumn">The date column.</param>
		/// <param name="typeColumn">The type column.</param>
		/// <param name="severityColumn">The severity column name.</param>
		/// <param name="messageColumn">The message column.</param>
		public SqlServerHandler(string tableName, string dateColumn, string typeColumn, string severityColumn, string messageColumn) : base(new SqlServerHelper("logger"), tableName, dateColumn, typeColumn, severityColumn, messageColumn)
		{
		}
	}
}