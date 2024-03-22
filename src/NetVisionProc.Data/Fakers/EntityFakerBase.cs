using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Bogus;

namespace NetVisionProc.Data.Fakers
{
    /// <summary>
    /// Base class for fluent fakers used to generate fake instances of entities.
    /// </summary>
    /// <typeparam name="T">The type of entity to generate.</typeparam>
    /// <typeparam name="TFaker">The type of the fluent faker class.</typeparam>
    /// <remarks>Create an instance using a PRIVATE parameterless constructor.</remarks>
    public abstract class EntityFakerBase<T, TFaker> : Faker<T>
        where T : class
        where TFaker : EntityFakerBase<T, TFaker>
    {
        // Cache for storing constructor information
        private static readonly ConcurrentDictionary<Type, Func<T>> ConstructorCache = new();

        protected int ScopedIndex { get; set; }

        /// <summary>
        /// Generates a fake instance of the entity with optional customization.
        /// </summary>
        /// <param name="faker">Action to customize the fluent faker instance.</param>
        /// <returns>A fake instance of the entity.</returns>
        public virtual T Generate(Action<TFaker>? faker = null)
        {
            BeforeGenerate();

            // Invoke customizations on the fluent faker instance
            faker?.Invoke((TFaker)this);

            // Create an instance of the entity using the constructor
            var instance = CreateInstance();

            // Populate the entity with fake data
            PopulateInternal(instance, new[] { Default });

            AfterGenerate();

            return instance;
        }

        public IEnumerable<T> Generate(int count, Action<TFaker>? faker = null)
        {
            return Enumerable.Range(0, count)
                .Select(_ => Generate(faker));
        }

        /// <summary>
        /// Executes actions before generating a fake object.
        /// </summary>
        protected virtual void BeforeGenerate()
        {
            ScopedIndex++;
            FakerHub = new Bogus.Faker()
            {
                Lorem =
                {
                    Locale = "en"
                }
            };

            // Apply default rule set
            RuleSet(Default, DefaultRuleSet);
        }

        /// <summary>
        /// Defines the default rule set for the entity.
        /// </summary>
        /// <param name="ruleSet">The rule set to define rules on.</param>
        protected abstract void DefaultRuleSet(IRuleSet<T> ruleSet);

        /// <summary>
        /// Actions to perform after generating a fake object.
        /// Useful for cleanup or additional customization.
        /// </summary>
        protected virtual void AfterGenerate()
        {
            // Optionally implement cleanup or additional actions here
        }

        private static Func<T> GetConstructorFunction()
        {
            var type = typeof(T);
            return ConstructorCache.GetOrAdd(type, LoadConstructorFunction);
        }

        private static Func<T> LoadConstructorFunction(Type type)
        {
            var ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

            if (ctor is not null)
            {
                return Expression.Lambda<Func<T>>(Expression.New(ctor)).Compile();
            }

            return Expression.Lambda<Func<T>>(Expression.Convert(
                Expression.Call(typeof(Activator), "CreateInstance", [typeof(T)]), typeof(T))).Compile();
        }

        private static T CreateInstance()
        {
            var createInstance = GetConstructorFunction();
            return createInstance.Invoke();
        }

        /*
         * private static ConstructorInfo GetConstructorInfo()
        {
            var type = typeof(T);
            return _cache.GetOrAdd(type, LoadConstructorInfo);
        }

        private static ConstructorInfo LoadConstructorInfo(Type type)
        {
            var ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            ctor ??= type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);

            Guard.Against.Null(ctor);

            return ctor;
        }
         */
    }
}
