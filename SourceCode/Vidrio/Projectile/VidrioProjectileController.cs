using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Clase que se encarga de gestionar el comportamiento del proyectil
 * del enemigo de vidrio
 * 
 * @author David Real Ortega
 */
public class VidrioProjectileController : MonoBehaviour
{
    /**
     * Funcion que gestiona el comportamiento del proyectil cuando este colisiona
     *
     * @param col los datos asociados a la colisión
     */
    void OnCollisionEnter2D(Collision2D col)
    {
            Destroy(this.gameObject);
    }

}
