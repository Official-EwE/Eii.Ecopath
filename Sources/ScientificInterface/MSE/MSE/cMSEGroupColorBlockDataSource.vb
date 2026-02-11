' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.MSE
Imports ScientificInterface.Ecosim
Imports SharedResources = ScientificInterfaceShared.My.Resources

#Region "IPolicyColorBlockDataSource implementation for MSE"

Public Class cMSEGroupColorBlockDataSource
    Implements IPolicyColorBlockDataSource

    Private m_uic As cUIContext
    Private m_BlockCells(,) As Integer
    Private m_BlockSelector As ucCVBlockSelector
    Private m_iTotalBlocks As Integer
    Private m_batchEdit As Boolean
    ''' <summary>Is this group exploited (fished) </summary>
    ''' <remarks>True if there is either catch or discards for this group, False otherwise.</remarks>
    Private m_isExploited() As Boolean

    Private m_fpStartYear As cIntegerProperty

    Public Sub New(UIContext As cUIContext)
        Me.m_uic = UIContext
    End Sub

    ''' <inheritdoc cref="IPolicyColorBlockDataSource.BlockCells"/>
    Public ReadOnly Property BlockCells() As Integer(,) _
        Implements IPolicyColorBlockDataSource.BlockCells
        Get
            Return Me.m_BlockCells
        End Get
    End Property

    ''' <inheritdoc cref="IPolicyColorBlockDataSource.TotalBlocks"/>
    Public ReadOnly Property TotalBlocks() As Integer _
        Implements IPolicyColorBlockDataSource.TotalBlocks
        Get
            Return Me.m_uic.Core.EcosimModelParameters.NumberYears
        End Get
    End Property

    ''' <inheritdoc cref="IPolicyColorBlockDataSource.Attach"/>
    Public Sub Attach(Blocks As IBlockSelector) _
        Implements IPolicyColorBlockDataSource.Attach

        Debug.Assert(TypeOf Blocks Is ucCVBlockSelector, Me.ToString & ".Atatch() Blocks must be a ucCVBlockSelector!")
        Try

            Me.m_BlockSelector = DirectCast(Blocks, ucCVBlockSelector)

            'populate the blocks with values from the data!!!!
            Dim cvs As New List(Of Single)
            cvs.Add(0) 'if adding values the first value should be zero
            Dim manager As cMSEManager = Me.m_uic.Core.MSEManager
            Dim blks() As Single = Me.m_BlockSelector.BlockValues

            For i As Integer = 1 To manager.NumGroups
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

                For iblk As Integer = 1 To Me.m_BlockSelector.NumBlocks
                    cvs.Insert(iblk, blks(iblk))
                Next ' For iblk As Integer = 1 To Me.m_blockCodes.NumBlocks
                cvs.Sort()
                Me.m_BlockSelector.BlockValues = cvs.ToArray
            End If

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    ''' <inheritdoc cref="IPolicyColorBlockDataSource.Init"/>
    Public Sub Init() _
        Implements IPolicyColorBlockDataSource.Init

        'Populate the m_isExploited(ngroups) array
        Me.PopIsExploited()

        Me.m_iTotalBlocks = Me.m_uic.Core.EcosimModelParameters.NumberYears

        ReDim Me.m_BlockCells(Me.nRows, Me.TotalBlocks)
        Dim mseData As cMSEGroupInput
        Dim sYear As Integer = Me.m_uic.Core.MSEManager.ModelParameters.MSEStartYear

        For igrp As Integer = 1 To Me.m_BlockCells.GetLength(0) - 1
            mseData = Me.m_uic.Core.MSEManager.GroupInputs(igrp)
            For iTime As Integer = 1 To Me.m_BlockCells.GetLength(1) - 1
                If Me.m_isExploited(igrp) And iTime >= sYear Then
                    Me.m_BlockCells(igrp, iTime) = Me.m_BlockSelector.ValuetoBlock(mseData.BiomassCV(iTime))
                Else
                    Me.m_BlockCells(igrp, iTime) = -1
                End If
            Next
        Next

    End Sub

    ''' <inheritdoc cref="IPolicyColorBlockDataSource.FillBlock"/>
    Public Sub FillBlock(iRow As Integer, iCol As Integer) _
        Implements IPolicyColorBlockDataSource.FillBlock

        ' Sanity checks
        'If (iCol <= Me.m_uic.Core.FishingPolicyManager.ObjectiveParameters.BaseYear) Then Return

        If (iRow < 1) Then Return
        If (iRow > Me.m_BlockCells.GetLength(0) - 1) Then Return

        If Not Me.m_isExploited(iRow) Or iCol < Me.m_uic.Core.MSEManager.ModelParameters.MSEStartYear Then
            'Not in bounds 
            'Don't set the value
            Return
        End If

        ' Fill single block
        Me.m_BlockCells(iRow, iCol) = Me.m_BlockSelector.SelectedBlock
        Me.m_uic.Core.MSEManager.GroupInputs(iRow).BiomassCV(iCol) = Me.m_BlockSelector.BlocktoValue(Me.m_BlockSelector.SelectedBlock)

    End Sub

    ''' <inheritdoc cref="IPolicyColorBlockDataSource.SetSeqColorCodes"/>
    Public Sub SetSeqColorCodes(startYear As Integer,
                                endYear As Integer,
                                yearPerBlock As Integer) _
        Implements IPolicyColorBlockDataSource.SetSeqColorCodes

        'Sequence years not implemented for MSE groups

    End Sub

    ''' <inheritdoc cref="IPolicyColorBlockDataSource.nRows"/>
    Public ReadOnly Property nRows() As Integer _
        Implements IPolicyColorBlockDataSource.nRows
        Get
            Return Me.m_uic.Core.nLivingGroups
        End Get
    End Property

    ''' <inheritdoc cref="IPolicyColorBlockDataSource.RowLabel"/>
    Public ReadOnly Property RowLabel(iRow As Integer) As String _
        Implements IPolicyColorBlockDataSource.RowLabel
        Get
            Try
                Return String.Format(SharedResources.GENERIC_LABEL_INDEXED,
                                     iRow,
                                     Me.m_uic.Core.MSEManager.GroupInputs(iRow).Name)
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".RowLabel() Exception: " & ex.Message)
            End Try
            Return String.Empty
        End Get
    End Property

    ''' <inheritdoc cref="IPolicyColorBlockDataSource.BatchEdit"/>
    Public Property BatchEdit() As Boolean _
        Implements Ecosim.IPolicyColorBlockDataSource.BatchEdit
        Get
            Return Me.m_batchEdit
        End Get

        Set(value As Boolean)

            Me.m_batchEdit = value

            Dim mse As cMSEManager = Me.m_uic.Core.MSEManager
            For igrp As Integer = 1 To Me.nRows
                mse.GroupInputs(igrp).BatchEdit = Me.m_batchEdit
            Next igrp

        End Set

    End Property

    ''' <inheritdoc cref="IPolicyColorBlockDataSource.Update"/>
    Public Sub Update() _
        Implements Ecosim.IPolicyColorBlockDataSource.Update

        Dim man As cMSEManager = Me.m_uic.Core.MSEManager
        Try
            For igrp As Integer = 1 To man.NumGroups
                man.GroupInputs(igrp).BatchEdit = True
                For iyr As Integer = 1 To Me.TotalBlocks
                    man.GroupInputs(igrp).BiomassCV(iyr) = Me.m_BlockSelector.BlocktoValue(Me.m_BlockCells(igrp, iyr))
                Next
                man.GroupInputs(igrp).BatchEdit = False
            Next igrp
        Catch ex As Exception
            System.Console.WriteLine(ex.Message)
        End Try
    End Sub

    ''' <inheritdoc cref="IPolicyColorBlockDataSource.isControlPanelVisible"/>
    Public ReadOnly Property isControlPanelVisible() As Boolean _
        Implements Ecosim.IPolicyColorBlockDataSource.isControlPanelVisible
        Get
            Return False
        End Get
    End Property

    Private Sub PopIsExploited()
        Dim core As EwECore.cCore = Me.m_uic.Core
        Dim nGrps As Integer = core.nGroups
        Dim nFlts As Integer = core.nFleets
        Dim epFlt As EwECore.cEcopathFleetInput

        ReDim Me.m_isExploited(nGrps)

        For igrp As Integer = 1 To nGrps
            Me.m_isExploited(igrp) = False
            For iflt As Integer = 1 To nFlts

                epFlt = core.EcopathFleetInputs(iflt)
                If epFlt.Landings(igrp) > 0 Or epFlt.Discards(igrp) > 0 Then
                    Me.m_isExploited(igrp) = True
                    Exit For
                End If

            Next iflt
        Next igrp

    End Sub

    Public Function BlockToValue(iBlock As Integer) As Single Implements Ecosim.IPolicyColorBlockDataSource.BlockToValue
        Try
            Return Me.m_BlockSelector.BlocktoValue(iBlock)
        Catch ex As Exception
            Return cCore.NULL_VALUE
        End Try
    End Function

End Class

#End Region
