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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore.SpatialData
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Ecospace.Controls

    Public Class dlgExportSpatialData

#Region " Private variables "

        Private Enum eSelectionMode As Integer
            None = 0
            Used
            All
        End Enum

        Private m_uic As cUIContext = Nothing
        Private m_manConn As cSpatialDataConnectionManager = Nothing
        Private m_manSets As cSpatialDataSetManager = Nothing
        Private m_htUsed As New HashSet(Of ISpatialDataSet)

#End Region ' Private variables

#Region " Constructor "

        Public Sub New(uic As cUIContext)
            Me.InitializeComponent()
            Me.m_uic = uic

            Me.m_manConn = Me.m_uic.Core.SpatialDataConnectionManager
            Me.m_manSets = Me.m_manConn.DatasetManager

        End Sub

#End Region ' Constructor

#Region " Form overloads "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            ' Nice!
            Me.CenterToParent()

            If (Me.m_uic Is Nothing) Then Return

            ' Populate dataset box
            For Each ds As ISpatialDataSet In Me.m_manSets
                Me.m_clbDatsets.Items.Add(ds)
            Next

            ' Make snapshot of all used adapters
            For Each adt As cSpatialDataAdapter In Me.m_manConn.Adapters
                For i As Integer = 0 To adt.MaxLength
                    For j As Integer = 1 To cSpatialDataStructures.cMAX_CONN
                        Dim ds As ISpatialDataSet = adt.Dataset(i, j)
                        If (ds IsNot Nothing) Then
                            If (Not Me.m_htUsed.Contains(ds)) Then
                                Me.m_htUsed.Add(ds)
                            End If
                        End If
                    Next
                Next
            Next

            ' Start with a default name
            Me.m_tbxName.Text = cFileUtils.ToValidFileName(Me.m_uic.Core.EwEModel.Name, False)

            ' Shabang
            Me.SelectDatasets(eSelectionMode.Used)

        End Sub

        Protected Overrides Sub OnSizeChanged(e As System.EventArgs)
            MyBase.OnSizeChanged(e)
            Me.UpdateControls()
        End Sub

#End Region ' Form overloads

#Region " Events "

        Private Sub OnTargetNameChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_tbxName.TextChanged
            Me.UpdateControls()
        End Sub

        Private Sub OnSelectUsed(sender As System.Object, e As System.EventArgs) _
            Handles m_btnUsed.Click
            Me.SelectDatasets(eSelectionMode.Used)
        End Sub

        Private Sub OnSelectAll(sender As System.Object, e As System.EventArgs) _
            Handles m_btnAll.Click
            Me.SelectDatasets(eSelectionMode.All)
        End Sub

        Private Sub OnSelectNone(sender As System.Object, e As System.EventArgs) _
            Handles m_btnNone.Click
            Me.SelectDatasets(eSelectionMode.None)
        End Sub

        Private Sub OnExport(sender As System.Object, e As System.EventArgs) _
            Handles m_btnExport.Click

            If Me.m_manSets.Save(Me.OutputLocation(), Me.SelectedDatasets()) Then
                Me.DialogResult = Windows.Forms.DialogResult.OK
                Me.Close()
            End If

        End Sub

        Private Sub OnCancel(sender As System.Object, e As System.EventArgs) Handles m_btnCancel.Click
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub OnFormatDataset(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
            Handles m_clbDatsets.Format

            Dim fmt As New cSpatialDatasetFormatter()
            e.Value = fmt.GetDescriptor(e.ListItem, eDescriptorTypes.Abbreviation)

        End Sub

#End Region ' Events

#Region " Internals "

        Private Function OutputLocation() As String
            Dim strPath As String = Path.Combine(Me.m_uic.Core.DefaultOutputPath(eAutosaveTypes.Ecospace), _
                                                 cFileUtils.ToValidFileName(Me.m_tbxName.Text, False))
            Return Path.Combine(strPath, "ewe_datasets.xml")
        End Function

        Private Sub SelectDatasets(ByVal mode As eSelectionMode)

            For i As Integer = 0 To Me.m_clbDatsets.Items.Count - 1

                Dim bCheck As Boolean = False

                Select Case mode
                    Case eSelectionMode.All
                        bCheck = True
                    Case eSelectionMode.None
                        bCheck = False
                    Case eSelectionMode.Used
                        Dim ds As ISpatialDataSet = CType(Me.m_clbDatsets.Items(i), ISpatialDataSet)
                        bCheck = Me.m_htUsed.Contains(ds)
                End Select

                Me.m_clbDatsets.SetItemChecked(i, bCheck)
            Next
            Me.UpdateControls()

        End Sub

        Private Function SelectedDatasets() As ISpatialDataSet()

            Dim lds As New List(Of ISpatialDataSet)
            Try
                For i As Integer = 0 To Me.m_clbDatsets.Items.Count - 1
                    If (Me.m_clbDatsets.GetItemChecked(i)) Then
                        lds.Add(DirectCast(Me.m_clbDatsets.Items(i), ISpatialDataSet))
                    End If
                Next
            Catch ex As Exception

            End Try
            Return lds.ToArray

        End Function

        Private Sub UpdateControls()

            If (Me.m_uic Is Nothing) Then Return

            Dim strFile As String = String.Copy(Path.GetDirectoryName(Me.OutputLocation()))
            Dim bHasTarget As Boolean = Not String.IsNullOrWhiteSpace(cFileUtils.ToValidFileName(Me.m_tbxName.Text, False))
            Dim bHasSelection As Boolean = (Me.m_clbDatsets.CheckedIndices.Count > 0)

            TextRenderer.MeasureText(strFile, Me.m_lblFolderPreview.Font, New Drawing.Size(Me.m_lblFolderPreview.ClientSize.Width, Me.m_lblFolderPreview.ClientSize.Height), _
                                     TextFormatFlags.SingleLine Or TextFormatFlags.PathEllipsis Or TextFormatFlags.ModifyString)
            Me.m_lblFolderPreview.Text = strFile

            Me.m_btnExport.Enabled = bHasTarget And bHasSelection

        End Sub

#End Region ' Internals

    End Class

End Namespace
