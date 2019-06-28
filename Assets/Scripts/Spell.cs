using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public string[] Modules;

    public SpellModuleList list;

    bool pressed = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !pressed)
        {
            pressed = true;

            list.StartCoroutine(list.HandleSpell(Modules));
        }
    }
}
