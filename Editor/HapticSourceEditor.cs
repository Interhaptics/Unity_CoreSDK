/* ​
* Copyright 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using UnityEditor;
using UnityEngine;
using Interhaptics;
using Interhaptics.Internal;

[CustomEditor(typeof(HapticSource))]
public class HapticSourceEditor : Editor
{
    SerializedProperty hapticMaterial;
    SerializedProperty vibrationOffset;
    SerializedProperty textureOffset;
    SerializedProperty stiffnessOffset;

    private void OnEnable()
    {
        hapticMaterial = serializedObject.FindProperty("hapticMaterial");
        vibrationOffset = serializedObject.FindProperty("vibrationOffset");
        textureOffset = serializedObject.FindProperty("textureOffset");
        stiffnessOffset = serializedObject.FindProperty("stiffnessOffset");
    }

    public override void OnInspectorGUI()
    {
        HapticSource script = (HapticSource)target;

        GUIContent hapticMaterialLabel = new GUIContent("Haptic Material", "The haptic material used by this source.");
        EditorGUILayout.PropertyField(hapticMaterial, hapticMaterialLabel, true);

        GUIContent vibrationOffsetLabel = new GUIContent("Vibration offset", "The offset applied to the vibration of the haptic effect.");
        EditorGUILayout.PropertyField(vibrationOffset, vibrationOffsetLabel);

        GUIContent textureOffsetLabel = new GUIContent("Texture offset", "The offset applied to the texture of the haptic effect.");
        EditorGUILayout.PropertyField(textureOffset, textureOffsetLabel);

        GUIContent stiffnessOffsetLabel = new GUIContent("Stiffness offset", "The offset applied to the stiffness of the haptic effect.");
        EditorGUILayout.PropertyField(stiffnessOffset, stiffnessOffsetLabel);

        serializedObject.ApplyModifiedProperties();
    }
}