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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore
Imports System.Windows.Forms
Imports ScientificInterface.Controls
Imports ScientificInterface.Other
Imports EwEUtils.Core

#End Region ' Imports

''' =======================================================================
''' <summary>
''' Dialog, implementing the Ecospace Edit Basemap user interface.
''' </summary>
''' =======================================================================
Public Class dlgEditBasemap

#Region " Private variables "

    Private m_uic As cUIContext = Nothing
    Private m_basemap As cEcospaceBasemap = Nothing

    Private m_fpInCol As cEwEFormatProvider = Nothing
    Private m_fpInRow As cEwEFormatProvider = Nothing
    Private m_fpLat As cEwEFormatProvider = Nothing
    Private m_fpLon As cEwEFormatProvider = Nothing
    Private m_fpCellLength As cEwEFormatProvider = Nothing
    Private m_fpCellSize As cEwEFormatProvider = Nothing

    Private m_bInitialized As Boolean = False
    Private m_bInUpdate As Boolean = False

#End Region ' Private variables

    Public Sub New(ByVal uic As cUIContext)
        Me.m_uic = uic
        Me.m_basemap = Me.m_uic.Core.EcospaceBasemap
        Me.InitializeComponent()
    End Sub

#Region " Events "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_fpInCol = New cEwEFormatProvider(Me.m_uic, Me.m_nudColCount, GetType(Integer), Me.m_basemap.GetVariableMetadata(eVarNameFlags.InCol))
        Me.m_fpInCol.Value = Me.m_basemap.InCol

        Me.m_fpInRow = New cEwEFormatProvider(Me.m_uic, Me.m_nudRowCount, GetType(Integer), Me.m_basemap.GetVariableMetadata(eVarNameFlags.InRow))
        Me.m_fpInRow.Value = Me.m_basemap.InRow

        Me.m_fpLat = New cEwEFormatProvider(Me.m_uic, Me.m_nudLatTL, GetType(Single), Me.m_basemap.GetVariableMetadata(eVarNameFlags.Latitude))
        Me.m_fpLat.Value = Me.m_basemap.Latitude

        Me.m_fpLon = New cEwEFormatProvider(Me.m_uic, Me.m_nudLonTL, GetType(Single), Me.m_basemap.GetVariableMetadata(eVarNameFlags.Longitude))
        Me.m_fpLon.Value = Me.m_basemap.Longitude

        Me.m_fpCellLength = New cEwEFormatProvider(Me.m_uic, Me.m_nudCellLength, GetType(Single), Me.m_basemap.GetVariableMetadata(eVarNameFlags.CellLength))
        Me.m_fpCellLength.Value = Me.m_basemap.CellLength

        Me.m_fpCellSize = New cEwEFormatProvider(Me.m_uic, Me.m_nudCellSize, GetType(Single), Me.m_basemap.GetVariableMetadata(eVarNameFlags.CellSize))
        Me.m_fpCellSize.Value = Me.m_basemap.CellSize

        Me.UpdateControls()

        Me.m_bInitialized = True

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        Me.m_fpCellLength.Release()
        Me.m_fpCellSize.Release()
        Me.m_fpInCol.Release()
        Me.m_fpInRow.Release()
        Me.m_fpLat.Release()
        Me.m_fpLon.Release()
        MyBase.OnFormClosed(e)

    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnOk.Click
        Me.Apply()
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnCancel.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub OnCellLengthChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_nudCellLength.ValueChanged

        If Not Me.m_bInitialized Or Me.m_bInUpdate Then Return

        Me.m_bInUpdate = True
        Dim sLen As Single = CSng(Me.m_nudCellLength.Value)
        Me.m_fpCellSize.Value = cEcospaceBasemap.ToCellSize(sLen)
        Me.m_bInUpdate = False

    End Sub

    Private Sub OnCellSizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_nudCellSize.ValueChanged

        If Not Me.m_bInitialized Or Me.m_bInUpdate Then Return

        Me.m_bInUpdate = True
        Dim sSize As Single = CSng(Me.m_nudCellSize.Value)
        Me.m_fpCellLength.Value = cEcospaceBasemap.ToCellLength(sSize)
        Me.m_bInUpdate = False

    End Sub

#End Region ' Events 

#Region " Implementation "

    Private Sub UpdateControls()
        Me.m_btnOk.Enabled = True
    End Sub

    Private Sub Apply()

        Dim iColCount As Integer = CInt(Me.m_fpInCol.Value)
        Dim iRowCount As Integer = CInt(Me.m_fpInRow.Value)
        Dim fmsg As cFeedbackMessage = Nothing
        Dim core As cCore = Me.m_uic.Core
        Dim bResizeMap As Boolean = False

        If ((iRowCount <> Me.m_basemap.InRow) Or (iColCount <> Me.m_basemap.InCol)) Then
            bResizeMap = True
            If ((iRowCount < Me.m_basemap.InRow) Or (iColCount < Me.m_basemap.InCol)) Then
                ' Prompt user
                fmsg = New cFeedbackMessage(My.Resources.ECOSPACE_BASEMAP_SHRINK_PROMPT, _
                                            eCoreComponentType.External, eMessageType.Any, _
                                            eMessageImportance.Question, cFeedbackMessage.eReplyStyle.YES_NO)
                fmsg.Reply = cFeedbackMessage.eReply.NO
                core.Messages.SendMessage(fmsg)
                If (fmsg.Reply = cFeedbackMessage.eReply.NO) Then
                    Return
                End If
            End If
        End If

        ' Apply other data first
        Me.m_basemap.CellLength = CSng(Me.m_fpCellLength.Value)
        Me.m_basemap.CellSize = CSng(Me.m_fpCellSize.Value)
        Me.m_basemap.Latitude = CSng(Me.m_fpLat.Value)
        Me.m_basemap.Longitude = CSng(Me.m_fpLon.Value)

        If bResizeMap Then
            cApplicationStatusNotifier.StartProgress(core, My.Resources.STATUS_RESIZING_MAP)
            Try
                ' Whooohooo! if THIS is not going to cause a Tsunami...
                Me.m_uic.Core.ResizeEcospaceBasemap(iRowCount, iColCount)
            Catch ex As Exception

            End Try
            cApplicationStatusNotifier.EndProgress(core)
        End If

    End Sub

#End Region ' Implementation

End Class
