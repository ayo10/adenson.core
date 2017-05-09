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
		private string folder;
		private StreamWriter writer;
		private DateTime fileDate = DateTime.Now;
		#endregion
		#region Constructor
		
		/// <summary>
		/// Initializes a new instance of the <see cref="FileHandler"/> class with the specified file name.
		/// </summary>
		/// <param name="filePath">The path to write the log.</param>
		public FileHandler(string filePath) : base()
		{
			Arg.IsNotEmpty(filePath, "filePath");
			if (!Path.IsPathRooted(filePath))
			{
				filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath.Replace("/", "\\"));
			}

			folder = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
				Trace.WriteLine(StringUtil.Format("Folder '{0}' did not exist, created.", folder));
			}

			if (Directory.Exists(folder))
			{
				this.FilePath = filePath;
			}
		}

		#endregion
		#region Properties

		/// <summary>
		/// Gets the full file name of the file into which the log will be written into.
		/// </summary>
		public string FilePath
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
			if (String.IsNullOrEmpty(this.FilePath))
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

					string fileName = Path.GetFileNameWithoutExtension(this.FilePath);
					string extension = Path.GetExtension(this.FilePath);
					string oldNewFileName = String.Concat(fileName, "-", fileDate.ToString("yyyyMMdd", CultureInfo.CurrentCulture), extension);
					string oldNewFilePath = Path.Combine(folder, oldNewFileName);
					if (!File.Exists(oldNewFilePath))
					{
						File.Move(fileName, oldNewFilePath);
					}
				}

				writer = new StreamWriter(this.FilePath, true);
			}

			writer.WriteLine(this.Formatter.Format(entry));
			writer.Flush();

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