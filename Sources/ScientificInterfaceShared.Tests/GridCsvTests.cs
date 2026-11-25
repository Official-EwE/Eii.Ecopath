using System;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using FluentAssertions;
using ScientificInterfaceShared.Controls.EwEGrid;
using ScientificInterfaceShared.Style;
using SourceGrid2;
using Xunit;

public class GridCsvTests
{
    private sealed class TestGrid : cEwEGrid
    {
        public override bool SuppressQuickEdits => false;
        protected override void FillData() { }
    }

    /// <summary>Runs a grid test on a single-threaded apartment required by WinForms.</summary>
    /// <param name="action">Test action to run on the STA thread.</param>
    /// <remarks>Waits for the thread and rethrows any exception on the caller with its original stack trace.</remarks>
    private static void RunSta(Action action)
    {
        Exception failure = null;
        var thread = new Thread(() =>
        {
            try { action(); }
            catch (Exception ex) { failure = ex; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (failure != null) ExceptionDispatchInfo.Capture(failure).Throw();
    }

    /// <summary>Creates a grid fixture with a fixed header, editable cells and a hidden column.</summary>
    /// <returns>A grid owned by the caller, which must dispose it after the test.</returns>
    /// <remarks>The data row contains a string and floating-point values, including a value with more than four decimal places.</remarks>
    private static TestGrid CreateGrid()
    {
        var grid = new TestGrid();
        grid.Redim(2, 4);
        grid.FixedRows = 1;
        grid[0, 0] = new cEwEColumnHeaderCell("Name");
        grid[0, 1] = new cEwEColumnHeaderCell("Hidden");
        grid[0, 2] = new cEwEColumnHeaderCell("Value");
        grid[0, 3] = new cEwEColumnHeaderCell("Other");
        grid.Columns[1].Visible = false;
        grid[1, 0] = new cEwECell("initial", typeof(string), cStyleGuide.eStyleFlags.OK);
        grid[1, 1] = new cEwECell(44f, typeof(float), cStyleGuide.eStyleFlags.OK);
        grid[1, 2] = new cEwECell(0.08001234f, typeof(float), cStyleGuide.eStyleFlags.OK);
        grid[1, 3] = new cEwECell(0f, typeof(float), cStyleGuide.eStyleFlags.OK);
        for (int i = 0; i < 4; i++)
        {
            grid[1, i].DataModel.EnableEdit = true;
            grid[1, i].DataModel.EditableMode = EditableMode.SingleClick;
        }
        return grid;
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("nl-NL")]
    public void ExportAndImportRoundTripAcrossCultures(string culture)
    {
        // Arrange (from InlineData)
        RunSta(() =>
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            using (var grid = CreateGrid())
            {
                grid[1, 0].Value = "A, \"quoted\"\nname";
                var csv = new cEwEGridCsv(grid);
                var output = new StringWriter();
                string error = null;
                bool changes = false;
                float expected = (float)grid[1, 2].Value;

                // Act & Assert: export must succeed before importing its output.
                csv.Write(output, ref error).Should().BeTrue("export failed: {0}", error);
                string text = output.ToString();
                grid[1, 0].Value = "changed";
                grid[1, 2].Value = 7f;
                bool success = csv.Read(new StringReader(text), ref error, ref changes);

                // Assert
                success.Should().BeTrue("import failed: {0}", error);
                text.Should().Contain("0.08001234");
                grid[1, 0].Value.Should().Be("A, \"quoted\"\nname");
                ((float)grid[1, 2].Value).Should().Be(expected);
                ((float)grid[1, 1].Value).Should().Be(44f);
                ((float)grid[1, 3].Value).Should().Be(0f);
            }
        });
    }

    [Fact]
    public void InvalidNumberDoesNotChangeEarlierCells()
    {
        // Arrange
        RunSta(() =>
        {
            using (var grid = CreateGrid())
            {
                var csv = new cEwEGridCsv(grid);
                string error = null;
                bool changes = false;

                // Act
                bool success = csv.Read(new StringReader("Name,Value,Other\r\nchanged,2,invalid\r\n"),
                                        ref error, ref changes);

                // Assert
                success.Should().BeFalse();
                changes.Should().BeFalse();
                error.Should().Contain("row 2, column 3");
                grid[1, 0].Value.Should().Be("initial");
                ((float)grid[1, 2].Value).Should().Be(0.08001234f);
            }
        });
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("nl-NL")]
    public void DecimalCommaIsRejectedWithoutChangingCells(string culture)
    {
        // Arrange (from InlineData)
        RunSta(() =>
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            using (var grid = CreateGrid())
            {
                var csv = new cEwEGridCsv(grid);
                const string text = "Name,Value,Other\r\nchanged,\"0,0800\",0\r\n";
                string error = null;
                bool changes = false;

                // Act
                bool success = csv.Read(new StringReader(text), ref error, ref changes);

                // Assert
                success.Should().BeFalse();
                changes.Should().BeFalse();
                error.Should().Contain("row 2, column 2");
                grid[1, 0].Value.Should().Be("initial");
                ((float)grid[1, 2].Value).Should().Be(0.08001234f);
            }
        });
    }

    [Fact]
    public void MismatchedLayoutDoesNotChangeCells()
    {
        // Arrange
        RunSta(() =>
        {
            using (var grid = CreateGrid())
            {
                var csv = new cEwEGridCsv(grid);
                string error = null;
                bool changes = false;

                // Act
                bool success = csv.Read(new StringReader("Name,Other,Value\r\nchanged,2,3\r\n"),
                                        ref error, ref changes);

                // Assert
                success.Should().BeFalse();
                changes.Should().BeFalse();
                grid[1, 0].Value.Should().Be("initial");
            }
        });
    }

    [Fact]
    public void ReorderedRowHeaderIsRejectedBeforeApplyingValues()
    {
        // Arrange
        RunSta(() =>
        {
            using (var grid = CreateGrid())
            {
                grid.FixedColumns = 1;
                grid[1, 0] = new cEwERowHeaderCell("Whales");
                var csv = new cEwEGridCsv(grid);
                string error = null;
                bool changes = false;

                // Act
                bool success = csv.Read(new StringReader("Name,Value,Other\r\nSeals,2,3\r\n"),
                                        ref error, ref changes);

                // Assert
                success.Should().BeFalse();
                changes.Should().BeFalse();
                ((float)grid[1, 2].Value).Should().Be(0.08001234f);
            }
        });
    }

    [Fact]
    public void MalformedQuotedFieldIsRejectedBeforeApplyingValues()
    {
        // Arrange
        RunSta(() =>
        {
            using (var grid = CreateGrid())
            {
                var csv = new cEwEGridCsv(grid);
                string error = null;
                bool changes = false;

                // Act
                bool success = csv.Read(new StringReader("Name,Value,Other\r\n\"unterminated,2,3"),
                                        ref error, ref changes);

                // Assert
                success.Should().BeFalse();
                changes.Should().BeFalse();
                grid[1, 0].Value.Should().Be("initial");
            }
        });
    }

    [Theory]
    [InlineData(";")]
    [InlineData("\t")]
    public void AlternateDelimitersAndBlankNumericFieldsAreSupported(string delimiter)
    {
        // Arrange (from InlineData)
        RunSta(() =>
        {
            using (var grid = CreateGrid())
            {
                var csv = new cEwEGridCsv(grid);
                string error = null;
                bool changes = false;
                string text = string.Join(delimiter, "Name", "Value", "Other") + "\r\n" +
                              string.Join(delimiter, "initial", "", "0") + "\r\n";

                // Act
                bool success = csv.Read(new StringReader(text), ref error, ref changes);

                // Assert
                success.Should().BeTrue("import failed: {0}", error);
                ((float)grid[1, 2].Value).Should().Be((float)EwECore.cCore.NULL_VALUE);
                ((float)grid[1, 3].Value).Should().Be(0f);
            }
        });
    }
}
