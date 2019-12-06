using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnController : MonoBehaviour
{
	//Burn timer
	[Tooltip("The amount of time the object burns for before fully disintergrating.\nValues lesser than or equal to 0 will make the object burn indefinitely.")]
	public float burnTime = 5.0f;

	//Burn trigger
	[Tooltip("The trigger that propagates fire.")]
	public Collider fireCollider;

	//Incinerate shader
	[Tooltip("Shader at Special FX/Incinerate.")]
	public Shader incinerateShader;

	//Burning particle effect
	[Tooltip("The particle effect spawned on the object when burning.")]
	public GameObject burningParticleEffect;
    private GameObject pfx;
    public AudioClip ignite;
    //could have multiple particle effects, differing by shape of emitter
    //specify here based on shape og object
    //on start, set the size of the particle effect to the scale of the object

    //Spawn burning particle effect and cause disintegration
    private void OnEnable()
	{
		//spawn particle effect here
		if(burningParticleEffect != null)
		{
			pfx = Instantiate(burningParticleEffect, transform.position, transform.rotation, transform);
			pfx.transform.localScale = transform.localScale;
            AudioSource.PlayClipAtPoint(ignite, transform.position,Info.optionsData.sfxLevel);           
		}
		
		if(burnTime > 0)
		{
			StartCoroutine(Disintegrate());
		}
        else if(GetComponent<TorchTrigger>() != null)
        {
            GetComponent<TorchTrigger>().ToggleState(true);
        }

		FireController fc = GetComponent<FireController>();

		if(fc != null)
		{
			StartCoroutine(FirePropagation(fc));
		}
	}

	//Control the disintegrating shader
	IEnumerator Disintegrate()
	{
		float elapsedTime = 0.0f;
		
		Renderer renderer = GetComponent<MeshRenderer>();

		while(elapsedTime < burnTime)
		{
			renderer.material.SetFloat("_DisintegrateAmount", elapsedTime / burnTime);
			
			elapsedTime += Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}

        if (burnTime == 2.5f)
        {
			SpellCreation table = FindObjectOfType<SpellCreation>();

			table.isCraftCooldown = false;
			table.isSpellCollected = true;

			if (!Info.IsCurrentlyVR())
			{
				PlayerController player = FindObjectOfType<PlayerController>();

				player.isCraftCooldown = false;
				player.isSpellCollected = true;
			}
        }
        
        if(pfx != null)
        {
            Destroy(pfx);
        }
        
        Destroy(gameObject);
	}

	//Control fire propagation
	IEnumerator FirePropagation(FireController fc)
	{
		yield return new WaitForSeconds(burnTime > 0 ? burnTime * 0.1f : 0.01f);

		fc.enabled = true;
		fireCollider.enabled = true;
	}
}
