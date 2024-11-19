This sample is intended to show how to create a completely new test framework, while using
the existing xUnit.net runners to perform test execution.

It overrides the entire pipeline while reusing much of the existing infrastructure, to allow
users to define their own concept of what it means to write a test.

This is considered a fairly advanced sample, because of the level of extensibility that is
exploited to perform the work.
