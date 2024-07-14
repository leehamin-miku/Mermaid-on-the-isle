using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Sign : MonoBehaviour
{
    Color color;
    [SerializeField] public Vector3 target;
    [SerializeField] float a = 430;
    [SerializeField] float width = 362;
    [SerializeField] float height = 195;
    void Update()
    {
        Vector2 vec = Camera.main.transform.InverseTransformPoint(target).normalized*a;

        float signX = vec.x;
        float signY = vec.y;
        if (vec.x > width)
        {
            signX = width;
        } else if (signX < -width)
        {
            signX = -width;
        }

        if (signY > height)
        {
            signY = height;
        }
        else if (signY < -height)
        {
            signY = -height;
        }

        GetComponent<RectTransform>().localPosition = new Vector2(signX, signY);
    }
}
