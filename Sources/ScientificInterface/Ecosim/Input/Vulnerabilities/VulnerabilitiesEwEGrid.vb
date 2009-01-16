#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports SourceGrid2
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class VulnerabilitiesEwEGrid
        Inherits EwEGrid

        Private m_Core As cCore = cCore.GetInstance()
        Private m_RowColClick As New BehaviorModels.CustomEvents

        Private m_VisDiagonal As New SourceGrid2.VisualModels.Common

        Public Sub New()
            MyBase.New()
            m_VisDiagonal.BackColor = Color.LightGray
            m_VisDiagonal.TextAlignment = ContentAlignment.MiddleCenter
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            ' Define grid dimensions
            Dim source As cCoreGroupBase = Nothing
            Me.Redim(m_Core.nGroups + 1, 2)

            ' Set header cells
            ' # (0,0)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_PREYPREDATOR)

            Dim columnIndex As Integer = 2

            For i As Integer = 1 To m_Core.nGroups
                source = m_Core.EcoPathGroupInputs(i)
                ' Group index header cell
                Me(i, 0) = New EwERowHeaderCell(i)
                Me(i, 0).Behaviors.Add(m_RowColClick)

                ' # Group name row header cells
                Me(i, 1) = New EwERowHeaderCell(source.Name)
                Me(i, 1).Behaviors.Add(m_RowColClick)

                If source.PP < 1 Then
                    Me.Columns.Insert(columnIndex)
                    Me(0, columnIndex) = New PropertyColumnHeaderCell(source, eVarNameFlags.Index)
                    Me(0, columnIndex).Behaviors.Add(m_RowColClick)
                    columnIndex = columnIndex + 1
                End If
            Next

        End Sub

        Protected Overrides Sub FillData()
            Dim grpPrey As cCoreGroupBase = Nothing
            Dim grpPred As cCoreGroupBase = Nothing
            Dim iCol As Integer = 2
            Dim prop As cProperty = Nothing
            Dim cell As PropertyCell = Nothing
            Dim pm As cPropertyManager = cPropertyManager.GetInstance()

            ' Populate grid data cells
            For iPrey As Integer = 1 To m_Core.nGroups
                grpPrey = m_Core.EcoSimGroupInputs(iPrey)
                iCol = 2
                For iPred As Integer = 1 To m_Core.nLivingGroups
                    ' JS 16may08: Use ecopath groups for sec indexes
                    grpPred = m_Core.EcoPathGroupInputs(iPred)

                    If grpPred.PP < 1 Then

                        prop = pm.GetProperty(grpPrey, eVarNameFlags.VulMult, grpPred)
                        cell = New PropertyCell(prop)
                        cell.SuppressZero = True

                        If iPrey = (iCol - 1) Then
                            cell.VisualModel = m_VisDiagonal
                        End If

                        ' Store cell
                        Me(iPrey, iCol) = cell

                        ' Next column
                        iCol += 1
                    End If
                Next
            Next
        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eCoreComponentType
            Get
                Return eCoreComponentType.EcoSim
            End Get
        End Property

    End Class

End Namespace
