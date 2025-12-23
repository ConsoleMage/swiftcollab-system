using System;
using System.Threading.Tasks;
using System.Diagnostics;
public class Sorting
{
    private const int InsertionSortThreshold = 10;

    public static void QuickSort(int[] arr, int low, int high)
    {
        if (high - low < InsertionSortThreshold)
        {
            InsertionSort(arr, low, high);
        }
        else if (low < high)
        {
            int pi = Partition(arr, low, high);
            QuickSort(arr, low, pi - 1);
            QuickSort(arr, pi + 1, high);
        }
    }

    public static void ParallelQuickSort(int[] arr, int low, int high, int threshold = 1000)
    {
        if (low < high)
        {
            if (high - low < threshold)
            {
                QuickSort(arr, low, high);
            }
            else
            {
                int pi = Partition(arr, low, high);
                Parallel.Invoke(
                    () => ParallelQuickSort(arr, low, pi - 1, threshold),
                    () => ParallelQuickSort(arr, pi + 1, high, threshold)
                );
            }
        }
    }

    private static void InsertionSort(int[] arr, int low, int high)
    {
        for (int i = low + 1; i <= high; i++)
        {
            int key = arr[i];
            int j = i - 1;
            while (j >= low && arr[j] > key)
            {
                arr[j + 1] = arr[j];
                j--;
            }
            arr[j + 1] = key;
        }
    }

    private static int Partition(int[] arr, int low, int high)
    {
        // Median of three pivot selection
        int mid = low + (high - low) / 2;
        if (arr[low] > arr[mid])
            Swap(arr, low, mid);
        if (arr[low] > arr[high])
            Swap(arr, low, high);
        if (arr[mid] > arr[high])
            Swap(arr, mid, high);
        // Now arr[mid] is the median, swap with high
        Swap(arr, mid, high);

        int pivot = arr[high];
        int i = (low - 1);
        for (int j = low; j < high; j++)
        {
            if (arr[j] < pivot)
            {
                i++;
                Swap(arr, i, j);
            }
        }
        Swap(arr, i + 1, high);
        return i + 1;
    }

    private static void Swap(int[] arr, int i, int j)
    {
        int temp = arr[i];
        arr[i] = arr[j];
        arr[j] = temp;
    }

    public static void PrintArray(int[] arr)
    {
        foreach (var item in arr)
        {
            Console.Write(item + " ");
        }
        Console.WriteLine();
    }

    private static int[] GenerateRandomArray(int size)
    {
        Random rand = new Random();
        int[] arr = new int[size];
        for (int i = 0; i < size; i++)
        {
            arr[i] = rand.Next(0, size * 10);
        }
        return arr;
    }

    public static void Main()
    {
        // Small dataset for demonstration
        int[] smallDataset = { 50, 20, 40, 10, 30 };
        Console.WriteLine("Small Dataset:");
        Console.WriteLine("Before Sorting:");
        PrintArray(smallDataset);
        ParallelQuickSort(smallDataset, 0, smallDataset.Length - 1);
        Console.WriteLine("After Sorting:");
        PrintArray(smallDataset);

        // Benchmarking with larger dataset
        int largeSize = 10000;
        int[] largeDataset = GenerateRandomArray(largeSize);
        int[] copyForSequential = (int[])largeDataset.Clone();

        Console.WriteLine($"\nBenchmarking with {largeSize} elements:");

        // Sequential QuickSort
        Stopwatch sw = Stopwatch.StartNew();
        QuickSort(copyForSequential, 0, copyForSequential.Length - 1);
        sw.Stop();
        Console.WriteLine($"Sequential QuickSort Time: {sw.ElapsedMilliseconds} ms");

        // Parallel QuickSort
        sw.Restart();
        ParallelQuickSort(largeDataset, 0, largeDataset.Length - 1);
        sw.Stop();
        Console.WriteLine($"Parallel QuickSort Time: {sw.ElapsedMilliseconds} ms");

        // Verify sorting
        bool isSorted = true;
        for (int i = 1; i < largeDataset.Length; i++)
        {
            if (largeDataset[i] < largeDataset[i - 1])
            {
                isSorted = false;
                break;
            }
        }
        Console.WriteLine($"Array is sorted: {isSorted}");
    }
}
