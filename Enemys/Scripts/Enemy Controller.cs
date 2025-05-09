using UnityEngine;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Enemy
{
	[RequireComponent(typeof(Transform), typeof(SpriteRenderer), typeof(Animator)), RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
	internal abstract class EnemyController : StateController, IGrabtable, IDamageable
    {
		protected SpriteRenderer _spriteRenderer;
		protected Animator _animator;
		protected Rigidbody2D _rigidybody;
		protected Collider2D _collider;
		private Vector2 _guardVelocity = new();
		private float _guardGravityScale = 0f;
		protected short _movementSide = 1;
		private bool _paralyzed = false;
		[Header("Enemy Controller"), SerializeField] protected LayerMask _groundLayer;
		[SerializeField] protected LayerMask _targetLayerMask;
		[SerializeField] private short _vitality;
		[SerializeField] protected ushort _movementSpeed;
		[SerializeField] private ushort _damage;
		[SerializeField] protected bool _stopMovement;
		[SerializeField] private bool _invertMovementSide;
		[SerializeField] private bool _noDamage;
		[SerializeField] private bool _noContactDamage;
		[SerializeField] private bool _saveObject;
		protected bool Paralyzed => this._paralyzed;
		public ushort Health => (ushort)this._vitality;
		protected new void Awake()
		{
			base.Awake();
			this._spriteRenderer = this.GetComponent<SpriteRenderer>();
			this._animator = this.GetComponent<Animator>();
			this._rigidybody = this.GetComponent<Rigidbody2D>();
			this._collider = this.GetComponent<Collider2D>();
			this._guardGravityScale = this._rigidybody.gravityScale;
			this._movementSide = (short)(this._invertMovementSide ? -1 : 1);
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			SaveController.Load(out SaveFile saveFile);
			if (this._saveObject && !saveFile.generalObjects.Contains(this.gameObject.name))
			{
				saveFile.generalObjects.Add(this.gameObject.name);
				SaveController.WriteSave(saveFile);
			}
		}
		private void OnEnable()
		{
			this._animator.enabled = true;
			this._rigidybody.gravityScale = this._guardGravityScale;
			this._rigidybody.linearVelocity = this._guardVelocity;
		}
		private void OnDisable()
		{
			this._animator.enabled = false;
			this._guardVelocity = this._rigidybody.linearVelocity;
			this._rigidybody.linearVelocity = Vector2.zero;
		}
		private void OnTrigger(GameObject collisionObject)
		{
			if (!this._paralyzed && !this._noContactDamage && collisionObject.TryGetComponent<IDamageable>(out var damageable))
				damageable.Damage(this._damage);
		}
		private void OnTriggerEnter2D(Collider2D other) => this.OnTrigger(other.gameObject);
		private void OnTriggerStay2D(Collider2D other) => this.OnTrigger(other.gameObject);
		public void Paralyze(bool value) => this._paralyzed = value;
		public bool Damage(ushort damage)
		{
			if (this._noDamage || this._paralyzed)
				return false;
			if ((this._vitality -= (short)damage) <= 0)
				Destroy(this.gameObject);
			return true;
		}
	};
};
