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

    PlayerMovement _playerMovement;

    public bool IsActive
    {
        get { return isActive; }
    }

    void Start()
    {
        renderer = GetComponent<Renderer>();
        playerState = playerHead.GetComponent<PlayerMovement>().playerState;
        _playerMovement = playerHead.GetComponent<PlayerMovement>();
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


        if (Input.GetButtonDown("Interaction") && (playerState == "Tiger"||_playerMovement.isAwaken))
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
        if (isActive && playerState=="Tiger" && collision.gameObject.CompareTag("NPC"))
        {
            if (playerHead.GetComponent<PlayerMovement>() != null)
            {
                playerHead.GetComponent<PlayerMovement>().HuntFailure();
            }
        }
        else if(isActive && _playerMovement.isAwaken && collision.gameObject.CompareTag("Player"))
        {
            // if(collision.gameObject.GetComponent<PlayerMovement>().playerState == "Tiger")
                Debug.Log("Fox Hunts!!!");
            
        }
    }
}
