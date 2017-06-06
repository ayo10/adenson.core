using System;

namespace Adenson.Data
{
	internal enum ExecuteType
	{
		#if !NETSTANDARD1_6 && !NETSTANDARD1_5 && !NETSTANDARD1_3
		Dataset,
		#endif

		NonQuery,

		Scalar
	}
}