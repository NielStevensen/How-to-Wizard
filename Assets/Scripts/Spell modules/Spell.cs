using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
	//Modules in spell
	public List<string> Modules = new List<string>();

    public SpellModuleList list;

	//Handle casting a spell
    public void CallSpell ()
    {
		SingleInstanceEnforcer sie = FindObjectOfType<SingleInstanceEnforcer>();

		list.spellID = sie.currentSpellID;
		sie.currentSpellID++;

		list.StartCoroutine(list.HandleSpell(Modules, 10));
    }
}
