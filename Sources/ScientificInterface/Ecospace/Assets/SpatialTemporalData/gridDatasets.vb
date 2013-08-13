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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.SpatialData
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2

#End Region ' Imports

Namespace Ecospace.Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' EwE grid for displaying datasets
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class gridDatasets
        Inherits EwEGrid

#Region " Private vars "

        Private m_adt As cSpatialDataAdapter = Nothing
        Private m_mhEcospace As cMessageHandler = Nothing
        Private m_man As cSpatialDataConnectionManager = Nothing
        Private m_manSets As cSpatialDataSetManager = Nothing

        Private Enum eColumnTypes As Integer
            Name
            DateFrom
            DateTo
            Variable
            Description
            Indexed
            TempOverlap
            SpatOverlap
        End Enum

#End Region ' Private vars

#Region " Construction / destruction "

        Public Sub New()
            MyBase.New()
        End Sub

#End Region ' Construction / destruction

#Region " Internals "

        Public Overrides Property UIContext As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As cUIContext)

                ' Deconfigure
                If (Me.UIContext IsNot Nothing) Then
                    Me.m_man = Nothing
                    Me.m_manSets = Nothing
                    Me.Core.Messages.RemoveMessageHandler(Me.m_mhEcospace)
                    Me.m_mhEcospace = Nothing
                End If

                ' Apply
                MyBase.UIContext = value

                ' Configure
                If (value IsNot Nothing) Then
                    Me.m_man = Me.Core.SpatialDataConnectionManager
                    Me.m_manSets = Me.m_man.DatasetManager
                    Me.m_mhEcospace = New cMessageHandler(AddressOf OnCoreMessage, EwEUtils.Core.eCoreComponentType.EcoSpace, eMessageType.DataModified, Me.UIContext.SyncObject)
                    Me.Core.Messages.AddMessageHandler(Me.m_mhEcospace)
#If DEBUG Then
                    Me.m_mhEcospace.Name = "gridDatasets"
#End If
                End If

            End Set
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_NAME)
            Me(0, eColumnTypes.DateFrom) = New EwEColumnHeaderCell(SharedResources.HEADER_FROM)
            Me(0, eColumnTypes.DateTo) = New EwEColumnHeaderCell(SharedResources.HEADER_TO)
            Me(0, eColumnTypes.Variable) = New EwEColumnHeaderCell(SharedResources.HEADER_VALUE)
            Me(0, eColumnTypes.Description) = New EwEColumnHeaderCell(SharedResources.HEADER_DESCRIPTION)
            Me(0, eColumnTypes.Indexed) = New EwEColumnHeaderCell(SharedResources.HEADER_INDEXED)
            Me(0, eColumnTypes.SpatOverlap) = New EwEColumnHeaderCell(SharedResources.HEADER_OVERLAP_SPATIAL)
            Me(0, eColumnTypes.TempOverlap) = New EwEColumnHeaderCell(SharedResources.HEADER_OVERLAP_TEMPORAL)

            Me.FixedColumns = 1
            Me.FixedColumnWidths = False
            Me.AllowBlockSelect = False

        End Sub

        Protected Overrides Sub FillData()

            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_manSets Is Nothing) Then Return
            If (Me.m_adt Is Nothing) Then Return

            Dim vfmt As New cVarnameTypeFormatter()
            Dim ds As ISpatialDataSet = Nothing
            Dim iRow As Integer = 0
            Dim cell As EwECell = Nothing

            For i As Integer = 0 To Me.m_manSets.Count - 1
                ds = Me.m_manSets(i)
                If (ds.VarName = eVarNameFlags.NotSet Or ds.VarName = Me.m_adt.VarName) Then

                    iRow = Me.AddRow()
                    Me(iRow, eColumnTypes.Name) = New EwERowHeaderCell(ds.DisplayName)
                    Me(iRow, eColumnTypes.Variable) = New EwECell(vfmt.GetDescriptor(ds.VarName), GetType(String), cStyleGuide.eStyleFlags.NotEditable)
                    Me(iRow, eColumnTypes.Description) = New EwECell(ds.Description, GetType(String), cStyleGuide.eStyleFlags.NotEditable)
                    Me(iRow, eColumnTypes.DateFrom) = New EwECell(ds.TimeStart.ToShortDateString, GetType(String), cStyleGuide.eStyleFlags.NotEditable)
                    Me(iRow, eColumnTypes.DateTo) = New EwECell(ds.TimeEnd.ToShortDateString, GetType(String), cStyleGuide.eStyleFlags.NotEditable)
                    Me(iRow, eColumnTypes.Indexed) = New EwECell("", GetType(String), cStyleGuide.eStyleFlags.NotEditable)
                    Me(iRow, eColumnTypes.SpatOverlap) = New EwECell("", GetType(String), cStyleGuide.eStyleFlags.NotEditable)
                    Me(iRow, eColumnTypes.TempOverlap) = New EwECell("", GetType(String), cStyleGuide.eStyleFlags.NotEditable)
                    Me.Rows(iRow).Tag = ds

                End If
                Me.UpdateDatasetRow(ds)
            Next

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            'Me.Columns(eColumnTypes.Description).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
        End Sub

        Private Sub UpdateDatasetRow(ds As ISpatialDataSet)

            Dim iRow As Integer = Me.DatasetRowIndex(ds)

            If (iRow < 1) Then Return

            Dim comp As New cDatasetCompatilibity(Me.Core, ds)
            Dim iNumTS As Integer = Math.Max(core.nEcospaceTimeSteps, 1)

            Me(iRow, eColumnTypes.Indexed).Value = String.Format(SharedResources.GENERIC_VALUE_PERCENTAGE, CInt(Math.Ceiling(100 * comp.NumIndexed / Math.Max(1, comp.NumOverlappingTimeSteps))))
            Me(iRow, eColumnTypes.TempOverlap).Value = String.Format(SharedResources.GENERIC_VALUE_PERCENTAGE, CInt(Math.Ceiling(100 * comp.NumOverlappingTimeSteps / iNumTS)))

            Dim strSpatial As String = SharedResources.GENERIC_VALUE_UNKNOWN
            If comp.NumIndexed > 0 Then
                If comp.NumFullSpatialOverlap = comp.NumIndexed Then
                    strSpatial = SharedResources.GENERIC_VALUE_FULL
                ElseIf comp.NumPartialSpatialOverlap > 0 Then
                    strSpatial = SharedResources.GENERIC_VALUE_PARTIAL
                Else
                    strSpatial = SharedResources.GENERIC_VALUE_NONE
                End If
            End If
            Me(iRow, eColumnTypes.SpatOverlap).Value = strSpatial

        End Sub

        Private Function DatasetRowIndex(ds As ISpatialDataSet) As Integer
            For Each ri As RowInfo In Me.Rows
                If (Object.ReferenceEquals(ri.Tag, ds)) Then
                    Return ri.Index
                End If
            Next
            Return -1
        End Function

        Public Overrides Sub OnCoreMessage(ByRef msg As cMessage)

            If (msg.DataType = EwEUtils.Core.eDataTypes.EcospaceSpatialDataConnection) Then
                Me.UpdateDatasetRow(Me.m_manSets.IndexDataset)
            End If

        End Sub

#End Region ' Internals

        Public Sub Fill(adt As cSpatialDataAdapter, Optional dsSelect As ISpatialDataSet = Nothing)

            If (adt Is Nothing) Then Return
            If (dsSelect Is Nothing) Then dsSelect = Me.SelectedDataset

            Me.m_adt = adt

            Me.RefreshContent()
            Me.SelectedDataset = dsSelect

        End Sub

        Public Property SelectedDataset As ISpatialDataSet
            Get
                If Me.SelectedRow < 1 Then Return Nothing
                Return DirectCast(Me.Rows(Me.SelectedRow).Tag, ISpatialDataSet)
            End Get
            Set(ds As ISpatialDataSet)
                Me.SelectRow(Me.DatasetRowIndex(ds))
            End Set
        End Property

    End Class

End Namespace
