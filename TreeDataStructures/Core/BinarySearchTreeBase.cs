using System.Collections;
using System.Diagnostics.CodeAnalysis;
using TreeDataStructures.Interfaces;

namespace TreeDataStructures.Core;

public abstract class BinarySearchTreeBase<TKey, TValue, TNode>(IComparer<TKey>? comparer = null)
    : ITree<TKey, TValue>
    where TNode : Node<TKey, TValue, TNode>
{
    protected TNode? Root;
    public IComparer<TKey> Comparer { get; protected set; } = comparer ?? Comparer<TKey>.Default; // use it to compare Keys

    public int Count { get; protected set; }

    public bool IsReadOnly => false;

    public ICollection<TKey> Keys => throw new NotImplementedException();
    public ICollection<TValue> Values => throw new NotImplementedException();


    public virtual void Add(TKey key, TValue value)
    {
        if (key == null)
        {
            throw new ArgumentNullException("invalid TreeKey");
        }

        if (Root == null)
        {
            try
            {
                Root = CreateNode(key, value);
                Count++;
                OnNodeAdded(Root);
                return;
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("Memory allocation error for root");
                return;
            }
        }

        TNode? current = Root;
        TNode? parent = null;

        while (current != null)
        {
            parent = current;

            int cmp = Comparer.Compare(key, current.Key);

            if (cmp < 0)
            {
                current = current.Left;
            }
            else if (cmp > 0)
            {
                current = current.Right;
            }
            else
            {
                throw new ArgumentException($"Node with this key already exists. key: {key}");
            }
        }

        try
        {
            current = CreateNode(key, value);
            Count++;

        }
        catch (OutOfMemoryException)
        {
            Console.WriteLine("Memory allocation error for node");
            return;
        }

        if (Comparer.Compare(key, parent!.Key) < 0)
        {
            parent.Left = current;
        }
        else
        {
            parent.Right = current;
        }

        current.Parent = parent;
        OnNodeAdded(current);
    }


    public virtual bool Remove(TKey key)
    {
        TNode? node = FindNode(key);
        if (node == null) { return false; }

        RemoveNode(node);
        this.Count--;
        return true;
    }


    protected virtual void RemoveNode(TNode node)
    {
        TNode? hookParent = null;
        TNode? hookChild = null;

        if (node.Right == null)
        {
            hookParent = node.Parent;
            hookChild = node.Left;
            Transplant(node, node.Left);
        }
        else if (node.Left == null)
        {
            hookParent = node.Parent;
            hookChild = node.Right;
            Transplant(node, node.Right);
        }
        else
        {
            var minNode = node.Right;

            while (minNode.Left != null)
            {
                minNode = minNode.Left;
            }

            if (minNode.Parent != node)
            {
                hookParent = minNode.Parent;
                hookChild = minNode.Right;

                Transplant(minNode, minNode.Right);
                minNode.Right = node.Right;
                minNode.Right.Parent = minNode;
            }
            else
            {
                hookParent = minNode;
                hookChild = minNode.Right;

            }

            Transplant(node, minNode);

            minNode.Left = node.Left;
            minNode.Left.Parent = minNode;
        }

        OnNodeRemoved(hookParent, hookChild);
    }

    public virtual bool ContainsKey(TKey key) => FindNode(key) != null;

    public virtual bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        TNode? node = FindNode(key);
        if (node != null)
        {
            value = node.Value;
            return true;
        }
        value = default;
        return false;
    }

    public TValue this[TKey key]
    {
        get => TryGetValue(key, out TValue? val) ? val : throw new KeyNotFoundException();
        set => Add(key, value);
    }


    #region Hooks

    /// <summary>
    /// Вызывается после успешной вставки
    /// </summary>
    /// <param name="newNode">Узел, который встал на место</param>
    protected virtual void OnNodeAdded(TNode newNode) { }

    /// <summary>
    /// Вызывается после удаления. 
    /// </summary>
    /// <param name="parent">Узел, чей ребенок изменился</param>
    /// <param name="child">Узел, который встал на место удаленного</param>
    protected virtual void OnNodeRemoved(TNode? parent, TNode? child) { }

    #endregion


    #region Helpers
    protected abstract TNode CreateNode(TKey key, TValue value);


    protected TNode? FindNode(TKey key)
    {
        TNode? current = Root;
        while (current != null)
        {
            int cmp = Comparer.Compare(key, current.Key);
            if (cmp == 0) { return current; }
            current = cmp < 0 ? current.Left : current.Right;
        }
        return null;
    }

    protected void RotateLeft(TNode x)
    {
        var y = x.Right;
        if (y == null) { return; }

        if (y.Left != null)
        {
            y.Left.Parent = x;
        }

        x.Right = y.Left;
        Transplant(x, y);
        y.Left = x;
        x.Parent = y;
    }

    protected void RotateRight(TNode y)
    {
        var x = y.Left;
        if (x == null) { return; }

        if (x.Right != null)
        {
            x.Right.Parent = y;
        }

        y.Left = x.Right;
        Transplant(y, x);
        x.Right = y;
        y.Parent = x;
    }

    protected void RotateBigLeft(TNode x)
    {
        if (x.Right != null)
        {
            RotateRight(x.Right);
        }
        RotateLeft(x);
    }

    protected void RotateBigRight(TNode y)
    {
        if (y.Left != null)
        {
            RotateLeft(y.Left);
        }
        RotateRight(y);
    }

    protected void RotateDoubleLeft(TNode x)
    {
        RotateLeft(x);
        if (x.Parent != null)
        {
            RotateLeft(x.Parent);
        }
    }

    protected void RotateDoubleRight(TNode y)
    {
        RotateRight(y);
        if (y.Parent != null)
        {
            RotateRight(y.Parent);
        }
    }

    protected void Transplant(TNode u, TNode? v)
    {
        if (u.Parent == null)
        {
            Root = v;
        }
        else if (u.IsLeftChild)
        {
            u.Parent.Left = v;
        }
        else
        {
            u.Parent.Right = v;
        }
        v?.Parent = u.Parent;
    }
    #endregion

    public IEnumerable<TreeEntry<TKey, TValue>> InOrder() => InOrderTraversal(Root);

    private IEnumerable<TreeEntry<TKey, TValue>> InOrderTraversal(TNode? node)
    {
        if (node == null) { yield break; }
        throw new NotImplementedException();
    }

    public IEnumerable<TreeEntry<TKey, TValue>> PreOrder() => throw new NotImplementedException();
    public IEnumerable<TreeEntry<TKey, TValue>> PostOrder() => throw new NotImplementedException();
    public IEnumerable<TreeEntry<TKey, TValue>> InOrderReverse() => throw new NotImplementedException();
    public IEnumerable<TreeEntry<TKey, TValue>> PreOrderReverse() => throw new NotImplementedException();
    public IEnumerable<TreeEntry<TKey, TValue>> PostOrderReverse() => throw new NotImplementedException();

    /// <summary>
    /// Внутренний класс-итератор. 
    /// Реализует паттерн Iterator вручную, без yield return (ban).
    /// </summary>
    private struct TreeIterator :
        IEnumerable<TreeEntry<TKey, TValue>>,
        IEnumerator<TreeEntry<TKey, TValue>>
    {

        private readonly Stack<TNode> _stack;
        private TNode? _current;
        private TreeEntry<TKey, TValue> _currentEntry;

        private readonly TraversalStrategy _strategy;

        public TreeIterator(TNode? root, TraversalStrategy strategy)
        {
            _strategy = strategy;
            _stack = new Stack<TNode>();
            _current = root;
            _currentEntry = default;
        }

        public IEnumerator<TreeEntry<TKey, TValue>> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        public TreeEntry<TKey, TValue> Current => _currentEntry;
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (_strategy == TraversalStrategy.InOrder)
            {
                while (_current != null)
                {
                    _stack.Push(_current);
                    _current = _current.Left;
                }

                if (_stack.Count == 0)
                {
                    return false;
                }

                TNode x = _stack.Pop();
                _currentEntry = new(x.Key, x.Value, 0); //заглушка на глубину пока стоит
                _current = x.Right;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }


        public void Dispose()
        {
            // TODO release managed resources here
        }
    }


    private enum TraversalStrategy { InOrder, PreOrder, PostOrder, InOrderReverse, PreOrderReverse, PostOrderReverse }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
    public void Clear() { Root = null; Count = 0; }
    public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotImplementedException();
    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
}
