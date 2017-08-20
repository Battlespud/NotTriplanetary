﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RawResources{
	METAL=1,
	LUMBER=2,
	OIL=3,
	CROPS=4,
	GOLD=5
}

public enum Products{
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
	public void Add(float a){
		Amount += a;
	}
	public RawResource(RawResources r){
		resource = r;
		Amount = 100f; //TODO change to 0 later
	}
}

public class Product {
	public float Amount;
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