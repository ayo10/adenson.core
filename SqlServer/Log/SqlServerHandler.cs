using Adenson.Data;

namespace Adenson.Log
{
	/// <summary>
	/// Handler for inserting logs into sql server.
	/// </summary>
	public class SqlServerHandler : DatabaseHandler
	{
		/// <inheritdoc />
		protected override SqlHelperBase CreateSqlHelper(string connection)
		{
			return new SqlClientHelper(connection);
		}
	}
}
