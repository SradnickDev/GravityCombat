using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace SCT
{
	public class ScriptableTextComponent : MonoBehaviour
	{
		public RectTransform SctBox;
		public Text Text;
		public Image Background;
		public LayoutElement BackgroundSize;
		public Image IconLeft;
		public LayoutElement IconLeftSize;
		public Image IconRight;
		public LayoutElement IconRightSize;


		private RectTransform m_rectTransform;
		private float m_amount;          //Amount to Increase FontSize
		private float m_fontSize;        //regular FontSize
		private Vector3 m_startPosition; //Position for this GameObject
		private Vector3 m_currentPosition;
		private AnimationCurve m_animCurveX;
		private AnimationCurve m_animCurveY;
		private AnimationCurve m_fontSizeAnimCurve;
		private float m_animDuration;         //duration from longest AnimationCurve
		private Gradient m_colorGradient;     //Text Color Gradient
		private Outline m_outline;            //Text outline Effect
		private Camera m_cam;                 //Main Camera
		private Vector2 m_animationDirection; // e.g. from 0,0 to 30,100
		private bool m_onScreen = false;      //Fake WorldSpace or ScreenSpace
		private float m_tempValue;
		private ScriptableText m_sct;
		private float m_timer = 0;
		private bool m_stack;
		private Vector3 m_randomOffset;

		private void OnEnable()
		{
			if (Text == null)
				Text = GetComponentInChildren<Text>();
			if (m_outline == null)
				m_outline = GetComponentInChildren<Outline>();
			if (m_rectTransform == null)
				m_rectTransform = GetComponent<RectTransform>();

			this.Text.gameObject.SetActive(false);
			Background.enabled = false;
			IconLeft.enabled = false;
			IconRight.enabled = false;
		}

		/// <summary>
		/// Initialize the Floating Text Component.
		/// </summary>
		/// <param Name="sct">To get alle the Information.</param>
		/// <param Name="pos">Start at this point.</param>
		/// <param Name="sctText">See this on Screen.</param>
		/// <param Name="targetCam">Main Camera, need this to convert World to Screen point</param>
		public ScriptableTextComponent Initialize(ScriptableText sct, Vector3 pos, string sctText, Camera targetCam)
		{
			SetCamera(targetCam);

			SetupOutline(sct);

			SetupText(sct);

			SetupIcon(sct);

			SetupComponent(sct);

			SetPosition(sct, pos);

			SetValue(sct, sctText);

			SetupBackground(sct);

			StartCoroutine(AnimateTextComponent());

			return this;
		}

		void SetCamera(Camera cam)
		{
			m_cam = cam;
		}

		private void SetPosition(ScriptableText sct, Vector3 pos)
		{
			if (m_onScreen)
			{
				this.m_startPosition = sct.ScreenOffset;
			}
			else
			{
				this.m_startPosition = pos + sct.WorldOffset;
			}
		}


		void SetupComponent(ScriptableText sct)
		{
			//Font Size from ScriptableText as ref / start point for Lerp
			this.m_fontSize = sct.FontSize;
			//Amount to increase the Text Size
			this.m_amount = sct.IncreaseAmount + sct.FontSize;
			//set the Text size
			this.Text.fontSize = sct.FontSize;
			//set Color Gradient
			this.m_colorGradient = sct.ColorGradient;
			//set Animation Curve for Fonz size Animation
			this.m_fontSizeAnimCurve = sct.FontSizeAnimation;
			//set animationCurve
			this.m_animCurveX = sct.AnimCurveX;
			this.m_animCurveY = sct.AnimCurveY;

			//set Animation Length
			//---Explanation Ternary if (XCurveTime >  YCurveTime) animDuration = XCurveTime else animDuration = YCurveTime
			this.m_animDuration =
				sct.AnimCurveX.keys[sct.AnimCurveX.length - 1].time >=
				sct.AnimCurveY.keys[sct.AnimCurveY.length - 1].time
					? sct.AnimCurveX.keys[sct.AnimCurveX.length - 1].time
					: sct.AnimCurveY.keys[sct.AnimCurveY.length - 1].time;
			this.m_onScreen = sct.RenderMode == ScriptableText.TextRenderMode.ScreenSpace;
			this.m_stack = sct.StackValues;

			//Override the current animDuration with the Activation Time from the Inspector
			if (m_stack)
			{
				m_sct = sct;
				m_animDuration = sct.ActivationTime;
			}

			m_currentPosition = m_startPosition;
			this.m_animationDirection = sct.AnimationDirection;

			//set Text
			m_tempValue = 0;
		}

		private void SetupIcon(ScriptableText sct)
		{
			if (sct.UseIcon)
			{
				switch (sct.Alignment)
				{
					case ScriptableText.IconAlignment.Left:
						IconLeft.enabled = true;
						IconRight.enabled = false;
						break;
					case ScriptableText.IconAlignment.Right:
						IconLeft.enabled = false;
						IconRight.enabled = true;
						break;
				}

				this.IconLeft.color = sct.IconColor;
				this.IconRight.color = sct.IconColor;

				this.IconLeft.sprite = sct.Icon;
				this.IconRight.sprite = sct.Icon;


				IconRightSize.preferredWidth = sct.IconSize.x;
				IconRightSize.preferredHeight = sct.IconSize.y;

				IconLeftSize.preferredWidth = sct.IconSize.x;
				IconLeftSize.preferredHeight = sct.IconSize.y;
			}
		}

		private void SetupText(ScriptableText sct)
		{
			this.Text.font = sct.Font;
		}

		private void SetupBackground(ScriptableText sct)
		{
			if (sct.UseBackground)
			{
				//Set Anchor,Pivot to center

				Background.sprite = sct.Background;
				Background.color = sct.BackgroundColor;
				BackgroundSize.preferredWidth = sct.BackgroundSize.x;
				BackgroundSize.preferredHeight = sct.BackgroundSize.y;

				SetTextPosition(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
				Background.enabled = true;
			}
			else
			{
				BackgroundSize.preferredWidth = Text.preferredWidth;
				BackgroundSize.preferredHeight = Text.preferredHeight;
			}
		}

		private void SetupOutline(ScriptableText sct)
		{
			if (sct.UseOutline)
			{
				this.m_outline.enabled = sct.UseOutline;
				this.m_outline.effectColor = sct.OutlineColor;
				this.m_outline.effectDistance = sct.OutlineEffectDistance;
			}
			else
			{
				this.m_outline.enabled = sct.UseOutline;
			}
		}

		void SetTextPosition(Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
		{
			Text.rectTransform.anchorMin = anchorMin;
			Text.rectTransform.anchorMax = anchorMax;
			Text.rectTransform.pivot = pivot;
		}

		void SetValue(ScriptableText sct, string value)
		{
			if (sct.StackValues)
			{
				if (float.TryParse(value, out float result) == true)
				{
					m_tempValue += float.Parse(value);
					Text.text = m_tempValue.ToString();
				}
				else
				{
					Debug.LogWarning(
									"ScriptableText : StackingValues, cant stack strings with floats, please check Input Values.");
				}
			}
			else
			{
				Text.text = value;
			}
		}

		public void SetStackValue(ScriptableText sct, string value, Vector3 pos)
		{
			m_timer = 0;
			SetValue(sct, value);
			if (m_onScreen)
			{
				this.m_startPosition = sct.ScreenOffset;
			}
			else
			{
				this.m_startPosition = pos + sct.WorldOffset;
			}
		}

		IEnumerator AnimateTextComponent()
		{
			//Increase timer over Time to Evaluate Animation Curve and Color Gradient
			m_timer = 0;
			this.Text.gameObject.SetActive(true);
			//Time factor for Lerping
			float curveTimerX = 0;
			float curveTimerY = 0;
			float x;
			float y;
			//Time factor forLerpin
			float fontSizeCurveAnim = 0;
			//Animation Length
			float animationTime = m_animDuration;
			//change Icon Alpha with Text Alpahe
			var tempColor = IconLeft.isActiveAndEnabled ? IconLeft.color : IconRight.color;
			var backgroundColorCache = Background.color;
			//as long as timer is not bigger than the Animation Length
			while (m_timer < animationTime)
			{
				//Evaluate  Timer with the Curve
				curveTimerX = m_animCurveX.Evaluate(m_timer);
				curveTimerY = m_animCurveY.Evaluate(m_timer);

				//if amount == 0 no Size Animation
				if (m_amount != 0)
				{
					fontSizeCurveAnim = m_fontSizeAnimCurve.Evaluate(m_timer);
					Text.fontSize = (int) Mathf.Lerp(this.m_fontSize, m_amount, fontSizeCurveAnim);
				}

				this.Text.color = m_colorGradient.Evaluate(m_timer);
				tempColor.a = Text.color.a;
				backgroundColorCache.a = Text.color.a;


				if (IconLeft.isActiveAndEnabled)
				{
					IconLeft.color = tempColor;
				}
				else
				{
					IconRight.color = tempColor;
				}

				if (Background.isActiveAndEnabled)
				{
					//Text.color;
					Background.color = backgroundColorCache;
				}

				//Based on the Transform Z-Axis, disable if less than 0
				bool isInView = m_rectTransform.position.z < 0;
				this.gameObject.SetActive(!isInView);
				if (m_stack && m_onScreen == false)
				{
					//m_startPosition = Vector3.Lerp(m_startPosition, m_currentPosition, m_timer / animationTime);
				}

				//ternary position = If(onScreen ==false)cam.WorldToScreenPoint(startPosition) else startPosition
				this.transform.position =
					m_onScreen == false ? m_cam.WorldToScreenPoint(m_startPosition) : m_startPosition;
				//Lerp the Text GameObject in localSpace
				x = Mathf.Lerp(0, m_animationDirection.x, curveTimerX);
				y = Mathf.Lerp(0, m_animationDirection.y, curveTimerY);
				this.SctBox.localPosition = new Vector3(x, y, 0);

				m_timer += Time.deltaTime;

				yield return null;
			}

			//disable Text Component to avoid weird jump behaviour
			this.Text.gameObject.SetActive(false);
			this.IconRight.enabled = false;
			this.IconLeft.enabled = false;
			Background.enabled = false;
			this.SctBox.localPosition = Vector3.zero;
			//deactivate this Object -> back to Pool
			this.gameObject.SetActive(false);
		}
	}
}