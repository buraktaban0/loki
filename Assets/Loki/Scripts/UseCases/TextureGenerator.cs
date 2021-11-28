using System;
using Loki.Runtime.Core;
using Loki.Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Loki.Scripts.UseCases
{
	[ExecuteInEditMode]
	public class TextureGenerator : MonoBehaviour
	{
		public LokiBehaviourGraph Graph;

		public RawImage RawImage;

		private ILokiRunner m_Runner;

		private void OnEnable()
		{
			m_Runner = Graph.GetRunner();
		}

		private void Update()
		{
			m_Runner.Run("Execute");
			var resultTexture = m_Runner.GetData<Texture2D>("Result");
			RawImage.texture = resultTexture;
		}
	}
}
