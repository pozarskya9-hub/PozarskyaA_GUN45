using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Рисуем обычное поле, но в "выключенном" состоянии: значение видно, а изменить его нельзя.
			EditorGUI.BeginDisabledGroup(true);
			EditorGUI.PropertyField(position, property, label, true);
			EditorGUI.EndDisabledGroup();
		}
	}
}
