using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTable : MonoBehaviour
{
    AttachCrystal[] moduleZones;
    bool clear = false;

	private Animator[] slotAnimators = new Animator[5];

    // Start is called before the first frame update
    void Start()
    {
        moduleZones = FindObjectsOfType<AttachCrystal>();

		Transform master = transform.parent;

		for (int i = 0; i < 5; i++)
		{
			slotAnimators[i] = master.GetChild(11 + i).GetComponent<Animator>();
		}
	}

    //Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PickupSpell>())
        {
			ClearCrystals();
        }
    }

    private void LateUpdate()
    {
        if(clear)
        {
            foreach (AttachCrystal a in moduleZones)
            {
                a.attachedModule = "";
                a.attachedType = -1;
            }

            clear = false;
        }
    }

	//Destroy crystals and reset slot stored values
	public void ClearCrystals()
	{
		foreach (CrystalInfo a in FindObjectsOfType<CrystalInfo>())
		{
			Destroy(a.gameObject);
		}

		clear = true;

		foreach (Animator anim in slotAnimators)
		{
			anim.enabled = false;
		}
	}
}