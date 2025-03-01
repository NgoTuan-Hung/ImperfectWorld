using System;

public class PausableScript
{
    public Action fixedUpdate = () => {};
    public Action pauseFixedUpdate = () => {};
    public Action resumeFixedUpdate = () => {};
}