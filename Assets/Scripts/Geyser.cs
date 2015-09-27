using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Geyser : MonoBehaviour {

	public float appliedForceSolid = 5f;
	public float appliedForceFluid = 5f;

    public float maxAffectorDistance = 3f;

    InputManager inputManager;

	void Awake() {
        inputManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<InputManager>();
	}

	void OnTriggerStay2D(Collider2D coll) {
        InputManager.Mode phase = inputManager.currentMode;
        
        // if we're solid it will only push us if the ball is in the geyser (so points stuck in geyser don't affect ball)
        if (phase == InputManager.Mode.Solid && coll.gameObject.layer == 12)
        {
            inputManager.GetComponent<Rigidbody2D>().AddForce((transform.rotation * Vector2.up) * appliedForceSolid);
        }
        // geyser doesn't push gas
        else if (phase == InputManager.Mode.Liquid)
        {
            float distanceFromRoot = (coll.transform.position - this.transform.position).magnitude;
            //Debug.DrawLine(transform.position, transform.position + (transform.rotation * Vector2.up));
            coll.GetComponent<Rigidbody2D>().AddForce((transform.rotation * Vector2.up) * appliedForceFluid * Mathf.Clamp01(1 - distanceFromRoot / maxAffectorDistance));
        }
	}
}