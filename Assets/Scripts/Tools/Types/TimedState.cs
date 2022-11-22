using System;
using UnityEngine;

namespace Tools.Types
{
	[Serializable]
	public struct TimedState
	{
		[SerializeField] private float lastTimeTrue;
		[SerializeField] private bool state;

		public float LastTimeTrue => lastTimeTrue;
		public bool State // never really accessed?
		{
			get => state;
			set
			{
				state = value;
				if (state)
				{
					lastTimeTrue = Time.time;
				}
			}
		}

		// IMPLICIT GET VALUE FROM STRUCT
		public static implicit operator bool(TimedState timedStateSource) => timedStateSource.State;

		// IMPLICIT ASSIGN STRUCT FROM STATE
		public static implicit operator TimedState(bool stateSource) => new TimedState(stateSource);

		public TimedState(bool newState)
		{
			state = newState;
			if (newState)
			{
				lastTimeTrue = Time.time;
			}
			else
			{
				lastTimeTrue = -999;
			}
		}
	}
}