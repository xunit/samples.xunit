using System;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using Xunit.v3;

/// <summary>
/// Apply this attribute to your test method to replace the <see cref="Thread.CurrentPrincipal"/> with
/// another role.
/// </summary>
/// <param name="roleName">The role name</param>
/// <param name="userName">The user name (defaults to "xUnit")</param>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class AssumeIdentityAttribute(string roleName, string userName = "xUnit") : BeforeAfterTestAttribute
{
    IPrincipal? originalPrincipal;

    public string RoleName { get; } = roleName;

    public string UserName { get; } = userName;

    public override void After(MethodInfo methodUnderTest, IXunitTest test)
    {
        if (originalPrincipal is not null)
            Thread.CurrentPrincipal = originalPrincipal;
    }

    public override void Before(MethodInfo methodUnderTest, IXunitTest test)
    {
        originalPrincipal = Thread.CurrentPrincipal;
        var identity = new GenericIdentity(UserName);
        var principal = new GenericPrincipal(identity, [RoleName]);
        Thread.CurrentPrincipal = principal;
    }
}
