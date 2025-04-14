using UnityEngine;
using System.Collections;
using GuwbaPrimeAdventure.Connection;
using GuwbaPrimeAdventure.Guwba;
namespace GuwbaPrimeAdventure.Enemy.Boss
{
	[DisallowMultipleComponent]
	internal sealed class JumperBoss : BossController
	{
		private bool _stopJump = false, _jumped = false, _usedHigh = false;
		private ushort _jumpIndex = 0, _fallIndex = 0;
		[Header("Jumper Boss"), SerializeField] private JumpPointStructure[] _jumpPointStructures;
		[SerializeField] private Vector2 _otherTarget;
		[SerializeField] private ushort[] _summonIndexJump, _summonIndexFall;
		[SerializeField] private ushort _followSpeed, _strenghtReact;
		[SerializeField] private bool
			_waitEvent,
			_highReact,
			_stopMoveReact,
			_summonOnJump,
			_bothJump,
			_justHighJump,
			_summonOnFall,
			_bothFall,
			_justHighFall,
			_randomJumpIndex,
			_sequentialJumpIndex,
			_randomFallIndex,
			_sequentialFallIndex,
			_useTarget,
			_randomFollow;
		private void HighJump(Vector2 otherTarget, bool useTarget)
		{
			this.StartCoroutine(FollowTarget());
			IEnumerator FollowTarget()
			{
				yield return new WaitUntil(() => !this.SurfacePerception() && this.enabled);
				this._usedHigh = true;
				if (this._randomJumpIndex && !this._sequentialJumpIndex)
					this._jumpIndex = (ushort)Random.Range(0, this._summonIndexJump.Length - 1);
				if (this._summonOnJump && (this._bothJump || this._justHighJump))
					Sender.Create().SetConnectionObject(ConnectionObject.Boss).SetConnectionState(ConnectionState.Action)
						.SetBossType(BossType.Summoner).SetIndex(this._summonIndexJump[this._jumpIndex]).Send();
				if (this._sequentialJumpIndex && !this._randomJumpIndex)
					this._jumpIndex = (ushort)(this._jumpIndex < this._summonIndexJump.Length - 1f ? this._jumpIndex + 1f : 0f);
				Sender.Create().SetConnectionObject(ConnectionObject.Boss).SetConnectionState(ConnectionState.Action)
					.SetBossType(BossType.Runner).SetToggle(false).Send();
				this._rigidybody.linearVelocityX = 0f;
				float randomDirection = 0f;
				if (this._randomFollow)
					randomDirection = Random.Range(-1f, 1f);
				while (!this.SurfacePerception())
				{
					float targetPosition = GuwbaTransformer<CommandGuwba>.Position.x;
					if (useTarget)
						targetPosition = otherTarget.x;
					if (this._randomFollow)
					{
						if (randomDirection >= 0f)
							targetPosition = GuwbaTransformer<CommandGuwba>.Position.x;
						else if (randomDirection < 0f)
							targetPosition = otherTarget.x;
					}
					float targetDirection = targetPosition - this.transform.position.x;
					this._movementSide = (short)(targetDirection > 0f ? 1f : -1f);
					if (this.enabled)
						this._rigidybody.linearVelocityX = this._movementSide * this._followSpeed;
					else
						this._rigidybody.linearVelocityX = 0f;
					yield return new WaitForFixedUpdate();
					yield return new WaitUntil(() => this.enabled);
				}
			}
		}
		private new void Awake()
		{
			base.Awake();
			for (ushort i = 0; i < this._jumpPointStructures.Length; i++)
			{
				JumpPoint jumpPoint = this._jumpPointStructures[i].JumpPointObject;
				JumpPoint jumpPointInstance = Instantiate(jumpPoint, this._jumpPointStructures[i].Point, Quaternion.identity);
				this._jumpPointStructures[i].RemovalJumpCount = (short)this._jumpPointStructures[i].JumpCount;
				jumpPointInstance.GetTouch(i, (ushort index) =>
				{
					this.StartCoroutine(WaitToHitSurface());
					IEnumerator WaitToHitSurface()
					{
						yield return new WaitUntil(() => this.SurfacePerception() && this.enabled && (!this._waitEvent || this._stopJump));
						if (!this._stopJump)
							if (this._jumpPointStructures[index].RemovalJumpCount-- <= 0f)
							{
								this._jumped = true;
								if (this._randomJumpIndex && !this._sequentialJumpIndex)
									this._jumpIndex = (ushort)Random.Range(0, this._summonIndexJump.Length - 1);
								if (this._summonOnJump && !this._justHighJump)
									Sender.Create().SetConnectionObject(ConnectionObject.Boss).SetConnectionState(ConnectionState.Action)
									.SetBossType(BossType.Summoner).SetIndex(this._summonIndexJump[this._jumpIndex]).Send();
								if (this._sequentialJumpIndex && !this._randomJumpIndex)
									this._jumpIndex = (ushort)(this._jumpIndex < this._summonIndexJump.Length - 1f ? this._jumpIndex + 1f : 0f);
								if (this._jumpPointStructures[index].StopMove)
								{
									Sender.Create().SetConnectionObject(ConnectionObject.Boss).SetConnectionState(ConnectionState.Action)
									.SetBossType(BossType.Runner).SetToggle(false).Send();
									this._rigidybody.linearVelocityX = 0f;
								}
								this._rigidybody.AddForceY(this._jumpPointStructures[index].Strength);
								if (this._jumpPointStructures[index].High)
									this.HighJump(this._jumpPointStructures[index].OtherTarget, this._jumpPointStructures[index].UseTarget);
								this._jumpPointStructures[index].RemovalJumpCount = (short)this._jumpPointStructures[index].JumpCount;
							}
					}
				});
			}
		}
		private new void FixedUpdate()
		{
			if (this._jumped && this.SurfacePerception())
			{
				this._jumped = false;
				if (this._summonOnFall && !this._justHighFall)
				{
					if (this._randomFallIndex && !this._sequentialFallIndex)
						this._fallIndex = (ushort)Random.Range(0, this._summonIndexFall.Length - 1);
					Sender.Create().SetConnectionObject(ConnectionObject.Boss).SetConnectionState(ConnectionState.Action)
						.SetBossType(BossType.Summoner).SetIndex(this._summonIndexFall[this._fallIndex]).Send();
					if (this._sequentialFallIndex && !this._randomFallIndex)
						this._fallIndex = (ushort)(this._fallIndex < this._summonIndexFall.Length - 1f ? this._fallIndex + 1f : 0f);
				}
			}
			if (this._usedHigh && this.SurfacePerception())
			{
				this._usedHigh = false;
				if (this._summonOnFall && (this._bothFall || this._justHighFall))
				{
					if (this._randomFallIndex && !this._sequentialFallIndex)
						this._fallIndex = (ushort)Random.Range(0, this._summonIndexFall.Length - 1);
					Sender.Create().SetConnectionObject(ConnectionObject.Boss).SetConnectionState(ConnectionState.Action)
						.SetBossType(BossType.Summoner).SetIndex(this._summonIndexFall[this._fallIndex]).Send();
					if (this._sequentialFallIndex && !this._randomFallIndex)
						this._fallIndex = (ushort)(this._fallIndex < this._summonIndexFall.Length - 1f ? this._fallIndex + 1f : 0f);
				}
			}
		}
		public new void Receive(DataConnection data)
		{
			base.Receive(data);
			if (!data.BossType.HasFlag(BossType.Jumper) || data.BossType.HasFlag(BossType.None))
				return;
			if (data.ConnectionState == ConnectionState.Action && data.ToggleValue.HasValue && this._hasToggle)
				this._stopJump = data.ToggleValue.Value;
			else if (data.ConnectionState == ConnectionState.Action && this._reactToDamage)
			{
				if (this._stopMoveReact)
				{
					Sender.Create().SetConnectionObject(ConnectionObject.Boss).SetConnectionState(ConnectionState.Action)
					.SetBossType(BossType.Runner).SetToggle(false).Send();
					this._rigidybody.linearVelocityX = 0f;
				}
				this._collider.isTrigger = true;
				this._rigidybody.AddForceY(this._strenghtReact);
				if (this._highReact)
					this.HighJump(this._otherTarget, this._useTarget);
			}
		}
		[System.Serializable]
		private struct JumpPointStructure
		{
			[SerializeField] private JumpPoint _jumpPointObject;
			[SerializeField] private Vector2 _point, _otherTarget;
			[SerializeField] private Vector2Int _jumpCountMaxMin;
			[SerializeField] private ushort _strength;
			[SerializeField] private bool _stopMove, _high, _useTarget;
			internal readonly JumpPoint JumpPointObject => this._jumpPointObject;
			internal readonly Vector2 Point => this._point;
			internal readonly Vector2 OtherTarget => this._otherTarget;
			internal readonly ushort Strength => this._strength;
			internal readonly ushort JumpCount => (ushort)Random.Range(this._jumpCountMaxMin.x, this._jumpCountMaxMin.y);
			internal readonly bool StopMove => this._stopMove;
			internal readonly bool High => this._high;
			internal readonly bool UseTarget => this._useTarget;
			internal short RemovalJumpCount { get; set; }
		};
	};
};
