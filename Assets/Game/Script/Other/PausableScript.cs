using System;

public class PausableScript
{
    /// <summary>
    /// This should be placed inside FixedUpdate of the MonoBehaviour
    /// </summary>
    public Action fixedUpdate = () => { };

    /// <summary>
    /// You should assign fixedUpdate to Empty Method using this
    /// </summary>
    public Action pauseFixedUpdate = () => { };

    /// <summary>
    /// You should assign fixedUpdate to Default Update Method using this
    /// </summary>
    public Action resumeFixedUpdate = () => { };
}
