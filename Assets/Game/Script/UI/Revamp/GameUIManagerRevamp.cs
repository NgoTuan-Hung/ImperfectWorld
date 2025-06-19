using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManagerRevamp : MonoEditorSingleton<GameUIManagerRevamp>
{
    Canvas canvas;
    public Button menuCharButton,
        characterScreenExitButton;
    public GameObject characterScreen,
        characterInfo,
        skillNodeUI;
    Dictionary<int, CharacterInfoUI> characterInfoUIDict = new();
    public CustomMono currentActiveCustomMono;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        menuCharButton.onClick.AddListener(() =>
        {
            characterInfoUIDict[currentActiveCustomMono.GetHashCode()].gameObject.SetActive(true);
            characterScreen.SetActive(true);
        });
        characterScreenExitButton.onClick.AddListener(() => characterScreen.SetActive(false));
    }

    void GetAllUI() { }

    public override void Start()
    {
        base.Start();
    }

    public void InitializeCharacterUI(CustomMono p_customMono)
    {
        currentActiveCustomMono ??= p_customMono;
        GameObject t_characterInfo = Instantiate(characterInfo);
        t_characterInfo.transform.SetParent(characterScreen.transform, false);
        t_characterInfo.transform.SetSiblingIndex(0);

        CharacterInfoUI t_characterInfoUI = t_characterInfo.GetComponent<CharacterInfoUI>();
        characterInfoUIDict.Add(p_customMono.GetHashCode(), t_characterInfoUI);
        t_characterInfo.SetActive(false);

        /* Load Skill UI */
        p_customMono.skill.skillDataSOs.ForEach(t_skillDataSO =>
        {
            GameObject t_skillNodeUI = Instantiate(skillNodeUI);
            t_skillNodeUI.transform.SetParent(t_characterInfoUI.skillSVContent.transform, false);

            SkillNodeUI t_skillNodeUIComp = t_skillNodeUI.GetComponent<SkillNodeUI>();
            t_skillNodeUIComp.icon.sprite = t_skillDataSO.skillImage;
        });
    }
}
