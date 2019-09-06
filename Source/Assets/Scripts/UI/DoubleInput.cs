using UnityEngine;

namespace UI
{
	public class DoubleInput
	{
		private int m_inputCount = 0;
		private float m_inputTime = 0.0f;
		private readonly float m_doubleInputTime;

		public DoubleInput(float timeBetweenInput = 0.3f)
		{
			m_doubleInputTime = timeBetweenInput;
		}

		public void RecordInput()
		{
			m_inputCount++;
			if (m_inputCount == 1)
			{
				m_inputTime = Time.time + m_doubleInputTime;
			}
		}

		public bool RecordedDoubleInput()
		{
			if (m_inputCount >= 2 && Time.time <= m_inputTime)
			{
				m_inputCount = 0;
				m_inputTime = 0;
				return true;
			}

			if (Time.time > m_inputTime)
			{
				m_inputCount = 0;
			}

			return false;
		}
	}
}