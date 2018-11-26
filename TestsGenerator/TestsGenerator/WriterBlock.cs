using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace TestsGenerator
{
	internal class WriterBlock
	{
		private static object locker = new object();
		private int threadsCount;
		private readonly string outputDirectoryPath;
		private int currWaiters;
		public WriterBlock(int threadsCount, string outputDirectoryPath)
		{
			this.threadsCount = threadsCount;
			this.outputDirectoryPath = outputDirectoryPath;
			currWaiters = 0;
		}
		public Task WriteToFiles(List<Task<List<CreatedTestTemplate>>> generationTasks)
		{
			Dictionary<Task<List<CreatedTestTemplate>>, Task> keyValue = new Dictionary<Task<List<CreatedTestTemplate>>, Task>();
			foreach (Task<List<CreatedTestTemplate>> generationTask in generationTasks)
			{
				keyValue.Add(generationTask, new Task(() => WriteToFile(generationTask)));
			}

			for (int i = 0; i < threadsCount; i++)
			{
				RunEveryReadyTaskAsync(generationTasks, keyValue);
			}

			return Task.WhenAll(keyValue.Values);
		}
		private async void RunEveryReadyTaskAsync(List<Task<List<CreatedTestTemplate>>> generationTasks, Dictionary<Task<List<CreatedTestTemplate>>, Task> keyValue)
		{
			Task<List<CreatedTestTemplate>> complitedGenerationTask = null;
			Task writeTask = null;
			bool isWaitingData = false;
			while (true)
			{
				lock (locker)
				{
					if (generationTasks.Count - currWaiters > 0 && !isWaitingData)
					{
						currWaiters++;
						isWaitingData = true;
					}
					else if (isWaitingData)
					{
						keyValue.TryGetValue(complitedGenerationTask, out writeTask);
						if (writeTask.Status == TaskStatus.Created)
						{
							currWaiters--;
							isWaitingData = false;
							generationTasks.Remove(complitedGenerationTask);
							writeTask.Start();
						}
					}
					else
						break;
				}

				if (isWaitingData)
					complitedGenerationTask = await Task.WhenAny(generationTasks);
				else
					await writeTask;
			}

		}

		private void WriteToFile(Task<List<CreatedTestTemplate>> generateResults)
		{
			var results = generateResults.Result;
			foreach (var result in results)
			{
				string filePath = $"{outputDirectoryPath}\\{result.Name}";
				using (StreamWriter sw = new StreamWriter(filePath))
				{
					sw.Write(result.Text);
				}
			}
		}
	}
}
