
using System;
using System.Collections.Generic;

namespace Global
{

	public class MetadataResponse
	{

		public MetaData metaData { get; set; }

		/// <summary>
		/// Examples: true
		/// </summary>
		public bool success { get; set; }

		/// <summary>
		/// Examples: "ce3f6b19-d2b7-46f6-95d7-c70823b9a864"
		/// </summary>
		public string correlationId { get; set; }

		/// <summary>
		/// Examples: 3
		/// </summary>
		public int version { get; set; }
	}

}
