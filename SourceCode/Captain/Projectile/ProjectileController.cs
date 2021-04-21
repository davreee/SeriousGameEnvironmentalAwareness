using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Clase que se encarga de gestionar el comportamiento de los 
 * proyectiles generados por el jugador
 * 
 * @author David Real Ortega
 */
public class ProjectileController : MonoBehaviour
{

    //Atributo que define la velocidad del proyectil
    public float velocidad = 3f;
    //Atributo que define el tiempo de vida del proyectil
    public float tiempoVida = .5f;
    //Atributo que define si el 
    public bool proyectilPotenciado = false;
    //Rigidbody ligado al proyectil
    public Rigidbody2D rigidBody;

    /**
     * Función que configura el comportamiento del proyectil
     *
     * @param clip audio que reproducirá el proyectil
     * @param potenciado define si el proyectil será normal o potenciado
     */
    public void configurarProyectil(AudioClip clip, bool potenciado)
    {
        proyectilPotenciado = potenciado;
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
        rigidBody.velocity = transform.right * velocidad;
        StartCoroutine("DestructionCoroutine");
    }

    /**
     * Función que gestiona el comportamiento del proyectil cuando este colisiona
     *
     * @param col los datos asociados a la colisión
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
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
     * Función que indica si el proyectil es potenciado o no
     * 
     * @return true si el proyectil es potenciado, false si no
     */
    public bool getProyectilPotenciado()
    {
        return proyectilPotenciado;
    }

}
