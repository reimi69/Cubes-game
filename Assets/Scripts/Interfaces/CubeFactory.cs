using UnityEngine;
using UnityEngine.UI;
using Zenject;


public interface ICubeFactory
{
    GameObject CreateCube(Transform parent, CubeConfig.CubeData cubeData);
}

public class CubeFactory : ICubeFactory
{
    [Inject] private readonly GameObject cubePrefab;

    public GameObject CreateCube(Transform parent, CubeConfig.CubeData cubeData)
    {
        var cube = Object.Instantiate(cubePrefab, parent);
        var imageComponent = cube.GetComponentInChildren<Image>();
        if (imageComponent != null)
            imageComponent.sprite = cubeData.Image;

        cube.name = cubeData.Identifier;
        return cube;
    }
}
