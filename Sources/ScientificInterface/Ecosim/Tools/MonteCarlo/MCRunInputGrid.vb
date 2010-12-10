#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class MCRunInputGrid
        : Inherits EwEGrid

        Private m_value As eMCRunDisplayInputValueTypes = 0
        Private m_mcmanager As cMonteCarloManager = Nothing

        Public Sub New()
            MyBase.New()
        End Sub

        Public Property DisplayInputValue() As eMCRunDisplayInputValueTypes
            Get
                Return m_value
            End Get
            Set(ByVal value As eMCRunDisplayInputValueTypes)
                Me.m_value = value
                Me.RefreshContent()
            End Set
        End Property

        Public Overrides Property UIContext() As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(ByVal value As cUIContext)
                If (value IsNot Nothing) Then
                    Me.m_mcmanager = value.Core.EcosimMonteCarlo
                Else
                    Me.m_mcmanager = Nothing
                End If
                MyBase.UIContext = value
            End Set
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Me.Redim(Me.Core.nLivingGroups + 1, 6)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell(SharedResources.HEADER_CV)
            Me(0, 3) = New EwEColumnHeaderCell(SharedResources.HEADER_LOWERLIMIT)
            Me(0, 4) = New EwEColumnHeaderCell(SharedResources.HEADER_MEAN)
            Me(0, 5) = New EwEColumnHeaderCell(SharedResources.HEADER_UPPERLIMIT)

            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub FillData()

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

            For i As Integer = 1 To Me.Core.nLivingGroups
                mcGrp = Me.m_mcmanager.Groups(i)
                Me(i, 0) = New EwERowHeaderCell(mcGrp.Index)
                Me(i, 1) = New PropertyRowHeaderCell(Me.PropertyManager, mcGrp, eVarNameFlags.Name)
                Me(i, 2) = New PropertyCell(Me.PropertyManager, mcGrp, flags(0))
                Me(i, 3) = New PropertyCell(Me.PropertyManager, mcGrp, flags(1))
                Me(i, 4) = New PropertyCell(Me.PropertyManager, mcGrp, flags(2))
                Me(i, 5) = New PropertyCell(Me.PropertyManager, mcGrp, flags(3))
            Next

        End Sub

    End Class

End Namespace


