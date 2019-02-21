using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sun : MonoBehaviour {
    
    RaycastHit hitRay;
    List<Vector2> positionsRays;
    [SerializeField] GameObject cell; 
    public int choques = 0;
    public float rayAmm = 360;
    float radius = 100;
    int timerForTheSun;
    byte countHits = 0;
    bool rayCast = false;

    //GENERACION DE LOS RAYOS DEL RAYCAST
    public void shine(bool rayDebug)
    {
        rayCast = rayDebug;
        for (int i = 1; i < rayAmm + 1; ++i)
        {
            countHits = 0;
            float partialAngle = (float)i / rayAmm;
            float angle = Constants.tau * partialAngle;
            float X = radius * Mathf.Cos(angle) + GetComponent<Transform>().position.x;
            float Y = radius * Mathf.Sin(angle) + GetComponent<Transform>().position.y;
            throwARay(this.GetComponent<Transform>().position, X, Y, angle);
        }
    }

    //PRIMERA COLISION DEL RAYO
    private void throwARay(Vector3 originVect ,float X, float Y, float angle)
    {
        if (Physics.Raycast(originVect, new Vector3( X, Y, GetComponent<Transform>().position.z) - originVect, out hitRay))
        {
            //Pintar el ray(debug) 
            if (rayCast)
            {
                Debug.DrawLine(
                        originVect,
                        hitRay.point,
                        Color.yellow,
                        1);
            }
            //Aumenta la aptitud
            if (hitRay.collider.gameObject.CompareTag("photosensible"))
            {
                ++choques;
            }
            //Recalculo del angulo + recursividad
            else if (hitRay.collider.gameObject.CompareTag("cell"))
            {
                //Para este calculo, se usan los cromosomas de refraccion y roughness
                Vector3 refraction = cell.GetComponent<eye>().getChromosome(5) * 5 * (-2 * Vector3.Dot(hitRay.point - originVect, hitRay.normal) * hitRay.normal + hitRay.point - originVect + new Vector3(cell.GetComponent<eye>().getChromosome(4) , 0.0f ,0.0f));
                throwARay(hitRay.point, refraction);
            }
        }
    }

    //GENERACION DE LOS REBOTES DEL RAYO
    private void throwARay(Vector3 originVect, Vector3 refraction)
    {
        ++countHits;
        if(countHits < Constants.maxHits)
            if (Physics.Raycast(originVect,refraction, out hitRay))
            {
                //Pintar el ray(debug) 
                if (rayCast)
                {
                    Debug.DrawLine(
                            originVect,
                            hitRay.point,
                            Color.yellow,
                            1);
                }
                //Aumenta la aptitud
                if (hitRay.collider.gameObject.CompareTag("photosensible"))
                {
                    ++choques;
                }
                //Recalculo del angulo + recursividad 
                else if (hitRay.collider.gameObject.CompareTag("cell"))
                {
                    //Para este calculo, se usan los cromosomas de refraccion y roughness
                    Vector3 refractionNew = cell.GetComponent<eye>().getChromosome(5) * 5 * (-2 * Vector3.Dot(hitRay.point - originVect, hitRay.normal) * hitRay.normal + hitRay.point - originVect + new Vector3(cell.GetComponent<eye>().getChromosome(4), 0.0f, 0.0f));
                    throwARay(hitRay.point , refractionNew);
                }
            }
    }
}
