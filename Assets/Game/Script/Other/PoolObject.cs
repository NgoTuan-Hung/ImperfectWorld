using UnityEngine;

public class PoolObject
{
	private GameObject gameObject;
	private RadialProgress radialProgress;
	public GameObject GameObject { get => gameObject; set => gameObject = value; }
    public RadialProgress RadialProgress { get => radialProgress; set => radialProgress = value; }
}