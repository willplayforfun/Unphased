using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Outliner : MonoBehaviour
{
    void Update()
    {
        JarvisWrap();
    }

    public void JarvisWrap()
    {

        //Start with point with smallest y-coordinate.
        PointManager pm = GetComponent<PointManager>();

        Debug.Log(pm.points.Count);


        Transform lowestPoint = pm.points[0];

        foreach(Transform point in pm.points)
        {
            if(point.position.y < lowestPoint.position.y)
            {
                lowestPoint = point;
            }
        }

        Debug.Log(lowestPoint.name);

        Transform currentPoint = lowestPoint;
        Vector3 lastEdge = Vector3.right;

        int count = 0;
        do
        {
            Transform bestPoint = null;
            float bestAngle = Mathf.Infinity;

            //Compute angle between current point and all remaining points.
            //Pick smallest angle larger than current angle.
            foreach (Transform point in pm.points)
            {
                if (!point.Equals(currentPoint))
                {
                    float CCWangle = Vector3.Angle(lastEdge, (point.position - currentPoint.position));

                    Vector3 cross = Vector3.Cross(lastEdge, (point.position - currentPoint.position));
                    if(cross.z < 0)
                    {
                        CCWangle = 360 - CCWangle;
                    }

                    if (bestPoint == null || CCWangle < bestAngle)
                    {
                        bestPoint = point;
                        bestAngle = CCWangle;
                    }
                }
            }

            Debug.Log(bestPoint.name + " " + bestAngle);
            Debug.DrawLine(currentPoint.position, bestPoint.position, Color.green);

            //Repeat
            lastEdge = (bestPoint.position - currentPoint.position);
            currentPoint = bestPoint;

            count++;

        } while (!currentPoint.Equals(lowestPoint) && count < pm.points.Count);
    }
}

