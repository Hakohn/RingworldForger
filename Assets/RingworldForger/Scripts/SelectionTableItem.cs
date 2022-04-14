using System;
using UnityEngine;

namespace ChironPE
{
	[Serializable]
	public class SelectionTableItem
	{
		public const float minWeight = 0.1f;
		public const float maxWeight = 100.0f;

		[HideInInspector]
		public string name = "";

		// Item
		[Tooltip("The item linked with this loot.")]
		public GameObject item = null;

		// Drop chance
		[Tooltip("The individual weight of the item. Long story short: the item " +
				 "drop chance, bypassing influences.")]
		[Range(minWeight, maxWeight)]
		public float weight = 100f;

		#region Ease of Access
		[Tooltip("The actual drop chance of the item per drop, influenced by all " +
				 "(excluding the drop count and 100% drop chance items) the other " +
				 "elements within the loot table.")]
		[DisableField]
		public string dropInfo = "Unknown";
		#endregion

		public void OnValidate()
        {
			name = item != null ? item.name : "";
		}
	}
}
