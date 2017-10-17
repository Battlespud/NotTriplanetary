using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System.Linq;


public class ComponentDesigner : MonoBehaviour {

	public InputField Name;
	public Dropdown Category;
	public InputField Mass;
	public Toggle isToggleable;
	public InputField PassiveSig;
	public InputField CrewRequired;
	public InputField Quarters;
	public InputField LifeSupport;
	public InputField PowerReq;
	public Toggle isControl;
	public Toggle isFlagControl;
	public Dropdown HTK;
	public Toggle isEngine;
	public InputField Thrust;
	public InputField TurnThrust;
	public InputField FuelConsumption;


	public void Save(){
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Components/" + Name.text + ".txt"); 
		using (StreamWriter writer = new StreamWriter (path)) {
			writer.WriteLine (Name.text ); //0
			writer.WriteLine (Category.value.ToString());
			writer.WriteLine (Mass.text);  //2
			writer.WriteLine (isToggleable.isOn.ToString());
			writer.WriteLine (PassiveSig.text); //4
			writer.WriteLine (CrewRequired.text);
			writer.WriteLine (Quarters.text ); //6
			writer.WriteLine (LifeSupport.text);
			writer.WriteLine (PowerReq.text); //8
			writer.WriteLine (isControl.isOn.ToString());
			writer.WriteLine (isFlagControl.isOn.ToString() ); //10
			writer.WriteLine (HTK.value);
			writer.WriteLine (isEngine.isOn.ToString()); //12
			writer.WriteLine (Thrust.text);
			writer.WriteLine (TurnThrust.text); //14
			writer.WriteLine (FuelConsumption.text); //15
		}
		Debug.Log ("Saved " + Name.text);
	}

	public void Clear(){
		Name.text = "";
		Mass.text = "";
		PassiveSig.text = "";
		CrewRequired.text = "";
		Quarters.text = "";
		LifeSupport.text = "";
		PowerReq.text = "";
		Thrust.text = "";
		TurnThrust.text = "";
		TurnThrust.text = "";
		FuelConsumption.text = "";
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
