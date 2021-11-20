using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GENERAL;
using System.Linq;
using System;
using Assets.Script.Server.Commands;

[ExecuteInEditMode]
public class CreateRoom : MonoBehaviour
{
    private enum PartToUpdate
    {
        FLOOR,
        WALL,
        ALL
    };
    public static CreateRoom instance;
    private void Awake()
    {
        instance = this;
    }

    public MaterialName[] materials;
    public float uvScale = 1;
    public float vertexOffsetScale = 0;
    public float wallSize = 5;
    public string floorMaterialName;
    public string wallsMaterialName;
    public Transform floorParent;
    public Transform wallParent;
    public Transform objectParent;
    public Transform caracterParent;
    public Transform roomParent;
    public List<List<float[,]>> floor = new List<List<float[,]>>();
    public bool _update = false;
    private float[,] getM(int x, int y, float offset)
    {
        var FirstF = new float[31, 31];
        for (int i = 0; i < FirstF.GetLength(0); i++)
            for (int j = 0; j < FirstF.GetLength(1); j++)
                FirstF[i, j] = Mathf.PerlinNoise((i + x * FirstF.GetLength(0)) * 0.1f + offset,
                                                (j + y * FirstF.GetLength(1)) * 0.1f);
        return FirstF;
    }

    private float getTileValue(int fx, int fy, int x, int y)
    {
        if (fx < floor.Count && fy < floor[fx].Count)
            if(floor[fx][fy] != null && 
                x < floor[fx][fy].GetLength(0) && 
                y < floor[fx][fy].GetLength(1))
                return floor[fx][fy][x, y];
        return -1;
    }


    private float getTileValue(int x, int y)
    {
        int fx = x / 30;
        int fy = y / 30;
        x = x % 30;
        y = y % 30;
        return getTileValue(fx, fy, x, y);
    }

    private Tuple<MeshFilter, MeshRenderer> getMeshRF(int x, int y, Transform parent)
    {
        var tile = parent.Find(x + " " + y);
        MeshFilter mf = null;
        MeshRenderer mr = null;
        if (tile == null)
        {
            GameObject go = new GameObject();
            go.transform.parent = parent;
            go.transform.localPosition = new Vector3(x * 30, 0, y * 30);
            go.name = x + " " + y;

            mf = go.AddComponent<MeshFilter>();
            mr = go.AddComponent<MeshRenderer>();

            WordBehaviour.instance.addGridPoint(go.transform.position);
            WordBehaviour.instance.addGridPoint(go.transform.position + new Vector3(30, 0, 30));
        }
        else
        {
            mf = tile.GetComponent<MeshFilter>();
            mr = tile.GetComponent<MeshRenderer>();
        }
        return new Tuple<MeshFilter, MeshRenderer>(mf, mr);
    }
    private void updateFloorMesh(int x, int y)
    {
        var mesh = CreteMesh.get2Dmesh(floor[x][y], uvScale, vertexOffsetScale, x, y);
        var ret = getMeshRF(x, y, floorParent);
        var mf = ret.Item1;
        var mr = ret.Item2;
        if (mr && mf)
        {
            mf.mesh = mesh;
            mr.sharedMaterial = materials.First(x => x.name == floorMaterialName).material;
        }
    }
    private void updateWallMesh(int x, int y)
    {
        var mesh = CreteMesh.get2Dmesh_wall(floor[x][y], uvScale, vertexOffsetScale, x, y, wallSize);
        var ret = getMeshRF(x, y, wallParent);
        var mf = ret.Item1;
        var mr = ret.Item2;
        if (mr && mf)
        {
            mf.mesh = mesh;
            mr.sharedMaterial = materials.First(x => x.name == wallsMaterialName).material;
        }
    }


    private void updateTileFloor(int x, int y, PartToUpdate part = PartToUpdate.ALL)
    {
        if (floor == null)
            return;
        if (x >= floor.Count && y >= floor[x].Count)
            return;
        if(part == PartToUpdate.ALL || part == PartToUpdate.FLOOR)
            updateFloorMesh(x, y);
        if (part == PartToUpdate.ALL || part == PartToUpdate.WALL)
            updateWallMesh(x, y);
    }
    public void updateAllTiles()
    {
        for (int i = 0; i < floor.Count; i++)
            for (int j = 0; j < floor[i].Count; j++)
                if (floor[i][j] != null)
                    updateTileFloor(i, j, PartToUpdate.ALL);
    }
    public void updateTileFloor(int fx, int fy, float[,] tile)
    {
        while (floor.Count <= fx)
            floor.Add(new List<float[,]>());
        while (floor[fx].Count <= fy)
            floor[fx].Add(null);
        floor[fx][fy] = tile;
        updateTileFloor(fx, fy);
    }
    public void updateTileFloor(int fx, int fy, int x, int y, float value)
    {
        while (floor.Count <= fx)
            floor.Add(new List<float[,]>());
        while (floor[fx].Count <= fy)
            floor[fx].Add(null);
        if (floor[fx][fy] == null)
            floor[fx][fy] = new float[31, 31];

        floor[fx][fy][x, y] = value;
        updateTileFloor(fx, fy);
    }
    public void updateTileFloor(int x, int y, float value)
    {
        int fx = x / 30;
        int fy = y / 30;
        x = x % 30;
        y = y % 30;

        while (floor.Count <= fx)
            floor.Add(new List<float[,]>());
        while (floor[fx].Count <= fy)
            floor[fx].Add(null);
        if (floor[fx][fy] == null)
            floor[fx][fy] = new float[31, 31];

        if(x == 0)
        {
            if(fx > 0)
                updateTileFloor(fx-1, fy, 30, y, value);
        }
        if (y == 0)
        {
            if(fy > 0)
                updateTileFloor(fx, fy - 1, x, 30, value);
        }

        floor[fx][fy][x, y] = value;
        updateTileFloor(fx, fy);
    }

    public void fillTileFloor(int x, int y, float value)
    {
        float sValue = getTileValue(x, y);
        if (sValue < 0)
            sValue = 0;
        if (GENERAL.compareFloat(sValue, value, 0.1f) == 0)
            return;
        int maxSize = 0;
        for (int i = 0; i < floor.Count; i++)
            if (floor[i].Count > maxSize)
                maxSize = floor[i].Count;
        float[][][,] nFloor = new float[floor.Count][][,];
        for (int i = 0; i < nFloor.Length; i++)
            nFloor[i] = new float[maxSize][,];

        int fx = x / 30;
        int fy = y / 30;
        x = x % 30;
        y = y % 30;
        if (fx < 0 || fy < 0 || fx >= nFloor.Length || fy >= maxSize)
            return;
        Queue<MyVector4Int> ps = new Queue<MyVector4Int>();
        ps.Enqueue(new MyVector4Int(fx, fy, x, y));
        int[] dx = { 0, 1, 0, -1};
        int[] dy = { 1, 0, -1, 0};
        for(int steps = 0; steps < 3000 && ps.Count > 0; steps++)
        {
            var p = ps.Dequeue();
            fx = p.x;
            fy = p.y;
            x = p.z;
            y = p.w;

            if(nFloor[fx][fy] == null)
            {
                if (getTileValue(fx, fy, x, y) < 0)
                    nFloor[fx][fy] = new float[31, 31];
                else nFloor[fx][fy] = floor[fx][fy];
            }

            if (GENERAL.compareFloat(sValue, nFloor[fx][fy][x, y], 0.1f) != 0)
            {
                steps--;
                continue;
            }
            if (GENERAL.compareFloat(value, nFloor[fx][fy][x, y], 0.1f) == 0)
            {
                steps--;
                continue;
            }
            nFloor[fx][fy][x, y] = value;
            for (int i = 0; i < dx.Length; i++)
            {
                int fxx = fx;
                int fyy = fy;
                int xx = x + dx[i];
                int yy = y + dy[i];
                if(xx > 30)
                {
                    xx = 0;
                    fxx++;
                }
                if(yy > 30)
                {
                    yy = 0;
                    fyy++;
                }
                if(xx < 0)
                {
                    xx = 30;
                    fxx--;
                }
                if(yy < 0)
                {
                    yy = 30;
                    fyy--;
                }
                if (fxx < 0 || fyy < 0 || fxx >= nFloor.Length || fyy >= maxSize)
                    continue;

                var v = getTileValue(fxx, fyy, xx, yy);
                if(v < 0)
                {
                    v = 0;
                    nFloor[fxx][fyy] = new float[31, 31];
                }
                if(GENERAL.compareFloat(sValue, v) == 0)
                {
                    //nFloor[fxx][fyy][xx, yy] = value;
                    ps.Enqueue(new MyVector4Int(fxx, fyy, xx, yy));
                }
            }
        }
        for (int i = 0; i < nFloor.Length; i++)
            for (int j = 0; j < nFloor[i].Length; j++)
                if (nFloor[i][j] != null)
                    updateTileFloor(i, j, nFloor[i][j]);
    }

    public void addObject(Transform wordObject)
    {
        wordObject.parent = objectParent;
    }
    public void addObject(string objLocation, Vector3 wordPozition, Quaternion wordRotation, Vector3 scale)
    {
        var pref = GENERAL.loadPrefab(objLocation);
        if (pref)
        {
            pref = Instantiate(pref);
            pref.position = wordPozition;
            pref.localScale = scale;
            pref.rotation = wordRotation;
            addObject(pref);
        }
    }
    public void addCaracter(Transform wordCaracter)
    {
        wordCaracter.parent = caracterParent;
    }
    public void addCaracter(string carLocation, Vector3 wordPozition, Quaternion wordRotation, Vector3 scale)
    {
        var pref = GENERAL.loadPrefab(carLocation);
        if (pref)
        {
            pref = Instantiate(pref);
            pref.position = wordPozition;
            pref.localScale = scale;
            pref.rotation = wordRotation;
            addCaracter(pref);
        }
    }
    private void fake(float val = 0)
    {
        if(roomParent == null)
        {
            creteRoom();
            return;
        }
        for (int i = 0; i < floor.Count; i++)
            for (int j = 0; j < floor[i].Count; j++)
                updateTileFloor(i, j, getM(i, j, val * Time.time));
    }

    private void updateAllParts(PartToUpdate part = PartToUpdate.ALL)
    {
        for (int i = 0; i < floor.Count; i++)
            for (int j = 0; j < floor[i].Count; j++)
                if (floor[i][j] != null)
                    updateTileFloor(i, j, part);
    }

    public void setWallHeights(float val)
    {
        this.wallSize = val;
        updateAllParts(PartToUpdate.WALL);
    }

    public void setUVSize(float val)
    {
        this.uvScale = val;
        updateAllParts(PartToUpdate.ALL);
    }

    public void setVertecisOffset(float val)
    {
        this.vertexOffsetScale = val;
        updateAllParts(PartToUpdate.ALL);
    }

    public void changeWallMaterialName(string name)
    {
        int indx = -1;
        for(int i=0;i<materials.Length;i++)
            if(materials[i].name == name)
            {
                indx = i;
                break;
            }
        if (indx < 0)
            return;

        MeshRenderer[] fs = wallParent.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < fs.Length; i++)
            fs[i].sharedMaterial = materials[indx].material;
        wallsMaterialName = name;
    }
    public void changeFloorMaterialName(string name)
    {
        int indx = -1;
        for (int i = 0; i < materials.Length; i++)
            if (materials[i].name == name)
            {
                indx = i;
                break;
            }
        if (indx < 0)
            return;

        MeshRenderer[] fs = floorParent.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < fs.Length; i++)
            fs[i].sharedMaterial = materials[indx].material;
        floorMaterialName = name;
    }

    public void creteRoom(string newRoomName = "room")
    {
        if (roomParent)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
                DestroyImmediate(roomParent.gameObject);
            else
                Destroy(roomParent.gameObject);
        }
        floor = new List<List<float[,]>>();
        roomParent = (new GameObject(newRoomName)).transform;
        roomParent.parent = transform;
        roomParent.localPosition = Vector3.zero;

        floorParent = (new GameObject("floor")).transform;
        floorParent.parent = roomParent;
        floorParent.localPosition = Vector3.zero;

        wallParent = (new GameObject("wall")).transform;
        wallParent.parent = roomParent;
        wallParent.localPosition = Vector3.zero;

        objectParent = (new GameObject("objects")).transform;
        objectParent.parent = roomParent;
        objectParent.localPosition = Vector3.zero;

        caracterParent = (new GameObject("caracters")).transform;
        caracterParent.parent = roomParent;
        caracterParent.localPosition = Vector3.zero;
        //fake();
    }
    public void selectRoom(string roomName)
    {
        if (roomParent.name == roomName)
            return;
        roomParent = transform.Find(roomName);
        if(roomParent == null)
        {
            creteRoom(roomName);
            return;
        }
        floorParent = roomParent.Find("floor");
        wallParent = roomParent.Find("wall");
        objectParent = roomParent.Find("objects");
        caracterParent = roomParent.Find("caracters");
    }
    public void creteNewRoom()
    {
        roomParent = null;
        creteRoom("room" + transform.childCount);
    }
    private void Start()
    {
        creteRoom();
    }

    void Update()
    {
        if (_update)
        {
            fake();
            _update = false;
        }
    }

    public GENERAL.RoomData getData(bool getFloorData = true, bool getObjectsData = true, bool getCaractersData = true)
    {
        var data = new GENERAL.RoomData();
        data.roomName = roomParent.name;
        data.floorMaterial = floorMaterialName;
        data.wallMaterial = wallsMaterialName;
        data.uvScale = uvScale;
        data.wallHeight = wallSize;
        data.verOffset = vertexOffsetScale;

        if (getFloorData)
        {
            var fl = new float[floor.Count][][,];
            for (int i = 0; i < fl.Length; i++)
                fl[i] = floor[i].ToArray();
            data.floor = fl;
        }
        List<GENERAL.RoomData.ObjectData> objs = new List<RoomData.ObjectData>();
        if (getObjectsData)
        {
            foreach(Transform ch in objectParent)
            {
                var d = WordBehaviour.instance.findObjectData(ch, true);
                if (d != null)
                    objs.Add(d);
            }
        }

        if (getCaractersData)
        {
            foreach (Transform ch in caracterParent)
            {
                var d = WordBehaviour.instance.findObjectData(ch, true);
                if (d != null)
                    objs.Add(d);
            }
        }

        if (objs.Count > 0)
            data.objects = objs.ToArray();

        return data;
    }

    public void updateRoom(GENERAL.RoomData data)
    {
        selectRoom(data.roomName);
        floorMaterialName = data.floorMaterial;
        wallsMaterialName = data.wallMaterial;
        uvScale = data.uvScale;
        wallSize = data.wallHeight;
        vertexOffsetScale = data.verOffset;
        if (data.floor != null)
        {
            floor = new List<List<float[,]>>();
            for (int i = 0; i < data.floor.Length; i++)
            {
                var newL = new List<float[,]>(data.floor[i]);
                floor.Add(newL);
            }
        }
        if(data.objects != null)
        {
            foreach (var d in data.objects)
                WordBehaviour.instance.updateRoomObject(d);
        }
        updateAllTiles();
    }

    public void updateFloorWallsToPlayers()
    {
        for(int i=0;i<floor.Count;i++)
            for(int j=0;j<floor[i].Count;j++)
                if(floor[i][j] != null)
                {
                    var cmd = CommandBuilder.Instance.updateFloor(roomParent.name, i, j, floor[i][j]);
                    ClienBehaviour.instace.Send(cmd);
                }
    }
}
