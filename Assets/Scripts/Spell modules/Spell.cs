using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
	//Spell delay
	[Tooltip("Delay between using spells in seconds.")]
	public float spellDelay = 2.5f;
	private bool isSpellCooldown = false;
	[HideInInspector]
	public bool isSpellCasted = false;
	
	[Space(10)]

	//Modules in spell
	public List<string> Modules = new List<string>();

    public SpellModuleList list;

    //Handle spell input
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !isSpellCooldown)
        {
			CallSpell();
        }
    }

	//Handle casting a spell
    public void CallSpell ()
    {
		StartCoroutine(HandleSpellDelay());

		SingleInstanceEnforcer sie = FindObjectOfType<SingleInstanceEnforcer>();

		list.spellID = sie.currentSpellID;
		sie.currentSpellID++;

		list.StartCoroutine(list.HandleSpell(Modules));
    }

	//Handle spell delay
	IEnumerator HandleSpellDelay()
	{
		isSpellCooldown = true;

		while (!isSpellCasted)
		{
			yield return new WaitForEndOfFrame();
		}

		isSpellCasted = false;

		yield return new WaitForSeconds(spellDelay);

		isSpellCooldown = false;
	}
}
