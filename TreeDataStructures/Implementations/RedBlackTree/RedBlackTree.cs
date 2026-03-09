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
                    //по идее красим родителя в черный 
                    //дядю в черный (осуждаю) 
                    //потом дедушку в черни 
                    //-> newNode = grandParent
                }
                else
                {
                    //логика поворотов дальше впадлу пока
                }
            }

            // правая сторона
            else if (true)
            {

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
