using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestsGenerator
{
    public class Reader
    {
		private int threadsCount;
		public Reader(int threadsCount)
		{
			this.threadsCount = threadsCount;
		}

		public List<Task<string>> ReadFromFiles(List<string> paths)
		{
			List<Task<string>> readTasks = new List<Task<string>>();
			foreach (string path in paths)
			{
				readTasks.Add(ReadFromFile(path));
			}

			for (int i = 0; i< threadsCount; i++)
			{
				RunFreeTasksAsync(readTasks);
			}
			return readTasks;
		}

		private async void RunFreeTasksAsync(List<Task<string>> readTasks)
		{
			foreach(Task<string> readTask in readTasks)
			{
				if (readTask.Status == TaskStatus.Created)
				{
					readTask.Start();
					await readTask;
				}
			}
			
		}

		private Task<string> ReadFromFile(string path)
		{
			using (StreamReader reader = new StreamReader(path))
			{
				return new Task<string>(reader.ReadToEnd);
			}
		}
	}
}
