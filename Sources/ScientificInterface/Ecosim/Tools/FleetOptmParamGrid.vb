'==============================================================================
'
' $Log: FleetOptmParamGrid.vb,v $
' Revision 1.1  2008/09/26 07:31:52  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.9  2008/08/02 03:04:18  jeroens
' Renamed resources
'
' Revision 1.8  2008/06/25 16:02:49  jeroens
' ByRef -> ByVal
'
' Revision 1.7  2008/06/02 00:01:40  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.6  2008/05/29 22:23:01  jeroens
' Moved eVarNameFlags to EwEUtils
'
'==============================================================================

#Region "Imports directive"

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.SearchObjectives
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class FleetOptmParamGrid
        : Inherits EwEGrid

        Private m_Core As cCore
        Private m_Manager As ISearchObjective
        Private m_isMaxByFleetValue As Boolean = False

        Public Sub New(ByVal Manager As ISearchObjective)
            MyBase.New()
            m_Core = cCore.GetInstance()
            m_Manager = Manager

            m_isMaxByFleetValue = False
        End Sub

        Public Property IsMaximizeByFleetValue() As Boolean
            Get
                Return m_isMaxByFleetValue
            End Get
            Set(ByVal value As Boolean)
                m_isMaxByFleetValue = value
                'Todo: refresh the grid
                Me.SuspendLayout()
                Me.InitStyle()
                Me.FillData()
                Me.FinishStyle()
                Me.ResumeLayout()
            End Set
        End Property


        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            If Not m_isMaxByFleetValue Then
                Me.Redim(1, 2)
                Me(0, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_FLEET_GEAR)
                Me(0, 1) = New EwEColumnHeaderCell(My.Resources.FPS_FG_JOBS)
            Else
                Me.Redim(1, 3)
                Me(0, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_FLEET_GEAR)
                Me(0, 1) = New EwEColumnHeaderCell(My.Resources.FPS_FG_JOBS)
                Me(0, 2) = New EwEColumnHeaderCell(My.Resources.FPS_FG_TP)
            End If

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreGroupBase = Nothing

            For i As Integer = 1 To m_Core.nFleets
                source = m_Manager.FleetObjectives(i)
                Me.Rows.Insert(i)

                If Not m_isMaxByFleetValue Then
                    Me(i, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
                    Me(i, 1) = New PropertyCell(source, eVarNameFlags.FPSFleetJobCatchValue)
                Else
                    Me(i, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
                    Me(i, 1) = New PropertyCell(source, eVarNameFlags.FPSFleetJobCatchValue)
                    Me(i, 2) = New PropertyCell(source, eVarNameFlags.FPSFleetTargetProfit)
                End If

            Next

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumns = 1
        End Sub

    End Class

End Namespace


