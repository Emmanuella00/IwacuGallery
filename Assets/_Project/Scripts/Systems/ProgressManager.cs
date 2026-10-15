using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Tracks which exhibits the player has discovered and keeps the on-screen counter up to date.
/// The total is counted from the Exhibit objects in the scene, so adding an exhibit
/// needs no code change.
/// </summary>
public class ProgressManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private TMP_Text counterLabel;

    private readonly HashSet<string> discoveredIds = new HashSet<string>();
    private int totalExhibits;

    public int DiscoveredCount => discoveredIds.Count;
    public int TotalExhibits => totalExhibits;

    /// <summary>Raised whenever the count changes: (discovered, total).</summary>
    public event Action<int, int> ProgressChanged;

    /// <summary>Raised once, when every exhibit has been discovered.</summary>
    public event Action AllDiscovered;

    private void Awake()
    {
        totalExhibits = FindObjectsByType<Exhibit>(FindObjectsSortMode.None).Length;
        UpdateLabel();
    }

    private void OnEnable() => uiManager.ExhibitDiscovered += OnExhibitDiscovered;

    private void OnDisable() => uiManager.ExhibitDiscovered -= OnExhibitDiscovered;

    private void OnExhibitDiscovered(Exhibit exhibit)
    {
        // A set ignores duplicates, so an exhibit can never be counted twice.
        if (!discoveredIds.Add(exhibit.Data.Id)) return;

        UpdateLabel();
        ProgressChanged?.Invoke(DiscoveredCount, totalExhibits);

        if (DiscoveredCount >= totalExhibits)
        {
            Debug.Log("All exhibits discovered!"); // TEMP: replaced by the completion screen
            AllDiscovered?.Invoke();
        }
    }

    private void UpdateLabel()
    {
        counterLabel.text = $"Discovered  {DiscoveredCount} / {totalExhibits}";
    }
}