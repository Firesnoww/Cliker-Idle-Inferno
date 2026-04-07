using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gestor_Economia : MonoBehaviour
{
    public float Agonia;
    public TMP_Text textAgonia;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        textAgonia.text = Agonia.ToString("F1");
    }
}
