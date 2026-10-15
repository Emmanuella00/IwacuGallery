using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Draws the "Heritage Trail": a Line Renderer path along the floor from the player to the
/// nearest undiscovered exhibit. The path comes from the NavMesh, so the line follows the
/// corridors instead of cutting through walls. If no NavMesh has been baked, it falls back
/// to a straight line.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class TrailGuide : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float refreshInterval = 0.15f;
    [SerializeField] private float lift = 0.08f;          // keeps the line just above the floor
    [SerializeField] private float snapDistance = 6f;     // how far to search for the nearest NavMesh point

    private LineRenderer line;
    private Exhibit[] exhibits;
    private NavMeshPath path;
    private float timer;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        exhibits = FindObjectsByType<Exhibit>(FindObjectsSortMode.None);

        path = new NavMeshPath();

    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        timer = refreshInterval;
        Refresh();
    }

    private void Refresh()
    {
        Vector3[] corners = FindPathToNearestUndiscovered(out Exhibit target);

        // Hide the line when everything is discovered, or when the player has arrived.
        bool show = corners != null && !target.IsInRange;
        line.enabled = show;
        if (!show) return;

        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] += Vector3.up * lift;
        }

        line.positionCount = corners.Length;
        line.SetPositions(corners);
    }

    private Vector3[] FindPathToNearestUndiscovered(out Exhibit target)
    {
        target = null;
        Vector3[] bestCorners = null;
        float bestLength = float.MaxValue;

        bool hasStart = NavMesh.SamplePosition(player.position, out NavMeshHit start, snapDistance, NavMesh.AllAreas);

        foreach (Exhibit exhibit in exhibits)
        {
            if (exhibit.IsDiscovered) continue;

            // Without a NavMesh, fall back to a straight line to the closest exhibit.
            if (!hasStart)
            {
                float straight = Vector3.Distance(player.position, exhibit.transform.position);
                if (straight < bestLength)
                {
                    bestLength = straight;
                    target = exhibit;
                    bestCorners = new[] { player.position, exhibit.transform.position };
                }
                continue;
            }

            if (!NavMesh.SamplePosition(exhibit.transform.position, out NavMeshHit end, snapDistance, NavMesh.AllAreas)) continue;
            if (!NavMesh.CalculatePath(start.position, end.position, NavMesh.AllAreas, path)) continue;
            if (path.status != NavMeshPathStatus.PathComplete) continue;

            Vector3[] corners = path.corners;
            float length = PathLength(corners);

            if (length < bestLength)
            {
                bestLength = length;
                target = exhibit;
                bestCorners = corners;
            }
        }

        return bestCorners;
    }

    private static float PathLength(Vector3[] corners)
    {
        float length = 0f;
        for (int i = 1; i < corners.Length; i++)
        {
            length += Vector3.Distance(corners[i - 1], corners[i]);
        }
        return length;
    }
}
