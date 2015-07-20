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

        [WpfFact]
        public static async void WpfFact_OnSTAThread()
        {
            Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
            await Task.Yield();
            Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState()); // still there
        }

        [WpfTheory]
        [InlineData(0)]
        public static async void WpfTheory_OnSTAThread(int unused)
        {
            Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
            await Task.Yield();
            Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState()); // still there
        }
    }
}
