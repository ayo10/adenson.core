using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Adenson.Log
{
	/// <summary>
	/// Sends logs to a file (rotates on a daily basis).
	/// </summary>
	public sealed class FileHandler : BaseHandler, IDisposable
	{
		#region Variables
		private bool allowWrite = true;
		private string folder;
		private StreamWriter writer;
		private DateTime fileDate = DateTime.Now;
		#endregion
		#region Constructor

		internal FileHandler(Configuration.HandlerElement element) : base()
		{
			this.FileName = element.GetValue("fileName", "eventlogger.log");
			string filePath = this.FileName;
			if (!Path.IsPathRooted(filePath))
			{
				filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath.Replace("/", "\\"));
			}

			folder = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(folder))
			{
				allowWrite = false;
				Trace.WriteLine(StringUtil.Format("Adenson.Log.Logger: ERROR: Folder {0} does not exist, file logging will not happen", folder));
			}
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the file name used to log.
		/// </summary>
		public string FileName
		{ 
			get; 
			private set; 
		}

		#endregion
		#region Methods

		/// <summary>
		/// Writes the log to a log file.
		/// </summary>
		/// <param name="entry">The entry to write.</param>
		/// <returns>True if the log was written successfully, false otherwise.</returns>
		public override bool Write(LogEntry entry)
		{
			if (!allowWrite)
			{
				return false;
			}

			int numDays = (int)DateTime.Now.Subtract(fileDate).TotalDays;
			if (writer == null || numDays > 1)
			{
				if (numDays > 1)
				{
					writer.Flush();
					writer.Close();

					string fileName = Path.GetFileNameWithoutExtension(this.FileName);
					string extension = Path.GetExtension(this.FileName);
					string oldNewFileName = String.Concat(fileName, "-", fileDate.ToString("yyyyMMdd", CultureInfo.CurrentCulture), extension);
					string oldNewFilePath = Path.Combine(folder, oldNewFileName);
					if (!File.Exists(oldNewFilePath))
					{
						File.Move(fileName, oldNewFilePath);
					}
				}

				writer = new StreamWriter(this.FileName);
			}

			writer.WriteLine(this.Formatter.Format(entry));
			return true;
		}

		/// <summary>
		/// Disposes the file writer.
		/// </summary>
		public void Dispose()
		{
			if (writer != null)
			{
				writer.Dispose();
			}
		}

		#endregion
	}
}