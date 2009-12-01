using System;
using System.Reflection;

namespace Adenson.Log
{
	/// <summary>
	/// Represents how to show a message box in applications that are configured to show one. 
	/// </summary>
	/// <remarks>Expecting to use System.Windows.Forms.MessageBox.Show or Adenson.Web.UI.MessageBox.Show, or any implementation that is structured thusly.</remarks>
	public class ErrorAlertType
	{
		#region Variables
		private string _assemblyName, _typeName, _showMethodName;
		private MethodInfo messageBoxMethod;
		#endregion
		#region Properties

		/// <summary>
		/// Gets the assembly name where our message box lives in 
		/// </summary>
		public string AssemblyName
		{
			get { return _assemblyName; }
			private set { _assemblyName = value; }
		}
		/// <summary>
		/// Gets the type name where our message box lives in 
		/// </summary>
		public string TypeName
		{
			get { return _typeName; }
			private set { _typeName = value; }
		}
		/// <summary>
		/// Gets the method name for show, if null, returns Show
		/// </summary>
		public string ShowMethodName
		{
			get { return string.IsNullOrEmpty(_showMethodName) ? "Show" : _showMethodName; }
			private set { _showMethodName = value; }
		}

		#endregion
		#region Methods

		internal void Initialize()
		{
			if (messageBoxMethod != null) return;

			Assembly assembly = Assembly.Load(this.AssemblyName);
			messageBoxMethod = assembly.GetType(this.TypeName).GetMethod(this.ShowMethodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);
			try
			{
			}
			catch { }
			if (messageBoxMethod != null)
			{
				try
				{
				}
				catch { }
			}
		}
		internal void Show(string message)
		{
			if (messageBoxMethod == null) this.Initialize();
			messageBoxMethod.Invoke(null, new object[] { message });
		}

		/// <summary>
		/// Attempts to create a new ErrorAlertType from 
		/// </summary>
		/// <param name="alertTypeAsStr"></param>
		/// <returns></returns>
		public static ErrorAlertType Parse(string alertTypeAsStr)
		{
			if (string.IsNullOrEmpty(alertTypeAsStr)) throw new ArgumentNullException("alertTypeAsStr", ExceptionMessages.ArgumentNullOrEmpty);
			string[] splits = alertTypeAsStr.Split(',');
			if (splits.Length < 2) throw new ArgumentOutOfRangeException("alertTypeAsStr", "The format of the error alert type must be in the form '[Assembly Full Name], [Type Full Name], [Static Show Method Name]'");
			
			ErrorAlertType alertType = new ErrorAlertType();
			alertType.AssemblyName = splits[0].Trim();
			alertType.TypeName = splits[1].Trim();
			if (splits.Length == 3) alertType.ShowMethodName = splits[2].Trim();
			alertType.Initialize();
			return alertType;
		}

		#endregion
	}
}