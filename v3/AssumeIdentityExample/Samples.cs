using System.Security;
using System.Security.Permissions;
using System.Threading;
using Xunit;

// Note: This sample is only built for .NET Framework, as PrincipalPermission is not part of .NET.

public class Samples
{
    [Fact, AssumeIdentity("editor")]
    public static void AttributeChangesRoleInTestMethod()
    {
        Assert.True(Thread.CurrentPrincipal.IsInRole("editor"));
    }

    [Fact]
    public static void CallingSecuredMethodWillThrow()
    {
        Assert.Throws<SecurityException>(DefeatVillian);
    }

    [Fact, AssumeIdentity("editor")]
    public static void CallingSecuredMethodWithWrongIdentityWillThrow()
    {
        Assert.Throws<SecurityException>(DefeatVillian);
    }

    [Fact, AssumeIdentity("superuser")]
    public static void CallingSecuredMethodWithAssumedIdentityPasses()
    {
        DefeatVillian();
    }

    [PrincipalPermission(SecurityAction.Demand, Role = "superuser")]
    static void DefeatVillian() { }
}
