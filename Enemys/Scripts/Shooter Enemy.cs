using UnityEngine;
using GuwbaPrimeAdventure.Connection;
namespace GuwbaPrimeAdventure.Enemy
{
	[DisallowMultipleComponent]
	internal sealed class ShooterEnemy : EnemyController
	{
		private Vector2 _targetDirection;
		private float _shootInterval = 0f;
		private float _timeStop = 0f;
		private float _gravityScale = 0f;
		private bool _isStopped = false;
		[Header("Shooter Enemy"), SerializeField] private Projectile[] _projectiles;
		[SerializeField] private float _perceptionDistance;
		[SerializeField] private float _rayAngleDirection;
		[SerializeField] private float _intervalToShoot;
		[SerializeField] private float _stopTime;
		[SerializeField] private bool _stop;
		[SerializeField] private bool _paralyze;
		[SerializeField] private bool _returnGravity;
		[SerializeField] private bool _circulateDetection;
		[SerializeField] private bool _shootInfinity;
		[SerializeField] private bool _pureInstance;
		[SerializeField] private bool _instanceOnSelf;
		private new void Awake()
		{
			base.Awake();
			this._gravityScale = this._rigidybody.gravityScale;
		}
		private bool AimPerception()
		{
			float originDirection = this._collider.bounds.extents.x * this._movementSide;
			Vector2 origin = new(this.transform.position.x + originDirection, this.transform.position.y);
			Vector2 direction = Quaternion.AngleAxis(this._rayAngleDirection, Vector3.forward) * Vector2.up;
			if (this._circulateDetection)
			{
				Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, this._perceptionDistance, this._targetLayerMask);
				foreach (Collider2D collider in colliders)
					if (collider.TryGetComponent<IDamageable>(out _))
					{
						this._targetDirection = collider.transform.position - this.transform.position;
						return true;
					}
			}
			else
				foreach (RaycastHit2D ray in Physics2D.RaycastAll(origin, direction, this._perceptionDistance, this._targetLayerMask))
					if (ray.collider.TryGetComponent<IDamageable>(out _))
						return true;
			return false;
		}
		private void FixedUpdate()
		{
			if (this._stopMovement || this.Paralyzed)
				return;
			if (this._shootInterval > 0f)
				this._shootInterval -= Time.deltaTime;
			if (this._timeStop > 0f)
				this._timeStop -= Time.deltaTime;
			else if (this._timeStop <= 0f && this._isStopped)
			{
				this._isStopped = false;
				Sender.Create().SetToWhereConnection(PathConnection.Enemy).SetConnectionState(ConnectionState.Enable).SetToggle(true).Send();
				if (this._returnGravity)
					this._rigidybody.gravityScale = this._gravityScale;
			}
			if ((this.AimPerception() || this._shootInfinity) && this._shootInterval <= 0f)
			{
				this._shootInterval = this._intervalToShoot;
				if (this._stop)
				{
					this._timeStop = this._stopTime;
					this._isStopped = true;
					this._rigidybody.linearVelocity = Vector2.zero;
					Sender.Create().SetToWhereConnection(PathConnection.Enemy).SetConnectionState(ConnectionState.Disable).SetToggle(true).Send();
					if (this._paralyze)
						this._rigidybody.gravityScale = 0f;
				}
				foreach (Projectile projectile in this._projectiles)
					if (this._pureInstance)
						Instantiate(projectile, this.transform.position, projectile.transform.rotation, this.transform);
					else
					{
						Vector2 position = this.transform.position;
						float angle = (Mathf.Atan2(this._targetDirection.y, this._targetDirection.x) * Mathf.Rad2Deg) - 90f;
						Quaternion rotation = Quaternion.AngleAxis(this._rayAngleDirection, Vector3.forward);
						if (this._circulateDetection)
							rotation = Quaternion.AngleAxis(angle, Vector3.forward);
						if (!this._instanceOnSelf)
							position += (Vector2)(rotation * Vector2.up);
						Instantiate(projectile, position, rotation, this.transform);
					}
			}
		}
	};
};
