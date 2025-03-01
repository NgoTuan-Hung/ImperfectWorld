/// <summary>
/// Any class which has a connection with CustomMono.
/// </summary>
public class CustomMonoPal : MonoEditor
{
    public CustomMono customMono;
    public override void Start()
    {
        base.Start();
    }
    
    public virtual void Awake() {customMono = GetComponent<CustomMono>();}
}