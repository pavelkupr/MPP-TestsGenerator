using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestsGenerator
{
	public delegate string ReadFromFileDelegate(string path);

	internal class ReaderBlock
    {
		static object locker = new object();
		private int threadsCount;
		private ReadFromFileDelegate readFunc;
		internal ReadFromFileDelegate ReadFunc { set { readFunc = value ?? CommonReadFromFile; } }
		
		public ReaderBlock(int threadsCount)
		{
			this.threadsCount = threadsCount;
			readFunc = CommonReadFromFile;
		}

		public List<Task<string>> ReadFromFiles(List<string> paths)
		{
			List<Task<string>> readTasks = new List<Task<string>>();
			foreach (string path in paths)
			{
				readTasks.Add(new Task<string>(() => readFunc.Invoke(path)));
			}
			
			for (int i = 0; i< threadsCount; i++)
			{
				RunFreeTasksAsync(readTasks);
			}
			return new List<Task<string>>(readTasks);
		}

		private async void RunFreeTasksAsync(List<Task<string>> readTasks)
		{
			bool isWaitingData = false;
			foreach (Task<string> readTask in readTasks)
			{
				lock (locker)
				{
					if (readTask.Status == TaskStatus.Created)
					{
						readTask.Start();
						isWaitingData = true;
					}
				}
				if (isWaitingData == true)
				{
					await readTask;
					isWaitingData = false;
				}
			}
			
		}

		private string CommonReadFromFile(string path)
		{
			using (StreamReader reader = new StreamReader(path))
			{
				return reader.ReadToEnd();
			}
		}
	}
}
