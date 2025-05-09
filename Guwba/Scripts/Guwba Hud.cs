using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class GuwbaHud : MonoBehaviour
	{
		private static GuwbaHud _instance;
		private VisualElement _rootElement;
		private VisualElement[] _vitality;
		private Label _lifeText;
		private Label _coinText;
		[SerializeField] private string _rootElementObject;
		[SerializeField] private string _vitalityVisual;
		[SerializeField] private string _lifeTextObject;
		[SerializeField] private string _coinTextObject;
		internal VisualElement RootElement => this._rootElement;
		internal VisualElement[] Vitality => this._vitality;
		internal Label LifeText => this._lifeText;
		internal Label CoinText => this._coinText;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this._rootElement = root.Q<VisualElement>(this._rootElementObject);
			this._vitality = new VisualElement[4];
			for (ushort i = 1; i <= this._vitality.Length; i++)
				this._vitality[i - 1] = root.Q<VisualElement>($"{this._vitalityVisual}{i}");
			this._lifeText = root.Q<Label>(this._lifeTextObject);
			this._coinText = root.Q<Label>(this._coinTextObject);
		}
	};
};
