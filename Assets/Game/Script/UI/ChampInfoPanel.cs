using TMPro;
using UnityEngine;

public class ChampInfoPanel : MonoBehaviour
{
    //
#if false
    CustomMono.Start
        GenerateAndBindChampUI => GUIM.GenerateAndBindChampUI

    GUIM.GenerateAndBindChampUI
        champUI = instantiate
        ChampUIDict.Add(customMono, champUI)

    ChampUI
        customMono
        Init()
            customMomo = cM
            name = name
            ValueChangeRegister
        FixUpdate
            transform.pos = cM.pos
#endif
    CustomMono owner;
    GameObject container,
        statContent;
    TextMeshProUGUI nameTMP,
        hpTMP,
        mpTMP,
        strengthTMP,
        reflexTMP,
        wisdomTMP,
        aspdTMP,
        armorTMP,
        omnivampTMP;
    public static Vector3 offset = new(1.33942f, 2.614366f);

    private void Awake()
    {
        container = transform.Find("Container").gameObject;
        nameTMP = container.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        statContent = container.transform.Find("StatSV/Viewport/Content").gameObject;
        hpTMP = statContent.transform.Find("HP").GetComponent<TextMeshProUGUI>();
        mpTMP = statContent.transform.Find("MP").GetComponent<TextMeshProUGUI>();
        strengthTMP = statContent.transform.Find("Strength").GetComponent<TextMeshProUGUI>();
        reflexTMP = statContent.transform.Find("Reflex").GetComponent<TextMeshProUGUI>();
        wisdomTMP = statContent.transform.Find("Wisdom").GetComponent<TextMeshProUGUI>();
        aspdTMP = statContent.transform.Find("ASPD").GetComponent<TextMeshProUGUI>();
        armorTMP = statContent.transform.Find("Armor").GetComponent<TextMeshProUGUI>();
        omnivampTMP = statContent.transform.Find("Omnivamp").GetComponent<TextMeshProUGUI>();
    }

    public void Init(CustomMono p_owner)
    {
        owner = p_owner;
        nameTMP.text = owner.name;
    }

    void FixedUpdate()
    {
        transform.position = owner.transform.position + offset;
    }
}
