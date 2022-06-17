using UnityEngine;
using System.Runtime.InteropServices;

public class BlockChainConnect : MonoBehaviour {

    [DllImport("__Internal")]
    public static extern void RequestPlugConnect(int cbIndex);

    /* 
    [DllImport("__Internal")]
    public static extern void CheckPlugConnection(int cbIndex);

    [DllImport("__Internal")]
    public static extern int pay(int cbIndex, double to, double amount);*/

    void Start() {

        Debug.Log("Block Chain Connect Start");
        
        /* Hello();
        
        HelloString("This is a string.");
        
        float[] myArray = new float[10];
        PrintFloatArray(myArray, myArray.Length);
        
        int result = AddNumbers(5, 7);
        Debug.Log(result);
        
        Debug.Log(StringReturnValueFunction());
        
        var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
        BindWebGLTexture(texture.GetNativeTexturePtr()); */
    }

    public void Call_Loaded()
    {
    }
}