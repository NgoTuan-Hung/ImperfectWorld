using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEffectPrefab : MonoBehaviour
{
    public TrailRenderer trailRenderer;
    public List<AnimateObject> animateObjects;

    public void Reset()
    {
        animateObjects = GetComponentsInChildren<AnimateObject>().ToList();
    }
}
