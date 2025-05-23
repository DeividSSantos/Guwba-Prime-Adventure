using UnityEngine;
using UnityEngine.UIElements;
namespace GuwbaPrimeAdventure.Hud
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(UIDocument))]
	internal sealed class MenuHud : MonoBehaviour
	{
		private static MenuHud _instance;
		private GroupBox _buttons;
		private GroupBox _saves;
		private Button _play;
		private Button _configurations;
		private Button _quit;
		private Button _back;
		private TextField[] _saveName;
		private Button[] _load;
		private Button[] _delete;
		[SerializeField] private string _buttonsGroup;
		[SerializeField] private string _savesGroup;
		[SerializeField] private string _playButton;
		[SerializeField] private string _configurationsButton;
		[SerializeField] private string _quitButton;
		[SerializeField] private string _backButton;
		[SerializeField] private string _saveNameTextField;
		[SerializeField] private string _loadButton;
		[SerializeField] private string _deleteButton;
		public GroupBox Buttons => this._buttons;
		public GroupBox Saves => this._saves;
		public Button Play => this._play;
		public Button Configurations => this._configurations;
		public Button Quit => this._quit;
		public Button Back => this._back;
		public TextField[] SaveName => this._saveName;
		public Button[] Load => this._load;
		public Button[] Delete => this._delete;
		private void Awake()
		{
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			VisualElement root = this.GetComponent<UIDocument>().rootVisualElement;
			this._buttons = root.Q<GroupBox>(this._buttonsGroup);
			this._play = root.Q<Button>(this._playButton);
			this._configurations = root.Q<Button>(this._configurationsButton);
			this._quit = root.Q<Button>(this._quitButton);
			this._saves = root.Q<GroupBox>(this._savesGroup);
			this._back = root.Q<Button>(this._backButton);
			this._saveName = new TextField[4];
			for (ushort i = 0; i < this._saveName.Length; i++)
				this._saveName[i] = root.Q<TextField>(this._saveNameTextField + $"{i + 1f}");
			this._load = new Button[4];
			for (ushort i = 0; i < this._load.Length; i++)
				this._load[i] = root.Q<Button>(this._loadButton + $"{i + 1f}");
			this._delete = new Button[4];
			for (ushort i = 0; i < this._delete.Length; i++)
				this._delete[i] = root.Q<Button>(this._deleteButton + $"{i + 1f}");
		}
	};
};
