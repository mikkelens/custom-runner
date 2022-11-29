namespace Menus
{
	public interface IMenu
	{
		public void Confirm();
		public void MoveSelection(MenuDirection direction);
	}
}