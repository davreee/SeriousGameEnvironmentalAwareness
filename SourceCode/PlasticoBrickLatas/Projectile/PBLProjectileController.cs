using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Clase que se encarga de gestionar el comportamiento del proyectil
 * del enemigo de plástico, brik y latas
 * 
 * @author David Real Ortega
 */
public class PBLProjectileController : MonoBehaviour
{
    //Posicion Inicial del movimiento
    private Vector3 posicionInicio;
    //Posicion Final del movimiento
    private Vector3 posicionObjetivo;
    //Velocidad del movimiento
    private float velocidad = 12.5f;
    //Tamaño de arco del movimiento
    private float tamArc = 1.0f;

    //Posición inicial y final en x
    private float xInicial;
    private float xFinal;
    //Distancia del movimiento
    private float distancia;
    //Rango donde se disminuye la velocidad del movimiento
    private float rangoDistancia = 6.5f;
    //Indica si el movimiento se ha configurado o no
    private bool movimientoIniciado = false;

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
        yield return new WaitForSeconds(3f);
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
     *  Update se invoca una vez por frame
     */
    void Update()
    {
        //Si se ha configurado el movimiento, se mueve el proyectil
        if (movimientoIniciado)
        {
            //Obtenemos la posición
            float siguienteX = Mathf.MoveTowards(transform.position.x, xFinal, velocidad * Time.deltaTime);
            float baseY = Mathf.Lerp(posicionInicio.y, posicionObjetivo.y, (siguienteX - xInicial) / distancia);
            float arco = tamArc * ((siguienteX - xInicial) * (siguienteX - xFinal) / (-0.25f * distancia * distancia));
            Vector3 nextPos = new Vector3(siguienteX, baseY + arco, transform.position.z);

            //Obtenemos la rotación
            transform.rotation = LookAt2D(nextPos - transform.position);

            //Movemos el objeto
            transform.position = nextPos;

            if (transform.position == posicionObjetivo)
            {
                Destroy(gameObject);
            }

        }
    }

    /**
     * Función que permite configurar el movimiento del proyectil
     *
     * @param posicionInicial posicion de la que parte el proyectil
     * @param posicionFinal posicion objetivo del proyectil
     */
    public void iniciarMovimiento(Transform posicionInicial, Vector3 posicionFinal)
    {
        posicionInicio = posicionInicial.position;
        posicionObjetivo = posicionFinal;
        xInicial = posicionInicial.position.x;
        xFinal = posicionFinal.x;
        distancia = xFinal - xInicial;
        movimientoIniciado = true;

        //Si el objetivo es cercano, se reduce la velocidad del proyectil
        if (Mathf.Abs(distancia) <= rangoDistancia)
        {
            velocidad = 8.5f;
        }
    }

    /**
     * Función que devuelve la rotación necesaria para orientar el proyectil
     * en los puntos de la trayectoria
     */
    static Quaternion LookAt2D(Vector2 forward)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }
}
