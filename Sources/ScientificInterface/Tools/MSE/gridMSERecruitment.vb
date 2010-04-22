#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.MSE
Imports EwEUtils.Core
Imports SourceGrid2
Imports SourceGrid2.Cells

#End Region ' Imports

Namespace Ecosim

    ''' ===========================================================================
    ''' <summary>
    ''' Grid to allow species quota interaction.
    ''' </summary>
    ''' ===========================================================================
    <CLSCompliant(False)> _
    Public Class gridMSERecruitment
        Inherits EwEGrid

#Region " Internal defs "

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            ForcastGain
            RHalfB
        End Enum

#End Region ' Internal defs

#Region " Constructor "

        Public Sub New()
            MyBase.new()
        End Sub

#End Region ' Constructor

#Region " Public interfaces "

        Public Property Group() As cEcoSimGroupInput
            Get
                If Me.Selection.SelectedRows.Length = 1 Then
                    Return DirectCast(Me.Selection.SelectedRows(0).Tag, cEcoSimGroupInput)
                End If
                Return Nothing
            End Get
            Set(ByVal value As cEcoSimGroupInput)
                Me.Selection.Clear()
                If value IsNot Nothing Then
                    Me.Selection.Add(New Position(value.Index, 0))
                End If
                Me.RaiseSelectionChangeEvent()
            End Set
        End Property

#End Region ' Public interfaces

#Region " Overrides "

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Dim iNumCols As Integer = [Enum].GetValues(GetType(eColumnTypes)).Length

            Me.Redim(1, iNumCols)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.RHalfB) = New EwEColumnHeaderCell("Ratio Bt/B0 for 50% recruitment")
            Me(0, eColumnTypes.ForcastGain) = New EwEColumnHeaderCell("Forcast gain")

            Me.FixedColumns = 2
            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub FillData()

            Dim group As cMSEGroupInput = Nothing

            ' For each group
            For iGroup As Integer = 1 To Core.nGroups

                'Get the group info
                group = Core.MSEManager.GroupInputs(iGroup)

                Me.AddRow()

                Me(iGroup, eColumnTypes.Index) = New EwERowHeaderCell(iGroup)
                Me(iGroup, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)

                Me(iGroup, eColumnTypes.RHalfB) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.RHalfB0Ratio)
                Me(iGroup, eColumnTypes.ForcastGain) = New PropertyCell(Me.PropertyManager, group, eVarNameFlags.MSEForcastGain)

                Me.Rows(iGroup).Tag = group

            Next iGroup

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.Selection.SelectionMode = GridSelectionMode.Row
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.MSE
            End Get
        End Property

#End Region ' Overrides

    End Class

End Namespace ' Ecosim
