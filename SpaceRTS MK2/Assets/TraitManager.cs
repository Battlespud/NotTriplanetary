using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class TraitManager : MonoBehaviour {
	Trait SelectedTrait;

	public GameObject ButtonPrefab;
	public GameObject ButtonsParent;
	public InputField Name;
	public InputField Description;
	public List<InputField>Aspects = new List<InputField>();
	public List<Text>AspectStrings = new List<Text>();
	public List<GameObject>Buttons = new List<GameObject>();
	public Text Summary;

	// Use this for initialization
	void Start () {
		Trait.Load ();
		ButtonPrefab = Resources.Load<GameObject>("Button") as GameObject;
		UpdateTraits ();
		for (int i = 0; i < Aspects.Count; i++) {
			AspectStrings [i].text = Character.PersonalityAspectsStrings [i];
				}
		}


	void UpdateTraits(){
		int yOff = -45;
		int interval = 1;
		Trait.Load ();
		foreach (GameObject g in Buttons) {
			Destroy (g);
		}

		Buttons.Clear ();
		foreach (Trait d in Trait.Traits) {
			GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
			Buttons.Add (g);
			RectTransform h = g.GetComponent<RectTransform> ();
			TraitButtonManager manager = g.AddComponent<TraitButtonManager> ();
			manager.Manager = this;
			manager.Assign(this,d);
			h.SetParent (ButtonsParent.transform);
			h.rotation = Camera.main.transform.rotation;
			h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
			h.sizeDelta = new Vector2 (200f, 35f);
			h.localScale = new Vector3 (1f, 1f, 1f);
			interval++;
		}
		if (!string.IsNullOrEmpty(SelectedTrait.Name)) {
			Name.text = SelectedTrait.Name;
			Description.text = SelectedTrait.Description;
			for (int i = 0; i < Aspects.Count; i++) {
				AspectStrings [i].text = Character.PersonalityAspectsStrings [i];
				Aspects [i].text = SelectedTrait.PersonalityModifiers [i].ToString ();
			}
		}
		UpdateSelectedTraitSampleSummary (" ");
	}

	void UpdateSelectedTraitSampleSummary(string proxy){
		Summary.text = SelectedTrait.Summary;
	}

	public void SelectTrait(Trait t){
		SelectedTrait = t;
		UpdateTraits ();
	}

	public void SaveAsNewTrait(){
		
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Traits/" ); 
		List<string> traits = new List<string> ();
		traits.AddRange(File.ReadAllLines(path+"Traits.txt"));
		string NewTrait = string.Format ("{0}+{{{1},{2},{3},{4},{5},{6}}}+{7}", Name.text, Aspects [0].text, Aspects [1].text, Aspects [2].text, Aspects [3].text, Aspects [4].text, Aspects [5].text, Description.text);
		traits.Add (NewTrait);
		traits = traits.OrderBy(x => x).ToList();
		using (StreamWriter writer = new StreamWriter (path + "Traits.txt")) {
			foreach (string s in traits) {
				writer.WriteLine (s);
			}
		}
		UpdateTraits ();
	}

	public void Overwrite(){
		if (!string.IsNullOrEmpty (SelectedTrait.Name)) {
			string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Traits/"); 
			List<string> traits = new List<string> ();
			traits.AddRange (File.ReadAllLines (path + "Traits.txt"));
			int i = traits.IndexOf (SelectedTrait.Raw);

			string NewTrait = string.Format ("{0}+{{{1},{2},{3},{4},{5},{6}}}+{7}", Name.text, Aspects [0].text, Aspects [1].text, Aspects [2].text, Aspects [3].text, Aspects [4].text, Aspects [5].text, Description.text);
			traits [i] = NewTrait;
			traits = traits.OrderBy (x => x).ToList ();
			using (StreamWriter writer = new StreamWriter (path + "Traits.txt")) {
				foreach (string s in traits) {
					writer.WriteLine (s);
				}
			}
			UpdateTraits ();
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
