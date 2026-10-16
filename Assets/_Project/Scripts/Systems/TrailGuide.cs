using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Draws a quiet connector and places flat arrow stepping-stones along the NavMesh
/// route to the nearest undiscovered exhibit. The trail remains visible while the
/// player is in range and disappears after all exhibits have been discovered.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class TrailGuide : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject arrowMarkerPrefab;

    [Header("Trail layout")]
    [SerializeField, Min(0.05f)] private float refreshInterval = 0.15f;
    [SerializeField, Min(0.1f)] private float snapDistance = 6f;
    [SerializeField, Min(0.5f)] private float markerSpacing = 2.2f;
    [SerializeField, Min(1)] private int maxMarkers = 48;
    [SerializeField, Min(0f)] private float lineLift = 0.018f;
    [SerializeField, Min(0f)] private float markerLift = 0.004f;
    [SerializeField, Min(0.005f)] private float connectorWidth = 0.025f;
    [SerializeField] private Color connectorColor = new Color(0.56f, 0.45f, 0.31f, 1f);

    private LineRenderer line;
    private Exhibit[] exhibits;
    private NavMeshPath path;
    private readonly List<GameObject> markerPool = new List<GameObject>();
    private float timer;
    private bool warnedNoNavMesh;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.positionCount = 0;
        line.enabled = false;
        line.widthMultiplier = connectorWidth;
        line.startColor = connectorColor;
        line.endColor = connectorColor;

        // Unity requires NavMeshPath to be created after this component is initialized.
        path = new NavMeshPath();
        exhibits = FindObjectsByType<Exhibit>();
    }

    private void Update()
    {
        if (player == null || line == null)
        {
            HideTrail();
            return;
        }

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        timer = refreshInterval;
        Refresh();
    }

    private void Refresh()
    {
        if (exhibits == null || exhibits.Length == 0)
        {
            HideTrail();
            return;
        }

        Vector3[] corners = FindPathToNearestUndiscovered(out Exhibit target);
        if (corners == null || corners.Length < 2 || target == null)
        {
            HideTrail();
            return;
        }

        // Keep the trail visible until the target is discovered, even when nearby.
        line.positionCount = corners.Length;
        for (int i = 0; i < corners.Length; i++)
        {
            line.SetPosition(i, corners[i] + Vector3.up * lineLift);
        }
        line.enabled = true;

        PlaceMarkers(corners);
    }

    private Vector3[] FindPathToNearestUndiscovered(out Exhibit target)
    {
        target = null;
        Vector3[] bestCorners = null;
        float bestLength = float.MaxValue;

        bool hasStart = NavMesh.SamplePosition(
            player.position, out NavMeshHit start, snapDistance, NavMesh.AllAreas);

        // Do not draw a misleading straight line through walls if the player is not
        // on a baked NavMesh. Hide the guide and warn once so the NavMesh can be fixed.
        if (!hasStart)
        {
            if (!warnedNoNavMesh)
            {
                Debug.LogWarning("TrailGuide: Player is not near a NavMesh; Heritage Trail is hidden.");
                warnedNoNavMesh = true;
            }
            return null;
        }

        foreach (Exhibit exhibit in exhibits)
        {
            if (exhibit == null || exhibit.IsDiscovered) continue;

            // Sample near the exhibit because its pedestal may be excluded from the NavMesh.
            if (!NavMesh.SamplePosition(
                    exhibit.transform.position, out NavMeshHit end, snapDistance, NavMesh.AllAreas))
                continue;

            bool calculated = NavMesh.CalculatePath(
                start.position, end.position, NavMesh.AllAreas, path);
            if (!calculated || path.status != NavMeshPathStatus.PathComplete) continue;

            Vector3[] candidateCorners = path.corners;
            if (candidateCorners == null || candidateCorners.Length < 2) continue;

            float candidateLength = PathLength(candidateCorners);
            if (candidateLength < bestLength)
            {
                bestLength = candidateLength;
                target = exhibit;
                bestCorners = candidateCorners;
            }
        }

        return bestCorners;
    }

    private void PlaceMarkers(Vector3[] corners)
    {
        if (arrowMarkerPrefab == null)
        {
            HideMarkers();
            return;
        }

        int markerIndex = 0;
        for (int segment = 0; segment < corners.Length - 1 && markerIndex < maxMarkers; segment++)
        {
            Vector3 start = corners[segment];
            Vector3 end = corners[segment + 1];
            Vector3 direction = end - start;
            direction.y = 0f;

            float segmentLength = direction.magnitude;
            if (segmentLength < 0.1f) continue;
            direction /= segmentLength;

            // Put a marker at each bend, facing down the next segment.
            if (segment > 0 && markerIndex < maxMarkers)
            {
                SetMarker(markerIndex++, start, direction);
            }

            // Start a little ahead of the player so the first marker is not underfoot.
            float distance = segment == 0 ? Mathf.Min(0.8f, segmentLength * 0.5f) : markerSpacing;
            float lastSafeDistance = segmentLength - 0.35f;

            while (distance < lastSafeDistance && markerIndex < maxMarkers)
            {
                SetMarker(markerIndex++, start + direction * distance, direction);
                distance += markerSpacing;
            }
        }

        for (int i = markerIndex; i < markerPool.Count; i++)
        {
            markerPool[i].SetActive(false);
        }
    }

    private void SetMarker(int index, Vector3 floorPosition, Vector3 direction)
    {
        while (markerPool.Count <= index)
        {
            GameObject marker = Instantiate(arrowMarkerPrefab, transform);
            marker.name = $"TrailArrow_{markerPool.Count + 1}";
            markerPool.Add(marker);
        }

        Vector3 position = floorPosition + Vector3.up * markerLift;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        markerPool[index].transform.SetPositionAndRotation(position, rotation);
        if (!markerPool[index].activeSelf) markerPool[index].SetActive(true);
    }

    private void HideTrail()
    {
        if (line != null)
        {
            line.positionCount = 0;
            line.enabled = false;
        }
        HideMarkers();
    }

    private void HideMarkers()
    {
        for (int i = 0; i < markerPool.Count; i++)
        {
            if (markerPool[i] != null) markerPool[i].SetActive(false);
        }
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

