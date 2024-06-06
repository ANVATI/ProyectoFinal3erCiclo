using UnityEngine;

public class ColisionDetectada : MonoBehaviour
{
    public int indiceArmaADesbloquear;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null)
            {
                inventory.DesbloquearArma(indiceArmaADesbloquear);
                gameObject.SetActive(false); // Desactivar el prefab de arma en el suelo
            }
        }
    }
}
