using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManagerScr : MonoBehaviour {

    [SerializeField] private GameObject enviroment;
    public List<GameObject> population = new List<GameObject>();
    public List<GameObject> populationOrder = new List<GameObject>();
    public List<GameObject> populationToGenerate = new List<GameObject>();
    public AudioSource digimon;
    GameObject mediumInd;
    public Text infoMedium;
    float[] mediumChromosomes = new float[Constants.minValues.Length];
    private int lightMax;
    int removed = 0;    
    bool flag = false;
    bool flagMedium = false;
    public bool shineRay = false;

    
    // Use this for initialization
    void Start () {
        FirstGeneration();
        InvokeRepeating("NewGeneration", Constants.firstSpawnTime, Constants.spawnInterval);
        MediumValue();
    }

    //CREACION DE LA PRIMERA GENERACION DE OJOS
    public void FirstGeneration()
    {   
        //Se crean los enviroments deseados (Cuadrado con ojo, sol y paredes)
        byte iteratorY = 0;
        for (byte iterator = 0; iterator < Constants.generationSize; ++iterator)
        {
            if (iterator % 10 == 0)
            {
                ++iteratorY;
            }

            //Se crea un ojo en cada enviroment, generando la poblacion
            population.Add(Instantiate(
                enviroment,
                new Vector3(15.0f * iterator - iteratorY * 150, -15.0f * iteratorY, 0.0f),
                Quaternion.identity)
                );
            population[iterator].transform.Find("photosensible").GetComponent<eye>().CreateEye();
            population[iterator].transform.Find("photosensible").GetComponent<eye>().generacion++;
            population[iterator].GetComponentInChildren<sun>().shine(shineRay);
            population[iterator].transform.Find("photosensible").gameObject.GetComponent<eye>().aptitud = population[iterator].GetComponentInChildren<sun>().choques;
        }


        //Se obtiene el valor medio de los cromosomas de toda la poblacion
        for (int i = 0; i < Constants.minValues.Length; i++)
        {
            for (int j = 0; j < population.Count; j++)
            {
                mediumChromosomes[i] += population[j].transform.Find("photosensible").GetComponent<eye>().getChromosome(i);
            }
            mediumChromosomes[i] = mediumChromosomes[i] / Constants.generationSize;            
        }

        //Se crea un nuevo enviroment con ojo, para mostrar estos valores medios (solo es una muestra, no se incluye en la poblacion)
        mediumInd = Instantiate(
                enviroment,
                new Vector3(0.0f, 0.0f, 10.0f),
                Quaternion.identity
                );
        mediumInd.tag = "Untagged";
        mediumInd.transform.Find("photosensible").GetComponent<eye>().CreateEye(mediumChromosomes);
        
    }

    //GESTIONA LOS VALORES MEDIOS DE LOS CROMOSOMAS DE TODA LA POBLACION Y SE LOS PASA AL ENVIORMENT DE MUESTRA
    void MediumValue()
    {
        flagMedium = false;
        infoMedium.text = "1_Distance, 2_Quantity , 3_m, 4_Rotacion,\n 5_Roughness(rgb), 6_Refraction(a), 7_Scale \n\n";
        
        //Se borra el ojo
        for (int j = 0; j < mediumInd.transform.Find("photosensible").GetComponent<eye>().cells.Count; j++)
        {
            Destroy(mediumInd.transform.Find("photosensible").GetComponent<eye>().cells[j]);
        }
        mediumInd.transform.Find("photosensible").GetComponent<eye>().cells.Clear();
        
        //Se obtiene la media de cromosomas de la poblacion
        for (int i = 0; i < Constants.minValues.Length; i++)
        {
            for (int j = 0; j < population.Count; j++)
            {
                if (!flagMedium)
                {
                    mediumChromosomes[i] = population[j].transform.Find("photosensible").GetComponent<eye>().getChromosome(i);
                    flagMedium = true;                    
                }
                else
                {
                    mediumChromosomes[i] += population[j].transform.Find("photosensible").GetComponent<eye>().getChromosome(i);
                }
            }

            mediumChromosomes[i] = mediumChromosomes[i] / Constants.generationSize;

            infoMedium.text += "Chromosome " + i + " : " + mediumChromosomes[i] + "\n";
        }

        //Se crea el ojo de nuevo
        mediumInd.transform.Find("photosensible").GetComponent<eye>().CreateEye(mediumChromosomes);
        mediumInd.GetComponentInChildren<sun>().choques = 0;
        mediumInd.GetComponentInChildren<sun>().shine(shineRay);
        infoMedium.text += "Aptitud:" + mediumInd.GetComponentInChildren<sun>().choques + "\n";

    }

    //GENERA OJOS HIJOS A PARTIR DE LOS PADRES SUPERVIVIENTES
    void NewGeneration()
    {
        GameObject populationToAdd;

        if (!flag)
        {
            digimon.Play(); 
            flag = true;
        }

        //Ordena los ojos por aptitud, los primeros son los que mas rayos de luz reciben
        while (population.Count != 0)
        {
            populationToAdd = null;
            lightMax = -1;

            for (int i = 0; i < population.Count; i++)
            {
                if (population[i].GetComponentInChildren<sun>().choques > lightMax)
                {
                    lightMax = population[i].GetComponentInChildren<sun>().choques;
                    populationToAdd = population[i];

                }
            }
            populationOrder.Add(populationToAdd);
            population.Remove(populationToAdd);
        }

        int removed = 0;
        int porcentToDie = 100;

        //Se borra la mitad de la poblacion, segun un porcentaje que aumenta de mayor a menor, empezando por los peores puntuados
        for (int i = populationOrder.Count - 1; i >= 0; i--)
        {
            if (removed != Constants.generationSize/2)
            {
                if (Random.Range(0, 100) < porcentToDie)
                {
                    for (int j = 0; j < populationOrder[i].transform.Find("photosensible").GetComponent<eye>().cells.Count; j++)
                    {
                        Destroy(populationOrder[i].transform.Find("photosensible").GetComponent<eye>().cells[j]);
                    }
                    populationToGenerate.Add(populationOrder[i]);
                    populationOrder.Remove(populationOrder[i]);
                    removed++;
                }
            }
            porcentToDie -= 100/populationOrder.Count;
        }


        //Comprobamos si no se han borrado 10, si es asi las borramos directas sin %
        if (removed != Constants.generationSize / 2)
        {
            for (int i = populationOrder.Count - 1; i >= 0; i--)
            {
                if (removed != Constants.generationSize / 2)
                {
                    for (int j = 0; j < populationOrder[i].transform.Find("photosensible").GetComponent<eye>().cells.Count; j++)
                    {
                        Destroy(populationOrder[i].transform.Find("photosensible").GetComponent<eye>().cells[j]);
                        populationOrder[i].transform.Find("photosensible").GetComponent<eye>().generacion++;
                    }

                    populationToGenerate.Add(populationOrder[i]);
                    populationOrder.Remove(populationOrder[i]);
                    removed++;
                }
            }
        }


        //Entre los ojos supervivientes, elegimos padres y creamos hijos con sus cromosomas
        for (int iterator = 0; iterator < populationToGenerate.Count; iterator++) { 
            short[] parents = new short[Constants.numberOfParents];

            // Elegir padres        
            for (short i = 0; i < parents.Length; i++)
            {
                parents[i] = (short)Random.Range(0, (int)(populationOrder.Count));
            }

            // Recombinación
            float[] chromosomes = new float[Constants.minValues.Length];
            for (byte i = 0; i < chromosomes.Length; i++)
            {
                short parent = (short)Random.Range(0, parents.Length);
                chromosomes[i] = populationOrder[parents[parent]].transform.Find("photosensible").GetComponent<eye>().getChromosome(i);
            }
            
            // Mutación       
            for (byte j = 0; j < chromosomes.Length; j++)
            {
                if (Random.Range(0f, 1f) <= Constants.mutationRatio)
                    chromosomes[j] = Random.Range(Constants.minValues[j], Constants.maxValues[j]);
            }

            // Crear Ojo        
            populationToGenerate[iterator].transform.Find("photosensible").GetComponent<eye>().CreateEye(chromosomes);
        }

        //Se reinician los datos para la siguiente vuelta
        populationToGenerate.Clear();
        populationOrder.Clear();
        population.AddRange(GameObject.FindGameObjectsWithTag("enviroment"));
        
        for (byte i = 0; i < population.Count; i++) {
            population[i].GetComponentInChildren<sun>().choques = 0;
            population[i].GetComponentInChildren<sun>().shine(shineRay);
            population[i].transform.Find("photosensible").gameObject.GetComponent<eye>().aptitud = population[i].GetComponentInChildren<sun>().choques;
        }

        //Obtencion del valor medio
        MediumValue();
    }
}
