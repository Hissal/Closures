using Closures;

namespace ClosureTests.Anonymous.Other;

[TestFixture]
[TestOf(typeof(Result))]
[TestOf(typeof(Result<>))]
public class ResultTest {
    [Test]
    public void Result_Success_CreatesSuccessResult() {
        var result = Result.Success();
        
        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Exception, Is.Null);
            Assert.That((bool)result, Is.True);
        });
    }

    [Test]
    public void Result_Failure_CreatesFailureResult() {
        var ex = new InvalidOperationException("fail");
        var result = Result.Failure(ex);
        
        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Exception, Is.EqualTo(ex));
            Assert.That((bool)result, Is.False);
        });
    }

    [Test]
    public void ResultT_Success_CreatesSuccessResult() {
        var value = 42;
        var result = Result<int>.Success(value);
        
        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(value));
            Assert.That(result.Exception, Is.Null);
            Assert.That((bool)result, Is.True);
        });
    }

    [Test]
    public void ResultT_Failure_CreatesFailureResult() {
        var ex = new ArgumentException("fail");
        var result = Result<string>.Failure(ex);
        
        Assert.Multiple(() => {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Value, Is.Null);
            Assert.That(result.Exception, Is.EqualTo(ex));
            Assert.That((bool)result, Is.False);
        });
    }
}