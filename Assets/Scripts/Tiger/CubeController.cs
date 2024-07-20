using UnityEngine;
using System.Collections;

public class CubeController : MonoBehaviour
{
    private bool isActive = false; 
    private Renderer renderer;

    public bool IsActive
    {
        get { return isActive; }
    }

    void Start()
    {
        renderer = GetComponent<Renderer>();
        SetCubeActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
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

    private void OnCollisionEnter(Collision collision)
    {
        if (isActive && collision.gameObject.CompareTag("NPC"))
        {
            TigerController tigerController = FindObjectOfType<TigerController>();
            if (tigerController != null)
            {
                tigerController.HuntFailure();
            }
        }
    }
}
