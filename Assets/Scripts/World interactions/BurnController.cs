using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnController : MonoBehaviour
{
	//Burn timer
	[Tooltip("The amount of time the object burns for before fully disintergrating.\nValues lesser than or equal to 0 will make the object burn indefinitely.")]
	public float burnTime = 5.0f;

	//Incinerate shader
	[Tooltip("Shader at Special FX/Incinerate.")]
	public Shader incinerateShader;

	//Burning particle effect
	[Tooltip("The particle effect spawned on the object when burning.")]
	public GameObject burningParticleEffect;
	//could have multiple particle effects, differing by shape of emitter
	//specify here based on shape og object
	//on start, set the size of the particle effect to the scale of the object

	//Spawn burning particle effect and cause disintegration
	private void OnEnable()
	{
		//spawn particle effect here
		if(burningParticleEffect != null)
		{
			GameObject pfx = Instantiate(burningParticleEffect, transform.position, Quaternion.identity);
		}
		
		if(burnTime > 0)
		{
			StartCoroutine(Disintegrate());
		}
        else if(GetComponent<TorchTrigger>() != null)
        {
            GetComponent<TorchTrigger>().ToggleState(true);
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
        
        Destroy(gameObject);
	}
}
