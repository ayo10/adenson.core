using System;
using System.Data;
using Adenson.Data;
using NUnit.Framework;

namespace Adenson.Tests.Data
{

    public class CustomSqlHelper : SqlHelperBase
	{
		public override IDbDataAdapter CreateAdapter(IDbCommand command)
		{
			throw new NotImplementedException();
		}

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
