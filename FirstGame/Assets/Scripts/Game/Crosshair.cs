using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public LayerMask targetmask;
    public SpriteRenderer dot;
    public Color dotHighlightColor;
    Color orginalDotColor;

    private void Start()
    {
        Cursor.visible = false;
        orginalDotColor = dot.color;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * -40 * Time.deltaTime);
    }

    public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetmask))
        {
            dot.color = dotHighlightColor;
        }
        else
        {
            dot.color = orginalDotColor;
        }
    }
}
