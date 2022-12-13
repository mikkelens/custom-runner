using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Tools.Helpers;
using Tools.Types;
using UnityEngine;

namespace Management
{
	/// <summary>
	/// Spawns obstacles in the way of the player, and despawns them when they move outside the camera again.
	/// </summary>
	public class LevelManager : Singleton<LevelManager>
	{
		[SerializeField, Required] private Transform hazardSpawnPoint;
		[SerializeField] private List<LevelData> levels = new List<LevelData>();

		private int _levelIndex;
		private LevelData CurrentLevel => levels.Count > _levelIndex ? levels[_levelIndex] : null;

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

		private HazardBox _lastSpawnedHazard;
		private float _lastSpawnTime;
		private float LastHazardDistance => _lastSpawnedHazard.TargetHorizontalSpeed.Abs() * _lastSpawnTime.TimeSince();

		private void Start()
		{
			_levelIndex = 0;
		}

		private void FixedUpdate()
		{
			LevelData level = CurrentLevel;
			if (level == null) return;

			if (level.LevelOver(Time.time))
			{
				_levelIndex++;
				if (CurrentLevel == null)
				{
					Debug.LogWarning("No more levels");
				}
			}
			else
			{
				if (_lastSpawnedHazard == null || LastHazardDistance >= level.WidthSpacingAtTime(Time.time))
				{
					// spawn hazard
					_lastSpawnedHazard = Instantiate(level.GetRandomRelativePrefab(), hazardSpawnPoint.position, Quaternion.identity, transform);
					_lastSpawnTime = Time.time;
				}
			}
		}
	}
}