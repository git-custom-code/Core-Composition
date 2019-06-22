#region Disclaimer

/*********************************************************************************

    This is a slightly modified copy of Bernhard Richter's light inject
    dependency injection repository, since at the time of creating this
    code, said repo was a) not signed and b) had no support for the new
    .Net core generic host.
    Once those two features are implemented by the author, this copy
    can be safely removed.

******************************************************************************
    The MIT License (MIT)
    Copyright (c) 2018 bernhard.richter@gmail.com
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
******************************************************************************
    LightInject.Microsoft.DependencyInjection version 2.2.0
    http://www.lightinject.net/
    http://twitter.com/bernhardrichter
******************************************************************************/

#endregion

namespace CustomCode.Core.Composition.Hosting.LightInject
{
    using global::LightInject;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// An <see cref="IServiceScopeFactory"/> that uses an <see cref="IServiceContainer"/> to create new scopes.
    /// </summary>
    internal class LightInjectServiceScopeFactory : IServiceScopeFactory
    {
        #region Dependencies

        /// <summary>
        /// Initializes a new instance of the <see cref="LightInjectServiceScopeFactory"/> class.
        /// </summary>
        /// <param name="container">The <see cref="IServiceContainer"/> used to create new scopes.</param>
        public LightInjectServiceScopeFactory(IServiceContainer container)
        {
            Container = container;
        }

        private IServiceContainer Container { get; }

        #endregion

        #region Logic

        /// <inheritdoc/>
        public IServiceScope CreateScope()
        {
            var scope = Container.BeginScope();
            return new LightInjectServiceScope(scope);
        }

        #endregion
    }
}