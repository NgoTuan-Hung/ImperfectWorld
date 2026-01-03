using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    GameObject layer1,
        layer2;
    RectTransform settingSectionRT;
    CanvasGroup settingSection,
        loadingScreen;
    Vector2 settingSectionHidePos = new(258, 1000),
        settingSectionDefaultPos;

    GameObject graphicSection,
        gameplaySection,
        soundSection;
    MainMenuButton start,
        _continue,
        setting,
        quit,
        graphic,
        gameplay,
        sound,
        _return,
        saveAndQuit;
    List<MainMenuButton> layer1Buttons = new List<MainMenuButton>(),
        layer2Buttons = new List<MainMenuButton>();
    bool layerTransition = false;
    GameObject activeSection;

    private void Awake()
    {
        layer1 = transform.Find("Layer1").gameObject;
        layer2 = transform.Find("Layer2").gameObject;
        settingSection = transform.Find("SettingSection").GetComponent<CanvasGroup>();
        settingSectionRT = settingSection.GetComponent<RectTransform>();
        settingSectionDefaultPos = settingSectionRT.localPosition;
        loadingScreen = transform.Find("LoadingScreen").GetComponent<CanvasGroup>();

        start = layer1.transform.Find("Start").GetComponent<MainMenuButton>();
        _continue = layer1.transform.Find("Continue").GetComponent<MainMenuButton>();
        setting = layer1.transform.Find("Setting").GetComponent<MainMenuButton>();
        quit = layer1.transform.Find("Quit").GetComponent<MainMenuButton>();
        graphic = layer2.transform.Find("Graphic").GetComponent<MainMenuButton>();
        gameplay = layer2.transform.Find("Gameplay").GetComponent<MainMenuButton>();
        sound = layer2.transform.Find("Sound").GetComponent<MainMenuButton>();
        _return = layer2.transform.Find("Return").GetComponent<MainMenuButton>();
        saveAndQuit = layer2.transform.Find("SaveAndQuit").GetComponent<MainMenuButton>();

        graphicSection = settingSection.transform.Find("GraphicSection").gameObject;
        gameplaySection = settingSection.transform.Find("GameplaySection").gameObject;
        soundSection = settingSection.transform.Find("SoundSection").gameObject;

        layer1Buttons.Add(start);
        layer1Buttons.Add(_continue);
        layer1Buttons.Add(setting);
        layer1Buttons.Add(quit);
        layer2Buttons.Add(graphic);
        layer2Buttons.Add(gameplay);
        layer2Buttons.Add(sound);
        layer2Buttons.Add(_return);
        layer2Buttons.Add(saveAndQuit);

        activeSection = graphicSection;
        RegisterEvents();
    }

    private void RegisterEvents()
    {
        start.pointerClickEvent += StartGame;
        setting.pointerClickEvent += ShowSetting;
        quit.pointerClickEvent += QuitGame;
        _return.pointerClickEvent += CloseSetting;
        graphic.pointerClickEvent += ShowGraphicSection;
        gameplay.pointerClickEvent += ShowGameplaySection;
        sound.pointerClickEvent += ShowSoundSection;
    }

    void ShowGraphicSection(PointerEventData pointerEventData) =>
        ShowSettingSection(graphicSection);

    void ShowGameplaySection(PointerEventData pointerEventData) =>
        ShowSettingSection(gameplaySection);

    void ShowSoundSection(PointerEventData pointerEventData) => ShowSettingSection(soundSection);

    void ShowSetting(PointerEventData pointerEventData)
    {
        StartCoroutine(ChainShowLayer2IE());
        ShowSettingSection(activeSection);
    }

    void ShowSettingSection(GameObject section)
    {
        activeSection.SetActive(false);
        activeSection = section;
        activeSection.SetActive(true);
        settingSection.gameObject.SetActive(true);
        settingSectionRT.localPosition = settingSectionHidePos;
        settingSection.alpha = 0;

        settingSectionRT.DOLocalMove(settingSectionDefaultPos, 0.5f).SetEase(Ease.OutBack);
        settingSection.DOFade(1, 0.5f).SetEase(Ease.OutQuint);
    }

    void CloseSetting(PointerEventData pointerEventData)
    {
        StartCoroutine(ChainShowLayer1IE());
        CloseSettingSection();
    }

    void CloseSettingSection()
    {
        settingSectionRT.DOLocalMove(settingSectionHidePos, 0.5f).SetEase(Ease.InBack);
        settingSection
            .DOFade(0, 0.5f)
            .SetEase(Ease.InQuint)
            .OnComplete(() => settingSection.gameObject.SetActive(true));
    }

    IEnumerator ChainShowLayer1IE()
    {
        if (layerTransition)
            yield break;
        layerTransition = true;
        yield return HideLayer2IE();
        layer2.SetActive(false);
        layer1.SetActive(true);
        yield return ShowLayer1IE();
        layerTransition = false;
    }

    IEnumerator ChainShowLayer2IE()
    {
        if (layerTransition)
            yield break;
        layerTransition = true;
        yield return HideLayer1IE();
        layer1.SetActive(false);
        layer2.SetActive(true);
        yield return ShowLayer2IE();
        layerTransition = false;
    }

    IEnumerator HideLayer1IE()
    {
        for (int i = 0; i < layer1Buttons.Count - 1; i++)
        {
            layer1Buttons[i].FadeOut();
            yield return new WaitForSeconds(0.1f);
        }

        yield return layer1Buttons[^1].FadeOut();
    }

    IEnumerator HideLayer2IE()
    {
        for (int i = 0; i < layer2Buttons.Count - 1; i++)
        {
            layer2Buttons[i].FadeOut();
            yield return new WaitForSeconds(0.1f);
        }

        yield return layer2Buttons[^1].FadeOut();
    }

    IEnumerator ShowLayer1IE()
    {
        for (int i = 0; i < layer1Buttons.Count - 1; i++)
        {
            layer1Buttons[i].FadeIn();
            yield return new WaitForSeconds(0.1f);
        }

        yield return layer1Buttons[^1].FadeIn();
    }

    IEnumerator ShowLayer2IE()
    {
        for (int i = 0; i < layer2Buttons.Count - 1; i++)
        {
            layer2Buttons[i].FadeIn();
            yield return new WaitForSeconds(0.1f);
        }

        yield return layer2Buttons[^1].FadeIn();
    }

    void StartGame(PointerEventData pointerEventData)
    {
        StartCoroutine(StartGameIE());
    }

    IEnumerator StartGameIE()
    {
        loadingScreen.gameObject.SetActive(true);
        yield return loadingScreen
            .DOFade(1, 1f)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => GameUIManager.Instance.LoadScene(SceneMode.MainGame));
    }

    void QuitGame(PointerEventData pointerEventData) => Application.Quit();
}
