#Region " Imports "

Imports System.Windows.Forms
Imports System.Drawing
Imports Microsoft.Win32

#End Region ' Imports

Namespace Controls

    ''' =======================================================================
    ''' <summary>
    ''' Factory for conjuring a custom message box.
    ''' </summary>
    ''' =======================================================================
    Public Class cCustomMessageBox

#Region " Public interfaces "

        Public Shared Function Show(ByVal strText As String) As DialogResult
            Return cCustomMessageBox.Show(strText, "")
        End Function

        Public Shared Function Show(ByVal strText As String, _
                                    ByVal strCaption As String) As DialogResult
            Return cCustomMessageBox.Show(strText, strCaption, _
                                          MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Function

        Public Shared Function Show(ByVal strText As String, _
                                    ByVal strCaption As String, _
                                    ByVal mbb As MessageBoxButtons, _
                                    ByVal mbi As MessageBoxIcon) As DialogResult

            Dim frm As frmCustomMessageBox = Nothing

            ' Provide defaults
            If String.IsNullOrEmpty(strCaption) Then strCaption = "Message"

            frm = New frmCustomMessageBox(strText, strCaption, mbb, mbi)
            frm.ShowDialog()

            Return frm.Result

        End Function

        Public Shared Function Show(ByVal strText As String, _
                                    ByRef bIsChecked As Boolean, _
                                    ByVal strCheckPrompt As String) As DialogResult
            Return cCustomMessageBox.Show(strText, "", bIsChecked, strCheckPrompt)
        End Function

        Public Shared Function Show(ByVal strText As String, _
                                    ByVal strCaption As String, _
                                    ByRef bIsChecked As Boolean, _
                                    ByVal strCheckPrompt As String) As DialogResult
            Return cCustomMessageBox.Show(strText, strCaption, _
                                          MessageBoxButtons.OK, MessageBoxIcon.Information, _
                                          bIsChecked, strCheckPrompt)
        End Function

        Public Shared Function Show(ByVal strText As String, _
                                    ByVal strCaption As String, _
                                    ByVal mbb As MessageBoxButtons, _
                                    ByVal mbi As MessageBoxIcon, _
                                    ByRef bIsChecked As Boolean, _
                                    ByVal strCheckPrompt As String) As DialogResult

            Dim frm As frmCustomMessageBox = Nothing

            ' Provide defaults
            If String.IsNullOrEmpty(strCaption) Then strCaption = "Message"

            frm = New frmCustomMessageBox(strText, strCaption, mbb, mbi, strCheckPrompt)
            frm.ShowDialog()

            ' Transfer checked state
            bIsChecked = frm.IsChecked
            ' Return result
            Return frm.Result

        End Function

#End Region ' Public interfaces

#Region " Internals "

        Private Shared Function GetIcon(ByVal Icon As MessageBoxIcon) As Icon

            Dim objIcon As Icon = Nothing

            Select Case Icon
                Case MessageBoxIcon.Asterisk
                    objIcon = SystemIcons.Asterisk
                Case MessageBoxIcon.Error
                    objIcon = SystemIcons.Error
                Case MessageBoxIcon.Exclamation
                    objIcon = SystemIcons.Exclamation
                Case MessageBoxIcon.Hand, _
                     MessageBoxIcon.Stop
                    objIcon = SystemIcons.Hand
                Case MessageBoxIcon.Information
                    objIcon = SystemIcons.Information
                Case MessageBoxIcon.Question
                    objIcon = SystemIcons.Question
                Case MessageBoxIcon.Warning
                    objIcon = SystemIcons.Warning
                Case Else
                    ' NOP
            End Select

            Return objIcon

        End Function

#End Region ' Internals

    End Class

End Namespace ' Controls
