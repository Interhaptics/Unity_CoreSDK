using UnityEngine;
using System.Collections;
using Interhaptics.Core;

namespace Interhaptics.Utils
{
	/// <summary>
	/// Defines and plays haptic presets inspired by Apple's Human Interface Guidelines.
	/// This class provides a set of predefined haptic patterns to enrich the user experience.
	/// Each pattern represents a unique tactile sensation, such as a light tap or a warning signal.
	/// </summary>
	public static class HapticPreset
	{
		/// <summary>
		/// Enumeration of available haptic presets.
		/// </summary>
		public enum PresetType
		{
			Selection,
			Light,
			Medium,
			Heavy,
			Rigid,
			Soft,
			Success,
			Failure,
			Warning
		}

		// Interleaved time and amplitude values for each preset.
		// These arrays represent the time-amplitude pairs for each haptic effect.
		private static readonly double[] selection = { 0.0, 0.471, 0.04, 0.471 };
		private static readonly double[] light = { 0.0, 0.156, 0.04, 0.156 };
		private static readonly double[] medium = { 0.0, 0.471, 0.08, 0.471 };
		private static readonly double[] heavy = { 0.0, 1.0, 0.16, 1.0 };
		private static readonly double[] rigid = { 0.0, 1.0, 0.04, 1.0 };
		private static readonly double[] soft = { 0.0, 0.156, 0.16, 0.156 };
		private static readonly double[] success = { 0.0, 0.0, 0.04, 0.157, 0.08, 0.0, 0.24, 1.0 };
		private static readonly double[] failure = { 0.0, 0.0, 0.08, 0.47, 0.12, 0.0, 0.2, 0.47, 0.24, 0.0, 0.4, 1.0, 0.44, 0.0, 0.48, 0.157 };
		private static readonly double[] warning = { 0.0, 0.0, 0.12, 1.0, 0.24, 0.0, 0.28, 0.47 };

		/// <summary>
		/// Plays a haptic effect based on the specified preset type.
		/// </summary>
		/// <param name="presetType">Type of the haptic preset to play.</param>
		/// <returns>A coroutine to play the specified haptic effect.</returns>
		public static void Play(PresetType presetType)
		{
			double[] preset = GetPreset(presetType);
			// Initiate the haptic effect based on the retrieved preset pattern
			HAR.Play(preset);
		}

		/// <summary>
		/// Retrieves the preset pattern based on the provided preset type.
		/// </summary>
		/// <param name="presetType">The type of preset to retrieve.</param>
		/// <returns>An array representing the haptic pattern of the specified type.</returns>
		private static double[] GetPreset(PresetType presetType)
		{
			switch (presetType)
			{
				case PresetType.Selection: return selection;
				case PresetType.Light: return light;
				case PresetType.Medium: return medium;
				case PresetType.Heavy: return heavy;
				case PresetType.Rigid: return rigid;
				case PresetType.Soft: return soft;
				case PresetType.Success: return success;
				case PresetType.Failure: return failure;
				case PresetType.Warning: return warning;
				default:
					Debug.LogError("Preset type out of range.");
					return null;
			}
		}
	}
}