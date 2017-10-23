using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Events;


public enum ShipPrefabTypes{
	DEF=0, //defensive wall ship
	DD=1, //destroyer 
	CS=2, //strike cruiser	
	CV=3, //carrier
	DN=4, //dreadnought
	FR=21,
	CT=22

}





public abstract class ShipAbstract : MonoBehaviour, ICAPTarget, IContext {

	//Static
	public static ShipEvent OnDeath = new ShipEvent();


	public List<UnityAction> ContextActions(){
		return new List<UnityAction>{new UnityAction(OpenShipMenu)};
	}

	public GameObject getGameObject(){
		return gameObject;
	}

	public void OpenShipMenu(){
		ShipUI.active = !ShipUI.activeSelf;
		if(ShipUI)
			ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,BuildArmorString());
	}

	//Resources
	public static GameObject Explosion;
	public	static GameObject EscapePod;
	public static GameObject Debris; //spawns on death.
	public static GameObject Torpedo;

	public static List<GameObject> ShipPrefabs = new List<GameObject>();
	public static Dictionary<ShipPrefabTypes, GameObject> ShipTypeDict = new Dictionary<ShipPrefabTypes, GameObject>();
	public static GameObject CustomShipPrefab;

	public float[,] Armor;

	public string ShipName;

	public ShipClass shipClass;

	public List<Renderer> rens; 


	//Ship Window
	public GameObject ShipUI;
	public Text UIArmorText;
	public Text UIName;
	public Text UIDate;

	public string BuildDate;

	public FAC faction;
	public GameObject stand;
	public LineRenderer standlr;
	public Color c;

	public Text ArmorText;

	public void DealDamage(float dam, Vector3 origin, Transform en, List<Int2> pattern){
		shipClass.Damage (dam, origin, en, pattern);
			}

	public void DealPhysicsDamage(float dam, Vector3 origin, float fMag, Transform s, List<Int2> pattern){
		shipClass.PhysicsDamage (dam,origin,fMag, s, pattern);
	}

	public bool isHostile(FAC caller){
		return FactionMatrix.IsHostile (caller, faction);
	}

	public FAC GetFaction(){
		return faction;
	}

	public GameObject GetGameObject(){
		return gameObject;
	}

	void Awake(){
		if (Explosion == null)
			LoadResources ();
		ArmorText = gameObject.AddComponent<Text> ();
//		BuildDate = ClockStatic.clock.GetDate ();
		SetupArmor (4*(int)BaseType,2*(int)BaseType, 2.5f );
		//c = FactionMatrix.FactionColors [(int)faction];
		ShipName = NameManager.AssignName(this);
		shipClass = gameObject.GetComponent<ShipClass> ();
		stand = new GameObject();
		standlr = stand.AddComponent<LineRenderer> ();
		stand.transform.position = new Vector3 (this.transform.position.x, 0f, transform.position.z);
		stand.transform.parent = transform;
		SetupStand (c);

		if (ShipUI) {
//			UIDate.text = BuildDate;
			UIName.text = ShipName;
			ShipUI.active = false;
		}
	}
	public ShipPrefabTypes BaseType;

	public static List<Int2> Pattern = new List<Int2> (){new Int2 (0, 0), new Int2 (-2, 0), new Int2 (1, 0), new Int2 (2, 0),  new Int2 (-1, 0), new Int2 (0, -1), new Int2 (-2, -1), new Int2 (1, -1), new Int2 (2, -1),  new Int2 (-1, -1), new Int2 (0, -2), new Int2 (-2, -2), new Int2 (1, -2), new Int2 (2, -2),  new Int2 (-1, -2) , new Int2 (0, -3), new Int2 (-2, -3), new Int2 (1, -3), new Int2 (2, -3),  new Int2 (-1, -3)};
	public float integrity = 100f;


	public void SetupArmor(int c, int r, float armorStrength){
		Armor = new float[c, r];
		for (int x = 0; x < Armor.GetLength(0); x += 1) {
			for (int y = 0; y < Armor.GetLength(1); y += 1) {
				Armor [x, y] = armorStrength;
			}
		}
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,BuildArmorString());
		MaxArmor = armorStrength;
	}

	public void TestArmor(){
	//	DamageArmor (SpaceGun.staticPattern, 1);
		Debug.Log("Function is deprecated");
	}

	public float MaxArmor;

	//lowest layer to hull is always 0, negatives have penetrated armor entirely.
	public void DamageArmor(List<Int2> pattern, float dam){
		int startX = Random.Range (0, Armor.GetLength (0) - 1);
		int startY = 0;
		for(startY = Armor.GetLength(1)-1; startY >= 0; startY--){
			if (Armor [startX, startY] > 0f) {
				break;
			}
		}
		int HullBound = 0;
		float counter = 0;
		//adjust penetration profile by damage
		for(int i = 0; dam > 0; i++) { //TODO REFACTOR PLS PLS PLS ITS SO BAD
			if (i >= pattern.Count)
				i = 0;
			Int2 v = pattern[i]; 
			if (startY + v.y < HullBound) {
				if (dam >= 1f) {
					counter++;
					dam -= 1;
				} else {
					counter += dam;
					dam = 0f;
				}
			}
			if (startX + v.x < 0) {
				v.x += Armor.GetLength (0)-1; 
			}
			if (startX + v.x > Armor.GetLength(0)-1) {
				v.x -= Armor.GetLength (0)-1; 
			}
				if (startX + v.x >= 0 && startX + v.x < Armor.GetLength (0) && startY + v.y >= 0 && startY + v.y < Armor.GetLength (1)) {
					try {
					if(dam > Armor [startX + v.x, startY + v.y]){
						dam -= Armor [startX + v.x, startY + v.y];
						Armor [startX + v.x, startY + v.y] = 0f;
					}
					else{
						Armor [startX + v.x, startY + v.y] -= dam;
						dam = 0f;
					}
				} catch {
					Debug.Log ("Invalid armor coords");
					}
				}
		}
		Debug.Log ("Penetrating hits: " + counter);
			if (shipClass.usingTemplate) {
			ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,shipClass.TakeComponentDamage(counter));
			} else {
			integrity -= Random.Range (2f, 5f) * counter;
			}
		if(ShipUI)
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,BuildArmorString());
	}


	IEnumerator BuildArmorString(){
			StringBuilder a = new StringBuilder();
			for (int y = 0; y < Armor.GetLength (1); y++) {
				for (int x = 0; x < Armor.GetLength (0); x++) {
				if (Armor [x, y] == MaxArmor) {
					a.Append( "<color=green>□</color>");
				}
				else if (Armor[x,y] > 0f && Armor[x,y] < MaxArmor) {
					a.Append( "<color=yellow>□</color>");
					}
				else if (Armor[x,y] <= 0f) {
					a.Append( "<color=red>□</color>");
				}
				}
				a.AppendLine();
			}
		yield return Ninja.JumpToUnity;
			UIArmorText.text = a.ToString ();
			if (shipClass.usingTemplate) {
				UIDate.text = "Integrity: " + shipClass.Integrity*100 +"%";

			} else {
				UIDate.text = "Integrity: " + Mathf.RoundToInt (integrity).ToString () + "%";
			}

		ArmorText.text = a.ToString();
	}

	public void SetupStand(Color c){
		float size = 1f;
		if (BaseType == ShipPrefabTypes.DD)
			size = .5f;
		if (BaseType == ShipPrefabTypes.CS)
			size = .75f;
		RenderCircle (standlr, size);
		standlr.SetColors (c, c);
	}


	public void RegenColors(){
		c = FactionMatrix.FactionColors [(int)faction];
		foreach (Renderer r in rens) {
			r.material.color = c;
			r.material.SetColor ("_EmissionColor", new Color(c.r,c.g,c.b,.35f));
			r.material.EnableKeyword ("_EMISSION");
		}
		SetupStand (c);
	}
	void LoadResources(){
			Explosion = Resources.Load<GameObject> ("Explosion") as GameObject;
			EscapePod = Resources.Load<GameObject> ("EscapePod") as GameObject;
			Debris = Resources.Load<GameObject> ("Debris") as GameObject;
			Torpedo = Resources.Load<GameObject> ("Torpedo") as GameObject;
		foreach (ShipPrefabTypes val in System.Enum.GetValues(typeof(ShipPrefabTypes))) {
			ShipPrefabs.Add (Resources.Load<GameObject> (val.ToString()) as GameObject);
			ShipTypeDict.Add (val, ShipPrefabs[ShipPrefabs.Count-1] );
			//				Debug.Log ("Loaded " + val.ToString ());
		}
	}

	public void RenderCircle (LineRenderer l, float r) {
		int numSegments = 255;
		float radius = r;
		l.material = new Material(Shader.Find("Particles/Additive"));
		//	l.SetColors(Color.yellow, Color.yellow);
		l.SetWidth(0.025f, 0.025f);
		l.SetVertexCount(numSegments + 1);
		l.useWorldSpace = false;

		float deltaTheta = (float) (2.0 * Mathf.PI) / numSegments;
		float theta = 0f;

		for (int i = 0 ; i < numSegments + 1 ; i++) {
			float x = radius * Mathf.Cos(theta);
			float z = radius * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, 0, z);
			l.SetPosition(i, pos);
			theta += deltaTheta;
		}
	}


	// Use this for initialization
	public virtual void Start(){

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DieAbstract(){
		transform.position = new Vector3 (10f, 10000f, 0f);
		Destroy (gameObject);
	}
}
