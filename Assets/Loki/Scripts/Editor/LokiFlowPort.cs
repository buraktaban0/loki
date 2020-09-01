using System;
using UnityEditor.Experimental.GraphView;

namespace Loki.Editor
{
	public class LokiFlowPort : LokiPort
	{
		public LokiFlowPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) :
			base(portOrientation, portDirection, portCapacity, type)
		{
		}
		
		
	}
}
