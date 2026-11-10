using UnityEngine;

// The free area documents lie on. No layout group: documents keep wherever the player drops them.
[RequireComponent(typeof(RectTransform))]
public class DeskSurface : MonoBehaviour
{
    [Tooltip("Where the first evidence document lands, measured from the centre of the desk.")]
    [SerializeField] private Vector2 firstPosition = new Vector2(250f, 0f);
    [Tooltip("How far each further document is shifted from the one before, so they fan out.")]
    [SerializeField] private Vector2 cascadeOffset = new Vector2(40f, -40f);
    [Tooltip("Random tilt in degrees, either way, so documents look tossed onto the desk.")]
    [SerializeField] private float maxTilt = 3f;

    public T Place<T>(T prefab, int index) where T : Component
    {
        T document = Instantiate(prefab, transform);
        RectTransform rect = (RectTransform)document.transform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = firstPosition + cascadeOffset * index;
        rect.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-maxTilt, maxTilt));
        return document;
    }
}
