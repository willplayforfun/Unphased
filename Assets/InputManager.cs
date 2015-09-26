using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    public Transform camera;

	// Use this for initialization
	void Start () {
        pm = GetComponent<PointManager>();
	}

    PointManager pm;

	// Update is called once per frame
	void Update () {

        camera.position = new Vector3(pm.centerOfMass.x,pm.centerOfMass.y, camera.position.z);

        if (Input.GetKey(KeyCode.D))
        {
            foreach (Transform t in pm.points)
            {
                t.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 5 + Vector2.up * 2);
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            foreach (Transform t in pm.points)
            {
                t.GetComponent<Rigidbody2D>().AddForce(Vector2.left * 5 + Vector2.up * 2);
            }
        }
	}
}
