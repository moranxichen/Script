using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;




public class CheckQuote
{
    public static List<string> allFiles;

    [MenuItem("Assets/检查引用状态",false,2)]
    
    static void Quote()
    {
        string[] selectobj = Selection.assetGUIDs;
        //string selectname = Selection.activeObject.name;
        //Debug.Log(selectname);

        allFiles = new List<string>(Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories));

        int QuoteQuantity = 0;
        for (int i = 0; i < allFiles.Count; i++)
        {

            string filealltext = allFiles[i]; 
            if (Regex.IsMatch(File.ReadAllText(filealltext), selectobj[0]))
            {
                string[] filesArray = allFiles[i].Split('/');
                string filename = filesArray[filesArray.Length - 1];
                if (Regex.IsMatch(filename,".meta"))
                {
                    continue;
                }
                else
                {
                    Debug.Log(filename + "引用了所选文件");
                    QuoteQuantity += 1;
                }

            }
        }
        if (QuoteQuantity == 0 )
        {
            Debug.Log("没有找到相关引用!");
        }
    }
}
