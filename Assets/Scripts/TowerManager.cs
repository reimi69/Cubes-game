using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class TowerManager : MonoBehaviour, ITowerManager
{
    [Inject] private readonly GameObject cubePrefab;
    [Inject] private readonly Canvas canvas;
    [Inject] private readonly ICubeFactory cubeFactory;
    [Inject(Id = "TowerArea")] private RectTransform towerArea;

    private IZoneChecker zoneChecker;
    private readonly List<Cube> towerCubes = new List<Cube>();
    private Transform towerBase;

    private float cubeHeight; 
    private float cubeWidth;

    private Subject<GameObject> cubeLoadedSubject = new Subject<GameObject>();
    public IObservable<GameObject> OnCubeLoaded => cubeLoadedSubject;


    private void Start()
    {   
        //TowerDataManager.ClearTowerData();
        (cubeWidth, cubeHeight) = Utils.GetCubeDimensions(cubePrefab, canvas);
        zoneChecker = new ZoneChecker(towerArea, cubeWidth, cubeHeight); 
        towerBase = new GameObject("TowerBase").transform;
        towerBase.SetParent(towerArea, false);

        LoadTower();
    }
    public ITowerManager.CubePlacementStatus TryAddCube(Cube cube, Vector2 dropPosition)
    {
        if (!zoneChecker.IsInTowerZone(dropPosition))
        {
            return ITowerManager.CubePlacementStatus.NotInZone;
        }
        return towerCubes.Count == 0 ? PlaceFirstCube(cube, dropPosition) : PlaceAdditionalCube(cube, dropPosition);
    }
   
  
    private ITowerManager.CubePlacementStatus PlaceFirstCube(Cube cube, Vector2 dropPosition)
    {
        Rect rect = towerArea.rect;
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(towerArea, dropPosition, null, out localPosition);

        if (localPosition.y + cubeHeight > rect.yMax)
        {
            return ITowerManager.CubePlacementStatus.NotInZone;
        }

        AddToTower(cube, dropPosition);
        return ITowerManager.CubePlacementStatus.Success;
    }

    private ITowerManager.CubePlacementStatus PlaceAdditionalCube(Cube cube, Vector2 dropPosition)
    {
        var topCube = towerCubes[towerCubes.Count - 1];
        Vector3 targetPosition = topCube.transform.position;

        float minX = targetPosition.x - cubeWidth / 2;
        float maxX = targetPosition.x + cubeWidth / 2;
        float minY = targetPosition.y + cubeHeight / 2;
        float maxY = targetPosition.y + cubeHeight * 1.5f;

        if (dropPosition.x < minX || dropPosition.x > maxX || dropPosition.y < minY || dropPosition.y > maxY)
        {
            Debug.Log("Кубик не попал на верхний куб башни");
            return ITowerManager.CubePlacementStatus.NotInZone;
        }

        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(towerArea, dropPosition, null, out localPosition);
        Rect rect = towerArea.rect;

        if (localPosition.y + cubeHeight > rect.yMax)
        {
            Debug.Log("Кубик выходит за границу экрана");
            return ITowerManager.CubePlacementStatus.OutOfBounds;
        }

        float randomOffset = Random.Range(-cubeWidth * 0.5f, cubeWidth * 0.5f);
        Vector3 newPosition = new Vector3(targetPosition.x + randomOffset, targetPosition.y + cubeHeight, targetPosition.z);
        AddToTower(cube, newPosition);
        return ITowerManager.CubePlacementStatus.Success;
    }

    private void AddToTower(Cube cube, Vector3 position)
    {
        cube.transform.SetParent(towerBase);
        towerCubes.Add(cube);
        cube.SetPosition(position);
        cube.AnimateAddition();

    }

    public void RemoveFromTower(Cube cube)
    {
        if (towerCubes.Contains(cube))
        {
            int removedIndex = towerCubes.IndexOf(cube);
            towerCubes.Remove(cube);
            Debug.Log($"Кубик {cube.name} удалён из башни");
            UpdateTowerAfterRemoval(removedIndex);
        }
        else
        {
            Debug.LogWarning($"Кубик {cube.name} не найден в башне");
        }

    }

    private void Save()
    {
        TowerDataManager.SaveTower(towerCubes);
    }
    private void OnApplicationQuit()
    {
        Save();
    }
    private void UpdateTowerAfterRemoval(int removedIndex)
    {
        for (int i = removedIndex; i < towerCubes.Count; i++) 
        {
            var currentCube = towerCubes[i];
            var belowCube = i > 0 ? towerCubes[i - 1] : null; 

            Vector3 targetPosition = new Vector3(
                currentCube.transform.position.x,
                currentCube.transform.position.y - cubeHeight,
                currentCube.transform.position.z
            );
            if (belowCube != null && !IsConnected(currentCube, belowCube, targetPosition))
            {
                Debug.Log($"Кубик {currentCube.name} не может упасть: не соприкасается с кубиком ниже");
                AnimateCollapse(i); 
                return;
            }
            currentCube.MoveTo(targetPosition, 0.5f);
        }
    }
    private void AnimateCollapse(int startIndex)
    {
        for (int i = startIndex; i < towerCubes.Count; i++)
        {
            var cube = towerCubes[i];

            cube.AnimateCollapse(cubeWidth, () =>
            {
                towerCubes.Remove(cube);
                cube.DestroyCube();
            });
        }
    }
    private bool IsConnected(Cube currentCube, Cube belowCube, Vector3 targetPosition)
    {
        float belowLeft = belowCube.transform.position.x - cubeWidth / 2;
        float belowRight = belowCube.transform.position.x + cubeWidth / 2;

        float currentLeft = targetPosition.x - cubeWidth / 2;
        float currentRight = targetPosition.x + cubeWidth / 2;

        float overlap = Mathf.Min(belowRight, currentRight) - Mathf.Max(belowLeft, currentLeft);

        return overlap >= cubeWidth * 0.5f;
    }
    public bool ContainsCube(Cube cube)
    {
        return towerCubes.Contains(cube); 
    }

    public void LoadTower()
    {
        var loadedData = TowerDataManager.LoadTower();

        foreach (var dataWithPosition in loadedData)
        {
            var cubeObject = cubeFactory.CreateCube(towerBase, dataWithPosition.CubeData);

            cubeObject.transform.localPosition = dataWithPosition.Position;

            cubeLoadedSubject.OnNext(cubeObject);

            if (!cubeObject.TryGetComponent(out Cube cube))
            {
                Debug.LogError($"Компонент Cube отсутствует у объекта {cubeObject.name}");
                return;
            }

            towerCubes.Add(cube);
        }
    }
}


