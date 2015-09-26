using UnityEngine;
using System.Collections;

public class Roll : MonoBehaviour {
    private Rigidbody2D _body;
    public float torqueFactor = 1.0f;

    public float breakSpeed;

    // Use this for initialization
    void Start () {
        broken = false;
        _body = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
	    _body.AddTorque(-Input.GetAxis("Horizontal") * torqueFactor);
        _body.AddForce(Vector2.right * Input.GetAxis("Horizontal") * 5 + Vector2.up * 1);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.relativeVelocity.magnitude > breakSpeed)
        {
            Break();
        }
    }

    public SpriteRenderer ball;
    public GameObject[] fragmentPrefabs;
    public float fragmentCount;

    private bool broken;

    private void Break()
    {
        if (!broken)
        {
            broken = true;
            GetComponent<InputManager>().enabled = false;
            ball.enabled = false;
            _body.isKinematic = true;
            GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine(WaitThenReload(0.4f));
        }
    }

    IEnumerator WaitThenReload(float rampDuration)
    {
        //spawn fragments
        for (int i = 0; i < fragmentCount; i++)
        {
            GameObject fragment = Instantiate(fragmentPrefabs[Random.Range(0, fragmentPrefabs.Length)]);
            Vector3 radial = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * Vector3.right * Random.Range(0, GetComponent<CircleCollider2D>().radius);
            fragment.transform.position = this.transform.position + radial;
            fragment.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);

            fragment.GetComponent<Rigidbody2D>().velocity = radial * 5f;
            fragment.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-250, 250);
        }

        // ramp time
        float timer = 0;
        while (timer < rampDuration)
        {
            Time.timeScale = Mathf.Lerp(1, 0.5f, timer / rampDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        //wait
        yield return new WaitForSeconds(2);

        //load lose
        Application.LoadLevelAsync(2);

    }
}
