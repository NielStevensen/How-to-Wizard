using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InheritYRotation : MonoBehaviour
{
    public Transform parentTransfrom;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, parentTransfrom.rotation.eulerAngles.y, 0);
    }
}
