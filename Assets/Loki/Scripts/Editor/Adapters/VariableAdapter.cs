using System.Reflection;
using Loki.Editor.Utility;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Loki.Editor.Adapters
{
	public class VariableAdapter : LokiNodeAdapter
	{
		public FieldInfo field;

		public VariableAdapter(FieldInfo field)
		{
			this.field = field;
		}

		public override void BuildNodeView(LokiNodeView view)
		{
			view.titleLabel.text = field.Name;

			view.midContainer.RemoveFromHierarchy();

			var setPort =
				view.headerContainer.InsertPort(0, Orientation.Horizontal, Direction.Input, Capacity.Single,
				                                field.Name);
			var getPort =
				view.headerContainer.AddPort(Orientation.Horizontal, Direction.Output, Capacity.Multi, field.Name);

			setPort.tooltip = $"Set {field.Name}";
			getPort.tooltip = $"Get {field.Name}";
		}
	}
}
