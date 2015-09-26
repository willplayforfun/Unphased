﻿using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    public enum Mode
    {
        Gas,
        Liquid,
        Solid
    }
    public Mode currentMode;

    public Transform trackingCamera;

	// Use this for initialization
	void Start () {
        pm = GetComponent<PointManager>();
        SetMode(Mode.Liquid);
	}

    PointManager pm;

    public float SolidAttraction;
    public float FluidAttraction;

    public float GasGravity;
    public float LiquidGravity;
    public float SolidGravity;

    public float RepulsionConstant;

    public float rampTime;

    public SpriteRenderer spriteRender;

    public PhaseManager ui;

    public Material gasInside;
    public Material gasOutside;
    public Material liquidInside;
    public Material liquidOutside;

    public float visualSwapDelay;

    public SpriteRenderer eyes;

    public void SetMode(Mode m)
    {
        switch(m)
        {
            case Mode.Gas:
                Debug.Log("Gas!");
                if (currentMode == Mode.Solid)
                {
                    foreach (Transform point in pm.points)
                    {
                        point.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;
                    }
                }
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponent<Rigidbody2D>().isKinematic = true;
                StartCoroutine(WaitAndSwapVisuals(visualSwapDelay, gasInside, gasOutside));
                
                GetComponent<Roll>().enabled = false;
                pm.SetActive(true);
                StartCoroutine(RampAttraction(rampTime, FluidAttraction));
                pm.attractionConstant = FluidAttraction;
                pm.gravityConstant = GasGravity;
                pm.repulsionConstant = RepulsionConstant;
                currentMode = m;
                ui.phase = Phase.Gas;
                break;
            case Mode.Liquid:
                Debug.Log("Liquid!");
                if (currentMode==Mode.Solid)
                {
                    foreach (Transform point in pm.points)
                    {
                        point.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;
                    }
                }
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponent<Rigidbody2D>().isKinematic = true;
                pm.SetActive(true);
                GetComponent<Roll>().enabled = false;
                StartCoroutine(WaitAndSwapVisuals(visualSwapDelay, liquidInside, liquidOutside));
                StartCoroutine(RampAttraction(rampTime, FluidAttraction));
                pm.gravityConstant = LiquidGravity;
                pm.repulsionConstant = RepulsionConstant;
                currentMode = m;
                ui.phase = Phase.Liquid;

                break;
            case Mode.Solid:
                Debug.Log("Solid!");
                StartCoroutine(RampAttraction(0.2f, SolidAttraction, true));
                pm.gravityConstant = SolidGravity;
                pm.repulsionConstant = RepulsionConstant;
                ui.phase = Phase.Solid;
                break;
        }
    }

    public bool eyesOn;

    IEnumerator WaitAndSwapVisuals(float length, Material insideMaterial, Material outsideMaterial)
    {
        yield return new WaitForSeconds(length);
        GetComponent<Outliner>().mf.GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Outliner>().mf.GetComponent<MeshRenderer>().material = insideMaterial;
        GetComponent<LineRenderer>().enabled = true;
        GetComponent<LineRenderer>().material = outsideMaterial;
        spriteRender.enabled = false;
        eyes.enabled = eyesOn && true;

    }

    IEnumerator RampAttraction(float length, float target, bool solidAfter = false)
    {
        float start = pm.attractionConstant;
        float timer = 0;
        while(timer < length)
        {
            pm.attractionConstant = Mathf.Lerp(start, target, timer / length);
            timer += Time.deltaTime;
            yield return null;
        }

        pm.attractionConstant = target;

        if(solidAfter)
        {
            GetComponent<CircleCollider2D>().enabled = true;
            GetComponent<Rigidbody2D>().isKinematic = false;
            GetComponent<Rigidbody2D>().velocity = pm.velocity;
            GetComponent<Outliner>().mf.GetComponent<MeshRenderer>().enabled = false;
            GetComponent<LineRenderer>().enabled = false;
            spriteRender.enabled = true;
            eyes.enabled = false;
            GetComponent<Roll>().enabled = true;
            pm.SetActive(false);
            currentMode = Mode.Solid;

        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }

	// Update is called once per frame
	void Update () {
        if (currentMode == Mode.Solid)
        {
            trackingCamera.position = new Vector3(transform.position.x, transform.position.y, trackingCamera.position.z);
        }
        else
        {
            trackingCamera.position = new Vector3(pm.centerOfMass.x, pm.centerOfMass.y, trackingCamera.position.z);
        }

        if (currentMode != Mode.Solid)
        {
            foreach (Transform t in pm.points)
            {
                float distanceFromCenter = (t.position - (Vector3)pm.centerOfMass).magnitude;
                float mult = (1 - distanceFromCenter / pm.distanceFromCenter);
                t.GetComponent<Rigidbody2D>().AddForce(Vector2.right * Input.GetAxis("Horizontal") * 5);
            }
        }

        //gas
        if (Input.GetButtonDown("Gas"))
        {
            SetMode(Mode.Gas);
        }
        //liquid
        if (Input.GetButtonDown("Liquid"))
        {
            SetMode(Mode.Liquid);
        }
        //solid
        if (Input.GetButtonDown("Solid"))
        {
            SetMode(Mode.Solid);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            pm.AddRandomPoint();
            pm.AddRandomPoint();
            pm.AddRandomPoint();
            pm.AddRandomPoint();
            pm.AddRandomPoint();
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            pm.SubtractRandomPoint();
        }
	}
}
