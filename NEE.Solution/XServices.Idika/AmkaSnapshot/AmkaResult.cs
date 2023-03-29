using System.Collections.Generic;
using System.Linq;

namespace XService.Idika
{
	public class AmkaResult
	{
		public AmkaResult(List<AmkaRow> rows)
		{
			Rows = rows;
		}

		public bool FoundNoRecords() => !Rows.Any();
		public List<AmkaRow> Rows { get; }
	}
}
