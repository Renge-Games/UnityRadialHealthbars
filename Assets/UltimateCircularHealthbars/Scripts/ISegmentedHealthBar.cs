using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RengeGames.HealthBars {

	public interface ISegmentedHealthBar {
		/// <summary>
		/// Set the number of segments in this health bar
		/// </summary>
		/// <param name="value">number of segments</param>
		void SetSegmentCount(float value);
		/// <summary>
		/// Sets the absolute count of removed segments
		/// </summary>
		/// <param name="value">segment count to be set as removed</param>
		void SetRemovedSegments(float value);
		/// <summary>
		/// Sets the absolute percentage of the health bar
		/// </summary>
		/// <param name="value">percentage from 0 (no health) to 1 (max health)</param>
		void SetPercent(float value);
		/// <summary>
		/// Add or remove a certain amount of segments from/to the health bar
		/// This does not alter the segment count
		/// </summary>
		/// <param name="value">Amount of segments to add(+)/remove(-)</param>
		void AddRemoveSegments(float value);
		/// <summary>
		/// Add or remove a certain percent of the health bar
		/// This does not alter the maximum value
		/// </summary>
		/// <param name="value">percentage to add(+)/remove(-) from/to the health bar</param>
		void AddRemovePercent(float value);
		
	}
}