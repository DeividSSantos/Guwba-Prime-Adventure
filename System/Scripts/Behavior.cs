namespace GuwbaPrimeAdventure
{
	public interface IDamageable
	{
		public bool Damage(ushort damage);
	};
	public interface IInteractable
	{
		public void Interaction();
	};
	public interface IGrabtable
	{
		public void Paralyze(bool value);
	};
	public interface ICollectable
	{
		public void Collect();
	};
};
