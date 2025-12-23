using System;
using System.Collections.Generic;
using System.Threading;
/// <summary>
/// Represents an API request with an endpoint and priority.
/// Implements IComparable to enable priority-based ordering (lower priority number = higher priority).
/// </summary>
public class ApiRequest : IComparable<ApiRequest>
{
    /// <summary>
    /// The API endpoint for the request.
    /// </summary>
    public string Endpoint { get; set; }
    /// <summary>
    /// The priority of the request (lower values indicate higher priority).
    /// </summary>
    public int Priority { get; set; }
    /// <summary>
    /// Initializes a new instance of the ApiRequest class.
    /// </summary>
    /// <param name="endpoint">The API endpoint.</param>
    /// <param name="priority">The priority level.</param>
    public ApiRequest(string endpoint, int priority)
    {
        Endpoint = endpoint;
        Priority = priority;
    }
    /// <summary>
    /// Compares this ApiRequest to another based on priority.
    /// </summary>
    /// <param name="other">The other ApiRequest to compare to.</param>
    /// <returns>A negative value if this has higher priority, zero if equal, positive if lower.</returns>
    public int CompareTo(ApiRequest? other)
    {
        if (other == null) return 1;
        return Priority.CompareTo(other.Priority);
    }
}
/// <summary>
/// A binary min-heap implementation for efficient priority queue operations.
/// Supports O(log n) insert and extract-min operations.
/// </summary>
/// <typeparam name="T">The type of elements in the heap, must implement IComparable&lt;T&gt;.</typeparam>
public class MinHeap<T> where T : IComparable<T>
{
    private List<T> heap = new List<T>();
    /// <summary>
    /// Swaps two elements in the heap array.
    /// </summary>
    private void Swap(int i, int j)
    {
        T temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;
    }
    /// <summary>
    /// Restores the heap property by bubbling up an element.
    /// Time complexity: O(log n)
    /// </summary>
    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (heap[index].CompareTo(heap[parent]) >= 0) break;
            Swap(index, parent);
            index = parent;
        }
    }
    /// <summary>
    /// Restores the heap property by bubbling down an element.
    /// Time complexity: O(log n)
    /// </summary>
    private void HeapifyDown(int index)
    {
        int size = heap.Count;
        while (true)
        {
            int left = 2 * index + 1;
            int right = 2 * index + 2;
            int smallest = index;
            if (left < size && heap[left].CompareTo(heap[smallest]) < 0) smallest = left;
            if (right < size && heap[right].CompareTo(heap[smallest]) < 0) smallest = right;
            if (smallest == index) break;
            Swap(index, smallest);
            index = smallest;
        }
    }
    /// <summary>
    /// Inserts a new element into the heap.
    /// Time complexity: O(log n)
    /// </summary>
    /// <param name="item">The item to insert.</param>
    public void Insert(T item)
    {
        heap.Add(item);
        HeapifyUp(heap.Count - 1);
    }
    /// <summary>
    /// Extracts and returns the minimum element from the heap.
    /// Time complexity: O(log n)
    /// </summary>
    /// <returns>The minimum element, or default(T) if heap is empty.</returns>
    public T ExtractMin()
    {
        if (heap.Count == 0) return default(T);
        T min = heap[0];
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);
        if (heap.Count > 0) HeapifyDown(0);
        return min;
    }
    /// <summary>
    /// Gets the number of elements in the heap.
    /// </summary>
    public int Count => heap.Count;
}
/// <summary>
/// A thread-safe priority queue for API requests using a min-heap.
/// Supports efficient enqueue/dequeue operations and concurrent access.
/// Lower priority numbers indicate higher priority.
/// </summary>
public class ApiRequestQueue
{
    private MinHeap<ApiRequest> heap = new MinHeap<ApiRequest>();
    private object lockObject = new object(); // Synchronization object for thread-safety
    /// <summary>
    /// Enqueues a single API request into the priority queue.
    /// Thread-safe operation.
    /// Time complexity: O(log n)
    /// </summary>
    /// <param name="request">The API request to enqueue.</param>
    public void Enqueue(ApiRequest request)
    {
        lock (lockObject)
        {
            heap.Insert(request);
        }
    }
    /// <summary>
    /// Enqueues multiple API requests in batch for efficiency.
    /// Thread-safe operation.
    /// Time complexity: O(k log n) where k is the number of requests
    /// </summary>
    /// <param name="requests">The collection of API requests to enqueue.</param>
    public void EnqueueBatch(IEnumerable<ApiRequest> requests)
    {
        lock (lockObject)
        {
            foreach (var request in requests)
            {
                heap.Insert(request);
            }
        }
    }
    /// <summary>
    /// Dequeues and returns the highest priority API request.
    /// Thread-safe operation.
    /// Time complexity: O(log n)
    /// </summary>
    /// <returns>The highest priority request, or null if queue is empty.</returns>
    public ApiRequest Dequeue()
    {
        lock (lockObject)
        {
            if (heap.Count == 0)
                return null;
            return heap.ExtractMin();
        }
    }
}
class Program
{
    /// <summary>
    /// Demonstrates the thread-safe API request queue with concurrent producers and consumers.
    /// Shows priority ordering and bulk enqueue functionality.
    /// </summary>
    static void Main()
    {
        ApiRequestQueue queue = new ApiRequestQueue();
        // Concurrent processing example
        Thread producer1 = new Thread(() =>
{
    for (int i = 0; i < 5; i++)
    {
        queue.Enqueue(new ApiRequest($"/endpoint{i}", i));
        Thread.Sleep(10); // Simulate some delay
    }
});
        Thread producer2 = new Thread(() =>
        {
            var batch = new List<ApiRequest>
        {
new ApiRequest("/batchA", 10),
new ApiRequest("/batchB", 11)
        };
            queue.EnqueueBatch(batch);
        });
        Thread consumer = new Thread(() =>
        {
            for (int i = 0; i < 7; i++)
            {
                var request = queue.Dequeue();
                if (request != null)
                {
                    Console.WriteLine($"Processed: {request.Endpoint} (Priority: {request.Priority})");
                }
                Thread.Sleep(15); // Simulate processing time
            }
        });
        producer1.Start();
        producer2.Start();
        consumer.Start();
        producer1.Join();
        producer2.Join();
        consumer.Join();
        Console.WriteLine("Concurrent processing completed.");
    }
}