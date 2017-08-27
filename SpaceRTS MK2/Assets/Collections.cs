﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Collections {

	public static List<City> Cities = new List<City>();
	public static List<Freighter> Freighters = new List<Freighter>();
	public static List<Freighter> Available = new List<Freighter>();
	public static List<Mine> Mines = new List<Mine>();

	public static List<IResources> ResourceSources = new List<IResources>();
	public static List<IMineable> Mineable = new List<IMineable>();
}