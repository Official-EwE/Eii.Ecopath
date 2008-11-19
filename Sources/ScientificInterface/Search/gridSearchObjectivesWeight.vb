'==============================================================================
'
' $Log: gridSearchObjectivesWeight.vb,v $
' Revision 1.6  2008/11/19 14:46:10  jeroens
' Renamed a few resources
'
' Revision 1.5  2008/11/14 00:27:33  jeroens
' Fixed resource
'
' Revision 1.4  2008/11/13 00:42:12  jeroens
' Boundary weight shown for mpa/random search
'
' Revision 1.3  2008/11/12 23:24:52  jeroens
' BiomassDiversity used for all searches
'
' Revision 1.2  2008/11/12 22:33:39  jeroens
' BoundWeight not exposed by proper object
'
' Revision 1.1  2008/11/12 21:37:33  jeroens
' Renamed, moved
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
    Public Class gridSearchObjectivesWeight
        : Inherits EwEGrid

        Private m_core As cCore = Nothing
        Private m_manager As ISearchObjective = Nothing
        Private m_bIsBatchRun As Boolean = False
        Private m_bShowMaxPortUtil As Boolean = False
        Private m_bShowMPAOptParams As Boolean = False

        Public Sub New(ByVal theManager As ISearchObjective)
            MyBase.New()
            m_core = cCore.GetInstance()
            m_manager = theManager
            Me.FixedColumns = 1
        End Sub

        ' JS 12Nov08: not implemented yet
        'Public Property IsBatchRun() As Boolean
        '    Get
        '        Return Me.m_bIsBatchRun
        '    End Get
        '    Set(ByVal value As Boolean)
        '        Me.m_bIsBatchRun = value
        '        Me.RefreshContent
        '    End Set
        'End Property

        Public Property ShowMaxPortUtil() As Boolean
            Get
                Return Me.m_bShowMaxPortUtil
            End Get
            Set(ByVal value As Boolean)
                Me.m_bShowMaxPortUtil = value
                Me.RefreshContent()
            End Set
        End Property

        Public Property ShowMPAOptParams() As Boolean
            Get
                Return Me.m_bShowMPAOptParams
            End Get
            Set(ByVal value As Boolean)
                Me.m_bShowMPAOptParams = value
                Me.RefreshContent()
            End Set
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Dim iCol As Integer = 0
            ' Resize grid
            Me.Redim(Me.NumRows, Me.NumCols)

            ' == Add columns (for details refer to NumCols) ==

            ' Standard cols
            Me(0, iCol) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUE_COMPONENT) : iCol += 1
            ' Batch run specific cols
            If Me.m_bIsBatchRun Then
                Me(0, iCol) = New EwEColumnHeaderCell(My.Resources.HEADER_MINWEIGHT) : iCol += 1
                Me(0, iCol) = New EwEColumnHeaderCell(My.Resources.HEADER_MAXWEIGHT) : iCol += 1
                Me(0, iCol) = New EwEColumnHeaderCell(My.Resources.GENERIC_LABEL_STEP_SIZE) : iCol += 1
            Else
                Me(0, iCol) = New EwEColumnHeaderCell(My.Resources.HEADER_RELATIVEWEIGHT_ABBR) : iCol += 1
            End If

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreInputOutputBase = m_manager.ValueWeights
            Dim iRow As Integer = 1

            ' == POPULATE ROWS (for details refer to NumRows) ==
            ' JS 12Nov08: this code does not account for Batch run columns yet

            ' Standard rows
            Me(iRow, 0) = New EwERowHeaderCell(My.Resources.HEADER_NET_ECONOMIC_VALUE)
            Me(iRow, 1) = New PropertyCell(source, eVarNameFlags.FPSEconomicWeight)
            iRow += 1

            ' MaxPortUtil rows
            If m_bShowMaxPortUtil Then
                Me(iRow, 0) = New EwERowHeaderCell(My.Resources.HEADER_PREDICTIONVARIANCE)
                Me(iRow, 1) = New PropertyCell(source, eVarNameFlags.FPSPredictionVariance)
                iRow += 1

                Me(iRow, 0) = New EwERowHeaderCell(My.Resources.HEADER_EXISTENCE_VALUE)
                Me(iRow, 1) = New PropertyCell(source, eVarNameFlags.FPSExistenceValue)
                iRow += 1
            Else
                Me(iRow, 0) = New EwERowHeaderCell(My.Resources.HEADER_SOCIAL_VALUE_EMPLOYMENT)
                Me(iRow, 1) = New PropertyCell(source, eVarNameFlags.FPSSocialWeight)
                iRow += 1

                Me(iRow, 0) = New EwERowHeaderCell(My.Resources.HEADER_MANDATED_REBUILDING)
                Me(iRow, 1) = New PropertyCell(source, eVarNameFlags.FPSMandatedRebuildingWeight)
                iRow += 1

                Me(iRow, 0) = New EwERowHeaderCell(My.Resources.HEADER_ECOSYSTEM_STRUCTURE)
                Me(iRow, 1) = New PropertyCell(source, eVarNameFlags.FPSEcoSystemWeight)
                iRow += 1
            End If

            Me(iRow, 0) = New EwERowHeaderCell(My.Resources.HEADER_BIOMASS_DIVERSITY)
            Me(iRow, 1) = New PropertyCell(source, eVarNameFlags.FPSBiomassDiversityWeight)
            iRow += 1

            If Me.m_bShowMPAOptParams Then
                ' HACK
                Me(iRow, 0) = New EwERowHeaderCell(My.Resources.HEADER_BOUNDARYWEIGHT)
                Me(iRow, 1) = New PropertyCell(Me.m_core.MPAOptimizationManager.MPAOptimizationParamters, eVarNameFlags.MPAOptBoundaryWeight)
                iRow += 1
            Else

            End If

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumns = 1
        End Sub

        Private Function NumCols() As Integer

            ' Fixed col: ValueComponent
            Dim iNumCols As Integer = 1

            ' Batch run?
            If Me.m_bIsBatchRun Then
                ' #Yes: MinWeight, MaxWeight, Stepsize
                iNumCols += 3
            Else
                ' #No: RelWeight
                iNumCols += 1
            End If

            Return iNumCols

        End Function

        Private Function NumRows() As Integer

            ' Fixed rows: Header, NetEconValue, BiomassDiversity
            Dim iNumRows As Integer = 3

            ' MaxPortUtil?
            If Me.m_bShowMaxPortUtil Then
                ' #Yes: add PredictionVariance, ExistenceValue rows
                iNumRows += 2
            Else
                ' #No: add SocialValue, MandatedRebuilding, EcosystemStructure
                iNumRows += 3
            End If

            ' MPAOpt?
            If Me.m_bShowMPAOptParams Then
                ' #Yes: add BoundaryWeight
                iNumRows += 1
            Else
                ' #No: NOP
            End If

            Return iNumRows

        End Function

    End Class

End Namespace

