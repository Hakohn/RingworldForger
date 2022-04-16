using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChironPE
{
	[CreateAssetMenu(fileName = "New Selection Table", menuName = "Ringworld Forger/Selection Table")]
	public class SelectionTableSO : ScriptableObject
	{
		#region Private Fields
		[Header("General")]
		[Tooltip("The chance for any item to be selected.")]
		[Range(0, 100)]
		[SerializeField] private float selectionChance = 50f;


		[Header("Items")]
		[Tooltip("The total weight of the items within the selection table. The items " +
				 "that have 100% individual weight will have no impact on this.")]
		[DisableField, SerializeField]
		private float totalWeight = 0;
		[Tooltip("The items that can be dropped from this loot table. The items " +
				 "with 100% individual weight will always be dropped.")]
		[SerializeField]
		private SelectionTableItem[] lootTableItems = null;
		/// <summary> 
		/// The actual loot table. This is the one that should be used in all algorithms. 
		/// The array loot table is there only for the ease of access in the inspector. 
		/// This dictionary is a sorted dictionary, where the keys are the weight, and the value
		/// is the game item. The weights are sorted in ascending order, and thus, when the
		/// loot generation takes place, it should be iterated in reverse.
		/// </summary>
		private SortedDictionary<float, List<SelectionTableItem>> lootTable = null;
		#endregion

		#region Unity Methods
		private void Awake()
		{
			OnValidate();
		}
		private void OnValidate()
		{
			if(lootTableItems != null)
            {
				foreach(SelectionTableItem item in lootTableItems)
                {
					item.OnValidate();
                }
            }

			#region Generating the dictionary
			// Creating it for the first time.
			lootTable = new SortedDictionary<float, List<SelectionTableItem>>();

			// Moving the items from the array to the dictionary accordingly.
			if (lootTableItems != null && lootTableItems.Length > 0)
			{
				foreach (SelectionTableItem item in lootTableItems)
				{
					if (!lootTable.ContainsKey(item.weight))
					{
						lootTable.Add(item.weight, new List<SelectionTableItem>());
					}

					lootTable[item.weight].Add(item);
				}
			}

			// Calculating the total weight, for the ease of access of the user.
			totalWeight = 0;
			if (lootTableItems != null && lootTableItems.Length > 0)
			{
				foreach (float weight in lootTable.Keys)
				{
					if (weight != SelectionTableItem.maxWeight)
					{
						totalWeight += weight;
					}
				}


				// Calculating the actual drop chance, for the ease of access of the user.
				foreach (SelectionTableItem item in lootTableItems)
				{
					if (item.weight != SelectionTableItem.maxWeight)
					{
						// This is the best actualDropChance. Tested it through math calculations, and this one is the most accurate.
						// Not the most pleasing to the eye, as all of them summed up will give you the itemDropChance, but it is
						// what it is.
						item.dropInfo = $"{System.Math.Round(item.weight / totalWeight * 100 * (selectionChance / 100) / lootTableItems.Length, 2)}%";
					}
					else
					{
						item.dropInfo = "Always drops";
					}
				}
			}
			#endregion
		}
		#endregion

		#region Public Methods
		#region Unity Editor Testing
#if UNITY_EDITOR
		//[TitleGroup("Table Testing"), HorizontalGroup("Table Testing/Row1"), Button("Sort by weight")]
		public void SortLootableItems()
		{
			System.Array.Sort(lootTableItems, (e1, e2) => e2.weight.CompareTo(e1.weight));
		}

        //[HorizontalGroup("Table Testing/Row1"), Button("Roll", ButtonStyle.FoldoutButton)]
        //private void LootTableTest(int testTimes = 1)
        //{
        //	for (int i = 0; i < testTimes; i++)
        //	{
        //		if (GenerateLoot(out List<GameItemStack> droppedItems))
        //		{
        //			foreach (GameItemStack itemStack in droppedItems)
        //			{
        //				Debug.LogFormat("{0} x {1}", itemStack.quantity, itemStack.gameItem.ItemName);
        //			}
        //		}
        //		else
        //		{
        //			Debug.LogFormat("No items dropped.");
        //		}
        //	}
        //}
#endif
        #endregion

        public GameObject GenerateLoot()
        {
            // Adding all the 100% drop chance items to the dropped items.
            if (lootTable.ContainsKey(SelectionTableItem.maxWeight))
            {
				return lootTable[SelectionTableItem.maxWeight].RandomElement().item;
            }

			float dropChance = Random.Range(0, 100);
			// If the drop chance is too high, then we'll skip this item.
			if (dropChance <= this.selectionChance)
			{
				// Now, an item will surely be picked. Let's see what item will that be.
				float generatedWeight = Random.Range(0, totalWeight);
				// Reversing the keys, as we need to go from the highest to the lowest.
				foreach (float weight in lootTable.Keys.Reverse())
				{
					if (weight == SelectionTableItem.maxWeight)
					{
						continue;
					}
					// If we were lucky enough to get this item, we'll add it to the dropped
					// items and set the 'anyDrop' variable to true.
					if (generatedWeight <= weight)
					{
						SelectionTableItem selectedLootTableItem = lootTable[weight].RandomElement();
						return selectedLootTableItem.item;
					}
					// Otherwise, we'll decrease the generated weight and look for the
					// next item to pick.
					else
					{
						generatedWeight -= weight;
					}
				}
			}

			return null;
        }
        #endregion
    }

	[System.Serializable]
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