using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DancingLinks
{
	public class Node
	{
		public Node North { get; set; }

		public Node East { get; set; }

		public Node South { get; set; }

		public Node West { get; set; }

		public Node Header { get; set; }

		public string Label { get; set; }

		public int Children { get; set; }

		public Node(string label = "")
		{
			Label = label;
			Children = 0;
		}
	}
}
