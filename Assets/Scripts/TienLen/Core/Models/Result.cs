using UnityEngine;
using System.Collections;

namespace TienLen.Core.Models
{
	public class Results
	{
		public  int Place = 0;
		public  int Turns = 0;

		public Results ()
		{

		}
		
		/// <summary>
		/// Clear the results to its default values.
		/// </summary>
		public void ClearResults ()
		{
			Place = 0;
			Turns = 0;
		}
	}
}