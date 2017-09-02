using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


public enum ShipPrefabTypes{
	DEF=0, //defensive wall ship
	DD=1, //destroyer 
	CS=2, //strike cruiser
	DN=3, //dreadnought
	CV=4, //carrier
	FR=21,
	CT=22

}





public abstract class ShipAbstract : MonoBehaviour, ICAPTarget {

	//Static
	public static ShipEvent OnDeath = new ShipEvent();

	//Resources
	public static GameObject Explosion;
	public	static GameObject EscapePod;
	public static GameObject Debris; //spawns on death.
	public static GameObject Torpedo;

	public static List<GameObject> ShipPrefabs = new List<GameObject>();
	public static Dictionary<ShipPrefabTypes, GameObject> ShipTypeDict = new Dictionary<ShipPrefabTypes, GameObject>();

	public bool[,] Armor;

	public string ShipName;

	public ShipClass shipClass;


	public FAC faction;
	public GameObject stand;
	public LineRenderer standlr;
	public Color c;

	public void DealDamage(float dam, Vector3 origin, Transform en){
		shipClass.Damage (dam, origin, en);
			}

	public void DealPhysicsDamage(float dam, Vector3 origin, float fMag, Transform s){
		shipClass.PhysicsDamage (dam,origin,fMag, s);
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
		SetupArmor (12, 8);
		if (Explosion == null)
			LoadResources ();
		c = FactionMatrix.FactionColors [(int)faction];
		ShipName = NameManager.AssignName(this);
		shipClass = gameObject.GetComponent<ShipClass> ();
		stand = new GameObject();
		standlr = stand.AddComponent<LineRenderer> ();
		stand.transform.position = new Vector3 (this.transform.position.x, 0f, transform.position.z);
		stand.transform.parent = transform;
		SetupStand (c);
	}
	public ShipPrefabTypes BaseType;

	public void SetupArmor(int c, int r){
		Armor = new bool[c, r];
		for (int x = 0; x < Armor.GetLength(0); x += 1) {
			for (int y = 0; y < Armor.GetLength(1); y += 1) {
				Armor [x, y] = true;
			}
		}
		Debug.Log("Initial: \n" + BuildArmorString());
	}

	public void TestArmor(){
		DamageArmor (SpaceGun.Pattern);
	}

	public void DamageArmor(List<Vector2> pattern){
		int startX = Random.Range (0, Armor.GetLength (0) - 1);
		int startY = 0;
		for(startY = Armor.GetLength(1)-1; startY >= 0; startY--){
			if (Armor [startX, startY] == true) {
				break;
			}
		}
		foreach (var v in pattern) {
			if (Armor [startX + (int)v.x, startY + (int)v.y] != null) {
				try {
					Armor [startX + (int)v.x, startY + (int)v.y] = false;
				} catch {

				}
			}
		}
		Debug.Log("Damaged: \n" + BuildArmorString());
	}

	/*string BuildArmorString(){
		string a = System.String.Empty;
		for (int y = 0; y < Armor.GetLength (1); y++) {
			for (int x = 0; x < Armor.GetLength (0); x++) {
				if (Armor [x, y]) {
					a += " O ";
				} else {
					a += " X ";
				}
			}
			a += "\n";
		}
		Debug.Log (a);
		return a;
	}
*/
	string BuildArmorString(){
			StringBuilder a = new StringBuilder();
			for (int y = 0; y < Armor.GetLength (1); y++) {
				for (int x = 0; x < Armor.GetLength (0); x++) {
					if (Armor [x, y]) {
						a.Append('O');
					} else {
						a.Append('X');
					}
				}
				a.AppendLine();
			}
			return a.ToString();
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
