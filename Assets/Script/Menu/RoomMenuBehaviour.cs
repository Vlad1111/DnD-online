using Assets.Script.Server.Commands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomMenuBehaviour : MonoBehaviour
{
    public static RoomMenuBehaviour instance;
    private void Awake()
    {
        instance = this;
    }
    public Dropdown floorTexture;
    public Dropdown wallTexture;
    public InputField propName;
    public Dropdown propFiles;
    public Transform menuParent;
    public Text debugText;
    public Transform onScreenPropMenu;
    public Dropdown objectTexture;
    private Transform objectProperiesTarget = null;

    private void Start()
    {
        floorTexture.ClearOptions();
        wallTexture.ClearOptions();
        objectTexture.ClearOptions();
        objectTexture.SetValueWithoutNotify(-1);
        objectTexture.options.Add(new Dropdown.OptionData(""));
        for (int i = 0; i < CreateRoom.instance.materials.Length; i++)
        {
            var drv = new Dropdown.OptionData(CreateRoom.instance.materials[i].name);
            floorTexture.options.Add(drv);
            wallTexture.options.Add(drv);
            objectTexture.options.Add(drv);
            if (CreateRoom.instance.wallsMaterialName == drv.text)
                wallTexture.SetValueWithoutNotify(i);
            if (CreateRoom.instance.floorMaterialName == drv.text)
                floorTexture.SetValueWithoutNotify(i);
        }
        updateProFiles();
    }

    public void selectObjet(Transform obj = null)
    {
        if(obj == null)
            onScreenPropMenu.gameObject.SetActive(false);
        else
        {
            onScreenPropMenu.gameObject.SetActive(true);
            objectTexture.SetValueWithoutNotify(0);
        }
        objectProperiesTarget = obj;
    }

    public void log(string text)
    {
        debugText.text = text;
    }
    public void toggleMenu()
    {
        menuParent.gameObject.SetActive(!menuParent.gameObject.activeSelf);
    }

    public void setRoomHeight(Slider val)
    {
        CreateRoom.instance.setWallHeights(val.value);
        updateSmall();
    }
    public void setRoomUvScale(Slider val)
    {
        CreateRoom.instance.setUVSize(val.value);
        updateSmall();
    }
    public void setRoomVerticsOffset(Slider val)
    {
        CreateRoom.instance.setVertecisOffset(val.value);
        updateSmall();
    }

    public void setFloorTexture()
    {
        CreateRoom.instance.changeFloorMaterialName(floorTexture.options[floorTexture.value].text);
        updateSmall();
    }

    public void setWalltexture()
    {
        CreateRoom.instance.changeWallMaterialName(wallTexture.options[wallTexture.value].text);
        updateSmall();
    }

    public void updateProFiles()
    {
        propFiles.options.Clear();
        var drv = new Dropdown.OptionData("/");
        propFiles.options.Add(drv);
        propFiles.SetValueWithoutNotify(0);
        string[] directory = GENERAL.getFiles(
            GENERAL.FileLoactions.prefabs + "/" + getPropName(),
            GENERAL.FileType.DIRECTORY);
        if (directory.Length != 0)
        {
            foreach (var d in directory)
            {
                drv = new Dropdown.OptionData(d + "/");
                propFiles.options.Add(drv);
            }
        }
        string[] pref = GENERAL.getFiles(
            GENERAL.FileLoactions.prefabs + "/" + getPropName(),
            GENERAL.FileType.PREFAB);
        foreach (var d in pref)
        {
            drv = new Dropdown.OptionData(d);
            propFiles.options.Add(drv);
        }
    }
    public void onPropDirectoryChange()
    {
        if (propName.text.Length > 0)
            propName.text += "/";
        propName.text += propFiles.options[propFiles.value].text;
        if (propName.text.EndsWith("/"))
            propName.text = propName.text.Substring(0, propName.text.Length - 1);
        updateProFiles();
    }

    public string getPropName()
    {
        return propName.text;
    }

    public void propNameBackDirectory()
    {
        string n = propName.text;
        int i = n.Length - 1;
        while (i > 0 && n[i] != '/')
            i--;
        if (i > 0)
            propName.text = n.Substring(0, i);
        else propName.text = "";
        updateProFiles();
    }

    public void createNewRoom()
    {
        CreateRoom.instance.creteNewRoom();
    }

    public void setSunIntensity(Slider slider)
    {
        WordBehaviour.instance.setSunPower(slider.value);
    }

    public void onObjectMaterialSelect()
    {
        string name = objectTexture.options[objectTexture.value].text;
        if (name == "")
            return;
        WordBehaviour.instance.setObjectTexture(objectProperiesTarget, name);
        selectObjet(null);
    }

    public void rotateObject(UiButtonDataDTO rotation)
    {
        WordBehaviour.instance.rotateObject(objectProperiesTarget, rotation.v3 * 30f);
    }

    public void moveObject(UiButtonDataDTO distance)
    {
        WordBehaviour.instance.moveObject(objectProperiesTarget, distance.v3 * 0.1f);
    }
    public void scaleObject(UiButtonDataDTO distance)
    {
        WordBehaviour.instance.resizeObject(objectProperiesTarget, distance.v3 * 0.1f);
    }

    public void updateFloorToOtherPlayers()
    {
        CreateRoom.instance.updateFloorWallsToPlayers();
    }

    public void updateSmall()
    {
        ClienBehaviour.instace.Send(
            CommandBuilder.Instance.updateRoom(
                CreateRoom.instance.getData(false, false, false)));
    }
    public void updateCurentRoom()
    {
        ClienBehaviour.instace.Send(
            CommandBuilder.Instance.updateRoom(
                CreateRoom.instance.getData()));
    }

    private void Update()
    {
        if (objectProperiesTarget)
        {
            var poz = objectProperiesTarget.position;
            poz = Camera.main.WorldToScreenPoint(poz);
            onScreenPropMenu.position = poz;
        }
    }
}