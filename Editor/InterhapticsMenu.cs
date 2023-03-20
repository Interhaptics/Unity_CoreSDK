/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

namespace Interhaptics.Editor
{

    public static class InterhapticsMenu
    {

        private const string AddHapticManagerMenuItem = "GameObject/Interhaptics/Add Haptic Manager to Main Camera";

        [UnityEditor.MenuItem(AddHapticManagerMenuItem, priority = 1)]
        private static void AddHapticManager()
        {
            UnityEngine.Camera camera = UnityEngine.Camera.main;
            if (camera == null)
            {
                camera = UnityEngine.GameObject.FindObjectOfType<UnityEngine.Camera>();
                if (camera == null)
                {
                    UnityEngine.Debug.LogWarning("No camera was found in the scene, so one was generated");
                    camera = (new UnityEngine.GameObject("Main Camera")).AddComponent<UnityEngine.Camera>();
                    camera.tag = "MainCamera";
                }
                else
                {
                    UnityEngine.Debug.LogWarning(string.Format("No main camera was found, so the HapticManager is generated on {0}", camera.gameObject.name));
                }
            }

            camera.gameObject.AddComponent<HapticManager>();
            UnityEngine.Debug.Log("Haptic Manager was correctly instantiated.");
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

    }

}
