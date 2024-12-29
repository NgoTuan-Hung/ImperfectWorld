/// <summary>
/// Represent a range for a keep distance action, if random value jump within this range,
/// the action is chosen.
/// </summary>
public class KeepDistanceActionBound
{
	public float lowBound;
	public float highBound;
	public KeepDistanceAction keepDistanceAction;
	public float duration;

	public KeepDistanceActionBound(float lowBound, float highBound, KeepDistanceAction keepDistanceAction, float duration)
	{
		this.lowBound = lowBound;
		this.highBound = highBound;
		this.keepDistanceAction = keepDistanceAction;
		this.duration = duration;
	}
}