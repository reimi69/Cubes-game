using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDataManager
{
    private const string SaveKey = "TowerData";

    [System.Serializable]
    public class CubeDataWithPosition
    {
        public CubeConfig.CubeData CubeData;
        public Vector3 Position;

        public CubeDataWithPosition(CubeConfig.CubeData cubeData, Vector3 position)
        {
            CubeData = cubeData;
            Position = position;
        }
    }
    [System.Serializable]
    public class CubeDataWrapper
    {
        public List<CubeDataWithPosition> CubeDataList;

        public CubeDataWrapper(List<CubeDataWithPosition> cubeDataList)
        {
            CubeDataList = cubeDataList;
        }
    }

    public static void SaveTower(List<Cube> towerCubes)
    {
        List<CubeDataWithPosition> cubeDataWithPositionList = new List<CubeDataWithPosition>();

        foreach (var cube in towerCubes)
        {
            var cubeData = cube.GetCubeData();
            var position = cube.transform.localPosition;
            cubeDataWithPositionList.Add(new CubeDataWithPosition(cubeData, position));
        }

        string json = JsonUtility.ToJson(new CubeDataWrapper(cubeDataWithPositionList));
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();

    }

    public static List<CubeDataWithPosition> LoadTower()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            return new List<CubeDataWithPosition>();
        }

        string json = PlayerPrefs.GetString(SaveKey);
        CubeDataWrapper cubeDataWrapper = JsonUtility.FromJson<CubeDataWrapper>(json);

        return cubeDataWrapper.CubeDataList;
    }

    public static void ClearTowerData()
    {
        PlayerPrefs.DeleteKey(SaveKey);
    }
}
