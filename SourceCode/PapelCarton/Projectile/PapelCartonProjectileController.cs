using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Clase que se encarga de gestionar el comportamiento del proyectil
 * del enemigo de papel y cartón
 * 
 * @author David Real Ortega
 */
public class PapelCartonProjectileController : MonoBehaviour
{
    //Atributo que define la velocidad del proyectil
    public float speed = .5f;
    //Atributo que define el tiempo de vida del proyectil
    private float tiempoVida = 3.0f;
    //Rigidbody ligado al proyectil
    public Rigidbody2D rigidBody;

    /**
     * Start es invocada antes de la actualización del primer frame
     */
    void Start()
    {
        StartCoroutine("DestructionCoroutine");
    }

    /**
     * Corrutina que se encarga de destruir el proyectil pasado su tiempo de vida
     *
     */
    IEnumerator DestructionCoroutine()
    {
        yield return new WaitForSeconds(tiempoVida);
        Destroy(gameObject);
    }

    /**
     * Función que gestiona el comportamiento del proyectil cuando este colisiona
     *
     * @param col los datos asociados a la colisión
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        Destroy(this.gameObject);
    }

    /**
     * Función que se encarga de mover el proyectil en la dirección deseada
     *
     */
    public void moverProyectil(Vector3 direccion)
    {
        rigidBody.velocity = direccion * speed;
    }

}
