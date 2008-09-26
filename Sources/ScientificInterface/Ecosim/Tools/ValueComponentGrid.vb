#Region "Imports directive"

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.SearchObjectives
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class ValueComponentGrid
        : Inherits EwEGrid

        Private m_Core As cCore
        ' Private m_FPManager As cFishingPolicyManager
        Private m_Manager As ISearchObjective
        Private m_IsBatchRun As Boolean = False
        Private m_IsMaxPortUtil As Boolean = False

        Public Sub New(ByVal theManager As ISearchObjective)
            MyBase.New()
            m_Core = cCore.GetInstance()
            m_Manager = theManager
            Me.FixedColumns = 1
        End Sub

        Public Property IsBatchRun() As Boolean
            Get
                Return m_IsBatchRun
            End Get
            Set(ByVal value As Boolean)
                m_IsBatchRun = value
                'Todo: Refresh the grid
            End Set
        End Property

        Public Property IsMaxPortUtil() As Boolean
            Get
                Return m_IsMaxPortUtil
            End Get
            Set(ByVal value As Boolean)
                m_IsMaxPortUtil = value
                Me.FillData()
            End Set
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            If Not m_IsBatchRun Then
                If Not m_IsMaxPortUtil Then
                    Me.Redim(5, 2)
                Else
                    Me.Redim(4, 2)
                End If
                Me(0, 0) = New EwEColumnHeaderCell(My.Resources.FPS_VALUE_COMPONENT)
                Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_RELATIVEWEIGHT_ABBR)
            Else

                If Not m_IsMaxPortUtil Then
                    Me.Redim(5, 4)
                Else
                    Me.Redim(4, 4)
                End If

                Me(0, 0) = New EwEColumnHeaderCell(My.Resources.FPS_VALUE_COMPONENT)
                Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_MINWEIGHT)
                Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_MAXWEIGHT)
                Me(0, 3) = New EwEColumnHeaderCell(My.Resources.FPS_VC_STEPSIZE)
            End If

            Me.Dock = DockStyle.Fill

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreInputOutputBase = m_Manager.ValueWeights

            If Not m_IsBatchRun Then

                If Not m_IsMaxPortUtil Then
                    Me(1, 0) = New EwERowHeaderCell(My.Resources.HEADER_NETECONOMICVALUE)
                    Me(1, 1) = New PropertyCell(source, eVarNameFlags.FPSEconomicWeight)

                    Me(2, 0) = New EwERowHeaderCell(My.Resources.FPS_VC_NET_SOCIAL_VALUE)
                    Me(2, 1) = New PropertyCell(source, eVarNameFlags.FPSSocialWeight)

                    Me(3, 0) = New EwERowHeaderCell(My.Resources.FPS_VC_NET_MANDATED_REBUILDING)
                    Me(3, 1) = New PropertyCell(source, eVarNameFlags.FPSMandatedRebuildingWeight)

                    If Me.RowsCount = 4 Then
                        Me.Rows.Insert(4)
                    End If
                    Me(4, 0) = New EwERowHeaderCell(My.Resources.FPS_VC_NET_ECOSYSTEM_STRUCTURE)
                    Me(4, 1) = New PropertyCell(source, eVarNameFlags.FPSEcoSystemWeight)

                Else
                    Me(1, 0) = New EwERowHeaderCell(My.Resources.HEADER_NETECONOMICVALUE)
                    Me(1, 1) = New PropertyCell(source, eVarNameFlags.FPSEconomicWeight)

                    Me(2, 0) = New EwERowHeaderCell(My.Resources.HEADER_PREDICTIONVARIANCE)
                    Me(2, 1) = New PropertyCell(source, eVarNameFlags.FPSPredictionVariance)

                    Me(3, 0) = New EwERowHeaderCell(My.Resources.FPS_VC_NET_EXISTENCE_VALUE)
                    Me(3, 1) = New PropertyCell(source, eVarNameFlags.FPSExistenceValue)

                    'Me.Rows(4).Visible = false property in SG2 does not work
                    If Me.RowsCount = 5 Then
                        Me.Rows.Remove(4)
                    End If

                End If

            End If

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumns = 1
        End Sub

    End Class

End Namespace

