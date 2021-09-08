﻿using Loki.Runtime.Attributes;
using Loki.Runtime.Core;
using UnityEngine;

namespace Loki.Tests
{
	[NodeMeta("Tests/Test Node 2")]
	public class TestNode2 : Node
	{
		protected override int    FlowInputCapacity  => Capacity.None;
		protected override int    FlowOutputCapacity => Capacity.Single;
		protected override string Name               => "Test Node 2";

		[Output("My Output")]
		public float MyOutput;
		
		
		public override void Evaluate()
		{
			Debug.Log($"Output: {MyOutput}" );
		}
	}
}