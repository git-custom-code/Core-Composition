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
        public void Test()
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
    }
}