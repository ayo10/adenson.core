using Adenson.Data;
using NUnit.Framework;
using System;
using System.Data;

namespace Adenson.CoreTest.Data
{
	[TestFixture]
	public abstract class SqlHelperBaseTest<T> where T : SqlHelperBase
	{
		#region Variables
		protected SqlHelperBase target;
		protected string connectionString;
		#endregion
		#region Tests

		[Test]
		public void ColumnExistsTest()
		{
			target.ExecuteNonQuery("create table columnexisttest (id int not null, test varchar(20) null)");
			Assert.IsTrue(target.ColumnExists("columnexisttest", "id"));
			Assert.IsTrue(target.ColumnExists("columnexisttest", "test"));
			Assert.IsTrue(target.ColumnExists("columnexisttest", "ID"), "Case is irrelevant");
			Assert.IsTrue(target.ColumnExists("columnexisttest", "TEST"), "Case is irrelevant");
			Assert.IsFalse(target.ColumnExists("columnexisttest", "woot"));
			Assert.IsFalse(target.ColumnExists("woot", "woot"));
			target.ExecuteNonQuery("drop table columnexisttest");
		}

		[Test]
		public void CreateAdapterTest()
		{
			Assert.IsNotNull(target.CreateAdapter(target.CreateCommand()));
		}

		[Test]
		public void CreateCommandTest()
		{
			Assert.IsNotNull(target.CreateCommand());
		}

		[Test]
		public void CreateConnectionTest()
		{
			IDbConnection actual = target.CreateConnection();
			Assert.IsNotNull(actual);
			Assert.AreEqual(ConnectionState.Closed, actual.State);
		}

		[Test]
		public virtual void CreateExistDropDatabaseTest()
		{
			Assert.IsTrue(target.DatabaseExists());
			target.DropDatabase();
			Assert.IsFalse(target.DatabaseExists());
			Assert.DoesNotThrow(delegate { target.DropDatabase(); }, "Should check first to see that the database exists before dropping");
			target.CreateDatabase();
			Assert.IsTrue(target.DatabaseExists());
		}

		[Test]
		public void CreateParameterTest()
		{
			IDbDataParameter actual = target.CreateParameter();
			Assert.IsNotNull(actual);
		}

		[Test]
		public void CreateParameterWithValueTest()
		{
			IDataParameter actual = target.CreateParameter("test", "value");
			Assert.IsNotNull(actual);
			Assert.AreEqual("test", actual.ParameterName);
			Assert.AreEqual("value", actual.Value);
		}

		[Test]
		public void CurrentConnectionTest()
		{
			Assert.IsNotNull(target.Connection);
		}

		[Test]
		public void ExecuteDataSetTypeTextValueTest()
		{
			CommandType type = new CommandType();
			string commandText = string.Empty; // TODO: Initialize to an appropriate value
			object[] parameterValues = null; // TODO: Initialize to an appropriate value
			DataSet expected = null; // TODO: Initialize to an appropriate value
			DataSet actual;
			actual = target.ExecuteDataSet(type, commandText, parameterValues);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteDataSetCommandTest()
		{
			IDbCommand command = null; // TODO: Initialize to an appropriate value
			DataSet expected = null; // TODO: Initialize to an appropriate value
			DataSet actual;
			actual = target.ExecuteDataSet(command);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteDataSetMultipleTextsTest()
		{
			string[] commandTexts = null; // TODO: Initialize to an appropriate value
			DataSet[] expected = null; // TODO: Initialize to an appropriate value
			DataSet[] actual;
			actual = target.ExecuteDataSets(commandTexts);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteDataSetMultipleCommandsTest()
		{
			IDbCommand[] commands = null; // TODO: Initialize to an appropriate value
			DataSet[] expected = null; // TODO: Initialize to an appropriate value
			DataSet[] actual;
			actual = target.ExecuteDataSets(commands);
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
			actual = target.ExecuteNonQuery(type, commandText, parameterValues);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteNonQueryMultipleCommmandsTest()
		{
			IDbCommand[] commands = null; // TODO: Initialize to an appropriate value
			int[] expected = null; // TODO: Initialize to an appropriate value
			int[] actual;
			actual = target.ExecuteNonQueries(commands);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteNonQueryCommandTest()
		{
			IDbCommand command = null; // TODO: Initialize to an appropriate value
			int expected = 0; // TODO: Initialize to an appropriate value
			int actual;
			actual = target.ExecuteNonQuery(command);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteNonQueryMultipleTextsTest()
		{
			string[] commandTexts = null; // TODO: Initialize to an appropriate value
			int[] expected = null; // TODO: Initialize to an appropriate value
			int[] actual;
			actual = target.ExecuteNonQueries(commandTexts);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteReaderTest1()
		{
			IDbCommand command = null; // TODO: Initialize to an appropriate value
			IDataReader expected = null; // TODO: Initialize to an appropriate value
			IDataReader actual;
			actual = target.ExecuteReader(command);
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
			actual = target.ExecuteReader(type, commandText, parameterValues);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteScalarTest()
		{
			IDbCommand command = null; // TODO: Initialize to an appropriate value
			object expected = null; // TODO: Initialize to an appropriate value
			object actual;
			actual = target.ExecuteScalar(command);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteScalarTest2()
		{
			IDbCommand[] commands = null; // TODO: Initialize to an appropriate value
			object[] expected = null; // TODO: Initialize to an appropriate value
			object[] actual;
			actual = target.ExecuteScalars(commands);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteScalarTest3()
		{
			CommandType type = new CommandType(); // TODO: Initialize to an appropriate value
			string commandText = string.Empty; // TODO: Initialize to an appropriate value
			object[] parameterValues = null; // TODO: Initialize to an appropriate value
			object expected = null; // TODO: Initialize to an appropriate value
			object actual;
			actual = target.ExecuteScalar(type, commandText, parameterValues);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ExecuteScalarTest4()
		{
			string[] commandTexts = null; // TODO: Initialize to an appropriate value
			object[] expected = null; // TODO: Initialize to an appropriate value
			object[] actual;
			actual = target.ExecuteScalars(commandTexts);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void TableExistsTest()
		{
			target.ExecuteNonQuery("create table TableExistsTest (id int not null, test varchar(20) null)");
			Assert.IsTrue(target.TableExists("TableExistsTest"));
			Assert.IsTrue(target.TableExists("tableexiststest"), "Case is irrelevant");
			Assert.IsFalse(target.TableExists("woot"));
			target.ExecuteNonQuery("drop table TableExistsTest");
		}

		#endregion
		#region Helpers

		protected abstract T CreateTarget();

		#endregion
	}
}