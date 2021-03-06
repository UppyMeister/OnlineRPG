﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingSlider : MonoBehaviour, IOptionsSetting
{
    [SerializeField] private TMP_Text settingNameText;
    [SerializeField] private Slider settingSlider;

    private GraphicsManager graphicsMan;
    private string settingDictionaryKey;
    private float currentValue;

    public IOptionsInfo info { get; set; }

    bool setup = false;
    bool checkingForChange = false;

    void Start()
    {
        EventHandler.OnGraphicsSettingsChanged += CheckForChange;
    }

    public void Setup(string settingName)
    {
        graphicsMan = GraphicsManager.singleton;
        settingDictionaryKey = settingName;
        SliderInfo sliderInfo = graphicsMan.GetSetting(settingName) as SliderInfo;
        info = sliderInfo;
        currentValue = (float)sliderInfo.Value;
        settingNameText.text = sliderInfo.Name;
        settingSlider.minValue = sliderInfo.MinValue;
        settingSlider.maxValue = sliderInfo.MaxValue;
        settingSlider.value = (float)sliderInfo.Value;
        setup = true;
    }

    public void OnSliderValueChanged(float newValue)
    {
        if (!setup || checkingForChange) return;
        graphicsMan.SetSetting(settingDictionaryKey, newValue);
        currentValue = newValue;
    }

    void CheckForChange()
    {
        checkingForChange = true;
        SliderInfo sliderInfo = graphicsMan.GetSetting(settingDictionaryKey) as SliderInfo;

        if ((float)sliderInfo.Value != currentValue)
        {
            currentValue = (float)sliderInfo.Value;
            settingSlider.value = currentValue;
        }
        checkingForChange = false;
    }
}