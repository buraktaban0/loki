using System;
using UnityEditor.Experimental.GraphView;

namespace Loki.Editor
{
	public class LokiPort : Port
	{
		protected LokiPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) :
			base(portOrientation, portDirection, portCapacity, type)
		{
			
		}
		
		
	}
}
