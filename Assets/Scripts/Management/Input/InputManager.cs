using Character;
using Menus;
using Scripts.Management.Input;
using Tools.Types;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Management.Input
{
	public class InputManager : PersistentSingleton<InputManager>
	{
		private InputBindings _input; // we assume this is set on awake, before everything else calls it

		private Player _player;
		public Player Player
		{
			get => _player;
			set // called by player in start/disable
			{
				_player = value;
				if (_player == null)
				{
					UnbindPlayerInputs();
				}
				else
				{
					BindPlayerInputs();
				}
			}
		}

		private Vector2Int _prevMove; // selection move handling buffer
		private IMenu _menu;
		public IMenu Menu
		{
			get => _menu;
			set // should be assigned by appearing menus or game manager of sorts
			{
				if (value == null)
				{
					UnbindMenuInputs();
				}
				_menu = value;
				if (_menu != null)
				{
					BindMenuInputs();
				}
			}
		}


		protected override void Awake()
		{
			base.Awake();
			_input = new InputBindings();
		}
		private void OnDisable()
		{
			_input?.Disable();
		}

	#region Bindings
		private void BindPlayerInputs()
		{
			_input.Move.Horizontal.performed += PlayerMove;
			_input.Move.Horizontal.canceled += PlayerMove;
			_input.Move.Jump.performed += PlayerJump;
			_input.Move.Jump.canceled += PlayerJump;
			_input.Move.Enable();
		}
		private void UnbindPlayerInputs()
		{
			_input.Move.Horizontal.performed -= PlayerMove;
			_input.Move.Horizontal.canceled -= PlayerMove;
			_input.Move.Jump.performed -= PlayerJump;
			_input.Move.Jump.canceled -= PlayerJump;
			_input.Move.Disable();
		}
		private void UnbindMenuInputs()
		{
			throw new System.NotImplementedException();
		}
		private void BindMenuInputs()
		{
			throw new System.NotImplementedException();
		}
	#endregion

	#region Forwarders
		// player character
		private void PlayerMove(InputAction.CallbackContext ctx)
		{
			Player.MoveAxis = ctx.canceled ? 0f : ctx.ReadValue<float>();
		}
		private void PlayerJump(InputAction.CallbackContext ctx)
		{
			Player.JumpInput = ctx.performed;
		}
		// menu
		private void MenuConfirm(InputAction.CallbackContext ctx)
		{
			if (ctx.canceled) return;
			Menu.Confirm();
		}
		private void MenuSelectMove(InputAction.CallbackContext ctx)
		{
			Vector2Int newMove = ctx.canceled ? Vector2Int.zero : Vector2Int.CeilToInt(ctx.ReadValue<Vector2>());
			if (newMove == Vector2Int.zero)
			{
				_prevMove = Vector2Int.zero; // next non-zero move will be guaranteed valid
				return;
			}

			Vector2Int deltaMove = newMove;
			if (newMove.x == _prevMove.x) deltaMove.x -= _prevMove.x;
			if (newMove.y == _prevMove.y) deltaMove.y -= _prevMove.y;

			if (deltaMove == Vector2Int.zero) return; // no difference

			_prevMove = newMove;

			// choose x over y, only allow one axis of movement at a time
			MenuDirection? direction = deltaMove.x switch
			{
				1 => MenuDirection.Right,
				-1 => MenuDirection.Left,
				_ => deltaMove.y switch
				{
					1 => MenuDirection.Up,
					-1 => MenuDirection.Down,
					_ => null
				}
			};

			if (direction != null) Menu.MoveSelection(direction.Value);
		}
	#endregion
	}
}