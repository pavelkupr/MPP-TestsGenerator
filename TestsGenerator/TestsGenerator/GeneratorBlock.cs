using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestsGenerator
{ 
	internal class GeneratorBlock
	{
		private static object locker = new object();
		private int currWaiters;
		private int threadsCount;
		
		public GeneratorBlock(int threadsCount)
		{
			this.threadsCount = threadsCount;
			currWaiters = 0;
		}

		public List<Task<List<CreatedTestTemplate>>> GenerateTestClasses(List<Task<string>> readTasks)
		{
			Dictionary<Task<string>, Task<List<CreatedTestTemplate>>> keyValue = new Dictionary<Task<string>, Task<List<CreatedTestTemplate>>>();
			foreach(Task<string> readTask in readTasks)
			{
				keyValue.Add(readTask,new Task<List<CreatedTestTemplate>>(() => Generate(readTask)));
			}

			for(int i = 0;i < threadsCount;i++)
			{
				RunEveryReadyTaskAsync(readTasks, keyValue);
			}

			return new List<Task<List<CreatedTestTemplate>>>(keyValue.Values);
		}

		private async void RunEveryReadyTaskAsync(List<Task<string>> readTasks, Dictionary<Task<string>, Task<List<CreatedTestTemplate>>> keyValue)
		{
			Task<string> complitedReadTask = null;
			Task <List <CreatedTestTemplate>> generationTask = null;
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

		private List<CreatedTestTemplate> Generate(Task<string> readTask)
		{
			TestTemplateCreator templateCreator = new TestTemplateCreator();
			return templateCreator.GenerateTestTemplate(readTask.Result);
		}
	}
}
