Namespace Parallelization Sample

This sample project arose from a [discussion question](https://github.com/xunit/xunit/discussions/2924) about whether it would be possible for xUnit.net to be able to significantly change the way it does parallelization.

The requirements that have been implemented here are as follows:

- Test cases are grouped by the namespace of the test class (one "test collection" per namespace).
- Namespaces are run sequentially with each other.
- Within a namespace, all tests (across all classes) are run in parallel.
- Rather than supporting the traditional fixture interfaces, the model allows the developer to create a "setup" class by convention: a type in the same namespace as the test class named `Setup` will become a fixture for all the tests in that namespace (since instance shared amongst all classes). Additionally, it can take as a constructor argument a "parent setup" class which effectively becomes an assembly-level fixture, a single instance of which is shared amongst all `Setup` classes in all namespaces.

Note that, as with all samples here, this code is written without tests, and only validated with the provided sample. Using it in production should be done with caution, either requiring extensive edge case testing and/or creating unit tests around the new types.
