using UnityEngine;

public class PoolObject
{
	private GameObject gameObject;
	private RadialProgress radialProgress;
	private CollideAndDamage collideAndDamage;
	public GameObject GameObject { get => gameObject; set => gameObject = value; }
	public RadialProgress RadialProgress { get => radialProgress; set => radialProgress = value; }
    public CollideAndDamage CollideAndDamage { get => collideAndDamage; set => collideAndDamage = value; }
}