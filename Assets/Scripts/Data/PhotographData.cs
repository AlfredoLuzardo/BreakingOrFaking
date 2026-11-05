using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Photograph", menuName = "Breaking or Faking/Documents/Photograph")]
public class PhotographData : DocumentData
{
    [SerializeField] private Sprite photo;
    [SerializeField] private Section caption = new Section();
    [Tooltip("Who took the photo, e.g. \"Photo: J. Alvarez\".")]
    [SerializeField] private Section credit = new Section();

    public Sprite Photo => photo;
    public Section Caption => caption;
    public Section Credit => credit;
    public override string Label => "Photo";

    public override IEnumerable<Section> GetSections()
    {
        yield return caption;
        yield return credit;
    }
}
