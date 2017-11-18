namespace CustomCode.Core.Composition.Test
{
    using Reflection;
    using System;
    using System.Reflection;
    using Xunit;

    /// <summary>
    /// Test the "reflection" capatibilities of the portable executable parser (since .Net Standard doesn't support
    /// AppDomains and ReflectionOnlyLoad currently).
    /// </summary>
    public sealed class SlimPortableExecutableTest
    {
        [Fact(DisplayName = "Test that this assembly is parsed as valid pe file")]
        [Trait("Category", "IntegrationTest")]
        public void ValidPortableExecutableTest()
        {
            // Given
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            var file = new SlimPortableExecutable(path);

            // When
            var isValid = file.IsValidPortableExecutable();

            // Then
            Assert.True(isValid);
        }

        [Fact(DisplayName = "Test that this assembly is parsed as valid .net assembly")]
        [Trait("Category", "IntegrationTest")]
        public void ValidNetAssemblyTest()
        {
            // Given
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            var file = new SlimPortableExecutable(path);

            // When
            var isValid = file.IsValidNetAssembly();

            // Then
            Assert.True(isValid);
        }

        [Fact(DisplayName = "Test that this assembly is parsed as valid .net assembly with a defined IocVisibleAssemblyAttribute")]
        [Trait("Category", "IntegrationTest")]
        public void ValidIocVisibleAssemblyTest()
        {
            // Given
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            var file = new SlimPortableExecutable(path);

            // When
            var isValid = file.IsIocVisibleAssembly();

            // Then
            Assert.True(isValid);
        }

        [Fact(DisplayName = "Test that this assembly is parsed as valid .net assembly with no defined IocVisibleAssemblyAttribute")]
        [Trait("Category", "IntegrationTest")]
        public void InvalidIocVisibleAssemblyTest()
        {
            // Given
            var codeBase = typeof(IocVisibleAssemblyAttribute).GetTypeInfo().Assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            var file = new SlimPortableExecutable(path);

            // When
            var isNetAssembly = file.IsValidNetAssembly();
            var isValid = file.IsIocVisibleAssembly();

            // Then
            Assert.True(isNetAssembly);
            Assert.False(isValid);
        }
    }
}