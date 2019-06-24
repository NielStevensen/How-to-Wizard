using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using SpellModuleList;

public class Spell : MonoBehaviour
{

    public string[] Modules;
    public SpellModuleList list;
    public int currentModule;

    bool pressed = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !pressed)
        {
            print("initiate");

            pressed = true;

            currentModule = 0;
            //list.StartCoroutine(Modules[currentModule]);
            list.StartCoroutine(Modules[currentModule]);
        }
    }
}
