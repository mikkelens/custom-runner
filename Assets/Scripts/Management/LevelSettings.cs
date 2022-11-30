using System.Collections.Generic;
using Physics;
using Sirenix.OdinInspector;
using Tools.Types;
using UnityEngine;

namespace Management
{
	[CreateAssetMenu(fileName = "New LevelSettings", menuName = "LevelSettings")]
	public class LevelSettings : ScriptableObject
	{
		[field: SerializeField] private Range<float> widthSpacing = (1f, 10f);
		[field: SerializeField] private float test = 1f;
		[field: SerializeField] private Optional<float> minHeightClearance = 0.5f;
		public float MinClearance => minHeightClearance.Enabled ? minHeightClearance : 0f;
		[field: SerializeField, AssetsOnly] public List<PhysicsBox> ObstaclePrefabs { get; private set; }
	}
}