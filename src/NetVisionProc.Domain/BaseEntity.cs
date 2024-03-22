using NetVisionProc.Adapter;

namespace NetVisionProc.Domain
{
    public abstract class BaseEntity : IBaseEntity
    {
        [Sieve(CanSort = true)]
        public virtual int Id { get; set; } // originally: protected set

        public static bool operator ==(BaseEntity a, BaseEntity b)
        {
            if (a is null && b is null)
            {
                return true;
            }

            if (a is null || b is null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(BaseEntity? a, BaseEntity? b)
        {
            if (a is null && b is not null)
            {
                return false;
            }

            if (b is null && a is not null)
            {
                return false;
            }

            return !(a! == b!);
        }

        public virtual bool IsExistingEntity()
        {
            return Id > 0;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not BaseEntity other)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetUnproxiedType(this) != GetUnproxiedType(other))
            {
                return false;
            }

            if (Id.Equals(default) || other.Id.Equals(default))
            {
                return false;
            }

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return (GetUnproxiedType(this).ToString() + Id).GetHashCode();
        }

        internal static Type GetUnproxiedType(object obj)
        {
            const string EFCoreProxyPrefix = "Castle.Proxies.";

            Type type = obj.GetType();
            string typeString = type.ToString();

            if (typeString.Contains(EFCoreProxyPrefix))
            {
                return type.BaseType ?? type;
            }

            return type;
        }
    }
}
