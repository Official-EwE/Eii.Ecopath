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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.MSE
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2
Imports SourceGrid2.Cells
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Grid to allow species quota interaction.
''' </summary>
''' ===========================================================================
<CLSCompliant(False)> _
Public Class gridMaxDecreaseEffort
    Inherits EwEGrid

#Region " Internal defs "

    'Private Class cDistributionTypeFormatter
    '    Implements ITypeFormatter

    '    Public Function GetDescribedType() As System.Type Implements ITypeFormatter.GetDescribedType
    '        Return GetType(cMSE.DistributionType)
    '    End Function

    '    Public Function GetDescriptor(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
    '        Implements ITypeFormatter.GetDescriptor
    '        Select Case (DirectCast(value, cMSE.DistributionType))
    '            Case cMSE.DistributionType.Triangular
    '                Return My.Resources.DISTR_TYPE_TRIANGULAR
    '            Case cMSE.DistributionType.Uniform
    '                Return My.Resources.DISTR_TYPE_UNIFORM
    '            Case Else
    '                Debug.Assert(False)
    '        End Select
    '        Return "?"
    '    End Function

    'End Class

    Private Enum eMaxDecreaseColumnTypes As Integer
        FleetIndex = 0
        FleetName
        MaxChangeEffort
    End Enum

#End Region ' Internal defs

    ''' <summary>The cMSE Plugin that contains the data.</summary>
    Private MSEPlugin As cMSE
    Private m_data As frmEditDecreaseEffort.cDecreaseInEffort() = Nothing

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region ' Constructor

#Region " Public access "

    Public Sub Init(Plugin As cMSE)
        MSEPlugin = Plugin
    End Sub

    'Public Property Mode As frmDistributionParameters.eParameterSet
    '    Get
    '        Return Me.m_mode
    '    End Get
    '    Set(value As frmDistributionParameters.eParameterSet)
    '        If (Me.m_mode <> value) Then
    '            Me.m_mode = value
    '            Me.m_data = Nothing
    '            Me.RefreshContent()
    '        End If
    '    End Set
    'End Property

    Public Property Data As frmEditDecreaseEffort.cDecreaseInEffort()
        Get
            Return Me.m_data
        End Get
        Set(value As frmEditDecreaseEffort.cDecreaseInEffort())
            Me.m_data = value
            Me.FillData()
        End Set
    End Property

    Public Event onEdited()

#End Region ' Public access

#Region " Overrides "

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Dim iNumCols As Integer = [Enum].GetValues(GetType(eMaxDecreaseColumnTypes)).Length
        Me.Redim(1, iNumCols)

        Me(0, eMaxDecreaseColumnTypes.FleetIndex) = New EwEColumnHeaderCell("")
        'TODO change the strings for these headers into shareresources
        Me(0, eMaxDecreaseColumnTypes.FleetName) = New EwEColumnHeaderCell("Fleet Name")
        Me(0, eMaxDecreaseColumnTypes.MaxChangeEffort) = New EwEColumnHeaderCell("Max Change in Effort")

        Me.FixedColumns = 2
        Me.FixedColumnWidths = False
        Me.AllowBlockSelect = False

    End Sub

    Protected Overrides Sub FillData()

        If (Me.m_data Is Nothing) Then Return

        Dim iRow As Integer = -1
        Dim cell As EwECell = Nothing
        'Dim lstOptions As New List(Of cMSE.DistributionType)
        'lstOptions.AddRange(DirectCast([Enum].GetValues(GetType(cMSE.DistributionType)), IEnumerable(Of cMSE.DistributionType)))
        'Dim cb As EwEComboBoxCellEditor = New EwEComboBoxCellEditor(New cDistributionTypeFormatter(), lstOptions)

        Me.RowsCount = 1

        For i As Integer = 0 To Me.m_data.Length - 1
            iRow = Me.AddRow()

            Dim data As frmEditDecreaseEffort.cDecreaseInEffort = DirectCast(Me.m_data(i), frmEditDecreaseEffort.cDecreaseInEffort)
            Dim fleet As cFleetInput = Me.Core.FleetInputs(data.FleetIndex)
            'Dim sg As cStanzaGroup = Nothing
            'Dim bUse As Boolean = True

            'If group.isMultiStanza Then
            '    sg = Me.Core.StanzaGroups(group.iStanza)
            '    bUse = (sg.iGroups(sg.LeadingB) = data.GroupNo)
            'End If

            Me(iRow, eMaxDecreaseColumnTypes.FleetIndex) = New EwERowHeaderCell(CStr(data.FleetIndex))
            Me(iRow, eMaxDecreaseColumnTypes.FleetName) = New EwERowHeaderCell(CStr(data.FleetName))
            Me(iRow, eMaxDecreaseColumnTypes.MaxChangeEffort) = New EwERowHeaderCell(CStr(data.MaxDecreaseInEffort))


            Me.Rows(iRow).Tag = data

        Next

        Me.Columns(eMaxDecreaseColumnTypes.FleetIndex).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
        Me.Columns(eMaxDecreaseColumnTypes.FleetName).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
        Me.AutoSizeColumn(eMaxDecreaseColumnTypes.MaxChangeEffort, 150)

        'Me.Columns(eMaxDecreaseColumnTypes.FleetIndex).AutoSizeMode = SourceGrid2.AutoSizeMode.None
        'Me.Columns(eMaxDecreaseColumnTypes.FleetName).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize Or SourceGrid2.AutoSizeMode.EnableStretch
        'Me.AutoSizeColumn(eMaxDecreaseColumnTypes.MaxChangeEffort, 150)

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
    End Sub

    Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
        Get
            Return eCoreComponentType.EcoSim
        End Get
    End Property

    Private Function DataCell(dValue As Double) As EwECell

        Dim style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK
        Dim cell As EwECell = Nothing

        If (dValue = cCore.NULL_VALUE) Then
            style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null
        End If

        cell = New EwECell(CSng(dValue), GetType(Single), style)
        cell.Behaviors.Add(Me.EwEEditHandler)
        Return cell

    End Function

    Protected Overrides Function OnCellEdited(p As SourceGrid2.Position, cell As SourceGrid2.Cells.ICellVirtual) As Boolean

                Dim tag As Object = Me.Rows(p.Row).Tag
                If (tag Is Nothing) Then Return False

                Debug.Assert(TypeOf tag Is frmDistributionParameters.EcopathParam)

        Dim data As frmEditDecreaseEffort.cDecreaseInEffort = DirectCast(tag, frmEditDecreaseEffort.cDecreaseInEffort)

        data.MaxDecreaseInEffort = CDbl(cell.GetValue(p))

        Me.RaiseDataChangeEvent()
        Return MyBase.OnCellEdited(p, cell)

    End Function

    Private Sub RaiseDataChangeEvent()
        Try
            RaiseEvent onEdited()
        Catch ex As Exception

        End Try
    End Sub

#End Region ' Overrides

End Class


