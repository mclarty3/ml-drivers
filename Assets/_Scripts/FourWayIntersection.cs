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


    protected override RoadConnection GetRoadConnectionFromVector(Vector3 vector) { return null; }
    public override RoadConnection AddConnectionFromVector(Vector3 vector, RoadConnection other,
                                                           RoadPiece otherPiece, out GameObject go)
    { 
        go = null;
        return null; 
    }
    
    public override GameObject[] HandleRoadPlacement(RoadPiece toPlace, bool dontRepeat=false) 
    { return null; }
}
