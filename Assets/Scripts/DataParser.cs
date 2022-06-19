using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataParser : MonoBehaviour
{
    public TextAsset itemsJSON;
    public TextAsset tfsJSON;


    [System.Serializable]
    public class ItemList
    {
        public Item[] items;
    }

    [System.Serializable]
    public class TfsList
    {
        public Tf[] tfs;
    }

    public ItemList currentItemList = new ItemList();
    public TfsList currentTfsList = new TfsList();

    //    public ItemList GetCurrentItemList(){
    //     return ItemList;
    //    }

    //     public TfsList GetCurrentTfsList(){
    //     return TfsList;
    //    }



    void Awake()
    {
        currentItemList = JsonUtility.FromJson<ItemList>(itemsJSON.text);
        currentTfsList = JsonUtility.FromJson<TfsList>(tfsJSON.text);
    }

    [System.Serializable]
    public class Color
    {
        public int r;
        public int g;
        public int b;
        public float a;
    }

    [System.Serializable]
    public class Rotation
    {
        public float w;
        public float x;
        public float y;
        public float z;
    }

    [System.Serializable]
    public class Position
    {
        public float x;
        public float y;
        public float z;
    }

    [System.Serializable]
    public class Item
    {
        public string id;
        public string shape;
        public string name;
        public string frame;
        public Position position;
        public Rotation rotation;
        public Color color;
        public Scale scale;
        public string transformMode;
        public bool highlighted;
        public bool hidden;
        public string uuid;
        public bool showName;
        public bool wireframe;
    }

    [System.Serializable]
    public class Scale
    {
        public int x;
        public int y;
        public int z;
    }

    [System.Serializable]
    public class Tf
    {
        public string id;
        public string frame;
        public Position position;
        public Rotation rotation;
        public Scale scale;
        public string transformMode;
    }



}