using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace IllogicGate.UI
{
    // --- Class Declaration ------------------------------------------------------------------------
    [CustomEditor(typeof(ColorToggle)), CanEditMultipleObjects]
    public class ColorToggleEditor : ToggleEditor
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        // --- Editor -----------------------------------------------------------------------------------
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("toggleOnColor"));
            serializedObject.ApplyModifiedProperties();
        }

        // -----------------------------------------------------------------------------------
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
    }
}