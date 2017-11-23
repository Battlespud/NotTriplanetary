using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DrawTest : MonoBehaviour {
	static List<Pixel> OverlayPixels = new List<Pixel> ();
	static List<Pixel> BackgroundPixels = new List<Pixel>();
	static List<Pixel> GridPixels = new List<Pixel>();

	static int gcf(int a, int b)
	{
		while (b != 0)
		{
			int temp = b;
			b = a % b;
			a = temp;
		}
		return a;
	}

	static int lcm(int a, int b)
	{
		return (a / gcf(a, b)) * b;
	}

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


	public Image RoomsOverlay;
	public Image Grid;
	public Image Background;
	public static string[] CompNames = new string[3]{"W","U","D"};

	void DrawQuad(Rect position, Color color) {
		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0,0,color);
		texture.Apply();
		GUI.skin.box.normal.background = texture;
		GUI.Box(position, GUIContent.none);
	}

	int RoomW = 50;
	int RoomH = 25;

	Texture2D Blueprint;
	Texture2D BackgroundTex;
	Texture2D GridTex;

	public struct Pixel{
		public int x;
		public int y;
		public Color c;
		public Pixel(int a, int b, Color d,List<Pixel> list){
			x = a;
			y = b;
			c = d;
			list.Add(this);
		}
	}

	void DrawLetter(char ch, Color c, List<Pixel> list, Vector2 center){
		if (ch == 'U') {
			Pixel p = new Pixel ((int)center.x - 1, (int)center.y, c, list);
			p = new Pixel ((int)center.x - 1, (int)center.y - 1, c, list);
			p = new Pixel ((int)center.x - 1, (int)center.y + 1, c, list);

			p = new Pixel ((int)center.x, (int)center.y - 1, c, list);

			p = new Pixel ((int)center.x + 1, (int)center.y, c, list);
			p = new Pixel ((int)center.x + 1, (int)center.y + 1, c, list);
			p = new Pixel ((int)center.x + 1, (int)center.y - 1, c, list);
		} else if (ch == 'D') {
			Pixel p = new Pixel ((int)center.x - 1, (int)center.y, c, list);
			p = new Pixel ((int)center.x - 1, (int)center.y - 1, c, list);
			p = new Pixel ((int)center.x - 1, (int)center.y + 1, c, list);

			p = new Pixel ((int)center.x, (int)center.y + 1, c, list);
			p = new Pixel ((int)center.x, (int)center.y - 1, c, list);

			p = new Pixel ((int)center.x+1, (int)center.y, c, list);
		} else if (ch == 'W') {
			Pixel	p = new Pixel ((int)center.x, (int)center.y, c, list);

			p = new Pixel ((int)center.x - 1, (int)center.y - 1, c, list);
			p = new Pixel ((int)center.x + 1, (int)center.y - 1, c, list);

			p = new Pixel ((int)center.x+2, (int)center.y, c, list);
			p = new Pixel ((int)center.x-2, (int)center.y, c, list);

		}

	}

	void DrawRectangle(Pixel start, int x, int y, Color c, List<Pixel> list, bool fill = false, Color? fillColor = null){
		if (fill) {
			if(fillColor != null)
				FillIn (new Rect (start.x, start.y, x, y), (Color)fillColor, list);
			else
				FillIn (new Rect (start.x, start.y, x, y), c, list);
		}
		for (int i = 0; i < x; i++) {
			Pixel f = new Pixel (start.x + i, start.y, Color.black,list);
			Pixel u = new Pixel (start.x + i, start.y + y, Color.black,list);
		}
		for (int i = 0; i < y; i++) {
			Pixel f = new Pixel (start.x, start.y + i, c,list);
			Pixel u = new Pixel (x+start.x, start.y + i, c,list);
		}
	}

	void ColorBackground(Color c){
		for (int x = 0; x < BackgroundTex.width; x++) {
			for (int y = 0; y < BackgroundTex.height; y++) {
				new Pixel (x, y, c,BackgroundPixels);
			}
		}
	}

	void ColorForeground(Color c){
		for (int x = 0; x < Blueprint.width; x++) {
			for (int y = 0; y < Blueprint.height; y++) {
				new Pixel (x, y, c,OverlayPixels);
			}
		}
	}

	void DrawGrid(Color c){
		int gridS = 25; //size of each side in pixels
		for (int x = 0; x < BackgroundTex.width / gridS; x++) {
			for (int y = 0; y < BackgroundTex.height / gridS; y++) {
				DrawRectangle (new Pixel(x*gridS,y*gridS,Color.white,GridPixels),gridS,gridS, Color.white,GridPixels);
			}
		}
	}

	void FillIn(Rect r, Color c, List<Pixel> list){
		for (int x = (int)r.x; x < r.x + r.width; x++) {
			for (int y = (int)r.y; y < r.y + r.height; y++) {
				Pixel p = new Pixel (x, y, c, list);
			}
		}
	}

	void DesignBlueprint(){
		ColorBackground (Color.black);
		ColorForeground (Color.clear);
		DrawConvergingTriangle(new Pixel(0,0,Color.white,GridPixels),new Pixel(GridTex.width,0,Color.white,GridPixels),Color.green,GridPixels,false);
		DrawGrid (Color.white);
		int offset = 0;
		FillIn (new Rect (0, 0, Blueprint.width, Blueprint.height), Color.grey, OverlayPixels);
		for (int i = 0; i < Rooms.Count / 2; i++) {
			Pixel h = new Pixel (0, (RoomH + offset) * i, Color.green,OverlayPixels);
			Vector2 center = new Vector2(h.x + .5f * RoomW, h.y + .5f * RoomH);
			DrawRectangle (h,RoomW,RoomH,Rooms[i].c,OverlayPixels,true, Color.white);
			DrawLetter (Rooms [i].comp [0], Color.black, OverlayPixels, center);
		}
		int secondIndexer = Rooms.Count / 2;
		for (int i = 0; i < Rooms.Count / 2; i++) {
			Pixel h = new Pixel (Blueprint.width - (RoomW + offset), (RoomH + offset) * i, Color.red,OverlayPixels);
		//	GUI.Label (new Rect ( h.x + .5f * RoomW, h.y + .5f * RoomH, RoomW * .75f, RoomH * .75f), Rooms [i].comp);
			Vector2 center = new Vector2(h.x + .5f * RoomW, h.y + .5f * RoomH);
			DrawRectangle (h,RoomW,RoomH,Rooms[i+secondIndexer].c,OverlayPixels,true,Color.white);
			DrawLetter (Rooms [i+secondIndexer].comp [0], Color.black, OverlayPixels, center);

		}

		foreach (Pixel p in OverlayPixels) {
			Blueprint.SetPixel (p.x, p.y, p.c);
		}
		Blueprint.Apply ();
		foreach (Pixel p in BackgroundPixels) {
			BackgroundTex.SetPixel (p.x, p.y, p.c);
		}
		foreach (Pixel p in GridPixels) {
			GridTex.SetPixel (p.x, p.y, p.c);
		}
		GridTex.Apply ();
		BackgroundTex.Apply ();
		Sprite Over = Sprite.Create (Blueprint, new Rect (0f, 0f, Blueprint.width, Blueprint.height), new Vector2 (.5f, .5f), 100f);
		RoomsOverlay.sprite = Over;
		RoomsOverlay.SetNativeSize ();
		RoomsOverlay.transform.localPosition = new Vector3 (0f, 0f, 0f);
		Sprite Back = Sprite.Create (BackgroundTex, new Rect (0f, 0f, BackgroundTex.width, BackgroundTex.height), new Vector2 (.5f, .5f), 100f);
		Background.sprite = Back;
		Background.SetNativeSize ();
		Background.transform.localPosition = new Vector3 (0f, 0f, 0f);
		Sprite gridSprite = Sprite.Create (GridTex, new Rect (0f, 0f, GridTex.width, GridTex.height), new Vector2 (.5f, .5f), 100f);
		Grid.sprite = gridSprite;
		Grid.SetNativeSize ();
		Grid.transform.localPosition = new Vector3 (0f, 0f, 0f);
	}

	Sprite Ship;

	// Use this for initialization
	void OnGUI () {
		GUI.Label (new Rect (0,0, RoomW * .75f, RoomH * .75f), "Uh hello");

		if (RenderNow) {
			RenderNow = false;
			Rooms.Clear ();
			BackgroundPixels.Clear ();
			OverlayPixels.Clear ();
			GridPixels.Clear ();

			MakeRooms ();

			int height = Rooms.Count / 2 * RoomH + RoomH*4;
			Blueprint = new Texture2D (150, height);

			BackgroundTex = new Texture2D ((int)(Blueprint.width*1.5), (int)(Blueprint.height*1.5));
			GridTex = new Texture2D (Blueprint.width, Blueprint.height);
			MakeRooms ();
			DesignBlueprint ();
		}
	}

	void DrawConvergingTriangle(Pixel LB, Pixel RB, Color c, List<Pixel> list, bool fill = false){
		//draw base line
		int width = RB.x - LB.x;
		int level = 0;
		while (LB.x + level != RB.x - level && level < 100) {
			DrawLine (LB.x + level, RB.x - level,LB.y+level,Color.magenta,GridPixels);
			level++;
		}

	}

	void DrawLine(int xStart, int xEnd, int y, Color c, List<Pixel> list){
		for (int x = xStart; x <= xEnd; x++) {
			Pixel p = new Pixel (x, y, c, list);
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
		int numRooms = Random.Range (12, 26);
		for (int i = 0; i < numRooms; i++) {
			Room r = new Room ((RoomStatus)Random.Range (0, 4));
		}
	}

}
