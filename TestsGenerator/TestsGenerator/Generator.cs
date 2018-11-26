using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestsGenerator
{ 
	internal class Generator
	{
		private static object locker = new object();
		private int currWaiters;
		private int threadsCount;
		
		public Generator(int threadsCount)
		{
			this.threadsCount = threadsCount;
			currWaiters = 0;
		}

		public List<Task<List<GeneratedTestTemplate>>> GenerateTestClasses(List<Task<string>> readTasks)
		{
			Dictionary<Task<string>, Task<List<GeneratedTestTemplate>>> keyValue = new Dictionary<Task<string>, Task<List<GeneratedTestTemplate>>>();
			foreach(Task<string> readTask in readTasks)
			{
				keyValue.Add(readTask,new Task<List<GeneratedTestTemplate>>(() => Generate(readTask)));
			}

			for(int i = 0;i < threadsCount;i++)
			{
				RunEveryReadyTaskAsync(readTasks, keyValue);
			}

			return new List<Task<List<GeneratedTestTemplate>>>(keyValue.Values);
		}

		private async void RunEveryReadyTaskAsync(List<Task<string>> readTasks, Dictionary<Task<string>, Task<List<GeneratedTestTemplate>>> keyValue)
		{
			Task<string> complitedReadTask = null;
			Task <List <GeneratedTestTemplate>> generationTask = null;
			bool isWaitingData = false;
			while (true)
			{
				lock (locker)
				{
					if (readTasks.Count - currWaiters > 0 && !isWaitingData)
					{
						currWaiters++;
						isWaitingData = true;
					}
					else if (isWaitingData)
					{
						keyValue.TryGetValue(complitedReadTask, out generationTask);
						if (generationTask.Status == TaskStatus.Created)
						{
							currWaiters--;
							isWaitingData = false;
							readTasks.Remove(complitedReadTask);
							generationTask.Start();
						}
					}
					else
						break;
				}

				if (isWaitingData)
					complitedReadTask = await Task.WhenAny(readTasks);
				else
					await generationTask;
			}
			
		}

		private List<GeneratedTestTemplate> Generate(Task<string> readTask)
		{
			TestClassCreator classCreator = new TestClassCreator();
			return classCreator.GenerateFileAsync(readTask.Result);
		}
	}
}
