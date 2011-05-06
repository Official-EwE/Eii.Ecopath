
Imports System.IO
Imports System.Text

Imports EwEUtils.Core
Imports EwECore.MSE
Imports EwEUtils.Utilities


Friend Class cMSECSVOutputWriter
    Implements IMSEOutputWriter

    Private m_core As cCore
    'Private m_dataDir As String
    Private m_MSEdata As cMSEDataStructures


    Public Sub New(ByVal theCore As cCore, ByVal MSEData As cMSEDataStructures)
        Me.m_core = theCore
        Me.m_MSEdata = MSEData
    End Sub

    Public Function getOutputFileName(ByVal strDataType As String, ByVal strDataName As String) As String
        Return Path.Combine(Me.DataDir, cFileUtils.ToValidFileName(strDataType & strDataName & ".csv", False))
    End Function

    'Private Function getOutputDirectory() As String

    '    Try

    '        Dim modelPath As String = DirectCast(Me.m_core.DataSource.Connection, Database.cEwEAccessDatabase).Name
    '        If File.Exists(modelPath) Then
    '            Return Path.Combine(Path.GetDirectoryName(modelPath), "MSE\")
    '        Else
    '            System.Console.WriteLine("MSE Failed to find database directory from the currently loaded model.")
    '            Return (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MSE\"))
    '        End If
    '    Catch ex As Exception
    '        Debug.Assert(False, Me.ToString & ".getOutputDirectory() Exception: " & ex.Message)
    '    End Try

    '    Return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MSE\")

    'End Function

    Public Sub saveIteration(ByVal ListOfData As Dictionary(Of cMSE.eResultsData, Single(,))) Implements IMSEOutputWriter.saveIteration

        If Not Me.m_MSEdata.SaveOutput Then Return

        Dim buff As StringBuilder = Nothing
        Dim strm As StreamWriter = Nothing
        Dim esData As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim epData As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Try
            'We could set this up so each type had a seperate flag for dumping

            'Biomass
            For igrp As Integer = 1 To Me.m_MSEdata.NGroups
                Try
                    buff = New StringBuilder()
                    For its As Integer = 1 To Me.m_core.GetCoreCounter(eCoreCounterTypes.nEcosimTimeSteps)
                        If (its > 1) Then buff.Append(", ")
                        buff.Append(cStringUtils.FormatSingle(esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, its)))
                    Next

                    strm = New StreamWriter(getOutputFileName(cMSE.BIOMASS_DATA, epData.GroupName(igrp)), True)
                    strm.WriteLine(buff)
                    strm.Close()
                    buff = Nothing
                Catch ex As Exception
                    ' Debug.Assert(False, Me.ToString & " Exception saving results to file " & getFilename(BIOMASS_DATA, epdata.GroupName(igrp)))
                    System.Console.WriteLine(Me.ToString & " Failed to write data to file " & getOutputFileName(cMSE.BIOMASS_DATA, epData.GroupName(igrp)) & " Exception: " & ex.Message)
                End Try
            Next

            'Catch by group
            For igrp As Integer = 1 To Me.m_MSEdata.NGroups
                Try
                    If epData.fCatch(igrp) > 0 Then
                        buff = New StringBuilder()
                        For its As Integer = 1 To Me.m_core.GetCoreCounter(eCoreCounterTypes.nEcosimTimeSteps)
                            If (its > 1) Then buff.Append(", ")
                            buff.Append(cStringUtils.FormatSingle(esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, its)))
                        Next

                        strm = New StreamWriter(getOutputFileName(cMSE.CATCH_DATA, epData.GroupName(igrp)), True)
                        strm.WriteLine(buff)
                        strm.Close()
                        buff = Nothing
                    End If
                Catch ex As Exception
                    ' Debug.Assert(False, Me.ToString & " Exception saving results to file " & getFilename(BIOMASS_DATA, epdata.GroupName(igrp)))
                    System.Console.WriteLine(Me.ToString & " Failed to write data to file " & getOutputFileName(cMSE.BIOMASS_DATA, epData.GroupName(igrp)) & " Exception: " & ex.Message)
                End Try
            Next

            'Quota by group
            For igrp As Integer = 1 To Me.m_MSEdata.NGroups
                Try
                    Dim data(,) As Single = ListOfData.Item(cMSE.eResultsData.GroupQuota)
                    If epData.fCatch(igrp) > 0 Then
                        buff = New StringBuilder()
                        For its As Integer = 1 To Me.m_core.GetCoreCounter(eCoreCounterTypes.nEcosimTimeSteps)
                            If (its > 1) Then buff.Append(", ")
                            buff.Append(cStringUtils.FormatSingle(data(igrp, its)))
                        Next

                        strm = New StreamWriter(getOutputFileName(cMSE.QUOTAGROUP_DATA, epData.GroupName(igrp)), True)
                        strm.WriteLine(buff)
                        strm.Close()
                        buff = Nothing
                    End If
                Catch ex As Exception
                    ' Debug.Assert(False, Me.ToString & " Exception saving results to file " & getFilename(BIOMASS_DATA, epdata.GroupName(igrp)))
                    System.Console.WriteLine(Me.ToString & " Failed to write data to file " & getOutputFileName(cMSE.BIOMASS_DATA, epData.GroupName(igrp)) & " Exception: " & ex.Message)
                End Try
            Next

            'Catch by fleet
            For iflt As Integer = 1 To Me.m_MSEdata.nFleets
                Try
                    buff = New StringBuilder()
                    For its As Integer = 1 To Me.m_core.GetCoreCounter(eCoreCounterTypes.nEcosimTimeSteps)
                        If (its > 1) Then buff.Append(", ")
                        buff.Append(cStringUtils.FormatSingle(esData.ResultsSumCatchByGear(iflt, its)))
                    Next

                    strm = New StreamWriter(getOutputFileName(cMSE.FLEETCATCH_DATA, epData.FleetName(iflt)), True)
                    strm.WriteLine(buff)
                    strm.Close()
                    buff = Nothing

                Catch ex As Exception
                    'Debug.Assert(False, Me.ToString & " Exception saving results to file " & getFilename(CATCH_DATA, epdata.FleetName(iflt)))
                    System.Console.WriteLine(Me.ToString & " Failed to write data to file " & getOutputFileName(cMSE.FLEETCATCH_DATA, epData.FleetName(iflt)) & " Exception: " & ex.Message)
                End Try
            Next

            'Effort by fleet
            For iflt As Integer = 1 To Me.m_MSEdata.nFleets
                Try
                    buff = New StringBuilder()
                    For its As Integer = 1 To Me.m_core.GetCoreCounter(eCoreCounterTypes.nEcosimTimeSteps)
                        If its > 1 Then buff.Append(", ")
                        buff.Append(cStringUtils.FormatSingle(esData.ResultsEffort(iflt, its)))
                    Next

                    strm = New StreamWriter(getOutputFileName(cMSE.EFFORT_DATA, epData.FleetName(iflt)), True)
                    strm.WriteLine(buff)
                    strm.Close()
                    buff = Nothing

                Catch ex As Exception
                    ' Debug.Assert(False, Me.ToString & " Exception saving results to file " & getFilename(EFFORT_DATA, epdata.GroupName(iflt)))
                    System.Console.WriteLine(Me.ToString & " Failed to write data to file " & Me.getOutputFileName(cMSE.EFFORT_DATA, epData.FleetName(iflt)) & " Exception: " & ex.Message)
                End Try
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".SaveIteration() Exception: " & ex.Message)
        End Try

    End Sub

    Private Sub WriteOutputHeader(ByVal DataDescription As String, ByVal GroupFleet As String, ByVal DataFileName As String)

        Try
            If Not Me.m_MSEdata.SaveOutput Then Exit Sub

            Dim header As StringBuilder
            Dim strm As StreamWriter

            header = New StringBuilder()
            Dim d As DateTime = Date.Now

            header.Append("MSE " & DataDescription & vbCrLf)
            header.Append("Date, '" & d.ToLongDateString & " " & d.ToLongTimeString & vbCrLf)
            header.Append("Group, '" & GroupFleet & "'" & vbCrLf)
            header.Append("Rows = MSE Run, Columns = Time" & vbCrLf)

            For it As Integer = 1 To Me.m_core.nEcosimTimeSteps
                If it > 1 Then header.Append(", ")
                header.Append(cStringUtils.FormatInteger(it))
            Next

            strm = New StreamWriter(Me.getOutputFileName(DataFileName, GroupFleet), True)
            strm.WriteLine(header)
            strm.Close()

        Catch ex As Exception

        End Try

    End Sub

    Public ReadOnly Property DataDir() As String
        Get
            'Return Me.m_dataDir
            Return Path.Combine(Me.m_core.OutputPath, "MSE")
        End Get
    End Property

    Public Sub Init() Implements IMSEOutputWriter.Init

        '  Me.m_dataDir = Me.getOutputDirectory

        If Not Me.m_MSEdata.SaveOutput Then Exit Sub

        Try
            Dim epData As cEcopathDataStructures = Me.m_core.m_EcoPathData

            If Not Directory.Exists(Me.DataDir) Then
                Directory.CreateDirectory(Me.DataDir)
            End If

            'clear out any existing data files
            For igrp As Integer = 1 To Me.m_MSEdata.NGroups
                Try
                    File.Delete(Me.getOutputFileName(cMSE.BIOMASS_DATA, epData.GroupName(igrp)))
                    File.Delete(Me.getOutputFileName(cMSE.CATCH_DATA, epData.GroupName(igrp)))
                    File.Delete(Me.getOutputFileName(cMSE.QUOTAGROUP_DATA, epData.GroupName(igrp)))
                Catch ex As Exception
                    System.Console.WriteLine(ex.Message)
                End Try
            Next igrp

            For iflt As Integer = 1 To Me.m_MSEdata.nFleets
                Try
                    File.Delete(Me.getOutputFileName(cMSE.FLEETCATCH_DATA, epData.FleetName(iflt)))
                    File.Delete(Me.getOutputFileName(cMSE.EFFORT_DATA, epData.FleetName(iflt)))
                Catch ex As Exception
                    System.Console.WriteLine()
                End Try
            Next iflt

            'Write output file headers

            For igrp As Integer = 1 To Me.m_MSEdata.NGroups
                Me.WriteOutputHeader("Biomass", epData.GroupName(igrp), cMSE.BIOMASS_DATA)
                If epData.fCatch(igrp) > 0 Then
                    Me.WriteOutputHeader("Catch by Group", epData.GroupName(igrp), cMSE.CATCH_DATA)
                    Me.WriteOutputHeader("Quota by Group", epData.GroupName(igrp), cMSE.QUOTAGROUP_DATA)
                End If
            Next

            For iflt As Integer = 1 To Me.m_MSEdata.nFleets
                Me.WriteOutputHeader("Catch by Fleet", epData.FleetName(iflt), cMSE.FLEETCATCH_DATA)
                Me.WriteOutputHeader("Effort by Fleet", epData.FleetName(iflt), cMSE.EFFORT_DATA)
            Next iflt

        Catch ex As Exception

        End Try
    End Sub

End Class
