using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellModuleList : MonoBehaviour
{
    public Spell caller;
    float potency;
    Collision collisionInfo;
    IEnumerator Charge()
    {
        Debug.Log("Charge");

        yield return new WaitForEndOfFrame();

        float holdTime = 0.0f;

        while (Input.GetButton("Fire1"))
        {
            yield return new WaitForEndOfFrame();

            holdTime += Time.deltaTime;
        }

        holdTime = Mathf.Min(holdTime, 5.0f);

        print(holdTime);

        potency = 0.5f + 1.0f * holdTime / 5.0f;

        print(potency);
        RunNext();
    }

    IEnumerator Fire()
    {
        Debug.Log("Fire");
        yield return new WaitForEndOfFrame();
        RunNext();
    }

    void RunNext()
    {
        if (caller.currentModule + 1 < caller.Modules.Length && caller.Modules[caller.currentModule + 1] != "")
        {
            caller.currentModule += 1;
            StartCoroutine(caller.Modules[caller.currentModule]);
        }
    }

}