﻿//construction requests classsssssssssss (extra s's for extra ass)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


    //construction requests classs
    public class ConstructionRequests
    {
        public NegroBundleOfSticks conShip;
        public Constructable constr;
		public GameObject go;
        public ConstructionRequests(Vector3 pos)
        {
		GameObject g = GameObject.Instantiate(ConstructionManager.ConstructablePrefab);
            g.transform.position = pos;
            constr = g.GetComponent<Constructable>();
		ConstructionManager.opened.Add (this);
		ConstructionManager.UpdateMissionsQuery ();
		go = g;
        }
}