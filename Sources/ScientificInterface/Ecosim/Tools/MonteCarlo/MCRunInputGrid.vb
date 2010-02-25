#Region " Imports "

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

        Private m_value As eMCRunDisplayInputValueTypes = 0
        Private m_core As cCore = Nothing
        Private m_mcmanager As cMonteCarloManager = Nothing

        Public Sub New()
            MyBase.New()
        End Sub

        Public Property DisplayInputValue() As eMCRunDisplayInputValueTypes
            Get
                Return m_value
            End Get
            Set(ByVal value As eMCRunDisplayInputValueTypes)
                Me.m_core = cCore.GetInstance()
                Me.m_mcmanager = m_core.EcosimMonteCarlo
                Me.m_value = value
                Me.RefreshContent()
            End Set
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            If Me.m_core Is Nothing Then Return

            Me.Redim(m_core.nLivingGroups + 1, 6)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.MCRUN_HEADER_CV)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_LOWERLIMIT)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_MEAN)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_UPPERLIMIT)

            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub FillData()

            If Me.m_core Is Nothing Then Return

            Select Case m_value
                Case eMCRunDisplayInputValueTypes.B
                    Me.FillValue(New eVarNameFlags() {eVarNameFlags.mcBcv, eVarNameFlags.mcBLower, eVarNameFlags.mcB, eVarNameFlags.mcBUpper})
                Case eMCRunDisplayInputValueTypes.PB
                    Me.FillValue(New eVarNameFlags() {eVarNameFlags.mcPBcv, eVarNameFlags.mcPBLower, eVarNameFlags.mcPB, eVarNameFlags.mcPBUpper})
                Case eMCRunDisplayInputValueTypes.EE
                    Me.FillValue(New eVarNameFlags() {eVarNameFlags.mcEEcv, eVarNameFlags.mcEELower, eVarNameFlags.mcEE, eVarNameFlags.mcEEUpper})
                Case eMCRunDisplayInputValueTypes.BA
                    Me.FillValue(New eVarNameFlags() {eVarNameFlags.mcBAcv, eVarNameFlags.mcBALower, eVarNameFlags.mcBA, eVarNameFlags.mcBAUpper})
                Case eMCRunDisplayInputValueTypes.VU
                    Me.FillValue(New eVarNameFlags() {eVarNameFlags.mcVUcv, eVarNameFlags.mcVULower, eVarNameFlags.mcVU, eVarNameFlags.mcVUUpper})
            End Select

        End Sub

        Private Sub FillValue(ByVal flags() As eVarNameFlags)

            Dim mcGrp As cCoreGroupBase = Nothing
            'Dim mcGroup As cMonteCarloGroup = Nothing

            For i As Integer = 1 To m_core.nLivingGroups
                mcGrp = m_mcmanager.Groups(i)
                Me(i, 0) = New EwERowHeaderCell(mcGrp.Index)
                Me(i, 1) = New EwERowHeaderCell(mcGrp.Name)
                Me(i, 2) = New PropertyCell(Me.PropertyManager, mcGrp, flags(0))
                Me(i, 3) = New PropertyCell(Me.PropertyManager, mcGrp, flags(1))
                Me(i, 4) = New PropertyCell(Me.PropertyManager, mcGrp, flags(2))
                Me(i, 5) = New PropertyCell(Me.PropertyManager, mcGrp, flags(3))
            Next

        End Sub

    End Class

End Namespace


