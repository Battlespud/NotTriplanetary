using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RawResources{
	NONE = 0,
	METAL=1,
	LUMBER=2,
	OIL=3,
	CROPS=4,
	GOLD=5
}

public enum Products{
	NONE=0,
	WEAPONS=1,
	WOOD=2,
	PETROL=3,
	FOOD=4,
	PRECIOUS=5
}



public class RawResource {
	float Amount;
	public RawResources resource;
	public bool Use(float a){
		if (Amount >= a) {
			Amount -= a;
			return true;
		}
		return false;
	}
	public float GetAmount(){
		return Amount;
	}
	public void Add(float a){
		Amount += a;
	}
	public RawResource(RawResources r){
		resource = r;
		Amount = 1000f; //TODO change to 0 later
	}
}

public class Product {
	float Amount;
	public float GetAmount(){
		return Amount;
	}
	public Products product;
	public bool Use(float a){
		if (Amount >= a) {
			Amount -= a;
			return true;
		}
		return false;
	}
	public void Add(float a){
		Amount += a;
//		Debug.Log ("Amount " + a + ". Now : " + Amount);
	}
	public Product(Products r){
		product = r;
		Amount = 5f;
	}
}

public class ResourceRequest {
	public RawResources resource;
	public int amount;
	public City patron;
	public Ship assigned;
	public bool exact; //will fill cargo if false
	public ResourceRequest(RawResources r, int a, bool b){
		resource = r;
		amount = a;
		exact = b;
	}
}
