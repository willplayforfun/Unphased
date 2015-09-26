﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointManager : MonoBehaviour {
	
    [HideInInspector]
	public float gravityConstant = 1;
    [HideInInspector]
    public float attractionConstant = 1;
    [HideInInspector]
    public float repulsionConstant = 1;

    public float attractiveLimit;
    public float repulsiveLimit;

	public float distanceFromCenter;

	public List<Transform> points;

    public GameObject pointPrefab;

	public Vector2 centerOfMass = Vector2.zero;

    void Awake()
    {
        turnOffPoint = transform.position;
        points = new List<Transform>();
    }

    private bool _active;
    private Vector2 turnOffPoint;
    private bool ignoreCoM;

    public LayerMask blockingLayers;

    public void SetActive(bool active)
    {
        if (active != _active)
        {
            
            if (!active)
            {
                turnOffPoint = transform.position;
            }
            Vector2 delta = Vector2.zero;
            if (active)
            {
                delta = (Vector2)transform.position - turnOffPoint;
                ignoreCoM = true;
            }
            foreach (Transform t in points)
            {
                t.GetComponent<Rigidbody2D>().isKinematic = !active;
                if (!active) 
                {
                    t.GetComponent<Rigidbody2D>().velocity = Vector2.zero; 
                }
                if  (active) 
                {
                    Vector2 start = transform.position;
                    Vector2 end = t.position + (Vector3)delta;
                    RaycastHit2D rh = Physics2D.Raycast(start, (end- start), (end-start).magnitude, blockingLayers);
                    if (rh.collider != null)
                    {
                        Debug.DrawLine(start, rh.point, Color.blue, 2);
                        t.position = (Vector2)transform.position + (end - start).normalized  * (((Vector2)transform.position - rh.point).magnitude *0.9f);
                    }
                    else
                    {
                        t.position = (Vector2)transform.position + (end - start); //(end - start).normalized * Mathf.Clamp((end - start).magnitude,0,(end - start).magnitude);// += (Vector3)delta;
                    } 
                }
                t.GetComponent<Collider2D>().enabled = active;
            }
            _active = active;
        }
    }

    public Vector3 velocity;

    void Start()
    {
        List<GameObject> objs = new List<GameObject>(GameObject.FindGameObjectsWithTag("Point"));
        foreach(GameObject obj in objs)
        {
            points.Add(obj.transform);
        }
        /*
        for (int i = 0; i < 10; i++)
        {
            GameObject newPoint = Instantiate(pointPrefab);
            newPoint.transform.position = this.transform.position + Vector3.up * Random.Range(-1.5f, 1.5f) + Vector3.right * Random.Range(-1.5f, 1.5f) ;
            points.Add(newPoint.transform);
        }
        */
    }

    public int minPoints;

    public AudioClip largeMovementSoundLiquid;

	void FixedUpdate(){
        if (_active)
        {
            List<Transform> markedForRemoval = new List<Transform>();
            foreach (Transform t in points)
            {
                if(t==null)
                {
                    markedForRemoval.Add(t);
                }
            }
            foreach (Transform t in markedForRemoval)
            {
                points.Remove(t);
            }
            markedForRemoval.Clear();


            if (points.Count < minPoints)
            {
                Application.LoadLevel(2);
            }

            Vector3 tmpCenterMass = Vector3.zero;
            // loop through points
            foreach (Transform point in points)
            {
                Rigidbody2D pointComponent = point.GetComponent<Rigidbody2D>();

                // Center of Mass
                tmpCenterMass += point.position;

                // Add Graivty Force to point
                pointComponent.AddForce(gravityConstant * Vector2.down);

                // Instantiate New PointManager when out of bound
                if (((Vector2)point.position - centerOfMass).magnitude > distanceFromCenter)
                {
                    markedForRemoval.Add(point);
                }

                // loop through other points
                foreach (Transform otherPoint in points)
                {

                    if (!point.Equals(otherPoint))
                    {
                        Vector2 distance = (point.position - otherPoint.position);

                        // Add attraction Force to point
                        pointComponent.AddForce(-attractionConstant * Vector2.ClampMagnitude((distance.normalized / Mathf.Pow(distance.magnitude, 1.5f)), attractiveLimit));

                        // Add repulsive Force to otherPoint
                        pointComponent.AddForce(repulsionConstant * Vector2.ClampMagnitude((distance.normalized / Mathf.Pow(distance.magnitude, 2.5f)), repulsiveLimit));
                    }
                }
            }

            tmpCenterMass /= points.Count;
            Vector3 newVelocity = (tmpCenterMass - (Vector3)centerOfMass)/Time.deltaTime;
            if(Vector3.Distance(newVelocity, velocity) > 15f)
            {
                if (GetComponent<InputManager>().currentMode == InputManager.Mode.Liquid)
                {
                    GetComponent<AudioSource>().PlayOneShot(largeMovementSoundLiquid);
                }
            }
            velocity = newVelocity;
            centerOfMass = tmpCenterMass;
            //Debug.Log (centerOfMass.magnitude);

            if (!ignoreCoM)
            {
                foreach (Transform t in markedForRemoval)
                {
                    //points.Remove(t);
                    //Destroy(t.gameObject);
                }
            }
            else
            {
                ignoreCoM = false;
            }

            transform.position = centerOfMass;
        }
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceFromCenter);
    }

    public void AddRandomPoint()
    {
        Vector3 radial = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * Vector3.right * Random.Range(0, distanceFromCenter);
        GameObject newPoint = Instantiate(pointPrefab);
        newPoint.transform.position = transform.position + radial;
        points.Add(newPoint.transform);
    }

    public void SubtractRandomPoint()
    {

    }
}
