using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CreteMesh
{
    private static Vector3[] __v = new Vector3[]
    {
        new Vector3(0, 0, 1),
        new Vector3(0, 0, 0.5f),
        new Vector3(0, 0, 0),
        new Vector3(0.5f, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 0, 0.5f),
        new Vector3(1, 0, 1),
        new Vector3(0.5f, 0, 1),

        new Vector3(0, 1, 1),
        new Vector3(0, 1, 0.5f),
        new Vector3(0, 1, 0),
        new Vector3(0.5f, 1, 0),
        new Vector3(1, 1, 0),
        new Vector3(1, 1, 0.5f),
        new Vector3(1, 1, 1),
        new Vector3(0.5f, 1, 1),
    };
    private static int[][] __t = new int[][]
    {
        /*00*/new int[]{ },
        /*01*/new int[]{ 1, 0, 7},
        /*02*/new int[]{ 3, 2, 1},
        /*03*/new int[]{ 2, 0, 3, 0, 7, 3},
        /*04*/new int[]{ 5, 4, 3},
        /*05*/new int[]{ 4, 3, 5, 5, 3, 1, 1, 7, 5, 1, 0, 7},
        /*06*/new int[]{ 2, 1, 4, 1, 5, 4},
        /*07*/new int[]{ 4, 2, 0, 4, 0, 5, 5, 0, 7},
        /*08*/new int[]{ 5, 7, 6},
        /*09*/new int[]{ 5, 1, 0, 5, 0, 6},
        /*10*/new int[]{ 3, 2, 1, 3, 1, 5, 1, 7, 5, 5, 7, 6},
        /*11*/new int[]{ 3, 2, 5, 5, 2, 6, 6, 2, 0},
        /*12*/new int[]{ 4, 3, 7, 4, 7, 6},
        /*13*/new int[]{ 4, 3, 0, 3, 1, 0, 4, 0, 6},
        /*14*/new int[]{ 4, 2, 6, 6, 2, 1, 1, 7, 6},
        /*15*/new int[]{ 4, 2, 0, 4, 0, 6},
    };
    private static int[][] __t_wall = new int[][]
    {
        /*00v*/new int[]{ },
        /*01v*/new int[]{ 1, 0, 7, 15, 9, 1, 7, 15, 1},
        /*02v*/new int[]{ 3, 2, 1, 9, 11, 3, 9, 3, 1},
        /*03v*/new int[]{ 2, 0, 3, 0, 7, 3, 15, 11, 3, 15, 3, 7},
        /*04v*/new int[]{ 5, 4, 3, 11, 13, 5, 11, 5, 3},
        /*05?*/new int[]{ 4, 3, 5, 5, 3, 1, 1, 7, 5, 1, 0, 7, 7, 15, 13, 5, 7, 13, 3, 11, 9, 1, 3, 9},
        /*06v*/new int[]{ 2, 1, 4, 1, 5, 4, 9, 13, 5, 9, 5, 1},
        /*07v*/new int[]{ 4, 2, 0, 4, 0, 5, 5, 0, 7, 7, 15, 13, 5, 7, 13},
        /*08v*/new int[]{ 5, 7, 6, 13, 15, 7, 13, 7, 5},
        /*09v*/new int[]{ 5, 1, 0, 5, 0, 6, 5, 13, 9, 1, 5, 9},
        /*10?*/new int[]{ 3, 2, 1, 3, 1, 5, 1, 7, 5, 5, 7, 6, 1, 9, 15, 1, 15, 7, 5, 13, 11, 3, 5, 11},
        /*11v*/new int[]{ 3, 2, 5, 5, 2, 6, 6, 2, 0, 5, 13, 11, 3, 5, 11},
        /*12v*/new int[]{ 4, 3, 7, 4, 7, 6, 3, 11, 15, 7, 3, 15},
        /*13v*/new int[]{ 4, 3, 0, 3, 1, 0, 4, 0, 6, 3, 11, 9, 1, 3, 9},
        /*14v*/new int[]{ 4, 2, 6, 6, 2, 1, 1, 7, 6, 1, 9, 15, 1, 15, 7},
        /*15v*/new int[]{ 4, 2, 0, 4, 0, 6},
    };
    private static int findPointindex(List<Vector3> points, Vector3 point, float delta = 0.001f)
    {
        for(int i = 0; i < points.Count; i++)
        {
            if (GENERAL.compareFloat(points[i].x, point.x) != 0)
                continue;
            if (GENERAL.compareFloat(points[i].y, point.y) != 0)
                continue;
            if (GENERAL.compareFloat(points[i].z, point.z) != 0)
                continue;
            return i;
        }
        return -1;
    }

    private static void setVertesesUv(List<Vector3> vertices, List<Vector2> uvs, float offx, float offy, float uvScale, float vertexOffset)
    {
        for(int i=0;i<vertices.Count;i++)
        {
            var v = vertices[i];
            float px = (offx + v.x) * 1.0f;
            float py = (offy + v.z) * 1.0f;
            float dx = 0.5f - Mathf.PerlinNoise(px, py);
            float dy = 0.5f - Mathf.PerlinNoise(px + 416.3f, py + 541.3f);

            uvs.Add(new Vector2((offx + v.x) * uvScale + 0.1f * dx * vertexOffset, (offy + v.z - v.y) * uvScale + 0.1f * dy * vertexOffset));

            //px = (offx + v.x) * 1.2f;
            //py = (offy + v.z) * 1.2f;
            //dx = 0.5f - Mathf.PerlinNoise(px, py);
            //dy = 0.5f - Mathf.PerlinNoise(px + 416.3f, py + 541.3f);

            vertices[i] += new Vector3(dx * vertexOffset, 0, dy * vertexOffset);
        }
    }

    public static Mesh get2Dmesh_1(float[,] points)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int i=0;i<points.GetLength(0) - 1; i++)
            for(int j=0;j<points.GetLength(1) - 1; j++)
            {
                if (points[i, j] < 0.5f)
                    continue;
                Vector3 p00 = new Vector3(i, 0, j);
                Vector3 p01 = new Vector3(i, 0, j + 1);
                Vector3 p10 = new Vector3(i + 1, 0, j);
                Vector3 p11 = new Vector3(i + 1, 0, j + 1);
                int i00 = findPointindex(vertices, p00);
                if(i00 < 0)
                {
                    i00 = vertices.Count;
                    vertices.Add(p00);
                }
                int i10 = findPointindex(vertices, p10);
                if (i10 < 0)
                {
                    i10 = vertices.Count;
                    vertices.Add(p10);
                }
                int i01 = findPointindex(vertices, p01);
                if (i01 < 0)
                {
                    i01 = vertices.Count;
                    vertices.Add(p01);
                }
                int i11 = findPointindex(vertices, p11);
                if (i11 < 0)
                {
                    i11 = vertices.Count;
                    vertices.Add(p11);
                }
                triangles.AddRange(new int[] { i00, i01, i10 });
                triangles.AddRange(new int[] { i01, i11, i10 });
            }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        return mesh;
    }

    public static Mesh get2Dmesh(float[,] points, float uvScale = 1, float vertexOffset = 0, int wordOffx = 0, int wordOffy = 0)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int i = 0; i < points.GetLength(0) - 1; i++)
            for (int j = 0; j < points.GetLength(1) - 1; j++)
            {
                int i00, i10, i01, i11;
                i00 = points[i, j] > 0.4 ?  2 : 0;
                i10 = points[i+1, j] > 0.4 ? 4 : 0;
                i01 = points[i, j+1] > 0.4 ? 1 : 0;
                i11 = points[i+1, j+1] > 0.4 ? 8 : 0;
                int inx = i00 + i10 + i01 + i11;
                var tris = __t[inx].Clone() as int[];
                var vexs = __v.Clone() as Vector3[];
                for(int k=0;k<vexs.Length && k < 8;k++)
                {
                    vexs[k].x += i;
                    vexs[k].z += j;
                }    
                for(int k = 0; k < tris.Length; k++)
                {
                    int ti = tris[k];
                    ti = findPointindex(vertices, vexs[ti]);
                    if(ti < 0)
                    {
                        ti = vertices.Count;
                        vertices.Add(vexs[tris[k]]);
                    }
                    tris[k] = ti;
                }
                triangles.AddRange(tris);
            }
        var uvs = new List<Vector2>();
        var normals = new List<Vector3>();
        for (int i = 0; i < vertices.Count; i++)
            normals.Add(Vector3.up);
        float offx = (points.GetLength(0) - 1) * wordOffx + 0.2f;
        float offy = (points.GetLength(1) - 1) * wordOffy + 0.2f;
        setVertesesUv(vertices, uvs, offx, offy, uvScale, vertexOffset);
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.normals = normals.ToArray();
        return mesh;
    }

    public static Mesh get2Dmesh_wall(float[,] points, float uvScale = 1, float vertexOffset = 0, int wordOffx = 0, int wordOffy = 0, float wallsSize = 5)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int i = 0; i < points.GetLength(0) - 1; i++)
            for (int j = 0; j < points.GetLength(1) - 1; j++)
            {
                int i00, i10, i01, i11;
                i00 = 0.4 >= points[i, j] && points[i, j] > 0.1 ? 2 : 0;
                i10 = 0.4 >= points[i + 1, j] && points[i + 1, j] > 0.1 ? 4 : 0;
                i01 = 0.4 >= points[i, j + 1] && points[i, j + 1] > 0.1 ? 1 : 0;
                i11 = 0.4 >= points[i + 1, j + 1] && points[i + 1, j + 1] > 0.1 ? 8 : 0;
                int inx = i00 + i10 + i01 + i11;
                var tris = __t_wall[inx].Clone() as int[];
                var vexs = __v.Clone() as Vector3[];
                for (int k = 0; k < vexs.Length; k++)
                {
                    vexs[k].x += i;
                    vexs[k].y *= wallsSize;
                    vexs[k].z += j;
                }
                for (int k = 0; k < tris.Length; k++)
                {
                    tris[k] = (tris[k] + 8) % vexs.Length;
                    int ti = tris[k];
                    ti = findPointindex(vertices, vexs[ti]);
                    if (ti < 0)
                    {
                        ti = vertices.Count;
                        vertices.Add(vexs[tris[k]]);
                    }
                    tris[k] = ti;
                }
                triangles.AddRange(tris);
            }
        var uvs = new List<Vector2>();
        float offx = (points.GetLength(0) - 1) * wordOffx + 0.2f;
        float offy = (points.GetLength(1) - 1) * wordOffy + 0.2f;
        setVertesesUv(vertices, uvs, offx, offy, uvScale, vertexOffset);
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}
