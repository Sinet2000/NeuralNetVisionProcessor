namespace NetVisionProc.Common.Data.AutoFixtureUtil
{
    public static class AutoFixtureExt
    {
        public static TReturn CreateMapped<TSource, TReturn>(this Fixture fixture, IMapper mapper)
        {
            return mapper.Map<TReturn>(fixture.Create<TSource>());
        }
        
        public static TReturn CreateMapped<TReturn>(this Fixture fixture, IMapper mapper)
        {
            return mapper.Map<TReturn>(fixture.Create<TReturn>());
        }

        public static (T1, T2) CreateMappedAsTuple<T1, T2>(this Fixture fixture, IMapper mapper)
        {
            return (
                fixture.CreateMapped<T1>(mapper),
                fixture.CreateMapped<T2>(mapper));
        }
        
        public static (T1, T2, T3) CreateMappedAsTuple<T1, T2, T3>(this Fixture fixture, IMapper mapper)
        {
            return (
                fixture.CreateMapped<T1>(mapper),
                fixture.CreateMapped<T2>(mapper),
                fixture.CreateMapped<T3>(mapper));
        }
        
        public static (T1, T2, T3, T4) CreateMappedAsTuple<T1, T2, T3, T4>(this Fixture fixture, IMapper mapper)
        {
            return (
                fixture.CreateMapped<T1>(mapper),
                fixture.CreateMapped<T2>(mapper),
                fixture.CreateMapped<T3>(mapper),
                fixture.CreateMapped<T4>(mapper));
        }
        
        public static (T1, T2, T3, T4, T5) CreateMappedAsTuple<T1, T2, T3, T4, T5>(this Fixture fixture, IMapper mapper)
        {
            return (
                fixture.CreateMapped<T1>(mapper),
                fixture.CreateMapped<T2>(mapper),
                fixture.CreateMapped<T3>(mapper),
                fixture.CreateMapped<T4>(mapper),
                fixture.CreateMapped<T5>(mapper));
        }

        public static (T1, T2, T3, T4, T5, T6) CreateMappedAsTuple<T1, T2, T3, T4, T5, T6>(this Fixture fixture, IMapper mapper)
        {
            return (
                fixture.CreateMapped<T1>(mapper),
                fixture.CreateMapped<T2>(mapper),
                fixture.CreateMapped<T3>(mapper),
                fixture.CreateMapped<T4>(mapper),
                fixture.CreateMapped<T5>(mapper),
                fixture.CreateMapped<T6>(mapper));
        }
        
        public static (T1, T2, T3, T4, T5, T6, T7) CreateMappedAsTuple<T1, T2, T3, T4, T5, T6, T7>(this Fixture fixture, IMapper mapper)
        {
            return (
                fixture.CreateMapped<T1>(mapper),
                fixture.CreateMapped<T2>(mapper),
                fixture.CreateMapped<T3>(mapper),
                fixture.CreateMapped<T4>(mapper),
                fixture.CreateMapped<T5>(mapper),
                fixture.CreateMapped<T6>(mapper),
                fixture.CreateMapped<T7>(mapper));
        }
        
        public static (T1, T2, T3, T4, T5, T6, T7, T8) CreateMappedAsTuple<T1, T2, T3, T4, T5, T6, T7, T8>(this Fixture fixture, IMapper mapper)
        {
            return (
                fixture.CreateMapped<T1>(mapper),
                fixture.CreateMapped<T2>(mapper),
                fixture.CreateMapped<T3>(mapper),
                fixture.CreateMapped<T4>(mapper),
                fixture.CreateMapped<T5>(mapper),
                fixture.CreateMapped<T6>(mapper),
                fixture.CreateMapped<T7>(mapper),
                fixture.CreateMapped<T8>(mapper));
        }
    }
}