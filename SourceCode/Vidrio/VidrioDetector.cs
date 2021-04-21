using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Clase que se encarga de gestionar la detección del jugador
 * 
 * @author David Real Ortega
 */
public class VidrioDetector : MonoBehaviour
{
    //Controlador del enemigo al que le pasará la información sobre el jugador
    private VidrioController controller;

    /**
     * Start es invocada antes de la actualización del primer frame
     */
    void Start()
    {
        controller = transform.parent.gameObject.GetComponent<VidrioController>();
    }

    /**
     * Función que gestiona el comportamiento del detector cuando alguien entra
     * en su rango
     *
     * @param col los datos asociados a la colisión
     */
    void OnTriggerEnter2D(Collider2D col)
    {
        //Si es un jugador, indicamos al enemigo que ha entrado dentro de su rango de ataque
        if (col.gameObject.tag == "Player")
        {
            controller.enemigoDetectado();
        }
    }

    /**
    * Función que gestiona el comportamiento del detector cuando alguien sale
    * de su rango
    *
    * @param col los datos asociados a la colisión
    */
    void OnTriggerExit2D(Collider2D other)
    {
        //Si es un jugador, indicamos al enemigo que ha salido de su rango de ataque
        if (other.gameObject.tag == "Player")
        {
            controller.enemigoPerdido();
        }
    }

    /**
     * Función que gestiona el comportamiento del detector cuando alguien se
     * encuentra dentro de su rango
     *
     * @param col los datos asociados a la colisión
     */
    void OnTriggerStay2D(Collider2D other)
    {
        //Si es un jugador, indicamos al enemigo la posición en la que se encuentra
        if (other.gameObject.tag == "Player")
        {
            controller.recibirPosicionEnemigo(other.gameObject.transform);
        }
    }
}
