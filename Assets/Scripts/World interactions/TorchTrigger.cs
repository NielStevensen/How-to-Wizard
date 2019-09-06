using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchTrigger : MonoBehaviour
{

    [Tooltip("The moving obstacles this torch activates.")]
    public List<MovingObstacleManager> targetObstacles;
    [Tooltip("The lasers this torch activates.")]
    public List<LaserController> targetLasers;

    public GameObject flame;

    public bool isActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        if (isActivated)
        {
            flame.SetActive(isActivated);

            foreach (MovingObstacleManager target in targetObstacles)
            {
                target.HandleState(isActivated);
            }

            foreach (LaserController target in targetLasers)
            {
                target.HandleState(isActivated);
            }
        }
    }

    // Update is called once per frame
    public void ToggleState(bool updatedState)
    {
        if (updatedState != isActivated)
        {
            isActivated = updatedState;
            flame.SetActive(isActivated);

            foreach (MovingObstacleManager target in targetObstacles)
            {
                target.HandleState(isActivated);
            }

			foreach (LaserController target in targetLasers)
            {
                target.HandleState(isActivated);
            }
        }
    }
}
