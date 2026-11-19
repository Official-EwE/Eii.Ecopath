// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).

using System.Collections.Concurrent;
using EwEUtils.Utilities;
using FluentAssertions;

namespace EwECore.Tests.Utilities;

public sealed class cFileUtilsTempFileTests
{
    [Fact]
    public void MakeAndPurgeTempFile_FromManyThreads_NeverThrowsAndLeavesNoFiles()
    {
        // Arrange
        const int threads = 16;
        const int perThread = 200;
        var files = new ConcurrentBag<string>();
        var errors = new ConcurrentBag<Exception>();

        // Act
        Parallel.For(0, threads, new ParallelOptions { MaxDegreeOfParallelism = threads }, _ =>
        {
            try
            {
                for (var i = 0; i < perThread; i++)
                {
                    var file = cFileUtils.MakeTempFile(".ewetmp");
                    files.Add(file);
                    File.WriteAllText(file, "x");
                    cFileUtils.PurgeTempFile(file);
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
        });

        // Assert
        errors.Should().BeEmpty();
        files.Should().HaveCount(threads * perThread);
        files.Distinct().Should().HaveCount(threads * perThread);
        files.Should().OnlyContain(f => !File.Exists(f));
    }

    [Fact]
    public async Task PurgeTempFiles_WhileOtherThreadsCreateFiles_DoesNotThrow()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var errors = new ConcurrentBag<Exception>();
        var creators = Enumerable.Range(0, 8).Select(_ => Task.Run(() =>
        {
            try
            {
                while (!cts.IsCancellationRequested)
                    cFileUtils.PurgeTempFile(cFileUtils.MakeTempFile(".ewetmp"));
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
        })).ToArray();

        // Act
        for (var i = 0; i < 500; i++)
        {
            try
            {
                cFileUtils.PurgeTempFiles();
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
        }
        cts.Cancel();
        await Task.WhenAll(creators);
        cFileUtils.PurgeTempFiles();

        // Assert
        errors.Should().BeEmpty();
    }
}
