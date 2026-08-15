using Xunit.Sdk;
using Xunit.v3;

// Need to turn off test parallelization so we can validate the run order in the examples
[assembly: Parallelization(Mode = ParallelMode.None)]
