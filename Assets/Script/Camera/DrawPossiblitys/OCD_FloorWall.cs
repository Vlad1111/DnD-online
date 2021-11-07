using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "__wallAndFloor", menuName = "On Camera Draw Objects/Wall and floor")]
public class OCD_FloorWall : OCD_BaseDraw
{
    bool wasPressed = false;
    float lastPress = -1;

    public override string getName() { return "Draw Floor/Wall tool"; }

    public override void draw(Camera camera, Transform cursor, object[] args)
    {
        var dir = camera.ScreenPointToRay(Input.mousePosition).direction;
        Vector3 poz = camera.transform.position;
        poz -= dir * (poz.y / dir.y);
        poz.y = 0;

        if (poz.x < 0)
            poz.x = 0;
        if (poz.z < 0)
            poz.z = 0;
        if ((cursor.position - poz).magnitude > 0.1f)
            lastPress = -1;
        cursor.position = poz;

        if (CreateRoom.instance)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            poz -= CreateRoom.instance.transform.position - Vector3.one / 2;
            if (lastPress < 0)
            {
                var floorValue = args[0] as Slider;
                if (Input.GetAxis("Fire1") > 0)
                {
                    lastPress = 0.1f;
                    if (Input.GetAxis("Run") > 0)
                        CreateRoom.instance.updateTileFloor((int)poz.x, (int)poz.z, 1 - floorValue.value);
                    else
                        CreateRoom.instance.updateTileFloor((int)poz.x, (int)poz.z, floorValue.value);
                }

                if (Input.GetAxis("Fire3") > 0)
                {
                    lastPress = 0.1f;
                    if (!wasPressed)
                    {
                        if (Input.GetAxis("Run") > 0)
                            CreateRoom.instance.fillTileFloor((int)poz.x, (int)poz.z, 1 - floorValue.value);
                        else
                            CreateRoom.instance.fillTileFloor((int)poz.x, (int)poz.z, floorValue.value);
                    }
                    wasPressed = true;
                }
                else wasPressed = false;
            }
            else lastPress -= Time.deltaTime;
        }
    }
}
