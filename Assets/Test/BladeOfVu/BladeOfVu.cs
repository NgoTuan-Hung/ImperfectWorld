using System.Collections;
using UnityEngine;

public class BladeOfVu : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public SpriteList spriteList;
    public GameObject slash;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    void BOV()
    {
        StartCoroutine(BOVIE());
    }

    public int totalSpriteSwap = 10;
    public float changeSpriteInterval = 0.1f;
    public float blinkRange = 4f;
    public GameObject ghostPrefab;

    IEnumerator BOVIE()
    {
        int currentSpriteSwap = 0;
        Sprite original = spriteRenderer.sprite;
        spriteRenderer.sprite = null;
        while (currentSpriteSwap < totalSpriteSwap)
        {
            EffectTest ghostET = Instantiate(ghostPrefab, transform.position, Quaternion.identity)
                .GetComponent<EffectTest>();
            ghostET.spriteRenderer.sprite = spriteList.sprites[
                Random.Range(0, spriteList.sprites.Count)
            ];
            ghostET.transform.position =
                transform.position
                + new Vector3(Random.Range(-1, 1f), Random.Range(-0.5f, 1f), 0).normalized
                    * Random.Range(0, blinkRange);
            ghostET.transform.localScale = transform.localScale.WithX(
                Random.Range(0, 2) == 0 ? -1 : 1
            );

            yield return new WaitForSeconds(changeSpriteInterval);
            currentSpriteSwap++;
        }

        slash.SetActive(true);
        slash.transform.position = transform.position;
        spriteRenderer.sprite = original;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            BOV();
        }
    }
}
