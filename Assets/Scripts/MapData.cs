using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PairInt
{
    public int x;
    public int y;

    public PairInt(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}

[System.Serializable]
public class ThreeInt
{
    public int x;
    public int y;
    public int z;

    public ThreeInt(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }
}

[System.Serializable]
public class MapData
{
    public string appVersion;
    public string mapDesigner;
    public PairInt startIdx;
    public int startLife;
    public int maxLife;
    public int mapWidth;
    public int mapHeight;
    public List<List<string>> floorData;
    public List<List<string>> itemData;
    public Dictionary<PairInt, List<PairInt>> powerData;
    public Dictionary<PairInt, PairInt> portalData;
    public Dictionary<PairInt, ThreeInt> trapData;
}