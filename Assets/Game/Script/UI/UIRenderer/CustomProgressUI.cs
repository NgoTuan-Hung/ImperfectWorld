
using UnityEngine;
using UnityEngine.UIElements;

public class CustomProgressUI : VisualElement
{
	public CustomProgressUI()
	{
		generateVisualContent += GenerateVisualContent;
	}
	
	Vector2 p0 = Vector2.zero, p1, p2, p3;
	float progressInWidth;
	
	public void GenerateVisualContent(MeshGenerationContext meshGenerationContext)
	{
		var painter2D = meshGenerationContext.painter2D;
		painter2D.fillColor = fillColor;
		progressInWidth = progress * layout.width;
		
		p1 = new Vector2(progressInWidth, 0);
		p2 = new Vector2(progressInWidth, layout.height);
		p3 = new Vector2(0, layout.height);
		
		painter2D.BeginPath();
		painter2D.MoveTo(p0);
		painter2D.LineTo(p1);
		painter2D.LineTo(p2);
		painter2D.LineTo(p3);
		painter2D.ClosePath();
		painter2D.Fill();
	}
	
	float progress = 0f;
	public float Progress
	{
		get => progress;
		set 
		{
			progress = value;
			MarkDirtyRepaint();
		}
	}
	
	Color fillColor = Color.red;
	public Color FillColor
	{
		get => fillColor;
		set
		{
			fillColor = value;
			MarkDirtyRepaint();
		}
	}
	
	public new class UxmlFactory : UxmlFactory<CustomProgressUI, UxmlTraits> { }
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		// The progress property is exposed to UXML.
		UxmlFloatAttributeDescription m_ProgressAttribute = new()
		{
			name = "progress"
		};
		
		UxmlColorAttributeDescription m_FillColorAttribute = new()
		{
			name = "fill-color"
		};

		// Use the Init method to assign the value of the progress UXML attribute to the C# progress property.
		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			var rp = ve as CustomProgressUI;

			rp.Progress = m_ProgressAttribute.GetValueFromBag(bag, cc);
			rp.FillColor = m_FillColorAttribute.GetValueFromBag(bag, cc);
		}
	}
}