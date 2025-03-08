using UnityEngine;
using UnityEngine.Rendering;
using Unity.Cinemachine;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(Camera), typeof(CinemachineBrain))]
	[RequireComponent(typeof(SortingGroup), typeof(Rigidbody2D), typeof(BoxCollider2D))]
	internal sealed class ManagerCamera : StateController
	{
		private static ManagerCamera _instance;
		private Camera _cameraGuwba;
		private BoxCollider2D _cameraCollider;
		private Transform[] _childrenTransforms;
		private SpriteRenderer[] _childrenRenderers;
		private Vector2 _startPosition = Vector2.zero;
		[SerializeField] private Transform _backgroundTransform;
		[SerializeField] private Sprite[] _backgroundImages;
		[SerializeField] private float _horizontalBackgroundSpeed, _verticalBackgroundSpeed, _slowHorizontal, _slowVertical;
		private new void Awake()
		{
			if (_instance)
				Destroy(_instance.gameObject);
			_instance = this;
			base.Awake();
			this._cameraGuwba = this.GetComponent<Camera>();
			this._cameraCollider = this.GetComponent<BoxCollider2D>();
			this._childrenTransforms = new Transform[this._backgroundImages.Length]; // Background Movement : Start
			this._childrenRenderers = new SpriteRenderer[this._backgroundImages.Length];
			for (ushort ia = 0; ia < this._backgroundImages.Length; ia++)
			{
				this._childrenTransforms[ia] = Instantiate(this._backgroundTransform, this.transform);
				this._childrenRenderers[ia] = this._childrenTransforms[ia].GetComponent<SpriteRenderer>();
				this._childrenRenderers[ia].sprite = this._backgroundImages[ia];
				this._childrenRenderers[ia].sortingOrder = this._backgroundImages.Length - 1 - ia;
				this._childrenTransforms[ia].GetComponent<SortingGroup>().sortingOrder = this._childrenRenderers[ia].sortingOrder;
				float centerX = this._childrenTransforms[ia].position.x;
				float centerY = this._childrenTransforms[ia].position.y;
				Vector2 imageSize = this._childrenRenderers[ia].bounds.size;
				float right = centerX + imageSize.x;
				float left = centerX - imageSize.x;
				float top = centerY + imageSize.y;
				float bottom = centerY - imageSize.y;
				this._childrenTransforms[ia].GetChild(0).position = new Vector2(left, top);
				this._childrenTransforms[ia].GetChild(1).position = new Vector2(centerX, top);
				this._childrenTransforms[ia].GetChild(2).position = new Vector2(right, top);
				this._childrenTransforms[ia].GetChild(3).position = new Vector2(left, centerY);
				this._childrenTransforms[ia].GetChild(4).position = new Vector2(right, centerY);
				this._childrenTransforms[ia].GetChild(5).position = new Vector2(left, bottom);
				this._childrenTransforms[ia].GetChild(6).position = new Vector2(centerX, bottom);
				this._childrenTransforms[ia].GetChild(7).position = new Vector2(right, bottom);
				for (ushort ib = 0; ib < this._childrenTransforms[ia].childCount; ib++)
					this._childrenTransforms[ia].GetChild(ib).GetComponent<SpriteRenderer>().sprite = this._backgroundImages[ia];
			} // Background Movement : End
			float sizeY = this._cameraGuwba.orthographicSize * 2f;
			float sizeX = sizeY * this._cameraGuwba.aspect;
			this._cameraCollider.size = new Vector2(sizeX, sizeY);
			foreach (Collider2D objects in Physics2D.OverlapBoxAll(this.transform.position, this._cameraCollider.size, 0f))
				this.SetOtherChildren(objects.gameObject, true);
		}
		private void SetOtherChildren(GameObject gameObject, bool activeValue)
		{
			if (gameObject.TryGetComponent<HiddenCamera>(out var hiddenCamera))
				for (ushort i = 0; i < hiddenCamera.transform.childCount; i++)
					hiddenCamera.transform.GetChild(i).gameObject.SetActive(activeValue);
		}
		private void FixedUpdate() // Background Movement
		{
			for (ushort i = 0; i < this._backgroundImages.Length; i++)
			{
				float axisX = 1f - (this._horizontalBackgroundSpeed - (i * this._slowHorizontal));
				float axisY = 1f - (this._verticalBackgroundSpeed - (i * this._slowVertical));
				float movementAxisX = this.transform.position.x * axisX;
				float movementAxisY = this.transform.position.y * axisY;
				this._childrenTransforms[i].position = new Vector2(this._startPosition.x + movementAxisX, this._startPosition.y + movementAxisY);
				Vector2 imageSize = this._childrenRenderers[i].bounds.size;
				float distanceAxisX = this.transform.position.x * (1f - axisX);
				float distanceAxisY = this.transform.position.y * (1f - axisY);
				if (distanceAxisX > this._startPosition.x + imageSize.x)
					this._startPosition = new Vector2(this._startPosition.x + imageSize.x, this._startPosition.y);
				else if (distanceAxisX < this._startPosition.x - imageSize.x)
					this._startPosition = new Vector2(this._startPosition.x - imageSize.x, this._startPosition.y);
				if (distanceAxisY > this._startPosition.y + imageSize.y)
					this._startPosition = new Vector2(this._startPosition.x, this._startPosition.y + imageSize.y);
				else if (distanceAxisY < this._startPosition.y - imageSize.y)
					this._startPosition = new Vector2(this._startPosition.x, this._startPosition.y - imageSize.y);
			}
		}
		private void OnTriggerEnter2D(Collider2D other) => this.SetOtherChildren(other.gameObject, true);
		private void OnTriggerExit2D(Collider2D other) => this.SetOtherChildren(other.gameObject, false);
	};
};
