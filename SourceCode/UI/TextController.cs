using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Clase que se encarga de gestionar el comportamiento del texto
 * que da feedback al usuario sobre la puntuación
 * 
 * @author David Real Ortega
 */
public class TextController : MonoBehaviour
{
    //Animación que realiza el texto
    public Animation anim;
    
    /**
     * Función que oculta el texto una vez que ha realizado la animación
     * completa
     */
    void desaparecer()
    {
        anim.Stop();
        gameObject.SetActive(false);
    }
}
