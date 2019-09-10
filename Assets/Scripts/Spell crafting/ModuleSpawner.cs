using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleSpawner : MonoBehaviour
{

    public GameObject crystal;
    CrystalInfo crystalref;
    public string Module;
    public int Moduletype;
    // Start is called before the first frame update
    void Start()
    {
        AttemptSpawn();
    }

    void AttemptSpawn()
    {
        crystalref = Instantiate(crystal,transform.position,Quaternion.identity).GetComponent<CrystalInfo>();
           crystalref.moduleType = Moduletype;
           crystalref.module = Module;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (crystalref == null)
        {
            AttemptSpawn();
        }
        else if (crystalref.unused == false)
        {
            AttemptSpawn();
        }
    }
}
