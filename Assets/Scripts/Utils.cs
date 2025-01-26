using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static (float width, float height) GetCubeDimensions(GameObject prefab, Canvas mainCanvas)
    {
        RectTransform prefabRectTransform = prefab.GetComponent<RectTransform>();

        float scaleFactor = mainCanvas.scaleFactor;

        float height = prefabRectTransform.rect.height * scaleFactor;
        float width = prefabRectTransform.rect.width * scaleFactor;

        return (width, height);
    }
}
