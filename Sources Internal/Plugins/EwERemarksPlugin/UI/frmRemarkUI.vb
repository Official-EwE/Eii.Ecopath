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
Imports ScientificInterfaceShared.Forms
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwECore
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Properties
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' EcoWriter plug-in user interface.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmRemarkUI

#Region " Private classes "

    Private Class cCoreComponentItem

        Private m_components As eDataTypes() = Nothing
        Private m_strDisplay As String = ""

        Public Sub New(ByVal strDisplay As String, ByVal components As eDataTypes())
            Me.m_components = components
            Me.m_strDisplay = strDisplay
        End Sub

        Public Overrides Function ToString() As String
            Return Me.m_strDisplay
        End Function

        Public ReadOnly Property Components As eDataTypes()
            Get
                Return Me.m_components
            End Get
        End Property

    End Class

    Private Class cSortItem

        Private m_sort As ePropertySortOrderTypes
        Private m_strDisplay As String = ""

        Public Sub New(ByVal strDisplay As String, ByVal sort As ePropertySortOrderTypes)
            Me.m_strDisplay = strDisplay
            Me.m_sort = sort
        End Sub

        Public Overrides Function ToString() As String
            Return Me.m_strDisplay
        End Function

        Public ReadOnly Property Sort As ePropertySortOrderTypes
            Get
                Return Me.m_sort
            End Get
        End Property

    End Class

#End Region ' Private classes

#Region " Private vars "

    Private m_filter() As eDataTypes = Nothing
    Private m_sortorder As ePropertySortOrderTypes = ePropertySortOrderTypes.Source
    Private m_monitor As cRemarkMonitor = Nothing
    Private m_bInvalid As Boolean = False

#End Region ' Private vars

#Region " Constructor "

    Public Sub New(uic As cUIContext)
        MyBase.New()
        Me.InitializeComponent()
        Me.UIContext = uic
        Me.Grid = Me.m_grid
    End Sub

#End Region ' Constructor

#Region " Form overloads "

    Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(ByVal value As ScientificInterfaceShared.Controls.cUIContext)
            MyBase.UIContext = value
            Me.m_grid.UIContext = value
        End Set
    End Property

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        ' Populate filter box
        Me.AddFilter(SharedResources.GENERIC_VALUE_ALL)
        Me.AddFilter(SharedResources.GENERIC_VALUE_ALLGROUPS, New eDataTypes() {eDataTypes.EcoPathGroupInput, eDataTypes.EcoPathGroupOutput, _
                                       eDataTypes.EcoSimGroupInput, eDataTypes.EcoSimGroupOutput, _
                                       eDataTypes.EcospaceGroup, eDataTypes.EcospaceGroupOuput})
        Me.AddFilter(SharedResources.GENERIC_VALUE_ALLFLEETS, New eDataTypes() {eDataTypes.FleetInput, eDataTypes.EcosimFleetInput, eDataTypes.EcosimFleetOutput})
        Me.m_tscmbFilter.SelectedIndex = 0

        ' Populate sort box
        Me.AddSortOption(My.Resources.HEADER_SOURCE, ePropertySortOrderTypes.Source)
        Me.AddSortOption(My.Resources.HEADER_SOURCE_SEC, ePropertySortOrderTypes.SourceSec)
        Me.AddSortOption(My.Resources.HEADER_PARAMETER, ePropertySortOrderTypes.VarName)
        Me.m_tscmbSort.SelectedIndex = 0

        ' Create monitor, start tracking the monitor
        Me.m_monitor = New cRemarkMonitor(Me.PropertyManager)
        AddHandler Me.m_monitor.OnRemarksListChanged, AddressOf OnRemarkListChanged

        ' Kick off
        Me.InvalidateGrid()

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        ' Get rid of monitor
        RemoveHandler Me.m_monitor.OnRemarksListChanged, AddressOf OnRemarkListChanged
        Me.m_monitor.Dispose()
        Me.m_monitor = Nothing

        MyBase.OnFormClosed(e)

    End Sub

#End Region ' Form overloads

#Region " Events "

    Private Sub OnFilterChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tscmbFilter.SelectedIndexChanged

        Me.m_filter = DirectCast(Me.m_tscmbFilter.SelectedItem, cCoreComponentItem).Components
        Me.InvalidateGrid()

    End Sub

    Private Sub OnSortChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tscmbSort.SelectedIndexChanged

        Me.m_sortorder = DirectCast(Me.m_tscmbSort.SelectedItem, cSortItem).Sort
        Me.InvalidateGrid()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Callback for <see cref="cRemarkMonitor.OnRemarksListChanged">Remark monitor
    ''' list change events</see>.
    ''' </summary>
    ''' <param name="monitor">The monitor that fired the event.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnRemarkListChanged(ByRef monitor As cRemarkMonitor)
        ' Just invalidate the grid, which will get updated when there is time.
        Me.InvalidateGrid()
    End Sub

#End Region ' Events

#Region " Form config helpers "

    ''' <summary>
    ''' Add an item to the filter combo box.
    ''' </summary>
    ''' <param name="strDisplay"></param>
    ''' <param name="components"></param>
    Private Sub AddFilter(ByVal strDisplay As String, Optional ByVal components As eDataTypes() = Nothing)
        Me.m_tscmbFilter.Items.Add(New cCoreComponentItem(strDisplay, components))
    End Sub

    ''' <summary>
    ''' Add an item to the sort combo box.
    ''' </summary>
    ''' <param name="strDisplay"></param>
    ''' <param name="sort"></param>
    Private Sub AddSortOption(ByVal strDisplay As String, ByVal sort As ePropertySortOrderTypes)
        Me.m_tscmbSort.Items.Add(New cSortItem(strDisplay, sort))
    End Sub

#End Region ' Form config helpers

#Region " Invalidation "

    Private Sub InvalidateGrid()

        ' Optimization
        If Me.m_bInvalid Then Return
        ' Set invalid flag
        Me.m_bInvalid = True
        ' Update grid asynchronously when there is time
        Me.BeginInvoke(New MethodInvoker(AddressOf UpdateGrid), Nothing)

    End Sub

    Private Sub UpdateGrid()

        ' Optimization. May not be necessary
        If Not Me.m_bInvalid Then Return
        Me.m_bInvalid = False

        Dim lData As New List(Of cProperty)

        ' Apply filter
        For Each prop As cProperty In Me.m_monitor.Remarks
            Dim bInclude As Boolean = False
            If (Me.m_filter Is Nothing) Then
                bInclude = True
            Else
                bInclude = (Array.IndexOf(Me.m_filter, prop.Source.DataType) >= 0)
            End If
            If bInclude Then
                lData.Add(prop)
            End If
        Next

        ' Apply sort order
        lData.Sort(New cPropertySorter(Me.m_sortorder))

        ' Update the grid
        Me.m_grid.SetData(lData.ToArray())

    End Sub

#End Region ' Invalidation

End Class