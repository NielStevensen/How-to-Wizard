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
	public string[] Modules;

    public SpellModuleList list;

    //Handle spell input
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !isSpellCooldown)
        {
			StartCoroutine(HandleSpellDelay());

			list.StartCoroutine(list.HandleSpell(Modules));
        }
    }
    public void VRSpell ()
    {
        Debug.Log("pickup");
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
