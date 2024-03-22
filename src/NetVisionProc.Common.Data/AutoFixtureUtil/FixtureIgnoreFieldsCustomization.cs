namespace NetVisionProc.Common.Data.AutoFixtureUtil
{
    public class FixtureIgnoreFieldsCustomization<T, TProperty>(Expression<Func<T, TProperty>> withoutPropertyPicker) : ICustomization
    where T : class
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<T>(c => c
                .Without(withoutPropertyPicker));
        }
    }
}