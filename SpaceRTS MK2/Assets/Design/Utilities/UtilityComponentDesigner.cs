using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using JetBrains.Annotations;

public class UtilityComponentDesigner : MonoBehaviour {

	
	void LoadToDropdown(Enum e, Dropdown d)
	{
		List<string>Names = new List<string>();

		foreach (var v in Enum.GetValues(e.GetType()))
		{
			Names.Add(v.ToString());
		}
		d.ClearOptions();
		d.AddOptions(Names);
	}
	
	
	// Use this for initialization
	void Awake () {
		Comp = new ShipComponents();
		
		MassInput.onValueChanged.AddListener(UpdateMass);
		
		AbilityDropdown.LoadEnum(typeof(AbilityCats));
		ComponentCategoryDropdown.LoadEnum(typeof(CompCategory));
		
		AddAbilityButton.onClick.AddListener(AddAbility);
		SaveComponentButton.onClick.AddListener(SaveComponent);
		
	}

	void Start()
	{
		LoadComponents();
	}
	
	// Update is called once per frame
	void Update () {
		List<string> Descriptions = new List<string>();
		Descriptions.AddRange(ShipComponents.AbilityDescriptions[(AbilityCats)AbilityDropdown.value]);
//		Debug.Log(Descriptions.Count);
		for (int i = 0; i < 4; i++)
		{
			DescriptionTexts[i].text = Descriptions[i];
		}
		AddedAbilities.text = "";
		foreach (var v in Comp.Abilities.Values)
		{
			string s = string.Format("{0}: Rating 1: {1} | Rating 2: {2} | Rating 3: {3} | Thrust: {4}\n", v.AbilityType.ToString(), v.Rating, v.Rating2, v.Rating3, v.thrust);
			AddedAbilities.text += s;
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//UI 
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public Dropdown AbilityDropdown;
	public Dropdown ComponentCategoryDropdown;

	public GameObject AbilitiesParent;
	
	//Component
	public InputField ComponentName;
	public InputField Description;

	public InputField MassInput;
	public InputField CrewInput;
	public InputField MaintInput;
	
	//Abilities
	public List<InputField> InputFields;
	public List<Text> DescriptionTexts;
	
	//Button
	public Button AddAbilityButton;
	public Button SaveComponentButton;

	public Text AddedAbilities;

	////////////////////////////////////////////////////////////////////////////////////////////////////
	//Backing Fields 
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public List<ShipComponents> LoadedComponents = new List<ShipComponents>();
	
	public ShipComponents Comp;
	public List<Ability> AbilitiesToAdd = new List<Ability>();

	public AbilityCats CurrentAbilityCat()
	{
		return (AbilityCats) AbilityDropdown.value;
	}
	
	
	public void AddAbility()
	{
		if (string.IsNullOrEmpty(MassInput.text))
		{
			Debug.LogError("Must set mass before adding ability.");
			return;
		}
		List<float> inputs = new List<float>();
		foreach (InputField f in InputFields)
		{
			float i = 0;
			string s = f.text;
			if (!String.IsNullOrEmpty(s))
			{
				i = float.Parse(s);
			}
			inputs.Add(i);
		}
		Comp.AddAbility(CurrentAbilityCat(),inputs[0], inputs[1], inputs[2]);
		ResetInputs();
	}

	

	void ResetInputs()
	{
		foreach (var v in InputFields)
		{
			v.text = "";
		}
	}

	public void SaveComponent()
	{
		UpdateMass();
		Comp.Name = ComponentName.text;
		Comp.Description = Description.text;
		Comp.CrewRequired = int.Parse(CrewInput.text);
		Comp.Category = GetComponentCategory();
		Comp.MaintReq = int.Parse(MaintInput.text);
		
		Output();

	}

	void UpdateMass(string s = "")
	{
		Comp.Mass = int.Parse(MassInput.text);
	}
	
	CompCategory GetComponentCategory()
	{
		return (CompCategory) ComponentCategoryDropdown.value;
	}





	public void LoadComponentToScreen(ShipComponents c)
	{
		Comp = c;
		ComponentName.text = Comp.Name;
		MassInput.text = Comp.Mass.ToString();
		Description.text = Comp.Description;
		CrewInput.text = Comp.CrewRequired.ToString();
		ComponentCategoryDropdown.value = (int)Comp.Category;
		MaintInput.text = Comp.MaintReq.ToString();

	}
	
	
	
	public void Output(){
		Debug.Log ("Printing blueprint of " + ComponentName + " to text file..");
		string path="Assets/Output/Components/Components" + ".txt";
		using(StreamWriter writer = new StreamWriter(path)){
			writer.WriteLine ("#"); //seperator
			writer.WriteLine(Comp.Name);
			writer.WriteLine(Comp.Description);
			writer.WriteLine(Comp.Category);
			writer.WriteLine(Comp.Mass);
			writer.WriteLine(Comp.CrewRequired);
			writer.WriteLine(Comp.MaintReq);
			writer.WriteLine("$");
			
			foreach (var v in Comp.Abilities.Values) {
					writer.WriteLine(string.Format("{0}|: Rating 1: |{1}| Rating 2: |{2}| Rating 3: |{3}| Thrust: {4}\n", v.AbilityType.ToString(), v.Rating, v.Rating2, v.Rating3, v.thrust));
			} //0, 2, 4 , 6
			writer.Close ();
		}
		Debug.Log ("Done. Check " + path);
	}

	
	
	public void LoadComponents()
	{
	string path="Assets/Output/Components/Components.txt";
	string file = File.ReadAllText(path);
	List<string>components = new List<string>();
	components.AddRange(file.Split('#'));
	foreach (string s in components)
		{
			ShipComponents c = new ShipComponents();
		string fields = s.Split('$')[0];
			string	abilities = s.Split('$')[1];
			
			List<string> FieldsSplit = new List<string>();
			FieldsSplit.AddRange(fields.Split('\n'));
			
			List<string> AbilitiesSplit = new List<string>(); 
			AbilitiesSplit.AddRange(abilities.Split('\n'));

			c.Name = fields[0].ToString();
			c.Description = fields[1].ToString();
		//	c.Category = Enum.Parse(, fields[2].ToString());
			c.Mass = int.Parse(fields[3].ToString());
			c.CrewRequired = int.Parse(fields[4].ToString());
			c.MaintReq = int.Parse(fields[5].ToString());

			foreach (string m in AbilitiesSplit)
			{
				
				List<string>split = new List<string>();
				split.AddRange(m.Split('|'));
				c.AddAbility((AbilityCats)Enum.Parse(typeof(CompCategory), (split[0])), float.Parse(split[2]), float.Parse(split[4]), float.Parse(split[6]));
			}
			LoadedComponents.Add(c);
		}
		if (LoadedComponents.Count > 0)
		{
			LoadComponentToScreen(LoadedComponents[0]);
		}
	}
	
}
