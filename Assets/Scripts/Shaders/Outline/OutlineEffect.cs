using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineEffect : MonoBehaviour
{
	//Cameras
	private Camera mainCamera;
	private Camera maskedCam;

	//Shaders
	public Shader simpleShader;
	public Shader outlineShader;

	public RawImage ui;

	//Material
	private Material outlineMaterial;

	//Render texture
	private RenderTexture maskedRT = null;
	
	//Set references and set up masked camera
	void Start()
	{
		mainCamera = GetComponent<Camera>();
		maskedCam = new GameObject().AddComponent<Camera>();
		maskedCam.enabled = false;

		outlineMaterial = new Material(outlineShader);
	}

	//Affect rendered image
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//Set up masked camera values
		maskedCam.CopyFrom(mainCamera);
		maskedCam.clearFlags = CameraClearFlags.Color;
		maskedCam.backgroundColor = Color.black;
		maskedCam.cullingMask = 1 << LayerMask.NameToLayer("Crafting");
		
		//Create render texture if it is null
		if(maskedRT == null)
		{
			maskedRT = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.R8);
		}

		//Assign to memory
		maskedRT.Create();

		//Set target texture and shader
		maskedCam.targetTexture = maskedRT;
		maskedCam.RenderWithShader(simpleShader, "");
		outlineMaterial.SetTexture("_SceneTex", source);
		
		//Pass through shader
		Graphics.Blit(maskedRT, destination, outlineMaterial);

		//Release memory
		maskedRT.Release();
	}
}
