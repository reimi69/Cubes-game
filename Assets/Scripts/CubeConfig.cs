
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Game/GameConfig")]
public class CubeConfig : ScriptableObject
{
    [System.Serializable]
    public struct CubeData
    {
        public string Identifier; 
        public Sprite Image;     
    }

    public List<CubeData> CubeVariants;   
}
