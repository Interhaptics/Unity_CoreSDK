/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

namespace Interhaptics
{

    public class HapticMaterial : UnityEngine.ScriptableObject
    {

        [UnityEngine.SerializeField, UnityEngine.HideInInspector]
        private string m_text;
        public string text => m_text;

        public static HapticMaterial CreateInstanceFromString(string text)
        {
            HapticMaterial hapticMaterial = CreateInstance<HapticMaterial>();
            hapticMaterial.m_text = text;
            return hapticMaterial;
        }

        public static HapticMaterial CreateInstance(UnityEngine.TextAsset asset)
        {
            return CreateInstanceFromString(asset.text);
        }

        public override string ToString()
        {
            return text;
        }

    }

}