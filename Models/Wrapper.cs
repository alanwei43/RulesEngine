using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.ComponentModel;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Specialized;

namespace RulesEngine.Model
{
    public class MyDynamic<T> : DynamicObject, IWrapper<T>, System.ComponentModel.INotifyPropertyChanged
    {
        //TODO: ATM, can only listen on internal changes. If Formulas/Getters involve external classes, would be nice to update.
        //TODO: Public Setter formulas.
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        T _inner;
        Dictionary<string, IGetter<T>> _getters = new Dictionary<string, IGetter<T>>();
        Dictionary<string, Delegate> _setters = new Dictionary<string, Delegate>();
        Dictionary<string, List<string>> _notifySinks = new Dictionary<string, List<string>>();
        private readonly object _synclock = new object();

        public MyDynamic(T inner)
        {
            if (inner == null) throw new System.ArgumentNullException("inner");
            _inner = inner;

            PropertyInfo[] properties = inner.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    _setters[property.Name] = ModelHelper.CreateSetter(property);
                    _notifySinks[property.Name] = new List<string>(new string[] { property.Name });
                }
                if (property.CanRead)
                {
                    _getters[property.Name] = ModelHelper.CreateGetter(this, property);
                }
            }
        }



        public static IWrapper<T> Create(T inner)
        {
            return new MyDynamic<T>(inner);
        }

        public IWrapper<T> CreateSetter<R>(string name, Expression<Action<T, R>> formula)
        {
            _setters[name] = formula.Compile();
            return this;
        }
        public Getter<T, R> CreateGetter<R>(string name, Expression<Func<T, R>> formula)
        {
            MyVisitor<T> visitor = new MyVisitor<T>();
            var result = new Getter<T, R>(this, (Expression<Func<T, R>>)visitor.Visit(formula));
            _getters[name] = result;

            foreach (string affectedProperty in visitor.AffectedProperties)
            {
                List<string> notifySink;
                if (_notifySinks.TryGetValue(affectedProperty, out notifySink))
                {
                    if (!notifySink.Contains(name))
                    {
                        notifySink.Add(name);
                    }
                }
            }

            return result;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _getters.Keys;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Delegate invoker;
            if (_setters.TryGetValue(binder.Name, out invoker))
            {
                invoker.DynamicInvoke(_inner, value);
                OnPropertyChanged(binder.Name);
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            IGetter<T> invoker;
            if (_getters.TryGetValue(binder.Name, out invoker))
            {
                result = invoker.Invoke(_inner);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }


        protected virtual void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                //TODO: Watch out for re-entrant calls here...
                foreach (string p in _notifySinks[property])
                {
                    this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(p));
                }
            }
        }
    }
    public class MyDynamicCollection<T> : DynamicObject, IWrapper<T>, INotifyPropertyChanged, INotifyCollectionChanged, ICollection<T>
    {
        private ICollection<T> _inner;
        Dictionary<string, IGetter<T>> _getters = new Dictionary<string, IGetter<T>>();
        Dictionary<string, Delegate> _setters = new Dictionary<string, Delegate>();
        Dictionary<string, List<string>> _notifySinks = new Dictionary<string, List<string>>();
        private readonly object _synclock = new object();
        private SimpleMonitor _monitor;

        private class SimpleMonitor : IDisposable
        {
            // Fields
            private int _busyCount;

            // Methods
            public void Dispose()
            {
                this._busyCount--;
            }

            public void Enter()
            {
                this._busyCount++;
            }

            // Properties
            public bool Busy
            {
                get
                {
                    return (this._busyCount > 0);
                }
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;


        public MyDynamicCollection(ICollection<T> inner)
        {
            if (inner == null) throw new System.ArgumentNullException("inner");
            _inner = inner;

            PropertyInfo[] properties = inner.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    _setters[property.Name] = ModelHelper.CreateSetter(property);
                    _notifySinks[property.Name] = new List<string>(new string[] { property.Name });
                }
                if (property.CanRead)
                {
                    _getters[property.Name] = ModelHelper.CreateGetter(this, property);
                }
            }
        }

        public IWrapper<T> CreateSetter<R>(string name, Expression<Action<T, R>> formula)
        {
            _setters[name] = formula.Compile();
            return this;
        }
        public Getter<T, R> CreateGetter<R>(string name, Expression<Func<T, R>> formula)
        {
            MyVisitor<T> visitor = new MyVisitor<T>();
            var result = new Getter<T, R>(this, (Expression<Func<T, R>>)visitor.Visit(formula));
            _getters[name] = result;

            foreach (string affectedProperty in visitor.AffectedProperties)
            {
                List<string> notifySink;
                if (_notifySinks.TryGetValue(affectedProperty, out notifySink))
                {
                    if (!notifySink.Contains(name))
                    {
                        notifySink.Add(name);
                    }
                }
            }

            return result;
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        protected virtual void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                //TODO: Watch out for re-entrant calls here...
                foreach (string p in _notifySinks[property])
                {
                    this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(p));
                }
            }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                using (this.BlockReentrancy())
                {
                    this.CollectionChanged(this, e);
                }
            }
        }

        private void OnCollectionReset()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected IDisposable BlockReentrancy()
        {
            this._monitor.Enter();
            return this._monitor;
        }

        protected void CheckReentrancy()
        {
            if ((_monitor.Busy && (this.CollectionChanged != null)) && (this.CollectionChanged.GetInvocationList().Length > 1))
            {
                throw new InvalidOperationException("Re-entrant calls not allowed.");
            }
        }

        public void Add(T item)
        {
            this.CheckReentrancy();
            int index = _inner.Count;
            _inner.Add(item);
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);

        }

        public void Clear()
        {
            this.CheckReentrancy();
            _inner.Clear();
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnCollectionReset();
        }

        public bool Contains(T item)
        {
            return _inner.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _inner.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return _inner.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            this.CheckReentrancy();
            int index = this.IndexOf(item);
            if (index < 0) return false;

            _inner.Remove(item);
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
            return true;
        }

        private int IndexOf(T item)
        {
            IList<T> list = _inner as IList<T>;
            if (list != null)
            {
                return list.IndexOf(item);
            }
            else
            {
                return new List<T>(_inner).IndexOf(item);
            }

        }

        public IEnumerator<T> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    internal interface IObserveable
    {
        void RegisterObserver(IObserver observer);
        void UnregisterObserver(IObserver observer);
    }
    internal interface IObserver
    {
        void NotifyPropertyChanged(IEnumerable<string> property);
    }

    public class MyConstantExpression : Expression
    {
        ConstantExpression _expression;
        public MyConstantExpression(ConstantExpression expression)
        {
            _expression = expression;
        }

        public override ExpressionType NodeType
        {
            get
            {
                return  ExpressionType.Constant;
            }
        }

        public override Expression Reduce()
        {
            return _expression.Reduce();
        }

        public override bool CanReduce
        {
            get
            {
                return true;
            }
        }

        public override Type Type
        {
            get
            {
                return typeof(int);// _expression.Type;
            }
        }
    }

    public class MyParameterReplacer : ExpressionVisitor
    {
        ParameterExpression _parameter;
        public MyParameterReplacer(ParameterExpression parameter)
        {
            _parameter = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _parameter;
        }
    }

    public class MyVisitor<T> : ExpressionVisitor
    {
        List<string> _affectedProperties = new List<string>();
        ParameterExpression _parameter;
        bool _initialized = false;

        /// <summary>
        /// Gets AffectedProperties
        /// </summary>
        public string[] AffectedProperties
        {
            get { return _affectedProperties.ToArray(); }
        }

        public override Expression Visit(Expression node)
        {
            if (!_initialized)
            {
                LambdaExpression lambda = (LambdaExpression)node;
                _parameter = lambda.Parameters[0];
                _initialized = true;
            }
            return base.Visit(node);
        }


        protected override Expression VisitMember(MemberExpression node)
        {
            Expression result = base.VisitMember(node);
            if (object.ReferenceEquals(node.Expression, _parameter))
            {
                if (node.Member is PropertyInfo)
                {
                    _affectedProperties.Add(node.Member.Name);
                }
            }
            return result;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Convert
                && typeof(IGetter<T>).IsAssignableFrom(node.Operand.Type))
            {
                var lambda = Expression.Lambda(node.Operand).Compile();
                var getter = (IGetter<T>)lambda.DynamicInvoke();
                MyParameterReplacer parameterReplacer = new MyParameterReplacer(_parameter);
                var getterExpression = parameterReplacer.Visit(getter.Expression.Body);
                return Visit(getterExpression);
            }
            return base.VisitUnary(node);
        }
    }

    public interface IWrapper<T>
    {
        Getter<T, R> CreateGetter<R>(string name, Expression<Func<T, R>> formula);
        IWrapper<T> CreateSetter<R>(string name, Expression<Action<T, R>> formula);
    }

    public interface IGetter<T>
    {
        object Invoke(T value);
        LambdaExpression Expression {get;}
    }

    public class Test
    {
        int _a;
        int _b;
        int _c;

        /// <summary>
        /// Gets or Sets A
        /// </summary>
        public int A
        {
            get { return _a; }
            set { _a = value; }
        }

        /// <summary>
        /// Gets or Sets B
        /// </summary>
        public int B
        {
            get { return _b; }
            set { _b = value; }
        }

        /// <summary>
        /// Gets or Sets C
        /// </summary>
        public int C
        {
            get { return _c; }
            set { _c = value; }
        }

        public static void Lala()
        {
            var innerTest = new Test();
            innerTest.A = 3;
            innerTest.B = 5;
            innerTest.C = 7;
            var test = new MyDynamic<Test>(innerTest);
            var sumAB = test.CreateGetter("SumAB", t => t.A + t.B);

            var ac = Expression.Constant(sumAB);
            var aa = Expression.Lambda(ac);
            



            test.CreateGetter("SumAll", t => t.C + (sumAB * sumAB * (sumAB + 1)));
            int a = ((dynamic)test).SumAll;


        }
    }
}
