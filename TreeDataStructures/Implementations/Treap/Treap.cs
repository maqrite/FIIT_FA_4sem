using System.Diagnostics.CodeAnalysis;
using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.Treap;

public class Treap<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, TreapNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    /// <summary>
    /// Разрезает дерево с корнем <paramref name="root"/> на два поддерева:
    /// Left: все ключи <= <paramref name="key"/>
    /// Right: все ключи > <paramref name="key"/>
    /// </summary>
    protected virtual (TreapNode<TKey, TValue>? Left, TreapNode<TKey, TValue>? Right) Split(TreapNode<TKey, TValue>? root, TKey key)
    {
        if (root == null)
        {
            return (null, null);
        }

        if (Comparer.Compare(root.Key, key) <= 0)
        {
            (TreapNode<TKey, TValue>? SplitLeft, TreapNode<TKey, TValue>? SplitRight) =
                Split(root.Right, key);

            root.Right = SplitLeft;

            if (root.Right != null)
            {
                root.Right.Parent = root;
            }

            if (SplitRight != null)
            {
                SplitRight.Parent = null;
            }

            return (root, SplitRight);

        }

        else
        {
            (TreapNode<TKey, TValue>? SplitLeft, TreapNode<TKey, TValue>? SplitRight) =
                Split(root.Left, key);

            root.Left = SplitRight;

            if (root.Left != null)
            {
                root.Left.Parent = root;
            }

            if (SplitLeft != null)
            {
                SplitLeft.Parent = null;

            }

            return (SplitLeft, root);
        }
    }

    /// <summary>
    /// Сливает два дерева в одно.
    /// Важное условие: все ключи в <paramref name="left"/> должны быть меньше ключей в <paramref name="right"/>.
    /// Слияние происходит на основе Priority (куча).
    /// </summary>
    protected virtual TreapNode<TKey, TValue>? Merge(TreapNode<TKey, TValue>? left, TreapNode<TKey, TValue>? right)
    {
        if (left == null)
        {
            return right;
        }
        else if (right == null)
        {
            return left;
        }

        if (left.Priority > right.Priority)
        {
            left.Right = Merge(left.Right, right);

            if (left.Right != null)
            {
                left.Right.Parent = left;
            }

            return left;
        }
        else
        {
            right.Left = Merge(left, right.Left);

            if (right.Left != null)
            {
                right.Left.Parent = right;
            }

            return right;
        }
    }


    public override void Add(TKey key, TValue value)
    {
        if (ContainsKey(key) == true)
        {
            throw new ArgumentException($"Error: key {key} is in Treap now");
        }

        var newNode = CreateNode(key, value);

        (TreapNode<TKey, TValue>? LTree, TreapNode<TKey, TValue>? RTree) =
            Split(this.Root, key);

        var MergedL = Merge(LTree, newNode);
        var result = Merge(MergedL, RTree);

        this.Root = result;
        if (this.Root != null)
        {
            this.Root.Parent = null;
        }

        Count++;
    }

    public override bool Remove(TKey key)
    {
        TreapNode<TKey, TValue>? NodeToRemove = FindNode(key);

        if (NodeToRemove == null)
        {
            return false;
        }

        TreapNode<TKey, TValue>? MergedChildren =
            Merge(NodeToRemove.Left, NodeToRemove.Right);

        Transplant(NodeToRemove, MergedChildren);

        Count--;

        return true;
    }

    protected override TreapNode<TKey, TValue> CreateNode(TKey key, TValue value)
    {
        return new TreapNode<TKey, TValue>(key, value);
    }

    protected override void OnNodeAdded(TreapNode<TKey, TValue> newNode) { }
    protected override void OnNodeRemoved(TreapNode<TKey, TValue>? parent, TreapNode<TKey, TValue>? child) { }
}
