using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SettingsController : MonoBehaviour
{
    [SerializeField]
    public List<ResolutionItem> resolutions = new List<ResolutionItem>();
    private TMP_Dropdown dropdown;

    void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        dropdown = transform.Find("Panel").Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener(delegate
        {
            PickResoltion(dropdown);
        });
    }

    void Update()
    {

    }
    public void PickResoltion(TMP_Dropdown change)
    {
        Screen.SetResolution(resolutions[change.value].horizontal, resolutions[change.value].vertical, FullScreenMode.Windowed);
    }

    [System.Serializable]
    public class ResolutionItem
    {
        public int horizontal, vertical;
    }
}