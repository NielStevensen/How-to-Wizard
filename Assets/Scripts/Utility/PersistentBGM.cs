using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentBGM : MonoBehaviour
{
    public int trackIndex = -1;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
