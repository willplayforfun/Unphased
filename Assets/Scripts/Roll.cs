﻿using UnityEngine;
using System.Collections;

public class Roll : MonoBehaviour {
    private Rigidbody2D _body;
    public float torqueFactor = 1.0f;

    public float breakSpeed;
    public AudioClip breakSound;

    // Use this for initialization
    void Start () {
        broken = false;
        _body = GetComponent<Rigidbody2D>();
    }

    public AudioClip[] impactSounds;

    private bool onGround;

    public float groundedForce = 5;
    public float ungroundedForce = 2;
    public float constantUpwardsForce = 1;

    // Update is called once per frame
    void Update () {
	    _body.AddTorque(-Input.GetAxis("Horizontal") * torqueFactor);
        _body.AddForce(Vector2.right * Input.GetAxis("Horizontal") * (onGround ? groundedForce : ungroundedForce) + Vector2.up * constantUpwardsForce);
        onGround = false;

    }

    private float lastImpactEffect;

    private float lastAudioEffect;
    public float audioEffectGap = 2f;

    void OnCollisionEnter2D(Collision2D collision)
    {

        //Debug.Log("Collision of magnitude " + collision.relativeVelocity.magnitude);
        if(collision.relativeVelocity.magnitude > breakSpeed)
        {
            Break();
        }
        else
        {
            if (Time.time - lastImpactEffect > 0.8f)
            {
                lastImpactEffect = Time.time;

                SpawnImpactEffects(5);
            }
            if (Time.time - lastAudioEffect > audioEffectGap)
            {
                lastAudioEffect = Time.time;

                GetComponent<AudioSource>().PlayOneShot(impactSounds[Random.Range(0, impactSounds.Length)]);

            }

        }
    }

    private void SpawnImpactEffects(int n)
    {
        //spawn fragments
        for (int i = 0; i < n; i++)
        {
            GameObject fragment = Instantiate(fragmentPrefabs[Random.Range(0, fragmentPrefabs.Length)]);
            Vector3 radial = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * Vector3.right * Random.Range(0, GetComponent<CircleCollider2D>().radius);
            fragment.transform.position = this.transform.position + radial;
            fragment.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);


            fragment.GetComponent<Rigidbody2D>().velocity = radial * Random.Range(0f, 5f);
            fragment.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-250, 250);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        onGround = true;
    }

    public SpriteRenderer ball;
    public GameObject[] fragmentPrefabs;
    public float fragmentCount;

    private bool broken;

    private void Break()
    {
        if (!broken)
        {
            GetComponent<AudioSource>().PlayOneShot(breakSound);

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
            Destroy(fragment.GetComponent<FadeOutAfterTime>());

            fragment.GetComponent<Rigidbody2D>().velocity = radial * Random.Range(0f,5f);
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
        timer = 0;
        while (timer < 2f)
        {
            if (Input.GetButtonDown("Start"))
            {
                Time.timeScale = 1;
                Application.LoadLevel(2);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        //load lose
        Time.timeScale = 1;
        Application.LoadLevelAsync(2);

    }
}
