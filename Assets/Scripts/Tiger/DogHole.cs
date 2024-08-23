using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DogHole : NetworkBehaviour
{
    public static List<DogHole> allHoles = new List<DogHole>();

    private bool isInside = false;

    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Collider holeCollider;

    private void Awake()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();

        if (holeCollider == null)
            holeCollider = GetComponent<Collider>();

        allHoles.Add(this);
        DisableHole();
    }

    public override void OnDestroy()
    {
        base.OnDestroy(); 
        allHoles.Remove(this);
    }

    public void EnableHole()
    {
        meshRenderer.enabled = true;
        holeCollider.enabled = true;
    }

    public void DisableHole()
    {
        meshRenderer.enabled = false;
        holeCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter: " + other.gameObject.name);
        if (other.CompareTag("Player") && other.GetComponent<PlayerMovement>().playerStateSync.Value == "Tiger")
        {
            Debug.Log("탈출 시작");
            isInside = true;
            StartCoroutine(ExitTime());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit: " + other.gameObject.name);
        if (other.CompareTag("Player") && other.GetComponent<PlayerMovement>().playerStateSync.Value == "Tiger")
        {
            Debug.Log("탈출 실패");
            isInside = false;
        }
    }

    private IEnumerator ExitTime()
    {
        yield return new WaitForSeconds(3f);
        if (isInside)
        {
            Debug.Log("탈출 성공!");
        }
    }
}
