' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "


#End Region ' Imports

Namespace Controls

    ''' =======================================================================
    ''' <summary>
    ''' Factory for conjuring a custom message box.
    ''' </summary>
    ''' =======================================================================
    Public Class cCustomMessageBox

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the system icon for a <see cref="System.Windows.Forms.MessageBoxIcon">message box 
        ''' icon</see> identifier.
        ''' </summary>
        ''' <param name="mbi"><see cref="System.Windows.Forms.MessageBoxIcon">message box icon</see>
        ''' identifier to get the system icon for.</param>
        ''' <returns>An <see cref="Icon">Icon</see>, or Nothing if the icon
        ''' could not be found.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetMessageBoxIcon(mbi As MessageBoxIcon) As Icon

            Dim objIcon As Icon = Nothing

            Select Case mbi
                Case MessageBoxIcon.Asterisk
                    objIcon = SystemIcons.Asterisk
                Case MessageBoxIcon.Error
                    objIcon = SystemIcons.Error
                Case MessageBoxIcon.Exclamation
                    objIcon = SystemIcons.Exclamation
                Case MessageBoxIcon.Hand,
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

        Public Shared Function Show(uic As cUIContext, strText As String) As DialogResult
            Return cCustomMessageBox.Show(uic, strText, "")
        End Function

        Public Shared Function Show(uic As cUIContext,
                                    strText As String,
                                    strCaption As String) As DialogResult
            Return cCustomMessageBox.Show(uic, strText, strCaption, MessageBoxButtons.OK, MessageBoxIcon.Information, Nothing)
        End Function

        Public Shared Function Show(uic As cUIContext,
                                    strText As String,
                                    strCaption As String,
                                    mbb As MessageBoxButtons,
                                    mbi As MessageBoxIcon,
                                    customtexts As Dictionary(Of DialogResult, String)) As DialogResult

            Dim frm As frmCustomMessageBox = Nothing

            ' Provide defaults
            If String.IsNullOrEmpty(strCaption) Then strCaption = My.Resources.HEADER_MESSAGE

            frm = New frmCustomMessageBox(strText, strCaption, mbb, mbi, "", customtexts)
            frm.UIContext = uic
            frm.ShowDialog(uic.FormMain)

            Return frm.Result

        End Function

        Public Shared Function Show(uic As cUIContext, strText As String, ByRef bIsChecked As Boolean, strCheckPrompt As String) As DialogResult
            Return cCustomMessageBox.Show(uic, strText, "", bIsChecked, strCheckPrompt)
        End Function

        Public Shared Function Show(uic As cUIContext,
                                    strText As String,
                                    strCaption As String,
                                    ByRef bIsChecked As Boolean,
                                    strCheckPrompt As String) As DialogResult
            Return cCustomMessageBox.Show(uic, strText, strCaption, MessageBoxButtons.OK, MessageBoxIcon.Information, bIsChecked, strCheckPrompt, Nothing)
        End Function

        Public Shared Function Show(uic As cUIContext,
                                    strText As String,
                                    strCaption As String,
                                    mbb As MessageBoxButtons,
                                    mbi As MessageBoxIcon,
                                    ByRef bIsChecked As Boolean,
                                    strCheckPrompt As String,
                                    customtexts As Dictionary(Of DialogResult, String)) As DialogResult

            Dim frm As frmCustomMessageBox = Nothing

            ' Provide defaults
            If String.IsNullOrEmpty(strCaption) Then strCaption = My.Resources.HEADER_MESSAGE

            frm = New frmCustomMessageBox(strText, strCaption, mbb, mbi, strCheckPrompt, customtexts)
            frm.UIContext = uic
            frm.ShowDialog(uic.FormMain)

            ' Transfer checked state
            bIsChecked = frm.IsChecked
            ' Return result
            Return frm.Result

        End Function

#End Region ' Public interfaces

#Region " Internals "

        Private Shared Function GetIcon(Icon As MessageBoxIcon) As Icon

            Dim objIcon As Icon = Nothing

            Select Case Icon
                Case MessageBoxIcon.Asterisk
                    objIcon = SystemIcons.Asterisk
                Case MessageBoxIcon.Error
                    objIcon = SystemIcons.Error
                Case MessageBoxIcon.Exclamation
                    objIcon = SystemIcons.Exclamation
                Case MessageBoxIcon.Hand,
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
