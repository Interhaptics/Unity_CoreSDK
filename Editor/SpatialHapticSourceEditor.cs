using UnityEditor;
using UnityEngine;
using Interhaptics;

[CustomEditor(typeof(SpatialHapticSource))]
public class SpatialHapticSourceEditor : Editor
{

    SerializedProperty playOnStart;
    SerializedProperty customBodyPart;
    SerializedProperty hapticBodyPartObject;
    SerializedProperty debugMode;
    SerializedProperty playOnCollision;
    SerializedProperty playOnTrigger;

    private void OnEnable()
    {
        playOnStart = serializedObject.FindProperty("playOnStart");
        customBodyPart = serializedObject.FindProperty("customBodyPart");
        hapticBodyPartObject = serializedObject.FindProperty("hapticBodyPartObject");
        debugMode = serializedObject.FindProperty("debugMode");
        playOnCollision = serializedObject.FindProperty("playOnCollision");
        playOnTrigger = serializedObject.FindProperty("playOnTrigger");
    }
    public override void OnInspectorGUI()
    {
        SpatialHapticSource script = (SpatialHapticSource)target;

        // Call the base class's OnInspectorGUI method to display the fields from the HapticSource class
        base.OnInspectorGUI();

        GUIContent playOnStartLabel = new GUIContent("Play on start", "Controls whether the haptic source should start playing when the object becomes active.");
        EditorGUILayout.PropertyField(playOnStart, playOnStartLabel);

        GUIContent customBodyPartLabel = new GUIContent("Custom body parts", "Controls whether the haptic source should start playing on a custom body part.");
        EditorGUILayout.PropertyField(customBodyPart, customBodyPartLabel);

        GUIContent debugModeLabel = new GUIContent("Debug mode", "Shows debug messages in Editor if checked.");
        EditorGUILayout.PropertyField(debugMode, debugModeLabel);

        GUIContent playOnCollisionLabel = new GUIContent("Play on collision", "Plays the haptic source on collision with a haptic body part.");
        EditorGUILayout.PropertyField(playOnCollision, playOnCollisionLabel);

        GUIContent playOnTriggerLabel = new GUIContent("Play on trigger", "Controls whether when the body part enters a custom body part.");
        EditorGUILayout.PropertyField(playOnTrigger, playOnTriggerLabel);

        if ((playOnStart.boolValue)||(customBodyPart.boolValue))
        {
            GUIContent hapticBodyPartObjectLabel = new GUIContent("Haptic body part object", "The game object containing the haptic body part script/controller.");
            EditorGUILayout.PropertyField(hapticBodyPartObject, hapticBodyPartObjectLabel);
        }
        serializedObject.ApplyModifiedProperties();
    }
}