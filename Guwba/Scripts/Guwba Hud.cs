using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class GuwbaHud : MonoBehaviour
	{
		private static GuwbaHud _instance;
		private VisualElement _rootElement;
		private Label _lifeText;
		private Label _coinText;
		[SerializeField] private string _rootElementObject;
		[SerializeField] private string _vitalityVisual;
		[SerializeField] private string _vitalityPieceVisual;
		[SerializeField] private string _lifeTextObject;
		[SerializeField] private string _coinTextObject;
		[SerializeField] private ushort _totalWidth;
		[SerializeField] private ushort _vitality;
		internal VisualElement RootElement => this._rootElement;
		internal VisualElement[] Vitality { get; private set; }
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
			this._lifeText = root.Q<Label>(this._lifeTextObject);
			this._coinText = root.Q<Label>(this._coinTextObject);
			VisualElement vitality = root.Q<VisualElement>($"{this._vitalityVisual}");
			vitality.style.width = new StyleLength(new Length(this._totalWidth, LengthUnit.Pixel));
			VisualElement vitalityPiece = root.Q<VisualElement>($"{this._vitalityPieceVisual}");
			this.Vitality = new VisualElement[this._vitality];
			for (ushort i = 0; i < this._vitality; i++)
			{
				VisualElement vitalityPieceClone = new() { name = vitalityPiece.name };
				vitality.Add(vitalityPieceClone);
				vitality[i + 1].style.width = new StyleLength(new Length(this._totalWidth / this._vitality, LengthUnit.Pixel));
				this.Vitality[i] = vitality[i + 1];
			}
			vitality.Remove(vitalityPiece);
		}
	};
};
