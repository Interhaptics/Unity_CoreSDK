/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

#if UNITY_2020_2_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif


namespace Interhaptics.Editor
{

    [UnityEditor.CustomEditor(typeof(InterhapticsImporter)), UnityEditor.CanEditMultipleObjects]
    internal class InterhapticsImporterEditor : ScriptedImporterEditor
    {
        protected override bool needsApplyRevert => false;
    }

}
