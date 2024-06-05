using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject[] armas;
    public Image[] imagenesArmas;
    private ListaArmas listaArmas;
    //private NodoArma nodoActual;
    private int indiceActual = 0;
    private PlayerController _player;

    private void Awake()
    {
        _player = GetComponent<PlayerController>();
    }

    void Start()
    {
        listaArmas = new ListaArmas(armas.Length);
        for (int i = 0; i < armas.Length; i = i + 1 )
        {
            listaArmas.Agregar(armas[i], imagenesArmas[i]);
        }
        ActualizarInterfaz();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CambiarArmaAnterior();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            CambiarArmaSiguiente();
        }
    }

    void CambiarArmaAnterior()
    {
        if (!_player.isAttacking)
        {
            indiceActual = (indiceActual - 1 + armas.Length) % armas.Length;
            ActualizarInterfaz();
        }
    }

    void CambiarArmaSiguiente()
    {
        if (!_player.isAttacking)
        {
            indiceActual = (indiceActual + 1) % armas.Length;
            ActualizarInterfaz();
        }
    }

    void ActualizarInterfaz()
    {
        if (!_player.isAttacking)
        {
            for (int i = 0; i < imagenesArmas.Length; i = i + 1)
            {
                if (i == indiceActual)
                {
                    imagenesArmas[i].color = new Color(1f, 1f, 1f, 1f);
                    armas[i].SetActive(true);
                }
                else
                {
                    imagenesArmas[i].color = new Color(1f, 1f, 1f, 0.5f);
                    armas[i].SetActive(false);
                }
            }
        }
    }
}
