using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loki.Runtime.Core
{
	public abstract class Node
	{
		public static class Capacity
		{
			public const int None = 0;
			public const int Single = 1;
			public const int Multiple = 64;
		}

		protected Guid m_Guid = Guid.NewGuid();

		protected abstract int FlowInputCapacity  { get; }
		protected abstract int FlowOutputCapacity { get; }

		protected bool HasFlowInput  => FlowInputCapacity >= Capacity.Single;
		protected bool HasFlowOutput => FlowOutputCapacity >= Capacity.Single;

		protected abstract string Name { get; }

		public abstract void Evaluate();
	}
}
