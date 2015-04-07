using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace STAExamples
{
    public class Samples
    {
        [Fact]
        public static void Fact_OnMTAThread()
        {
            Assert.Equal(ApartmentState.MTA, Thread.CurrentThread.GetApartmentState());
        }

        [STAFact]
        public static async Task STAFact_OnSTAThread()
        {
            Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
            await Task.Yield();
            Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState()); // still there
        }

        [STATheory]
        [InlineData(0)]
        public static async Task STATheory_OnSTAThread(int unused)
        {
            Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
            await Task.Yield();
            Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState()); // still there
        }
    }
}
