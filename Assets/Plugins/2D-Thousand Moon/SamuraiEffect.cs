using System.Collections;
using UnityEngine;

public class SamuraiEffect : MonoBehaviour 
{
    public bool signal = false;
    public Vector3 moveDir;
    public GameObject slashEffect;
    public Vector3 effectOffset;
    public void WaitForSignal()
    {
        StartCoroutine(WaitForSignalCoroutine());
    }
    
    public void SendSignal() => signal = true;
    
    IEnumerator WaitForSignalCoroutine()
    {
        while (!signal) yield return new WaitForSeconds(Time.fixedDeltaTime);
        
        slashEffect = Instantiate(slashEffect);
        slashEffect.transform.rotation = Quaternion.FromToRotation(Vector3.right, moveDir);
        slashEffect.transform.position = transform.position + effectOffset;
    } 
}