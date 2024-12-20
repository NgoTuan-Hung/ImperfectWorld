using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
	public readonly int attackButtonScrollViewIndex = 7;
	public readonly int attackButtonIndex = 0;
	BinarySearchTree<CustomMono> customMonos = new BinarySearchTree<CustomMono>();
	
	public void AddCustomMono(CustomMono customMono)
	{
		customMonos.Insert(customMono);
	}
	
	public CustomMono GetCustomMono(GameObject gameObject)
	{
		return customMonos.Search((customMono) => customMono.gameObject.GetHashCode(), gameObject.GetHashCode());
	}
}