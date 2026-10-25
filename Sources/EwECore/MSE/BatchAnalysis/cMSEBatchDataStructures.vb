' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Namespace MSEBatchManager

    Public Class cMSEBatchDataStructures

        Public RunType As eMSEBatchRunTypes
        ''' <summary>
        ''' Number of PP forcing functions iterations
        ''' </summary>
        Public nForcing As Integer

        ''' <summary>
        ''' Is there forcing data in the command file
        ''' </summary>
        ''' <remarks></remarks>
        Public bForcingLoaded As Boolean

        ''' <summary>
        ''' Number of Control type iterations
        ''' </summary>
        ''' <remarks></remarks>
        Public nControlTypes As Integer

        ''' <summary>
        ''' Number of TFM (hockey stick) iteration
        ''' </summary>
        ''' <remarks></remarks>
        Public nTFM As Integer

        ''' <summary>
        ''' Number of Fixed Fishing Mortality iterations
        ''' </summary>
        ''' <remarks></remarks>
        Public nFixedF As Integer

        ''' <summary>
        ''' Number of Total Allowable Catch iterations
        ''' </summary>
        ''' <remarks></remarks>
        Public nTAC As Integer

        ''' <summary>
        ''' Number of iterations for the selected run type 
        ''' </summary>
        ''' <remarks>If RunType = eMSEBatchRunTypes.TFM then nParIters = nTFMs</remarks>
        Public nParIters As Integer

        ''' <summary>
        ''' Names of the loaded forcing functions
        ''' </summary>
        Public ForcingNames() As String

        ''' <summary>
        ''' Index to forcing function to use
        ''' </summary>
        Public ForcingIndexes() As Integer
        ''' <summary>
        ''' Index to group PP forcing function is applied to
        ''' </summary>
        ''' <remarks></remarks>
        Public ForcingGroup() As Integer

        ''' <summary>
        ''' number of Control type
        ''' </summary>
        ''' <remarks>dimensioned NControlTypes, nFleets</remarks>
        Public ControlType(,) As eQuotaTypes

        Public OuputType() As eMSEBatchOuputTypes
        Public isOuputSaved() As Boolean

        ''' <summary>
        ''' MSE Blim
        ''' </summary>
        ''' <remarks>tfmBlim(nTFM,nGroups) </remarks>
        Public tfmBlim(,) As Single
        Public tfmBbase(,) As Single
        Public tfmFmax(,) As Single
        Public tfmFmin(,) As Single

        Public FixedF(,) As Single
        Public TAC(,) As Single
        Public STDevForcing As Single
        Public isInit As Boolean

        Public iCurRun As Integer

        Public OuputDir As String

        Public StopRun As Boolean

        Public m_nGroups As Integer
        Public m_nFleets As Integer

        Public m_orgBlim() As Single
        Public m_orgBbase() As Single
        Public m_orgFmax() As Single
        Public m_orgFmin() As Single

        Public m_orgFixedF() As Single
        Public m_orgTAC() As Single

        Public CommandFilename As String

        Public VersionNumber As Single

        ''' <summary>
        ''' Database ID of the currently loaded Batch Scenario
        ''' </summary>
        ''' <remarks></remarks>
        Public ScenarioDBID As Integer

        Public TFMDBIDs() As Integer

        Public BlimLower() As Single
        Public BlimUpper() As Single

        Public BBaseLower() As Single
        Public BBaseUpper() As Single

        Public FOptLower() As Single
        Public FOptUpper() As Single

        Public FixedFLower() As Single
        Public FixedFUpper() As Single

        Public TACLower() As Single
        Public TACUpper() As Single

        Public IterCalcType As eMSEBatchIterCalcTypes = eMSEBatchIterCalcTypes.Percent

        Public GroupRunType() As eMSEBatchRunTypes
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cMSEBatchDataStructures)()

        Public ReadOnly Property nGroups As Integer
            Get
                Return Me.m_nGroups
            End Get
        End Property

        Public ReadOnly Property nFleets As Integer
            Get
                Return Me.m_nFleets
            End Get
        End Property

        Public Sub redimForcing(nForcingFunctions As Integer)
            Me.nForcing = nForcingFunctions
            If Me.nForcing = 0 Then Me.nForcing = 1

            ReDim Me.ForcingGroup(Me.nForcing)
            ReDim Me.ForcingIndexes(Me.nForcing)
            ReDim Me.ForcingNames(Me.nForcing)

        End Sub

        Public ReadOnly Property nOuputTypes() As Integer
            Get
                Return System.Enum.GetValues(GetType(eMSEBatchOuputTypes)).Length
            End Get
        End Property

        Public Sub redimTFM(nTFM As Integer, nGroups As Integer)
            Me.nTFM = nTFM
            If nTFM = 0 Then nTFM = 1

            ReDim Me.tfmBlim(Me.nTFM, nGroups)
            ReDim Me.tfmBbase(Me.nTFM, nGroups)
            ReDim Me.tfmFmax(Me.nTFM, nGroups)
            ReDim Me.tfmFmin(Me.nTFM, nGroups)

            ReDim Me.TFMDBIDs(nGroups)

            ' Temporary fix: set group dbids to bogus values; needs to be configured from Datasource
            For i As Integer = 1 To nGroups
                Me.TFMDBIDs(i) = i
            Next

            ReDim Me.BlimLower(nGroups)
            ReDim Me.BlimUpper(nGroups)

            ReDim Me.BBaseLower(nGroups)
            ReDim Me.BBaseUpper(nGroups)

            ReDim Me.FOptLower(nGroups)
            ReDim Me.FOptUpper(nGroups)

            '    Me.setDefaultTFM()

        End Sub

        Public Sub redimFixedF(nFIters As Integer, nGroups As Integer)
            Me.nFixedF = nFIters
            If Me.nFixedF = 0 Then Me.nFixedF = 1

            ReDim Me.FixedF(Me.nFixedF, nGroups)
            ReDim Me.FixedFLower(nGroups)
            ReDim Me.FixedFUpper(nGroups)

        End Sub

        Public Sub redimTAC(nTACIters As Integer, nGroups As Integer)
            Me.nTAC = nTACIters
            If Me.nTAC = 0 Then Me.nTAC = 1

            ReDim Me.TAC(Me.nTAC, nGroups)
            ReDim Me.TACLower(nGroups)
            ReDim Me.TACUpper(nGroups)

        End Sub

        Public Sub redimControlTypes(nTypes As Integer, nFleets As Integer)
            Me.nControlTypes = nTypes
            If Me.nControlTypes = 0 Then Me.nControlTypes = 1

            ReDim Me.ControlType(Me.nControlTypes, nFleets)

        End Sub

        Public Sub redimOuputTypes()

            ReDim Me.OuputType(Me.nOuputTypes)
            ReDim Me.isOuputSaved(Me.nOuputTypes)

        End Sub

        Public Sub setDefaultLimits()
            Dim defautlLL As Single = 0.5
            Dim defautlUp As Single = 1.0
            For igrp As Integer = 1 To Me.nGroups

                Me.GroupRunType(igrp) = eMSEBatchRunTypes.TFM

                Me.BlimLower(igrp) = defautlLL
                Me.BlimUpper(igrp) = defautlUp

                Me.BBaseLower(igrp) = defautlLL
                Me.BBaseUpper(igrp) = defautlUp

                Me.FOptLower(igrp) = defautlLL
                Me.FOptUpper(igrp) = defautlUp

                Me.FixedFLower(igrp) = defautlLL
                Me.FixedFUpper(igrp) = defautlUp

                Me.TACLower(igrp) = defautlLL
                Me.TACUpper(igrp) = defautlUp

            Next
        End Sub

        'Public Sub New()
        '    Me.bForcingLoaded = False
        'End Sub

        Public Sub New(MSEdata As MSE.cMSEDataStructures)
            Try
                Me.redimToMSE(MSEdata)
            Catch ex As Exception

            End Try
        End Sub

        Private Sub redimToMSE(MSEdata As MSE.cMSEDataStructures)

            Me.m_nGroups = MSEdata.nGroups
            Me.m_nFleets = MSEdata.nFleets

            ReDim Me.GroupRunType(MSEdata.nGroups)

            Me.redimForcing(1)
            Me.redimControlTypes(1, MSEdata.nFleets)

            Me.redimOuputTypes()
            Me.redimTAC(1, MSEdata.nGroups)
            Me.redimTFM(1, MSEdata.nGroups)
            Me.redimFixedF(1, MSEdata.nGroups)

        End Sub

        ''' <summary>
        ''' Store the initial state of the MSE data so it can be restored later
        ''' </summary>
        ''' <param name="MSEdata"></param>
        ''' <remarks></remarks>
        Public Sub StoreMSEState(MSEdata As EwECore.MSE.cMSEDataStructures)
            ReDim Me.m_orgBlim(Me.nGroups)
            ReDim Me.m_orgBbase(Me.nGroups)
            ReDim Me.m_orgFmax(Me.nGroups)
            ReDim Me.m_orgFmin(Me.nGroups)

            ReDim Me.m_orgFixedF(Me.nGroups)
            ReDim Me.m_orgTAC(Me.nGroups)

            For igrp As Integer = 1 To Me.nGroups
                Me.m_orgBlim(igrp) = MSEdata.Blim(igrp)
                Me.m_orgBbase(igrp) = MSEdata.Bbase(igrp)
                Me.m_orgFmax(igrp) = MSEdata.Fopt(igrp)
                Me.m_orgFmin(igrp) = MSEdata.Fmin(igrp)

                Me.m_orgFixedF(igrp) = MSEdata.FixedF(igrp)
                Me.m_orgTAC(igrp) = MSEdata.TAC(igrp)
            Next

        End Sub

        ''' <summary>
        ''' Restore the MSE data to its original state
        ''' </summary>
        ''' <param name="MSEdata"></param>
        ''' <remarks></remarks>
        Public Sub ReStoreMSEState(MSEdata As EwECore.MSE.cMSEDataStructures)
            Try

                For igrp As Integer = 1 To Me.nGroups
                    MSEdata.Blim(igrp) = Me.m_orgBlim(igrp)
                    MSEdata.Bbase(igrp) = Me.m_orgBbase(igrp)
                    MSEdata.Fopt(igrp) = Me.m_orgFmax(igrp)
                    MSEdata.Fmin(igrp) = Me.m_orgFmin(igrp)

                    MSEdata.FixedF(igrp) = Me.m_orgFixedF(igrp)
                    MSEdata.TAC(igrp) = Me.m_orgTAC(igrp)
                Next

            Catch ex As Exception
                m_logger.LogError(ex, "cMSEBatchDataStructures::ReStoreMSEState() Error restoring MSE state")
                Debug.Assert(False, ex.Message)
            End Try

        End Sub

    End Class

End Namespace