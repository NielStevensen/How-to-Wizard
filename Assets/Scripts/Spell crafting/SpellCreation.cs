using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellCreation : MonoBehaviour
{
	//Spell slot references
	public AttachCrystal[] VRSpellSlots;
	public AttachCrystal[] PCSpellSlots;
	private AttachCrystal[] moduleZones;

	[Space(10)]
	
    public List<string> spellInstructions = new List<string>();
    public List<int> moduletypes = new List<int>();
    bool isValid;
    public GameObject spellPrefab;
    public GameObject IncorrectSpell;
    public Transform spawnpoint;

    //VR crafting cooldown
    [Tooltip("The cooldown on crafting after clicking the craft confirm.")]
    public float craftCooldown = 2.5f;
    [HideInInspector]
    public bool isCraftCooldown = false;
    [HideInInspector]
    public bool isSpellCollected = true;

    //sounds
    private AudioSource source;
    public AudioClip sucesssSound;
    public AudioClip failSound;

	//Determine spell slots to use
	private void Start()
	{
        source = GetComponent<AudioSource>();
        source.volume = Info.optionsData.sfxLevel;

        moduleZones = Info.IsCurrentlyVR() ? VRSpellSlots : PCSpellSlots;
	}

	// Update is called once per frame
	public void ConfirmSpell()
    {
        // clear lists
        spellInstructions.Clear();
        moduletypes.Clear();
        isValid = true; //aceptable spell until proven optherwiise
        // check all zones and add to list of modules
        for (int i = 0; i < 5; i++)
        {
            if (moduleZones[i].attachedType > -1)
            {
                spellInstructions.Add(moduleZones[i].attachedModule);
                //print(spellInstructions[spellInstructions.Count -1]);
                moduletypes.Add(moduleZones[i].attachedType);
            }
        }
        // check valididty
        if (moduletypes.Count > 0)
        {
            isSpellCollected = false;

            StartCoroutine(HandleCraftingCooldown());

            if (moduletypes[0] != 0) isValid = false; // if the first is  a primary
            if (moduletypes.Count > 1) if (moduletypes[1] == 0) isValid = false; // if there is a second and second is a not primary

            for (int j = 2; j < moduletypes.Count; j++)
            {
                if (moduletypes[j] != 2) isValid = false; // if anthing from 3 - 5 is not an effect set false
            }

            if (isValid == true) // if the spell passes all checks
            {
                GameObject currentSpell = Instantiate(spellPrefab, spawnpoint.position, spawnpoint.rotation);// create a spell object
                source.PlayOneShot(sucesssSound);
                for ( int i = 0; i < spellInstructions.Count; i++)
                {
                    currentSpell.GetComponent<Spell>().Modules.Add(spellInstructions[i]);
                }
            }
            else
            {
                Instantiate(IncorrectSpell, spawnpoint.position, spawnpoint.rotation);
                source.PlayOneShot(failSound);
            }
        }
    }
    
    //Handle crafting cooldown while a spell is crafting
    IEnumerator HandleCraftingCooldown()
    {
        isCraftCooldown = true;

		//yield return new WaitForSeconds(craftCooldown);

		//temp crafting cooldown feedback
		float elapsedTime = 0.0f;
		float percentage;
		Renderer buttonRenderer = GetComponentInChildren<FinalizeSpell>().gameObject.GetComponent<Renderer>();
		Color initialColour = new Color(1, 1, 1, 0.125f);
		Color originalColour = buttonRenderer.material.color;
		Image cooldownImage = buttonRenderer.gameObject.GetComponentInChildren<Image>();

		while(elapsedTime < craftCooldown)
		{
			elapsedTime += Time.deltaTime;

			percentage = Mathf.Min(elapsedTime / craftCooldown, 1.0f);

			buttonRenderer.material.color = Color.Lerp(Color.white, originalColour, percentage);
			cooldownImage.color = Color.Lerp(Color.clear, Color.black, percentage < 0.5f ? Mathf.Min(percentage * 10.0f, 1.0f) : Mathf.Min((1 - percentage) * 18.75f, 1.0f));
			cooldownImage.fillAmount = percentage;
			
			yield return new WaitForEndOfFrame();
		}

        isCraftCooldown = false;
    }

    public void FXManagment(GameObject target, float time)
    {
        Destroy(target, time);
    }
}
