using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.OffEnviroment
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class StorySceneHud : MonoBehaviour
	{
		private static StorySceneHud _instance;
		[SerializeField] private string _sceneImageVisual;
		internal VisualElement SceneImage { get; private set; }
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this.SceneImage = root.Q<VisualElement>(this._sceneImageVisual);
		}
	};
};