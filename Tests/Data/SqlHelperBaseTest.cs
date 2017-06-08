using System.Data;
using Adenson.Data;
using NUnit.Framework;

namespace Adenson.CoreTest.Data
{
	[TestFixture]
	public abstract class SqlHelperBaseTest<T> where T : SqlHelperBase
	{
		#region Properties

		protected SqlHelperBase Target { get; set; }

		protected string ConnectionString { get; set; }

		#endregion
		#region Tests

		[Test]
		public void ColumnExistsTest()
		{
			Target.ExecuteNonQuery("create table columnexisttest (id int not null, test varchar(20) null)");
			Assert.IsTrue(Target.ColumnExists("columnexisttest", "id"));
			Assert.IsTrue(Target.ColumnExists("columnexisttest", "test"));
			Assert.IsTrue(Target.ColumnExists("columnexisttest", "ID"), "Case is irrelevant");
			Assert.IsTrue(Target.ColumnExists("columnexisttest", "TEST"), "Case is irrelevant");
			Assert.IsFalse(Target.ColumnExists("columnexisttest", "woot"));
			Assert.IsFalse(Target.ColumnExists("woot", "woot"));
			Target.ExecuteNonQuery("drop table columnexisttest");
		}

		[Test]
		public void CreateAdapterTest()
		{
			Assert.IsNotNull(Target.CreateAdapter(Target.CreateCommand()));
		}

		[Test]
		public void CreateCommandTest()
		{
			Assert.IsNotNull(Target.CreateCommand());
		}

		[Test]
		public void CreateConnectionTest()
		{
			IDbConnection actual = Target.CreateConnection();
			Assert.IsNotNull(actual);
			Assert.AreEqual(ConnectionState.Closed, actual.State);
		}

		[Test]
		public virtual void CreateExistDropDatabaseTest()
		{
			Assert.IsTrue(Target.DatabaseExists());
			Target.DropDatabase();
			Assert.IsFalse(Target.DatabaseExists());
			Assert.DoesNotThrow(() => Target.DropDatabase(), "Should check first to see that the database exists before dropping");
			Target.CreateDatabase();
			Assert.IsTrue(Target.DatabaseExists());
		}

		[Test]
		public void CreateParameterTest()
		{
			IDbDataParameter actual = Target.CreateParameter();
			Assert.IsNotNull(actual);
		}

		[Test]
		public void CreateParameterWithValueTest()
		{
			IDataParameter actual = Target.CreateParameter("test", "value");
			Assert.IsNotNull(actual);
			Assert.AreEqual("test", actual.ParameterName);
			Assert.AreEqual("value", actual.Value);
		}

		[Test]
		public void CurrentConnectionTest()
		{
			Assert.IsNotNull(Target.Connection);
		}

		[Test]
		public void ExecuteDataSetTypeTextValueTest()
		{
			CommandType type = new CommandType();
			string commandText = string.Empty; // TODO: Initialize to an appropriate value
			object[] parameterValues = null; // TODO: Initialize to an appropriate value
			DataSet expected = null; // TODO: Initialize to an appropriate value
			DataSet actual;
			actual = Target.ExecuteDataSet(type, commandText, parameterValues);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteDataSetCommandTest()
		{
			IDbCommand command = null; // TODO: Initialize to an appropriate value
			DataSet expected = null; // TODO: Initialize to an appropriate value
			DataSet actual;
			actual = Target.ExecuteDataSet(command);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteDataSetMultipleTextsTest()
		{
			string[] commandTexts = null; // TODO: Initialize to an appropriate value
			DataSet[] expected = null; // TODO: Initialize to an appropriate value
			DataSet[] actual;
			actual = Target.ExecuteDataSets(commandTexts);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteDataSetMultipleCommandsTest()
		{
			IDbCommand[] commands = null; // TODO: Initialize to an appropriate value
			DataSet[] expected = null; // TODO: Initialize to an appropriate value
			DataSet[] actual;
			actual = Target.ExecuteDataSets(commands);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteNonQueryTypeTextValueTest()
		{
			CommandType type = new CommandType(); // TODO: Initialize to an appropriate value
			string commandText = string.Empty; // TODO: Initialize to an appropriate value
			object[] parameterValues = null; // TODO: Initialize to an appropriate value
			int expected = 0; // TODO: Initialize to an appropriate value
			int actual;
			actual = Target.ExecuteNonQuery(type, commandText, parameterValues);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteNonQueryMultipleCommmandsTest()
		{
			IDbCommand[] commands = null; // TODO: Initialize to an appropriate value
			int[] expected = null; // TODO: Initialize to an appropriate value
			int[] actual;
			actual = Target.ExecuteNonQueries(commands);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteNonQueryCommandTest()
		{
			IDbCommand command = null; // TODO: Initialize to an appropriate value
			int expected = 0; // TODO: Initialize to an appropriate value
			int actual;
			actual = Target.ExecuteNonQuery(command);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteNonQueryMultipleTextsTest()
		{
			string[] commandTexts = null; // TODO: Initialize to an appropriate value
			int[] expected = null; // TODO: Initialize to an appropriate value
			int[] actual;
			actual = Target.ExecuteNonQueries(commandTexts);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteReaderTest1()
		{
			IDbCommand command = null; // TODO: Initialize to an appropriate value
			IDataReader expected = null; // TODO: Initialize to an appropriate value
			IDataReader actual;
			actual = Target.ExecuteReader(command);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteReaderTest2()
		{
			CommandType type = new CommandType(); // TODO: Initialize to an appropriate value
			string commandText = string.Empty; // TODO: Initialize to an appropriate value
			object[] parameterValues = null; // TODO: Initialize to an appropriate value
			IDataReader expected = null; // TODO: Initialize to an appropriate value
			IDataReader actual;
			actual = Target.ExecuteReader(type, commandText, parameterValues);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteScalarTest()
		{
			IDbCommand command = null; // TODO: Initialize to an appropriate value
			object expected = null; // TODO: Initialize to an appropriate value
			object actual;
			actual = Target.ExecuteScalar(command);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteScalarTest2()
		{
			IDbCommand[] commands = null; // TODO: Initialize to an appropriate value
			object[] expected = null; // TODO: Initialize to an appropriate value
			object[] actual;
			actual = Target.ExecuteScalars(commands);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteScalarTest3()
		{
			CommandType type = CommandType.StoredProcedure; // TODO: Initialize to an appropriate value
			string commandText = string.Empty; // TODO: Initialize to an appropriate value
			object[] parameterValues = null; // TODO: Initialize to an appropriate value
			object expected = null; // TODO: Initialize to an appropriate value
			object actual;
			actual = Target.ExecuteScalar(type, commandText, parameterValues);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteScalarTest4()
		{
			string[] commandTexts = null; // TODO: Initialize to an appropriate value
			object[] expected = null; // TODO: Initialize to an appropriate value
			object[] actual;
			actual = Target.ExecuteScalars(commandTexts);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TableExistsTest()
		{
			Target.ExecuteNonQuery("create table TableExistsTest (id int not null, test varchar(20) null)");
			Assert.IsTrue(Target.TableExists("TableExistsTest"));
			Assert.IsTrue(Target.TableExists("tableexiststest"), "Case is irrelevant");
			Assert.IsFalse(Target.TableExists("woot"));
			Target.ExecuteNonQuery("drop table TableExistsTest");
		}

		#endregion
		#region Helpers

		protected abstract T CreateTarget();

		#endregion
	}
}