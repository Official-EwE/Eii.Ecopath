'==============================================================================
' $Log: gridFitToTimeSeriesGroup.vb,v $
' Revision 1.1  2008/11/27 20:56:11  sherman
' Switched MaxFishing Mortality to Search routines.
'
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
    Public Class gridFitToTimeSeriesGroup
        : Inherits EwEGrid

        Private m_core As cCore
        Private m_manager As ISearchObjective

        Private Enum eColumnTypes As Integer
            Group = 0
            FLimit
        End Enum

        Public Sub New(ByVal Manager As ISearchObjective)
            MyBase.New()
            Me.m_core = cCore.GetInstance()
            Me.m_manager = Manager
        End Sub

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Group) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUP)
            Me(0, eColumnTypes.FLimit) = New EwEColumnHeaderCell(My.Resources.GENERIC_LABEL_MAXFISHINGMORTAILITY)

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreGroupBase = Nothing

            For i As Integer = 1 To m_core.nGroups
                source = m_manager.GroupObjectives(i)

                Me.Rows.Insert(i)
                Me(i, eColumnTypes.Group) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
                Me(i, eColumnTypes.FLimit) = New PropertyCell(source, eVarNameFlags.FPSFishingLimit)
            Next

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumns = 1
        End Sub

        Protected Overrides Function DefaultDockStyle() As System.Windows.Forms.DockStyle
            Return DockStyle.Fill
        End Function

    End Class

End Namespace


