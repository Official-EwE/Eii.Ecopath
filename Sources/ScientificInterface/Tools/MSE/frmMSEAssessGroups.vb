#Region " Imports "

Option Strict On
Imports ScientificInterface.Ecosim
Imports EwECore.MSE

#End Region ' Imports

Public Class frmMSEAssessGroups

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Public Overrides Property UIContext() As cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(ByVal value As cUIContext)
            MyBase.UIContext = value
            Me.m_blocks.UIContext = value
        End Set
    End Property


    Private Sub frmMSEAssessGroups_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'attach the datasource and the block selector to the ucPolicyColorBlocks control
        Dim ds As New cMSEGroupColorBlockDataSource(Me.UIContext)
        Me.m_blocks.Attach(ds, New ucCVBlockSelector)

    End Sub


End Class


#Region "IPolicyColorBlockDataSource implementation for MSE"


Public Class cMSEGroupColorBlockDataSource
    Implements IPolicyColorBlockDataSource

    Private m_uic As cUIContext

    Private m_BlockCells(,) As Integer

    Private m_blockCodes As ucCVBlockSelector
    Private m_iTotalBlocks As Integer
    Private m_batchEdit As Boolean

    Public ReadOnly Property BlockCells() As Integer(,) Implements IPolicyColorBlockDataSource.BlockCells
        Get
            Return m_BlockCells
        End Get
    End Property


    Public ReadOnly Property TotalBlocks() As Integer Implements IPolicyColorBlockDataSource.TotalBlocks
        Get
            Return Me.m_uic.Core.EcoSimModelParameters.NumberYears
        End Get
    End Property

    Public Sub New(ByVal UIContext As cUIContext)
        Me.m_uic = UIContext
    End Sub

    Public Sub Atatch(ByVal Blocks As IBlockSelector) Implements IPolicyColorBlockDataSource.Atatch

        Debug.Assert(TypeOf Blocks Is ucCVBlockSelector, Me.ToString & ".Atatch() Blocks must be a ucCVBlockSelector!")
        Try
            m_blockCodes = DirectCast(Blocks, ucCVBlockSelector)

            'populate the blocks with values from the data!!!!
            Dim cvs As New List(Of Single)
            cvs.Add(0) 'if adding values the first value should be zero
            Dim manager As cMSEManager = Me.m_uic.Core.MSEManager
            Dim blks() As Single = Me.m_blockCodes.BlockValues

            For i As Integer = 1 To Me.m_uic.Core.nGroups
                Dim grp As cMSEGroupInput = manager.GroupInputs(i)
                For it As Integer = 1 To Me.m_uic.Core.nEcosimYears
                    Dim cv As Single = grp.BiomassCV(it)

                    If Not blks.Contains(cv) Then
                        If Not cvs.Contains(cv) Then
                            cvs.Add(cv)
                        End If
                    End If

                Next ' Me.m_uic.Core.nEcosimYears
            Next '  Me.m_uic.Core.nFleets

            'cvs in the datasource that are not in the control
            If cvs.Count > 1 Then

                For iblk As Integer = 1 To Me.m_blockCodes.NumBlocks
                    cvs.Insert(iblk, blks(iblk))
                Next ' For iblk As Integer = 1 To Me.m_blockCodes.NumBlocks
                cvs.Sort()
                m_blockCodes.BlockValues = cvs.ToArray
            End If

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    Public Sub Init() Implements IPolicyColorBlockDataSource.Init

        m_iTotalBlocks = Me.m_uic.Core.EcoSimModelParameters.NumberYears

        ReDim m_BlockCells(Me.nRows, Me.TotalBlocks)
        Dim mseData As cMSEGroupInput

        For igrp As Integer = 1 To m_BlockCells.GetLength(0) - 1
            mseData = Me.m_uic.Core.MSEManager.GroupInputs(igrp)
            For iTime As Integer = 1 To m_BlockCells.GetLength(1) - 1
                m_BlockCells(igrp, iTime) = Me.m_blockCodes.ValuetoBlock(mseData.BiomassCV(iTime))
            Next
        Next

    End Sub

    Public Sub FillBlock(ByVal iRow As Integer, ByVal iCol As Integer) Implements IPolicyColorBlockDataSource.FillBlock

        ' Sanity checks
        'If (iCol <= Me.m_uic.Core.FishingPolicyManager.ObjectiveParameters.BaseYear) Then Return

        If (iRow < 1) Then Return
        If (iRow > m_BlockCells.GetLength(0) - 1) Then Return

        ' Fill single block

        Me.m_BlockCells(iRow, iCol) = Me.m_blockCodes.SelectedBlock
        Me.m_uic.Core.MSEManager.GroupInputs(iRow).BiomassCV(iCol) = Me.m_blockCodes.BlocktoValue(Me.m_blockCodes.SelectedBlock)

    End Sub

    Public Sub SetSeqColorCodes(ByVal startYear As Integer, ByVal endYear As Integer, ByVal yearPerBlock As Integer) Implements IPolicyColorBlockDataSource.SetSeqColorCodes

        'Sequence years not implemented for MSE groups

    End Sub

    Public ReadOnly Property nRows() As Integer Implements IPolicyColorBlockDataSource.nRows
        Get
            Return Me.m_uic.Core.nGroups
        End Get
    End Property

    Public ReadOnly Property RowLabel(ByVal iRow As Integer) As String Implements IPolicyColorBlockDataSource.RowLabel
        Get
            Try
                Return Me.m_uic.Core.MSEManager.GroupInputs(iRow).Name
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".RowLabel() Exception: " & ex.Message)
            End Try
            Return String.Empty
        End Get
    End Property

    Public Property BatchEdit() As Boolean Implements Ecosim.IPolicyColorBlockDataSource.BatchEdit
        Get
            Return Me.m_batchEdit
        End Get

        Set(ByVal value As Boolean)

            Me.m_batchEdit = value

            Dim mse As cMSEManager = Me.m_uic.Core.MSEManager
            For igrp As Integer = 1 To Me.nRows
                mse.GroupInputs(igrp).BatchEdit = Me.m_batchEdit
            Next igrp

        End Set

    End Property

    Public Sub Update() Implements Ecosim.IPolicyColorBlockDataSource.Update

        Try
            For igrp As Integer = 1 To Me.m_uic.Core.nGroups
                Me.m_uic.Core.MSEManager.GroupInputs(igrp).BatchEdit = True
                For iyr As Integer = 1 To Me.TotalBlocks
                    Me.m_uic.Core.MSEManager.GroupInputs(igrp).BiomassCV(iyr) = Me.m_blockCodes.BlocktoValue(m_BlockCells(igrp, iyr))
                Next
                Me.m_uic.Core.MSEManager.GroupInputs(igrp).BatchEdit = False
            Next igrp
        Catch ex As Exception
            System.Console.WriteLine(ex.Message)
        End Try
    End Sub

    Public ReadOnly Property isControlPanelVisible() As Boolean Implements Ecosim.IPolicyColorBlockDataSource.isControlPanelVisible
        Get
            Return False
        End Get
    End Property
End Class

#End Region