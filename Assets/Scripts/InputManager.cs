using UnityEngine;
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

    public AudioClip toGasSound;
    public AudioClip toLiquidSound;
    public AudioClip toSolidSound;

    public void SetMode(Mode m)
    {
        if (m != currentMode)
        {
            switch (m)
            {
                case Mode.Gas:
                    GetComponent<AudioSource>().PlayOneShot(toGasSound);
                    //Debug.Log("Gas!");
                    GetComponent<Outliner>().show = true;
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
                    transform.rotation = Quaternion.identity;

                    break;
                case Mode.Liquid:
                    //Debug.Log("Liquid!");
                    GetComponent<Outliner>().show = true;
                    GetComponent<AudioSource>().PlayOneShot(toLiquidSound);
                    if (currentMode == Mode.Solid)
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
                    transform.rotation = Quaternion.identity;

                    break;
                case Mode.Solid:
                    //Debug.Log("Solid!");
                    GetComponent<Outliner>().show = false;
                    GetComponent<AudioSource>().PlayOneShot(toSolidSound);
                    StartCoroutine(RampAttraction(0.2f, SolidAttraction, true));
                    pm.gravityConstant = SolidGravity;
                    pm.repulsionConstant = RepulsionConstant;
                    ui.phase = Phase.Solid;
                    break;
            }
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
        }
    }

    public float cameraDampening;

    public float gasMovementForce = 5;
    public float liquidMovementForce = 5;

	// Update is called once per frame
	void Update () {
        if (currentMode == Mode.Solid)
        {
            trackingCamera.position = Vector3.Lerp(trackingCamera.position, new Vector3(transform.position.x, transform.position.y, trackingCamera.position.z), Time.deltaTime/cameraDampening);
        }
        else
        {
            trackingCamera.position = Vector3.Lerp(trackingCamera.position, new Vector3(pm.centerOfMass.x, pm.centerOfMass.y, trackingCamera.position.z), Time.deltaTime / cameraDampening);
        }

        if (currentMode != Mode.Solid)
        {
            foreach (Transform t in pm.points)
            {
                if (t != null)
                {
                    float distanceFromCenter = (t.position - (Vector3)pm.centerOfMass).magnitude;
                    float mult = (1 - distanceFromCenter / pm.distanceFromCenter);
                    t.GetComponent<Rigidbody2D>().AddForce(Vector2.right * Input.GetAxis("Horizontal") * (currentMode==Mode.Gas ? gasMovementForce : liquidMovementForce));
                }
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
