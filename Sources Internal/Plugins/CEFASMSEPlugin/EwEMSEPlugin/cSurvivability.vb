#Region " Imports "

Imports System.IO
Imports LumenWorks.Framework.IO.Csv
Imports EwEUtils.Utilities
Imports Troschuetz.Random

#End Region

Public Class cSurvivability

#Region " Internal Variables "

    ''' <summary>
    ''' Stores a list of all the information for each survivability distribution
    ''' </summary>
    ''' <remarks></remarks>
    Private mListofSuriveDistParams As List(Of cSurvivabilityDistributonParam)
    ''' <summary>
    ''' Stores a list of all the sampled survivabilities
    ''' </summary>
    ''' <remarks></remarks>
    Private mSampledSurvivability As List(Of cSampledSurvivability)
    ''' <summary>
    ''' Reference to the EwE core
    ''' </summary>
    ''' <remarks></remarks>
    Private mcore As EwECore.cCore
    ''' <summary>
    ''' Path to the data directory
    ''' </summary>
    ''' <remarks></remarks>
    Private mdatapath As String
    ''' <summary>
    ''' Reference to the MSE plugin
    ''' </summary>
    ''' <remarks></remarks>
    Private mMSE As cMSE
    ''' <summary>
    ''' Equals True if the survivability distribution parameters file exists
    ''' </summary>
    ''' <remarks></remarks>
    Private mSurvDistFileExists As Boolean
    ''' <summary>
    ''' Equals TRUE if the survivability distribution parameters file is formatted correctly
    ''' </summary>
    ''' <remarks></remarks>
    Private mSurvDistFileValid As Boolean
    ''' <summary>
    ''' Equals True if the sampled survivability parameters file exists
    ''' </summary>
    ''' <remarks></remarks>
    Private mSurvParamFileExists As Boolean
    ''' <summary>
    ''' Equals TRUE if the sampled survivability parameters file is formatted correctly
    ''' </summary>
    ''' <remarks></remarks>
    Private mSurvParamFileValid As Boolean

#End Region

#Region " Construction "

    Public Sub New(ByRef DataPath As String, core As EwECore.cCore, MSE As cMSE)
        mdatapath = DataPath
        mcore = core
        mMSE = MSE
    End Sub

    Sub New()
        ' TODO: Complete member initialization 
    End Sub

#End Region

#Region " Internal Classes "

    ''' <summary>
    ''' Stores a single survivability probabability distribution
    ''' </summary>
    ''' <remarks></remarks>
    Public Class cSurvivabilityDistributonParam

        Public Sub New(ByVal FleetNumber As Integer, ByVal FleetName As String, ByVal GroupNumber As Integer, ByVal GroupName As String, _
                       ByVal Alpha As Double, ByVal Beta As Double)
            Me.FleetNo = FleetNumber
            Me.FleetName = FleetName
            Me.GroupNo = GroupNumber
            Me.GroupName = GroupName
            Me.Alpha = Alpha
            Me.Beta = Beta

        End Sub

        Public Property FleetNo As Integer
        Public Property FleetName As Integer
        Public Property GroupNo As Integer
        Public Property GroupName As String
        Public Property Alpha As Double
        Public Property Beta As Double

    End Class

    ''' <summary>
    ''' Stores a single sampled survivability parameter
    ''' </summary>
    ''' <remarks></remarks>
    Public Class cSampledSurvivability

        Public Property Iteration As Integer
        Public Property FleetNo As Integer
        Public Property FleetName As String
        Public Property GroupNo As Integer
        Public Property GroupName As String
        Public Property Survivability As Double

        Public Sub New(ByVal Iteration As Integer, ByVal FleetNumber As Integer, ByVal FleetName As String, ByVal GroupNumber As Integer, ByVal GroupName As String, ByVal Survivability As Double)

            Me.Iteration = Iteration
            Me.FleetNo = FleetNumber
            Me.FleetName = FleetName
            Me.GroupNo = GroupNumber
            Me.GroupName = GroupName
            Me.Survivability = Survivability

        End Sub

    End Class

#End Region

#Region " Distribution Parameter Elements "

#Region " Properties "

    ''' <summary>
    ''' Returns the list of all the survivability distribution parameters
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property ListofSurvDistParams() As List(Of cSurvivabilityDistributonParam)
        Get
            Return mListofSuriveDistParams
        End Get
    End Property

    ''' <summary>
    ''' Returns whether the Survivability distribution file exists
    ''' </summary>
    ''' <remarks></remarks>
    Public Property SurvivabilityDistFileExists() As Boolean
        Get
            Return mSurvDistFileExists
        End Get
        Set(ByVal value As Boolean)
            mSurvDistFileExists = value
        End Set
    End Property

    ''' <summary>
    ''' Returns the number of elements in the survivability distribution list
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property CountDist() As Integer
        Get
            Return mListofSuriveDistParams.Count
        End Get
    End Property

#End Region

#Region " Functions "

    ''' <summary>
    ''' Checks whether the survivability distribution file is valid
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function DistFileValid()
        'TODO MP add validation code to check whether distribution file is okay

        'what checks need doing?

    End Function

    ''' <summary>
    ''' Adds a distribution parameter to the list of survivability distribution parameters
    ''' and if it can't returns FALSE
    ''' </summary>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddDist(param) As Boolean
        Dim ValuesOkay As Boolean

        'Check Fleet Number
        If param.FleetNumber < 0 Or param.FleetNumber > mcore.nFleets Then Return False

        'Check F.leet Name
        ValuesOkay = False
        For iFleet = 1 To mcore.nFleets
            If mcore.FleetInputs(iFleet).Name = param.FleetName Then ValuesOkay = True
        Next
        If ValuesOkay = False Then Return False

        'Check GroupNo
        If param.GroupNo < 0 Or param.GroupNo > mcore.nGroups Then Return False

        'Check GroupName
        ValuesOkay = False
        For iGroup = 1 To mcore.nGroups
            If mcore.EcoPathGroupInputs(iGroup).Name = param.GroupName Then ValuesOkay = True
        Next
        If ValuesOkay = False Then Return False

        'Check Alpha and Beta
        If param.Alpha <= 0 Or param.Beta <= 0 Then Return False

        mListofSuriveDistParams.Add(New cSurvivabilityDistributonParam(param.FleetNumber, param.FleetName, param.GroupNo, _
                                                                  param.GroupName, param.Alpha, param.Beta))

        Return True

    End Function

    ''' <summary>
    ''' Reads the iRow_th from the list of survivability distribution parameters
    ''' </summary>
    ''' <param name="iRow"></param>
    ''' <returns></returns>
    ''' <remarks>iRow is zero-based</remarks>
    Public Function ReadRowDist(iRow As Integer) As cSurvivabilityDistributonParam
        If iRow < 0 Then Return Nothing
        If iRow > mListofSuriveDistParams.Count - 1 Then Return Nothing
        Return mListofSuriveDistParams(iRow)
    End Function

    ''' <summary>
    ''' Reads the survivability distribution parameters for iGroup as discarded by iFleet
    ''' </summary>
    ''' <param name="iFleet"></param>
    ''' <param name="iGroup"></param>
    ''' <returns></returns>
    ''' <remarks>iFleet and iGroup are zero-based</remarks>
    Public Function ReadiFleetiGroupDist(iFleet As Integer, iGroup As Integer) As cSurvivabilityDistributonParam

        If iFleet < 1 Or iFleet > mcore.nFleets Then Return Nothing
        If iGroup < 1 Or iGroup > mcore.nGroups Then Return Nothing

        For iRow As Integer = 0 To mListofSuriveDistParams.Count - 1
            If mListofSuriveDistParams(iRow).FleetNo = iFleet And mListofSuriveDistParams(iRow).GroupNo = iGroup Then Return mListofSuriveDistParams(iRow)
        Next

        Return Nothing

    End Function

    ''' <summary>
    ''' Reads the survivability distribution parameters of GroupName as fished by FleetName
    ''' </summary>
    ''' <param name="FleetName"></param>
    ''' <param name="GroupName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadFleetGroupNamesDist(FleetName As String, GroupName As String)

        For iRow As Integer = 0 To mListofSuriveDistParams.Count - 1
            If mListofSuriveDistParams(iRow).FleetName = FleetName And mListofSuriveDistParams(iRow).GroupName = GroupName Then Return mListofSuriveDistParams(iRow)
        Next

        Return Nothing

    End Function

    ''' <summary>
    ''' Loads the survivabilities distribution parameters from CSV
    ''' </summary>
    ''' <returns>True if successful, false otherwise</returns>
    ''' <remarks></remarks>
    Public Function LoadDistFromCSV()

        Dim reader As StreamReader = Nothing
        Dim csv As CsvReader = Nothing
        Dim param As cSurvivabilityDistributonParam
        Dim bSuccess As Boolean = True
        Dim filePath As String = cMSEUtils.MSEFile(mdatapath, cMSEUtils.eMSEPaths.DistrParams, "Survivabilities.csv")

        If File.Exists(filePath) Then

            reader = cMSEUtils.GetReader(filePath)
            If (reader IsNot Nothing) Then
                Try
                    csv = New CsvReader(reader, True)
                    While Not csv.EndOfStream
                        param = ExtractSurvivabilityDist(csv)
                        If (param IsNot Nothing) Then
                            AddDist(param)
                        End If
                    End While
                    csv.Dispose()

                Catch ex As Exception
                    'Debug.Assert(False, Me.ToString & ".LoadEcosimParameters() Exception: " & ex.Message)
                    bSuccess = False
                End Try
                cMSEUtils.ReleaseReader(reader)
            End If
        Else
            bSuccess = False
        End If

        Return bSuccess

    End Function

    ''' <summary>
    ''' Extracts a survivability distribution parameter + information from csv
    ''' </summary>
    ''' <param name="csv">The CSV object linking to the survivability distribution parameter file</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ExtractSurvivabilityDist(ByVal csv As CsvReader) As cSurvivabilityDistributonParam

        ' Sanity checks
        If (csv Is Nothing) Then Return Nothing
        If (Not csv.ReadNextRecord()) Then Return Nothing

        Dim TFleetNumber As Integer
        Dim TFleetName As String
        Dim TGroupNumber As Integer
        Dim TGroupName As String
        Dim TAlpha As Double
        Dim TBeta As Double

        Try
            TFleetNumber = cStringUtils.ConvertToInteger(csv(0))
            TFleetName = cMSEUtils.FromCSVField(csv(1))
            TGroupNumber = cStringUtils.ConvertToInteger(csv(2))
            TGroupName = cMSEUtils.FromCSVField(csv(3))
            TAlpha = cStringUtils.ConvertToDouble(csv(4))
            TBeta = cStringUtils.ConvertToDouble(csv(5))

        Catch ex As Exception
            ' ToDo_JS: respond to error
            Return Nothing
        End Try

        Return New cSurvivabilityDistributonParam(TFleetNumber, TFleetNumber, TGroupNumber, TGroupName, TAlpha, TBeta)

    End Function

    ''' <summary>
    ''' Saves survivability distribution information to CSV
    ''' </summary>
    ''' <returns>False if there was an error</returns>
    ''' <remarks></remarks>
    Public Function SaveDistributionParamsToCSV()

        Dim writer As StreamWriter = cMSEUtils.GetWriter(cMSEUtils.MSEFile(mdatapath, cMSEUtils.eMSEPaths.DistrParams, "Survivabilities.csv"), False)
        Dim bSuccess As Boolean = False

        If (writer Is Nothing) Then Return bSuccess

        Try
            writer.WriteLine("FleetNumber,FleetName,GroupNumber,GroupName,Alpha,Beta")

            For Each entry As cSurvivabilityDistributonParam In mListofSuriveDistParams
                writer.WriteLine(cStringUtils.ToCSVField(entry.FleetNo) & "," & _
                                 cStringUtils.ToCSVField(entry.FleetName) & "," & _
                                 cStringUtils.ToCSVField(entry.GroupNo) & "," & _
                                 cStringUtils.ToCSVField(entry.GroupName) & "," & _
                                 cStringUtils.ToCSVField(entry.Alpha) & "," & _
                                 cStringUtils.ToCSVField(entry.Beta))
            Next

            bSuccess = True

        Catch ex As Exception

        End Try
        cMSEUtils.ReleaseWriter(writer)
        Return bSuccess

    End Function

#End Region

#End Region

#Region " Sampled Parameter Elements "

#Region " Properties"

    ''' <summary>
    ''' Checks whether the Sampled Parameters file is valid
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ParamFileValid()

    End Function

    ''' <summary>
    ''' Returns whether the sampled param file is valid
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property SurvParamFileValid
        Get
            Return mSurvParamFileValid
        End Get
    End Property

    ''' <summary>
    ''' Returns whether the Sampled survivability file exists
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SurvivabilityParamFileExists() As Boolean
        Get
            Return mSurvParamFileExists
        End Get
        Set(ByVal value As Boolean)
            mSurvParamFileExists = value
        End Set
    End Property

#End Region

#Region " Functions "

    ''' <summary>
    ''' Samples the survivability parameters
    ''' </summary>
    ''' <param name="nParams">The number of models to generate</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SampleParams(nParams As Integer) As Boolean
        Dim TempSurvDistParam As cSurvivabilityDistributonParam
        Dim TempSampledParam As Double
        Dim BetaGenerator As New BetaDistribution

        Try
            For iParameter = 1 To nParams
                For iRow = 0 To mListofSuriveDistParams.Count - 1
                    TempSurvDistParam = Me.ReadRowDist(iRow)
                    BetaGenerator.Alpha = TempSurvDistParam.Alpha
                    BetaGenerator.Beta = TempSurvDistParam.Beta
                    TempSampledParam = BetaGenerator.NextDouble()
                    mSampledSurvivability.Add(New cSampledSurvivability(iParameter, TempSurvDistParam.FleetNo, TempSurvDistParam.FleetName, _
                                                                        TempSurvDistParam.GroupNo, TempSurvDistParam.GroupName, _
                                                                        TempSampledParam))
                Next
            Next
        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' Load the sampled parameters from CSV
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LoadParamFromCSV()

        Dim reader As StreamReader = Nothing
        Dim csv As CsvReader = Nothing
        Dim param As cSampledSurvivability
        Dim bSuccess As Boolean = True
        Dim filePath As String = cMSEUtils.MSEFile(mdatapath, cMSEUtils.eMSEPaths.ParamsOut, "Survivabilities_out.csv")

        If File.Exists(filePath) Then

            reader = cMSEUtils.GetReader(filePath)
            If (reader IsNot Nothing) Then
                Try
                    csv = New CsvReader(reader, True)
                    While Not csv.EndOfStream
                        param = ExtractSurvivabilityParam(csv)

                        If (param IsNot Nothing) Then
                            mSampledSurvivability.Add(New cSampledSurvivability(param.Iteration, param.FleetNo, param.FleetName, param.GroupNo, _
                                                          param.GroupName, param.Survivability))
                        End If
                    End While
                    csv.Dispose()

                Catch ex As Exception
                    Debug.Assert(False, Me.ToString & ".LoadEcosimParameters() Exception: " & ex.Message)
                    bSuccess = False
                End Try
                cMSEUtils.ReleaseReader(reader)
            End If
        Else
            Return bSuccess = False
        End If

        Return bSuccess

    End Function

    ''' <summary>
    ''' Extract a single line from the sampled survivability parameter file
    ''' </summary>
    ''' <param name="csv">The csv object that links to the file</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ExtractSurvivabilityParam(ByVal csv As CsvReader) As cSampledSurvivability
        ' Sanity checks
        If (csv Is Nothing) Then Return Nothing
        If (Not csv.ReadNextRecord()) Then Return Nothing

        Dim TIteration As Integer
        Dim TFleetNumber As Integer
        Dim TFleetName As String
        Dim TGroupNumber As Integer
        Dim TGroupName As String
        Dim TSurvivability As Double

        Try
            TIteration = cStringUtils.ConvertToInteger(csv(0))
            TFleetNumber = cMSEUtils.FromCSVField(csv(1))
            TFleetName = cStringUtils.ConvertToInteger(csv(2))
            TGroupNumber = cMSEUtils.FromCSVField(csv(3))
            TGroupName = cStringUtils.ConvertToDouble(csv(4))
            TSurvivability = cStringUtils.ConvertToDouble(csv(5))

        Catch ex As Exception
            ' ToDo_JS: respond to error
            Return Nothing
        End Try

        Return New cSampledSurvivability(TIteration, TFleetNumber, TFleetNumber, TGroupNumber, TGroupName, TSurvivability)


    End Function

    ''' <summary>
    ''' Saves the sampled survivabilities to CSV
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SaveSampledToCSV()

        Dim writer As StreamWriter = cMSEUtils.GetWriter(cMSEUtils.MSEFile(mdatapath, cMSEUtils.eMSEPaths.ParamsOut, "Survivabilities_out.csv"), False)

        Dim bSuccess As Boolean = False

        If (writer Is Nothing) Then Return bSuccess

        Try
            writer.WriteLine("Iteration,FleetNumber,FleetName,GroupNumber,GroupName,Survivability")

            For Each entry As cSampledSurvivability In mSampledSurvivability
                writer.WriteLine(cStringUtils.ToCSVField(entry.Iteration) & "," & _
                                 cStringUtils.ToCSVField(entry.FleetNo) & "," & _
                                 cStringUtils.ToCSVField(entry.FleetName) & "," & _
                                 cStringUtils.ToCSVField(entry.GroupNo) & "," & _
                                 cStringUtils.ToCSVField(entry.GroupName) & "," & _
                                 cStringUtils.ToCSVField(entry.Survivability))
            Next

            bSuccess = True

        Catch ex As Exception

        End Try
        cMSEUtils.ReleaseWriter(writer)
        Return bSuccess

    End Function

#End Region

#End Region

#Region " General Elements "

    ''' <summary>
    ''' Runs when the MSE plugin has been loaded up
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PluginLoaded()
        Dim reader As StreamReader = Nothing

        'Todo MP
        ' check file exists for surivability distribution parameters
        If Not File.Exists(cMSEUtils.MSEFile(Me.mdatapath, cMSEUtils.eMSEPaths.DistrParams, "Survivabilities.csv")) Then
            SurvivabilityDistFileExists = False
            mSurvDistFileValid = False
        Else
            ' check file is correct
            If Not DistFileValid() Then
                mSurvDistFileValid = False
            Else
                'If it is load the file into memory
                mSurvDistFileValid = True
                LoadDistFromCSV()
            End If
        End If

        If Not File.Exists(cMSEUtils.MSEFile(Me.mdatapath, cMSEUtils.eMSEPaths.ParamsOut, "Survivabilites_out.csv")) Then
            SurvivabilityParamFileExists = False
            mSurvParamFileValid = False
        Else
            If Not ParamFileValid() Then
                mSurvParamFileValid = False
            Else
                mSurvParamFileValid = True
                LoadParamFromCSV()
            End If
        End If

    End Sub

#End Region







End Class

