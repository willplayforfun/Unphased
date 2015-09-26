using UnityEngine;
using System.Collections;

public class Roll : MonoBehaviour {
    private Rigidbody2D _body;
    public float torqueFactor = 1.0f;

    // Use this for initialization
    void Start () {
        _body = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
	    _body.AddTorque(-Input.GetAxis("Horizontal") * torqueFactor);
        _body.AddForce(Vector2.right * Input.GetAxis("Horizontal") * 5 + Vector2.up * 1);
    }
}
