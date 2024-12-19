using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class CameraFollower : MonoBehaviour
    {
        [SerializeField] private Transform target;

        public float height = 7f;

        public float zOffset = 5f;

        public float xOffset = 5f;

        public float followSpeed = 15f;

        void LateUpdate()
        {
            if (target != null)
            {
                transform.position = Vector3.Lerp(transform.position,
                      new Vector3(target.position.x + xOffset, target.position.y + height, target.position.z + zOffset),
                          followSpeed * Time.deltaTime);

            }
        }
    }
}

