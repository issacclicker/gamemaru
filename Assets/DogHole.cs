using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
// using Unity.VisualScripting;

public class DogHole : NetworkBehaviour
{

    bool isInside = false;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PlayerMovement>().playerStateSync.Value=="Tiger")
        {
            Debug.Log("탈출~시작");
            isInside = true;
            StartCoroutine(ExitTime());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PlayerMovement>().playerStateSync.Value=="Tiger")
        {
            Debug.Log("탈출~실패~");
            isInside = false;
        }
    }

    IEnumerator ExitTime() 
    {
        yield return new WaitForSeconds(3f);
        if(!isInside)
        {
            yield break;
        }
        isExitSuccess();
    }

    private void isExitSuccess()
    {
        if(isInside)
        {
            Debug.Log("성공!");
        }
    }
}
