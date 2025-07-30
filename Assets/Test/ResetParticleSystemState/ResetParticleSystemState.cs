using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{
    GameObject targetParticleSystemGO;
    ParticleSystem particleSystem,
        targetParticleSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        targetParticleSystem = targetParticleSystemGO.GetComponent<ParticleSystem>();
    }

    IEnumerator ResetParticleSystemStateIE()
    {
        float currentTime = 0;
        while (true)
        {
            if (Keyboard.current.kKey.isPressed && currentTime > 1)
            {
                currentTime = 0;
                var emission = particleSystem.emission;
                emission = targetParticleSystem.emission;
            }

            yield return new WaitForSeconds(Time.fixedDeltaTime);
            currentTime += Time.fixedDeltaTime;
        }

        // particleSystem.clear
    }

    // Update is called once per frame
    void Update() { }
}
