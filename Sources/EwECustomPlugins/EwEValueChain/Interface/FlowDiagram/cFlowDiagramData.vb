Imports ScientificInterfaceShared.Controls

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
Public Class cFlowDiagramData
    Implements IFlowDiagramData

#Region " Private vars "

    Private m_data As cData = Nothing
    Private m_uic As cUIContext = Nothing

    ' Units, to be accessed by iGroup. Nyuk nyuk nyuk
    Private m_lUnits As New List(Of cUnit)
    Private m_nLivingGroups As Integer

#End Region ' Private vars

    Public Sub New(ByVal uic As cUIContext, ByVal data As cData)

        Me.m_uic = uic
        Me.m_data = data

        Me.m_lUnits.AddRange(Me.m_data.GetUnits(cUnitFactory.eUnitType.Producer))
        Me.m_lUnits.AddRange(Me.m_data.GetUnits(cUnitFactory.eUnitType.Distribution))
        Me.m_lUnits.AddRange(Me.m_data.GetUnits(cUnitFactory.eUnitType.Processing))
        Me.m_lUnits.AddRange(Me.m_data.GetUnits(cUnitFactory.eUnitType.Wholesaler))
        Me.m_lUnits.AddRange(Me.m_data.GetUnits(cUnitFactory.eUnitType.Retailer))
        Me.m_nLivingGroups = Me.m_lUnits.Count
        Me.m_lUnits.AddRange(Me.m_data.GetUnits(cUnitFactory.eUnitType.Consumer))

    End Sub

#Region " Properties "

    Public ReadOnly Property GroupColor(iGroup As Integer) As System.Drawing.Color _
        Implements IFlowDiagramData.GroupColor
        Get
            Select Case Me.GetUnit(iGroup).UnitType
                Case cUnitFactory.eUnitType.Consumer
                Case cUnitFactory.eUnitType.Distribution
                Case cUnitFactory.eUnitType.Processing
                Case cUnitFactory.eUnitType.Producer
                Case cUnitFactory.eUnitType.Retailer
                Case cUnitFactory.eUnitType.Wholesaler
            End Select
        End Get
    End Property

    Public ReadOnly Property GroupName(iGroup As Integer) As String _
        Implements IFlowDiagramData.GroupName
        Get
            Dim u As cUnit = Me.GetUnit(iGroup)
            Dim strName As String = ""
            If My.Settings.ShowAltNames Then strName = u.NameLocal
            If String.IsNullOrWhiteSpace(strName) Then strName = u.Name
            Return strName
        End Get
    End Property

    Public ReadOnly Property IsGroupVisible(iGroup As Integer) As Boolean _
        Implements IFlowDiagramData.IsGroupVisible
        Get
            Return True
        End Get
    End Property

    Public ReadOnly Property LinkValue(iPred As Integer, iPrey As Integer) As Single _
        Implements IFlowDiagramData.LinkValue
        Get
            ' hmm
        End Get
    End Property

    Public ReadOnly Property LinkValueMax As Single _
        Implements IFlowDiagramData.LinkValueMax
        Get

        End Get
    End Property

    Public ReadOnly Property LinkValueMin As Single _
        Implements IFlowDiagramData.LinkValueMin
        Get

        End Get
    End Property

    Public ReadOnly Property NumGroups As Integer _
        Implements IFlowDiagramData.NumGroups
        Get
            Return Me.m_lUnits.Count
        End Get
    End Property

    Public ReadOnly Property NumLivingGroups As Integer _
        Implements IFlowDiagramData.NumLivingGroups
        Get
            Return Me.m_nLivingGroups
        End Get
    End Property

    Public ReadOnly Property Rank(iGroup As Integer) As Single _
        Implements IFlowDiagramData.Rank
        Get

        End Get
    End Property

    Public Sub Refresh() _
        Implements IFlowDiagramData.Refresh

    End Sub

    Public ReadOnly Property Value(iGroup As Integer) As Single _
        Implements IFlowDiagramData.Value
        Get

        End Get
    End Property

    Public ReadOnly Property ValueLabel(sValue As Single) As String _
        Implements IFlowDiagramData.ValueLabel
        Get
            Return Me.m_uic.StyleGuide.FormatNumber(sValue)
        End Get
    End Property

    Public ReadOnly Property ValueMax As Single _
        Implements IFlowDiagramData.ValueMax
        Get

        End Get
    End Property

    Public ReadOnly Property ValueMin As Single _
        Implements IFlowDiagramData.ValueMin
        Get

        End Get
    End Property

    Public Property UIContext As cUIContext _
        Implements IUIElement.UIContext
        Get
            Return Me.m_uic
        End Get
        Private Set(value As cUIContext)
            ' NOP
        End Set
    End Property

#End Region ' Properties

#Region " Private vars "

    Private Function GetUnit(iGroup As Integer) As cUnit
        Return Me.m_lUnits(iGroup)
    End Function

#End Region ' Private vars

End Class
