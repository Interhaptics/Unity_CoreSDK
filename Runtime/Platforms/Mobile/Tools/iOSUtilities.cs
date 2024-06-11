/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interhaptics.Platforms.Mobile.Tools
{
	public static class iOSUtilities
	{
		const int PARAMETER_CURVE_MAX_SIZE = 16;
		const double D_EPSILON = 0.00001;

		private static bool IsEqual(double _a, double _b, double _epsilon = D_EPSILON)
		{
			return Mathf.Abs((float)(_a - _b)) <= _epsilon;
		}

		// check if the ramp between two values is going up, down or is constant
		private static int GetCurrentRamp(double _a, double _b)
		{
			if (IsEqual(_a, _b)) // constant
			{
				return 0;
			}

			if (_a > _b) // going down
			{
				return -1;
			}
			
			return 1; // going up
		}

		// converts HAR output buffers to simplified data for AHAP construction
		// HAR == [value, value, value....] each value separated by _step seconds
		// out == [time, value, time, value....] with linear interpolation between each pair
        private static List<double> ProcessBuffer(double [] _buffer, double _step)
        {
            List<double> outBuffer = new List<double>();

			double currentTimeStamp = 0.0;
            int lastRamp = -2; //set to -2 to always get the first value

            for (int i = 0; i < _buffer.Count(); i++)
            {
                if (i == _buffer.Count() - 1) //if last value, stores a pair
                {
                    outBuffer.Add(currentTimeStamp);
                    outBuffer.Add(_buffer[i]);
					break;
                }

				int currentRamp = GetCurrentRamp(_buffer[i], _buffer[i+1]); //check if curve is going up or down
				
				// if ramp changes, stores a pair
				if (currentRamp != lastRamp) //will trigger at first iteration
				{
					lastRamp = currentRamp;
                    outBuffer.Add(currentTimeStamp);
                    outBuffer.Add(_buffer[i]);
				}
				
				currentTimeStamp += _step;
            }

            return outBuffer;
        }

		public static string BufferToAHAP(float[] _buffer, float[] _frequencies, float[] _transientTimer, float[] _transientGain, float _duration, float _timestep)
		{
			double[] transient = new double[_transientTimer.Length * 4];

			for (int j = 0; j < transient.Length; j += 4)
			{
				transient[j] = _transientTimer[j / 4];
				transient[j + 1] = _transientGain[j / 4];
				transient[j + 2] = 0;
				transient[j + 3] = 0;
			}

			double[] buffer = new double[_buffer.Length];
			double[] frequencies = new double[_frequencies.Length];

			for (int j = 0; j < _frequencies.Length; j++)
			{
				frequencies[j] = _frequencies[j];
			}
			for (int j = 0; j < _buffer.Length; j++)
			{
				buffer[j] = _buffer[j];
			}
			return BufferToAHAP(buffer, frequencies, transient, _duration, _timestep);
		}

		public static string BufferToAHAP(double[] _buffer, double[] _frequencies, double[] _transients, double _duration, double _timestep)
		{
			if (_buffer == null)
			{
				return null;
			}

			//UnityEngine.Debug.Log(_buffer.Length);

			AHAP ahap = new AHAP();
			ahap.Pattern = new List<PatternObject>();

			// transients ------------------------------------------------------------
			if (_transients != null)
			{
				for (int i = 0; i + 3 < _transients.Length; i += 4)
				{
					EventParameter intensityTrans = new EventParameter
					{
						parameterID = ParameterID.HapticIntensity,
						parameterValue = (float)_transients[i + 1]
					};
					EventParameter sharpnessTrans = new EventParameter
					{
						parameterID = ParameterID.HapticSharpness,
						parameterValue = 1.0f
					};

					Event CurrentTransient = new Event
					{
						Time = (float)_transients[i],
						eventType = EventType.HapticTransient,
						eventParameters = new EventParameter[2] { intensityTrans, sharpnessTrans }
					};
					ahap.Pattern.Add(CurrentTransient);

					//puting the amplitude modulation at 1 for the transients ---
					
					float currentTransStart = CurrentTransient.Time;
					float currentTransEnd = currentTransStart + 0.022f;

					int startIndex = (int)(currentTransStart / _timestep);
					int endIndex = Math.Max((int)(currentTransEnd / _timestep), startIndex + 1);

					int end = Math.Min(endIndex, _buffer.Length);
					for (int index = startIndex; index < end; index++)
					{
						_buffer[index] = (float)_transients[i + 1];
					}
					
				}
			}
			// -----------------------------------------------------------------------

			// Convert Amplitude and Frequency pattern to have less values
			List<double> ampPattern = ProcessBuffer(_buffer, _timestep);
			List<double> freqPattern = ProcessBuffer(_frequencies, _timestep);

			EventParameter intensity = new EventParameter
			{
				parameterID = ParameterID.HapticIntensity,
				parameterValue = 1
			};
			EventParameter sharpness = new EventParameter
			{
				parameterID = ParameterID.HapticSharpness,
				parameterValue = 1
			};

			/*
			// remove amp modulation when there are only transients
			if (ampPattern.Count() == 2 && IsEqual(ampPattern[0], ampPattern[1]))
			{
				intensity.parameterValue = ampPattern[0];
				ampPattern.Clear();
			}

			//remove freq modulation when there are only transients
			if (freqPattern.Count() == 2 && IsEqual(freqPattern[0], freqPattern[1]))
			{
				sharpness.parameterValue = freqPattern[0];
				freqPattern.Clear();
			}*/

			Event e = new Event
			{
				Time = 0,
				eventType = EventType.HapticContinuous,
				eventDuration = (float)_duration,
				eventParameters = new EventParameter[2] { intensity, sharpness }
			};

			ahap.Pattern.Add(e);

			//Amplitude pattern ------------------------------------------------------
			for (int j = 0; j < ampPattern.Count(); j += PARAMETER_CURVE_MAX_SIZE * 2)
			{
				List<ParameterCurveControlPoint> controlPoints = new List<ParameterCurveControlPoint>();
				for (int k = 0; k < Math.Min(ampPattern.Count() - j, PARAMETER_CURVE_MAX_SIZE * 2); k+=2)
				{
					controlPoints.Add(new ParameterCurveControlPoint
					   {
						   Time = ampPattern[k + j] - ampPattern[j],
						   ParameterValue = Mathf.Max(0, (float)ampPattern[k + j + 1])
					   });
				}

				ParameterCurve parameterCurve = new ParameterCurve
				{
					parameterID = ParameterID.HapticIntensityControl,
					Time = ampPattern[j],
					parameterCurveControlPoints = controlPoints
				};
				ahap.Pattern.Add(parameterCurve);
			}
			// -----------------------------------------------------------------------

			// Frequency pattern ------------------------------------------------------
			for (int j = 0; j < freqPattern.Count(); j += PARAMETER_CURVE_MAX_SIZE * 2)
			{
				List<ParameterCurveControlPoint> controlPoints = new List<ParameterCurveControlPoint>();
                for (int k = 0; k < Math.Min(freqPattern.Count() - j, PARAMETER_CURVE_MAX_SIZE * 2); k+=2)
				{
					controlPoints.Add(new ParameterCurveControlPoint
					   {
						   Time = freqPattern[k + j] - freqPattern[j],
						   ParameterValue = Mathf.Max(0, (Mathf.Clamp((float)freqPattern[k + j + 1], 65.0f, 300.0f) - 300.0f) / 235.0f + 1) - 1
					   });
				}

				ParameterCurve parameterCurve = new ParameterCurve
				{
					parameterID = ParameterID.HapticSharpnessControl,
					Time = freqPattern[j],
					parameterCurveControlPoints = controlPoints
				};
				ahap.Pattern.Add(parameterCurve);
			}
			// -----------------------------------------------------------------------
			//Debug.Log(ahap.ToJson());
			return ahap.ToJson();
		}
	}

	public class AHAP
	{
		public int Version = 1;
		public MetaData Metadata = new MetaData();
		public List<PatternObject> Pattern;

		public string ToJson()
		{
			string json = "{" +
					 "\"Version\": " + Version + ","
					 + Metadata.ToJson();

			json += "\"Pattern\": [";
			json += string.Join(",", Pattern.Select(e => e.ToJson()));
			json += "]}";
			return json;
		}
	}

	public class MetaData
	{
		public string Project = "";
		public string Created = "";

		public string ToJson()
		{
			return "\"Metadata\": {" +
						 "\"Project\": \"" + Project + "\"," +
						 "\"Created\": \"" + Created + "\"" +
						"},";
		}
	}


	public abstract class PatternObject
	{
		public abstract string ToJson();
	}

	public class Event : PatternObject
	{
		public override string ToJson()
		{
			string json = "";
			string eventParametersStr = "\"EventParameters\": [";

			if (eventParameters.Length > 0)
			{
				for (int i = 0; i < eventParameters.Length; i++)
				{
					if (i > 0)
					{
						eventParametersStr += ", ";
					}
					eventParametersStr += "{" + "" +
											"\"ParameterID\": \"" + eventParameters[i].parameterID.ToString() + "\"," +
											"\"ParameterValue\": " + eventParameters[i].parameterValue +
										  "}";
				}
			}
			eventParametersStr += "]";

			json += "{" +
						"\"Event\": {" +
							"\"Time\": " + Time + "," +
							"\"EventType\": \"" + eventType.ToString() + "\"," +
							"\"EventDuration\": " + eventDuration + "," +
							eventParametersStr +
						"}" +
					"}";

			return json;
		}

		public float Time = 0;
		public EventType eventType = EventType.HapticContinuous;
		public float eventDuration = 0;
		public EventParameter[] eventParameters;

	}

	public class ParameterCurve : PatternObject
	{

		public override string ToJson()
		{
			string json = "";

			string pccp = "\"ParameterCurveControlPoints\": [";

			if (parameterCurveControlPoints.Count > 0)
			{
				for (int i = 0; i < parameterCurveControlPoints.Count; i++)
				{
					if (i > 0)
					{
						pccp += ", ";
					}

					pccp += "{" + "" +
								"\"Time\": " + parameterCurveControlPoints[i].Time + "," +
								"\"ParameterValue\": " + parameterCurveControlPoints[i].ParameterValue +
							"}";
				}
			}
			pccp += "]";

			json += "{" +
						"\"ParameterCurve\": {" +
							"\"ParameterID\": \"" + parameterID.ToString() + "\"," +
							"\"Time\": " + Time + "," +
							pccp +
						"}" +
					"}";

			return json;
		}

		public ParameterID parameterID = ParameterID.HapticIntensityControl;
		public double Time = 0;
		public List<ParameterCurveControlPoint> parameterCurveControlPoints;

	}

	public enum EventType
	{
		HapticContinuous,
		HapticTransient
	}

	public enum ParameterID
	{
		HapticIntensity,
		HapticIntensityControl,
		HapticSharpness,
		HapticSharpnessControl
	}

	public struct EventParameter
	{
		public ParameterID parameterID;
		public double parameterValue;
	}

	public struct ParameterCurveControlPoint
	{
		public double Time;
		public double ParameterValue;
	}

}