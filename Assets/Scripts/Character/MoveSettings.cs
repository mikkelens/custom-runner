﻿using Tools.Types;
using UnityEngine;

namespace Character
{
	/// <summary>
	/// Settings for player movement
	/// </summary>
	[CreateAssetMenu(fileName = "New MoveSettings", menuName = "MoveSettings")]
	public class MoveSettings : ScriptableObject
	{
		[field: SerializeField] public float MoveSpeed { get; private set; } = 2f;
		[field: SerializeField] public float AccelSpeed { get; private set; } = 100f;
		[field: SerializeField] public float StopAccelSpeed { get; private set; } = 200f;
		[field: SerializeField] public float MinJumpHeight { get; private set; } = 2f;
		[field: SerializeField] public float MaxJumpHeight { get; private set; } = 4f;
		[field: SerializeField] public Optional<float> PeakGravityModifier { get; private set; } = 2f; // used with min jump heigt
		[field: SerializeField] public Optional<float> FallGravityModifier { get; private set; } = 2f;
	}
}