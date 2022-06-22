using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Siccity.GLTFUtility;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

using RosPosVector3 = Unity.Robotics.ROSTCPConnector.ROSGeometry.Vector3<Unity.Robotics.ROSTCPConnector.ROSGeometry.FLU>;


public class Loader : MonoBehaviour
{
    // Start is called before the first frame update

    // "package://ur_description/meshes/ur3/visual/base.dae" : Ur3Base,
    // "package://ur_description/meshes/ur3/visual/forearm.dae": Ur3Forearm,
    // "package://ur_description/meshes/ur3/visual/shoulder.dae" : Ur3Shoulder,
    // "package://ur_description/meshes/ur3/visual/upperarm.dae" : Ur3Upperarm,
    // "package://ur_description/meshes/ur3/visual/wrist1.dae" : Ur3Wrist1, 
    // "package://ur_description/meshes/ur3/visual/wrist2.dae" : Ur3Wrist2,
    // "package://ur_description/meshes/ur3/visual/wrist3.dae" : Ur3Wrist3,


    public DataParser parser;
    public List<GameObject> meshList = new List<GameObject>();
    

    private const string Ur3Base = "Assets/Meshes/Ur3/visual/base.glb";
    private const string Ur3Forearm = "Assets/Meshes/Ur3/visual/forearm.glb";
    private const string Ur3Shoulder = "Assets/Meshes/Ur3/visual/shoulder.glb";
    private const string Ur3Upperarm = "Assets/Meshes/Ur3/visual/upperarm.glb";
    private const string Ur3Wrist1 = "Assets/Meshes/Ur3/visual/wrist1.glb";
    private const string Ur3Wrist2 = "Assets/Meshes/Ur3/visual/wrist2.glb";
    private const string Ur3Wrist3 = "Assets/Meshes/Ur3/visual/wrist3.glb";

    private Quaternion unityQuat;
    private Vector3 unityPos;



    private Dictionary<string, string> meshLookupTable = new Dictionary<string, string>();
    void Start()
    {
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/base.dae", Ur3Base);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/forearm.dae", Ur3Forearm);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/shoulder.dae", Ur3Shoulder);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/upperarm.dae", Ur3Upperarm);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/wrist1.dae", Ur3Wrist1);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/wrist2.dae", Ur3Wrist2);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/wrist3.dae", Ur3Wrist3);
        foreach (DataParser.Item item in parser.currentItemList.items)
        {
            if (meshLookupTable.ContainsKey(item.shape))
            {
                foreach (DataParser.Tf tf in parser.currentTfsList.tfs)
                    {
                        if (item.id == tf.id){

                            GameObject tfGameObject = new GameObject(tf.id);
                //GameObject mesh = ImportGLTF(meshLookupTable[item.shape]);
                GameObject mesh = Importer.LoadFromFile(meshLookupTable[item.shape]);
                mesh.transform.parent = tfGameObject.transform;

                mesh.name = item.name;
                Vector3<FLU> rosTFPos = new Vector3<FLU>(tf.position.x, tf.position.y, tf.position.z);

               
                Vector3<FLU> rosItemPos = new Vector3<FLU>(item.position.x, item.position.y, item.position.z);

               
                Vector3 unityTFPos = rosTFPos.toUnity;
                Vector3 unityItemPos = rosItemPos.toUnity;

                Quaternion<FLU> rosTFQuat = new Quaternion<FLU>(tf.rotation.x, tf.rotation.y, tf.rotation.z, tf.rotation.w);
                Quaternion<FLU> rosItemQuat = new Quaternion<FLU>(item.rotation.x, item.rotation.y, item.rotation.z, item.rotation.w);


               
                Quaternion unityTFQuat = rosTFQuat.toUnity;
                Quaternion unityItemQuat = rosItemQuat.toUnity;
              

                Vector3 itemScale = new Vector3(item.scale.x, item.scale.y, item.scale.z);
               
                tfGameObject.transform.position = unityTFPos;
                tfGameObject.transform.rotation = unityTFQuat;
                mesh.transform.localRotation = unityItemQuat;
               
                mesh.transform.localPosition = unityItemPos;
               
                
                //Debug.Log(mesh.name+"\nROS : " + itemPos + " & " + itemQuat + "\nUnity : " + unityPos + " & " + unityQuat);
               
                mesh.GetComponent<Transform>().localScale = itemScale;
               
                meshList.Add(mesh);
                        }

                    }
                

                
            }
        }


    }

    // Update is called once per frame
    void Update()
    {

    }

    GameObject ImportGLTF(string filepath)
    {
        GameObject result = Importer.LoadFromFile(filepath);
        //Debug.Log(result.name);
        return result;
    }

    public (Vector3, Quaternion) toLeftHanded(Matrix4x4 m)
    {
        // this matrix switches y and z
        Matrix4x4 M = new Matrix4x4(
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(0, 0, 0, 1)
        );
        //m = M.inverse * m * M;
        m = M * m * M.inverse;
        Vector3 t = m.GetPosition();
        Quaternion q = m.rotation;
        return (t, q);
    }

    public (Vector3, Quaternion) toRightHanded(Matrix4x4 m)
    {

        Matrix4x4 M = new Matrix4x4(
            new Vector4(0, -1, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 0, 0, 1)
        ).transpose;
        m = M.inverse * m * M;
        Vector3 t = m.GetPosition();
        Quaternion q = m.rotation;
        return (new Vector3(t.x, t.y, t.z), new Quaternion(q.x, q.y, q.z, q.w));
    }

    private Quaternion ConvertToUnity(Quaternion input)
    {
        Matrix4x4 M = new Matrix4x4(
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(0, 0, 0, 1)
        );

        Matrix4x4 unity_rot_mat = M * Matrix4x4.Rotate(input) * M.transpose;

        Quaternion output = QuaternionFromMatrix(unity_rot_mat);

        return output;

        //return new Quaternion(
        //     input.y,   // -(  right = -left  )
        //    -input.z,   // -(     up =  up     )
        //     input.x,   // -(forward =  forward)
        //     input.w
        //);
    }

    public static Quaternion QuaternionFromMatrix(Matrix4x4 m) {
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
    }
}
