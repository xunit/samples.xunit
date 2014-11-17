using System.Threading;
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
        public static void STAFact_OnSTAThread()
        {
            Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
        }

        [STATheory]
        [InlineData(0)]
        public static void STATheory_OnSTAThread(int unused)
        {
            Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
        }
    }
}
