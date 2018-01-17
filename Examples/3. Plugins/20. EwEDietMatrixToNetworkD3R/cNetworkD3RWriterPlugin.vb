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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports EwEPlugin
Imports EwECore
Imports EwEUtils.Core
Imports System.Windows.Forms
Imports System.Text

Public Class cNetworkD3RWriterPlugin
    Implements IMenuItemPlugin

    Private m_core As cCore

    Public ReadOnly Property MenuItemLocation As String Implements IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuEcopath"
        End Get
    End Property

    Public ReadOnly Property ControlImage As System.Drawing.Image Implements IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements IGUIPlugin.ControlText
        Get
            Return "Export diet matrix to NetworkD3 R format"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcopathLoaded
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "EwEEcopathExportDietToNeworkD3"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "Plug-in for the EwE desktop software that exports a diet matrix to R arrays for use in NetworkD3 (https://christophergandrud.github.io/networkD3/)"
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Windows.Forms.Form) Implements IGUIPlugin.OnControlClick
        Try
            Me.ToSimpleNetworkArrays(True)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
    End Sub

#Region " Internals "

    ''' <summary>
    ''' <para>Copy the diet matrix sources and targets to R arrays 'src' and 'target' for use in the NetworkD3 R package, simpleNetwork 
    ''' https://christophergandrud.github.io/networkD3/. The R arrays are copied to the clipboard.
    ''' </para>
    ''' </summary>
    ''' <param name="bUseSymbolicNames"></param>
    ''' <param name="src"></param>
    ''' <param name="target"></param>
    Private Sub ToSimpleNetworkArrays(bUseSymbolicNames As Boolean, Optional src As String = "src", Optional target As String = "target")

        Dim lSrc As New List(Of String)
        Dim lTgt As New List(Of String)

        For iPred As Integer = 1 To Me.m_core.nLivingGroups
            Dim pred As cEcoPathGroupInput = Me.m_core.EcoPathGroupInputs(iPred)
            For iPrey As Integer = 1 To Me.m_core.nGroups
                If pred.DietComp(iPrey) > 0 Then
                    Dim prey As cEcoPathGroupInput = Me.m_core.EcoPathGroupInputs(iPred)
                    If bUseSymbolicNames Then
                        lSrc.Add(ToExcelColumnName(iPred))
                        lTgt.Add(ToExcelColumnName(iPrey))
                    Else
                        lSrc.Add(ToRString(pred.Name))
                        lTgt.Add(ToRString(prey.Name))
                    End If
                End If
            Next
        Next

        Dim sb As New StringBuilder()
        sb.AppendLine("library(networkD3)")
        sb.AppendLine()
        sb.AppendLine("# Diet network from EwE model " & Me.ToRString(Me.m_core.EwEModel.Name))
        If (bUseSymbolicNames) Then sb.AppendLine("# Group names have been replaced with symbolic names")
        sb.AppendLine(ToR(src, lSrc))
        sb.AppendLine(ToR(target, lTgt))
        sb.AppendLine()
        sb.AppendLine("networkData <- data.frame(src, target)")
        sb.AppendLine("# Plot")
        sb.AppendLine("simpleNetwork(networkData)")
        Clipboard.SetText(sb.ToString())

        Dim msg As New cMessage("NetworkD3 simple plot script copied to clipboard", eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
        Me.m_core.Messages.SendMessage(msg)

    End Sub

    Private Function ToRString(strIn As String) As String

        Dim sb As New StringBuilder()

        For i As Integer = 0 To strIn.Length - 1
            Dim c As Char = strIn(i)
            If Char.IsLetterOrDigit(c) Or Char.IsWhiteSpace(c) Then
                sb.Append(c)
            End If
        Next

        Return sb.ToString()

    End Function

    Private Function ToExcelColumnName(iValue As Integer) As String

        Dim iDiv As Integer = iValue
        Dim iMod As Integer
        Dim sb As New StringBuilder()

        While iDiv > 0
            iMod = (iDiv - 1) Mod 26
            sb.Insert(0, Convert.ToChar(65 + iMod))
            iDiv = CInt((iDiv - iMod) / 26)
        End While

        Return sb.ToString()

    End Function

    Private Function ToR(strPrefix As String, items As IEnumerable(Of String)) As String

        Dim sb As New StringBuilder()
        Dim iLineLength As Integer = 0
        Dim iLeadIn As Integer = 0

        sb.Append(strPrefix)
        sb.Append(" <- c(")
        iLeadIn = sb.Length

        iLineLength = sb.Length

        For i As Integer = 0 To items.Count - 1

            Dim strBit As String = """" & items(i) & """"
            If (i < items.Count - 1) Then
                strBit = strBit & ", "
            End If

            Dim iTest As Integer = strBit.Length
            If (iLineLength + iTest >= 999) Then
                sb.AppendLine()
                strBit = strBit.PadLeft(iLeadIn + iTest, " "c)
                iLineLength = iLeadIn
            End If

            sb.Append(strBit)
            iLineLength += iTest
        Next
        sb.Append(")")
        Return sb.ToString()

    End Function

#End Region ' Internals

End Class
