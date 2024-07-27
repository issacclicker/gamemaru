using UnityEngine;
using System.Collections;
using Unity.Netcode;


//호랑이 공격 스크립트
public class CubeController : NetworkBehaviour
{

    public GameObject playerHead;
    private string playerState; 
    private bool isActive = false; 
    private Renderer renderer;

    public bool IsActive
    {
        get { return isActive; }
    }

    void Start()
    {
        renderer = GetComponent<Renderer>();
        playerState = playerHead.GetComponent<PlayerMovement>().playerState;
        // if(playerState == "Tiger"){
        //     SetCubeActive(false);
        // }
        SetCubeActive(false);
        
    }

    void Update()
    {
        if(!IsOwner){
            return;
        }


        if (Input.GetButtonDown("Interaction") && playerState == "Tiger")
        {
            Debug.Log("Attack!");
            StartCoroutine(ActivateAndDeactivateCube());
        }
    }

    IEnumerator ActivateAndDeactivateCube()
    {
        if (!isActive)
        {
            isActive = true;
            SetCubeActive(true);

            yield return new WaitForSeconds(2f);

            SetCubeActive(false);
            isActive = false;
        }
    }

    void SetCubeActive(bool active)
    {
        if (renderer != null)
        {
            renderer.enabled = active;
        }
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = active;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (isActive && collision.gameObject.CompareTag("NPC"))
        {
            if (playerHead.GetComponent<PlayerMovement>() != null)
            {
                playerHead.GetComponent<PlayerMovement>().HuntFailure();
            }
        }
    }
}
