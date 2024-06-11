/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using UnityEngine;
using Interhaptics.Utils;

namespace Interhaptics.Samples
{
    public class OnTriggerEventCall : MonoBehaviour
    {
        [SerializeField]
        private EventHapticSource eventHapticSource;
        [SerializeField]
        private float delayPlayTime = 2.0f;

        private void OnTriggerEnter(Collider other)
        {
            eventHapticSource.vibrationOffset = delayPlayTime;
            eventHapticSource.PlayEventVibration();
        }
    }
}
