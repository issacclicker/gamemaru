using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingCollision : MonoBehaviour
{
    public GameObject gameObject;
    private void OnTriggerEnter(Collider other) {
        // if(other.CompareTag("Player")){
        //     if(gameObject!=null){
        //         gameObject.SetActive(false);
        //     }
        // }
        GetComponent<MeshRenderer>().enabled = false;
    }
}
