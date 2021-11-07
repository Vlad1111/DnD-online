using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "__changeObject", menuName = "On Camera Draw Objects/Select Object To Change")]
public class OCD_ChangeObject : OCD_BaseDraw
{
    public override string getName() { return "Change object property tool"; }
    private bool lastClick = false;
    public override void draw(Camera camera, Transform cursor, object[] args)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform obj = hit.transform;
            if (obj.tag == "Object" || obj.tag == "Caracter")
            {
                cursor.position = hit.point;
                if (Input.GetAxis("Fire1") > 0.1f)
                {
                    if (!lastClick)
                        RoomMenuBehaviour.instance.selectObjet(obj);
                    lastClick = true;
                }
                else lastClick = false;
            }
        }
    }

    public override void onDeselect(Camera camera, Transform cursor, object[] args)
    {
        base.onDeselect(camera, cursor, args);
        RoomMenuBehaviour.instance.selectObjet(null);
    }
}