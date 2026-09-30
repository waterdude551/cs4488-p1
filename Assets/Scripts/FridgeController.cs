using UnityEngine;

public class FridgeController : MonoBehaviour
{
    Animator anim;
    void Awake()
    {
        anim = GetComponent<Animator>();
    } 

    void OnTriggerEnter(Collider c) {
        if (c.CompareTag("Player")) {
            anim.SetBool("cameraClose", true);
        }
    }

    void OnTriggerExit(Collider c) {
        if (c.CompareTag("Player")) {
            anim.SetBool("cameraClose", false);
        }
    }
}
