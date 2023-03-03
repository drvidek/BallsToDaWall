using UnityEngine;
using UnityEditor;

public class MeterTool : EditorWindow
{
    [MenuItem("Tools/Meters")]
    public static void ShowWindow()
    {
        thisWindow = GetWindow(typeof(MeterTool), false, "Meters");
        windowRect = thisWindow.position;
    }

    private static Rect windowRect;
    private static EditorWindow thisWindow;

    private Vector2 windowScrollPos;


    private void OnGUI()
    {
        if (thisWindow == null)
            thisWindow = GetWindow(typeof(MeterTool));
        if (windowRect.size != thisWindow.position.size)
        {
            windowRect = thisWindow.position;
        }

        windowScrollPos = EditorGUILayout.BeginScrollView(windowScrollPos, GUILayout.MaxWidth(windowRect.width));

        EditorGUILayout.LabelField("Options",EditorStyles.boldLabel);
        MeterDrawer.colorType = (MeterDrawer.ColorType)EditorGUILayout.EnumPopup("Meter color type",MeterDrawer.colorType);
        switch (MeterDrawer.colorType)
        {
            case MeterDrawer.ColorType.single:
                MeterDrawer.meterCol = EditorGUILayout.ColorField("Meter color", MeterDrawer.meterCol);
                break;
            case MeterDrawer.ColorType.gradient:
                MeterDrawer.meterGrad = EditorGUILayout.GradientField("Meter gradient", MeterDrawer.meterGrad);
                break;
            default:
                Debug.LogError("Invalid color type selection, check MeterDrawer.colorType is valid");
                break;
        }
        MeterDrawer.meterBgCol = EditorGUILayout.ColorField("Meter background color", MeterDrawer.meterBgCol);
        EditorGUILayout.EndScrollView();
    }

}
