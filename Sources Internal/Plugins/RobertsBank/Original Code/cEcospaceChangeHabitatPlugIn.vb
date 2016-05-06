' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2013 UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada.
' ===============================================================================
'
Option Strict On
Imports EwEPlugin
Imports EwECore

''' ---------------------------------------------------------------------------
''' <summary>
''' Plug-in that will change cells specified in code from water to land.
''' The change will happen at the halfway point during an Ecospace run.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceChangeHabitatPlugIn
    Implements IEcospaceBeginTimestepPostPlugin
    Implements IEcospaceInitRunCompletedPlugin
    Implements IEcospaceRunCompletedPlugin
    Implements EwEPlugin.ICorePlugin


    ''' <summary>Reference to the core</summary>
    Private m_core As cCore = Nothing
    ''' <summary>Original depth layer</summary>
    Private m_DepthOrig(,) As Integer
    ''' <summary>Changed depth layer</summary>
    Private m_DepthNew(,) As Integer
    Private m_PP_Orig(,) As Single
    Private m_Bcell(,) As Single

    Private habmap As cEcospaceBasemap
    Private noRows As Integer
    Private noCols As Integer
    Private SpaceData As cEcospaceDataStructures

    Private m_EwEIsChanged As Boolean = False
    Private m_Ecospace As cEcoSpace
    Private UsePlugin As Boolean
    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Capture a reference to the EwE core when the plug-in initializes. We need
    ''' the core later to find our MPA.
    ''' </summary>
    ''' <param name="core">The EwE core.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize
        Try
            Me.m_core = DirectCast(core, cCore)
        Catch ex As Exception
            Me.m_core = Nothing
        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace is prepared to run, and is about to start executing its time steps.
    ''' In this plug-in point we want to preserve the original open/closed state
    ''' of 'our' MPA so we can restore this state after the Ecospace run.
    ''' </summary>
    ''' <param name="EcospaceDatastructures">- ignored -</param>
    ''' -----------------------------------------------------------------------
    Public Sub EcospaceInitRunCompleted(ByVal EcospaceDatastructures As Object) _
        Implements EwEPlugin.IEcospaceInitRunCompletedPlugin.EcospaceInitRunCompleted

        ' Santiy checks
        If (Me.m_core Is Nothing) Then Return

        Dim response As String = InputBox("Type 'Yes' to run RBT2 habitat modification", "Roberts Bank T2 plugin")


        If response = "Yes" Then
            UsePlugin = True
        Else
            UsePlugin = False
            Return
        End If

        habmap = Me.m_core.EcospaceBasemap
        noRows = habmap.InRow
        noCols = habmap.InCol

        ReDim m_DepthOrig(noRows, noCols)
        ReDim m_DepthNew(noRows, noCols)
        ReDim m_PP_Orig(noRows, noCols)
        ReDim m_Bcell(noRows, noCols)

        SpaceData = DirectCast(EcospaceDatastructures, cEcospaceDataStructures)

        For iRow As Integer = 1 To noRows
            For iCol As Integer = 1 To noCols
                m_DepthOrig(iRow, iCol) = SpaceData.Depth(iRow, iCol)
                'First set new depth to original depth, then store the change in memory afterwards
                m_DepthNew(iRow, iCol) = SpaceData.Depth(iRow, iCol)
                'Store PP 
                m_PP_Orig(iRow, iCol) = SpaceData.RelPP(iRow, iCol)
            Next
        Next

        'Make the Roberts Bank Terminal 2 area land:
        For iRow As Integer = 33 To 37
            For iCol As Integer = 28 To 28
                m_DepthNew(iRow, iCol) = 0
            Next
        Next
        For iRow As Integer = 38 To 44
            For iCol As Integer = 15 To 28
                m_DepthNew(iRow, iCol) = 0
            Next
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace is about to compute a time step. Change water to land when halfway
    ''' </summary>
    ''' <param name="EcospaceDatastructures">- ignored -</param>
    ''' <param name="iTime">The time step that is currently being executed.</param>
    ''' -----------------------------------------------------------------------
    Public Sub EcospaceBeginTimeStepPost(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer) _
        Implements EwEPlugin.IEcospaceBeginTimestepPostPlugin.EcospaceBeginTimeStepPost

        ' Sanity checks
        If (Me.m_core Is Nothing) Then Return
        If UsePlugin = False Then Return

        Dim IsHalfWay As Boolean = iTime = Me.m_core.nEcospaceTimeSteps / 2

        If IsHalfWay Then
            'Dim SpaceData As cEcospaceDataStructures = DirectCast(EcospaceDatastructures, cEcospaceDataStructures)
            Dim AbsoluteDateForTimeStep As Date = Me.m_core.EcospaceTimestepToAbsoluteTime(iTime)

            Dim iSumWaterNew As Integer = 0
            Dim iSumWaterOrig As Integer = 0
            For iRow As Integer = 1 To noRows
                For iCol As Integer = 1 To noCols
                    If SpaceData.Depth(iRow, iCol) > 0 Then iSumWaterOrig += 1
                    SpaceData.Depth(iRow, iCol) = m_DepthNew(iRow, iCol)
                    If SpaceData.Depth(iRow, iCol) > 0 Then iSumWaterNew += 1

                    If m_DepthNew(iRow, iCol) = 0 Then 'new land
                        SpaceData.RelPP(iRow, iCol) = 0
                        For igrp As Integer = 1 To SpaceData.NGroups
                            SpaceData.Bcell(iRow, iCol, igrp) = 0
                        Next
                    End If
                Next
            Next

            Console.Write("Number of water cells, before = " & iSumWaterOrig.ToString)
            Console.WriteLine()
            Console.Write("Number of water cells, after  = " & iSumWaterNew.ToString)
            Console.WriteLine()

            InitSpatialChanges()


            ' Extra feature: notify the world of the change
            'If (iTime = CInt(Me.m_core.nEcospaceTimeSteps / 2)) Then
            ' This message will appear in the EwE6 status panel
            'Dim msg As New cMessage(Me.Name & ": Habitat change activated at time step " & iTime & ", " & AbsoluteDateForTimeStep.ToShortDateString, _
            '                        eMessageType.Any, EwEUtils.Core.eCoreComponentType.External, eMessageImportance.Information)
            'Me.m_core.Messages.SendMessage(msg)
            'End If

        End If



    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace has finished running. Restore the original layout of the MPA.
    ''' </summary>
    ''' <param name="EcoSpaceDatastructures">- ignored -</param>
    ''' -----------------------------------------------------------------------
    Public Sub EcospaceRunCompleted(ByVal EcoSpaceDatastructures As Object) _
        Implements EwEPlugin.IEcospaceRunCompletedPlugin.EcospaceRunCompleted

        ' Sanity checks
        If (Me.m_core Is Nothing) Then Return
        If UsePlugin = False Then Return

        'Restore original depth map and primary production
        For iRow As Integer = 1 To noRows
            For iCol As Integer = 1 To noCols
                SpaceData.Depth(iRow, iCol) = m_DepthOrig(iRow, iCol)
                SpaceData.RelPP(iRow, iCol) = m_PP_Orig(iRow, iCol)
            Next
        Next

        'reset the habitat changes
        InitSpatialChanges()

        ' Discard any changes that were caused by changes
        If Not Me.m_EwEIsChanged Then
            Me.m_core.DiscardChanges()
        End If

    End Sub

    Private Sub InitSpatialChanges()
        'tell core to make chagnes 
        Me.m_EwEIsChanged = Me.m_core.HasChanges
        WaterCells()
        Me.m_Ecospace.ScaleRelativePrimaryProductivityToEcopathLevel()

        SpaceData.bHasCapacityChanged = True
        Me.m_Ecospace.SetHabCap()
        Me.m_Ecospace.CalcHabitatArea()
        Me.m_Ecospace.SetMovementParameters()
        'Me.m_Ecospace.VaryMovementParameters()

    End Sub
    Private Sub WaterCells()
        'this finds the start and end rows and columns so that solvegrid doesn't go through every one
        Dim foundRow As Boolean
        Dim waterCtr As Integer = 0
        For j = 1 To SpaceData.InCol
            foundRow = False
            SpaceData.iStartRow(j) = SpaceData.InRow + 1
            SpaceData.iEndRow(j) = 0
            For i = 1 To SpaceData.InRow
                If SpaceData.Depth(i, j) > 0 Then
                    waterCtr = waterCtr + 1
                    SpaceData.iWaterCellIndex(waterCtr) = i
                    SpaceData.jWaterCellIndex(waterCtr) = j
                    If SpaceData.iStartRow(j) = SpaceData.InRow + 1 Then
                        SpaceData.iStartRow(j) = i
                        foundRow = True
                    End If
                    SpaceData.iEndRow(j) = i
                End If
            Next
            'spacedata.iStartRow(j) = 1
            'spacedata.iEndRow(j) = spacedata.Inrow
        Next
        SpaceData.iTotalWaterCells = waterCtr

        For i = 1 To SpaceData.InRow
            SpaceData.jStartCol(i) = SpaceData.InCol + 1
            SpaceData.jEndCol(i) = 0
            For j = 1 To SpaceData.InCol
                If SpaceData.Depth(i, j) > 0 Then
                    If SpaceData.jStartCol(i) = SpaceData.InCol + 1 Then
                        SpaceData.jStartCol(i) = j
                    End If
                    SpaceData.jEndCol(i) = j
                End If
            Next
        Next
    End Sub

#Region " Generic plug-in bits "

    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Plug-in that opens and closes MPAs"
        End Get
    End Property

    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "MPAOpenStatePlugin"
        End Get
    End Property

#End Region ' Generic plug-in bits

    Public Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized
        Me.m_Ecospace = DirectCast(objEcoSpace, cEcoSpace)
    End Sub
End Class
