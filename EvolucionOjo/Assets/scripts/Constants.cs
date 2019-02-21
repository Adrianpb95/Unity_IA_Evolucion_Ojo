using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Constants  {

    /* Distance * Quantity * m * Rotacion * Roughness(rgb) * Refractivity(a) * Scale */

    //Distance --> Distancia entre la primera celula y la ultima de la pared ocular
    //Quantity --> Cantidad de celulas 
    //m --> Pendiente de la curva creada por el parpado
    //Rotacion --> Angulo de saluda de la pared ocular
    //Roughness --> Angulo de rebote del rayo --> GRIS = MISMO ANGULO DE INCIDENCIA // BLANCO = MENOR ANGULO DE INCIDENCIA // NEGRO = MAYOR ANGULO DE INCIDENCIA
    //Refractivity --> Distancia de reboto del rayo(modulo)
    //Scale --> Tamaño de cada celula de la pared

    public const float tau = Mathf.PI * 2;
    public const byte maxHits = 10;

    public static float[] minValues = { 1f,  3f, -2f, -90f, -1f,    0.1f, 0.1f };
    public static float[] maxValues = { 4f, 20f,  2f,  90f,  1f,    1f,   0.5f };

    public static short generationSize = 100;
    public static float elegibleParents = 0.1f;
    public static byte numberOfParents = 2;
    public static float mutationRatio = 0.05f;

    public static float firstSpawnTime = 3f, spawnInterval = 1f;
}


