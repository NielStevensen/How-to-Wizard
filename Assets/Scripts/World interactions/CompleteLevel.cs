using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteLevel : MonoBehaviour
{
    GameObject Player;
    public string toLoad;

    private void Start()
    {
        Player = GameObject.Find("Player Character");
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - Player.transform.position).magnitude <= 1)
        {
            SceneManager.LoadScene(toLoad);
        }
    }
}
