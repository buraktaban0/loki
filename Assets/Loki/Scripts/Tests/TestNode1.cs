using Loki.Runtime.Attributes;
using Loki.Runtime.Core;
using UnityEngine;

namespace Loki.Tests
{
	[NodeMeta("Tests/Test Node 1")]
	public class TestNode1 : Node
	{
		protected override int    FlowInputCapacity  => Capacity.Single;
		protected override int    FlowOutputCapacity => Capacity.None;
		protected override string Name               => "Test Node 1";

		[Input("My Number")]
		public float MyNumber = 1f;

		public override void Evaluate()
		{
			Debug.Log($"Input: {MyNumber}" );
		}

	}
}
