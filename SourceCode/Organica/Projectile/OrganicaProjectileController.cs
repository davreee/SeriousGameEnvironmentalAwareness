using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Clase que se encarga de gestionar el comportamiento del proyectil
 * del enemigo de basura orgánica
 * 
 * @author David Real Ortega
 */
public class OrganicaProjectileController : MonoBehaviour
{
    //Atributo que define la velocidad del proyectil
    private float speed = 10.0f;
    //Atributo que define el tiempo de vida del proyectil
    private float tiempoVida = 1.75f;
    //Rigidbody ligado al proyectil
    public Rigidbody2D rigidBody;

    /**
     * Start es invocada antes de la actualización del primer frame
     */
    void Start()
    {
        StartCoroutine("DestructionCoroutine");
        rigidBody.velocity = transform.right * speed;
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
}
