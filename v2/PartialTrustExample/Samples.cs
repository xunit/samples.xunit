using System;
using System.Security;
using System.Security.Permissions;
using Xunit;

namespace PartialTrustExample
{
    // Test class has to be MarshalByRefObject, because it's created in the partial trust app domain sandbox.
    // The PartialTrustTestInvoker catches serialization issues and lets the user know they forgot to use
    // MarshalByRefObject (as well as cleaning up the sandbox).
    public class Samples : MarshalByRefObject
    {
        [Fact]
        public void Passing()
        {
            DemandFullTrust();
        }

        [PartialTrustFact]
        public void Failing()
        {
            DemandFullTrust();
        }

        static void DemandFullTrust()
        {
            // This demands full trust. It should pass from an full trust app domain, and
            // fail from a partial trust domain.
            new PermissionSet(PermissionState.Unrestricted).Demand();
        }
    }
}
