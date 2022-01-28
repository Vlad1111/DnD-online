using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [System.Serializable]
    public class SettingData
    {
        public float mouseSensitivity = 1;
    }

    private SettingData data = null;
    public SettingData Data
    {
        get
        {
            if (data == null)
                data = new SettingData();
            return data;
        }
    }
}
