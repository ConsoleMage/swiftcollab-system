using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
public class TaskExecutor
{
    private readonly ILogger<TaskExecutor> _logger;
    private Queue<string> taskQueue = new Queue<string>();

    public TaskExecutor(ILogger<TaskExecutor> logger)
    {
        _logger = logger;
    }
    public void AddTask(string task)
    {
        if (task == null)
        {
            _logger.LogWarning("Invalid task: cannot be null.");
            return;
        }
        taskQueue.Enqueue(task);
    }
    public void ProcessTasks()
    {
        while (taskQueue.Count > 0)
        {
            string task = taskQueue.Dequeue();
            _logger.LogInformation($"Processing task: {task}");
            int maxRetries = 3;
            bool success = false;
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    ExecuteTask(task);
                    success = true;
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Attempt {attempt} failed for task '{task}': {ex.Message}");
                    if (attempt == maxRetries)
                    {
                        _logger.LogError(ex, $"Task '{task}' failed after {maxRetries} attempts");
                    }
                }
            }
            if (!success)
            {
                _logger.LogError($"Skipping task '{task}' after retries.");
            }
        }
    }
    private void ExecuteTask(string task)
    {
        if (task == null)
        {
            throw new Exception("Task is null");
        }
        if (task.Contains("Fail"))
        {
            throw new Exception("Task execution failed");
        }
        _logger.LogInformation($"Task {task} completed successfully.");
    }
}
class Program
{
    static void Main()
    {
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<TaskExecutor>();
        TaskExecutor executor = new TaskExecutor(logger);
        executor.AddTask("Task 1");
        executor.AddTask(null); // This will be rejected
        executor.AddTask("Fail Task"); // This will fail during execution
        executor.AddTask("Task 2");
        executor.ProcessTasks();
    }
}
