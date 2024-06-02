using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject Options;
    public void AppearOptions()
    {
        
    }
    public void DissapearOptions()
    {
        Options.SetActive(false);
    }

}
