'==============================================================================
'
' $Log: dlgEditBasemap.vb,v $
' Revision 1.2  2009/03/19 16:02:26  jeroens
' Added FormatProvider.Release
'
' Revision 1.1  2008/09/26 07:31:56  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.5  2008/08/11 04:39:35  jeroens
' Simplified  Ecospace core class names
'
' Revision 1.4  2008/06/02 00:01:23  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.3  2008/05/29 22:22:39  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.2  2007/12/03 22:17:22  jeroens
' * Uses EwE format providers
'
' Revision 1.1  2007/10/21 15:22:07  jeroens
' * Moved
'
' Revision 1.4  2007/09/21 14:44:05  jeroens
' * Fixed map resize test bug
'
'==============================================================================

Option Strict On
Imports EwECore
Imports System.Windows.Forms
Imports ScientificInterface.Controls
Imports ScientificInterface.Other
Imports EwEUtils.Core

Public Class dlgEditBasemap

    Private m_basemap As cEcospaceBasemap = Nothing
    Private m_fpInCol As cEwEFormatProvider = Nothing
    Private m_fpInRow As cEwEFormatProvider = Nothing
    Private m_fpLat As cEwEFormatProvider = Nothing
    Private m_fpLon As cEwEFormatProvider = Nothing
    Private m_fpCellLength As cEwEFormatProvider = Nothing

    Public Sub New(ByVal basemap As cEcospaceBasemap)
        Me.InitializeComponent()
        ' Sanity checks
        Debug.Assert(basemap IsNot Nothing)
        ' Remember the milk
        Me.m_basemap = basemap
    End Sub

#Region " Events "

    Private Sub dlgEditBasemap_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles Me.Load

        Me.m_fpInCol = New cEwEFormatProvider(Me.nudColCount, GetType(Integer), Me.m_basemap.GetVariableMetadata(eVarNameFlags.InCol))
        Me.m_fpInCol.Value = Me.m_basemap.InCol

        Me.m_fpInRow = New cEwEFormatProvider(Me.nudRowCount, GetType(Integer), Me.m_basemap.GetVariableMetadata(eVarNameFlags.InRow))
        Me.m_fpInRow.Value = Me.m_basemap.InRow

        Me.m_fpLat = New cEwEFormatProvider(Me.nudLatTL, GetType(Single), Me.m_basemap.GetVariableMetadata(eVarNameFlags.Latitude))
        Me.m_fpLat.Value = Me.m_basemap.Latitude

        Me.m_fpLon = New cEwEFormatProvider(Me.nudLonTL, GetType(Single), Me.m_basemap.GetVariableMetadata(eVarNameFlags.Longitude))
        Me.m_fpLon.Value = Me.m_basemap.Longitude

        Me.m_fpCellLength = New cEwEFormatProvider(Me.nudCellLength, GetType(Single), Me.m_basemap.GetVariableMetadata(eVarNameFlags.CellLength))
        Me.m_fpCellLength.Value = Me.m_basemap.CellLength

        Me.UpdateControls()

    End Sub

    Private Sub dlgEditBasemap_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) _
        Handles Me.FormClosing

        Me.m_fpCellLength.Release()
        Me.m_fpInCol.Release()
        Me.m_fpInRow.Release()
        Me.m_fpLat.Release()
        Me.m_fpLon.Release()

    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.Apply()
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub nudColCount_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles nudColCount.ValueChanged, nudLonTL.ValueChanged, nudLatTL.ValueChanged, nudCellLength.ValueChanged
        Me.UpdateControls()
    End Sub

    Private Sub nudRowCount_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles nudRowCount.ValueChanged
        Me.UpdateControls()
    End Sub

    Private Sub tbLatTL_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.UpdateControls()
    End Sub

    Private Sub tbLonTL_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.UpdateControls()
    End Sub

    Private Sub tbCellLength_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.UpdateControls()
    End Sub

#End Region ' Events 

#Region " Implementation "

    Private Sub UpdateControls()
        Me.OK_Button.Enabled = True
    End Sub

    Private Sub Apply()

        Dim iColCount As Integer = CInt(Me.m_fpInCol.Value)
        Dim iRowCount As Integer = CInt(Me.m_fpInRow.Value)
        Dim bResizeMap As Boolean = False

        If ((iRowCount <> Me.m_basemap.InRow) Or (iColCount <> Me.m_basemap.InCol)) Then
            bResizeMap = True
            If ((iRowCount < Me.m_basemap.InRow) Or (iColCount < Me.m_basemap.InCol)) Then
                ' Prompt user
                If MsgBox(My.Resources.ECOSPACE_BASEMAP_SHRINK_PROMPT, MsgBoxStyle.YesNo Or MsgBoxStyle.Exclamation) = MsgBoxResult.No Then
                    Return
                End If
            End If
        End If

        ' Apply other data first
        Me.m_basemap.CellLength = CSng(Me.m_fpCellLength.Value)
        Me.m_basemap.Latitude = CSng(Me.m_fpLat.Value)
        Me.m_basemap.Longitude = CSng(Me.m_fpLon.Value)

        If bResizeMap Then
            ' Whooohooo! if THIS is not going to cause a Tsunami...
            cCore.GetInstance().ResizeEcospaceBasemap(iRowCount, iColCount)
        End If
    End Sub

#End Region ' Implementation

End Class
