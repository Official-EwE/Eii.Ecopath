'==============================================================================
'
' $Log: MCRunInputGrid.vb,v $
' Revision 1.1  2008/09/26 07:31:47  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.10  2008/09/26 00:22:50  villyc
' updating ecosimMonteCarlo to pick vulnerabilities
'
' Revision 1.9  2008/08/12 16:09:11  jeroens
' Fixed header style
'
' Revision 1.8  2008/08/02 03:04:15  jeroens
' Renamed resources
'
' Revision 1.7  2008/07/31 21:29:11  sherman
' Removed dead groups bug
'
' Revision 1.6  2008/06/02 00:01:33  jeroens
' Added ScientificInterfaceShared
'
'==============================================================================

#Region "Imports Directive"

Option Strict On
Option Explicit On

Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class MCRunInputGrid
        : Inherits EwEGrid

        Private m_DisplayInputValue As MCRunDisplayInputValue
        Private m_Core As cCore
        Private m_McManager As cMonteCarloManager = Nothing

        Public Sub New()
            MyBase.New()
            m_Core = cCore.GetInstance()
            m_McManager = m_Core.EcosimMonteCarlo
        End Sub

        Public Property DisplayInputValue() As MCRunDisplayInputValue
            Get
                Return m_DisplayInputValue
            End Get
            Set(ByVal value As MCRunDisplayInputValue)
                m_DisplayInputValue = value
            End Set
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(m_Core.nLivingGroups + 1, 6)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.MCRUN_HEADER_CV)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_LOWERLIMIT)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_MEAN)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_UPPERLIMIT)

            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub FillData()

            Select Case m_DisplayInputValue
                Case MCRunDisplayInputValue.B
                    FillValue(New eVarNameFlags() {eVarNameFlags.mcBcv, eVarNameFlags.mcBLower, eVarNameFlags.mcB, eVarNameFlags.mcBUpper})
                Case MCRunDisplayInputValue.PB
                    FillValue(New eVarNameFlags() {eVarNameFlags.mcPBcv, eVarNameFlags.mcPBLower, eVarNameFlags.mcPB, eVarNameFlags.mcPBUpper})
                Case MCRunDisplayInputValue.EE
                    FillValue(New eVarNameFlags() {eVarNameFlags.mcEEcv, eVarNameFlags.mcEELower, eVarNameFlags.mcEE, eVarNameFlags.mcEEUpper})
                Case MCRunDisplayInputValue.BA
                    FillValue(New eVarNameFlags() {eVarNameFlags.mcBAcv, eVarNameFlags.mcBALower, eVarNameFlags.mcBA, eVarNameFlags.mcBAUpper})
                Case MCRunDisplayInputValue.VU
                    FillValue(New eVarNameFlags() {eVarNameFlags.mcVUcv, eVarNameFlags.mcVULower, eVarNameFlags.mcVU, eVarNameFlags.mcVUUpper})
            End Select

        End Sub

        Private Sub FillValue(ByVal flags() As eVarNameFlags)

            Dim mcGrp As cCoreGroupBase = Nothing
            'Dim mcGroup As cMonteCarloGroup = Nothing

            For i As Integer = 1 To m_Core.nLivingGroups
                mcGrp = m_McManager.Groups(i)
                Me(i, 0) = New EwERowHeaderCell(mcGrp.Index)
                Me(i, 1) = New EwERowHeaderCell(mcGrp.Name)
                Me(i, 2) = New PropertyCell(mcGrp, flags(0))
                Me(i, 3) = New PropertyCell(mcGrp, flags(1))
                Me(i, 4) = New PropertyCell(mcGrp, flags(2))
                Me(i, 5) = New PropertyCell(mcGrp, flags(3))
            Next

        End Sub

    End Class

End Namespace


