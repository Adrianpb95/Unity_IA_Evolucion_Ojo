using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eye : MonoBehaviour {

    /* Distance * Quantity * m * Rotacion * Roughness(rgb) * Refractivity(a) * Scale */
    [SerializeField] float[] chromosomes;

    public float aptitud = 0;
    public int generacion = 0;

    [SerializeField] Color colorCell;
    [SerializeField] Vector3 localSc;
    [SerializeField] GameObject cellGO;
    [SerializeField] List<Vector3> cellsLeft  = new List<Vector3>();
    [SerializeField] List<Vector3> cellsRight = new List<Vector3>();
    [SerializeField] public List<GameObject> cells = new List<GameObject>();
    

    //SE CREAN LOS CROMOSOMAS DE LA PRIMERA GENERACION DE MANERA ALEATORIA
    public void GenerateChromosome()
    {
        chromosomes = new float[Constants.minValues.Length];
        for (byte i = 0; i < chromosomes.Length; ++i)
        {
            chromosomes[i] = Random.Range(Constants.minValues[i], Constants.maxValues[i]);
        }
    }


    //SE CREAN LAS PAREDES DE LOS OJOS CON LOS CROMOSOMAS RECIBIDOS
    public void CreateEye()
    {
        //Solo entra en la primera generacion
        if(chromosomes.Length == 0)
        GenerateChromosome();        
        
        cells.Clear();
        colorCell = new Color(chromosomes[4], chromosomes[4], chromosomes[4], chromosomes[5]);        
        localSc = new Vector3(chromosomes[6], 1.0f , chromosomes[6]);        
        
        //Creacion de la pared derecha
        cellsRight.Clear();
        for(int i = 0  ; i<= chromosomes[1] ; ++i)
        {
            float plotA = chromosomes[0];
            float plotB = chromosomes[2];
            double xeq = plotA / chromosomes[1] * i;//((float)i * plotA) / ((chromosomes[1])-1); //Double para aumentar la precision
            
            cellsRight.Add(new Vector3(
                 (float)xeq ,                                                         //X
                (-plotB / plotA) * Mathf.Pow((float)xeq, 2.0f) + plotB * (float)xeq , //Y
                -1.0f));                                                              //Z
            cells.Add(Instantiate(cellGO, Quaternion.AngleAxis(chromosomes[3], Vector3.forward) * cellsRight[i] + transform.position + new Vector3(gameObject.GetComponent<Collider>().bounds.size.x/2,0.0f,0.0f)   , Quaternion.Euler(new Vector3(270, 0, 0))));
            cells[i].transform.SetParent(this.transform);
            cells[i].GetComponent<Renderer>().material.color = colorCell;
            cells[i].GetComponent<Transform>().localScale = localSc;
        }

        //Creacion de la pared izquierda
        cellsLeft.Clear();
        for (int i = 0; i <= chromosomes[1]; ++i)
        {
            float plotA = -chromosomes[0];
            float plotB = -chromosomes[2];
            double xeq = plotA / chromosomes[1] * i;//((float)i * plotA) / ((chromosomes[1])-1); //Double para aumentar la precision

            cellsLeft.Add(new Vector3(
                 (float)xeq,                                                         //X
                (-plotB / plotA) * Mathf.Pow((float)xeq, 2.0f) + plotB * (float)xeq, //Y
                -1.0f));                                                             //Z
            cells.Add(Instantiate(cellGO, Quaternion.AngleAxis(-chromosomes[3], Vector3.forward) * cellsLeft[i] + transform.position + new Vector3(-gameObject.GetComponent<Collider>().bounds.size.x / 2, 0.0f, 0.0f), Quaternion.Euler(new Vector3(270, 0, 0))));
            cells[(int)chromosomes[1] + i + 1].transform.SetParent(this.transform);
            cells[(int)chromosomes[1] + i +1].GetComponent<Renderer>().material.color = colorCell;
            cells[(int)chromosomes[1] + i +1].GetComponent<Transform>().localScale = localSc;
        } 
    }

    //Creacion de ojos hijos
    public void CreateEye(float[] newChromosome)
    {        
        chromosomes = newChromosome;       
        CreateEye();
    }

    //Getter de los cromosomas
    public float getChromosome(int chromosome)
    {
        return chromosomes[chromosome];
    }
}
