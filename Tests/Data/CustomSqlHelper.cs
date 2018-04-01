using System;
using System.Data;
using Adenson.Data;

namespace Adenson.CoreTests.Data
{
	public class CustomSqlHelper : SqlHelperBase
	{
		public override IDbCommand CreateCommand()
		{
			throw new NotImplementedException();
		}

		public override IDbConnection CreateConnection()
		{
			throw new NotImplementedException();
		}

		public override IDbDataParameter CreateParameter()
		{
			throw new NotImplementedException();
		}

		public override bool DatabaseExists()
		{
			throw new NotImplementedException();
		}
	}
}
