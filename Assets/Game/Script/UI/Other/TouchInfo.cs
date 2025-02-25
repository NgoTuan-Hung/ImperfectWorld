using UnityEngine;
using UnityEngine.UIElements;

public class TouchInfo
{
	public Touch touch;
	public Vector2 panelPosition;
	public int fingerId;
	public IPanel panel;
	public TouchInfo(){}
	public TouchInfo(IPanel p_panel)
	{
		panel = p_panel;
	}
	
	public void UpdateSelf()
	{
		for (int i=0;i<Input.touchCount;i++)
		{
			if (Input.GetTouch(i).fingerId == fingerId)
			{
				touch = Input.GetTouch(i);
				panelPosition = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(touch.position.x, Screen.height - touch.position.y));
				return;
			}
		}
		
		touch.phase = TouchPhase.Ended;
	}
}