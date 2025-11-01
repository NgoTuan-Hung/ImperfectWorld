using System;

public class PausableScript
{
    /// <summary>
    /// This should be placed inside FixedUpdate of the MonoBehaviour
    /// </summary>
    public Action fixedUpdate = () => { };

    /// <summary>
    /// You should assign fixedUpdate to Empty Method when calling this
    /// </summary>
    public Action pauseFixedUpdate = () => { };

    /// <summary>
    /// You should assign fixedUpdate to Default Update Method when calling this
    /// </summary>
    public Action resumeFixedUpdate = () => { };

    public void Setup(Action pauseAction, Action resumeAction)
    {
        pauseFixedUpdate = () => fixedUpdate = pauseAction;
        resumeFixedUpdate = () => fixedUpdate = resumeAction;
    }
}
