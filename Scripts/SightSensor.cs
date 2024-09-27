using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SightSensor : MonoBehaviour
{
    public Mesh mesh;
    [SerializeField] float Distance = 10;
    [SerializeField] float Angle = 10;
    [SerializeField] float Height = 1;
    [SerializeField] Color meshColor = Color.red;





    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int triangles = 8;
        int vertices = triangles * 3;

        Vector3 [] mesh_vertices = new Vector3[vertices];
        int [] mesh_triangles = new int[vertices];


        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -Angle, 0) * Vector3.forward * Distance;
        Vector3 bottomRight = Quaternion.Euler(0, Angle, 0) * Vector3.forward * Distance;


        Vector3 topCenter = bottomCenter + Vector3.up * Height;
        Vector3 topLeft = bottomLeft + Vector3.up * Height;
        Vector3 topRight = bottomRight + Vector3.up * Height;


        int vert = 0;
        // left side 
        mesh_vertices[vert++] = bottomCenter;
        mesh_vertices[vert++] = bottomLeft;
        mesh_vertices[vert++] = topLeft;

        mesh_vertices[vert++] = topLeft;
        mesh_vertices[vert++] = topCenter;
        mesh_vertices[vert++] = bottomCenter;


        // right side 
        mesh_vertices[vert++] = bottomCenter;
        mesh_vertices[vert++] = topCenter;
        mesh_vertices[vert++] = topRight;


        mesh_vertices[vert++] = topRight;
        mesh_vertices[vert++] = bottomRight;
        mesh_vertices[vert++] = bottomCenter;

        // far side
        mesh_vertices[vert++] = bottomLeft;
        mesh_vertices[vert++] = bottomRight;
        mesh_vertices[vert++] = topRight;


        mesh_vertices[vert++] = topRight;
        mesh_vertices[vert++] = topLeft;
        mesh_vertices[vert++] = bottomLeft;


        // top
        mesh_vertices[vert++] = topCenter;
        mesh_vertices[vert++] = topLeft;
        mesh_vertices[vert++] = topRight;


        // bottom
        mesh_vertices[vert++] = bottomCenter;
        mesh_vertices[vert++] = bottomRight;
        mesh_vertices[vert++] = bottomLeft;

        for(int i = 0; i < vertices; ++i)
        {
            mesh_triangles[i] = i;
        }

        mesh.vertices = mesh_vertices;
        mesh.triangles = mesh_triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

    private void OnValidate()
    {
        mesh =  CreateWedgeMesh();
    }
    
    private void OnDrawGizmos()  
    {
        if (mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh,transform.position , transform.rotation);
        }
    }

    private void Update()
    {
       
    }

}
