using Management.Input;
using Physics;
using Sirenix.OdinInspector;
using Tools.Helpers;
using Tools.Types;
using UnityEngine;

namespace Character
{
	[DefaultExecutionOrder(-1)]
	public class Player : PhysicsBox
	{
		[SerializeField, Required] private MoveSettings move;
		[SerializeField, Required] private ForgivenessSettings forgiveness;

	#region Input
		public float MoveAxis { get; set; }

		private bool _pressedJumpSinceLastJump;
		private TimedState _jumpInput;
		public bool JumpInput
		{
			set
			{
				_jumpInput.State = value;
				if (_jumpInput.State) _pressedJumpSinceLastJump = true;
			}
		}

	#endregion

		private bool _jumping;

		// if just let go of jump press and just hit the ground
		private bool PressedWithinTimeframe => !forgiveness.JumpPressTimeframe.Enabled || _jumpInput.StartTimeTrue.TimeSince() <= forgiveness.JumpPressTimeframe.Value;
		private bool WithinCancelForgiveness => forgiveness.JumpBufferFromCancel.Enabled && _jumpInput.StartTimeFalse.TimeSince() <= forgiveness.JumpBufferFromCancel.Value;
		private bool WantsToJump => (_jumpInput || WithinCancelForgiveness) && PressedWithinTimeframe;

		private bool CanJump => CollisionStates.grounded && _pressedJumpSinceLastJump;


		private protected override Vector2 UpdateVelocity(Vector2 newVelocity)
		{
			// walking
			float moveAccel = MoveAxis.AsIntSign() != newVelocity.x.AsSignedIntOrZero(0.1f) ? move.StopAccelSpeed : move.AccelSpeed;
			newVelocity.x = Mathf.MoveTowards(newVelocity.x, move.MoveSpeed * MoveAxis, moveAccel * Time.fixedDeltaTime);

			// jumping
			if (_jumping)
			{
				if (_jumpInput)
				{
					if (newVelocity.y <= 0f) _jumping = false;
				}
				else // cancel jump
				{
					_jumping = false;
					float minJumpVelocity = HeightToUpwardsVelocity(move.MinJumpHeight);
					newVelocity.y = Mathf.Min(newVelocity.y, minJumpVelocity);
				}
			}
			else if (CanJump && WantsToJump)
			{
				_jumping = true;
				_pressedJumpSinceLastJump = false;
				float maxJumpVelocity = HeightToUpwardsVelocity(move.MaxJumpHeight);
				newVelocity.y = Mathf.Max(newVelocity.y, maxJumpVelocity);
			}

			return newVelocity;
		}

		private protected override void Start()
		{
			base.Start();
			if (InputManager.Exists && InputManager.Instance.Player == null) InputManager.Instance.Player = this;
		}
		private void OnDisable()
		{
			if (InputManager.Exists && InputManager.Instance.Player == this) InputManager.Instance.Player = null;
		}
	}
}