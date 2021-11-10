using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class MyQuaternion
{
    private float x = 0;
    private float y = 0;
    private float z = 0;
    private float w = 0;
    public static implicit operator Quaternion(MyQuaternion other) => new Quaternion(other.x, other.y, other.z, other.w);
    public static implicit operator MyQuaternion(Quaternion other) => new MyQuaternion(other);

    public MyQuaternion(Quaternion data)
    {
        this.x = data.x;
        this.y = data.y;
        this.z = data.z;
        this.w = data.w;
    }
    public MyQuaternion() : this(Quaternion.identity) { }
}
