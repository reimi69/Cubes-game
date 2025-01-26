using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCanGizmo : MonoBehaviour
{
    [SerializeField] private RectTransform holeRect;
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private float holeWidth;
    [SerializeField] private float holeHeight;

    private void OnDrawGizmos()
    {
        if (holeRect == null) return;
        mainCanvas = FindObjectOfType<Canvas>();
        float scaleFactor = mainCanvas.scaleFactor;
         holeWidth = holeRect.rect.width * scaleFactor;
         holeHeight = holeRect.rect.height * scaleFactor;

        Vector3 center = holeRect.position;

        Gizmos.color = Color.red;
        for (int i = 0; i < 360; i += 10)
        {
            float rad1 = Mathf.Deg2Rad * i;
            float rad2 = Mathf.Deg2Rad * (i + 10);

            Vector3 point1 = new Vector3(
                center.x + Mathf.Cos(rad1) * (holeWidth / 2),
                center.y + Mathf.Sin(rad1) * (holeHeight / 2),
                0);

            Vector3 point2 = new Vector3(
                center.x + Mathf.Cos(rad2) * (holeWidth / 2),
                center.y + Mathf.Sin(rad2) * (holeHeight / 2),
                0);

            Gizmos.DrawLine(point1, point2);
        }
    }
}
