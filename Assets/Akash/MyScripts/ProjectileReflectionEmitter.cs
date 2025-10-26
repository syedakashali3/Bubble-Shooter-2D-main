using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class ProjectileReflectionEmitter : MonoBehaviour
{
    [Header("Reflection Settings")]
    public int maxReflectionCount = 5;
    public float maxStepDistance = 200f;

    [Header("Line Settings")]
    public Color lineColor = Color.yellow;
    public float lineWidth = 0.05f;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        DrawReflections();
    }

    private void DrawReflections()
    {
        List<Vector3> points = new List<Vector3>();

        Vector3 position = transform.position + transform.forward * 0.75f;
        Vector3 direction = transform.forward;

        points.Add(transform.position);

        for (int i = 0; i < maxReflectionCount; i++)
        {
            Ray ray = new Ray(position, direction);

            if (Physics.Raycast(ray, out RaycastHit hit, maxStepDistance))
            {
                points.Add(hit.point);

                // ✅ Stop drawing if hit object tagged "bubble"
                if (hit.collider.CompareTag("bubble"))
                {
                    break;
                }

                // Reflect the direction and continue
                direction = Vector3.Reflect(direction, hit.normal);
                position = hit.point;
            }
            else
            {
                points.Add(position + direction * maxStepDistance);
                break;
            }
        }

        // Update line renderer
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.15f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.5f);
    }
#endif
}
