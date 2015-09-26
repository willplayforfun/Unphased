using UnityEngine;
using System.Collections;

public class SphereGiz : MonoBehaviour {
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}
