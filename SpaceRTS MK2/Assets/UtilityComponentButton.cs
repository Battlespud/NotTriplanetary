using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UtilityComponentButton : MonoBehaviour
{

	public Button button;
	public ShipComponents comp;

	public void Setup(ShipComponents c)
	{
		comp = c;
		GetComponentInChildren<Text>().text = c.Name;
		button.onClick.AddListener(LoadComponent);
	}

	void LoadComponent()
	{
		UtilityComponentDesigner.StaticComponentDesigner.LoadComponentToScreen(comp);
	}
	
	// Use this for initialization
	void Awake ()
	{
		button = GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
