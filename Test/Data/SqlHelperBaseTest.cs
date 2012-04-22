using Adenson.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;

namespace Adenson.CoreTest.Data
{
	[TestClass]
	public abstract class SqlHelperBaseTest<T> where T : SqlHelperBase
	{
		#region Variables
		protected SqlHelperBase target;
		#endregion
		#region Tests

		[TestMethod]
		public void CloseConnectionTest()
		{
			target.CloseConnection();
		}

		[TestMethod]
		public void ColumnExistsTest()
		{
			string tableName = string.Empty; // TODO: Initialize to an appropriate value
			string columnName = string.Empty; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = target.ColumnExists(tableName, columnName);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void CreateAdapterTest()
		{
			IDbCommand command = null; // TODO: Initialize to an appropriate value
			IDbDataAdapter expected = null; // TODO: Initialize to an appropriate value
			IDbDataAdapter actual;
			actual = target.CreateAdapter(command);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void CreateCommandTest()
		{
			IDbCommand expected = null; // TODO: Initialize to an appropriate value
			IDbCommand actual;
			actual = target.CreateCommand();
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void CreateConnectionTest()
		{
			IDbConnection expected = null; // TODO: Initialize to an appropriate value
			IDbConnection actual;
			actual = target.CreateConnection();
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public virtual void CreateExistDropDatabaseTest()
		{
			var target = this.CreateTarget("Data Source=(local);Initial Catalog=TEST;Integrated Security=True;MultipleActiveResultSets=true;");
			Assert.IsFalse(target.DatabaseExists());
			target.CreateDatabase();
			Assert.IsTrue(target.DatabaseExists());
			target.DropDatabase();
			Assert.IsFalse(target.DatabaseExists());
		}

		[TestMethod]
		public void CreateParameterTest()
		{
			string name = string.Empty; // TODO: Initialize to an appropriate value
			object value = null; // TODO: Initialize to an appropriate value
			IDataParameter expected = null; // TODO: Initialize to an appropriate value
			IDataParameter actual;
			actual = target.CreateParameter(name, value);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void CreateParameterTest1()
		{
			IDbDataParameter expected = null; // TODO: Initialize to an appropriate value
			IDbDataParameter actual;
			actual = target.CreateParameter();
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void DisposeTest()
		{
			target.Dispose();
		}

		[TestMethod]
		public void ExecuteDataSetTest()
		{
			CommandType type = new CommandType(); // TODO: Initialize to an appropriate value
			string commandText = string.Empty; // TODO: Initialize to an appropriate value
			object[] parameterValues = null; // TODO: Initialize to an appropriate value
			DataSet expected = null; // TODO: Initialize to an appropriate value
			DataSet actual;
			actual = target.ExecuteDataSet(type, commandText, parameterValues);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ExecuteDataSetTest1()
		{
			IDbCommand command = null; // TODO: Initialize to an appropriate value
			DataSet expected = null; // TODO: Initialize to an appropriate value
			DataSet actual;
			actual = target.ExecuteDataSet(command);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ExecuteDataSetTest2()
		{
			CommandType type = new CommandType(); // TODO: Initialize to an appropriate value
			IDbTransaction transaction = null; // TODO: Initialize to an appropriate value
			string commandText = string.Empty; // TODO: Initialize to an appropriate value
			object[] parameterValues = null; // TODO: Initialize to an appropriate value
			DataSet expected = null; // TODO: Initialize to an appropriate value
			DataSet actual;
			actual = target.ExecuteDataSet(type, transaction, commandText, parameterValues);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ExecuteDataSetTest3()
		{
			string[] commandTexts = null; // TODO: Initialize to an appropriate value
			DataSet[] expected = null; // TODO: Initialize to an appropriate value
			DataSet[] actual;
			actual = target.ExecuteDataSet(commandTexts);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ExecuteDataSetTest4()
		{
			IDbCommand[] commands = null; // TODO: Initialize to an appropriate value
			DataSet[] expected = null; // TODO: Initialize to an appropriate value
			DataSet[] actual;
			actual = target.ExecuteDataSet(commands);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ExecuteNonQueryTest()
		{
			CommandType type = new CommandType(); // TODO: Initialize to an appropriate value
			string commandText = string.Empty; // TODO: Initialize to an appropriate value
			object[] parameterValues = null; // TODO: Initialize to an appropriate value
			int expected = 0; // TODO: Initialize to an appropriate value
			int actual;
			actual = target.ExecuteNonQuery(type, commandText, parameterValues);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ExecuteNonQueryTest1()
		{
			IDbCommand[] commands = null; // TODO: Initialize to an appropriate value
			int[] expected = null; // TODO: Initialize to an appropriate value
			int[] actual;
			actual = target.ExecuteNonQuery(commands);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ExecuteNonQueryTest2()
		{
			IDbCommand command = null; // TODO: Initialize to an appropriate value
			int expected = 0; // TODO: Initialize to an appropriate value
			int actual;
			actual = target.ExecuteNonQuery(command);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ExecuteNonQueryTest3()
		{
			CommandType type = new CommandType(); // TODO: Initialize to an appropriate value
			IDbTransaction transaction = null; // TODO: Initialize to an appropriate value
			string commandText = string.Empty; // TODO: Initialize to an appropriate value
			object[] parameterValues = null; // TODO: Initialize to an appropriate value
			int expected = 0; // TODO: Initialize to an appropriate value
			int actual;
			actual = target.ExecuteNonQuery(type, transaction, commandText, parameterValues);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ExecuteNonQueryTest4()
		{
			string[] commandTexts = null; // TODO: Initialize to an appropriate value
			int[] expected = null; // TODO: Initialize to an appropriate value
			int[] actual;
			actual = target.ExecuteNonQuery(commandTexts);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ExecuteReaderTest()
		{
			CommandType type = new CommandType(); // TODO: Initialize to an appropriate value
			IDbTransaction transaction = null; // TODO: Initialize to an appropriate value
			string commandText = string.Empty; // TODO: Initialize to an appropriate value
			object[] parameterValues = null; // TODO: Initialize to an appropriate value
			IDataReader expected = null; // TODO: Initialize to an appropriate value
			IDataReader actual;
			actual = target.ExecuteReader(type, transaction, commandText, parameterValues);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ExecuteReaderTest1()
		{
			IDbCommand command = null; // TODO: Initialize to an appropriate value
			IDataReader expected = null; // TODO: Initialize to an appropriate value
			IDataReader actual;
			actual = target.ExecuteReader(command);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
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

		[TestMethod]
		public void ExecuteScalarTest()
		{
			IDbCommand command = null; // TODO: Initialize to an appropriate value
			object expected = null; // TODO: Initialize to an appropriate value
			object actual;
			actual = target.ExecuteScalar(command);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ExecuteScalarTest1()
		{
			CommandType type = new CommandType(); // TODO: Initialize to an appropriate value
			IDbTransaction transaction = null; // TODO: Initialize to an appropriate value
			string commandText = string.Empty; // TODO: Initialize to an appropriate value
			object[] parameterValues = null; // TODO: Initialize to an appropriate value
			object expected = null; // TODO: Initialize to an appropriate value
			object actual;
			actual = target.ExecuteScalar(type, transaction, commandText, parameterValues);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void ExecuteScalarTest2()
		{
			IDbCommand[] commands = null; // TODO: Initialize to an appropriate value
			object[] expected = null; // TODO: Initialize to an appropriate value
			object[] actual;
			actual = target.ExecuteScalar(commands);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
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

		[TestMethod]
		public void ExecuteScalarTest4()
		{
			string[] commandTexts = null; // TODO: Initialize to an appropriate value
			object[] expected = null; // TODO: Initialize to an appropriate value
			object[] actual;
			actual = target.ExecuteScalar(commandTexts);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void OpenConnectionTest()
		{
			IDbConnection expected = null; // TODO: Initialize to an appropriate value
			IDbConnection actual;
			actual = target.OpenConnection();
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void TableExistsTest()
		{
			string tableName = string.Empty; // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			actual = target.TableExists(tableName);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void CurrentConnectionTest()
		{
			IDbConnection actual;
			actual = target.CurrentConnection;
		}

		[TestMethod]
		public void UseTransactionAlwaysTest()
		{
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			target.UseTransactionAlways = expected;
			actual = target.UseTransactionAlways;
			Assert.AreEqual(expected, actual);
		}

		#endregion
		#region Helpers

		protected abstract T CreateTarget(string connectionString);

		#endregion
	}
}