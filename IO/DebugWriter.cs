using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Adenson.IO
{
	public sealed class DebugWriter : TextWriter
	{
		#region Variables
		private UnicodeEncoding _encoding;
		#endregion
		#region Properties

		public override Encoding Encoding
		{
			get
			{
				if (_encoding == null) _encoding = new UnicodeEncoding(false, false);
				return _encoding;
			}
		}

		#endregion
		#region Methods

		public override void Write(char[] buffer, int index, int count)
		{
			String str = new String(buffer, index, count);
			if (buffer[0] == '-' && buffer[3] == '@')
			{
				string param = str.Substring(3, str.IndexOf(":") - 3);
				string whole = "(Size = 0; Prec = 0; Scale = 0)";
				str = str.Replace("--", "-- DECLARE");
				str = str.Replace(": Input", "");
				if (str.IndexOf("VarChar") > -1 || str.IndexOf("DateTime") > -1)
				{
					str = str.Replace("[", "= '");
					str = str.Replace("]", "'");
					if (str.IndexOf("VarChar") > -1)
					{
						int i = str.IndexOf("(Size = ");
						string size = str.Substring(i + 8, str.IndexOf("; Prec = ") - (i + 8));
						whole = str.Substring(i, str.IndexOf(")") - i + 1);
						str = str.Replace("VarChar", "VarChar(" + size + ")");
					}
				}
				else
				{
					str = str.Replace("[", " = ");
					str = str.Replace("]", "");
				}
				str = str.Replace(" " + whole, "; SET " + param);
			}
			Debug.Write(str);
		}

		#endregion
	}
}