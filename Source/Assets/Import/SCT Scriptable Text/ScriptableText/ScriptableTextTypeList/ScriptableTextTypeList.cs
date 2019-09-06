using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCT
{
    [System.Serializable]
    public class ScriptableText
    {
        public string TextTypeName;
        public bool UseIcon;
        public enum IconAlignment { Left = 0, Right = 1}

        public IconAlignment Alignment = IconAlignment.Left;
        public Sprite Icon;
        public Color IconColor;
        public Vector2 IconSize = new Vector3(50, 50);

        public bool UseBackground = false;
        public Sprite Background;
        public Vector2 BackgroundSize = new Vector2(100,100);
        public Color BackgroundColor = Color.white;

        [Tooltip("Offset From Start(Spawn) Position")]
        public Vector2 Offset;

        public Vector3 WorldOffset
        {
            get
            {
                float x = Random.Range(Min.x, Max.x);
                float y = Random.Range(Min.y, Max.y);
                return new Vector3(Offset.x, Offset.y, 0) + new Vector3(x, y, 0);
            }
        }

        [Tooltip("Radndom.Range(min,max)")]
        public Vector2 Min;

        [Tooltip("Radndom.Range(min,max)")]
        public Vector2 Max;

        public enum TextRenderMode { ScreenSpace = 0, WorldSpace = 1 }
        public TextRenderMode RenderMode = TextRenderMode.WorldSpace;

        [Tooltip("Overwrite the StartPosition.")]
        public Vector2 StartPos;
        public Vector3 ScreenOffset
        {
            get
            {
                float x = Random.Range(Min.x,Max.x);
                float y = Random.Range(Min.y, Max.y);
                return new Vector3(Screen.width * (StartPos.x + x), Screen.height * (StartPos.y + y), 0);
            }
        }

        [Tooltip("This is not converted to Screenspace just added to the SpawnPosition.")]
        public Vector2 AnimationDirection = new Vector2(0,0);
        public AnimationCurve AnimCurveX = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve AnimCurveY = AnimationCurve.Linear(0, 0, 1, 1);
        public bool StackValues;
        public float ActivationTime;
        public Font Font;
        public int FontSize;

        [Tooltip("Is used to Increase the FontSize over Time with the AnimationCurve.Tip: 0 means no Animation")]
        public int IncreaseAmount;

        [Tooltip("Use this to Animate the size of the Font.")]
        public AnimationCurve FontSizeAnimation = AnimationCurve.Linear(0, 0, 1, 1);
        public float FontAnimLength;


        [Tooltip("Change smoothly between Colors.")]
        public Gradient ColorGradient;

        [Tooltip("Check this for Text Outline Effect.ColorGradient with Alpha dont work right if true.")]
        public bool UseOutline = false;

        [Tooltip("Text Outline Color.")]
        public Color OutlineColor = Color.black;

        [Tooltip("Size of the Outline Effect.")]
        public Vector2 OutlineEffectDistance = new Vector2(1, -1);

    }


    [CreateAssetMenu(menuName = "ScriptableTextList/newList")]
    public class ScriptableTextTypeList : ScriptableObject
    {
        public List<ScriptableText> ScriptableTextTyps = new List<ScriptableText>();

        public int ListSize
        {
            get { return ScriptableTextTyps.Count; }
        }

        public string GetName(int index)
        {
            return ScriptableTextTyps[index].TextTypeName;
        }

    }

}