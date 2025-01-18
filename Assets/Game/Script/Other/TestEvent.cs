using System;
using UnityEngine;

class TestObject
{
	public int i;
}
public class TestEvent : MonoBehaviour {
	TestObject testObject = new();
	new Action print;
	
	private void Awake() {
		testObject.i = 1;
		print = () => {Debug.Log(testObject.i);};
	}
	
	public bool called = false;
	private void Update() {
		if (called) 
		{
			called = false;
			print();
		}
		
		Change();
	}
	
	public bool changed = false;
	public void Change()
	{
		if (changed)
		{
			changed = false;
			testObject = new TestObject()
			{
				i = testObject.i + 1
			};
		}
	}
}