
Option Strict On

Imports EwEUtils.Core

Namespace MSEBatchManager

    Public Class cMSEBatchDataStructures

        Public RunType As eMSEBatchRunTypes
        ''' <summary>
        ''' Number of PP forcing functions iterations
        ''' </summary>
        Public nForcing As Integer

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
        Public ControlType(,) As EwEUtils.Core.eQuotaTypes

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

        Public nGroups As Integer
        Public nFleets As Integer


        Public m_orgBlim() As Single
        Public m_orgBbase() As Single
        Public m_orgFmax() As Single
        Public m_orgFmin() As Single

        Public m_orgFixedF() As Single
        Public m_orgTAC() As Single

        Public CommandFilename As String

        Public VersionNumber As Single

        Public Sub redimForcing(ByVal nForcingFunctions As Integer)
            nForcing = nForcingFunctions
            If nForcing = 0 Then nForcing = 1

            ReDim ForcingGroup(nForcing)
            ReDim ForcingIndexes(nForcing)
            ReDim ForcingNames(nForcing)

        End Sub


        Public ReadOnly Property nOuputTypes() As Integer
            Get
                Return System.Enum.GetValues(GetType(eMSEBatchOuputTypes)).Length
            End Get
        End Property

        Public Sub redimTFM(ByVal nTFM As Integer, ByVal nGroups As Integer)
            Me.nTFM = nTFM
            If nTFM = 0 Then nTFM = 1
            Me.nGroups = nGroups

            ReDim tfmBlim(Me.nTFM, nGroups)
            ReDim tfmBbase(Me.nTFM, nGroups)
            ReDim tfmFmax(Me.nTFM, nGroups)
            ReDim tfmFmin(Me.nTFM, nGroups)

        End Sub

        Public Sub redimFixedF(ByVal nFIters As Integer, ByVal nGroups As Integer)
            Me.nFixedF = nFIters
            If nFixedF = 0 Then nFixedF = 1
            Me.nGroups = nGroups

            ReDim FixedF(nFixedF, nGroups)

        End Sub

        Public Sub redimTAC(ByVal nTACIters As Integer, ByVal nGroups As Integer)
            Me.nTAC = nTACIters
            If nTAC = 0 Then nTAC = 1
            Me.nGroups = nGroups

            ReDim TAC(nTAC, nGroups)

        End Sub

        Public Sub redimControlTypes(ByVal nTypes As Integer, ByVal nFleets As Integer)
            nControlTypes = nTypes
            If nControlTypes = 0 Then nControlTypes = 1
            Me.nFleets = nFleets

            ReDim ControlType(nControlTypes, nFleets)

        End Sub

        Public Sub redimOuputTypes()

            ReDim Me.OuputType(Me.nOuputTypes)
            ReDim Me.isOuputSaved(Me.nOuputTypes)

        End Sub

        Public Sub New()

        End Sub

        ''' <summary>
        ''' Store the initial state of the MSE data so it can be restored later
        ''' </summary>
        ''' <param name="MSEdata"></param>
        ''' <remarks></remarks>
        Public Sub StoreMSEState(ByVal MSEdata As EwECore.MSE.cMSEDataStructures)
            ReDim m_orgBlim(Me.nGroups)
            ReDim m_orgBbase(Me.nGroups)
            ReDim m_orgFmax(Me.nGroups)
            ReDim m_orgFmin(Me.nGroups)

            ReDim m_orgFixedF(Me.nGroups)
            ReDim m_orgTAC(Me.nGroups)

            For igrp As Integer = 1 To Me.nGroups
                m_orgBlim(igrp) = MSEdata.Blim(igrp)
                m_orgBbase(igrp) = MSEdata.Bbase(igrp)
                m_orgFmax(igrp) = MSEdata.Fopt(igrp)
                m_orgFmin(igrp) = MSEdata.Fmin(igrp)

                m_orgFixedF(igrp) = MSEdata.FixedF(igrp)
                m_orgTAC(igrp) = MSEdata.TAC(igrp)
            Next

        End Sub

        ''' <summary>
        ''' Restore the MSE data to its original state
        ''' </summary>
        ''' <param name="MSEdata"></param>
        ''' <remarks></remarks>
        Public Sub ReStoreMSEState(ByVal MSEdata As EwECore.MSE.cMSEDataStructures)
            Try

                For igrp As Integer = 1 To Me.nGroups
                    MSEdata.Blim(igrp) = m_orgBlim(igrp)
                    MSEdata.Bbase(igrp) = m_orgBbase(igrp)
                    MSEdata.Fopt(igrp) = m_orgFmax(igrp)
                    MSEdata.Fmin(igrp) = m_orgFmin(igrp)

                    MSEdata.FixedF(igrp) = m_orgFixedF(igrp)
                    MSEdata.TAC(igrp) = m_orgTAC(igrp)
                Next

            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, ex.Message)
            End Try

        End Sub

    End Class


End Namespace