using UnityEngine;
using UnityEditor;

public class PlayerPrefsCleaner : EditorWindow
{
    [MenuItem("Tools/Clear PlayerPrefs")]
    public static void ShowWindow()
    {
        GetWindow<PlayerPrefsCleaner>("PlayerPrefs Cleaner");
    }

    void OnGUI()
    {
        GUILayout.Label("PlayerPrefs Cleaner", EditorStyles.boldLabel);

        GUILayout.Space(10);

        if (GUILayout.Button("Clear All PlayerPrefs", GUILayout.Height(30)))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("Tüm PlayerPrefs silindi!");
        }

        GUILayout.Space(10);
        GUILayout.Label("Bu butona basarak tüm PlayerPrefs verilerini silebilirsiniz.", EditorStyles.helpBox);
    }
}