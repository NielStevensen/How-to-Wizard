using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalizeSpell : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        GetComponentInParent<SpellCreation>().Invoke("ConfirmSpell", 0);
    }
}
