using Assets.Script.Server;
using Assets.Script.Server.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    private Queue<Command> comands = new Queue<Command>();
    private Dictionary<int, GENERAL.RoomData.ObjectData> objects = new Dictionary<int, GENERAL.RoomData.ObjectData>();

    private void Start()
    {
        //gridMaterial.renderQueue = 0;
        addGridPoint(Vector2.zero);
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

    public int findOrCreateObjectData(GENERAL.RoomData.ObjectData data)
    {
        if (objects.ContainsKey(data.id))
            return data.id;
        int min = 100;
        int max = 1000;
        int k = (int)(min + UnityEngine.Random.value * (max - min));
        while (objects.ContainsKey(k))
        {
            k = (int)(min + UnityEngine.Random.value * (max - min));
            max += 1000;
        }
        objects.Add(k, data);
        return k;
    }
    public int findObjectData(Transform obj, bool createNew = false)
    {
        foreach(var i in objects)
        {
            if (i.Value.wordObject == obj)
                return i.Key;
        }
        if (createNew)
        {
            int min = 100;
            int max = 1000;
            int k = (int)(min + UnityEngine.Random.value * (max - min));
            while (objects.ContainsKey(k))
            {
                k = (int)(min + UnityEngine.Random.value * (max - min));
                max += 1000;
            }
            objects.Add(k, new GENERAL.RoomData.ObjectData(obj));
            return k;
        }
        return int.MinValue;
    }

    public GENERAL.RoomData.ObjectData findObjectData(Transform obj)
    {
        int k = findObjectData(obj, false);
        if (objects.ContainsKey(k))
            return objects[k];
        return null;
    }

    public void updateRoomObject(GENERAL.RoomData.ObjectData data)
    {
        data.wordObject = null;
        var k = findOrCreateObjectData(data);
        var oldData = objects[k];
        if(oldData.wordObject == null)
        {
            var pref = GENERAL.loadPrefab(data.prefLocation);
            if(pref == null)
            {
                objects.Remove(k);
                return;
            }
            data.wordObject = Instantiate(pref);
            data.wordObject.name = data.prefLocation;
        }
        else
        {
            data.wordObject = oldData.wordObject;
        }
        if (data.material != null)
            setObjectTexture(data.wordObject, data.material);
        data.wordObject.position = data.location;
        data.wordObject.rotation = data.rotation;
        data.wordObject.localScale = data.scale;
        objects[k] = data;


        if (data.wordObject.tag == "Object")
            CreateRoom.instance.addObject(data.wordObject);
        else CreateRoom.instance.addCaracter(data.wordObject);
    }

    public void updateRoomObjectAndSendFurther(Transform obj)
    {
        int k = findObjectData(obj, true);
        objects[k] = new GENERAL.RoomData.ObjectData(obj);
        if (obj.tag == "Object")
            CreateRoom.instance.addObject(obj);
        else CreateRoom.instance.addCaracter(obj);

        ClienBehaviour.instace.Send(CommandBuilder.Instance.updateWordObject(obj));
    }

    public void addCommand(Command cmd)
    {
        lock (comands)
            comands.Enqueue(cmd);
    }

    public void FixedUpdate()
    {
        lock (comands)
        {
            while(comands.Count > 0)
            {
                CommandInterpretor.Instance.doCommand(comands.Dequeue());
            }
        }
    }
}
