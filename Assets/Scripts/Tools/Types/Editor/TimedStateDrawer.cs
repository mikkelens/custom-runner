using Tools.Types;
using UnityEditor;
using UnityEngine;

namespace Tools.Editor
{
	[CustomPropertyDrawer(typeof(TimedState))]
	public class TimedStateDrawer : PropertyDrawer
	{
		private const string TimeSerializedRef = "lastTimeTrue";
		private const string StateSerializedRef = "state";

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			SerializedProperty stateProperty = property.FindPropertyRelative(StateSerializedRef);
			return EditorGUI.GetPropertyHeight(stateProperty);
		}

		public override void OnGUI
		(
			Rect fullRect,
			SerializedProperty fullProperty,
			GUIContent label
		)
		{
			// define constant ish variables
			float spacing = EditorGUIUtility.standardVerticalSpacing;

			// find sub-properties
			SerializedProperty timeProperty = fullProperty.FindPropertyRelative(TimeSerializedRef);
			SerializedProperty stateProperty = fullProperty.FindPropertyRelative(StateSerializedRef);

			Rect labelAndTime = fullRect;
			Rect stateRect = fullRect;

			stateRect.width = stateRect.height;
			labelAndTime.width -= stateRect.width;

			stateRect.x += labelAndTime.width + spacing;

			// BEGIN DRAWING //
			EditorGUI.BeginProperty(fullRect, label, fullProperty);

			EditorGUI.BeginDisabledGroup(stateProperty.boolValue == false);
			EditorGUI.PropertyField(labelAndTime, timeProperty, label, false);
			EditorGUI.EndDisabledGroup();

			EditorGUI.PropertyField(stateRect, stateProperty, GUIContent.none, false);
			EditorGUI.EndProperty();
			// END DRAWING //
		}
	}
}