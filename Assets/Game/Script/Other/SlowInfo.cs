public class SlowInfo
{
    /// <summary>
    /// Ammount of slow to apply, can be either additive or multiplicative
    /// e.g slow by 30% or slow spped by 5
    /// </summary>
    public FloatStatModifier totalSlow;
    public float slowDuration;

    public SlowInfo(FloatStatModifier totalSlow, float slowDuration)
    {
        this.totalSlow = totalSlow;
        this.slowDuration = slowDuration;
    }
}
