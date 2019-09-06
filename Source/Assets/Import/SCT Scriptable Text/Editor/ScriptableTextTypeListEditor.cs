#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SCT
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ScriptableTextTypeList))]
    public class ScriptableTextTypeListEditor : Editor
    {
        private ScriptableTextTypeList m_t;
        private SerializedObject m_target;
        private SerializedProperty m_typeList;
        private int m_listSize;
        private List<bool> m_toggled = new List<bool>(); // Folded or not

        private float m_xDuration;
        private float m_yDuration;

        private float m_cachedMin;
        private float m_cachedMax;

        private bool m_iconGroupFolded = false;
        private bool m_backgroundGroupFolded = false;
        private bool m_renderGroupFolded = false;
        private bool m_optionGroupFolded = false;
        private bool m_animatoinGroupFolded = false;
        private bool m_fontGroupFolded = false;
        

        void OnEnable()
        {
            m_t = (ScriptableTextTypeList) target;
            m_target = new SerializedObject(m_t);
            m_typeList = m_target.FindProperty("ScriptableTextTyps");
        }

        public override void OnInspectorGUI()
        {
            m_target.Update();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUIStyle header = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                alignment = (TextAnchor) TextAlignment.Center
            };
            EditorGUILayout.LabelField("TextType List", header);

            if (GUILayout.Button("Add New"))
            {
                m_t.ScriptableTextTyps.Add(new ScriptableText());
            }

            EditorGUILayout.Space();


            for (int i = 0; i < m_typeList.arraySize; i++)
            {
                m_toggled.Add(false); // Initialliazed as false
                SerializedProperty listReference = m_typeList.GetArrayElementAtIndex(i);

                SerializedProperty nameProp = listReference.FindPropertyRelative("TextTypeName");

                SerializedProperty iconBoolProp = listReference.FindPropertyRelative("UseIcon");
                SerializedProperty iconAlignmentProp = listReference.FindPropertyRelative("Alignment");
                SerializedProperty iconProp = listReference.FindPropertyRelative("Icon");
                SerializedProperty iconColorProp = listReference.FindPropertyRelative("IconColor");
                SerializedProperty iconSizeProp = listReference.FindPropertyRelative("IconSize");

                SerializedProperty useBackgroundProp = listReference.FindPropertyRelative("UseBackground");
                SerializedProperty backgroundrefProp = listReference.FindPropertyRelative("Background");
                SerializedProperty backgroundSizeProp = listReference.FindPropertyRelative("BackgroundSize");
                SerializedProperty backgroundColorProp = listReference.FindPropertyRelative("BackgroundColor");


                SerializedProperty offsetProp = listReference.FindPropertyRelative("Offset");
                SerializedProperty minProp = listReference.FindPropertyRelative("Min");
                SerializedProperty maxProp = listReference.FindPropertyRelative("Max");

                SerializedProperty renderModeProp = listReference.FindPropertyRelative("RenderMode");

                SerializedProperty startPosProp = listReference.FindPropertyRelative("StartPos");

                SerializedProperty animDirectionProp = listReference.FindPropertyRelative("AnimationDirection");

                SerializedProperty animCurveXProp = listReference.FindPropertyRelative("AnimCurveX");
                SerializedProperty animCurveYProp = listReference.FindPropertyRelative("AnimCurveY");


                SerializedProperty stackValuesProp = listReference.FindPropertyRelative("StackValues");
                SerializedProperty activationTimeProp = listReference.FindPropertyRelative("ActivationTime");

                SerializedProperty fontProp = listReference.FindPropertyRelative("Font");

                SerializedProperty fontSizeProp = listReference.FindPropertyRelative("FontSize");

                SerializedProperty increaseProp = listReference.FindPropertyRelative("IncreaseAmount");
                SerializedProperty fontCurveProp = listReference.FindPropertyRelative("FontSizeAnimation");

                SerializedProperty gradientProp = listReference.FindPropertyRelative("ColorGradient");
                SerializedProperty outlineBoolProp = listReference.FindPropertyRelative("UseOutline");
                SerializedProperty outlineProp = listReference.FindPropertyRelative("UseOutline");

                SerializedProperty outlineColorProp = listReference.FindPropertyRelative("OutlineColor");
                SerializedProperty outlineSizeProp = listReference.FindPropertyRelative("OutlineEffectDistance");
                string label = nameProp.stringValue != ""
                    ? "[" + i + "] : " + nameProp.stringValue
                    : "[" + i + "]  No Name";


                m_toggled[i] = EditorGUILayout.Foldout(m_toggled[i], label); // Index Foldout

                if (m_toggled[i] == true)
                {
                    nameProp.stringValue = EditorGUILayout.TextField("Text Type Name", nameProp.stringValue);

                    EditorGUILayout.Space();
                    m_iconGroupFolded = EditorGUILayout.Foldout(m_iconGroupFolded, "Icon Settings");

                    if (m_iconGroupFolded)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.Space();
                        EditorGUILayout.PropertyField(iconBoolProp);
                        EditorGUILayout.PropertyField(iconAlignmentProp);
                        EditorGUILayout.PropertyField(iconColorProp);
                        EditorGUILayout.PropertyField(iconProp);
                        EditorGUILayout.PropertyField(iconSizeProp);
                        EditorGUILayout.Space();
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.Space();

                    m_backgroundGroupFolded = EditorGUILayout.Foldout(m_backgroundGroupFolded, "Background Settings");
                    
                    if (m_backgroundGroupFolded)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.PropertyField(useBackgroundProp);
                        EditorGUILayout.Space();
                        EditorGUILayout.PropertyField(backgroundrefProp);
                        EditorGUILayout.PropertyField(backgroundSizeProp);
                        EditorGUILayout.PropertyField(backgroundColorProp);
                        EditorGUILayout.Space();
                        EditorGUILayout.EndVertical();
                    }


                    EditorGUILayout.Space();

                    m_renderGroupFolded = EditorGUILayout.Foldout(m_renderGroupFolded, "Render Settings");

                    if (m_renderGroupFolded)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.PropertyField(renderModeProp);
                        EditorGUILayout.Space();
                        var t = (ScriptableText.TextRenderMode) renderModeProp.enumValueIndex;

                        switch (t)
                        {
                            case ScriptableText.TextRenderMode.ScreenSpace:
                                var pos = EditorGUILayout.Vector2Field("Start Position", startPosProp.vector2Value);
                                pos.x = Mathf.Clamp(pos.x, 0.0f, 1.0f);
                                pos.y = Mathf.Clamp(pos.y, 0.0f, 1.0f);
                                startPosProp.vector2Value = pos;

                                EditorGUILayout.LabelField("Randomize", header);
                                var min = EditorGUILayout.Vector2Field("min", minProp.vector2Value);
                                min.x = Mathf.Clamp(min.x, -1.0f, 1.0f);
                                min.y = Mathf.Clamp(min.y, -1.0f, 1.0f);
                                minProp.vector2Value = min;

                                var max = EditorGUILayout.Vector2Field("max", maxProp.vector2Value);
                                max.x = Mathf.Clamp(max.x, -1.0f, 1.0f);
                                max.y = Mathf.Clamp(max.y, -1.0f, 1.0f);

                                maxProp.vector2Value = max;
                                break;

                            case ScriptableText.TextRenderMode.WorldSpace:

                                offsetProp.vector2Value =
                                    EditorGUILayout.Vector2Field("Offset", offsetProp.vector2Value);

                                EditorGUILayout.LabelField("Randomize", header);
                                minProp.vector2Value = EditorGUILayout.Vector2Field("min", minProp.vector2Value);
                                maxProp.vector2Value = EditorGUILayout.Vector2Field("max", maxProp.vector2Value);
                                break;
                        }

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.Space();

                    m_optionGroupFolded  = EditorGUILayout.Foldout(m_optionGroupFolded, "Options");
                    if (m_optionGroupFolded)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.PropertyField(stackValuesProp);
                        if (stackValuesProp.boolValue)
                        {
                            EditorGUILayout.PropertyField(activationTimeProp);
                            EditorGUILayout.HelpBox("Overrides the Animation Time.", MessageType.Info);
                        }
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.Space();
                    m_animatoinGroupFolded = EditorGUILayout.Foldout(m_animatoinGroupFolded, "Animation");
                    if (m_animatoinGroupFolded)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        animDirectionProp.vector2Value =
                            EditorGUILayout.Vector2Field("Animation Direction", animDirectionProp.vector2Value);

                        EditorGUILayout.PropertyField(animCurveXProp);
                        m_xDuration =
                            animCurveXProp.animationCurveValue.keys[animCurveXProp.animationCurveValue.keys.Length - 1]
                                .time != 0
                                ? animCurveXProp.animationCurveValue
                                    .keys[animCurveXProp.animationCurveValue.keys.Length - 1].time
                                : 0;
                        EditorGUILayout.LabelField("Duration: " + m_xDuration);
                        EditorGUILayout.PropertyField(animCurveYProp);
                        m_yDuration =
                            animCurveYProp.animationCurveValue.keys[animCurveYProp.animationCurveValue.keys.Length - 1]
                                .time != 0
                                ? animCurveYProp.animationCurveValue
                                    .keys[animCurveYProp.animationCurveValue.keys.Length - 1].time
                                : 0;
                        EditorGUILayout.LabelField("Duration: " + m_yDuration);
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.Space();

                    #region Font
                    
                    m_fontGroupFolded = EditorGUILayout.Foldout(m_fontGroupFolded,"Font");

                    if (m_fontGroupFolded)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.LabelField("Font", header);
                        EditorGUILayout.PropertyField(fontProp);
                        EditorGUILayout.PropertyField(fontSizeProp);

                        EditorGUILayout.PropertyField(increaseProp);
                        EditorGUILayout.PropertyField(fontCurveProp);
                        EditorGUILayout.LabelField("Duration : " +
                                                   fontCurveProp
                                                       .animationCurveValue[
                                                           fontCurveProp.animationCurveValue.length - 1]
                                                       .time);
                        EditorGUILayout.PropertyField(gradientProp);


                        EditorGUILayout.LabelField("Outline", header);
                        EditorGUILayout.PropertyField(outlineBoolProp);
                        if (outlineProp.boolValue)
                        {
                            EditorGUILayout.PropertyField(outlineColorProp);
                            EditorGUILayout.PropertyField(outlineSizeProp);
                        }
                        EditorGUILayout.EndVertical();
                    }
                    #endregion

                    EditorGUILayout.Space();

                    if (GUILayout.Button("Remove This Text Type"))
                    {
                        m_typeList.DeleteArrayElementAtIndex(i);
                    }

                    EditorGUILayout.Space();

                }
            }

            //Apply the changes to our list
            m_target.ApplyModifiedProperties();
        }
    }
}
#endif