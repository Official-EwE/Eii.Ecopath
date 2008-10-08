'==============================================================================
'
' $Log: gridTFMpolicy.vb,v $
' Revision 1.2  2008/10/08 20:47:49  jeroens
' Added CVBest, KalWT
'
' Revision 1.1  2008/10/08 17:57:36  jeroens
' Initial version
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports SourceGrid2
Imports SourceGrid2.Cells

#End Region ' Imports directive

Namespace Ecosim

    ''' ===========================================================================
    ''' <summary>
    ''' Grid to allow species quota interaction.
    ''' </summary>
    ''' ===========================================================================
    <CLSCompliant(False)> _
    Public Class gridSpeciesQuota
        Inherits EwEGrid

#Region " Internal defs "

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            BLim
            BBase
            FOpt
            CVBest
            Kalwt
        End Enum

#End Region ' Internal defs

#Region " Constructor "

        Public Sub New()
            MyBase.new()
        End Sub

#End Region ' Constructor

#Region " Public interfaces "

        Public ReadOnly Property SelectedGroup() As cEcoSimGroupInput
            Get
                If Me.Selection.SelectedRows.Length = 1 Then
                    Return DirectCast(Me.Selection.SelectedRows(0).Tag, cEcoSimGroupInput)
                End If
                Return Nothing
            End Get
        End Property

#End Region ' Public interfaces

#Region " Overrides "

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Dim core As cCore = cCore.GetInstance()
            Dim iNumCols As Integer = [Enum].GetValues(GetType(eColumnTypes)).Length
            Dim src As cCoreInputOutputBase = Nothing

            Me.Redim(1, iNumCols)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.BBase) = New EwEColumnHeaderCell("Biomass base")
            Me(0, eColumnTypes.BLim) = New EwEColumnHeaderCell("Biomass limit")
            Me(0, eColumnTypes.FOpt) = New EwEColumnHeaderCell("F optimum")
            Me(0, eColumnTypes.CVBest) = New EwEColumnHeaderCell("Biomass CV")
            Me(0, eColumnTypes.Kalwt) = New EwEColumnHeaderCell("Kalman weight")

            Me.FixedColumns = 2
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim group As cEcoSimGroupInput = Nothing

            ' For each group
            For iGroup As Integer = 1 To core.nGroups

                'Get the group info
                group = core.EcoSimGroupInputs(iGroup)

                Me.AddRow()

                Me(iGroup, eColumnTypes.Index) = New EwERowHeaderCell(iGroup)
                Me(iGroup, eColumnTypes.Name) = New PropertyRowHeaderCell(group, eVarNameFlags.Name)

                Me(iGroup, eColumnTypes.BBase) = New PropertyCell(group, eVarNameFlags.BBase)
                Me(iGroup, eColumnTypes.BLim) = New PropertyCell(group, eVarNameFlags.BLim)
                Me(iGroup, eColumnTypes.FOpt) = New PropertyCell(group, eVarNameFlags.Fopt)

                Me(iGroup, eColumnTypes.CVBest) = New PropertyCell(group, eVarNameFlags.RegCVBest)
                Me(iGroup, eColumnTypes.Kalwt) = New PropertyCell(group, eVarNameFlags.RegKalWt)

                Me.Rows(iGroup).Tag = group

            Next iGroup

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.Selection.SelectionMode = GridSelectionMode.Row
        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eMessageSource
            Get
                Return eMessageSource.EcoSim
            End Get
        End Property

#End Region ' Overrides

    End Class

End Namespace ' Ecosim
