using TMPro;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private CubeConfig cubeConfig; 
    [SerializeField] private RectTransform trashCanRect;
    [SerializeField] private Transform cubeParent;
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private GameObject cubePrefab; 
    [SerializeField] private RectTransform towerArea; 
    [SerializeField] private TMP_Text messageText; 

    public override void InstallBindings()
    {
        Container.Bind<TMP_Text>().FromInstance(messageText).AsSingle();

        Container.Bind<Canvas>().FromInstance(mainCanvas).AsSingle();

        Container.Bind<GameObject>().FromInstance(cubePrefab).AsSingle();

        Container.Bind<CubeConfig>().FromInstance(cubeConfig).AsSingle();

        Container.Bind<MessageManager>().FromComponentInHierarchy().AsSingle();

        Container.Bind<ICubeFactory>().To<CubeFactory>().AsSingle();

        Container.Bind<Transform>().WithId("ÑubeParent").FromInstance(cubeParent).AsSingle();

        Container.Bind<CubeManager>().FromComponentInHierarchy().AsSingle();

        Container.Bind<RectTransform>().WithId("TowerArea").FromInstance(towerArea).AsCached();

        Container.Bind<RectTransform>().WithId("TrashCan").FromInstance(trashCanRect).AsCached();

        Container.Bind<TowerManager>().FromComponentInHierarchy().AsSingle();

        Container.Bind<TrashCan>().AsSingle();
    }
}
