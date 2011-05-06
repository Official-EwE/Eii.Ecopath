Imports System.IO
Imports System.Text

Imports EwEUtils.Core
Imports EwEUtils.Utilities

'ToDo_jb cMonteCarloResultsWriter
'Added checkbox to interface to turn saving on and off
'SaveIteration() save instead of buffer the data
'Get the model name for the header and output filename


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

            If Not Directory.Exists(Me.DataDir) Then
                Directory.CreateDirectory(Me.DataDir)
            End If

            If File.Exists(OuputFilename) Then
                File.Delete(Me.OuputFilename)
            End If

            Me.WriteHeader()

        Catch ex As Exception

        End Try
    End Sub

    Private ReadOnly Property OuputFilename() As String
        Get
            'ToDo_jb MonteCarloWriter get the model name for the output filename
            Return Path.Combine(Me.DataDir, cFileUtils.ToValidFileName("MontCarlo-output.csv", False))
        End Get
    End Property



    Private ReadOnly Property DataDir() As String
        Get
            'For now 
            Return Path.Combine(Me.m_core.OutputPath, "MonteCarlo")
        End Get
    End Property

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

            header = New StringBuilder()
            Dim d As DateTime = Date.Now

            'safe a bunch of crap here....
            'model name blaaaaaa
            Dim ver As String = System.Reflection.Assembly.GetAssembly(GetType(cCore)).GetName.Version.ToString
            header.Append("Monte Carlo,EwE version number," & ver & vbCrLf) 'version number
            header.Append("Model name, " & Me.Core.EwEModel.Name & vbCrLf)
            header.Append("Ecosim scenario, " & Me.Core.EcosimScenarios(Me.Core.ActiveEcosimScenarioIndex).Name & vbCrLf)

            header.Append("Run Date, '" & d.ToLongDateString & " " & d.ToLongTimeString & vbCrLf)

            strm = New StreamWriter(Me.OuputFilename, True)
            strm.WriteLine(header)
            strm.Close()

        Catch ex As Exception

        End Try

    End Sub


    Public Sub SaveIteration(ByVal iIterationNumber As Integer)

        Dim buff As StringBuilder
        'Dim buff As String
        Dim strm As StreamWriter
        Dim igrp As Integer

        Try

            'Change this so writes the results for each data type
            'instead of buffering them
            buff = New StringBuilder()
            strm = New StreamWriter(Me.OuputFilename, True)

            buff.Append("Original SS,")
            buff.Append(cStringUtils.FormatSingle(Me.MC.SSorg))
            buff.Append(vbCrLf)

            buff.Append("Current SS,")
            buff.Append(cStringUtils.FormatSingle(Me.MC.SSCurrent))
            buff.Append(vbCrLf)

            buff.Append("Group Name")
            buff.Append(Me.ToCSVString(Core.m_EcoPathData.GroupName))

            buff.Append("Biomass")
            buff.Append(Me.ToCSVString(Core.m_EcoPathData.B))

            buff.Append("PB")
            buff.Append(Me.ToCSVString(Core.m_EcoPathData.PB))

            buff.Append("EE")
            buff.Append(Me.ToCSVString(Core.m_EcoPathData.EE))

            buff.Append("BA")
            buff.Append(Me.ToCSVString(Core.m_EcoPathData.BA))

            buff.Append("Biomass at Timestep")
            For it As Integer = 1 To Me.Core.m_EcoSimData.NTimes
                buff.Append(",")
                buff.Append(it.ToString)
            Next
            buff.Append(vbCrLf)

            For igrp = 1 To Me.Core.m_EcoPathData.NumGroups
                buff.Append(Core.m_EcoPathData.GroupName(igrp))
                buff.Append(Me.ToCSVString(Me.Core.m_EcoSimData.ResultsOverTime, cEcosimDatastructures.eEcosimResults.Biomass, igrp))
            Next

            strm.WriteLine(buff)
            strm.Close()

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".SaveIteration(" & iIterationNumber.ToString & ") Exception: " & ex.Message)
        End Try

    End Sub


    Private Function ToCSVString(ByVal Values(,,) As Single, ByVal Variable As cEcosimDatastructures.eEcosimResults, ByVal iGroup As Integer) As String
        Dim buff As String
        Try

            For it As Integer = 1 To Me.Core.m_EcoSimData.NTimes
                buff = buff & ","
                buff = buff & cStringUtils.FormatSingle(Values(Variable, iGroup, it))
            Next
            buff = buff & vbCrLf

        Catch ex As Exception
            Debug.Assert("ArrayToString() Exception: " & ex.Message)
        End Try

        Return buff

    End Function



    Private Function ToCSVString(ByVal Values() As String) As String
        Dim buff As String
        Try

            For igrp As Integer = 1 To Core.m_EcoPathData.NumGroups
                buff = buff & ","
                buff = buff & Values(igrp)
            Next
            buff = buff & vbCrLf

        Catch ex As Exception
            Debug.Assert("ArrayToString() Exception: " & ex.Message)
        End Try

        Return buff

    End Function


    Private Function ToCSVString(ByVal values() As Single) As String
        Dim buff As String
        Try

            For igrp As Integer = 1 To Core.m_EcoPathData.NumGroups
                buff = buff & ","
                buff = buff & cStringUtils.FormatSingle(values(igrp))
            Next
            buff = buff & vbCrLf


        Catch ex As Exception
            Debug.Assert("ArrayToString() Exception: " & ex.Message)
        End Try

        Return buff

    End Function

End Class
