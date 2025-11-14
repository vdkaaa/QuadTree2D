using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CirclePoint : MonoBehaviour
{
    public SpriteRenderer sr;
    public Color baseColor = new Color(0.7f, 0.9f, 1f, 1f);
    public Color highlightColor = new Color(1f, 0.35f, 0.35f, 1f);

    void Reset()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = baseColor;
    }

    public void SetHighlighted(bool on)
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
        sr.color = on ? highlightColor : baseColor;
    }
}
