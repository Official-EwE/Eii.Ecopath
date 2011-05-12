Imports System.IO
Imports System.Text

Imports EwEUtils.Core
Imports EwEUtils.Utilities

'ToDo_jb cMonteCarloResultsWriter
'Error handling could set the bSaveOutput flag to false if there is an error
'this is problematic as the interface now has to be updated but the MonteCarlo is not a proper CoreInputOutput object...

Public Class cMonteCarloResultsWriter

    Private m_MC As cEcosimMonteCarlo
    Private m_core As cCore

    Public Sub New(ByVal MonteCarlo As cEcosimMonteCarlo, ByVal theCore As cCore)

        Me.m_MC = MonteCarlo
        Me.m_core = theCore

    End Sub


    Public Sub Init()

        If Not Me.MC.bSaveOutput Then Exit Sub

        Try

            If cFileUtils.IsDirectoryAvailable(Me.Core.OutputPath, True) Then

                If File.Exists(OuputFilename) Then
                    File.Delete(Me.OuputFilename)
                End If

                Me.WriteHeader()

                'save the baseline data
                Me.Save(True)

            End If

        Catch ex As Exception
            'Me.MC.bSaveOutput = False
            Dim msg As New cMessage("Error saving Monte Carlo data to file. " & ex.Message, eMessageType.ErrorEncountered, _
                                    eCoreComponentType.EcoSimMonteCarlo, eMessageImportance.Warning, eDataTypes.MonteCarlo)
            Me.Core.Messages.SendMessage(msg)
            cLog.Write(ex)
        End Try
    End Sub

    Private ReadOnly Property OuputFilename() As String
        Get
            Return Path.Combine(Me.DataDir, Me.Core.EcosimOutputFileName("MonteCarlo", "IterationData", ".csv"))
        End Get
    End Property


    Private Function DataDir() As String
        Return Me.Core.OutputPath
    End Function


    Private ReadOnly Property ModelName() As String
        Get
            Return Me.Core.DataSource.Filename
        End Get
    End Property


    Private Function ScenarioName() As String
        Return Me.m_core.EcosimScenarios(Me.m_core.ActiveEcosimScenarioIndex).Name
    End Function

    Private ReadOnly Property MC() As cEcosimMonteCarlo
        Get
            Return Me.m_MC
        End Get
    End Property


    Private ReadOnly Property Core() As cCore
        Get
            Return Me.m_core
        End Get
    End Property


    Private Sub WriteHeader()
        Try
            If Not Me.MC.bSaveOutput Then Exit Sub

            Dim header As StringBuilder
            Dim strm As StreamWriter
            Dim ver As String = System.Reflection.Assembly.GetAssembly(GetType(cCore)).GetName.Version.ToString
            Dim d As Date = Date.Now

            header = New StringBuilder()

            'save a bunch of crap here....
            'model name blaaaaaa
            header.AppendLine("EwE Monte Carlo version number," & ver) 'version number
            header.AppendLine("Model name," & Chr(34) & Me.ModelName & Chr(34))
            header.AppendLine("Ecosim scenario," & Chr(34) & Me.ScenarioName & Chr(34))
            header.AppendLine("Run Date," & Chr(34) & d.ToShortDateString & " " & d.ToShortTimeString & Chr(34))

            strm = New StreamWriter(Me.OuputFilename, True)
            strm.WriteLine(header)
            strm.Close()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".WriteHeader() Exception: " & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Save both iteration and baseline data to file
    ''' </summary>
    ''' <param name="isBaseLineData"></param>
    ''' <remarks></remarks>
    Public Sub Save(ByVal isBaseLineData As Boolean)
        Dim strm As StreamWriter

        Try

            If Not Me.MC.bSaveOutput Then Exit Sub

            strm = New StreamWriter(Me.OuputFilename, True)

            'empty line at the start of a new data block
            strm.WriteLine("")

            If isBaseLineData Then
                strm.WriteLine(Me.getParameterVariance)
            End If

            If isBaseLineData Then
                strm.WriteLine("Base line data")
            Else
                strm.WriteLine("Trial number," & Me.MC.nTrialIterations.ToString)
            End If

            strm.Write("Original SS,")
            strm.WriteLine(cStringUtils.FormatSingle(Me.MC.SSorg))

            'Don't output the current SS if this is the baseline data
            If Not isBaseLineData Then
                strm.Write("Current SS,")
                strm.WriteLine(cStringUtils.FormatSingle(Me.MC.SSCurrent))
            End If

            strm.WriteLine("Ecopath parameters")

            strm.Write("Group Name,")
            strm.WriteLine(Me.ToCSVString(Core.m_EcoPathData.GroupName))

            strm.Write("Biomass,")
            strm.WriteLine(Me.ToCSVString(Core.m_EcoPathData.B))

            strm.Write("PB,")
            strm.WriteLine(Me.ToCSVString(Core.m_EcoPathData.PB))

            strm.Write("EE,")
            strm.WriteLine(Me.ToCSVString(Core.m_EcoPathData.EE))

            strm.Write("BA,")
            strm.WriteLine(Me.ToCSVString(Core.m_EcoPathData.BA))

            strm.Write("Ecosim biomass")
            For it As Integer = 1 To Me.Core.m_EcoSimData.NTimes
                strm.Write(",")
                strm.Write(it.ToString)
            Next
            strm.Write(vbCrLf)

            'biomass at T from Ecosim results
            For igrp As Integer = 1 To Me.Core.m_EcoPathData.NumGroups
                strm.Write(Core.m_EcoPathData.GroupName(igrp) & ",")
                strm.WriteLine(Me.ToCSVString(Me.Core.m_EcoSimData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.Biomass, igrp))
            Next

            strm.Close()
            strm = Nothing

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".SaveIteration(...) Exception: " & ex.Message)
        End Try

        'Make sure the stream did not get left open somehow
        Try
            If strm IsNot Nothing Then
                strm.Close()
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Function getParameterVariance() As String
        Dim buff As New StringBuilder
        Dim igrp As Integer

        'Group name
        buff.AppendLine("Group Name," & Me.ToCSVString(Core.m_EcoPathData.GroupName))

        'CV's
        buff.AppendLine("Biomass CV," & Me.ToCSVString(Me.MC.CVpar, eMCParams.Biomass))
        buff.AppendLine("Biomass lower limit," & Me.ToCSVString(Me.MC.ParLimit, 0, eMCParams.Biomass))
        buff.AppendLine("Biomass upper limit," & Me.ToCSVString(Me.MC.ParLimit, 1, eMCParams.Biomass))

        buff.AppendLine("P/B CV," & Me.ToCSVString(Me.MC.CVpar, eMCParams.PB))
        buff.AppendLine("P/B lower limit," & Me.ToCSVString(Me.MC.ParLimit, 0, eMCParams.PB))
        buff.AppendLine("P/B upper limit," & Me.ToCSVString(Me.MC.ParLimit, 1, eMCParams.PB))

        buff.AppendLine("QB CV," & Me.ToCSVString(Me.MC.CVpar, eMCParams.QB))
        buff.AppendLine("QB lower limit," & Me.ToCSVString(Me.MC.ParLimit, 0, eMCParams.QB))
        buff.AppendLine("QB upper limit," & Me.ToCSVString(Me.MC.ParLimit, 1, eMCParams.QB))

        buff.AppendLine("EE CV," & Me.ToCSVString(Me.MC.CVpar, eMCParams.EE))
        buff.AppendLine("EE lower limit," & Me.ToCSVString(Me.MC.ParLimit, 0, eMCParams.EE))
        buff.AppendLine("EE upper limit," & Me.ToCSVString(Me.MC.ParLimit, 1, eMCParams.EE))

        Return buff.ToString

    End Function



    Private Function ToCSVString(ByVal Values(,,) As Single, ByVal FirstFixedIndex As Integer, ByVal SecondFixedIndex As Integer) As String
        Dim buff As String
        Try

            For igrp As Integer = 1 To Core.m_EcoPathData.NumGroups
                If igrp > 1 Then buff = buff & ","
                buff = buff & cStringUtils.FormatSingle(Values(FirstFixedIndex, SecondFixedIndex, igrp))
            Next

        Catch ex As Exception
            Debug.Assert("ArrayToString() Exception: " & ex.Message)
        End Try

        Return buff

    End Function



    Private Function ToCSVString(ByVal Values(,) As Single, ByVal FixedIndex As Integer) As String
        Dim buff As String
        Try

            For igrp As Integer = 1 To Core.m_EcoPathData.NumGroups
                If igrp > 1 Then buff = buff & ","
                buff = buff & cStringUtils.FormatSingle(Values(FixedIndex, igrp))
            Next

        Catch ex As Exception
            Debug.Assert("ArrayToString() Exception: " & ex.Message)
        End Try

        Return buff

    End Function


    Private Function ToCSVString(ByVal Values(,,) As Single, ByVal Variable As cEcosimDatastructures.eEcosimResults, ByVal iGroup As Integer) As String
        Dim buff As String
        Try

            For it As Integer = 1 To Me.Core.m_EcoSimData.NTimes
                If it > 1 Then buff = buff & ","
                buff = buff & cStringUtils.FormatSingle(Values(Variable, iGroup, it))
            Next

        Catch ex As Exception
            Debug.Assert("ArrayToString() Exception: " & ex.Message)
        End Try

        Return buff

    End Function



    Private Function ToCSVString(ByVal Values() As String) As String
        Dim buff As String
        Try

            For igrp As Integer = 1 To Core.m_EcoPathData.NumGroups
                If igrp > 1 Then buff = buff & ","
                buff = buff & Chr(34) & Values(igrp) & Chr(34)
            Next

        Catch ex As Exception
            Debug.Assert("ArrayToString() Exception: " & ex.Message)
        End Try

        Return buff

    End Function


    Private Function ToCSVString(ByVal values() As Single) As String
        Dim buff As String
        Try

            For igrp As Integer = 1 To Core.m_EcoPathData.NumGroups
                If igrp > 1 Then buff = buff & ","
                buff = buff & cStringUtils.FormatSingle(values(igrp))
            Next

        Catch ex As Exception
            Debug.Assert("ArrayToString() Exception: " & ex.Message)
        End Try

        Return buff

    End Function

End Class
