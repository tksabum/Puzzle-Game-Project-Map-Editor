using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("- Core -")]
    public List<GameObject> floorSets;
    public List<GameObject> floorPrefabs;
    [Space]
    public List<GameObject> itemSets;
    public List<GameObject> itemPrefabs;

    Dictionary<string, GameObject> floorPrfDic;
    Dictionary<string, GameObject> itemPrfDic;
    [Space]
    public Transform floorgroup;
    public Transform itemgroup;

    Dictionary<string, Queue<GameObject>> poolDic = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        // create floorPrfDic
        floorPrfDic = new Dictionary<string, GameObject>();

        for (int i = 0; i < floorSets.Count; i++)
        {
            GameObject floorSet = floorSets[i];
            for (int j = 0; j < floorSet.transform.childCount; j++)
            {
                GameObject floorPrf = floorSet.transform.GetChild(j).gameObject;
                floorPrfDic.Add(floorPrf.name, floorPrf);
            }
        }

        for (int i = 0; i < floorPrefabs.Count; i++)
        {
            GameObject floorPrf = floorPrefabs[i];
            floorPrfDic.Add(floorPrf.name, floorPrf);
        }

        // create itemPrfDic
        itemPrfDic = new Dictionary<string, GameObject>();

        for (int i = 0; i < itemSets.Count; i++)
        {
            GameObject itemSet = itemSets[i];
            for (int j = 0; j < itemSet.transform.childCount; j++)
            {
                GameObject itemPrf = itemSet.transform.GetChild(j).gameObject;
                itemPrfDic.Add(itemPrf.name, itemPrf);
            }
        }

        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            GameObject itemPrf = itemPrefabs[i];
            itemPrfDic.Add(itemPrf.name, itemPrf);
        }


    }

    void CreateNewObject(string objname)
    {
        GameObject prefab = null;
        Transform newtransform = null;

        if (itemPrfDic.ContainsKey(objname))
        {
            prefab = itemPrfDic[objname];
            newtransform = itemgroup;
        }
        else if (floorPrfDic.ContainsKey(objname))
        {
            prefab = floorPrfDic[objname];
            newtransform = floorgroup;
        }
        else
        {
            throw new System.Exception("Error: 맞는 프리펩을 찾지 못했습니다. objname:" + objname);
        }

        GameObject instant = Instantiate(prefab, Vector3.zero, Quaternion.identity, newtransform);
        instant.SetActive(false);
        poolDic[objname].Enqueue(instant);
    }

    public GameObject GetObject(string objname)
    {
        if (!poolDic.ContainsKey(objname))
        {
            poolDic.Add(objname, new Queue<GameObject>());
            CreateNewObject(objname);
        }
        else if(poolDic[objname].Count == 0)
        {
            CreateNewObject(objname);
        }


        GameObject obj = poolDic[objname].Dequeue();
        obj.SetActive(true);
        return obj;
    }
    
    public void ReturnObject(GameObject obj)
    {
        string objname = obj.name.Substring(0, obj.name.Length - 7);
        obj.SetActive(false);
        poolDic[objname].Enqueue(obj);
    }
}
