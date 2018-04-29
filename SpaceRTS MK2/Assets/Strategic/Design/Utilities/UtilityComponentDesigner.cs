using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using JetBrains.Annotations;
using System.Text;

public class UtilityComponentDesigner : MonoBehaviour
{

	public static UtilityComponentDesigner StaticComponentDesigner;
	
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
		StaticComponentDesigner = this;
		MassInput.onValueChanged.AddListener(UpdateMass);
		
		AbilityDropdown.LoadEnum(typeof(AbilityCats));
		ComponentCategoryDropdown.LoadEnum(typeof(CompCategory));
		
		AddAbilityButton.onClick.AddListener(AddAbility);
		SaveComponentButton.onClick.AddListener(SaveComponent);

		foreach (InputField f in InputFields)
		{
			f.onValueChanged.AddListener(CalculateThrust);
		}
		
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

	public GameObject ButtonsParent;
	public GameObject Prefab;
	
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

	void CalculateThrust(string ignore = "")
	{

		if (string.IsNullOrEmpty(MassInput.text) || string.IsNullOrEmpty(InputFields[0].text) || string.IsNullOrEmpty(InputFields[1].text))
			return;
			
		float m = 0;

		int mass = 0;

			mass = int.Parse(MassInput.text);
		
		float rating1 = 0;

			rating1 = float.Parse(InputFields[0].text);
		
		float rating2 = 0;

			rating2 = int.Parse(InputFields[1].text);
		
		switch (CurrentAbilityCat())
		{
			case AbilityCats.THRUST:
				m = (mass / 50f) * (1 + rating1 * rating2);
				break;
			case AbilityCats.STRATEGICMOVE:
				m =(int)((mass / 50) * rating1);
				break;
			case AbilityCats.TURN:
				break;
			case AbilityCats.CONTROL:
				break;
			case AbilityCats.CREW:
				break;
			case AbilityCats.SENSOR:
				break;
			case AbilityCats.USEFUEL:
				break;
			case AbilityCats.FUEL:
				break;
			case AbilityCats.CARGO:
				break;
			case AbilityCats.MAINT:
				break;
			case AbilityCats.POW:
				break;
			case AbilityCats.SHIELD:
				break;
			case AbilityCats.EXPLODE:
				break;
			case AbilityCats.ARMOR:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		InputFields[3].text = m.ToString();
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
		ShipComponents.AllShipComponents.Add(Comp);
		Comp = new ShipComponents();
		ResetInputs();
		Output();

	}

	void UpdateMass(string s = "")
	{
		Comp.Mass = int.Parse(MassInput.text);
		CalculateThrust();
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
		Debug.Log ("Printing blueprint of " + ShipComponents.AllShipComponents.Count + " components to text file..");
		string path="Assets/Output/Components/Components" + ".txt";
		using(StreamWriter writer = new StreamWriter(path)){

		foreach (var Comp in ShipComponents.AllShipComponents)
		{
				writer.WriteLine ("#"); //seperator
				writer.WriteLine("+Name: " + Comp.Name);
				writer.WriteLine("+Description: " + Comp.Description);
				writer.WriteLine("+Component Category: " + (int)Comp.Category);
				writer.WriteLine("+Mass: "+Comp.Mass);
				writer.WriteLine("+Crew: "+Comp.CrewRequired);
				writer.WriteLine("+Maint: "+Comp.MaintReq);
				writer.WriteLine("$");
			
				foreach (var v in Comp.Abilities.Values) {
					writer.WriteLine(string.Format("{0}|: Rating 1: |{1}| Rating 2: |{2}| Rating 3: |{3}| Thrust: {4}", v.AbilityType.ToString(), v.Rating, v.Rating2, v.Rating3, v.thrust));
				} //0, 2, 4 , 6
		}
			writer.Close ();
			Reload();
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
		if (string.IsNullOrEmpty(s))
			continue;
		
	//	Debug.LogError(s); //ok
		
		ShipComponents c = new ShipComponents();
		string fields = s.Split('$')[0];
		string	abilities = s.Split('$')[1];
			
		//	Debug.LogError(fields); //ok
		
			List<string> FieldsSplit = new List<string>();
			FieldsSplit.AddRange(fields.Split('\n'));
			
		//	Debug.LogError(FieldsSplit.Count);
		
		Debug.LogWarning(FieldsSplit[3]);

		
			for (int i = 1; i < FieldsSplit.Count; i++)
			{
				if (string.IsNullOrEmpty(FieldsSplit[i]))
					continue;
				string cleaned = FieldsSplit[i].Split(':')[1].Trim();
				FieldsSplit[i] = cleaned;
			}

		FieldsSplit.RemoveAt(0);
	//	Debug.LogError(FieldsSplit[0]);

		try
		{
			c.Name = FieldsSplit[0].ToString();
		}
		catch
		{
			Debug.LogError(FieldsSplit[0].ToString());
		}
		try
		{
			c.Description = FieldsSplit[1].ToString();
		}
		catch
		{
			Debug.LogError(FieldsSplit[1].ToString());
		}
		try
		{
			//	c.Category = Enum.Parse(, fields[2].ToString());
		}
		catch
		{
			Debug.LogError(FieldsSplit[2].ToString());
		}
		try
		{
			c.Mass = int.Parse(FieldsSplit[3].ToString());
		}
		catch
		{
			Debug.LogError(FieldsSplit[3].ToString());
		}
		try
		{
			c.CrewRequired = int.Parse(FieldsSplit[4].ToString());
		}
		catch
		{
			Debug.LogError(FieldsSplit[4].ToString());
		}
		try
		{
			c.MaintReq = int.Parse(FieldsSplit[5].ToString());
		}
		catch
		{
			Debug.LogError(FieldsSplit[5].ToString());
		}

		List<string> AbilitiesSplit = new List<string>(); 
			AbilitiesSplit.AddRange(abilities.Trim().Split('\n'));
			//AbilitiesSplit.RemoveAt(0);	
		Debug.LogError(c.Name + " has " + AbilitiesSplit.Count);
			foreach (string m in AbilitiesSplit)
			{
				if (string.IsNullOrEmpty(m))
					continue;
				List<string>split = new List<string>();
				split.AddRange(m.Trim().Split('|'));
				Debug.LogError(split[0]);

				try
				{
					c.AddAbility((AbilityCats) int.Parse(split[0]), float.Parse(split[2]), float.Parse(split[4]),float.Parse(split[6]));
				}
				catch
				{
					c.AddAbility((AbilityCats)Enum.Parse(typeof(AbilityCats), (split[0].Trim())), float.Parse(split[2]), float.Parse(split[4]),float.Parse(split[6]));
				}
			}
			LoadedComponents.AddExclusive(c);
			ShipComponents.AllShipComponents.AddExclusive(c);
		}
		if (LoadedComponents.Count > 0)
		{
		//	LoadComponentToScreen(LoadedComponents[0]);
		}

		SetupButtons();
	}
	
	List<GameObject> Buttons = new List<GameObject>();

	void SetupButtons()
	{
		int i = 0;
		foreach (var  c in ShipComponents.AllShipComponents)
		{
			GameObject g = GameObject.Instantiate(Prefab, ButtonsParent.transform);
			Buttons.Add(g);
			g.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, i*g.GetComponent<RectTransform>().sizeDelta.y);
			g.GetComponent<UtilityComponentButton>().Setup(c);
			i--;
		}
		//Resize scrollview to fit amount of components added.
		ButtonsParent.GetComponent<RectTransform>().sizeDelta = new Vector2(ButtonsParent.GetComponent<RectTransform>().sizeDelta.x, 60*i*-1);
	}

	void Reload()
	{
		foreach (var g in Buttons)
		{
			Destroy(g);
		}
		Buttons.Clear();
		ShipComponents.AllShipComponents.Clear();
		LoadComponents();
	}
	
}
