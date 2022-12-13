using System.Collections.Generic;
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
			: move.FallGravityModifier.Enabled && !CollisionStates.grounded.State && Velocity.y < 0f
				? move.FallGravityModifier.Value * base.CurrentGravity
				: base.CurrentGravity;

		private protected override void FixedUpdate()
		{
			UpdateVelocity();
			base.FixedUpdate();
		}

		private void UpdateVelocity()
		{
			Vector2 newVelocity = Velocity;
			// walking
			float moveAccel = MoveAxis.AsSignedIntOrZero() * newVelocity.x.AsSignedIntOrZero() <= 0 // opposite or zero
				? CollisionStates.grounded ? move.GroundStopAccelSpeed : move.AirStopAccelSpeed // stopping
				: CollisionStates.grounded ? move.GroundAccelSpeed : move.AirAccelSpeed; // gaining speed
			newVelocity.x = Mathf.MoveTowards(newVelocity.x, move.MoveSpeed * MoveAxis, moveAccel * Time.fixedDeltaTime);

			// jumping
			if (_jumping)
			{
				if (!_jumpInput) // early/cancel end
				{
					_jumping = false;
					newVelocity.y = Mathf.Min(newVelocity.y, HeightToUpwardsVelocity(move.MinJumpHeight));
				}
				else if (newVelocity.y <= HeightToUpwardsVelocity(move.MinJumpHeight))
				{
					// natural end gives "peak" jump arc
					_jumping = false;
					_peakingJump = true; // lower gravity
					newVelocity.y = Mathf.Min(newVelocity.y, HeightToUpwardsVelocity(move.MinJumpHeight));
				}
			}
			else if (_peakingJump)
			{
				if (newVelocity.y <= 0f)
				{
					_peakingJump = false;
				}
			}
			else if (CanJump && WantsToJump)
			{
				_jumping = true;
				_pressedJumpSinceLastJump = false;
				newVelocity.y = Mathf.Max(newVelocity.y, HeightToUpwardsVelocity(move.MaxJumpHeight));
			}
			Velocity = newVelocity;
		}

		private void Start()
		{
			if (InputManager.Exists && InputManager.Instance.Player == null) InputManager.Instance.Player = this;
		}


		private void OnDisable()
		{
			if (InputManager.Exists && InputManager.Instance.Player == this) InputManager.Instance.Player = null;
		}

		public void TryHit()
		{
			Destroy(gameObject); // change to be nicer ig
		}
	}
}