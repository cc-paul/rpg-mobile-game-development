using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trainingDummy : MonoBehaviour
{ 
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        animator.SetTrigger("action");
    }

}
