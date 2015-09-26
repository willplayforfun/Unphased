using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour
{
    public bool active;

	void OnCollisionEnter2D(Collision2D collision)
    {
        if (active)
        {
            if (collision.collider.tag == "Point")
            {
                Destroy(collision.collider.gameObject);
            }
        }
    }
}
