using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameEffectPrefab : MonoBehaviour
{
    public TrailRenderer trailRenderer;
    public List<AnimateObject> animateObjects;
    public List<BoxCollider2D> boxCollider2Ds = new();
    public List<CircleCollider2D> circleCollider2Ds = new();
    public List<PolygonCollider2D> polygonCollider2Ds = new();

    public void Reset()
    {
        animateObjects = GetComponentsInChildren<AnimateObject>().ToList();
        boxCollider2Ds = GetComponentsInChildren<BoxCollider2D>().ToList();
        circleCollider2Ds = GetComponentsInChildren<CircleCollider2D>().ToList();
        polygonCollider2Ds = GetComponentsInChildren<PolygonCollider2D>().ToList();
    }
}
