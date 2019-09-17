using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalizeSpell : MonoBehaviour
{
    private SpellCreation master;

    private void Start()
    {
        master = GetComponentInParent<SpellCreation>();
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (!master.isCraftCooldown && master.isSpellCollected && other.GetComponent<PickupSpell>())
        {
            master.Invoke("ConfirmSpell", 0);
        }
    }
}
