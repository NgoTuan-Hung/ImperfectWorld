using UnityEngine;

public enum ItemType {Usable, Equippable}
[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
public class ItemData : ScriptableObject 
{
	public string itemName;
	public ItemType itemType;
	public Texture2D itemImage;
	public Texture2D itemHelperImage;
	[TextArea] public string itemHelperDescription;
}