using FluentAssertions;
using Rochas.Extensions;

namespace Rochas.Extensions.Tests;

public class ExceptionExtensionTests
{
    [Fact]
    public void GetResume_SimpleException_ReturnsMessageAndTrace()
    {
        var ex = new InvalidOperationException("Something failed");
        try { throw ex; } catch (Exception caught) { ex = (InvalidOperationException)caught; }

        var resume = ex.GetResume();

        resume.Should().NotBeNull();
        resume.Message.Should().Be("Something failed");
        resume.Trace.Should().NotBeNullOrEmpty();
        resume.ChildMessage.Should().BeNull();
        resume.ChildTrace.Should().BeNull();
    }

    [Fact]
    public void GetResume_InnerException_ReturnsChildDetails()
    {
        var inner = new ArgumentException("Inner problem");
        var outer = new InvalidOperationException("Outer failure", inner);
        try { throw outer; } catch (Exception caught) { outer = (InvalidOperationException)caught; }

        var resume = outer.GetResume();

        resume.Message.Should().Be("Outer failure");
        resume.ChildMessage.Should().Be("Inner problem");
    }

    [Fact]
    public void GetResume_NestedExceptions_ReturnsImmediateChild()
    {
        var innermost = new ArgumentException("deep");
        var middle = new InvalidOperationException("middle", innermost);
        var outer = new ApplicationException("top", middle);

        var resume = outer.GetResume();

        resume.Message.Should().Be("top");
        resume.ChildMessage.Should().Be("middle");
    }

    [Fact]
    public void GetResume_ReturnsExceptionResumeType()
    {
        var ex = new Exception("test");

        var resume = ex.GetResume();

        resume.Should().BeOfType<Rochas.Extensions.ValueObjects.ExceptionResume>();
    }
}
