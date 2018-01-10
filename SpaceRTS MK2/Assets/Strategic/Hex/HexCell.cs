using UnityEngine;


public enum HexTypes
{
	SOLAR,
	EXTRASOLAR
}




public class HexCell : MonoBehaviour
{


	public HexTypes HexType;
	public bool Passable = true;
	
	
	
	
	
	public HexCoordinates coordinates;
	public HexGrid Grid;
	

	
	public Color color;
}