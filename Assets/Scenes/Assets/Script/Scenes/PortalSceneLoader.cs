using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalSceneLoader : MonoBehaviour
{
    [SerializeField] private string BossToLoad = "Boss";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Trigger entered by: {collision.gameObject.name}");

        if (collision.CompareTag("Player"))
        {
            if (CompareTag("BossPortal"))
            {
                Debug.Log("Collided With Boss Portal");
                SceneManager.LoadScene(BossToLoad);
            }
            else
            {
                Debug.LogWarning("GameObject is not tagged correctly (needs 'Door' or 'BossPortal').");
            }
        }
    }

}

