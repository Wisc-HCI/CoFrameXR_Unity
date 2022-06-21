using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Siccity.GLTFUtility;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;


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
    public List<GameObject> meshList = new List<GameObject> ();
    
    private const string Ur3Base = "Assets/Meshes/Ur3/visual/base.glb";
    private const string Ur3Forearm = "Assets/Meshes/Ur3/visual/forearm.glb";
    private const string Ur3Shoulder = "Assets/Meshes/Ur3/visual/shoulder.glb";
    private const string Ur3Upperarm = "Assets/Meshes/Ur3/visual/upperarm.glb";
    private const string Ur3Wrist1 = "Assets/Meshes/Ur3/visual/wrist1.glb";
    private const string Ur3Wrist2 = "Assets/Meshes/Ur3/visual/wrist2.glb";
    private const string Ur3Wrist3 = "Assets/Meshes/Ur3/visual/wrist3.glb";

    
    
    private Dictionary<string, string> meshLookupTable = new Dictionary<string,string>();
    void Start()
    {
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/base.dae",Ur3Base);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/forearm.dae",Ur3Forearm);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/shoulder.dae",Ur3Shoulder);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/upperarm.dae",Ur3Upperarm);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/wrist1.dae",Ur3Wrist1);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/wrist2.dae",Ur3Wrist2);
        meshLookupTable.Add("package://ur_description/meshes/ur3/visual/wrist3.dae",Ur3Wrist3);
        foreach (DataParser.Item item in parser.currentItemList.items){
            if (meshLookupTable.ContainsKey(item.shape)){
                GameObject mesh = ImportGLTF(meshLookupTable[item.shape]);
                Debug.Log(new Vector3(item.position.x,item.position.y,item.position.z));
                Vector3<FLU>rosPos = new Vector3<FLU>(item.position.x,item.position.y,item.position.z);
                Debug.Log(item.name + rosPos);
                //Vector3<FLU> rosPos = tempPos.To<FLU>();
                 Vector3 unityPos = rosPos.toUnity;
                 Debug.Log(unityPos);
                 mesh.GetComponent<Transform>().position = unityPos;

                Debug.Log(new Quaternion(item.rotation.x, item.rotation.y, item.rotation.z, item.rotation.w));
                Quaternion<FLU> rosQuat = new Quaternion<FLU>(item.rotation.x, item.rotation.y, item.rotation.z, item.rotation.w);
                //Quaternion<FLU> rosQuat= tempQuat.To<FLU>();
                Debug.Log(rosQuat);
                Quaternion unityQuat = rosQuat.toUnity;
                Quaternion<FLU> tempQuat = unityQuat.To<FLU>();
                 Debug.Log(unityQuat);
                 Debug.Log(tempQuat);
                // unityQuat.x *= 0.5;
                // unityQuat.y += 
                mesh.GetComponent<Transform>().rotation = unityQuat;
                //new Quaternion(item.rotation.x, item.rotation.y, item.rotation.z, item.rotation.w));
                ;
                //Debug.Log(new Quaternion(item.rotation.x, item.rotation.y, item.rotation.z, item.rotation.w));
                mesh.GetComponent<Transform>().localScale = new Vector3(item.scale.x,item.scale.y,item.scale.z);
                //Matrix4x4 transformMatrix = Matrix4x4.TRS(mesh.GetComponent<Transform>().position,mesh.GetComponent<Transform>().rotation,mesh.GetComponent<Transform>().localScale);
                //(Vector3 transformPos,Quaternion transformRot) = toRightHanded(transformMatrix);
                //Debug.Log(transformPos);
                //Debug.Log(transformRot);
                // mesh.GetComponent<Transform>().position = transformPos;
                // mesh.GetComponent<Transform>().rotation = transformRot;
                meshList.Add(mesh);
                
                //Debug.Log(item.name);
            }
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject ImportGLTF(string filepath) {
        GameObject result = Importer.LoadFromFile(filepath);
    return result;
}

 public (Vector3, Quaternion) toLeftHanded(Matrix4x4 m) {

        Matrix4x4 M = new Matrix4x4(
            new Vector4( 1, 0, 0, 0),
            new Vector4( 0, 0, 1, 0),
            new Vector4( 0, 1, 0, 0),
            new Vector4( 0, 0, 0, 1)
        );
        m = M.inverse * m * M.inverse;
        Vector3 t = m.GetPosition();
        Quaternion q = m.rotation;
        return (t, q);
    }

private Quaternion ConvertToUnity(Quaternion input) {
    return new Quaternion(
         input.y,   // -(  right = -left  )
        -input.z,   // -(     up =  up     )
         input.x,   // -(forward =  forward)
         input.w
    );
}
}
