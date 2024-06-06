using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolveButtonManager : MonoBehaviour
{
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonPress()
    {
        gameManager.UpdateFunction();
    }
}
