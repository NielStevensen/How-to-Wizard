using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteLevel : MonoBehaviour
{

    public Scene toLoad;

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(toLoad.name);
    }
}
