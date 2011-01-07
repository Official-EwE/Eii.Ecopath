Imports System.IO

Imports EwECore.MSE
Imports EwEUtils.Core
Imports EwECore.MSEBatchManager

Namespace MSECommandFile


    Public Class cMSECommandFileReader

        Public Const F_DATA_TAG As String = "Constant_F"
        Public Const Y_DATA_TAG As String = "Constant_Y"
        Public Const TFM_DATA_TAG As String = "TFM"
        Public Const NSIMS_DATA_TAG As String = "Number_Sims"
        Public Const CV_DATA_TAG As String = "Error_CV"
        Public Const RUNTYPE_DATA_TAG As String = "Run_Type"
        Public Const CONTROLTYPE_DATA_TAG As String = "Control_Type"
        Public Const PP_DATA_TAG As String = "PP"
        Public Const PPDEV_DATA_TAG As String = "PP_STDEV"
        Public Const ENDYEAR_DATA_TAG As String = "End_Year"
        Public Const VERSION_DATA_TAG As String = "Control_File_Version"

        Public Const OUTPUT_DATA_TAG As String = "Ouput_Directory"

        Public Const F_INDEX_TAG As String = "Constant_F_INDEX"
        Public Const Y_INDEX_TAG As String = "Constant_Y_INDEX"
        Public Const TFM_INDEX_TAG As String = "TFM_INDEX"
        Public Const CONTROLTYPE_INDEX_TAG As String = "Control_Type_INDEX"

        Public Const MODEL_NAME_TAG As String = "Model_Name"

        Public Const SAVE_OUTPUT_TAG As String = "_OUTPUT"
        Public Const BIOMASS_OUTPUT_TAG As String = "Biomass_OUTPUT"
        Public Const QB_OUTPUT_TAG As String = "Consumption/Biomass_OUTPUT"
        Public Const FEEDING_OUTPUT_TAG As String = "FeedingTime_OUTPUT"
        Public Const MORT_OUTPUT_TAG As String = "FishingMortRate_OUTPUT"
        Public Const PRED_OUTPUT_TAG As String = "PredationRates_OUTPUT"
        Public Const CATCH_OUTPUT_TAG As String = "CatchByGroup_OUTPUT"

        Private m_core As cCore

        Private m_dicControls As Dictionary(Of String, List(Of IMSEParameter)) = New Dictionary(Of String, List(Of IMSEParameter))
        Private m_MSEdata As cMSEDataStructures

        Private m_ConTypeLookup As Dictionary(Of Integer, EwEUtils.Core.eQuotaTypes)
        Private m_RunTypeLookup As Dictionary(Of Integer, eMSEBatchRunTypes)
        Private m_OuputTagToEnumLookup As Dictionary(Of String, eMSEBatchOuputTypes)
        'Private m_RunTypeToTagLookup As Dictionary(Of eMSEBatchRunTypes, String)

        Private m_BatchData As cMSEBatchDataStructures
        Private m_Manager As cMSEBatchManager

        Private m_lstValidationMessages As List(Of String)

        Private m_curForceIter As Integer

        Public Sub New(ByVal Core As cCore, ByVal BatchManager As cMSEBatchManager)
            Me.m_core = Core
            Me.m_MSEdata = Me.Core.m_MSEData
            Me.m_BatchData = BatchManager.BatchData
            Me.m_Manager = BatchManager

            Me.m_ConTypeLookup = New Dictionary(Of Integer, EwEUtils.Core.eQuotaTypes)
            Me.m_ConTypeLookup.Add(0, EwEUtils.Core.eQuotaTypes.NoControls)
            Me.m_ConTypeLookup.Add(1, EwEUtils.Core.eQuotaTypes.Weakest)
            Me.m_ConTypeLookup.Add(2, EwEUtils.Core.eQuotaTypes.HighestValue)
            Me.m_ConTypeLookup.Add(3, EwEUtils.Core.eQuotaTypes.Selective)

            Me.m_RunTypeLookup = New Dictionary(Of Integer, eMSEBatchRunTypes)
            Me.m_RunTypeLookup.Add(0, eMSEBatchRunTypes.Any)
            Me.m_RunTypeLookup.Add(1, eMSEBatchRunTypes.Constant_F)
            Me.m_RunTypeLookup.Add(2, eMSEBatchRunTypes.Constant_Y)
            Me.m_RunTypeLookup.Add(3, eMSEBatchRunTypes.TFM)

            Me.m_OuputTagToEnumLookup = New Dictionary(Of String, eMSEBatchOuputTypes)
            Me.m_OuputTagToEnumLookup.Add(BIOMASS_OUTPUT_TAG, eMSEBatchOuputTypes.Biomass)
            Me.m_OuputTagToEnumLookup.Add(QB_OUTPUT_TAG, eMSEBatchOuputTypes.QB)
            Me.m_OuputTagToEnumLookup.Add(FEEDING_OUTPUT_TAG, eMSEBatchOuputTypes.FeedingTime)
            Me.m_OuputTagToEnumLookup.Add(MORT_OUTPUT_TAG, eMSEBatchOuputTypes.FishingMortRate)
            Me.m_OuputTagToEnumLookup.Add(PRED_OUTPUT_TAG, eMSEBatchOuputTypes.PredRate)
            Me.m_OuputTagToEnumLookup.Add(CATCH_OUTPUT_TAG, eMSEBatchOuputTypes.CatchByGroup)

        End Sub

        Public Sub Clear()
            Try

                Me.m_ConTypeLookup.Clear()
                Me.m_RunTypeLookup.Clear()
                Me.m_OuputTagToEnumLookup.Clear()

                Me.m_ConTypeLookup = Nothing
                Me.m_RunTypeLookup = Nothing
                Me.m_OuputTagToEnumLookup = Nothing

            Catch ex As Exception

            End Try

        End Sub

        Public Function Read(ByVal filename As String) As Boolean

            Me.m_BatchData.isInit = False

            If Not File.Exists(filename) Then
                Me.Manager.MarshallMessage("File does not exist.")
                Return False
            End If

            Dim bSuccess As Boolean
            Try


                Dim line As String
                Dim ParamReader As IMSEParameter
                Dim stream As StreamReader = New StreamReader(filename)

                Me.m_dicControls.Clear()

                Do
                    line = stream.ReadLine()
                    ParamReader = Me.ParameterReaderFactory(line)
                    If ParamReader IsNot Nothing Then
                        Dim bInit As Boolean = ParamReader.Init(line)
                        If bInit Then
                            Me.addParameter(ParamReader)
                        End If
                        Debug.Assert(bInit, "Failed to read data from command file.")
                    End If

                Loop Until line Is Nothing

                stream.Close()
                bSuccess = True

            Catch ex As Exception
                bSuccess = False
                Me.Manager.MarshallMessage("ERROR reading command file. ")
                Me.Manager.MarshallMessage(vbTab & ex.Message)
            End Try

            Debug.Assert(bSuccess, Me.ToString & " Failed to read command file!")

            Me.m_BatchData.isInit = bSuccess
            Return bSuccess

        End Function


        Public Function ValidateData() As Boolean
            Dim bReturn As Boolean = True

            'First file version
            Dim Version As List(Of IMSEParameter) = Me.getTagData(VERSION_DATA_TAG)
            If Version.Count = 0 Then
                'no version tag in the file it must be invalid
                Me.Manager.MarshallMessage("ERROR:")
                Me.Manager.MarshallMessage(vbTab & "Invalid command file.")
                Return False
            End If
            If Not Version.Item(0).Validate() Then
                'failed validation
                Return False
            End If

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Validate all the data in the IMSEParameter objects
            For Each lstPs As List(Of IMSEParameter) In Me.m_dicControls.Values
                For Each par As IMSEParameter In lstPs
                    Try
                        If Not par.Validate() Then
                            'If the object failed validation then it should have posted a message
                            bReturn = False
                        End If
                    Catch ex As Exception
                        Debug.Assert(False, "WOW Dude Exception validating data! " & ex.Message)
                        bReturn = False
                    End Try

                Next
            Next
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Is there a different number of iteration when RunType = Any
            If Me.BatchData.RunType = eMSEBatchRunTypes.Any Then
                If Me.BatchData.nFixedF <> Me.BatchData.nTAC <> Me.BatchData.nTFM Then
                    Me.Manager.MarshallMessage("WARNING:")
                    Me.Manager.MarshallMessage(vbTab & "Run Type 'Any' there is a different number of iterations for F, TAC and TFM.")
                    Me.Manager.MarshallMessage(vbTab & "Last entry will be used the when number of iteration has been exceeded.")
                End If
            End If
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Is there data for the RunType
            Dim bMissing As Boolean ' = True
            bMissing = False
            bMissing = bMissing Or (Me.BatchData.RunType = eMSEBatchRunTypes.Constant_F And Me.getTagData(F_DATA_TAG).Count = 0)
            bMissing = bMissing Or (Me.BatchData.RunType = eMSEBatchRunTypes.Constant_Y And Me.getTagData(Y_DATA_TAG).Count = 0)
            bMissing = bMissing Or (Me.BatchData.RunType = eMSEBatchRunTypes.TFM And Me.getTagData(TFM_DATA_TAG).Count = 0)

            If bMissing Then
                Me.Manager.MarshallMessage("WARNING:")
                Me.Manager.MarshallMessage(vbTab & "No enteries in command file for this Run Type.")
            End If
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            Return bReturn

        End Function

        Private Sub addParameter(ByVal parameter As IMSEParameter)

            'Does the dictionary of control parameters contain this Tag
            If Not m_dicControls.ContainsKey(parameter.Tag) Then
                'No then add it
                m_dicControls.Add(parameter.Tag, New List(Of IMSEParameter))
            End If
            'set the index of this parameter object 
            Dim lst As List(Of IMSEParameter) = m_dicControls.Item(parameter.Tag)
            parameter.Index = lst.Count + 1
            m_dicControls.Item(parameter.Tag).Add(parameter)

        End Sub

        Private Function ParameterReaderFactory(ByVal controlString As String) As IMSEParameter

            If controlString Is Nothing Then
                Return Nothing
            End If

            If cFParameter.CanRead(controlString) Then
                Return New cFParameter(Me)
            End If

            If cTACParameter.CanRead(controlString) Then
                Return New cTACParameter(Me)
            End If

            If cTFMParameter.CanRead(controlString) Then
                Return New cTFMParameter(Me)
            End If

            If cNumSimsParameter.CanRead(controlString) Then
                Return New cNumSimsParameter(Me)
            End If

            If cErrorCVParameter.CanRead(controlString) Then
                Return New cErrorCVParameter(Me)
            End If

            If cRunTypeParameter.CanRead(controlString) Then
                Return New cRunTypeParameter(Me)
            End If

            If cControlTypeParameter.CanRead(controlString) Then
                Return New cControlTypeParameter(Me)
            End If

            If cPPParameter.CanRead(controlString) Then
                Return New cPPParameter(Me)
            End If

            If cPPDevParameter.CanRead(controlString) Then
                Return New cPPDevParameter(Me)
            End If

            If cOutputDirParameter.CanRead(controlString) Then
                Return New cOutputDirParameter(Me)
            End If


            If cOuputParameter.CanRead(controlString) Then
                Return New cOuputParameter(Me)
            End If


            If cEndYearParameter.CanRead(controlString) Then
                Return New cEndYearParameter(Me)
            End If

            If cVersionNumberParameter.CanRead(controlString) Then
                Return New cVersionNumberParameter(Me)
            End If

            If cModelNameParameter.CanRead(controlString) Then
                Return New cModelNameParameter(Me)
            End If

            Dim values() As String = controlString.Split(",")
            Dim tag As String = values(0)

            If isIndexTag(tag) Then
                Return New cIndexParameter(tag)
            End If

            Return Nothing

        End Function

        Public Function updateDataStructures() As Boolean
            Dim bSuccess As Boolean = True
            Dim errMsg As String
            Dim lst As List(Of IMSEParameter)
            Try

                'Run Type
                Me.getTagData(RUNTYPE_DATA_TAG).Item(0).Update()

                'update the number of simulation
                Me.getTagData(NSIMS_DATA_TAG).Item(0).Update()

                'update Error CV
                Me.getTagData(CV_DATA_TAG).Item(0).Update()

                'Primary Production variation
                Me.getTagData(PPDEV_DATA_TAG).Item(0).Update()

                'Output Directory
                Me.getTagData(OUTPUT_DATA_TAG).Item(0).Update()

                'End Year
                Me.getTagData(ENDYEAR_DATA_TAG).Item(0).Update()

                'Output files
                lst = Me.getTagData(SAVE_OUTPUT_TAG)
                Me.m_BatchData.redimOuputTypes()
                For Each par As IMSEParameter In lst
                    par.Update()
                Next

                'Primary production
                lst = Me.getTagData(PP_DATA_TAG)
                Me.m_BatchData.redimForcing(lst.Count)
                For Each par As IMSEParameter In lst
                    par.Update()
                Next

                'Control type
                lst = Me.getTagData(CONTROLTYPE_DATA_TAG)
                Me.m_BatchData.redimControlTypes(lst.Count, Me.m_MSEdata.nFleets)
                For Each par As IMSEParameter In lst
                    par.Update()
                Next

                'Target Fishing Mortality
                lst = Me.getTagData(TFM_DATA_TAG)
                Me.m_BatchData.redimTFM(lst.Count, Me.m_MSEdata.NGroups)
                For Each par As IMSEParameter In lst
                    par.Update()
                Next

                'Fixed Fishing Mort
                lst = Me.getTagData(F_DATA_TAG)
                Me.m_BatchData.redimFixedF(lst.Count, Me.m_MSEdata.NGroups)
                For Each par As IMSEParameter In lst
                    par.Update()
                Next

                'Total Allowable Catch
                lst = Me.getTagData(Y_DATA_TAG)
                Me.m_BatchData.redimTAC(lst.Count, Me.m_MSEdata.NGroups)
                For Each par As IMSEParameter In lst
                    par.Update()
                Next

                'Number of parameters iterations based on run type
                Select Case Me.BatchData.RunType

                    Case eMSEBatchRunTypes.Constant_F
                        Me.BatchData.nParIters = Me.BatchData.nFixedF
                    Case eMSEBatchRunTypes.Constant_Y
                        Me.BatchData.nParIters = Me.BatchData.nTAC
                    Case eMSEBatchRunTypes.TFM
                        Me.BatchData.nParIters = Me.BatchData.nTFM
                    Case eMSEBatchRunTypes.Any
                        'Max of all the types
                        Me.BatchData.nParIters = Math.Max(Me.BatchData.nTFM, Me.BatchData.nFixedF)
                        Me.BatchData.nParIters = Math.Max(Me.BatchData.nTAC, Me.BatchData.nParIters)

                End Select

            Catch ex As Exception
                bSuccess = False
                errMsg = ex.Message
            End Try

            Debug.Assert(bSuccess, Me.ToString & ".updateDataStructures() Error: " & errMsg)

            If Not bSuccess Then
                Me.Manager.MarshallMessage(errMsg)
            End If

            Return bSuccess

        End Function

        ''' <summary>
        ''' Is this Tag an Index Tag
        ''' </summary>
        ''' <param name="tag">Tag to check</param>
        ''' <returns>True if this Tag is for an Index parameter</returns>
        ''' <remarks></remarks>
        Private Function isIndexTag(ByVal tag As String) As Boolean
            If isTag(tag, TFM_INDEX_TAG) Then
                Return True
            End If
            If isTag(tag, F_INDEX_TAG) Then
                Return True
            End If
            If isTag(tag, Y_INDEX_TAG) Then
                Return True
            End If
            If isTag(tag, CONTROLTYPE_INDEX_TAG) Then
                Return True
            End If
        End Function


        ''' <summary>
        ''' Is this string a Tag from the control file
        ''' </summary>
        ''' <param name="InputTag">String to check</param>
        ''' <param name="TagConstant"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function isTag(ByVal InputTag As String, ByVal TagConstant As String) As Boolean
            If String.Compare(InputTag, TagConstant) = 0 Then
                Return True
            End If
            Return False
        End Function


        Public Shared Function CanRead(ByVal Tag As String, ByVal ControlString As String) As Boolean
            Dim values() As String

            Try

                If ControlString Is Nothing Then
                    Return False
                End If

                values = ControlString.Split(",")

                'Does the first value in the ControlString match the tag 
                If (String.Compare(values(0), Tag) = 0) Then
                    'Yes...
                    Return True
                End If
                Return False

            Catch ex As Exception
                System.Console.WriteLine("MSEBatchManager Exception: " & ex.Message)
                Return False
            End Try

            Debug.Assert(False, "MSEBatchManager.CanRead() Failed.")

        End Function

        Public ReadOnly Property BatchData() As cMSEBatchDataStructures
            Get
                Return Me.m_BatchData
            End Get
        End Property

        Public ReadOnly Property Manager() As cMSEBatchManager
            Get
                Return Me.m_Manager
            End Get
        End Property

        ''' <summary>
        ''' Returns a list of parameters read from the control file for this Tag 
        ''' </summary>
        ''' <param name="tag"></param>
        ''' <returns>Returns List(Of IMSEParameter) </returns>
        ''' <remarks></remarks>
        Friend Function getTagData(ByVal tag As String) As List(Of IMSEParameter)

            Dim list As List(Of IMSEParameter)
            If Me.m_dicControls.ContainsKey(tag) Then
                list = Me.m_dicControls.Item(tag)
            End If

            If list Is Nothing Then
                list = New List(Of IMSEParameter)
                System.Console.WriteLine(Me.ToString & ".getTagData(" & tag & ") No data for tag!")
            End If

            Return list

        End Function

    
        Friend ReadOnly Property Core() As cCore
            Get
                Return Me.m_core
            End Get
        End Property


        Friend ReadOnly Property MSEData() As cMSEDataStructures
            Get
                Return Me.m_MSEdata
            End Get
        End Property


        Friend ReadOnly Property nGroups() As Integer
            Get
                Return Me.m_MSEdata.NGroups
            End Get
        End Property


        Friend ReadOnly Property nFleets() As Integer
            Get
                Return Me.m_MSEdata.nFleets
            End Get
        End Property

        ''' <summary>
        ''' Convert the Control Index from the command file into an EwEUtils.Core.eQuotaTypes
        ''' </summary>
        ''' <param name="iControlIndex"></param>
        ''' <returns>A valid EwEUtils.Core.eQuotaTypes</returns>
        ''' <remarks></remarks>
        Friend Function ControlToQuotaType(ByVal iControlIndex As Integer) As EwEUtils.Core.eQuotaTypes
            Try
                Return Me.m_ConTypeLookup.Item(iControlIndex)
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".getQuotaType() Exception: " & ex.Message)
            End Try
            Return EwEUtils.Core.eQuotaTypes.NoControls
        End Function


        Friend Function RunIndexToRunType(ByVal RunIndex As Integer) As eMSEBatchRunTypes

            Try
                Return Me.m_RunTypeLookup(RunIndex)
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".RunIndexToRunType() Exception: " & ex.Message)
            End Try
            Return eMSEBatchRunTypes.Any

        End Function


        Friend Function OuputTagToOuputType(ByVal OuputTag As String) As eMSEBatchOuputTypes
            Try
                Return Me.m_OuputTagToEnumLookup(OuputTag)
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".OuputTagToOuputType() Exception: " & ex.Message)
            End Try
            Return eMSEBatchOuputTypes.NotSet
        End Function

    End Class

End Namespace
