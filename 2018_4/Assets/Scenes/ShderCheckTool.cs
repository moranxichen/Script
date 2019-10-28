using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class ShaderCheckTool : EditorWindow
{


    public Object checkShader;
    public string ShaderProperties;
    //private Shader checkShader;
    public float OutLineScale;

    [MenuItem("Tools/ShaderCheckTool")]

    static void Toolswindow()
    {
        ShaderCheckTool window = (ShaderCheckTool)EditorWindow.GetWindow(typeof(ShaderCheckTool));
        window.Show();
    }

    public static string shadername;

    public  string skipFilesName;
    public string OnlyCheckFilesName;

    bool ioscheck = true;
    bool androidcheck = false;
   // Vector2 TextureCheckGUIOPosition;
    private void OnGUI()
    {
        GUILayout.Label("About shader", EditorStyles.boldLabel);
        //EditorGUILayout.BeginHorizontal();
        checkShader = EditorGUILayout.ObjectField(label: "SelectShader", checkShader, typeof(Shader), true);
        ShaderProperties = EditorGUILayout.TextField("ShaderProperties:", ShaderProperties);
        GUILayout.Label("OutLineScale是取缩放值为1的prefab单位查看描边宽度", EditorStyles.boldLabel);
        OutLineScale = EditorGUILayout.FloatField("OutLineScale", OutLineScale);

        if (checkShader != null)
        {
            shadername = checkShader.ToString();
        }

        if (GUILayout.Button("Check"))
        {
            shadername = shadername.Replace("(UnityEngine.Shader)", "");
            shadername = shadername.Trim();

            Listmat();
           //Write();

            Debug.Log("写入成功,写入到" + Application.dataPath + "/MateCheckPath.txt");
            Debug.Log("工程材质球数量:" + MaterialPath.Count);
            Debug.Log("ShaderGUID:" + guid01);  
            Debug.Log("ShaderName:" + shadername);
        }

        GUILayout.Label("TextureCheck", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        ioscheck = GUILayout.Toggle(ioscheck, "IOS");
        androidcheck = GUILayout.Toggle(androidcheck, "Android");
        GUILayout.EndHorizontal();
        GUILayout.Label("贴图maxTextureSize");

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("256"))
        {
            maxTextureSize = 256;
           
        }
        if (GUILayout.Button("512"))
        {
            maxTextureSize = 512;
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("贴图大小选择:"+maxTextureSize);

        skipFilesName = EditorGUILayout.TextField("跳过检查关键词(关键词):", skipFilesName);
        OnlyCheckFilesName = EditorGUILayout.TextField("只检查修改文件夹(文件夹名):", OnlyCheckFilesName);

        if (GUILayout.Button("TextureCheck"))
        {
            TextureCheck();
        }
       
    }

    public List<string> MaterialPath = new List<string>();
    //public static List<Material> Shadermat = new List<Material>();
    public List<string> Checkshaderpath = new List<string>();

    public List<string> PrefabPath = new List<string>();
    public List<string> PrefabQuotePath = new List<string>();

    //public static List<string> ObjectPath = new List<string>();
    //public static List<GameObject> gameobjectlist = new List<GameObject>();
    //public static List<string> objGUID = new List<string>();

    public static string guid01;

    void Listmat()
    {
        string path = Application.dataPath;
        MaterialPath = new List<string>(Directory.GetFiles(path, "*.mat", SearchOption.AllDirectories));
        PrefabPath = new List<string>(Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories));
        Checkshaderpath = new List<string>();
        PrefabQuotePath = new List<string>();

        Shader shaderpath = (Shader.Find(shadername));
        string shaderguid = AssetDatabase.GetAssetPath(shaderpath);
        guid01 = AssetDatabase.AssetPathToGUID(shaderguid); //获取shaderGUID

        for (int i = 0; i < MaterialPath.Count; i++)
        {
            string filepath = MaterialPath[i];


            if (Regex.IsMatch(File.ReadAllText(filepath), guid01))
            {
                Checkshaderpath.Add(MaterialPath[i]);

                string matpath = MaterialPath[i];
                string[] matArray = matpath.Split('/');
                string path01 = matArray[matArray.Length - 1];
                string matguid = AssetDatabase.AssetPathToGUID(path01);

                for (int t = 0; t < PrefabPath.Count; t++)
                {
                    string prefabfilepath = PrefabPath[t];
                    if (Regex.IsMatch(File.ReadAllText(prefabfilepath), matguid))
                    {
                        PrefabQuotePath.Add(PrefabPath[t]);
                        //Debug.LogWarning(PrefabPath[t] + "引用了" + path01);

                        string prefabpathstring = PrefabPath[t];
                        string[] prefabArray = prefabpathstring.Split('/');
                        string path02 = prefabArray[prefabArray.Length - 1];
                        string[] PFchildname = path02.Split(new char[2] { '.', '\\' });
                        string childname = PFchildname[PFchildname.Length - 2];
                        string[] childname01Array = childname.Split('_');
                        string childname02 = childname01Array[0];

                        GameObject thisloadprefab = AssetDatabase.LoadAssetAtPath<GameObject>(path02);
                        // Debug.Log(childname02);
                        Vector3 prefabscale = thisloadprefab.transform.GetChild(0).gameObject.transform.localScale;
                        if (prefabscale != null)
                        {
                            Material thismat = AssetDatabase.LoadAssetAtPath<Material>(path01);
                            thismat.SetFloat(ShaderProperties, prefabscale.x * OutLineScale);
                        }
                        else
                        {
                            Debug.LogError("未找到子对象");
                        }
                        }
                    else
                    {
                        //  Debug.LogWarning("没有找到" + MaterialPath[i]+"关联的prefab");
                    }
                }
                //Debug.LogWarning("shader关联材质matGUID:" + matguid);
            }
        }
    }

    public List<string> TexturePath = new List<string>();
    int maxTextureSize ;
    public string texturePathlist;

    //enum PVRTC_RGBA4button = TextureImporterFormat.PVRTC_RGBA4;
    void TextureCheck()
    {
        var listTypes = new List<string>() { ".png", ".tga" };
        string[] TexturePath01 =Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories).Where(s=> listTypes.Contains(Path.GetExtension(s).ToLower())).ToArray();
        
        TexturePath = new List<string>(TexturePath01);

        for (int i = 0; i < TexturePath.Count; i++)
        {
            texturePathlist = TexturePath[i];
            //if (Regex.IsMatch(texturePathlist,Regex.Escape(@"\"))) //win平台路径
            //{
            //    string[] texturePathArray = texturePathlist.Split('/');
            //    string textureAssetPath = texturePathArray[texturePathArray.Length - 1];
            //}
            int skiptexturejudge = 0;
            if (string.IsNullOrEmpty(skipFilesName) == false)
            {
                int skiptexturefile = texturePathlist.IndexOf(skipFilesName, System.StringComparison.Ordinal);
                skiptexturejudge = skiptexturefile;
            }

            if (string.IsNullOrEmpty(skipFilesName) || skiptexturejudge < 0)
            {
                if (string.IsNullOrEmpty(OnlyCheckFilesName))
                {
                    TextureSetting();
                }
                else
                {
                    int onlycheckfile = texturePathlist.IndexOf(OnlyCheckFilesName, System.StringComparison.Ordinal) + 1;
                    if (onlycheckfile != 0)
                    {
                        TextureSetting();
                    }
                }
                skiptexturejudge = 0;
            }


        }
        if (ioscheck == false && androidcheck == false) 
        {
            Debug.LogError("贴图工具平台未选择!");
        }
        Debug.Log("贴图资源刷新完毕!");
    }

    void TextureSetting()
    {

        int index = texturePathlist.IndexOf("Assets", System.StringComparison.Ordinal);
        string textureAssetPath = texturePathlist.Substring(index, texturePathlist.Length - index);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(textureAssetPath);
        Texture2D testuretext = AssetDatabase.LoadAssetAtPath<Texture2D>(textureAssetPath);

        if (importer != null)
        {
            importer.mipmapEnabled = false ;

            if (ioscheck)
            {
                TextureImporterPlatformSettings importTexSetting = new TextureImporterPlatformSettings();
                importTexSetting.name = "iPhone";
                importTexSetting.overridden = true;
                importTexSetting.format = TextureImporterFormat.PVRTC_RGBA4;
                importTexSetting.compressionQuality = 50;


                if (testuretext.width > 256 && testuretext.height > 256)
                {
                    importTexSetting.maxTextureSize = maxTextureSize;
                }
                else
                {
                    importTexSetting.maxTextureSize = testuretext.width;
                }
                importer.SetPlatformTextureSettings(importTexSetting);
            }
            if (androidcheck)
            {
                TextureImporterPlatformSettings importTexSettingAndroid = new TextureImporterPlatformSettings();
                importTexSettingAndroid.name = "Android";
                importTexSettingAndroid.overridden = true;
                importTexSettingAndroid.format = TextureImporterFormat.PVRTC_RGBA4;
                importTexSettingAndroid.compressionQuality = 50;

                if (testuretext.width > 256 && testuretext.height > 256)
                {
                    importTexSettingAndroid.maxTextureSize = maxTextureSize;
                }
                else
                {
                    importTexSettingAndroid.maxTextureSize = testuretext.width;
                }
                importer.SetPlatformTextureSettings(importTexSettingAndroid);
            }

        }
        else
        {
            Debug.LogError("批量刷图出错！,没有找到贴图资源.");
        }
        AssetDatabase.ImportAsset(textureAssetPath, ImportAssetOptions.ForceUpdate);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    //void Write()
    //{
    //    FileStream matTxt = new FileStream(Application.dataPath + "/MateCheckPath(请勿提交).txt", FileMode.Create);

    //    StreamWriter matTxtStreamWriter = new StreamWriter(matTxt);

    //    matTxtStreamWriter.Write("--------------------------" + "\r\n");
    //    matTxtStreamWriter.Write("引用到" + shadername + "的材质: " + "------------" + "共" + Checkshaderpath.Count + "个" + "\r\n");

    //    if (Checkshaderpath.Count != 0)
    //    {
    //        for (int i = 0; i < Checkshaderpath.Count; i++)
    //        {
    //            matTxtStreamWriter.Write(Checkshaderpath[i] + "\r\n");
    //        }
    //    }
    //    else
    //    {
    //        matTxtStreamWriter.Write("没有找到相关引用" + "\r\n");
    //    }

    //    matTxtStreamWriter.Write("--------------------------" + "\r\n");
    //    matTxtStreamWriter.Write("引用到" + shadername + "的prefab: " + "------------" + "共" + PrefabQuotePath.Count + "个" + "\r\n");
    //    if (PrefabQuotePath.Count != 0)
    //    {
    //        for (int i = 0; i < PrefabQuotePath.Count; i++)
    //        {
    //            matTxtStreamWriter.Write(PrefabQuotePath[i] + "\r\n");
    //        }
    //    }


    //    matTxtStreamWriter.Flush();
    //    matTxtStreamWriter.Close();
    //    matTxt.Close();
    //}
}
