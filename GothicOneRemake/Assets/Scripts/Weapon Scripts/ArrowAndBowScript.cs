using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAndBowScript : MonoBehaviour
{

    private Rigidbody myBody;

    public float speed = 30f;

    public float deactivate_Timer = 3f;

    public float damage = 15f;

    private void Awake() {
        myBody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DeactivateGameObject", deactivate_Timer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DeactivateGameObject () {
        if(gameObject.activeInHierarchy) {
            gameObject.SetActive(false);
        }
    }

    public void Launch( Camera mainCamera) {

        myBody.velocity = mainCamera.transform.forward * speed;

        //transform.LookAt(transform.position + myBody.velocity);
    }

    void OnTriggerEnter(Collider target) {

        //after we touch an enemy, deactivate game object


    }
} // class
