using UnityEngine;
namespace GuwbaPrimeAdventure.Item
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(BoxCollider2D))]
	internal sealed class ExtraLife : StateController, ICollectable
	{
		[SerializeField] private bool _saveOnSpecifics;
		private new void Awake()
		{
			base.Awake();
			if (SaveFileData.LifesAcquired.Contains(this.gameObject.name))
				Destroy(this.gameObject, 0.001f);
		}
		public void Collect()
		{
			if (SaveFileData.Lifes < 99f)
				SaveFileData.Lifes += 1;
			SaveFileData.LifesAcquired.Add(this.gameObject.name);
			if (this._saveOnSpecifics)
				SaveFileData.GeneralObjects.Add(this.gameObject.name);
			Destroy(this.gameObject);
		}
	};
};