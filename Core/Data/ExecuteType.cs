using System;

namespace Adenson.Data
{
	internal enum ExecuteType
	{
		#if !NETSTANDARD1_6
		Dataset,
		#endif
		NonQuery,
		Scalar
	}
}