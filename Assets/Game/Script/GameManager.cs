using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
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