using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.RedBlackTree;

public class RedBlackTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, RbNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    protected override RbNode<TKey, TValue> CreateNode(TKey key, TValue value) => new(key, value);

    protected override void OnNodeAdded(RbNode<TKey, TValue> newNode)
    {
        while (newNode != null && newNode != this.Root && IsRed(newNode.Parent))
        {
            var grandParent = GetGrandparent(newNode);
            var uncle = GetUncle(newNode);

            //левая сторона
            if (grandParent != null && newNode.Parent == grandParent.Left)
            {
                if (IsRed(uncle))
                {
                    newNode.Parent!.Color = RbColor.Black;
                    uncle!.Color = RbColor.Black;
                    grandParent!.Color = RbColor.Red;
                    newNode = grandParent;
                }
                else
                {
                    if (newNode == newNode.Parent!.Right)
                    {
                        newNode = newNode.Parent;
                        RotateLeft(newNode);
                    }

                    newNode.Parent!.Color = RbColor.Black;
                    grandParent.Color = RbColor.Red;
                    RotateRight(grandParent);
                }
            }

            // правая сторона
            else if (grandParent != null && newNode.Parent == grandParent.Right)
            {
                if (IsRed(uncle))
                {
                    newNode.Parent!.Color = RbColor.Black;
                    uncle!.Color = RbColor.Black;
                    grandParent!.Color = RbColor.Red;
                    newNode = grandParent;
                }
                else
                {
                    if (newNode == newNode.Parent!.Left)
                    {
                        newNode = newNode.Parent;
                        RotateRight(newNode);
                    }

                    newNode.Parent!.Color = RbColor.Black;
                    grandParent.Color = RbColor.Red;
                    RotateLeft(grandParent);
                }
            }
        }

        if (this.Root != null)
        {
            this.Root.Color = RbColor.Black;
        }
    }
    protected override void OnNodeRemoved(RbNode<TKey, TValue>? parent, RbNode<TKey, TValue>? child)
    {
        var current = child;
        var currentParent = parent;

        while (current != this.Root && IsBlack(current))
        {
            // левая сторона
            if (current == currentParent!.Left)
            {
                var sibling = GetSibling(current, currentParent);

                if (IsRed(sibling))
                {
                    sibling!.Color = RbColor.Black;
                    currentParent!.Color = RbColor.Red;
                    RotateLeft(currentParent);
                    sibling = currentParent.Right;
                }

                if (IsBlack(sibling?.Left) && IsBlack(sibling?.Right))
                {
                    sibling!.Color = RbColor.Red;
                    current = currentParent;
                    currentParent = current?.Parent;
                    continue;
                }

                if (IsBlack(sibling!.Right))
                {
                    sibling.Left!.Color = RbColor.Black;
                    sibling.Color = RbColor.Red;
                    RotateRight(sibling);
                    sibling = currentParent!.Right;
                }

                sibling!.Color = currentParent.Color;
                currentParent.Color = RbColor.Black;
                sibling.Right!.Color = RbColor.Black;
                RotateLeft(currentParent);
                current = this.Root;
            }

            // правая сторона
            else
            {
                var sibling = GetSibling(current, currentParent);

                if (IsRed(sibling))
                {
                    sibling!.Color = RbColor.Black;
                    currentParent!.Color = RbColor.Red;
                    RotateRight(currentParent);
                    sibling = currentParent.Left;
                }

                if (IsBlack(sibling?.Right) && IsBlack(sibling?.Left))
                {
                    sibling!.Color = RbColor.Red;
                    current = currentParent;
                    currentParent = current?.Parent;
                    continue;
                }

                if (IsBlack(sibling!.Left))
                {
                    sibling.Right!.Color = RbColor.Black;
                    sibling.Color = RbColor.Red;
                    RotateLeft(sibling);
                    sibling = currentParent!.Left;
                }

                sibling!.Color = currentParent.Color;
                currentParent.Color = RbColor.Black;
                sibling.Left!.Color = RbColor.Black;
                RotateRight(currentParent);
                current = this.Root;
            }
        }

        if (current != null)
        {
            current.Color = RbColor.Black;
        }
    }

    protected bool IsRed(RbNode<TKey, TValue>? node)
    {
        return node == null ? false : (RbColor.Red == node.Color);
    }

    protected bool IsBlack(RbNode<TKey, TValue>? node)
    {
        return !IsRed(node);
    }

    protected RbNode<TKey, TValue>? GetGrandparent(RbNode<TKey, TValue> node)
    {
        return node.Parent?.Parent;
    }

    protected RbNode<TKey, TValue>? GetUncle(RbNode<TKey, TValue> node)
    {
        RbNode<TKey, TValue>? grandParent = GetGrandparent(node);

        if (grandParent == null)
        {
            return null;
        }

        if (node.Parent == grandParent.Left)
        {
            return grandParent.Right;
        }
        else
        {
            return grandParent.Left;
        }
    }

    protected RbNode<TKey, TValue>? GetSibling(RbNode<TKey, TValue>? node, RbNode<TKey, TValue>? parent)
    {
        if (parent == null) { return null; }

        return node == parent.Left ? parent.Right : parent.Left;
    }
}

