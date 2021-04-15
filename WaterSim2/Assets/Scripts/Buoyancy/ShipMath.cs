using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMath
{
    //general fields(applicable on several Algorithms)
    private Transform shipTransform;
    private Vector3[] shipVerts;
    private int[] shipTris;
    private Rigidbody shipRB;

    //triangle algoritm function
    public Vector3[] shipVertsGlobal;
    float[] allDistancesToWater;

    private Mesh underWaterMesh;
    public List<TriangleData> underWaterTriangleData = new List<TriangleData>();
    public List<TriangleData> aboveWaterTriangleData = new List<TriangleData>();

    //for volume approximation
    private MeshCollider underWaterMeshCollider;

    public List<SlammingForceData> slammingForceData = new List<SlammingForceData>();

    public List<int> indexOfOriginalTriangle = new List<int>();

    public float boatArea;

    /*-----------TriangleAlgoritm----BEGIN-------*/
    public void InitTriCutAlgoritm(GameObject actor, GameObject underWaterObj, GameObject aboveWaterObj, Rigidbody shipRB)
    {
        //Get the transform
        shipTransform = actor.transform;
        this.shipRB = shipRB;

        underWaterMeshCollider = underWaterObj.GetComponent<MeshCollider>();
        underWaterMesh = underWaterObj.GetComponent<MeshFilter>().mesh;

        //Init the arrays and lists
        shipVerts = actor.GetComponent<MeshFilter>().mesh.vertices;
        shipTris = actor.GetComponent<MeshFilter>().mesh.triangles;
        //The boat vertices in global position
        shipVertsGlobal = new Vector3[shipVerts.Length];

        //Find all the distances to water once because some triangles share vertices, so reuse
        allDistancesToWater = new float[shipVerts.Length];

        // Setup the slamming force data
        for (int i = 0; i < (shipTris.Length / 3); i++)
        {
            slammingForceData.Add(new SlammingForceData());
        }
        CalculateOriginalTrianglesArea();
    }
    public void GenerateUnderwaterMesh() // generate under(and above)water mesh
    {
        //Reset
        underWaterTriangleData.Clear();
        aboveWaterTriangleData.Clear();

        for (int j = 0; j < slammingForceData.Count; j++)
        {
            slammingForceData[j].previousSubmergedArea = slammingForceData[j].submergedArea;
        }
        indexOfOriginalTriangle.Clear();

        //reset time here


        //Find all the distances to water once because some triangles share vertices, so reuse
        for (int j = 0; j < shipVerts.Length; j++) {
            Vector3 globalPos = shipTransform.TransformPoint(shipVerts[j]);
            shipVertsGlobal[j] = globalPos;
            //Debug.Log(globalPos);
            allDistancesToWater[j] = WaveHandler.instance.DistanceToWater(globalPos, Time.time);
            //Debug.Log(allDistancesToWater[j]);
        }
        AddTriangles();
        //Debug.Log(underWaterTriangleData.Count);
    }

    //Add all the triangles that's part of the underwater mesh
    private void AddTriangles()
    {
        //List that will store the data we need to sort the vertices based on distance to water
        List<VertexData> vertexData = new List<VertexData>();

        //Add init data that will be replaced
        vertexData.Add(new VertexData());
        vertexData.Add(new VertexData());
        vertexData.Add(new VertexData());


        //Loop through all the triangles (3 vertices at a time = 1 triangle)
        int i = 0, triangleCounter = 0;

        while (i < shipTris.Length)
        {
            //Loop through the 3 vertices
            for (int x = 0; x < 3; x++)
            {
                //Save the data we need
                vertexData[x].distance = allDistancesToWater[shipTris[i]];

                vertexData[x].index = x;

                vertexData[x].globalVertexPos = shipVertsGlobal[shipTris[i]];

                i++;
            }


            //All vertices are above the water
            if (vertexData[0].distance > 0f && vertexData[1].distance > 0f && vertexData[2].distance > 0f)
            {
                Vector3 p1 = vertexData[0].globalVertexPos;
                Vector3 p2 = vertexData[1].globalVertexPos;
                Vector3 p3 = vertexData[2].globalVertexPos;

                aboveWaterTriangleData.Add(new TriangleData(p1, p2, p3, shipRB, GameManager.secondsSinceStart));

                slammingForceData[triangleCounter].submergedArea = 0f;
                continue;
            }


            //Create the triangles that are below the waterline

            //All vertices are underwater
            if (vertexData[0].distance < 0f && vertexData[1].distance < 0f && vertexData[2].distance < 0f)
            {
                Vector3 p1 = vertexData[0].globalVertexPos;
                Vector3 p2 = vertexData[1].globalVertexPos;
                Vector3 p3 = vertexData[2].globalVertexPos;

                //Save the triangle
                underWaterTriangleData.Add(new TriangleData(p1, p2, p3, shipRB, GameManager.secondsSinceStart));
                slammingForceData[triangleCounter].submergedArea = slammingForceData[triangleCounter].originalArea;
                indexOfOriginalTriangle.Add(triangleCounter);
            }
            //1 or 2 vertices are below the water
            else
            {
                //Sort the vertices
                vertexData.Sort((x, y) => x.distance.CompareTo(y.distance));

                vertexData.Reverse();

                //One vertice is above the water, the rest is below
                if (vertexData[0].distance > 0f && vertexData[1].distance < 0f && vertexData[2].distance < 0f)
                {
                    AddTrianglesOneAboveWater(vertexData, triangleCounter);
                }
                //Two vertices are above the water, the other is below
                else if (vertexData[0].distance > 0f && vertexData[1].distance > 0f && vertexData[2].distance < 0f)
                {
                    AddTrianglesTwoAboveWater(vertexData, triangleCounter);
                }
            }
        }
    }

    
    public static Vector3 GetTriangleVelocity(Rigidbody boatRB, Vector3 triangleCenter)
    {
        //The connection formula for velocities (directly translated from Swedish)
        // v_A = v_B + omega_B cross r_BA
        // v_A - velocity in point A
        // v_B - velocity in point B
        // omega_B - angular velocity in point B
        // r_BA - vector between A and B

        Vector3 v_B = boatRB.velocity;

        Vector3 omega_B = boatRB.angularVelocity;

        Vector3 r_BA = triangleCenter - boatRB.worldCenterOfMass;

        Vector3 v_A = v_B + Vector3.Cross(omega_B, r_BA);

        return v_A;
    }

    //Calculate the area of a triangle with three coordinates
    public static float GetTriangleArea(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //Alternative 1 - Heron's formula
        float a = Vector3.Distance(p1, p2);
        //float b = Vector3.Distance(vertice_2_pos, vertice_3_pos);
        float c = Vector3.Distance(p3, p1);

        //float s = (a + b + c) / 2f;

        //float areaHeron = Mathf.Sqrt(s * (s-a) * (s-b) * (s-c));

        //Alternative 2 - Sinus
        float areaSin = (a * c * Mathf.Sin(Vector3.Angle(p2 - p1, p3 - p1) * Mathf.Deg2Rad)) / 2f;

        float area = areaSin;

        return area;
    }

    //Build the new triangles where one of the old vertices is above the water
    private void AddTrianglesOneAboveWater(List<VertexData> vertexData, int triangleCounter)
    {
        //H is always at position 0
        Vector3 H = vertexData[0].globalVertexPos;

        //Left of H is M
        //Right of H is L

        //Find the index of M
        int M_index = vertexData[0].index - 1;
        if (M_index < 0)
        {
            M_index = 2;
        }

        //We also need the heights to water
        float h_H = vertexData[0].distance;
        float h_M = 0f;
        float h_L = 0f;

        Vector3 M = Vector3.zero;
        Vector3 L = Vector3.zero;

        //This means M is at position 1 in the List
        if (vertexData[1].index == M_index)
        {
            M = vertexData[1].globalVertexPos;
            L = vertexData[2].globalVertexPos;

            h_M = vertexData[1].distance;
            h_L = vertexData[2].distance;
        }
        else
        {
            M = vertexData[2].globalVertexPos;
            L = vertexData[1].globalVertexPos;

            h_M = vertexData[2].distance;
            h_L = vertexData[1].distance;
        }


        //Now we can calculate where we should cut the triangle to form 2 new triangles
        //because the resulting area will always form a square

        //Point I_M
        Vector3 MH = H - M;

        float t_M = -h_M / (h_H - h_M);

        Vector3 MI_M = t_M * MH;

        Vector3 I_M = MI_M + M;


        //Point I_L
        Vector3 LH = H - L;

        float t_L = -h_L / (h_H - h_L);

        Vector3 LI_L = t_L * LH;

        Vector3 I_L = LI_L + L;


        //Save the data, such as normal, area, etc      
        //2 triangles below the water  
        underWaterTriangleData.Add(new TriangleData(M, I_M, I_L, shipRB, GameManager.secondsSinceStart));
        underWaterTriangleData.Add(new TriangleData(M, I_L, L, shipRB, GameManager.secondsSinceStart));
        //1 triangle above the water
        aboveWaterTriangleData.Add(new TriangleData(I_M, H, I_L, shipRB, GameManager.secondsSinceStart));

        float totalArea = ShipMath.GetTriangleArea(M, I_M, I_L) + ShipMath.GetTriangleArea(M, I_L, L);
        slammingForceData[triangleCounter].submergedArea = totalArea;
            
        indexOfOriginalTriangle.Add(triangleCounter);
        indexOfOriginalTriangle.Add(triangleCounter);
    }

    //Build the new triangles where two of the old vertices are above the water
    private void AddTrianglesTwoAboveWater(List<VertexData> vertexData, int triangleCounter)
    {
        //H and M are above the water
        //H is after the vertice that's below water, which is L
        //So we know which one is L because it is last in the sorted list
        Vector3 L = vertexData[2].globalVertexPos;

        //Find the index of H
        int H_index = vertexData[2].index + 1;
        if (H_index > 2)
        {
            H_index = 0;
        }


        //We also need the heights to water
        float h_L = vertexData[2].distance;
        float h_H = 0f;
        float h_M = 0f;

        Vector3 H = Vector3.zero;
        Vector3 M = Vector3.zero;

        //This means that H is at position 1 in the list
        if (vertexData[1].index == H_index)
        {
            H = vertexData[1].globalVertexPos;
            M = vertexData[0].globalVertexPos;

            h_H = vertexData[1].distance;
            h_M = vertexData[0].distance;
        }
        else
        {
            H = vertexData[0].globalVertexPos;
            M = vertexData[1].globalVertexPos;

            h_H = vertexData[0].distance;
            h_M = vertexData[1].distance;
        }


        //Now we can find where to cut the triangle

        //Point J_M
        Vector3 LM = M - L;

        float t_M = -h_L / (h_M - h_L);

        Vector3 LJ_M = t_M * LM;

        Vector3 J_M = LJ_M + L;


        //Point J_H
        Vector3 LH = H - L;

        float t_H = -h_L / (h_H - h_L);

        Vector3 LJ_H = t_H * LH;

        Vector3 J_H = LJ_H + L;


        //Save the data, such as normal, area, etc
        //1 triangle below the water
        underWaterTriangleData.Add(new TriangleData(L, J_H, J_M, shipRB, GameManager.secondsSinceStart));
        //2 triangles below the water
        aboveWaterTriangleData.Add(new TriangleData(J_H, H, J_M, shipRB, GameManager.secondsSinceStart));
        aboveWaterTriangleData.Add(new TriangleData(J_M, H, M, shipRB, GameManager.secondsSinceStart));
        //Calculate the submerged area
        slammingForceData[triangleCounter].submergedArea = BoatPhysicsMath.GetTriangleArea(L, J_H, J_M);

        indexOfOriginalTriangle.Add(triangleCounter);

    }

    //Help class to store triangle data so we can sort the distances
    public void DisplayMesh(Mesh mesh, string name, List<TriangleData> triangesData)
    { //Display the underwater mesh
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        //Build the mesh
        for (int i = 0; i < triangesData.Count; i++)
        {
            //From global coordinates to local coordinates
            Vector3 p1 = shipTransform.InverseTransformPoint(triangesData[i].p0);
            Vector3 p2 = shipTransform.InverseTransformPoint(triangesData[i].p1);
            Vector3 p3 = shipTransform.InverseTransformPoint(triangesData[i].p2);

            vertices.Add(p1);
            triangles.Add(vertices.Count - 1);

            vertices.Add(p2);
            triangles.Add(vertices.Count - 1);

            vertices.Add(p3);
            triangles.Add(vertices.Count - 1);
        }

        //Remove the old mesh
        mesh.Clear();

        //Give it a name
        mesh.name = name;

        //Add the new vertices and triangles
        mesh.vertices = vertices.ToArray();

        mesh.triangles = triangles.ToArray();

        mesh.RecalculateBounds();
    }

    private void CalculateOriginalTrianglesArea()
    {
        //Loop through all the triangles (3 vertices at a time = 1 triangle)
        int i = 0;
        int triangleCounter = 0;
        while (i < shipTris.Length)
        {
            Vector3 p1 = shipVerts[shipTris[i]];

            i++;

            Vector3 p2 = shipVerts[shipTris[i]];

            i++;

            Vector3 p3 = shipVerts[shipTris[i]];

            i++;

            //Calculate the area of the triangle
            float triangleArea = GetTriangleArea(p1, p2, p3);

            //Store the area in a list
            slammingForceData[triangleCounter].originalArea = triangleArea;

            //The total area
            boatArea += triangleArea;

            triangleCounter += 1;
        }
    }

    /*-----------TriangleAlgoritm----END-------*/

    /*-----------ResistanceAlgoritm----BEGIN-------*/

    //Force 1 - Viscous Water Resistance (Frictional Drag)
    public static Vector3 ViscousWaterResistanceForce(float rho, TriangleData triangleData, float Cf)
    {
        //Viscous resistance occurs when water sticks to the boat's surface and the boat has to drag that water with it

        // F = 0.5 * rho * v^2 * S * Cf
        // rho - density of the medium you have
        // v - speed
        // S - surface area
        // Cf - Coefficient of frictional resistance

        //We need the tangential velocity 
        //Projection of the velocity on the plane with the normal normalvec
        //http://www.euclideanspace.com/maths/geometry/elements/plane/lineOnPlane/
        Vector3 B = triangleData.normal;
        Vector3 A = triangleData.velocity;

        Vector3 velocityTangent = Vector3.Cross(B, (Vector3.Cross(A, B) / B.magnitude)) / B.magnitude;

        //The direction of the tangential velocity (-1 to get the flow which is in the opposite direction)
        Vector3 tangentialDirection = velocityTangent.normalized * -1f;

        //Debug.DrawRay(triangleCenter, tangentialDirection * 3f, Color.black);
        //Debug.DrawRay(triangleCenter, velocityVec.normalized * 3f, Color.blue);
        //Debug.DrawRay(triangleCenter, normal * 3f, Color.white);

        //The speed of the triangle as if it was in the tangent's direction
        //So we end up with the same speed as in the center of the triangle but in the direction of the flow
        Vector3 v_f_vec = triangleData.velocity.magnitude * tangentialDirection;

        //The final resistance force
        Vector3 viscousWaterResistanceForce = 0.5f * rho * v_f_vec.magnitude * v_f_vec * triangleData.area * Cf;

        viscousWaterResistanceForce = CheckForceIsValid(viscousWaterResistanceForce, "Viscous Water Resistance");

        return viscousWaterResistanceForce;
    }

    //The Coefficient of frictional resistance - belongs to Viscous Water Resistance but is same for all so calculate once
    public static float ResistanceCoefficient(float rho, float velocity, float length)
    {
        //Reynolds number

        // Rn = (V * L) / nu
        // V - speed of the body
        // L - length of the sumbmerged body
        // nu - viscosity of the fluid [m^2 / s]

        //Viscocity depends on the temperature, but at 20 degrees celcius:
        float nu = 0.000001f;
        //At 30 degrees celcius: nu = 0.0000008f; so no big difference

        //Reynolds number
        float Rn = (velocity * length) / nu;

        //The resistance coefficient
        float Cf = 0.075f / Mathf.Pow((Mathf.Log10(Rn) - 2f), 2f);

        return Cf;
    }

    //Force 2 - Pressure Drag Force
    public static Vector3 PressureDragForce(TriangleData triangleData)
    {
        //Modify for different turning behavior and planing forces
        //f_p and f_S - falloff power, should be smaller than 1
        //C - coefficients to modify 

        float velocity = triangleData.velocity.magnitude;

        //A reference speed used when modifying the parameters
        float velocityReference = velocity;

        velocity = velocity / velocityReference;

        Vector3 pressureDragForce = Vector3.zero;

        if (triangleData.cosTheta > 0f)
        {
            //float C_PD1 = 10f;
            //float C_PD2 = 10f;
            //float f_P = 0.5f;

            //To change the variables real-time - add the finished values later
            float C_PD1 = DebugPhysics.current.C_PD1;
            float C_PD2 = DebugPhysics.current.C_PD2;
            float f_P = DebugPhysics.current.f_P;

            pressureDragForce = -(C_PD1 * velocity + C_PD2 * (velocity * velocity)) * triangleData.area * Mathf.Pow(triangleData.cosTheta, f_P) * triangleData.normal;
        }
        else
        {
            //float C_SD1 = 10f;
            //float C_SD2 = 10f;
            //float f_S = 0.5f;

            //To change the variables real-time - add the finished values later
            float C_SD1 = DebugPhysics.current.C_SD1;
            float C_SD2 = DebugPhysics.current.C_SD2;
            float f_S = DebugPhysics.current.f_S;

            pressureDragForce = (C_SD1 * velocity + C_SD2 * (velocity * velocity)) * triangleData.area * Mathf.Pow(Mathf.Abs(triangleData.cosTheta), f_S) * triangleData.normal;
        }

        pressureDragForce = CheckForceIsValid(pressureDragForce, "Pressure drag");

        return pressureDragForce;
    }

    //Force 3 - Slamming Force (Water Entry Force)
    public static Vector3 SlammingForce(SlammingForceData slammingData, TriangleData triangleData, float boatArea, float boatMass)
    {
        //To capture the response of the fluid to sudden accelerations or penetrations

        //Add slamming if the normal is in the same direction as the velocity (the triangle is not receding from the water)
        //Also make sure thea area is not 0, which it sometimes is for some reason
        if (triangleData.cosTheta < 0f || slammingData.originalArea <= 0f)
        {
            return Vector3.zero;
        }


        //Step 1 - Calculate acceleration
        //Volume of water swept per second
        Vector3 dV = slammingData.submergedArea * slammingData.velocity;
        Vector3 dV_previous = slammingData.previousSubmergedArea * slammingData.previousVelocity;

        //Calculate the acceleration of the center point of the original triangle (not the current underwater triangle)
        //But the triangle the underwater triangle is a part of
        Vector3 accVec = (dV - dV_previous) / (slammingData.originalArea * Time.fixedDeltaTime);

        //The magnitude of the acceleration
        float acc = accVec.magnitude;

        //Debug.Log(slammingForceData.originalArea);

        //Step 2 - Calculate slamming force
        // F = clamp(acc / acc_max, 0, 1)^p * cos(theta) * F_stop
        // p - power to ramp up slamming force - should be 2 or more

        // F_stop = m * v * (2A / S)
        // m - mass of the entire boat
        // v - velocity
        // A - this triangle's area
        // S - total surface area of the entire boat

        Vector3 F_stop = boatMass * triangleData.velocity * ((2f * triangleData.area) / boatArea);

        //float p = DebugPhysics.current.p;

        //float acc_max = DebugPhysics.current.acc_max;

        float p = 2f;

        float acc_max = acc;

        float slammingCheat = DebugPhysics.current.slammingCheat;

        Vector3 slammingForce = Mathf.Pow(Mathf.Clamp01(acc / acc_max), p) * triangleData.cosTheta * F_stop * slammingCheat;

        //Vector3 slammingForce = Vector3.zero;

        //Debug.Log(slammingForce);

        //The force acts in the opposite direction
        slammingForce *= -1f;

        slammingForce = CheckForceIsValid(slammingForce, "Slamming");

        return slammingForce;

    }

    //
    // Resistance forces from the book "Physics for Game Developers"
    //

    //Force 1 - Frictional drag - same as "Viscous Water Resistance" above, so empty
    //FrictionalDrag()

    //Force 2 - Residual resistance - similar to "Pressure Drag Forces" above
    public static float ResidualResistanceForce()
    {
        // R_r = R_pressure + R_wave = 0.5 * rho * v * v * S * C_r
        // rho - water density
        // v - speed of ship
        // S - surface area of the underwater portion of the hull
        // C_r - coefficient of residual resistance - increases as the displacement and speed increases

        //Coefficient of residual resistance
        //float C_r = 0.002f; //0.001 to 0.003

        //Final residual resistance
        //float residualResistanceForce = 0.5f * rho * v * v * S * C_r; 

        //return residualResistanceForce;

        float residualResistanceForce = 0f;

        return residualResistanceForce;
    }

    //Force 3 - Air resistance on the part of the ship above the water (typically 4 to 8 percent of total resistance)
    public static Vector3 AirResistanceForce(float rho, TriangleData triangleData, float C_air)
    {
        // R_air = 0.5 * rho * v^2 * A_p * C_air
        // rho - air density
        // v - speed of ship
        // A_p - projected transverse profile area of ship
        // C_r - coefficient of air resistance (drag coefficient)

        //Only add air resistance if normal is pointing in the same direction as the velocity
        if (triangleData.cosTheta < 0f)
        {
            return Vector3.zero;
        }

        //Find air resistance force
        Vector3 airResistanceForce = 0.5f * rho * triangleData.velocity.magnitude * triangleData.velocity * triangleData.area * C_air;

        //Acting in the opposite side of the velocity
        airResistanceForce *= -1f;

        airResistanceForce = CheckForceIsValid(airResistanceForce, "Air resistance");

        return airResistanceForce;
    }

    //Check that a force is not NaN
    private static Vector3 CheckForceIsValid(Vector3 force, string forceName)
    {
        if (!float.IsNaN(force.x + force.y + force.z))
        {
            return force;
        }
        else
        {
            Debug.Log(forceName += " force is NaN");

            return Vector3.zero;
        }
    }

    /*-----------ResistanceAlgoritm----END-------*/

    /*-----------BouyancyForceAlgoritm----BEGIN-------*/

    public static Vector3 BuoyancyForce(float waterDensity, TriangleData triangleData)
    {
        Vector3 buoyancyForce = waterDensity * Physics.gravity.y * triangleData.distanceToSurface * triangleData.area * triangleData.normal;

        //The vertical component of the hydrostatic forces don't cancel out but the horizontal do
        buoyancyForce.x = 0f;
        buoyancyForce.z = 0f;

        buoyancyForce = CheckForceIsValid(buoyancyForce, "Buoyancy");

        return buoyancyForce;
    }

    /*-----------BouyancyForceAlgoritm----END-------*/

    /*-----------NewAlgoritm----END-------*/

    /*-----------Structs----BEGIN-------*/

    private class VertexData
    {
        //The distance to water from this vertex
        public float distance;
        //An index so we can form clockwise triangles
        public int index;
        //The global Vector3 position of the vertex
        public Vector3 globalVertexPos;
    }


    /*-----------Structs----END-------*/
}
