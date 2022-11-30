using Sirenix.OdinInspector;
using Tools.Types;
using UnityEngine;

namespace Management
{
	/// <summary>
	/// Spawns obstacles in the way of the player, and despawns them when they move outside the camera again.
	/// </summary>
	public class LevelManager : Singleton<LevelManager>
	{
		[SerializeField, Required] private LevelSettings settings;

		private Camera _cam;
		private Camera Cam
		{
			get
			{
				if (_cam != null) return _cam;
				return _cam = Camera.main;
			}
		}
		private float TotalSpawnHeight => Cam.orthographicSize;
		private float SpawnDistance => Cam.orthographicSize * Cam.aspect;

		private void FixedUpdate()
		{

		}
	}
}