using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "__placeObject", menuName = "On Camera Draw Objects/Place object")]
public class OCD_PlaceObject : OCD_BaseDraw
{
    public bool singlePlaceObject = false;
    private string objectPrefabLocation;
    private bool click = false;
    public float distance = 1;
    public override string getName() { return "Place object tool"; }
    public override void draw(Camera camera, Transform cursor, object[] args)
    {
        if (!singlePlaceObject)
        {
            if (RoomMenuBehaviour.instance)
                objectPrefabLocation = RoomMenuBehaviour.instance.getPropName();
            for (int i = 1; i < cursor.childCount; i++)
                Destroy(cursor.GetChild(i).gameObject);
            if (cursor.childCount <= 0 || cursor.GetChild(0).name != objectPrefabLocation)
            {
                if (cursor.childCount > 0)
                    Destroy(cursor.GetChild(0).gameObject);
                var pref = GENERAL.loadPrefab(objectPrefabLocation);
                if (pref)
                {
                    Instantiate(pref, cursor).name = objectPrefabLocation;
                }
                //else Debug.LogWarning("PREFAB: " + objectPrefabLocation + " does not exist you fucker!");
            }
        }
        var dir = camera.ScreenPointToRay(Input.mousePosition).direction;
        Vector3 poz = camera.transform.position;
        poz += dir * distance;


        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Transform obj = null;
        if (cursor.childCount > 0)
        {
            obj = cursor.GetChild(0);
            obj.eulerAngles += new Vector3(0, Input.GetAxis("Depth"), 0) * Time.deltaTime * 45;
            obj.localPosition += new Vector3(0, Input.GetAxis("Vertical"), 0) * Time.deltaTime * 5;
            var sc = obj.localScale;
            sc.x *= 1 + Input.GetAxis("Horizontal") * Time.deltaTime;
            sc.y *= 1 + Input.GetAxis("Horizontal") * Time.deltaTime;
            sc.z *= 1 + Input.GetAxis("Horizontal") * Time.deltaTime;
            obj.localScale = sc;
        }
        if (Input.GetAxis("Run") < 0)
        {
            obj.eulerAngles += new Vector3(Input.GetAxis("Mouse Y"), 0, -Input.GetAxis("Mouse X")) * Time.deltaTime * 360;
        }
        else
        {
            if (Input.GetAxis("Run") > 0)
            {
                poz.x = (int)(poz.x + 0.5f);
                poz.y = (int)(poz.y + 0.5f);
                poz.z = (int)(poz.z + 0.5f);
            }
            else
            {
                distance += Input.mouseScrollDelta.y / 10;
                if (distance < 0.2f)
                    distance = 0.2f;
                else if (distance > 50)
                    distance = 50;
            }

            //poz -= dir * (poz.y / dir.y);
            if (poz.y < 0)
                poz.y = 0;

            if (poz.x < 0)
                poz.x = 0;
            if (poz.z < 0)
                poz.z = 0;
            cursor.position = poz;
        }
        if (CreateRoom.instance)
        {
            if (Input.GetAxis("Fire1") > 0)
            {
                if (!click && cursor.childCount > 0)
                {
                    var cursorChild = cursor.GetChild(0);
                    if(cursorChild.tag == "Object")
                        CreateRoom.instance.addObject(cursorChild);
                    else
                        CreateRoom.instance.addCaracter(cursorChild);
                    singlePlaceObject = false;
                }
                click = true;
            }
            else click = false;

        }
    }

    public override void onDeselect(Camera camera, Transform cursor, object[] args)
    {
        var mr = cursor.GetComponent<MeshRenderer>();
        if (mr)
            mr.enabled = true;
        for (int i = 0; i < cursor.childCount; i++)
            Destroy(cursor.GetChild(i).gameObject);
        singlePlaceObject = false;
    }

    public override void onSelect(Camera camera, Transform cursor, object[] args)
    {
        click = true;
        var mr = cursor.GetComponent<MeshRenderer>();
        if (mr)
            mr.enabled = false;
    }
}
