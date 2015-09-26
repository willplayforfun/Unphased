using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Outliner : MonoBehaviour
{
    private int outsideVertexCount;

    void LateUpdate()
    {
        JarvisWrap();
    }

    public MeshFilter mf;

    public void JarvisWrap()
    {
        

        //Start with point with smallest y-coordinate.
        PointManager pm = GetComponent<PointManager>();

        if (pm.points.Count > 3)
        {
            //Debug.Log(pm.points.Count);
            mf.GetComponent<MeshRenderer>().enabled = true;
            GetComponent<LineRenderer>().enabled = true;

            Transform lowestPoint = pm.points[0];

            foreach (Transform point in pm.points)
            {
                if (lowestPoint == null || (point != null && point.position.y < lowestPoint.position.y))
                {
                    lowestPoint = point;
                }
            }

            //Debug.Log(lowestPoint.name);

            Transform currentPoint = lowestPoint;
            Vector3 lastEdge = Vector3.right;

            List<Vector3> outsidePositions = new List<Vector3>();
            outsideVertexCount = 0;

            int count = 0;
            do
            {
                Transform bestPoint = null;
                float bestAngle = Mathf.Infinity;

                //Compute angle between current point and all remaining points.
                //Pick smallest angle larger than current angle.
                foreach (Transform point in pm.points)
                {
                    if (point != null && !point.Equals(currentPoint))
                    {
                        float CCWangle = Vector3.Angle(lastEdge, (point.position - currentPoint.position));

                        Vector3 cross = Vector3.Cross(lastEdge, (point.position - currentPoint.position));
                        if (cross.z < 0)
                        {
                            CCWangle = 360 - CCWangle;
                        }

                        if (bestPoint == null || CCWangle < bestAngle || (CCWangle == bestAngle && (point.position - currentPoint.position).magnitude < (bestPoint.position - currentPoint.position).magnitude))
                        {
                            bestPoint = point;
                            bestAngle = CCWangle;
                        }
                    }
                }

                //Debug.Log(bestPoint.name + " " + bestAngle);
                Debug.DrawLine(currentPoint.position, bestPoint.position, Color.green);

                outsidePositions.Add(currentPoint.position);

                //Repeat
                lastEdge = (bestPoint.position - currentPoint.position);
                currentPoint = bestPoint;

                count++;
                outsideVertexCount++;

            } while (!currentPoint.Equals(lowestPoint) && count < pm.points.Count);

            outsidePositions.Add(currentPoint.position);

            //bezier curves

            List<Vector3> smoothedPositions = new List<Vector3>();

            for (int i = 0; i < outsidePositions.Count; i++)
            {
                if (i < outsidePositions.Count - 1)
                {
                    Vector2 P0 = outsidePositions[(int)Mathf.Repeat((float)(i - 1), (float)(outsidePositions.Count - 1))];
                    Vector2 P1 = outsidePositions[i];
                    Vector2 P2 = outsidePositions[(int)Mathf.Repeat((float)(i + 1), (float)(outsidePositions.Count - 1))];

                    for (float t = 0.4f; t <= 0.6; t += 0.1f)
                    {
                        Vector2 bezier = (1f - t) * (1f - t) * P0 + 2f * t * (1 - t) * P1 + t * t * P2;
                        smoothedPositions.Add(bezier);
                    }
                    //smoothedPositions.Add(outsidePositions[i]);
                }
            }
            smoothedPositions.Add(smoothedPositions[0]);

            // render

            LineRenderer lr = GetComponent<LineRenderer>();
            lr.SetVertexCount(smoothedPositions.Count);
            for (int i = 0; i < smoothedPositions.Count; i++)
            {
                lr.SetPosition(i, smoothedPositions[i]);
            }

            smoothedPositions.RemoveAt(smoothedPositions.Count - 1);

            Mesh mesh = new Mesh();
            /*
            smoothedPositions.Add(pm.centerOfMass + Vector2.right + Vector2.down);
            smoothedPositions.Add(pm.centerOfMass + Vector2.left + Vector2.down);
            smoothedPositions.Add(pm.centerOfMass + Vector2.left + Vector2.up);
            smoothedPositions.Add(pm.centerOfMass + Vector2.right + Vector2.up);
            */
            Vector3[] vertices = new Vector3[smoothedPositions.Count];
            Vector2[] uv = new Vector2[smoothedPositions.Count];
            Vector3[] normals = new Vector3[smoothedPositions.Count];
            for (int i = 0; i < smoothedPositions.Count; i++)
            {
                vertices[i] = smoothedPositions[i] - transform.position;
                uv[i] = (Vector2)smoothedPositions[i];
                normals[i] = Vector3.forward;
            }

            Vector2[] vertices2D = new Vector2[smoothedPositions.Count];
            for (int i = 0; i < smoothedPositions.Count; i++)
            {
                vertices2D[i] = (Vector2)smoothedPositions[i];
            }
            // Use the triangulator to get indices for creating triangles
            Triangulator tr = new Triangulator(vertices2D);
            int[] indices = tr.Triangulate();

            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.normals = normals;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            mf.mesh = mesh;
        }
        else
        {
            mf.GetComponent<MeshRenderer>().enabled = false;
            GetComponent<LineRenderer>().enabled = false;
        }
    }
}

