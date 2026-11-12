' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Diagnostics
Imports System.Drawing
Imports System.Threading.Tasks
Imports System.Windows.Forms

Namespace Import

    ''' <summary>
    ''' Code-only (no .resx) modal window standing in for EwEImportHelper.exe's own console window:
    ''' shows its live stdout/stderr in a read-only box that stays open with a Continue button, instead
    ''' of a real console that flashes past progress and closes immediately on exit.
    ''' </summary>
    ''' <remarks>
    ''' Also stands in for stdin: typing into the (normally hidden) input box and pressing Enter sends
    ''' the line to the process, exactly like a real console - EwEImportHelper's Program.vb needs no
    ''' change, since Console.ReadLine() can't tell a redirected pipe from a real console. The input row
    ''' only appears once the helper's output is detected asking a question (see SetAwaitingInput).
    ''' The Continue button stays disabled until the process exits. Aborting a running import is done
    ''' via the title-bar X (see OnFormClosing), not Continue.
    ''' </remarks>
    Friend Class cImportProgressDialog
        Inherits Form

        ''' <summary>
        ''' Exact prefix EwEImportHelper's Program.vb prints before its model-choice Console.ReadLine().
        ''' No shared constant between the two projects - keep both in sync if this text ever changes.
        ''' </summary>
        Private Const InputPromptPrefix As String = "Enter a number (1-"

        Private Const ModelChoicePromptLabel As String = "Please enter the model number and press Enter."

        Private ReadOnly m_proc As Process
        Private ReadOnly m_txtOutput As New TextBox()
        Private ReadOnly m_txtInput As New TextBox()
        Private ReadOnly m_btnContinue As New Button()
        Private ReadOnly m_lblInput As New Label()

        ''' <summary>Signaled once each stream's DataReceived event fires with e.Data = Nothing.</summary>
        Private ReadOnly m_outputClosed As New Threading.ManualResetEventSlim(False)
        Private ReadOnly m_errorClosed As New Threading.ManualResetEventSlim(False)

        ''' <summary>Set once by OnProcessExited - read by OnFormClosing to decide whether closing
        ''' this window needs to kill a still-running process first.</summary>
        Private m_bProcessExited As Boolean = False

        ''' <summary>
        ''' Builds the window and starts draining the given, already-started process's output.
        ''' </summary>
        ''' <param name="proc">Already started with RedirectStandardOutput/Error/Input all True. Owned
        ''' by the caller - this class only subscribes to its events, writes to its input, and reads
        ''' its ExitCode.</param>
        Public Sub New(proc As Process)

            Me.m_proc = proc

            Me.Text = "Importing legacy Ecopath 5 model..."
            Me.StartPosition = FormStartPosition.CenterParent
            Me.Width = 720
            Me.Height = 480
            Me.MinimizeBox = False
            Me.MaximizeBox = True
            Me.FormBorderStyle = FormBorderStyle.Sizable
            AddHandler Me.FormClosing, AddressOf OnFormClosing

            ' One outer TableLayoutPanel for the whole window (output box + bottom panel as its two
            ' rows), rather than Dock.Fill/Dock.Bottom against the Form directly - keeps one consistent
            ' row-sizing engine governing both, instead of two engines that can disagree.
            Dim tableMain As New TableLayoutPanel() With {
                .Dock = DockStyle.Fill,
                .ColumnCount = 1,
                .RowCount = 2
            }
            tableMain.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0!))
            tableMain.RowStyles.Add(New RowStyle(SizeType.AutoSize))

            m_txtOutput.Multiline = True
            m_txtOutput.ReadOnly = True
            m_txtOutput.ScrollBars = ScrollBars.Vertical
            m_txtOutput.WordWrap = False
            m_txtOutput.Font = New Font("Consolas", 9.0!)
            m_txtOutput.Dock = DockStyle.Fill
            tableMain.Controls.Add(m_txtOutput, 0, 0)

            ' 3 rows (label, input, button), each sized to its own content via AutoSize/Absolute.
            Dim pnlBottom As New TableLayoutPanel() With {
                .Dock = DockStyle.Fill,
                .ColumnCount = 1,
                .RowCount = 3,
                .AutoSize = True,
                .AutoSizeMode = AutoSizeMode.GrowAndShrink,
                .Padding = New Padding(4)
            }
            pnlBottom.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            pnlBottom.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            pnlBottom.RowStyles.Add(New RowStyle(SizeType.Absolute, 40))

            ' Both hidden by default - see SetAwaitingInput, which shows/hides this row only while the
            ' helper is actually waiting for a response. A hidden control in an AutoSize row contributes
            ' no height, so the row collapses to ~0 until then.
            m_lblInput.Text = ModelChoicePromptLabel
            m_lblInput.Visible = False
            m_lblInput.Dock = DockStyle.Fill
            m_lblInput.AutoSize = True
            m_lblInput.AutoEllipsis = True
            m_lblInput.Margin = New Padding(2, 2, 2, 4)
            pnlBottom.Controls.Add(m_lblInput, 0, 0)

            m_txtInput.Visible = False
            m_txtInput.Dock = DockStyle.Fill
            m_txtInput.Margin = New Padding(2, 0, 2, 4)
            AddHandler m_txtInput.KeyDown, AddressOf OnInputKeyDown
            pnlBottom.Controls.Add(m_txtInput, 0, 1)

            m_btnContinue.Text = "Continue"
            m_btnContinue.Dock = DockStyle.Fill
            m_btnContinue.Margin = New Padding(2, 2, 2, 2)
            m_btnContinue.Enabled = False
            AddHandler m_btnContinue.Click, Sub() Me.Close()
            pnlBottom.Controls.Add(m_btnContinue, 0, 2)

            tableMain.Controls.Add(pnlBottom, 0, 1)

            Me.Controls.Add(tableMain)

            AddHandler proc.OutputDataReceived, AddressOf OnStdOutReceived
            AddHandler proc.ErrorDataReceived, AddressOf OnStdErrReceived
            proc.BeginOutputReadLine()
            proc.BeginErrorReadLine()

            ' Enable Continue only once the process has exited AND both streams are fully drained.
            Task.Run(Sub()
                         proc.WaitForExit()
                         m_outputClosed.Wait()
                         m_errorClosed.Wait()
                         Me.OnProcessExited()
                     End Sub)

        End Sub

        ''' <summary>Sends the input box's text + newline to the process's stdin, same as typing
        ''' into a real console and pressing Enter.</summary>
        Private Sub OnInputKeyDown(sender As Object, e As KeyEventArgs)
            If e.KeyCode <> Keys.Enter Then Return
            e.SuppressKeyPress = True ' Swallow the beep a single-line TextBox makes on Enter.
            Dim strLine As String = m_txtInput.Text
            Try
                m_proc.StandardInput.WriteLine(strLine)
                m_proc.StandardInput.Flush()
                Me.AppendOutput("> " & strLine)
                m_txtInput.Clear()
                Me.SetAwaitingInput(False)
            Catch
                ' Process may already have exited - nothing to send to.
            End Try
        End Sub

        Private Sub OnStdOutReceived(sender As Object, e As DataReceivedEventArgs)
            If e.Data Is Nothing Then
                m_outputClosed.Set()
                Return
            End If
            Me.AppendOutput(e.Data)
            ' The model-choice prompt is stdout only, never stderr.
            If e.Data.StartsWith(InputPromptPrefix, StringComparison.Ordinal) Then
                Me.SetAwaitingInput(True)
            End If
        End Sub

        Private Sub OnStdErrReceived(sender As Object, e As DataReceivedEventArgs)
            If e.Data Is Nothing Then
                m_errorClosed.Set()
                Return
            End If
            Me.AppendOutput(e.Data)
        End Sub

        ''' <summary>Shows/hides the input row together, focusing the input box when it appears.</summary>
        Private Sub SetAwaitingInput(bAwaiting As Boolean)
            If Me.InvokeRequired Then
                Try
                    Me.BeginInvoke(New Action(Of Boolean)(AddressOf SetAwaitingInput), bAwaiting)
                Catch
                End Try
                Return
            End If
            m_lblInput.Visible = bAwaiting
            m_txtInput.Visible = bAwaiting
            If bAwaiting Then m_txtInput.Focus()
        End Sub

        ''' <summary>
        ''' Appends one line, marshaling to the UI thread first if needed.
        ''' </summary>
        ''' <remarks>
        ''' TextBox.AppendText only reliably auto-scrolls into view while focused, which m_txtOutput
        ''' generally is not - SelectionStart + ScrollToCaret forces the scroll regardless. Update()
        ''' forces an immediate repaint rather than a possibly-deferred one.
        ''' </remarks>
        Private Sub AppendOutput(strLine As String)
            If Me.InvokeRequired Then
                Try
                    Me.BeginInvoke(New Action(Of String)(AddressOf AppendOutput), strLine)
                Catch
                    ' Window handle may already be gone - nothing left to show this in.
                End Try
                Return
            End If
            m_txtOutput.AppendText(strLine & Environment.NewLine)
            m_txtOutput.SelectionStart = m_txtOutput.TextLength
            m_txtOutput.ScrollToCaret()
            m_txtOutput.Update()
        End Sub

        Private Sub OnProcessExited()
            If Me.InvokeRequired Then
                Try
                    Me.BeginInvoke(New Action(AddressOf OnProcessExited))
                Catch
                End Try
                Return
            End If
            Me.AppendOutput($"--- Process exited with code {m_proc.ExitCode} ---")
            m_bProcessExited = True
            m_txtInput.Enabled = False
            m_btnContinue.Enabled = True
            m_btnContinue.Focus()
        End Sub

        ''' <summary>
        ''' Handles any attempt to close this window (title-bar X, or Continue once enabled): lets it
        ''' close normally if the process has exited, otherwise confirms and kills it first.
        ''' </summary>
        Private Sub OnFormClosing(sender As Object, e As FormClosingEventArgs)
            If m_bProcessExited Then Return

            Dim result As DialogResult = MessageBox.Show(Me,
                "The import is still running. Aborting it now will leave an incomplete, likely unusable target database behind." & Environment.NewLine & Environment.NewLine & "Abort the import anyway?",
                "Abort import?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)
            If result <> DialogResult.Yes Then
                e.Cancel = True
                Return
            End If

            Try
                If Not m_proc.HasExited Then
                    m_proc.Kill()
                    m_proc.WaitForExit(3000) ' Let Windows actually release file handles before returning.
                End If
            Catch
                ' Already exited, or the kill failed - nothing more useful to do.
            End Try
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing Then
                m_outputClosed.Dispose()
                m_errorClosed.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

    End Class

End Namespace
