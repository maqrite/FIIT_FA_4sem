using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.AVL;

public class AvlTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, AvlNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    protected override AvlNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    protected override void OnNodeAdded(AvlNode<TKey, TValue> newNode)
    {
        var current = newNode.Parent;

        while (current != null)
        {
            Balance(current);
            current = current.Parent;
        }
    }

    protected private int GetHeight(AvlNode<TKey, TValue>? node)
    {
        if (node == null)
        {
            return 0;
        }
        return node.Height;
    }

    protected private int GetBalanceFactor(AvlNode<TKey, TValue>? node)
    {
        return (GetHeight(node?.Right) - GetHeight(node?.Left));
    }

    protected private void UpdateHeight(AvlNode<TKey, TValue> node)
    {
        int leftHeight = GetHeight(node.Left);
        int rightHeight = GetHeight(node.Right);

        int maxHeight = Math.Max(leftHeight, rightHeight);
        node.Height = ++maxHeight;
    }

    protected override void RotateRight(AvlNode<TKey, TValue> node)
    {
        base.RotateRight(node);
        UpdateHeight(node);
        UpdateHeight(node.Parent!);

        return;
    }

    protected override void RotateLeft(AvlNode<TKey, TValue> node)
    {
        base.RotateLeft(node);
        UpdateHeight(node);
        UpdateHeight(node.Parent!);

        return;
    }

    protected void Balance(AvlNode<TKey, TValue> node)
    {
        UpdateHeight(node);
        int balance = GetBalanceFactor(node);

        if (balance == 2)
        {
            if (GetBalanceFactor(node.Right) < 0)
            {
                RotateRight(node.Right!);
            }

            RotateLeft(node);
        }
        else if (balance == -2)
        {
            if (GetBalanceFactor(node.Left) > 0)
            {
                RotateLeft(node.Left!);
            }

            RotateRight(node);
        }
    }
}

