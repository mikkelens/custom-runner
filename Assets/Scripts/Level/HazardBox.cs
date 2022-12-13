using Character;
using Physics;
using Tools.Helpers;
using UnityEngine;

namespace Management
{
	public class HazardBox : PhysicsBox
	{
		[field: Header("Hazard Settings")]
		[field: SerializeField] public float TargetHorizontalSpeed { get; private set; } = 4f;
		[field: SerializeField] private float horizontalAcceleration = 50f;
		[field: SerializeField] private float maximumLifetime = 10f;

		private float _spawnTime;

		private protected override void Start()
		{
			base.Start();
			_spawnTime = Time.time;
		}

		private protected override void FixedUpdate()
		{
			if (_spawnTime.TimeSince() >= maximumLifetime)
			{
				Destroy(gameObject);
			}
			else
			{
				UpdateeVelocity();
				 base.FixedUpdate();
			}
		}

		private void UpdateeVelocity()
		{
			Vector2 newVelocity = Velocity;

			newVelocity.x = Mathf.MoveTowards(newVelocity.x, TargetHorizontalSpeed, horizontalAcceleration * Time.fixedDeltaTime);

			Velocity = newVelocity;
		}

		private protected override Vector2 HandleCollision(Vector2 newStep, RaycastHit2D hit)
		{
			Player player = hit.transform.GetComponent<Player>();
			if (player == null) return base.HandleCollision(newStep, hit);

			player.TryHit();
			return newStep;
		}
	}
}