using UnityEngine;

[CreateAssetMenu(fileName = "Edificio", menuName = "Idle Game/Generator Edificios")]
public class Datos_Edificios : ScriptableObject
{
    public int      id;
    public string   nombre;
    public float    costoBase;
    public float    produccionBase;
    public float    aumentoCosto;
}   
