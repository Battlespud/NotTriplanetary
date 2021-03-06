﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Events;

public class Asteroid : MonoBehaviour, IMineable, IContext
{
	//Context
	public List<UnityAction> ContextActions(){
		return new List<UnityAction>{new UnityAction(ConstructMiningFacility)};
	}

	public GameObject getGameObject(){
		return gameObject;
	}



    public ResourceDeposit deposit;

    public Text stockpileText;
    public List<Mine> mines;

    // Use this for initialization
    void Awake()
    {

        deposit = new ResourceDeposit((RawResources)Random.Range(1, 6));
        gameObject.name = deposit.resource.ToString() + " Asteroid";
        Collections.Mineable.Add(this);
        mines = new List<Mine>();
    }



    public float Mine(RawResources r, float amount)
    {
        if (deposit.resource != r)
            return 0f;
        amount = amount * deposit.access;
        if (amount <= deposit.amount)
        {
            deposit.amount -= amount;
            UpdateText();
            return amount;
        }
        return 0f;
    }

    void UpdateText()
    {
        StringBuilder sb = new StringBuilder();
        //stockpileText.text = "Resources||\n";
        sb.AppendLine("Resources||");
        foreach (Mine m in mines)
        {
            //stockpileText.text += string.Format ("{0}: {1} \n", m.resource.ToString (), m.Stockpile );
            sb.AppendFormat("{0}: {1}\n", m.resource.ToString(), m.Stockpile);
        }
        stockpileText.text = sb.ToString();
    }


    void Update()
    {
    }

    [SerializeField]
    public void ConstructMiningFacility()
    {
        Debug.Log("Queueing a mining Facility");
		ConstructionRequests r = new ConstructionRequests(new Vector3(transform.position.x + GetComponent<Renderer>().bounds.extents.x, 0f, transform.position.y));
    }
}