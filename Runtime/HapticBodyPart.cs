/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using System.Collections.Generic;
using Interhaptics.HapticBodyMapping;


namespace Interhaptics
{

    [UnityEngine.AddComponentMenu("Interhaptics/HapticBodyPart")]
    public class HapticBodyPart : UnityEngine.MonoBehaviour
    {

        public GroupID BodyPart = GroupID.Hand;
        public LateralFlag Side = LateralFlag.Global;

        public List<CommandData> ToCommandData()
        {
            return new List<CommandData> { new CommandData(Operator.Plus, this.BodyPart, this.Side) };
        }

    }

}

