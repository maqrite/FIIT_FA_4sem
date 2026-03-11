using System.Diagnostics.CodeAnalysis;
using TreeDataStructures.Implementations.BST;

namespace TreeDataStructures.Implementations.Splay;

public class SplayTree<TKey, TValue> : BinarySearchTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    protected override BstNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    protected override void OnNodeAdded(BstNode<TKey, TValue> newNode)
    {
        Splay(newNode);
    }

    protected override void OnNodeRemoved(BstNode<TKey, TValue>? parent, BstNode<TKey, TValue>? child)
    {
        Splay(parent ?? child);
    }

    public override bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        var current = this.Root;
        BstNode<TKey, TValue>? lastAccessed = null;

        while (current != null)
        {
            lastAccessed = current;
            int cmp = Comparer.Compare(key, current.Key);

            if (cmp == 0)
            {
                Splay(current);
                value = current.Value;
                return true;
            }

            current = cmp < 0 ? current.Left : current.Right;
        }

        if (lastAccessed != null)
        {
            Splay(lastAccessed);
        }

        value = default;
        return false;
    }

    private void Splay(BstNode<TKey, TValue>? node)
    {
        if (node == null) { return; }

        while (node.Parent != null)
        {
            var parent = node.Parent;
            var grandParent = parent.Parent;

            if (grandParent == null)
            {
                if (node == parent.Left)
                {
                    RotateRight(parent);
                }
                else
                {
                    RotateLeft(parent);
                }
            }
            else if (node == parent.Left && parent == grandParent.Left)
            {
                RotateDoubleRight(grandParent);
            }
            else if (node == parent.Right && parent == grandParent.Right)
            {
                RotateDoubleLeft(grandParent);
            }
            else if (node == parent.Right && parent == grandParent.Left)
            {
                RotateBigRight(grandParent);
            }
            else
            {
                RotateBigLeft(grandParent);
            }
        }
    }

    public override bool ContainsKey(TKey key)
    {
        return TryGetValue(key, out _);
    }
}
