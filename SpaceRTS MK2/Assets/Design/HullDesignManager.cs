using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class HullDesignManager : MonoBehaviour
{
	
	
	//UI Elements
	public InputField InputDesiredDisplacement;

	public Text AvailableSpaceText;
	public Text CurrentArmorLayersText;
	public Text MinimumArmorLayersText;
	public Button button;
	
	public  DesignerToken Designer;
	public  string HullName;
	public  HullTypes HullType;

	public int MinimumArmorLayers;
	public int DesiredDisplacement;
	public int AvailableSpace;
	public int ArmorLayer;
	public int ArmorLength;
	public int ArmorMass;
	public ArmorTypes ArmorType = ArmorTypes.STEEL;
	
	public List<Hardpoint> Hardpoints = new List<Hardpoint>();
	
	public int Length;
	public int Beam;

	public float LengthToBeamRatio;




	public void SetDisplacement(string input)
	{

			DesiredDisplacement = int.Parse(input);

		
		CalculateArmorRequirements();
		CalculateArmorMass();
		CalculateAvailableSpace();
		SetStats();
	}


	void CalculateArmorWidth(){
		int hs = DesiredDisplacement / 50;
		ArmorLength = (int)(1 + 1.1*(Mathf.Pow (hs, .55f))); //.66 works well
		if (ArmorLength <= 0)
			ArmorLength = 1;
	}
	
	void CalculateArmorRequirements()
	{
		MinimumArmorLayers = Mathf.FloorToInt(DesiredDisplacement/1000);
		if (MinimumArmorLayers <= 0)
			MinimumArmorLayers = 1;
		MinimumArmorLayers--;
		if (ArmorLayer < MinimumArmorLayers)
			ArmorLayer = MinimumArmorLayers;
		ArmorLayer = MinimumArmorLayers; //for testing

		CalculateArmorWidth();
	}

	void CalculateArmorMass()
	{
		float MassPerArmor = (float)ArmorType / 10;
//		Debug.Log(MassPerArmor);
		if (ArmorLayer > 0)
		{
			ArmorMass = (int) (50 / MassPerArmor * (ArmorLength * (ArmorLayer + (int) Mathf.Pow(ArmorLayer, .35f))));
		}
		else
		{
			ArmorMass = (int) (50 / MassPerArmor * (ArmorLength * (.66f + (int) Mathf.Pow(.66f, .35f))));
		}
	}

	void CalculateAvailableSpace()
	{
		int HardpointMass = 0;
		foreach (var v in Hardpoints)
		{
			HardpointMass += v.Size;
		}
		AvailableSpace = DesiredDisplacement - (ArmorMass + HardpointMass);
	}

	void UpdateUI()
	{
		AvailableSpaceText.text = AvailableSpace.ToString();
		CurrentArmorLayersText.text = ArmorLayer.ToString();
		MinimumArmorLayersText.text = MinimumArmorLayers.ToString();

	}

	void ProxySetDisplacement()
	{
		SetDisplacement(DesiredDisplacement.ToString());
	}

	void SetStats()
	{
		Length = ArmorLength * 20;
		LengthToBeamRatio = Mathf.Lerp(10f, 4f, (float) DesiredDisplacement / 45000);
		Beam = (int)(Length / LengthToBeamRatio);
	}
	
	// Use this for initialization
	void Start () {
		InputDesiredDisplacement.onValueChanged.AddListener(SetDisplacement);
		button.onClick.AddListener(ProxySetDisplacement);
		ArmorTypes ArmorType = ArmorTypes.STEEL;

	}
	
	
	
	// Update is called once per frame
	void Update () {
		
	}
	
	
	
	
	
	
	
}
