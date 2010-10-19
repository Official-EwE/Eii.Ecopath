#Region " Imports "

Option Strict On
Option Explicit On

Imports System
Imports System.IO
Imports System.Text
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Form for the EwE flow diagram plug-in.
''' </summary>
''' ===========================================================================
Public Class frmFlowDiagramPlugin

#Region " Privates "

    Private m_EcopathDs As cEcopathDataStructures
    Private m_Parent As cEwEFlowDiagramPlugin

#End Region ' Privates

    Public Sub New(ByVal strText As String, ByVal Parent As cEwEFlowDiagramPlugin)

        Me.InitializeComponent()
        Me.Text = strText
        Me.TabText = strText

        Me.m_Parent = Parent

    End Sub

    ''' <summary>
    '''  Create Ascci Flw put together all the necessary information from EcoPathGroupOutputs and EcoPathDataStructures
    ''' </summary>
    ''' <param name="flowfile"> Flw file's name to be exported</param>
    ''' <remarks></remarks>
    Public Sub CreateAsciiFlw(ByVal flowfile As String, Optional ByVal CreateFile As Boolean = True)  '(EiiFile As String)

        Dim core As cCore = Me.m_Parent.Core

        Dim GrpName As String
        Dim impVal As Single
        Dim fi(,) As Single
        Dim aStreamWriter As TextWriter
        Dim breakline As String
        Dim strError As String = ""
        ReDim fi(core.nGroups, core.nGroups)

        'Check if the extra necessery information is avaiable
        If (m_Parent.EcoPathDs IsNot Nothing) Then

            If CreateFile Then
                aStreamWriter = New StreamWriter(flowfile)

                breakline = aStreamWriter.NewLine()

                aStreamWriter.WriteLine(Format$(core.nGroups, "00"))
                aStreamWriter.Write(Format$(core.nLivingGroups, "00") & vbCrLf)
                aStreamWriter.NewLine = breakline

                'compute food index
                For i As Integer = 1 To core.nGroups
                    For j As Integer = 1 To core.nGroups
                        fi(i, j) = core.EcoPathGroupOutputs(i).Biomass * core.EcoPathGroupOutputs(i).QBOutput * core.EcoPathGroupInputs(i).DietComp(j)
                    Next j
                Next i

                For i As Integer = 1 To core.nGroups 'To 1 Step -1
                    GrpName = New String(" "c, 20)
                    Mid$(GrpName, 1, 15) = Trim(core.EcoPathGroupInputs(i).Name) 'Specie(i)

                    aStreamWriter.Write(GrpName)
                    aStreamWriter.Write(Stuff(Trim(MakeAmerican(Math.Abs(core.EcoPathGroupOutputs(i).TTLX), 3)), 12))
                    aStreamWriter.Write(Stuff(Trim(MakeAmerican((core.EcoPathGroupOutputs(i).Biomass), 3)), 12))
                    aStreamWriter.Write(Stuff(Trim(MakeAmerican(Math.Abs(core.EcoPathGroupOutputs(i).Biomass * core.EcoPathGroupOutputs(i).PBOutput), 3)), 12))
                    aStreamWriter.Write(Stuff(Trim(MakeAmerican((m_Parent.EcoPathDs.fCatch(i)), 3)), 12))
                    aStreamWriter.Write(Stuff(Trim(MakeAmerican((m_Parent.EcoPathDs.Ex(i)), 3)), 12))
                    aStreamWriter.Write(Stuff(Trim(MakeAmerican((core.EcoPathGroupOutputs(i).FlowToDet), 3)), 12))
                    aStreamWriter.Write(Stuff(Trim(MakeAmerican((core.EcoPathGroupOutputs(i).Respiration), 3)), 12))
                    aStreamWriter.Write(Stuff(Trim(MakeAmerican((fi(i, i)), 3)), 12))

                    If i > core.nLivingGroups Then
                        impVal = core.EcoPathGroupInputs(i).DetImport
                    Else
                        impVal = core.EcoPathGroupOutputs(i).Biomass * core.EcoPathGroupOutputs(i).QBOutput * core.EcoPathGroupInputs(i).DietComp(0)
                    End If

                    aStreamWriter.WriteLine(Stuff(Trim(MakeAmerican(impVal, 3)), 12))
                Next i

                'save food index => nt% = 21
                For i As Integer = 1 To core.nGroups
                    aStreamWriter.Write("                    ")
                    For j As Integer = 1 To core.nGroups
                        aStreamWriter.Write(Stuff(Trim(MakeAmerican(Math.Abs(fi(i, j)), 3)), 12))
                    Next j
                    aStreamWriter.WriteLine("")
                Next i

                'saves the Det() matrix for multiple det 121895 eli.
                For i As Integer = 1 To core.nGroups
                    aStreamWriter.Write("                    ")
                    'm_core.nLivingGroups +1 
                    For z As Integer = 1 To core.nDetritusGroups '+ 1 To m_core.nGroups
                        aStreamWriter.Write(Stuff(Trim(MakeAmerican(Math.Abs(core.EcoPathGroupInputs(i).DetritusFate(z)), 3)), 12) & vbCrLf)
                    Next z
                    aStreamWriter.Write("")
                Next i

                aStreamWriter.Close()
            End If

            'Execute the external application through the general function on EwEUtils
            If Not EwEUtils.SystemUtilities.cSystemUtils.AppExec("fd.exe", flowfile, strError, "") Then
                Dim msg As New cMessage("Unable to run application 'fd.exe': " & strError, _
                                        eMessageType.Any, eCoreComponentType.External, eMessageImportance.Critical)
                core.Messages.SendMessage(msg)
            End If
        Else
            Throw New Exception("EwEFlowDiagramPlugin: Ecopath data Structure was not initialized properly.")
        End If
    End Sub

    Private Function Stuff(ByVal tmpstr As String, ByVal length As Integer) As String
        Dim cpos As Integer
        Dim tmpstr2 As String

        If Len(tmpstr) >= length Then
            tmpstr2 = New String("0"c, Len(tmpstr))
            Mid(tmpstr2, 1) = tmpstr
        Else
            tmpstr2 = New String("0"c, length)
            Mid$(tmpstr2, (length - Len(tmpstr)) + 1) = tmpstr

            cpos = InStr(tmpstr2, "-")
            If cpos > 1 Then Mid$(tmpstr2, cpos, 1) = "0"
        End If

        If Strings.Left(tmpstr, 1) = "-" Then Mid$(tmpstr2, 1, 1) = "-"
        Stuff = tmpstr2
    End Function

    Private Function MakeAmerican(ByVal oldVal As Single, ByVal numdec As Integer) As String
        Dim tmpstr As String
        Dim tmpstr2 As String

        tmpstr2 = New String("0"c, numdec)
        tmpstr = Format(oldVal, "0." + tmpstr2)

        If InStr(tmpstr, ","c) = 1 Then tmpstr = ReplaceCommaWithPt(tmpstr, ","c, "."c)
        MakeAmerican = " " + tmpstr
    End Function

    Private Function ReplaceCommaWithPt(ByVal tmpstr As String, ByVal comma As Char, ByVal pt As Char) As String
        Dim cpos As Integer
        Dim newstr As String

        newstr = tmpstr
        For cpos = 1 To Len(tmpstr) Step 1
            If Mid(tmpstr, cpos, 1) = comma Then Mid(newstr, cpos, 1) = pt
        Next

        ReplaceCommaWithPt = newstr
    End Function

    Private Sub SDF_btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SDF_btn.Click
        Try
            Dim OutputFile As String = cFileUtils.MakeTempFile(Me.m_Parent.Core.EwEModel.Name & ".flw")

            If System.IO.File.Exists(OutputFile) Then
                ' File exist, prompt user
                Dim result As MsgBoxResult = MsgBox("Flow diagram file already exist, would you like to load it? " & vbCrLf & "Yes to load the file, No to make a new file", MsgBoxStyle.YesNo)
                If result = MsgBoxResult.Yes Then
                    ' No don't create the file
                    CreateAsciiFlw(OutputFile, False)
                Else
                    CreateAsciiFlw(OutputFile, True)
                End If
            Else
                CreateAsciiFlw(OutputFile, True)
            End If
        Catch ex As Exception
            Throw New Exception(ex.ToString)
        End Try

    End Sub

End Class