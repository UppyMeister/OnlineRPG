﻿using UnityEngine;

public class OptionsInitializer : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField] private GameObject graphicsDropdownPrefab;
    [SerializeField] private GameObject graphicsTogglePrefab;
    [SerializeField] private Transform graphicsContent;

    [Space]

    [Header("Game")]
    [SerializeField] private GameObject gamePrefab;
    [SerializeField] private Transform gameContent;

    [Space]

    [Header("Controls")]
    [SerializeField] private GameObject controlsPrefab;
    [SerializeField] private Transform controlsContent;

    [Header("Audio")]
    [SerializeField] private GameObject audioSliderPrefab;
    [SerializeField] private Transform audioContent;

    private GraphicsManager graphicsMan;
    private GeneralOptionsManager generalMan;
    private InputManager inputMan;
    private AudioManager audioMan;

    void Start()
    {
        graphicsMan = GraphicsManager.singleton;
        generalMan = GeneralOptionsManager.singleton;
        inputMan = InputManager.singleton;
        audioMan = AudioManager.singleton;

        foreach (string graphicsSetting in graphicsMan.GetAllGraphicsSettings())
        {
            IOptionsInfo info = graphicsMan.GetSetting(graphicsSetting);
            if (info is ToggleInfo)
            {
                GameObject graphicsSettingGO = Instantiate(graphicsTogglePrefab, graphicsContent, false);
                graphicsSettingGO.GetComponent<GraphicsSettingToggle>().Setup(graphicsSetting);
            } else if (info is DropdownInfo)
            {
                GameObject graphicsSettingGO = Instantiate(graphicsDropdownPrefab, graphicsContent, false);
                graphicsSettingGO.GetComponent<GraphicsSettingDropdown>().Setup(graphicsSetting);
            }
        }

        foreach (string keybind in inputMan.GetAllKeyTypes())
        {
            GameObject keybindGO = Instantiate(controlsPrefab, controlsContent, false);
            keybindGO.GetComponent<Keybind>().Setup(keybind);
        }

        foreach (string audioSetting in audioMan.GetAllAudioSettings())
        {
            IOptionsInfo info = audioMan.GetSetting(audioSetting);

            if (info is SliderInfo)
            {
                GameObject audioSettingGO = Instantiate(audioSliderPrefab, audioContent, false);
                audioSettingGO.GetComponent<AudioSettingSlider>().Setup(audioSetting);
            }
        }
    }
}