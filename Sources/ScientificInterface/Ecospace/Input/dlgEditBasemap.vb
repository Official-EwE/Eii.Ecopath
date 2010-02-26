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

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_basemap As cEcospaceBasemap = Nothing

    Private m_fpInCol As cEwEFormatProvider = Nothing
    Private m_fpInRow As cEwEFormatProvider = Nothing
    Private m_fpLat As cEwEFormatProvider = Nothing
    Private m_fpLon As cEwEFormatProvider = Nothing
    Private m_fpCellLength As cEwEFormatProvider = Nothing

#End Region ' Private vars

    Public Sub New(ByVal uic As cUIContext)
        Me.m_uic = uic
        Me.m_basemap = Me.m_uic.Core.EcospaceBasemap
        Me.InitializeComponent()
    End Sub

#Region " Events "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

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

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        Me.m_fpCellLength.Release()
        Me.m_fpInCol.Release()
        Me.m_fpInRow.Release()
        Me.m_fpLat.Release()
        Me.m_fpLon.Release()
        MyBase.OnFormClosed(e)

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
            Me.m_uic.Core.ResizeEcospaceBasemap(iRowCount, iColCount)
        End If
    End Sub

#End Region ' Implementation

End Class
