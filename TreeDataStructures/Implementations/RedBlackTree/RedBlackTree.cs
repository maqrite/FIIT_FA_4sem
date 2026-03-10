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
        throw new NotImplementedException();
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
}
