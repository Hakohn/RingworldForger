using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChironPE
{
	public abstract class GenerationAlgorithmSO : ScriptableObject
    {
		public abstract float[,] GenerateHeightmap(Vector2Int size);
	}
}
