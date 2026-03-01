// тут можно что-то тестировать
var tree = new TreeDataStructures.Implementations.BST.BinarySearchTree<int, string>();

int[] keys = { 8, 3, 10, 1, 6, 14 };

foreach (var k in keys)
{
    tree.Add(k, $"Value_{k}");
}

Console.WriteLine("InOrder:");
foreach (var entry in tree.InOrder())
{
    Console.WriteLine($"Key: {entry.Key}, value: {entry.Value}");
}

Console.WriteLine("InOrderRev");
foreach (var entry in tree.InOrderReverse())
{
    Console.WriteLine($"Key: {entry.Key}");
}

Console.WriteLine("delete node with 2 kids");
tree.Remove(3);

Console.WriteLine("tree after:");
foreach (var entry in tree.InOrder())
{
    Console.WriteLine($"Key: {entry.Key}");
}

