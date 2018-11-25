using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGenerator
{ 
	class Generator
	{
		private int threadsCount;
		public Generator(int threadsCount)
		{
			this.threadsCount = threadsCount;
		}

		public List<Task<string>> GenerateTestClasses(List<Task<string>> readTasks)
		{

		}

		private async void GetFirstResult(List<Task<string>> readTasks)
		{
			while (readTasks.Count != 0)
			{
				Task<string> complited = await Task.WhenAny(readTasks);

			}
			
		}
	}
}
