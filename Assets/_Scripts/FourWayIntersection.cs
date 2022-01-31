using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

public class FourWayIntersection : RoadPiece
{
    public PathCreator zMinusToZPlusPath = null;
    public PathCreator zMinusToXPlusPath = null;
    public PathCreator zMinusToXMinusPath = null;
    public PathCreator xPlusToXMinusPath = null;
    public PathCreator xPlusToZPlusPath = null;
    public PathCreator xPlusToZMinusPath = null;
    public PathCreator zPlusToZMinusPath = null;
    public PathCreator zPlusToXPlusPath = null;
    public PathCreator zPlusToXMinusPath = null;
    public PathCreator xMinusToXPlusPath = null;
    public PathCreator xMinusToZMinusPath = null;
    public PathCreator xMinusToZPlusPath = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
