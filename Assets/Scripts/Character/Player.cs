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
		private bool _peakingJump; // top of jump arc

		// if just let go of jump press and just hit the ground
		private bool PressedWithinTimeframe => !forgiveness.JumpPressTimeframe.Enabled || _jumpInput.StartTimeTrue.TimeSince() <= forgiveness.JumpPressTimeframe.Value;
		private bool WithinCancelForgiveness => forgiveness.JumpBufferFromCancel.Enabled && _jumpInput.StartTimeFalse.TimeSince() <= forgiveness.JumpBufferFromCancel.Value;
		private bool WantsToJump => (_jumpInput || WithinCancelForgiveness) && PressedWithinTimeframe;
		private bool CanJump => CollisionStates.grounded && _pressedJumpSinceLastJump;

		private protected override float CurrentGravity => move.PeakGravityModifier.Enabled && _peakingJump
			? move.PeakGravityModifier.Value * base.CurrentGravity
			: move.FallGravityModifier.Enabled && !CollisionStates.grounded.State && physicsVelocity.y < 0f
				? move.FallGravityModifier.Value * base.CurrentGravity
				: base.CurrentGravity;

		private protected override Vector2 UpdateVelocity(Vector2 newVelocity)
		{
			// walking
			float moveAccel = MoveAxis.AsIntSign() != newVelocity.x.AsSignedIntOrZero(0.1f) ? move.StopAccelSpeed : move.AccelSpeed;
			newVelocity.x = Mathf.MoveTowards(newVelocity.x, move.MoveSpeed * MoveAxis, moveAccel * Time.fixedDeltaTime);

			// jumping
			if (_jumping)
			{
				if (newVelocity.y <= HeightToUpwardsVelocity(move.MinJumpHeight)) // peak of jump, arc manipulation
				{
					// natural jump eak arc:
					// lower gravity at peak
					_peakingJump = true; // this changes result of height-to-velocity function
					// remap velocity to new gravity
					newVelocity.y = Mathf.Min(newVelocity.y, HeightToUpwardsVelocity(move.MinJumpHeight));
				}
				if (!_jumpInput)
				{
					_jumping = false; // cancel jump
					_peakingJump = true; // cancel-based jump peak arc
					newVelocity.y = Mathf.Min(newVelocity.y, HeightToUpwardsVelocity(move.MinJumpHeight));
				}
				else if (newVelocity.y < 0f)
				{
					_jumping = false; // jump has naturally ended
				}
			}
			else if (CanJump && WantsToJump)
			{
				_jumping = true;
				_pressedJumpSinceLastJump = false;
				newVelocity.y = Mathf.Max(newVelocity.y, HeightToUpwardsVelocity(move.MaxJumpHeight));
			}
			if (_peakingJump && newVelocity.y <= 0f)
			{
				_peakingJump = false; // peak has (naturally) ended
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