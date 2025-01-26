using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CubeManager : MonoBehaviour
{
    [Inject] private readonly CubeConfig cubeConfig;
    [Inject] private readonly TrashCan trashCan;
    [Inject] private readonly TowerManager towerManager;
    [Inject] private readonly ICubeFactory cubeFactory;
    [Inject] private readonly Canvas canvas;
    [Inject] private readonly MessageManager messageManager;
    [Inject(Id = "СubeParent")] private readonly Transform cubeParent;
    private void Start()
    {
        InitializeCubes();

        towerManager.OnCubeLoaded
            .Subscribe(cubeObject =>
            SetupDragAndDrop(cubeObject)).AddTo(this);

        Observable.NextFrame() 
           .Subscribe(_ =>
           {
               DisableContent();
           })
           .AddTo(this); 
    }
    private void InitializeCubes()
    {
        foreach (var cubeData in cubeConfig.CubeVariants)
        {
            var cube = cubeFactory.CreateCube(cubeParent, cubeData);
            SetupDragAndDrop(cube);
        }
    }
    private void DisableContent()
    {
        cubeParent.GetComponent<ContentSizeFitter>().enabled = false;
        cubeParent.GetComponent<HorizontalLayoutGroup>().enabled = false;
    }
    private void SetupDragAndDrop(GameObject cube)
    {
        if (!cube.TryGetComponent(out DragHandler dragHandler))
        {
            Debug.LogError($"Компонент DragHandler отсутствует у объекта {cube.name}");
            return;
        }

        dragHandler.OnDrop
            .Subscribe(dropPosition => HandleDrop(cube, dropPosition))
            .AddTo(this);
    }
    private void HandleDrop(GameObject cube, Vector2 position)
    {
        if (!cube.TryGetComponent(out Cube originalCube))
        {
            Debug.LogError($"Компонент Cube отсутствует у объекта {cube.name}");
            return;
        }

        if (towerManager.ContainsCube(originalCube))
        {
            HandleExistingCube(cube, position);
        }
        else
        {
            HandleNewCube(cube, position);
        }
    }

    private void HandleExistingCube(GameObject cube, Vector2 position)
    {
        if (trashCan.IsOverHole(position))
        {
            trashCan.RemoveCube(cube);
            messageManager.ShowMessage("CubeRemoved");
        }
    }

    private void HandleNewCube(GameObject cube, Vector2 position)
    {
        var clonedCube = cubeFactory.CreateCube(canvas.transform, cube.GetComponent<Cube>().GetCubeData());
        var clonedCubeComponent = clonedCube.GetComponent<Cube>();
        SetupDragAndDrop(clonedCube);

        var placementStatus = towerManager.TryAddCube(clonedCubeComponent, position);

        switch (placementStatus)
        {
            case ITowerManager.CubePlacementStatus.Success:
                clonedCubeComponent.ResetScale();
                messageManager.ShowMessage("CubePlaced");
                break;

            case ITowerManager.CubePlacementStatus.OutOfBounds:
                clonedCubeComponent.SetPosition(position);
                clonedCubeComponent.DestroyCube();
                messageManager.ShowMessage("HeightLimit"); 
                break;

            case ITowerManager.CubePlacementStatus.NotInZone:
                if (trashCan.IsOverHole(position))
                {
                    trashCan.RemoveCube(clonedCube);
                    messageManager.ShowMessage("CubeRemoved");
                }
                else
                {
                    clonedCubeComponent.SetPosition(position);
                    clonedCubeComponent.DestroyCube();
                    messageManager.ShowMessage("CubeFell");

                }
                break;
        }

    }
}



