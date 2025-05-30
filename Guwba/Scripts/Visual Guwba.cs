using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using System.Collections;
using GuwbaPrimeAdventure.Effects;
using GuwbaPrimeAdventure.Data;
namespace GuwbaPrimeAdventure.Guwba
{
	[DisallowMultipleComponent, RequireComponent(typeof(Transform), typeof(BoxCollider2D))]
	public sealed class VisualGuwba : GuwbaAstral<VisualGuwba>, IDamageable
	{
		private static VisualGuwba _instance;
		private GuwbaHud _guwbaHud;
		private SpriteRenderer _spriteRenderer;
		private short _vitality;
		private bool _invencibility = false;
		[SerializeField] private GuwbaHud _guwbaHudObject;
		[SerializeField] private string _levelSelectorScene;
		[SerializeField] private ushort _invencibilityTime;
		[SerializeField, Tooltip("The value applied to visual when hit is taken.")] private float _invencibilityValue;
		[SerializeField, Tooltip("The amount of time that the has to stay before fade.")] private float _timeStep;
		[SerializeField, Tooltip("The amount of time to stop the game when hit is taken.")] private float _hitStopTime;
		[SerializeField, Tooltip("The amount of time to slow the game when hit is taken.")] private float _hitStopSlow;
		public ushort Health => (ushort)this._vitality;
		private new void Awake()
		{
			base.Awake();
			if (_instance)
			{
				Destroy(this.gameObject, 0.001f);
				return;
			}
			_instance = this;
			SaveController.Load(out SaveFile saveFile);
			this._spriteRenderer = this.GetComponentInParent<SpriteRenderer>();
			this._guwbaHud = Instantiate(this._guwbaHudObject, this.transform);
			this._guwbaHud.LifeText.text = $"X {saveFile.lifes}";
			this._guwbaHud.CoinText.text = $"X {saveFile.coins}";
			this._vitality = (short)this._guwbaHud.Vitality;
			_actualState += this.ManualInvencibility;
		}
		private new void OnDestroy()
		{
			base.OnDestroy();
			if (!_instance || _instance != this)
				return;
			_actualState -= this.ManualInvencibility;
			this.StopAllCoroutines();
			this._spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
		}
		private void OnEnable()
		{
			if (!_instance || _instance != this)
				return;
			if (this.gameObject.scene.name == this._levelSelectorScene)
				this._guwbaHud.RootElement.style.display = DisplayStyle.None;
			else
				this._guwbaHud.RootElement.style.display = DisplayStyle.Flex;
		}
		private void OnDisable()
		{
			if (!_instance || _instance != this)
				return;
			this._guwbaHud.RootElement.style.display = DisplayStyle.None;
		}
		private IEnumerator Invencibility()
		{
			this.StartCoroutine(VisualEffect());
			IEnumerator VisualEffect()
			{
				while (this._invencibility)
				{
					this._spriteRenderer.color = new Color(1f, 1f, 1f, this._spriteRenderer.color.a >= 1f ? this._invencibilityValue : 1f);
					yield return new WaitTime(this, this._timeStep);
				}
			}
			yield return new WaitTime(this, this._invencibilityTime);
			this._invencibility = false;
			this._spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
		}
		private UnityAction<bool> ManualInvencibility => isInvencible =>
		{
			if (isInvencible)
				this.StartCoroutine(this.Invencibility());
			else
				this.StopCoroutine(this.Invencibility());
		};
		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.TryGetComponent<ICollectable>(out var collectable))
			{
				collectable.Collect();
				SaveController.Load(out SaveFile saveFile);
				this._guwbaHud.LifeText.text = $"X {saveFile.lifes}";
				this._guwbaHud.CoinText.text = $"X {saveFile.coins}";
			}
		}
		public bool Damage(ushort damage)
		{
			if (this._invencibility || damage < 1f)
				return false;
			this._invencibility = true;
			this._vitality -= (short)damage;
			for (ushort i = (ushort)this._guwbaHud.VitalityVisual.Length; i > (this._vitality >= 0f ? this._vitality : 0f); i--)
			{
				this._guwbaHud.VitalityVisual[i - 1].style.backgroundColor = new StyleColor(new Color(0.75f, 0.75f, 0.75f));
				this._guwbaHud.VitalityVisual[i - 1].style.borderBottomColor = new StyleColor(new Color(0.75f, 0.75f, 0.75f));
				this._guwbaHud.VitalityVisual[i - 1].style.borderLeftColor = new StyleColor(new Color(0.75f, 0.75f, 0.75f));
				this._guwbaHud.VitalityVisual[i - 1].style.borderRightColor = new StyleColor(new Color(0.75f, 0.75f, 0.75f));
				this._guwbaHud.VitalityVisual[i - 1].style.borderTopColor = new StyleColor(new Color(0.75f, 0.75f, 0.75f));
			}
			if (this._vitality <= 0f)
			{
				this._vitality = 0;
				SaveController.Load(out SaveFile saveFile);
				saveFile.lifes -= 1;
				this._guwbaHud.LifeText.text = $"X {saveFile.lifes}";
				SaveController.WriteSave(saveFile);
				if (_grabObject)
					Destroy(_grabObject.gameObject);
				GuwbaAstral<CommandGuwba>._actualState.Invoke(true);
				this.ManualInvencibility.Invoke(false);
				GuwbaAstral<AttackGuwba>._actualState.Invoke(false);
				GuwbaAstral<AttackGuwba>.Position = this.transform.position;
				this.StopAllCoroutines();
				this._spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
				return true;
			}
			EffectsController.SetHitStop(this._hitStopTime, this._hitStopSlow);
			this.StartCoroutine(this.Invencibility());
			return true;
		}
	};
};
