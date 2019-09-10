using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalInfo : MonoBehaviour
{
    public string module;
    public int moduleType;
    public bool unused = true;

	//Used for pc crafting outlines
	[HideInInspector]
	public bool isSelected = false;
}
