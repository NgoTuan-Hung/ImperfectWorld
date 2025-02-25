using UnityEngine;
using UnityEngine.UIElements;
public class TouchExtension
{
	public static TouchInfo GetTouchInfoAt(Vector2 screenPosition, VisualElement root)
	{
		TouchInfo touchInfo = new(root.panel);
		for (int i=0;i<Input.touchCount;i++)
		{
			touchInfo.touch = Input.GetTouch(i);
			touchInfo.panelPosition = RuntimePanelUtils.ScreenToPanel(touchInfo.panel, new Vector2(touchInfo.touch.position.x, Screen.height - touchInfo.touch.position.y));
			
			if (touchInfo.panelPosition == screenPosition)
			{
				touchInfo.fingerId = touchInfo.touch.fingerId;
				return touchInfo;
			}
		}

		return touchInfo;
	}
}