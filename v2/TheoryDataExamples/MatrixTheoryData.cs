using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Xunit;

namespace TheoryDataExamples
{
    public class MatrixTheoryData<T1, T2> : TheoryData<T1, T2>
    {
        public MatrixTheoryData(IEnumerable<T1> data1, IEnumerable<T2> data2)
        {
            Contract.Assert(data1 != null && data1.Any());
            Contract.Assert(data2 != null && data2.Any());

            foreach (T1 t1 in data1)
                foreach (T2 t2 in data2)
                    Add(t1, t2);
        }
    }

    public class SampleUsage
    {
        public static int[] Numbers = { 42, 5, 6 };
        public static string[] Strings = { "Hello", "world!" };
        public static MatrixTheoryData<string, int> MatrixData = new MatrixTheoryData<string, int>(Strings, Numbers);

        [Theory]
        [MemberData(nameof(MatrixData))]
        public void MyTestMethod(string x, int y)
        {
            Assert.Equal(y, x.Length);
        }
    }
}
