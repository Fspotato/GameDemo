using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class MapManager : BaseManager<MapManager>
{
    public MapConfig config;

    public MapUI mapUI;

    [SerializeField] public Map CurrentMap;

    public void LoadMap()
    {
        if (File.Exists(Application.persistentDataPath + "/map.json"))
        {
            var map = JsonUtility.FromJson<Map>(File.ReadAllText(Application.persistentDataPath + "/map.json"));
            if (map.path.Any(p => p.Equals(map.GetBoss().point)))
            {
                GenerateNewMap();
            }
            else
            {
                CurrentMap = map;
                mapUI.ShowMap(map);
                mapUI.SetAttainableNodes();
            }
        }
        else
        {
            GenerateNewMap();
        }
    }

    public void GenerateNewMap()
    {
        var map = MapGenerator.GetMap(config);
        CurrentMap = map;
        mapUI.ShowMap(map);
        mapUI.SetAttainableNodes();
    }

    public void SaveMap()
    {
        File.WriteAllText(Application.persistentDataPath + "/map.json", CurrentMap.ToJson());
    }

    public void EnterNode()
    {
        GetComponent<MapPlayerCtrl>().EnterNode();
    }
}
