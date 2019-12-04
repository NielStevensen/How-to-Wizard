using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchTrigger : MonoBehaviour
{

    [Tooltip("The moving obstacles this torch activates.")]
    public List<MovingObstacleManager> targetObstacles;
    [Tooltip("The lasers this torch activates.")]
    public List<LaserController> targetLasers;
	[Tooltip("Associated mechanism symbols.")]
	public List<SpriteRenderer> targetSymbols;
	private List<ParticleSystem> symbolPFX = new List<ParticleSystem>();

	public GameObject flame;

    public bool isActivated = false;

    // Start is called before the first frame update
    void Start()
    {
		foreach (SpriteRenderer obj in targetSymbols)
		{
			foreach (ParticleSystem pfx in obj.GetComponentsInChildren<ParticleSystem>())
			{
				symbolPFX.Add(pfx);
			}
		}

		if (isActivated)
        {
			UpdateSymbols(true);

			flame.SetActive(isActivated);

            foreach (MovingObstacleManager target in targetObstacles)
            {
                target.HandleState(isActivated);
            }

            foreach (LaserController target in targetLasers)
            {
                target.HandleState(isActivated);
            }

			GetComponent<BurnController>().enabled = true;
		}
    }

    // Update is called once per frame
    public void ToggleState(bool updatedState)
    {
        if (updatedState != isActivated)
        {
            isActivated = updatedState;
            flame.SetActive(isActivated);
			UpdateSymbols(isActivated);

            foreach (MovingObstacleManager target in targetObstacles)
            {
                target.HandleState(isActivated);
            }

			foreach (LaserController target in targetLasers)
            {
                target.HandleState(isActivated);
            }
        }

        if(!isActivated) GetComponent<BurnController>().enabled = false;
    }

	//Update mechanism symbols
	void UpdateSymbols(bool state)
	{
		for (int i = 0; i < targetSymbols.Count; i++)
		{
			targetSymbols[i].color = state ? Color.red : Color.white;

			if (state)
			{
				symbolPFX[i].Play();
			}
			else
			{
				symbolPFX[i].Stop();
			}
		}
	}
}
