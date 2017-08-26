﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class Constructable : MonoBehaviour
    {
        int faction = 0;
        public GameObject prefabType;
        public float TimeToBuild;
        public float currTime;


        void Update()
        {
            if (currTime >= TimeToBuild)
                Finish();
        }

        void Finish()
        {
            GameObject g = Instantiate(prefabType);
            g.transform.position = transform.position;
        }
    }
}