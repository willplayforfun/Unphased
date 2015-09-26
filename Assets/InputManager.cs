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
                GetComponent<Outliner>().mf.GetComponent<MeshRenderer>().enabled = true;
                GetComponent<LineRenderer>().enabled = true;
                GetComponent<Roll>().enabled = false;
                pm.SetActive(true);
                spriteRender.enabled = false;
                StartCoroutine(RampAttraction(rampTime, FluidAttraction));
                pm.attractionConstant = FluidAttraction;
                pm.gravityConstant = GasGravity;
                pm.repulsionConstant = RepulsionConstant;
                currentMode = m;

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
                GetComponent<Outliner>().mf.GetComponent<MeshRenderer>().enabled = true;
                GetComponent<LineRenderer>().enabled = true;
                spriteRender.enabled = false;
                pm.SetActive(true);
                GetComponent<Roll>().enabled = false;

                StartCoroutine(RampAttraction(rampTime, FluidAttraction));
                pm.gravityConstant = LiquidGravity;
                pm.repulsionConstant = RepulsionConstant;
                currentMode = m;
                break;
            case Mode.Solid:
                Debug.Log("Solid!");
                StartCoroutine(RampAttraction(rampTime, SolidAttraction, true));
                pm.gravityConstant = SolidGravity;
                pm.repulsionConstant = RepulsionConstant;
                break;
        }
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

        foreach (Transform t in pm.points)
        {
            t.GetComponent<Rigidbody2D>().AddForce(Vector2.right * Input.GetAxis("Horizontal") * 5 + Vector2.up * 2);
        }

        //gas
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetMode(Mode.Gas);
        }
        //liquid
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetMode(Mode.Liquid);
        }
        //solid
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetMode(Mode.Solid);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            pm.AddRandomPoint();
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            pm.SubtractRandomPoint();
        }
	}
}
