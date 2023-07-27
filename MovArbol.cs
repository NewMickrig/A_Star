using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovArbol : MonoBehaviour
{
    public GameObject[,] Planos = new GameObject[20, 20];
    public int[,] Almacen = new int[20, 20];
    public float[,] Distancias = new float[20, 20];
    public GameObject Prefab;
    public GameObject Prefab2;
    public GameObject Meta;

    public float xMeta;
    public float zMeta;

    int Posicionx;
    int Posicionz;

    public bool Win;

    public List<Nodo> Camino;
    public List<Nodo> OpenSet;
    public List<Nodo> ClosedSet;

    public int[,] Vista = new int[,] { { 0, -1 }, { 1, 0 }, { -1, 0 }, { 0, 1 }, { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };

    public int[] Pesos = new int[] { 1, 1, 1, 1, 1, 1, 1, 1 };

    // Start is called before the first frame update
    void Start()
    {

        Win = true;
        Posicionx = (int)transform.localPosition.x;
        Posicionz = (int)transform.localPosition.z;
        xMeta = 10;
        zMeta = 10;
        Instantiate(Meta, new Vector3(xMeta, 0, zMeta), Quaternion.Euler(0, 0, 0));
        Construir();
        Camino = new List<Nodo> { };
        OpenSet = new List<Nodo> { };
        ClosedSet = new List<Nodo> { };

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            
            A_Star();
        }
    }

    
    void A_Star() 
    {
        Nodo Inicio = new Nodo(Posicionx, Posicionz);
        Nodo Final = new Nodo((int)xMeta, (int)zMeta);

        OpenSet.Add(Inicio);


        while (Win)
        {

            if (OpenSet.Count > 0)
            {
                Nodo MenorPeso = OpenSet[0];
                for (int i = 0; i < OpenSet.Count; i++)
                {
                    if (OpenSet[i].f < MenorPeso.f || OpenSet[i].f == MenorPeso.f && OpenSet[i].h < MenorPeso.h)
                    {
                        MenorPeso = OpenSet[i];
                    }
                }

                Nodo actual = MenorPeso;
                Borrar(OpenSet, actual);
                ClosedSet.Add(actual);
                print(actual.x + " ," + actual.z);
                if (actual.z == Final.z && actual.x == Final.x)
                {

                    Camino.Add(actual);
                    while (actual.padre != null)
                    {
                        actual = actual.padre;
                        Camino.Add(actual);
                    }
                    Camino.Reverse();
                    StartCoroutine(MovimientoArbol());
                    Win = false;
                }
                else
                {
                    if (actual.Hijos.Count < 1)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            int x = actual.x + Vista[i, 0];
                            int z = actual.z + Vista[i, 1];
                            if ((x < 20 && z < 20) && (z > 0 && x > 0))
                            {
                                Nodo Hijo = new Nodo(x, z);
                                if (Planos[x, z].CompareTag("Obstaculo"))
                                {
                                    Hijo.pared = true;
                                }
                                actual.Hijos.Add(Hijo);
                            }
                        }
                    }
                    foreach (Nodo Vecino in actual.Hijos)
                    {

                        if (Si_esta(Vecino) || Vecino.pared == true)
                        {
                        }
                        else
                        {
                            int MovCosto = actual.g + actual.Heuristica(Vecino);
                            if (MovCosto < Vecino.g || !Si_esta(Vecino))
                            {
                                Vecino.g = MovCosto;
                                Vecino.h = Vecino.Heuristica(Final);
                                Vecino.padre = actual;
                                if (!Si_esta(Vecino))
                                {
                                    OpenSet.Add(Vecino);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    IEnumerator MovimientoArbol()
    {
        for (int i = 0; i < Camino.Count; i++)
        {
            print(Camino[i].pared);
            float x = (float)Camino[i].x;
            float z = (float)Camino[i].z;
            this.transform.position = new Vector3(x, 0.5f, z);
            yield return new WaitForSeconds(0.2f);
        }

    }
        bool Si_esta(Nodo Vecino) 
        {
        for (int i = ClosedSet.Count - 1; i >= 0; i--)
        {
            if (ClosedSet[i].z == Vecino.z && ClosedSet[i].x == Vecino.x)
            {
                return true;
            }
        }
        return false; 
        }

        void Borrar(List<Nodo> array, Nodo Actual)
        {
            for (int i = array.Count - 1; i >= 0; i--)
            {
                if (array[i] == Actual)
                {
                    array.RemoveAt(i);
                }
            }
        }
   

        void Construir()
        {
            bool Forma = true;
            for (int x = 1; x < 20; x++)
            {
                for (int z = 1; z < 20; z++)
                {
                    Forma = !Forma;
                    if (Forma)
                    {
                        Planos[x, z] = Instantiate(Prefab, new Vector3((float)x, 0, (float)z), Quaternion.Euler(0, 0, 0)) as GameObject;
                        Distancias[x, z] = Vector3.Distance(Planos[x, z].transform.localPosition, new Vector3(xMeta, 0, zMeta));

                    }
                    else
                    {
                        Planos[x, z] = Instantiate(Prefab2, new Vector3((float)x, 0, (float)z), Quaternion.Euler(0, 0, 0)) as GameObject;
                        Distancias[x, z] = Vector3.Distance(Planos[x, z].transform.localPosition, new Vector3(xMeta, 0, zMeta));
                    }
                }
            }
        }


        /*
        void NodosHijos(Node<Node> Nodo)
        {
            for (int i = 0; i < 8; i++)
            {
                float x = Node.Position.localPosition.x + Vista[i, 0];
                float z = Node.Position.localPosition.z + Vista[i, 1];
                if ((x < 20 && z < 20) && (z > 0 && x > 0))
                {
                    if (x == UltPosx && z == UltPosz)
                    {

                    }

                }
            }

        }*/
    }


/*for (int i = 0; i < 8; i++)
{
    int x = Inicio.x + Vista[i, 0];
    int z = Inicio.z + Vista[i, 1];
    if ((x < 20 && z < 20) && (z > 0 && x > 0))
    {
        Nodo Hijo = new Nodo(x, z, Distancias[x, z]);
        Inicio.Hijos.Add(Hijo);
    }
    yield return null;
    //print("Primera Prueba");
}
Camino.Add(Inicio);

for (int i = 0; i < Inicio.Hijos.Count; i++) 
{
    //print("Peso :" + Inicio.Hijos[i].peso + " X: " + Inicio.Hijos[i].x + " Z:" + Inicio.Hijos[i].z);
}
Nodo Siguiente = Inicio.BuscarNodo();
//print(Siguiente.x + ", " +Siguiente.z+ " Peso: "+ );
//print("Inicio Segunda Prueba");
while (Win)
{
    //print("Segunda Prueba");
    for (int i = 0; i < 8; i++)
    {
        int x = Siguiente.x + Vista[i, 0];
        int z = Siguiente.z + Vista[i, 1];
        if ((x < 20 && z < 20) && (z > 0 && x > 0))
        {
            Nodo Hijo = new Nodo(x, z, Distancias[x, z]);
            Siguiente.Hijos.Add(Hijo);
        }
    }
    if (Siguiente.x == xMeta && Siguiente.z == zMeta)
    {
        Camino.Add(Siguiente);
        Win = false;
    }
    else {
        Camino.Add(Siguiente); 
        Siguiente = Siguiente.BuscarNodo();
    }
    yield return null;
}

for (int i = 0; i < Camino.Count; i++)
{
    float x = (float)Camino[i].x;
    float z = (float)Camino[i].z;
    this.transform.position = new Vector3(x, 0.5f, z);
    yield return new WaitForSeconds(0.5f);
}
*/
/*
OPEN_LIST
CLOSED_LIST
ADD start_cell to OPEN_LIST

LOOP
current_cell = cell in OPEN_LIST with the lowest F_COST
REMOVE current_cell from OPEN_LIST
ADD current_cell to CLOSED_LIST

IF current_cell is finish_cell
RETURN

FOR EACH adjacent_cell to current_cell
IF adjacent_cell is unwalkable OR adjacent_cell is in CLOSED_LIST
    SKIP to the next adjacent_cell

IF new_path to adjacent_cell is shorter OR adjacent_cell is not in OPEN_LIST
    SET F_COST of adjacent_cell
    SET parent of adjacent_cell to current_cell
    IF adjacent_cell is not in OPEN_LIST
        ADD adjacent_cell to OPEN_LIST */

/*Nodo Inicio = new Nodo(Posicionx, Posicionz);
        Nodo Final = new Nodo((int)xMeta, (int)zMeta);

        OpenSet.Add(Inicio);


        while (Win)
        {

            if (OpenSet.Count > 0)
            {
                Nodo MenorPeso = OpenSet[0];
                for (int i = 0; i < OpenSet.Count; i++)
                {
                    if (OpenSet[i].f < MenorPeso.f || OpenSet[i].f == MenorPeso.f && OpenSet[i].h < MenorPeso.h)
                    {
                        MenorPeso = OpenSet[i];
                    }
                    yield return null;
                }

                Nodo actual = MenorPeso;
                Borrar(OpenSet, actual);
                ClosedSet.Add(actual);
                print(actual.x + " ," + actual.z);
                if (actual.z == Final.z && actual.x == Final.x)
                {
                    Camino.Add(actual);
                    while (actual.padre != null)
                    {
                        actual = actual.padre;
                        Camino.Add(actual);
                    }
                    Camino.Reverse();
                    for (int i = 0; i < Camino.Count; i++)
                    {
                        print(Camino[i].pared);
                        float x = (float)Camino[i].x;
                        float z = (float)Camino[i].z;
                        this.transform.position = new Vector3(x, 0.5f, z);
                        yield return new WaitForSeconds(0.2f);
                    }
                    Win = false;
                }
                else
                {
                    if (actual.Hijos.Count < 1) 
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            int x = actual.x + Vista[i, 0];
                            int z = actual.z + Vista[i, 1];
                            if ((x < 20 && z < 20) && (z > 0 && x > 0))
                            {
                                Nodo Hijo = new Nodo(x, z);
                                if (Planos[x, z].CompareTag("Obstaculo"))
                                {
                                    Hijo.pared = true;
                                }
                                actual.Hijos.Add(Hijo);
                            }
                            yield return null;
                        }
                    }
                    foreach (Nodo Vecino in actual.Hijos)
                    {

                        if (Si_esta(Vecino) || Vecino.pared == true)
                        {
                            yield return null;
                        }
                        else {
                            int MovCosto = actual.g + actual.Heuristica(Vecino);
                            if (MovCosto < Vecino.g || !Si_esta(Vecino))
                            {
                                Vecino.g = MovCosto;
                                Vecino.h = Vecino.Heuristica(Final);
                                Vecino.padre = actual;
                                if (!Si_esta(Vecino))
                                {
                                    OpenSet.Add(Vecino);
                                }
                            }
                            yield return null;
                        }
                    }
                }
            }

            yield return null;
        }
        yield return new WaitForSeconds(1f);*/