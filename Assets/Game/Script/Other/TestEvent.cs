
using UnityEngine;
using UnityEngine.UIElements;

public class TestEvent : MonoBehaviour {
	UIDocument uIDocument;
	VisualElement root, grand_parent, parent, child;
	ScrollView scrollView;
	[SerializeField] private bool blockScrollView = false;
	private void Awake() {
		uIDocument = GetComponent<UIDocument>();
		root = uIDocument.rootVisualElement;
		
		grand_parent = root.Q<VisualElement>(name: "grand-parent");
		parent = grand_parent.Q<VisualElement>(name: "parent");
		child = parent.ElementAt(0);
		scrollView = root.Q<ScrollView>();
		
		grand_parent.RegisterCallback<PointerDownEvent>((evt) => 
		{
			evt.StopPropagation();
		}, TrickleDown.TrickleDown);
		
		parent.RegisterCallback<PointerDownEvent>((evt) => 
		{
			print("parent");
		}, TrickleDown.TrickleDown);
		
		child.RegisterCallback<PointerDownEvent>((evt) => 
		{
			print("child");
		}, TrickleDown.TrickleDown);
		
		child.RegisterCallback<PointerDownEvent>((evt) => print("also child"), TrickleDown.TrickleDown);
		
		foreach (VisualElement visualElement in scrollView.contentContainer.Children())
		{
			visualElement.RegisterCallback<PointerMoveEvent>((evt) => 
			{
				if (blockScrollView) evt.StopPropagation();
			});
		}
	}
}