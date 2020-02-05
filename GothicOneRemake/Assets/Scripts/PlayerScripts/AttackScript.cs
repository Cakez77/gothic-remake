using UnityEngine;

public class AttackScript : MonoBehaviour {

    public float damage = 2f;
    public float radius = 1f;
    public LayerMask layerMask;

    //TODO: 50 hits every time a boar attacks
    private void Update() {
        
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, layerMask);

        if(hits.Length > 0){
            print("We touched: " + hits[0].gameObject.tag);

            gameObject.SetActive(false);
        }
    }
    
} // class