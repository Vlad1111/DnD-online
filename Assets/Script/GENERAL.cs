using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class GENERAL
{
    public struct FileLoactions{
        public const string prefabs = "Objects";
    }
    public enum FileType
    {
        DIRECTORY,
        UNITY_META,
        FILES,
        FILES_WITH_EXTENSION,
        FILE_WITHOUT_META,
        ALL,
        ALL_WITHOUT_META,
        PREFAB
    }
    [System.Serializable]
    public class MaterialName
    {
        public string name;
        public Material material;
    }
    [System.Serializable]
    public class MyVector4Int
    {
        public int x, y, z, w;
        public MyVector4Int(int x, int y, int z, int w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }
    [System.Serializable]
    public class RoomData
    {
        [System.Serializable]
        public class ObjectData
        {
            public string prefLocation;
            public Vector3 location;
            public Quaternion rotation;
            public Vector3 scale;
        }
        public string roomName;
        public Vector3 offset;
        public float[][][,] floor;
        public float verOffset;
        public float uvScale;
        public float wallHeight;
        public string floorMaterial;
        public string wallMaterial;
    }
    [System.Serializable]
    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        FRONT,
        BACK
    }
    public static int compareFloat(float x, float y, float delta = 0.01f)
    {
        float d = x - y;
        if (-delta < d && d < delta)
            return 0;
        if (d < 0)
            return -1;
        return 1;
    }

    public static Transform loadPrefab(string prefabLocation)
    {
        Transform rez = Resources.Load<Transform>(FileLoactions.prefabs + "/" + prefabLocation) as Transform;
        return rez;
    }
    public static string[] getFiles(string location, FileType ft = FileType.ALL_WITHOUT_META, string extension = null, bool useResources = true)
    {
        if (useResources)
            location = Application.dataPath + "/Resources/" + location;
        if (!Directory.Exists(location))
            return new string[] { };
        DirectoryInfo d = new DirectoryInfo(location);
        List<string> rez = new List<string>();
        if (ft != FileType.DIRECTORY)
        {
            if(extension == null)
            {
                if (ft == FileType.UNITY_META)
                {
                    extension = "*.meta";
                }
                else if (ft == FileType.PREFAB)
                {
                    extension = "*.prefab";
                }
                else
                    extension = "";
            }
            FileInfo[] Files = d.GetFiles(extension);

            foreach (FileInfo file in Files)
            {
                string n = file.Name;
                if (ft == FileType.ALL_WITHOUT_META || ft == FileType.FILE_WITHOUT_META)
                {
                    if (n.EndsWith(".meta"))
                        continue;
                }
                else if(ft != FileType.FILES_WITH_EXTENSION)
                    n = n.Split('.')[0];
                rez.Add(n);
            }
        }
        if ((new[] { FileType.ALL, FileType.ALL_WITHOUT_META, FileType.DIRECTORY }).Contains(ft))
        {
            DirectoryInfo[] Files = d.GetDirectories();

            foreach (DirectoryInfo dir in Files)
            {
                rez.Add(dir.Name);
            }
        }
        return rez.ToArray();
    }

    public static byte[] SerilizeObject<T>(T obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public static T DeserilizeObject<T>(byte[] data)
    {
        using (var memStream = new MemoryStream())
        {
            var binForm = new BinaryFormatter();
            memStream.Write(data, 0, data.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return (T)obj;
        }
    }
}
