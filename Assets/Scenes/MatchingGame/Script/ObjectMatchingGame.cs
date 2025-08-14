using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ObjectMatchingGame : MonoBehaviour
{
    [SerializeField] private int matchId;
    private bool isDragging;
    private Vector3 startPoint, endPoint;

    private LineRenderer currentLine;
    private ObjectMatchform potentialTarget;

    private static MatchGameManager manager;
    public bool IsLocked { get; private set; } = false;

    private void Awake()
    {
        if (manager == null)
            manager = FindObjectOfType<MatchGameManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = RaycastMouse();
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                startPoint = GetMouseWorldPosition();
                currentLine = CreateLineRenderer();
                currentLine.SetPosition(0, startPoint);
            }
        }

        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPosition();
            currentLine.SetPosition(1, mousePos);
            endPoint = mousePos;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            RaycastHit2D hit = Physics2D.Raycast(endPoint, Vector2.zero);
            if (hit.collider != null && hit.collider.TryGetComponent(out ObjectMatchform matchform))
            {
                currentLine.SetPosition(1, matchform.transform.position);
                manager.AddConnection(this, matchform, currentLine);
            }
            else
            {
                Destroy(currentLine.gameObject); // Invalid connection
            }
        }
    }
    public void Lock()
    {
        IsLocked = true;
    }
    public void Unlock()
    {
        IsLocked = false;
        // Optional: reset visuals
    }
    private RaycastHit2D RaycastMouse()
    {
        Vector3 mouse = GetMouseWorldPosition();
        return Physics2D.Raycast(mouse, Vector2.zero);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        return pos;
    }

    public int GetMatchID() => matchId;

    private LineRenderer CreateLineRenderer()
    {
        GameObject lineObj = new GameObject("Line");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.widthMultiplier = 0.1f;

        // ✅ Make the line render behind the cards
        lr.sortingLayerName = "Default"; // or match your card layer
        lr.sortingOrder = -1; // Lower order = rendered behind

        return lr;
    }
}
