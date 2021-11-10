using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class MyVector3
{
    private float x = 0;
    private float y = 0;
    private float z = 0;
    public static implicit operator Vector3(MyVector3 other) => new Vector3(other.x, other.y, other.z);
    public static implicit operator MyVector3(Vector3 other) => new MyVector3(other);

    public MyVector3(Vector3 data)
    {
        this.x = data.x;
        this.y = data.y;
        this.z = data.z;
    }
    public MyVector3() : this(Vector3.zero) { }
}
