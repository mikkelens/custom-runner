using Sirenix.OdinInspector;
using Tools.Types;
using UnityEngine;

namespace Character
{
	[CreateAssetMenu(fileName = "New ForgivenessSettings", menuName = "ForgivenessSettings")]
	public class ForgivenessSettings : ScriptableObject
	{
		[field: ValidateInput("@!JumpPressTimeframe.Enabled || JumpPressTimeframe.Value != 0.0f")]
		[field: SerializeField] public Optional<float> JumpPressTimeframe { get; private set; } = 0.5f;

		[field: ValidateInput("@!JumpBufferFromCancel.Enabled || JumpBufferFromCancel.Value != 0.0f")]
		[field: SerializeField] public Optional<float> JumpBufferFromCancel { get; private set; } = 0.1f;
	}
}