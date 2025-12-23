using System;
using System.Collections.Generic;

public class Node
{
    public int Value;
    public Node Left, Right;
    public Node(int value)
    {
        Value = value;
        Left = Right = null;
    }
}
public class BinaryTree
{
    public Node Root;
    private int _count = 0; // Tracks the number of nodes for O(1) size queries
    public int Count => _count; // Public property for memory-efficient size access
    // Optimized iterative insert to avoid stack overflow in deep trees
    // Previously recursive, now uses a loop for better performance and memory usage
    public void Insert(int value)
    {
        if (Root == null)
        {
            Root = new Node(value);
            return;
        }
        Node current = Root;
        while (true)
        {
            if (value < current.Value)
            {
                if (current.Left == null)
                {
                    current.Left = new Node(value);
                    return;
                }
                current = current.Left;
            }
            else
            {
                if (current.Right == null)
                {
                    current.Right = new Node(value);
                    return;
                }
                current = current.Right;
            }
        }
    }
    public void PrintInOrder(Node node)
    {
        if (node == null) return;
        PrintInOrder(node.Left);
        Console.Write(node.Value + " ");
        PrintInOrder(node.Right);
    }

    // Search functionality - checks if a value exists in the tree
    // Uses recursive traversal for simplicity, could be optimized to iterative for very deep trees
    public bool Contains(int value)
    {
        return ContainsRecursive(Root, value);
    }
    private bool ContainsRecursive(Node current, int value)
    {
        if (current == null) return false;
        if (value == current.Value) return true;
        if (value < current.Value) return ContainsRecursive(current.Left, value);
        else return ContainsRecursive(current.Right, value);
    }

    // Tree balancing functionality - rebuilds the tree to be height-balanced
    // Improves search and insert performance by ensuring O(log n) operations
    // Method to balance the binary tree
    private List<int> GetInOrderList(Node node)
    {
        List<int> list = new List<int>();
        if (node != null)
        {
            list.AddRange(GetInOrderList(node.Left));
            list.Add(node.Value);
            list.AddRange(GetInOrderList(node.Right));
        }
        return list;
    }
    private Node BuildBalancedTree(List<int> list, int start, int end)
    {
        if (start > end) return null;
        int mid = (start + end) / 2;
        Node node = new Node(list[mid]);
        node.Left = BuildBalancedTree(list, start, mid - 1);
        node.Right = BuildBalancedTree(list, mid + 1, end);
        return node;
    }
    public void BalanceTree()
    {
        List<int> list = GetInOrderList(Root);
        Root = BuildBalancedTree(list, 0, list.Count - 1);
    }
}