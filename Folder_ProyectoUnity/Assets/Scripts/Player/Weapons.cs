using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public BoxCollider[] armas;
    public int Damage = 3;
    void Start()
    {
        
        DesactivarColliders();
    }
    public void ActivarColliders()
    {
        for (int i = 0; i < armas.Length; i++)
        {
            if (armas[i] != null)
            {
                armas[i].enabled = true;
            }
        }
        Debug.Log("Se activo el arma");

    }
    public void DesactivarColliders()
    {
        for (int i = 0; i < armas.Length; i++)
        {
            if (armas[i] != null)
            {
                armas[i].enabled = false;
            }
        }
        Debug.Log("Se desactivo el arma");
    }
}
