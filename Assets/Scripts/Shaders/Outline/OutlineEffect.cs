using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
	//Should the outline be drawn?
	[Tooltip("Is the player within range to draw an outline?")]
	public bool shouldDrawOutline = false;
	
	//Cameras
	private Camera mainCamera;
	private Camera maskedCam;

	//Shaders
	public Shader simpleShader;
	public Shader outlineShader;
	
	//Material
	private Material outlineMaterial;
	
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
		if (shouldDrawOutline)
		{
			//Set up masked camera values
			maskedCam.CopyFrom(mainCamera);
			maskedCam.clearFlags = CameraClearFlags.Color;
			maskedCam.backgroundColor = Color.black;
			maskedCam.cullingMask = 1 << LayerMask.NameToLayer("Outline");
			
			//Assign to memory
			RenderTexture temporaryRT = RenderTexture.GetTemporary(Screen.width, Screen.height);

			//Set target texture and shader
			maskedCam.targetTexture = temporaryRT;
			maskedCam.RenderWithShader(simpleShader, "");
			outlineMaterial.SetTexture("_SceneTex", source);

			//Pass through shader and draw to screen
			Graphics.Blit(temporaryRT, destination, outlineMaterial);

			//Release memory
			RenderTexture.ReleaseTemporary(temporaryRT);
		}
		else
		{
			Graphics.Blit(source, destination);
		}
	}
}
