// MIT License
// 
// Copyright (c) 2020 Alexey Politov
// https://github.com/EmptyBucket/Modules
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using FluentAssertions;
using NUnit.Framework;

namespace Modules.Tests;

public class ServiceCollectionExtensionsTests
{
    [Test]
    public void KvTarjan_WhenLinearPath_ReturnReverseSequence()
    {
        var dependencies = new Dictionary<int, int[]>
        {
            { 1, new[] { 2 } }, { 2, new[] { 3 } }, { 3, Array.Empty<int>() }
        };

        var kvTarjan = ServiceCollectionExtensions.KvTarjan(1, k => k, k => (dependencies)[k]);

        kvTarjan.Should().BeEquivalentTo(new[] { 3, 2, 1 });
    }

    [Test]
    public void KvTarjan_WhenSharedPath_ReturnUniqueReverseSequence()
    {
        var dependencies = new Dictionary<int, int[]>
        {
            { 1, new[] { 2, 3 } }, { 2, new[] { 3 } }, { 3, Array.Empty<int>() }
        };

        var kvTarjan = ServiceCollectionExtensions.KvTarjan(1, k => k, k => (dependencies)[k]);

        kvTarjan.Should().BeEquivalentTo(new[] { 3, 2, 1 });
    }

    [Test]
    public void KvTarjan_WhenCyclePath_ThrowException()
    {
        var dependencies = new Dictionary<int, int[]>
        {
            { 1, new[] { 2 } }, { 2, new[] { 3 } }, { 3, new[] { 1 } }
        };

        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        Action act = () => ServiceCollectionExtensions.KvTarjan(1, k => k, k => (dependencies)[k]).ToArray();

        act.Should().Throw<InvalidOperationException>().WithMessage("Cycle detected");
    }

    [Test]
    public void KvTarjan_WhenComplexPath_ReturnTopologicalOrder()
    {
        var dependencies = new Dictionary<int, int[]>
        {
            { 1, new[] { 2, 3 } },
            { 2, new[] { 4, 5, 6 } },
            { 3, new[] { 7, 8, 9 } },
            { 4, new[] { 10, 14 } },
            { 5, new[] { 11 } },
            { 6, new[] { 12 } },
            { 7, new[] { 10 } },
            { 8, new[] { 11 } },
            { 9, new[] { 12, 16 } },
            { 10, new[] { 13 } },
            { 11, new[] { 13 } },
            { 12, new[] { 13 } },
            { 13, new[] { 15 } },
            { 14, Array.Empty<int>() },
            { 15, Array.Empty<int>() },
            { 16, Array.Empty<int>() }
        };

        var kvTarjan = ServiceCollectionExtensions.KvTarjan(1, k => k, k => (dependencies)[k]);

        kvTarjan.Should().BeEquivalentTo(new[] { 14, 15, 16, 13, 10, 11, 12, 4, 5, 6, 7, 8, 9, 2, 3, 1 });
    }
}
