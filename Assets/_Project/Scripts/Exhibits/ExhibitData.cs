using UnityEngine;

/// <summary>
/// Content for one exhibit. Create one asset per exhibit
/// (Project window: Create > Iwacu > Exhibit Data). Holds data only, no logic.
/// </summary>
[CreateAssetMenu(fileName = "NewExhibit", menuName = "Iwacu/Exhibit Data")]
public class ExhibitData : ScriptableObject
{
    [SerializeField] private string title;
    [SerializeField] private string subtitle;
    [SerializeField, TextArea(4, 10)] private string story;
    [SerializeField] private Sprite image;
    [SerializeField] private string sourceCredit;

    public string Title => title;
    public string Subtitle => subtitle;
    public string Story => story;
    public Sprite Image => image;
    public string SourceCredit => sourceCredit;

    /// <summary>Unique key used when saving progress (the asset's file name).</summary>
    public string Id => name;
}
