using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "__selectObject", menuName = "On Camera Draw Objects/Select Object To Move")]
public class OCD_SelectObject : OCD_PlaceObject
{
    public override string getName() { return "Select object tool"; }
    private float deleteButon = -1;
    public override void draw(Camera camera, Transform cursor, object[] args)
    {
        if (Input.GetAxis("Fire1") > 0)
        {
            RaycastHit hit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform obj = hit.transform;
                if (obj.tag == "Object" || obj.tag == "Caracter")
                {
                    cursor.position = hit.point;
                    if (Input.GetAxis("Run") > 0)
                    {
                        if(deleteButon < 0)
                            Destroy(obj.gameObject);
                        deleteButon = 1;
                    }
                    else if (OnCameraDraw.instance)
                    {
                        var ocds = OnCameraDraw.instance.cameraDraws;
                        for (int i = 0; i < ocds.Length; i++)
                        {
                            if (ocds[i].getName() == "Place object tool")
                            {
                                try
                                {
                                    var ocd = (OCD_PlaceObject)(ocds[i]);
                                    ocd.selectedObject = obj;
                                    ocd.distance = hit.distance;
                                    obj.parent = cursor;
                                }
                                catch (InvalidCastException err)
                                {
                                    Debug.LogWarning(err);
                                }
                                OnCameraDraw.instance.selectOCD(i);
                            }
                        }
                        deleteButon = -1;
                    }
                    else
                        deleteButon = -1;
                }
            }
        }
    }
    public override void onDeselect(Camera camera, Transform cursor, object[] args)
    {
        var mr = cursor.GetComponent<MeshRenderer>();
        if (mr)
            mr.enabled = true;
    }

    public override void onSelect(Camera camera, Transform cursor, object[] args)
    {
        var mr = cursor.GetComponent<MeshRenderer>();
        if (mr)
            mr.enabled = false;
    }
}
