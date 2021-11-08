using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLogBehaviour : MonoBehaviour
{
    public static UiLogBehaviour instance;
    private void Awake()
    {
        instance = this;
    }
    public Transform parent;
    public Text log;
    public void addMesage(string mesage)
    {
        log.text += mesage + '\n';
        if (!parent.gameObject.activeSelf) toggle();
    }

    public void toggle()
    {
        parent.gameObject.SetActive(!parent.gameObject.activeSelf);
    }
}
