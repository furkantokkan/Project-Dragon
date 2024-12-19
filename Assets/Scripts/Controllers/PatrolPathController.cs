using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPathController : MonoBehaviour
    {
        private const float waypointGizmosRadius = 0.3f;
        private int index;
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(GetWayPoint(i), waypointGizmosRadius);
                Gizmos.color = Color.blue;
                int j = GetNextWaypointIndex(i);
                Gizmos.DrawLine(GetWayPoint(i), GetWayPoint(j));
            }
        }

        public Vector3 GetWayPoint(int index)
        {
            return transform.GetChild(index).position;
        }
        public int GetNextWaypointIndex(int index)
        {
            return index + 1 >= transform.childCount ? 0 : index + 1;
        }
    }
}

