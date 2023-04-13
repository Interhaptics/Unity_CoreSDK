/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using UnityEngine;

namespace Interhaptics.Samples
{
    public class ObjectTransform : MonoBehaviour
    {
        public float speed = 1f;
        public float leftLimit = -1.5f;
        public float rightLimit = 1.5f;
        public bool moveAtStart = false;

        private float targetX;
        private bool movingRight = true;

        private void Start()
        {
            targetX = rightLimit;
        }

        private void Update()
        {
            if (moveAtStart)
            {
                MoveSpatialHapticSource();
            }
        }

        public void MoveSpatialHapticSource()
        {
            transform.position = new Vector3(
                Mathf.MoveTowards(transform.position.x, targetX, speed * Time.deltaTime),
                transform.position.y,
                transform.position.z
            );

            if (Mathf.Approximately(transform.position.x, targetX))
            {
                if (movingRight)
                {
                    targetX = leftLimit;
                }
                else
                {
                    targetX = rightLimit;
                }
                movingRight = !movingRight;
            }
        }
    }
}

