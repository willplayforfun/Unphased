using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour
{

	void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Point")
        {
            //Destroy(collision.collider.gameObject);
        }
    }
}
