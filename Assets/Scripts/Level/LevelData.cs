using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Tools.Types;
using UnityEngine;

namespace Management
{
	[CreateAssetMenu(fileName = "New LevelSettings", menuName = "LevelSettings")]
	public class LevelData : ScriptableObject
	{
		[field: SerializeField] private float levelTime = 7.5f;
		[field: SerializeField] private Range<float> widthSpacing = (1f, 10f);
		[field: SerializeField] private Range<float> heightSpacing = (6f, 2f);
		[field: SerializeField, AssetsOnly] public List<Relative<HazardBox>> Obstacles { get; private set; }

		public bool LevelOver(float time)
		{
			return time >= levelTime;
		}
		public float WidthSpacingAtTime(float time)
		{
			float t = time / levelTime;
			return Mathf.Lerp(widthSpacing.Min, widthSpacing.Min, t);
		}
		public float HeightSpacingAtTime(float time)
		{
			float t = time / levelTime;
			return Mathf.Lerp(heightSpacing.Min, heightSpacing.Min, t);
		}

		public HazardBox GetRandomRelativePrefab()
		{
			float total = Obstacles.Select(x => x.Relativity).Sum();
			float random = Random.Range(0, total);
			foreach (Relative<HazardBox> obstacle in Obstacles)
			{
				if (random <= obstacle.Relativity) return obstacle.Value;
				random -= obstacle.Relativity;
			}
			Debug.LogWarning("Did not randomly select prefab...");
			return Obstacles.Last().Value;
		}
	}
}