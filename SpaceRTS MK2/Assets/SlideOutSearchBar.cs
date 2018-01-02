using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideOutSearchBar : MonoBehaviour
{
	
	public RectTransform pos;

	public Vector3 Original;
	public Vector3 Target;

	public float speed = 5f;

	private bool flip = false;
	
	public void Move()
	{
		flip = !flip;
		StopAllCoroutines();
		if (flip)
				StartCoroutine(MoveTowards(Target));
		else
			StartCoroutine(MoveTowards(Original));

	}

	IEnumerator MoveTowards(Vector3 targ)
	{
		Debug.Log("Slide Time");
		while (Mathf.Abs(pos.anchoredPosition.x - targ.x) > .3f)
		{
//			Debug.Log("In loop");
			pos.anchoredPosition3D = Vector3.Lerp(pos.anchoredPosition3D, targ, speed*Time.deltaTime);
			yield return null;
		}
		pos.anchoredPosition3D = targ;
		yield return null;
	}

	// Use this for initialization
	void Start ()
	{
		Original = pos.anchoredPosition3D;
		Target = new Vector3(-700f, Original.y, Original.z);
	}
	
	
	// Update is called once per frame
	void Update () {
		
	}
}
