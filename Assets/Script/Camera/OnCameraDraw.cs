using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnCameraDraw : MonoBehaviour
{
    public static OnCameraDraw instance;
    private new Camera camera;
    public Transform cursor;
    public OCD_BaseDraw[] cameraDraws;
    public int selected_OCD = 0;
    public Slider floorValue;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        selectOCD(0);
    }

    public void selectOCD(int index)
    {
        while (index < 0)
            index += cameraDraws.Length;
        index = index % cameraDraws.Length;
        cameraDraws[selected_OCD].onDeselect(camera, cursor, new object[] { floorValue });
        selected_OCD = index;
        cameraDraws[selected_OCD].onSelect(camera, cursor, new object[] { floorValue });
        RoomMenuBehaviour.instance.log(cameraDraws[selected_OCD].getName());
    }
    public void selectNextOCD(int step)
    {
        selectOCD(selected_OCD + step);
    }

    public void selectOCD(string toolName)
    {
        for(int i=0;i< cameraDraws.Length;i++)
            if(cameraDraws[i].getName() == toolName)
                selectOCD(i);
    }

    float movementSpeed = 0;
    bool isWeelMoving = false;
    private void move()
    {
        camera.transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * 300;
        Vector3 newPoz = Vector3.zero;
        newPoz += Input.GetAxis("Vertical") * camera.transform.forward;
        newPoz += Input.GetAxis("Horizontal") * camera.transform.right;
        newPoz += Input.GetAxis("Depth") * camera.transform.up;
        movementSpeed += Time.deltaTime * 100;
        if (movementSpeed > 10)
            movementSpeed = 10;
        float multiplier = 1;

        if (Input.GetAxis("Run") > 0.1f)
            multiplier = 4;
        else if (Input.GetAxis("Run") < -0.1f)
            multiplier = 0.05f;
        camera.transform.localPosition += newPoz * Time.deltaTime * movementSpeed * multiplier;

        if (Input.mouseScrollDelta.y > 0.1f)
        {
            if (!isWeelMoving)
                selectOCD(selected_OCD + 1);
            isWeelMoving = true;
        }
        else if (Input.mouseScrollDelta.y < -0.1f)
        {
            if (!isWeelMoving)
                selectOCD(selected_OCD - 1);
            isWeelMoving = true;
        }
        else isWeelMoving = false;
    }
    void Update()
    {
        if (Input.GetAxis("Fire2") > 0)
        {
            move();
        }
        else
        {
            movementSpeed = 0;
            cameraDraws[selected_OCD].draw(camera, cursor, new object[] { floorValue });
        }
    }
}
