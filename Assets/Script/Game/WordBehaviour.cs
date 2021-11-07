using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordBehaviour : MonoBehaviour
{
    public static WordBehaviour instance;
    private void Awake()
    {
        instance = this;
    }
    public CreateRoom createRoom;
    public Light sun;
    public Transform grid;
    public Material gridMaterial;

    private Vector2 grid_minPoint = Vector2.zero;
    private Vector2 grid_maxPoint = Vector2.one * 10;

    public ServerSocket server;

    private void Start()
    {
        //gridMaterial.renderQueue = 0;
        addGridPoint(Vector2.zero);
        server = ServerSocket.getInstance(42069);
    }

    public void setSunPower(float val)
    {
        sun.intensity = val;
    }

    public void addGridPoint(Vector3 point)
    {
        if (point.x < grid_minPoint.x)
            grid_minPoint.x = point.x;
        if (point.z < grid_minPoint.y)
            grid_minPoint.y = point.z;
        if (point.x > grid_maxPoint.x)
            grid_maxPoint.x = point.x;
        if (point.z > grid_maxPoint.y)
            grid_maxPoint.y = point.z;
        grid.position = new Vector3((grid_maxPoint.x + grid_minPoint.x) / 2, 0, (grid_maxPoint.y + grid_minPoint.y) / 2);
        grid.localScale = new Vector3(grid_maxPoint.x - grid_minPoint.x, grid_maxPoint.y - grid_minPoint.y, 0);
    }

    public void setObjectTexture(Transform selectedObject, string textureName)
    {
        Material m = null;
        foreach(var v in  CreateRoom.instance.materials)
            if(v.name == textureName)
            {
                m = v.material;
                break;
            }
        if (m == null)
            return;
        var rend = selectedObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var r in rend)
        {
            var mats = new Material[r.sharedMaterials.Length];
            for (int i = 0; i < mats.Length; i++)
                mats[i] = m;
            r.sharedMaterials = mats;
        }
    }

    public void rotateObject(Transform selectedObject, Vector3 rotation)
    {
        selectedObject.rotation *= Quaternion.Euler(rotation);
    }

    public void moveObject(Transform selectedObject, Vector3 distance)
    {
        selectedObject.position += distance;
    }

    public void resizeObject(Transform selectedObject, Vector3 delta)
    {
        Vector3 scale = selectedObject.localScale;
        scale.x *= 1 + delta.x;
        scale.y *= 1 + delta.y;
        scale.z *= 1 + delta.z;
        selectedObject.localScale = scale;
    }
}
