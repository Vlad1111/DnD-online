using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class MyVector3
{
    private Vector3 data;
    public static implicit operator Vector3(MyVector3 other) => other.data;
    public static implicit operator MyVector3(Vector3 other) => new MyVector3(other);

    public MyVector3(Vector3 data)
    {
        this.data = data;
    }
    public MyVector3()
    {
        this.data = Vector3.zero;
    }
}
