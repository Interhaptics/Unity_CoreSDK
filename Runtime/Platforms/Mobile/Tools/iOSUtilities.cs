/* ​
* Copyright (c) 2023 Go Touch VR SAS. All rights reserved. ​
* ​
*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interhaptics.Platforms.Mobile.Tools
{
    public static class iOSUtilities
    {
        const int PARAMETER_CURVE_MAX_SIZE = 16;

        public static string BufferToAHAP(float[] _buffer, float[] _frequencies, float[] _transientTimer, float[] _transientGain, float _duration, float _timestep)
        {
            double[] transient = new double[_transientTimer.Length * 4];

            for (int j = 0; j < transient.Length; j += 4) 
            {
                transient[j] = _transientTimer[j/4];
                transient[j + 1] = _transientGain[j/4];
                transient[j + 2] = 0;
                transient[j + 3] = 0;
            }

            double[] buffer = new double[_buffer.Length];
            double[] frequencies = new double[_frequencies.Length];

            for (int j = 0; j < _frequencies.Length;j++)
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

            AHAP ahap = new AHAP();
            ahap.Pattern = new List<PatternObject>();

            EventParameter intensity = new EventParameter
            {
                parameterID = ParameterID.HapticIntensity,
                parameterValue = 1
            };
            EventParameter sharpness = new EventParameter
            {
                parameterID = ParameterID.HapticSharpness,
                parameterValue = 0
            };
            Event e = new Event
            {
                Time = 0,
                eventType = EventType.HapticContinuous,
                eventDuration = (float)_duration,
                eventParameters = new EventParameter[2] { intensity, sharpness }
            };

            ahap.Pattern.Add(e);

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
                        parameterValue = 0.5f
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
                    float currentTransEnd = currentTransStart + 0.01f;

                    int startIndex = (int)(currentTransStart / _timestep);
                    int endIndex = Math.Min((int)(currentTransEnd / _timestep), startIndex + 1);

                    int end = Math.Min(endIndex, _buffer.Length);
                    for (int index = Math.Max(startIndex,0); index < end; index++)
                    {
                        _buffer[index] = 1;
                    }
                }
            }
            // -----------------------------------------------------------------------

            //Amplitude pattern ------------------------------------------------------
            for (int j = 0; j < _buffer.Length; j += PARAMETER_CURVE_MAX_SIZE)
            {
                List<ParameterCurveControlPoint> controlPoints = _buffer.Skip(j)
                       .Take(Math.Min(_buffer.Length - j, PARAMETER_CURVE_MAX_SIZE))
                       .Select((Value, Index) => new ParameterCurveControlPoint
                       {
                           Time = (j + Index) * _timestep,
                           ParameterValue = Mathf.Max(0, (float)Value)
                           //ParameterValue = 1
                       }).ToList();

                ParameterCurve parameterCurve = new ParameterCurve
                {
                    parameterID = ParameterID.HapticIntensityControl,
                    Time = j * _timestep,
                    parameterCurveControlPoints = controlPoints
                };
                ahap.Pattern.Add(parameterCurve);
            }
            // -----------------------------------------------------------------------

            // Frequency pattern ------------------------------------------------------

            for (int j = 0; j < _buffer.Length; j += PARAMETER_CURVE_MAX_SIZE)
            {
                List<ParameterCurveControlPoint> controlPoints = _frequencies.Skip(j)
                       .Take(Math.Min(_frequencies.Length - j, PARAMETER_CURVE_MAX_SIZE))
                       .Select((Value, Index) => new ParameterCurveControlPoint
                       {
                           Time = (j + Index) * _timestep,
                           ParameterValue = Mathf.Max(0, (float)Value)
                       }).ToList();

                ParameterCurve parameterCurve = new ParameterCurve
                {
                    parameterID = ParameterID.HapticSharpnessControl,
                    Time = j * _timestep,
                    parameterCurveControlPoints = controlPoints
                };
                ahap.Pattern.Add(parameterCurve);
            }
            // -----------------------------------------------------------------------

            return ahap.ToJson();
        }
    }

    public class AHAP
    {
        public int Version = 1;
        public MetaData Metadata = new MetaData() ;
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
                         "\"Project\": \""+Project+"\"," +
                         "\"Created\": \""+Created+"\"" +
                        "},";
        }
    }


    public abstract class PatternObject
    {
        public abstract string ToJson();
    }

    public class Event : PatternObject
    {
        public override string  ToJson()
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