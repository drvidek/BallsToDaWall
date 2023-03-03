using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(Meter))]
public class MeterDrawer : PropertyDrawer
{
    private float lineH = EditorGUIUtility.singleLineHeight;
    private float lineBreak = EditorGUIUtility.singleLineHeight + 4;
    private float lineCount = 3.5f;

    public enum ColorType
    {
        single,
        gradient
    }


    public static ColorType colorType;
    public static Color meterCol = Color.cyan, meterBgCol = new Color(0.7f,0.7f,0.7f,0.7f);
    public static Gradient meterGrad = new Gradient();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var val = property.FindPropertyRelative("_value");
        var min = property.FindPropertyRelative("_min");
        var max = property.FindPropertyRelative("_max");
        var up = property.FindPropertyRelative("_rateUp");
        var down = property.FindPropertyRelative("_rateDown");

        float defaultLabelWidth = EditorGUIUtility.labelWidth;

        float percent = Mathf.Clamp01((val.floatValue - min.floatValue) / (max.floatValue - min.floatValue));

        Rect bg = position;
        bg.height -= 2f;

        GUI.Box(bg, GUIContent.none);

        //LabelWidth = EditorGUI.IntSlider(position, LabelWidth, 0, 40);


        int line = 0;
        int extraSpace = 0;
        float x = position.x;
        float y = position.y;
        float w = position.width;
        float valwidth = 40f;

        Rect nameRect = new Rect(position.x, position.y + (lineBreak * line), position.width, lineH);
        line++;
        extraSpace += 6;
        Rect bgRect = new Rect(x, y + (lineBreak * line), w, lineH + extraSpace);
        Rect fillRect = new Rect(x, y + (lineBreak * line), w * percent, lineH + extraSpace);

        Rect minRect = new Rect(x + 2, y + (lineBreak * line) + extraSpace / 2, valwidth, lineH);
        Rect valRect = new Rect(x + w / 2 - valwidth / 2, y + (lineBreak * line) + extraSpace / 2, valwidth, lineH);
        Rect maxRect = new Rect(x - 2 + w - valwidth, y + (lineBreak * line) + extraSpace / 2, valwidth, lineH);
        line++;
        
        Rect minLabelRect = new Rect(x + 2, y + (lineBreak * line) + extraSpace / 2, valwidth, lineH);
        Rect valLabelRect = new Rect(x + w / 2 - valwidth / 2, y + (lineBreak * line) + extraSpace / 2, valwidth, lineH);
        Rect maxLabelRect = new Rect(x + w - 2 - valwidth, y + (lineBreak * line) + extraSpace / 2, valwidth, lineH);
        valwidth *= 1.25f;

        float arrowOffset = Mathf.Min(valwidth, w / 6);
        
        Rect downRect = new Rect(x + w / 2 - arrowOffset * 2f, y + (lineBreak * line) + extraSpace, valwidth, lineH);
        Rect upRect = new Rect(x + w / 2 + arrowOffset, y + (lineBreak * line) + extraSpace, valwidth, lineH);

        EditorGUI.DrawRect(bgRect, meterBgCol);
        Color fillCol = colorType == ColorType.single ? meterCol : meterGrad.Evaluate(percent);
        EditorGUI.DrawRect(fillRect, fillCol);
        EditorGUI.DrawRect(valRect, new Color(.3f, .3f, .3f, .8f));
        EditorGUI.DrawRect(minRect, new Color(.3f, .3f, .3f, .8f));
        EditorGUI.DrawRect(maxRect, new Color(.3f, .3f, .3f, .8f));

        EditorGUI.LabelField(nameRect, property.displayName);

        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        EditorGUI.BeginChangeCheck();

        float oldMin = min.floatValue;
        float oldMax = max.floatValue;

        min.floatValue = EditorGUI.FloatField(minRect, min.floatValue, style);
        val.floatValue = EditorGUI.FloatField(valRect, val.floatValue, style);
        max.floatValue = EditorGUI.FloatField(maxRect, max.floatValue, style);

        EditorGUIUtility.labelWidth = 32f;

        style.fontStyle = FontStyle.Normal;
        style.alignment = TextAnchor.MiddleCenter;
        EditorGUI.LabelField(minLabelRect, "Min", style);
        EditorGUI.LabelField(valLabelRect, "Current", style);
        EditorGUI.LabelField(maxLabelRect, "Max", style);

        EditorGUIUtility.labelWidth = 25;
        
        GUIContent lbl = new GUIContent();
        lbl.text = "x ->";
        Rect upLabelRect = upRect;
        upLabelRect.width /= 2;
        EditorGUI.PropertyField(upLabelRect, up, GUIContent.none);
        upLabelRect.x += upLabelRect.width;
        EditorGUI.LabelField(upLabelRect, lbl);

        lbl.text = "<- x";
        Rect downLabelRect = downRect;
        downLabelRect.width /= 2;
        EditorGUI.LabelField(downLabelRect, lbl);
        downLabelRect.x += downLabelRect.width;
        EditorGUI.PropertyField(downLabelRect, down, GUIContent.none);

        if (EditorGUI.EndChangeCheck())
        {
            if (min.floatValue != oldMin && min.floatValue > max.floatValue)
                min.floatValue = max.floatValue;
            if (max.floatValue != oldMax && max.floatValue < min.floatValue)
                max.floatValue = min.floatValue;
        }

        EditorGUIUtility.labelWidth = defaultLabelWidth;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return lineBreak * lineCount;
    }
}
