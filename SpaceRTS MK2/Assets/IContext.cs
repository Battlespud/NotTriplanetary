using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IContext
{
	List<UnityAction> ContextActions();
	GameObject getGameObject();
}