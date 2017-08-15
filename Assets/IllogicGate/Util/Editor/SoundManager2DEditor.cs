using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace IllogicGate
{
    // --- Class Declaration ------------------------------------------------------------------------
    [CustomEditor(typeof(SoundManager2D))]
    public class SoundManager2DEditor : Editor
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        // --- Inspector --------------------------------------------------------------------------------
        // --- Properties -------------------------------------------------------------------------------
        ReorderableList clips;
        SerializedProperty prop_clips;

        // --- Editor -----------------------------------------------------------------------------------
        private void OnEnable()
        {
            prop_clips = serializedObject.FindProperty("clips");

            clips = new ReorderableList(serializedObject, prop_clips, true, true, true, true);
            clips.drawHeaderCallback = OnClipsHeaderGUI;
            clips.drawElementCallback = OnClipsElementGUI;
            clips.drawElementBackgroundCallback = OnClipsElementBackgroundGUI;

            float height = EditorGUI.GetPropertyHeight(SerializedPropertyType.ObjectReference, GUIContent.none);
            height += EditorGUI.GetPropertyHeight(SerializedPropertyType.Integer, new GUIContent("Priority"));
            height += 20;
            clips.elementHeight = height;
        }
        
        // -----------------------------------------------------------------------------------	
        void OnClipsHeaderGUI(Rect rect)
        {
            GUI.Label(rect, "Audio clips");
        }

        // -----------------------------------------------------------------------------------	
        void OnClipsElementBackgroundGUI(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isActive)
                EditorGUI.DrawRect(rect, GUI.skin.settings.selectionColor);
            else if (index % 2 == 1)
            {
                Color color = EditorGUIUtility.isProSkin ? Color.black : Color.gray;
                EditorGUI.DrawRect(rect, color);
            }
        }

        // -----------------------------------------------------------------------------------	
        void OnClipsElementGUI(Rect src, int index, bool isActive, bool isFocused)
        {
            SerializedProperty data = prop_clips.GetArrayElementAtIndex(index);
            SerializedProperty clip = data.FindPropertyRelative("clip");
            SerializedProperty loop = data.FindPropertyRelative("loop");
            SerializedProperty volume = data.FindPropertyRelative("volume");
            SerializedProperty priority = data.FindPropertyRelative("priority");

            // width of the whole rect
            float width = src.width;

            // clip
            Rect rect = new Rect(src);
            rect.y += 10;
            rect.height = EditorGUI.GetPropertyHeight(clip);
            rect.width = width - 50;
            EditorGUI.ObjectField(rect, clip, GUIContent.none);

            // loop
            rect.x += rect.width; rect.width = 32;
            GUI.Label(rect, "Loop");
            rect.x += rect.width; rect.width = 18;
            EditorGUI.PropertyField(rect, loop, GUIContent.none);

            // second line
            rect.x = src.x;
            rect.y += rect.height + 2;

            // volume
            rect.width = 25;
            GUI.Label(rect, "Vol");
            rect.x += rect.width; rect.width = (width * 0.66f) - rect.width;
            EditorGUI.Slider(rect, volume, 0, 1, GUIContent.none);

            // priority
            rect.x += rect.width; rect.width = 32;
            GUI.Label(rect, "Prio");
            rect.x += rect.width; rect.width = width - rect.x + 32;
            EditorGUI.PropertyField(rect, priority, GUIContent.none);
        }

        // -----------------------------------------------------------------------------------	
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dontDestroyOnLoad"));
            clips.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
    }
}