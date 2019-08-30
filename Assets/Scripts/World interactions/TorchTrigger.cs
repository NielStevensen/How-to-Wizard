using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchTrigger : MonoBehaviour
{

    [Tooltip("The moving obstacles this Torch activates.")]
    public List<MovingObstacleManager> targetObstacles;
    //[Tooltip("The lasers this toech activates.")]
    //public List<LaserManager> targetLasers;

    public GameObject flame;

    public bool isActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        
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
            /*foreach (LaserManager target in targetLasers)
            {
                target.HandleState(isActivated);
            }*/
        }
    }
}
