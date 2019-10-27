using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
	//Modules in spell
	public List<string> Modules = new List<string>();

    public SpellModuleList list;

	public SpriteRenderer[] symbolSlots;
	public Sprite[] moduleSymbols;

	private void Start()
	{
		for(int i = 0; i < Modules.Count; i++)
		{
			int moduleIndex = 0;

			switch (Modules[i])
			{
				case "Projectile":
					moduleIndex = 0;

					break;
				case "Split":
					moduleIndex = 1;

					break;
				case "Charge":
					moduleIndex = 2;

					break;
				case "Touch":
					moduleIndex = 3;

					break;
				case "AOE":
					moduleIndex = 4;

					break;
				case "Proximity":
					moduleIndex = 5;

					break;
				case "Timer":
					moduleIndex = 6;

					break;
				case "Fire":
					moduleIndex = 7;

					break;
				case "Push":
					moduleIndex = 8;

					break;
				case "Pull":
					moduleIndex = 9;

					break;
				case "Weight":
					moduleIndex = 10;

					break;
				case "Barrier":
					moduleIndex = 11;

					break;
				case "Null":
					moduleIndex = 12;

					break;
			}
			
			symbolSlots[i].sprite = moduleSymbols[moduleIndex];
		}
	}

	//Handle casting a spell
	public void CallSpell ()
    {
		SingleInstanceEnforcer sie = FindObjectOfType<SingleInstanceEnforcer>();

		list.spellID = sie.currentSpellID;
		sie.currentSpellID++;

		list.StartCoroutine(list.HandleSpell(Modules, 10));
    }
}
