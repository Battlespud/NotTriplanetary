using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DrawTest : MonoBehaviour {
	static List<Pixel> Pixels = new List<Pixel> ();

	public enum RoomStatus{
		NONE,
		DESTROYED,
		IRRADIATED,
		VACUUM
	}


	public struct Room
	{
		public RoomStatus status;
		public Color c;
		public string comp;


		public Room(RoomStatus r){
			status = r;
			c = Color.green;
			switch(status){
			case(RoomStatus.NONE):{
					c = Color.green;
					break;
				}
			case(RoomStatus.DESTROYED):{
					c = Color.red;
					break;
				}
			case(RoomStatus.IRRADIATED):{
					c = Color.yellow;
					break;
				}
			case(RoomStatus.VACUUM):{
					c = Color.cyan;
					break;
				}
			}
			comp = DrawTest.CompNames[Random.Range(0,3)];
				
			DrawTest.Rooms.Add(this);
		}

	}

	public static List<Room> Rooms = new List<Room> ();


	public Image PlsWork;
	public static string[] CompNames = new string[3]{"Laser","Missiles","Sensors"};

	void DrawQuad(Rect position, Color color) {
		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0,0,color);
		texture.Apply();
		GUI.skin.box.normal.background = texture;
		GUI.Box(position, GUIContent.none);
	}

	const int RoomW = 120;
	int RoomH = 50;

	Texture2D Blueprint;


	public struct Pixel{
		public int x;
		public int y;
		public Color c;
		public Pixel(int a, int b, Color d ){
			x = a;
			y = b;
			c = d;
			DrawTest.Pixels.Add(this);
		}
	}

	void DrawRectangle(Pixel start, int x, int y, Color c){
		for (int i = 0; i < x; i++) {
			Pixel f = new Pixel (start.x + i, start.y, c);
			Pixel u = new Pixel (start.x + i, start.y + y, c);
		}
		for (int i = 0; i < y; i++) {
			Pixel f = new Pixel (start.x, start.y + i, c);
			Pixel u = new Pixel (x+start.x, start.y + i, c);
		}
	}



	void DesignBlueprint(){
		RoomH = (int)( .8 * Blueprint.height / (Rooms.Count / 2));

		for (int i = 0; i < Rooms.Count / 2; i++) {
			Pixel h = new Pixel (0, (RoomH + 1) * i, Color.green);
			GUI.Label(new Rect (h.x + .5f * RoomW, h.y + .5f * RoomH, RoomW * .75f, RoomH * .75f), Rooms [i].comp);
			DrawRectangle (h,RoomW,RoomH,Rooms[i].c);
		}
		int secondIndexer = Rooms.Count / 2;
		for (int i = 0; i < Rooms.Count / 2; i++) {
			Pixel h = new Pixel (Blueprint.width - (RoomW + 1), (RoomH + 1) * i, Color.red);
			GUI.Label (new Rect ( h.x + .5f * RoomW, h.y + .5f * RoomH, RoomW * .75f, RoomH * .75f), Rooms [i].comp);

			DrawRectangle (h,RoomW,RoomH,Rooms[i+secondIndexer].c);
		}

		foreach (Pixel p in Pixels) {
			Blueprint.SetPixel (p.x, p.y, p.c);
		}
		Blueprint.Apply ();
		Sprite s = Sprite.Create (Blueprint, new Rect (0f, 0f, Blueprint.width, Blueprint.height), new Vector2 (.5f, .5f), 100f);
		PlsWork.sprite = s;
		PlsWork.SetNativeSize ();
		PlsWork.transform.localPosition = new Vector3 (0f, 0f, 0f);
	}

	Sprite Ship;

	// Use this for initialization
	void OnGUI () {
		GUI.Label (new Rect (0,0, RoomW * .75f, RoomH * .75f), "Uh hello");

		if (RenderNow) {
			RenderNow = false;
			Rooms.Clear ();
			Pixels.Clear ();
			Blueprint = new Texture2D (300, 900);
			MakeRooms ();
			DesignBlueprint ();
		}
	}

	public bool RenderNow = true;


	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space))
			RenderNow = true;
	}
	void Start(){

	}


	void MakeRooms(){
		int numRooms = Random.Range (12, 50);
		for (int i = 0; i < numRooms; i++) {
			Room r = new Room ((RoomStatus)Random.Range (0, 4));
		}
	}

}
