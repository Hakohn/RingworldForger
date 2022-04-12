using UnityEngine;

namespace ChironPE
{
    [CreateAssetMenu(menuName = "Ringworld Forger/Generation Algorithms/Diamond Square")]
	public class DiamondSquareGASO : GenerationAlgorithmSO
	{
		public float heightMin = 0;
		public float heightMax = 1;

		public override float[,] GenerateHeightmap(Vector2Int size)
		{
			float h1, h2, h3, h4, aver, h;
			h1 = Random.Range(heightMin, heightMax);
			h2 = Random.Range(heightMin, heightMax);
			h3 = Random.Range(heightMin, heightMax);
			h4 = Random.Range(heightMin, heightMax);

			// Initialize terrain - set values in the height map to 0
			float[,] heights = new float[size.x, size.y];
			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					heights[x, y] = 0;
				}
			}

			heights[0, 0] = h1;
			heights[size.x - 1, 0] = h2;
			heights[size.x - 1, size.y - 1] = h3;
			heights[0, size.y - 1] = h4;
			int step_size, tt, H, count;
			float rand_max;
			tt = /**/size.y;
			step_size = tt - 1;
			H = 1;
			rand_max = 1.0f;

			while (step_size > 1)
			{
				for (int x = 0; x < size.x - 1; x += step_size)
				{
					for (int y = 0; y < size.y - 1; y += step_size)
					{
						// diamond_step(x, y, step size)
						h1 = heights[x, y];
						h2 = heights[x + step_size, y];
						h3 = heights[x, y + step_size];
						h4 = heights[x + step_size, y + step_size];
						aver = (h1 + h2 + h3 + h4) / 4.0f;
						h = Random.Range(heightMin, heightMax);
						aver += h * rand_max;
						heights[x + step_size / 2, y + step_size / 2] = aver;
					}
				}

				for (int x = 0; x < size.x - 1; x += step_size)
				{
					for (int y = 0; y < size.y - 1; y += step_size)
					{
						// square_step(x, y)
						count = 0;
						h1 = heights[x, y]; count++;
						h2 = heights[x, y + step_size]; count++; //below
						if ((x - step_size / 2) >= 0) { h3 = heights[x - step_size / 2, y + step_size / 2]; count++; }
						else { h3 = 0f; }
						if ((x + step_size / 2) < /**/size.x) { h4 = heights[x + step_size / 2, y + step_size / 2]; count++; }
						else { h4 = 0f; }
						aver = (h1 + h2 + h3 + h4) / count;
						h = Random.Range(heightMin, heightMax);
						aver += h * rand_max;
						heights[x, y + step_size / 2] = aver;

						//second one
						count = 0;
						h1 = heights[x, y]; count++;
						h2 = heights[x + step_size, y]; count++; //below
						if ((y - step_size / 2) >= 0) { h3 = heights[x + step_size / 2, y - step_size / 2]; count++; }
						else { h3 = 0f; }
						if ((y + step_size / 2) < /**/size.y) { h4 = heights[x + step_size / 2, y + step_size / 2]; count++; }
						else { h4 = 0f; }
						aver = (h1 + h2 + h3 + h4) / count;
						h = Random.Range(heightMin, heightMax);
						aver += h * rand_max;
						heights[x + step_size / 2, y] = aver;

						//third one
						count = 0;
						h1 = heights[x + step_size, y]; count++;
						h2 = heights[x + step_size, y + step_size]; count++; //below
						h3 = heights[x + step_size / 2, y + step_size / 2]; count++;
						if ((x + 3 * step_size / 2) < /**/size.x) { h4 = heights[x + 3 * step_size / 2, y + step_size / 2]; count++; }
						else { h4 = 0f; }
						aver = (h1 + h2 + h3 + h4) / count;
						h = Random.Range(heightMin, heightMax);
						aver += h * rand_max;
						heights[x + step_size, y + step_size / 2] = aver;

						//fourth one
						count = 0;
						h1 = heights[x, y + step_size]; count++;
						h2 = heights[x + step_size, y + step_size]; count++; //below
						h3 = heights[x + step_size / 2, y + step_size / 2]; count++;
						if ((y + 3 * step_size / 2) < /**/size.y) { h4 = heights[x + step_size / 2, y + 3 * step_size / 2]; count++; }
						else { h4 = 0f; }
						aver = (h1 + h2 + h3 + h4) / count;
						h = Random.Range(heightMin, heightMax);
						aver += h * rand_max;
						heights[x + step_size / 2, y + step_size] = aver;
					}
				}

				rand_max *= Mathf.Pow(2, -H);
				step_size /= 2;
			}

			return heights;
		}
	}
}
