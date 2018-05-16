namespace CustomCode.Core.Composition.Tests
{
    using Reflection;
    using System;
    using System.Reflection;
    using Test.BehaviorDrivenDevelopment;
    using Xunit;

    /// <summary>
    /// Test the "reflection" capatibilities of the portable executable parser (since .Net Standard doesn't support
    /// AppDomains and ReflectionOnlyLoad currently).
    /// </summary>
    [IntegrationTest]
    public sealed class SlimPortableExecutableTest : TestCase
    {
        [Fact(DisplayName = "Test that this assembly is parsed as valid pe file")]
        public void ValidPortableExecutableTest()
        {
            Given(() =>
                {
                    var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    var uri = new UriBuilder(codeBase);
                    var path = Uri.UnescapeDataString(uri.Path);
                    return new SlimPortableExecutable(path);
                })
            .When(file => file.IsValidPortableExecutable())
            .Then(result => result.Should().BeTrue());
        }

        [Fact(DisplayName = "Test that this assembly is parsed as valid .net assembly")]
        public void ValidNetAssemblyTest()
        {
            Given(() =>
                {
                    var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    var uri = new UriBuilder(codeBase);
                    var path = Uri.UnescapeDataString(uri.Path);
                    return new SlimPortableExecutable(path);
                })
            .When(file => file.IsValidNetAssembly())
            .Then(result => result.Should().BeTrue());
        }

        [Fact(DisplayName = "Test that this assembly is parsed as valid .net assembly with a defined IocVisibleAssemblyAttribute")]
        public void ValidIocVisibleAssemblyTest()
        {
            Given(() =>
                {
                    var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    var uri = new UriBuilder(codeBase);
                    var path = Uri.UnescapeDataString(uri.Path);
                    return new SlimPortableExecutable(path);
                })
            .When(file => file.IsIocVisibleAssembly())
            .Then(result => result.Should().BeTrue());
        }

        [Fact(DisplayName = "Test that this assembly is parsed as valid .net assembly with no defined IocVisibleAssemblyAttribute")]
        public void InvalidIocVisibleAssemblyTest()
        {
            Given(() =>
                {
                    var codeBase = typeof(IocVisibleAssemblyAttribute).GetTypeInfo().Assembly.CodeBase;
                    var uri = new UriBuilder(codeBase);
                    var path = Uri.UnescapeDataString(uri.Path);
                    return new SlimPortableExecutable(path);
                })
            .When(file =>
                {
                    var isNetAssembly = file.IsValidNetAssembly();
                    var isValid = file.IsIocVisibleAssembly();
                    return (isNetAssembly, isValid);
                })
            .Then(result =>
                {
                    result.isNetAssembly.Should().BeTrue();
                    result.isValid.Should().BeFalse();
                });
        }
    }
}