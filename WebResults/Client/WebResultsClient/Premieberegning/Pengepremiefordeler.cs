using System;
using System.Collections.Generic;
using System.Linq;

namespace WebResultsClient.Premieberegning
{
	public static class PengepremieFordeler
	{
		public static List<int> BeregnPremieSummer(int n, double premietotal)
		{
			if (n < 1)
			{
				return new List<int>();
			}

			if (n == 1)
			{
				return new List<int> { (int)Math.Round(premietotal) };
			}

			double k = 2.0 / (2 * n * n - 7 * n + 16);
			double x = (1.0 - k * (n * (n - 1) / 2.0)) / n;

			var premier = new List<int>();
			for (var i = n - 1; i >= 0; i--)
			{
				var rundetSum = (int)Math.Round(premietotal * (x + k * i));
				premier.Add(rundetSum);
			}

			return premier.ToList();
		}
	}
}
