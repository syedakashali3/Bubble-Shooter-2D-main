using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public bool canShoot = true;
    public float speed = 25f;

    [Header("Bubble References")]
    public Transform nextBubblePosition;
    public GameObject currentBubble;
    public GameObject nextBubble;
    public GameObject bottomShootPoint;

    [Header("Aiming Line Settings")]
    public int maxReflections = 3;
    public float maxRayDistance = 30f;
    public Color lineColor = Color.yellow;
    public float lineWidth = 0.05f;

    private Vector2 lookDirection;
    private float lookAngle;
    private GameObject limit;
    private LineRenderer lineRenderer;
    private Vector2 gizmosPoint;

    private void Awake()
    {
        limit = GameObject.FindGameObjectWithTag("Limit");

        // Create line renderer dynamically if not already present
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 0;
        lineRenderer.sortingOrder = 10;
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (GameManagerLevelsNew.instance.gameState != "play")
            return;

        gizmosPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lookDirection = gizmosPoint - (Vector2)transform.position;
        lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        // While aiming
        if (Input.GetMouseButton(0)
            && Camera.main.ScreenToWorldPoint(Input.mousePosition).y > bottomShootPoint.transform.position.y
            && Camera.main.ScreenToWorldPoint(Input.mousePosition).y < limit.transform.position.y)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, lookAngle - 90f);

            if (LevelManager.instance != null && LevelManager.instance.GetBubbleAreaChildCount() > 0)
            {
                lineRenderer.enabled = true;
                DrawReflections();
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }

        // On release, shoot the bubble
        if (canShoot && Input.GetMouseButtonUp(0)
            && Camera.main.ScreenToWorldPoint(Input.mousePosition).y > bottomShootPoint.transform.position.y
            && Camera.main.ScreenToWorldPoint(Input.mousePosition).y < limit.transform.position.y)
        {
            canShoot = false;
            lineRenderer.enabled = false;
            Shoot();
        }
    }

    private void DrawReflections()
    {
        List<Vector3> points = new List<Vector3>();

        Vector2 startPos = transform.position;
        Vector2 direction = transform.up;

        points.Add(startPos);

        for (int i = 0; i < maxReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(startPos, direction, maxRayDistance);

            if (hit.collider != null)
            {
                points.Add(hit.point);

                // If hit a wall, reflect the direction
                if (hit.collider.CompareTag("Wall"))
                {
                    direction = Vector2.Reflect(direction, hit.normal);
                    startPos = hit.point + direction * 0.01f;
                }
                // If hit a bubble, stop drawing
                else if (hit.collider.CompareTag("Bubble"))
                {
                    break;
                }
                else
                {
                    // any other collider - stop as safety
                    break;
                }
            }
            else
            {
                points.Add(startPos + direction * maxRayDistance);
                break;
            }
        }

        // Update the LineRenderer
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ConvertAll(p => (Vector3)p).ToArray());
    }

    public void Shoot()
    {
        if (currentBubble == null) CreateNextBubble();
        ScoreManager.GetInstance().AddThrows();
        AudioManager.instance.PlaySound("shoot");
        transform.rotation = Quaternion.Euler(0f, 0f, lookAngle - 90f);
        currentBubble.transform.rotation = transform.rotation;

        Rigidbody2D rb = currentBubble.GetComponent<Rigidbody2D>();
        rb.AddForce(currentBubble.transform.up * speed, ForceMode2D.Impulse);
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.gravityScale = 0;

        currentBubble.GetComponent<CircleCollider2D>().enabled = true;
        currentBubble = null;
    }

    public void SwapBubbles()
    {
        List<GameObject> bubblesInScene = LevelManager.instance.bubblesInScene;
        if (bubblesInScene.Count < 1) return;

        currentBubble.transform.position = nextBubblePosition.position;
        nextBubble.transform.position = transform.position;
        GameObject temp = currentBubble;
        currentBubble = nextBubble;
        nextBubble = temp;
    }

    public void CreateNewBubbles()
    {
        if (nextBubble != null) Destroy(nextBubble);
        if (currentBubble != null) Destroy(currentBubble);

        nextBubble = null;
        currentBubble = null;
        CreateNextBubble();
        canShoot = true;
    }

    public void CreateNextBubble()
    {
        List<GameObject> bubblesInScene = LevelManager.instance.bubblesInScene;
        List<string> colors = LevelManager.instance.colorsInScene;

        if (bubblesInScene.Count < 1) return;

        if (nextBubble == null)
        {
            nextBubble = InstantiateNewBubble(bubblesInScene);
        }

        if (currentBubble == null)
        {
            currentBubble = nextBubble;
            currentBubble.transform.position = transform.position;
            nextBubble = InstantiateNewBubble(bubblesInScene);
        }
    }

    private GameObject InstantiateNewBubble(List<GameObject> bubblesInScene)
    {
        if (bubblesInScene.Count > 0)
        {
            GameObject newBubble = Instantiate(bubblesInScene[Random.Range(0, bubblesInScene.Count)]);
            newBubble.transform.position = nextBubblePosition.position;
            newBubble.GetComponent<Bubble>().isFixed = false;
            newBubble.GetComponent<CircleCollider2D>().enabled = false;
            Rigidbody2D rb2d = newBubble.AddComponent<Rigidbody2D>();
            rb2d.gravityScale = 0f;
            return newBubble;
        }
        return null;
    }
}
