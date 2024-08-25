using UnityEngine;
using System.Collections;
using Unity.Netcode;


//호랑이 공격 스크립트
public class CubeController : NetworkBehaviour
{

    public GameObject playerHead;
    private string playerState;
    private bool isActive = false;
    private bool isHunting = false;
    private Renderer renderer;
    private Tiger_animation tigerAnimation;

    PlayerMovement _playerMovement;

    public bool IsActive
    {
        get { return isActive; }
    }

    void Start()
    {
        renderer = GetComponent<Renderer>();
        playerState = playerHead.GetComponent<PlayerMovement>().playerState; //serverRpc로 고쳐야함.
        _playerMovement = playerHead.GetComponent<PlayerMovement>();
        // if(playerState == "Tiger"){
        //     SetCubeActive(false);
        // }
        SetCubeActive(false);

    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }


        if (Input.GetButtonDown("Interaction") && (playerState == "Tiger" || _playerMovement.isAwaken.Value) && !isActive)
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

            //Tiger_animation
            Tiger_animation tigerAnimation = FindObjectOfType<Tiger_animation>();
            if (tigerAnimation != null)
            {
                tigerAnimation.PlayAttackAnimation();
            }

            SetCubeActive(true);

            yield return new WaitForSeconds(0.1f);

            SetCubeActive(false);
            isActive = false;
        }
    }

    void SetCubeActive(bool active)
    {
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = active;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {

        Debug.Log(collision.gameObject.tag + " : " + collision.gameObject.GetComponent<NetworkObject>().OwnerClientId);

        if (isActive && playerState=="Tiger" && collision.gameObject.CompareTag("NPC")) //호랑이가 NPC 사냥
        {
            Debug.Log("호랑이가 NPC를 사냥!!");
            if (playerHead.GetComponent<PlayerMovement>() != null)
            {
                playerHead.GetComponent<PlayerMovement>().HuntFailure();
            }
        }
        else if (isActive && playerState == "Tiger" && collision.gameObject.CompareTag("Player")) //호랑이가 여우 사냥
        {
            if(collision.gameObject == this.gameObject)
            {
                Debug.Log("좆버그");
                return;
            }
            Debug.Log("호랑이가 여우를 사냥!!");

            OnTigerHuntsServerRpc(collision.gameObject);
        }
        else if (isActive && PlayerMovement.Instance.isAwaken.Value && collision.gameObject.CompareTag("Player") && !collision.gameObject.GetComponent<PlayerMovement>().isAwaken.Value) //여우(이미호)가 호랑이 사냥
        {

            if(collision.gameObject == this.gameObject)
            {
                Debug.Log("좆버그");
                return;
            }
            Debug.Log("여우가 호랑이를 사냥!!");

            OnSecFoxHuntsServerRpc(collision.gameObject);

        }
    }


    [ServerRpc]
    private void OnTigerHuntsServerRpc(NetworkObjectReference player)
    {
        if (player.TryGet(out var p))
        {
            Debug.Log("Tiger Hunts!");
            if (!p.GetComponent<PlayerMovement>().isAwaken.Value)
            {
                PlayerMovement.ProcessDieOnServer(p.GetComponent<PlayerMovement>());
            }

        }
    }

    [ServerRpc]
    private void OnSecFoxHuntsServerRpc(NetworkObjectReference player)
    {
        if (player.TryGet(out var p))
        {
            Debug.Log("Sec_Fox Hunts!");
            PlayerMovement.ProcessDieOnServer(p.GetComponent<PlayerMovement>());
            OnTigerDiesClientRpc(); 
        }
    }

    [ClientRpc]
    private void OnTigerDiesClientRpc()
    {
        if(IsOwner)
        _playerMovement.GetComponent<PlayerMovement>()._uiManager.__EngGame__.GetComponent<EndGame>().GameOver();
    }
}
