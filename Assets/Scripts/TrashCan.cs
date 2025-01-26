using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TrashCan : IInitializable
{
    [Inject] private TowerManager towerManager;
    [Inject(Id = "TrashCan")] private RectTransform holeRect;

    [Inject] private readonly Canvas canvas;
    [Inject] private readonly GameObject cubePrefab;

    private float holeWidth;
    private float holeHeight;
    private float cubeWidth;
    private float cubeHeight;

    [Inject]
    public void Initialize()
    {
        float scaleFactor = canvas.scaleFactor;
        holeWidth = holeRect.rect.width * scaleFactor;
        holeHeight = holeRect.rect.height * scaleFactor;
        (cubeWidth, cubeHeight) = Utils.GetCubeDimensions(cubePrefab, canvas);
    }
    private bool IsPointInsideHole(Vector2 point)
    {
        Vector2 normalizedPoint = new Vector2(point.x / (holeWidth / 2), point.y / (holeHeight / 2));
        return (normalizedPoint.x * normalizedPoint.x + normalizedPoint.y * normalizedPoint.y) <= 1;
    }
    public bool IsOverHole(Vector3 position)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(holeRect, position, null, out localPosition);

        if (IsPointInsideHole(localPosition))
        {
            return true;
        }

        float halfWidth = cubeWidth / 2;
        float halfHeight = cubeHeight / 2;

        Vector2[] corners = new Vector2[]
        {
            new Vector2(localPosition.x - halfWidth, localPosition.y - halfHeight),
            new Vector2(localPosition.x + halfWidth, localPosition.y - halfHeight),
            new Vector2(localPosition.x - halfWidth, localPosition.y + halfHeight),
            new Vector2(localPosition.x + halfWidth, localPosition.y + halfHeight)
        };

        foreach (var corner in corners)
        {
            if (IsPointInsideHole(corner))
            {
                return true;
            }
        }

        return false;
    }

    public void RemoveCube(GameObject cube)
    {
        Cube cubeComponent = cube.GetComponent<Cube>();
        if (cubeComponent == null) return;

        if (towerManager.ContainsCube(cubeComponent))
        {
            towerManager.RemoveFromTower(cubeComponent);
        }
        cubeComponent.AnimateToTrashCan(holeRect, 0.5f);

        Debug.Log("TrashCan. Кубик удалён в дыру после анимации");
    }

}