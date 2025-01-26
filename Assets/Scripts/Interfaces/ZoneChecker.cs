
using UnityEngine;

public interface ITowerManager
{
    public enum CubePlacementStatus
    {
        Success,       
        OutOfBounds,  
        NotInZone     
    }
    CubePlacementStatus TryAddCube(Cube cube, Vector2 dropPosition);
    void RemoveFromTower(Cube cube);
}

public interface IZoneChecker
{
    bool IsInTowerZone(Vector2 dropPosition);
}

public class ZoneChecker : IZoneChecker
{
    private RectTransform towerArea;
    private float cubeWidth;
    private float cubeHeight;

    public ZoneChecker(RectTransform towerArea, float cubeWidth, float cubeHeight)
    {
        this.towerArea = towerArea; 
        this.cubeWidth = cubeWidth;
        this.cubeHeight = cubeHeight;
    }

    public bool IsInTowerZone(Vector2 dropPosition)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(towerArea, dropPosition, null, out localPosition);
        Rect rect = towerArea.rect;
        float left = localPosition.x - cubeWidth;
        float right = localPosition.x + cubeWidth;
        float bottom = localPosition.y - cubeHeight;
        float top = localPosition.y + cubeHeight;

        return left >= rect.xMin &&
               right <= rect.xMax &&
               bottom >= rect.yMin;
               //&& top <= rect.yMax;
    }
}
