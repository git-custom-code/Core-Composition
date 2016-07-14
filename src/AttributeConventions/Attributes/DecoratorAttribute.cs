namespace LightInject.AttributeConventions
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public /*abstract*/ class DecoratorAttribute : Attribute
    {
    }
}