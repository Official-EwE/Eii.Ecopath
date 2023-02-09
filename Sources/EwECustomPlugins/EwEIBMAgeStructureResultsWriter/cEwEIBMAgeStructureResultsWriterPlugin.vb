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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports System.IO
Imports EwECore
Imports EwEPlugin
Imports EwEUtils

#End Region ' Imports

Public Class cEwEIBMAgeStructureResultsWriterPlugin
    Inherits cEcospaceBaseResultsWriter
    Implements IEcospaceResultWriterPlugin
    Implements ICoreDataPlugin

    Private m_StanzaData As cStanzaDatastructures
    Private m_bInitialized As Boolean
    Private m_iTime As Integer

    Private m_lstDataTypes As List(Of cInputDataTypes)

    Friend Enum eCalcType
        weight
        number
    End Enum

    Private Class cInputDataTypes
        Public ReadOnly Property DataTypeName As String

        'StanzaData.Npacket(isp, iage, ipkt)
        'or
        'StanzaData.Wpacket(isp, iage, ipkt)
        Public ReadOnly Property CalcType As eCalcType
        Public InputValues(,,) As Single

        Public SimBaseLineData(,) As Single

        Sub New(Inputs(,,) As Single, EcosimBaseLineData(,) As Single, TypeName As String, WeightOrNumber As eCalcType)
            InputValues = Inputs
            DataTypeName = TypeName
            CalcType = WeightOrNumber
            SimBaseLineData = EcosimBaseLineData
        End Sub

    End Class

    Public Sub New()
        MyBase.New()
        Me.vars = New eVarNameFlags() {eVarNameFlags.MultiStanzaAgeStructure}
    End Sub


#Region "Implementation"

    Private Sub SaveAgeStructure()
        Try

            If Not Me.EcospaceData.UseIBM Then
                Return
            End If

            Me.InitOutputTypes()
            Me.InitBaseFiles()

            For Each InputDataType As cInputDataTypes In Me.m_lstDataTypes

                'By species, region, ageclass
                Dim values()()() As Single
                values = ComputeAgeStructureByRegion(InputDataType)
                SaveRegionAgeStructureToFile(values, InputDataType)
                values = Nothing
            Next InputDataType


        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub


    Private Function ComputeAgeStructureByRegion(InputData As cInputDataTypes) As Single()()()

        Dim RegionValues(Me.m_StanzaData.Nsplit)()() As Single
        For isp As Integer = 1 To Me.m_StanzaData.Nsplit
            'Allocate memory for the all the regions
            RegionValues(isp) = New Single(Me.EcospaceData.nRegions)() {}
            'Now all the age classes for all the regions
            For iRgn As Integer = 0 To Me.EcospaceData.nRegions
                RegionValues(isp)(iRgn) = New Single(Me.m_StanzaData.MaxAgeSpecies(isp)) {}
            Next iRgn

            For iage As Integer = 0 To Me.m_StanzaData.MaxAgeSpecies(isp)

                For ipkt As Integer = 1 To Me.m_StanzaData.Npackets
                    Dim irow As Integer, icol As Integer
                    irow = CInt(Math.Truncate(Me.m_StanzaData.iPacket(isp, iage, ipkt)))
                    icol = CInt(Math.Truncate(Me.m_StanzaData.jPacket(isp, iage, ipkt)))

                    Dim iRgn As Integer = Me.EcospaceData.Region(irow, icol)

                    Dim value As Single
                    If InputData.CalcType = eCalcType.weight Then
                        value = InputData.InputValues(isp, iage, ipkt)
                    ElseIf InputData.CalcType = eCalcType.number Then
                        value = InputData.InputValues(isp, iage, ipkt) * Me.m_StanzaData.Npackets / Me.EcospaceData.ThabArea
                    End If

                    If iRgn > 0 Then
                        RegionValues(isp)(iRgn)(iage) += value
                    End If

                    RegionValues(isp)(0)(iage) += value

                Next ipkt

                For iiRgn As Integer = 0 To Me.EcospaceData.nRegions
                    'make sure this region has been allocated
                    If RegionValues(isp)(iiRgn) IsNot Nothing Then
                        RegionValues(isp)(iiRgn)(iage) /= Me.m_StanzaData.Npackets
                    End If
                Next iiRgn

            Next iage
        Next isp

        Return RegionValues

    End Function


    Private Function ComputeAgeStructureByRegion_LowMemory(InputData As cInputDataTypes) As Single()()()

        Dim RegionValues(Me.m_StanzaData.Nsplit)()() As Single

        For isp As Integer = 1 To Me.m_StanzaData.Nsplit
            'Allocate memory for the 0 region for both values and n
            RegionValues(isp) = New Single(Me.EcospaceData.nRegions)() {}
            RegionValues(isp)(0) = New Single(Me.m_StanzaData.MaxAgeSpecies(isp)) {}

            For iage As Integer = 0 To Me.m_StanzaData.MaxAgeSpecies(isp)

                For ipkt As Integer = 1 To Me.m_StanzaData.Npackets
                    Dim irow As Integer, icol As Integer
                    irow = CInt(Math.Truncate(Me.m_StanzaData.iPacket(isp, iage, ipkt)))
                    icol = CInt(Math.Truncate(Me.m_StanzaData.jPacket(isp, iage, ipkt)))

                    Dim iRgn As Integer = Me.EcospaceData.Region(irow, icol)
                    'Only allocate memory for age arrays if there is some packets in this region
                    If RegionValues(isp)(iRgn) Is Nothing Then
                        RegionValues(isp)(iRgn) = New Single(Me.m_StanzaData.MaxAgeSpecies(isp)) {}
                    End If

                    Dim value As Single
                    If InputData.CalcType = eCalcType.weight Then
                        value = InputData.InputValues(isp, iage, ipkt)
                    ElseIf InputData.CalcType = eCalcType.number Then
                        value = InputData.InputValues(isp, iage, ipkt) * Me.m_StanzaData.Npackets / Me.EcospaceData.ThabArea
                    End If

                    If iRgn > 0 Then
                        RegionValues(isp)(iRgn)(iage) += value
                    End If

                    RegionValues(isp)(0)(iage) += value

                Next ipkt

                For iiRgn As Integer = 0 To Me.EcospaceData.nRegions
                    'make sure this region has been allocated
                    If RegionValues(isp)(iiRgn) IsNot Nothing Then
                        RegionValues(isp)(iiRgn)(iage) /= Me.m_StanzaData.Npackets
                    End If
                Next iiRgn

            Next iage
        Next isp

        Return RegionValues

    End Function



    Protected Overrides Function FileExtension() As String
        Return "RegionAgeStructure"
    End Function

    Private Sub InitBaseFiles()

        If Not m_bInitialized Then
            For Each InputDataType As cInputDataTypes In Me.m_lstDataTypes
                Me.CreateBaseFiles(InputDataType)
            Next InputDataType
        End If

    End Sub


    Private Sub CreateBaseFiles(InputData As cInputDataTypes)

        Try
            Me.m_bInitialized = False
            For isp As Integer = 1 To Me.m_StanzaData.Nsplit

                For irgn As Integer = 0 To EcospaceData.nRegions

                    Dim strm As IO.StreamWriter = New IO.StreamWriter(getRegionFileName(isp, irgn, InputData.DataTypeName))
                    WriteRunInfo(strm)

                    Dim sbAges As Text.StringBuilder = New Text.StringBuilder

                    For iage As Integer = 0 To Me.m_StanzaData.MaxAgeSpecies(isp)
                        Dim iEco As Integer = Me.m_StanzaData.EcopathCode(isp, Me.m_StanzaData.StanzaNo(isp, iage))
                        sbAges.Append("," + EwEUtils.Utilities.cStringUtils.ToCSVField(Me.EcopathData.GroupName(iEco)) + "_" + CStr(iage))
                    Next iage
                    Dim header As String = "Timestep, Region" + sbAges.ToString


                    strm.WriteLine("Max Age," + CStr(Me.m_StanzaData.MaxAgeSpecies(isp)))
                    strm.WriteLine(header)

                    Dim SimAges As Text.StringBuilder = New Text.StringBuilder
                    SimAges.Append("0,Ecosim Base Values")
                    For iage As Integer = 0 To Me.m_StanzaData.MaxAgeSpecies(isp)
                        'Debug.Assert(Not Single.IsNaN(Values(isp)(irow, icol)(iage)))
                        SimAges.Append("," + CStr(InputData.SimBaseLineData(isp, iage)))
                    Next iage

                    strm.WriteLine(SimAges)

                    strm.Close()
                Next irgn

            Next isp

            Me.m_bInitialized = True

        Catch ex As Exception
            Dim msg As cMessage = New cMessage("Error creating IBM Age Structure file " + ex.Message,
                                               eMessageType.ErrorEncountered, eCoreComponentType.Ecospace,
                                               eMessageImportance.Warning)
            Me.m_core.Messages.AddMessage(msg)
            cLog.Write(ex)
        End Try


    End Sub



    Private Sub SaveRegionAgeStructureToFile(Values()()() As Single, InputDataType As cInputDataTypes)
        Dim iage As Integer
        For isp As Integer = 1 To Me.m_StanzaData.Nsplit
            Try

                Dim sbAges As Text.StringBuilder = New Text.StringBuilder

                For irgn As Integer = 0 To Me.EcospaceData.nRegions
                    Dim strm As IO.StreamWriter = New IO.StreamWriter(getRegionFileName(isp, irgn, InputDataType.DataTypeName), True)

                    'Does this group row col contain data
                    If Values(isp)(irgn) IsNot Nothing Then
                        'Yes it contains data write it out to file
                        Dim sb As Text.StringBuilder = New Text.StringBuilder

                        sb.Append(CStr(Me.m_iTime) + "," + CStr(irgn))
                        For ii As Integer = 0 To Me.m_StanzaData.MaxAgeSpecies(isp)

                            iage = ii + Me.m_StanzaData.MaxAgeSpecies(isp) - (Me.m_StanzaData.AgeIndex1(isp) - 1)
                            If iage > Me.m_StanzaData.MaxAgeSpecies(isp) Then
                                iage = ii - (Me.m_StanzaData.AgeIndex1(isp))
                            End If

                            sb.Append("," + CStr(Values(isp)(irgn)(iage)))
                        Next ii

                        strm.WriteLine(sb.ToString)
                        'may not need to do this as it will go out of scope
                        sb = Nothing
                    End If

                    strm.Close()
                Next irgn

            Catch ex As Exception
                cLog.Write(ex)
            End Try

        Next isp

    End Sub


    Private Function getRegionFileName(isp As Integer, iRegion As Integer, DataType As String) As String

        Dim region As String = CStr(iRegion)
        If iRegion = 0 Then
            region = "All"
        End If

        Dim fnTemplate As String = "AgeStructure_{0}_Region_{1}_{2}.csv"
        Return Path.Combine(MyBase.OutputDirectory, String.Format(fnTemplate, Me.m_StanzaData.StanzaName(isp), region, DataType))

    End Function


    Private Sub InitOutputTypes()

        m_lstDataTypes = New List(Of cInputDataTypes)

        Dim weight As New cInputDataTypes(Me.m_StanzaData.Wpacket, Me.m_StanzaData.WageS, "Weight", eCalcType.weight)
        m_lstDataTypes.Add(weight)

        Dim Number As New cInputDataTypes(Me.m_StanzaData.Npacket, Me.m_StanzaData.NageS, "Number", eCalcType.number)
        m_lstDataTypes.Add(Number)

    End Sub


#End Region


#Region "Plugin stuff"

    Public Overrides Sub WriteResults(SpaceTimeStepResults As Object)

        Try
            m_iTime = DirectCast(SpaceTimeStepResults, cEcospaceTimestep).iTimeStep
            SaveAgeStructure()
        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

    Public Overrides Sub Init(theCore As Object)
        MyBase.Init(theCore)
    End Sub

    Public Overrides ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            ' ToDo: globalize this
            Return "IBM Age Structure (csv format)"
        End Get
    End Property

    Public Overrides Sub StartWrite()
        MyBase.CreateOutputDir()

        Me.m_bInitialized = False
        Me.InitOutputTypes()


    End Sub

    Public Overrides Sub EndWrite()

    End Sub


    Public Sub Initialize(theCore As Object) Implements IPlugin.Initialize

    End Sub

    Public Sub CoreDataInitialized(objEcopathData As Object, objStanzaData As Object, objTaxonData As Object, objEcosamplerData As Object, objPDSdata As Object, objEcosimData As Object, objEcosimTimeSeriesData As Object, objSearchData As Object, objEcoSpaceData As Object) Implements ICoreDataPlugin.CoreDataInitialized


        m_StanzaData = TryCast(objStanzaData, cStanzaDatastructures)
        If m_StanzaData IsNot Nothing Then
            m_bInitialized = True
        End If


    End Sub


    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "IBM Age Structure"
        End Get
    End Property


    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            ' ToDo: localize this
            Return "IBM Age Structure"
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Dave Chagaris, Joe Buszowski"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

#End Region


#Region "NOT USED Age Structure by Cell"


#If False Then

'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
'AGE STRUCTURE BY CELL
'This code probable with not work as is
'It would need to be modified to work the same way as the Region Code
'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    Private Sub SaveAgeStructureToFile(Values()(,)() As Single)

        Dim fnTemplate As String = "{0}_AgeStructure_{1}.csv"


        For isp As Integer = 1 To Me.m_StanzaData.Nsplit

            Dim filename As String = Path.Combine(MyBase.OutputDirectory, String.Format(fnTemplate, Me.m_StanzaData.StanzaName(isp), CStr(Me.m_iTime)))
            Dim strm As IO.StreamWriter = New IO.StreamWriter(filename)
            WriteRunInfo(strm)

            Dim sbAges As Text.StringBuilder = New Text.StringBuilder

            For iage As Integer = 1 To Me.m_StanzaData.MaxAgeSpecies(isp) - 1
                Dim iEco As Integer = Me.m_StanzaData.EcopathCode(isp, Me.m_StanzaData.StanzaNo(isp, iage))
                sbAges.Append("," + EwEUtils.Utilities.cStringUtils.ToCSVField(Me.EcopathData.GroupName(iEco)) + "_" + CStr(iage))
            Next iage
            Dim header As String = "Stanza_Index, Multi_Name, Column, Row" + sbAges.ToString


            strm.WriteLine("Max Age," + CStr(Me.m_StanzaData.MaxAgeSpecies(isp)))
            strm.WriteLine(header)

            For irow As Integer = 1 To Me.EcospaceData.InRow
                For icol As Integer = 1 To Me.EcospaceData.InCol

                    'Does this group row col contain data
                    If Values(isp)(irow, icol) IsNot Nothing Then
                        'Yes it contains data write it out to file
                        Dim sb As Text.StringBuilder = New Text.StringBuilder

                        sb.Append(CStr(isp) + "," + Me.m_StanzaData.StanzaName(isp) + "," + CStr(icol) + "," + CStr(irow))
                        For iage As Integer = 1 To Me.m_StanzaData.MaxAgeSpecies(isp) - 1
                            'Dim ii As Integer = Me.m_StanzaData.AgeIndex1(isp) + iage
                            'If ii > Me.m_StanzaData.MaxAgeSpecies(isp) Then
                            '    ii = ii - Me.m_StanzaData.MaxAgeSpecies(isp) - 1
                            'End If
                            'Debug.Assert(Not Single.IsNaN(Values(isp)(irow, icol)(iage)))
                            sb.Append("," + CStr(Values(isp)(irow, icol)(iage)))
                        Next iage

                        strm.WriteLine(sb.ToString)
                        'may not need to do this as it will go out of scope
                        sb = Nothing
                    End If
                Next icol
            Next irow

            strm.Close()
        Next isp

    End Sub


    Private Function ComputeAgeStructureByCell() As Single()(,)()

        Dim Values(Me.m_StanzaData.Nsplit)(,)() As Single
        Dim n(Me.m_StanzaData.Nsplit)(,)() As Single 'n(ngroups)(row,col)(age)

        Dim RegionValues(Me.m_StanzaData.Nsplit)()() As Single

        Dim iage As Integer

        For isp As Integer = 1 To Me.m_StanzaData.Nsplit
            If Values(isp) Is Nothing Then
                Values(isp) = New Single(Me.EcospaceData.InRow, Me.EcospaceData.InCol)() {}
                n(isp) = New Single(Me.EcospaceData.InRow, Me.EcospaceData.InCol)() {}
                RegionValues(isp) = New Single(Me.EcospaceData.nRegions)() {}
            End If

            Console.WriteLine("---------------------------------------------isp = " + isp.ToString + "--------------------------------------------------------")

            For ipkt As Integer = 1 To Me.m_StanzaData.Npackets

                'This needs to use ageIndex1 as the location of the first age one index
                'For iage As Integer = Me.StanzaData.Age1(isp, ist) To Me.StanzaData.Age2(isp, ist)
                For ii As Integer = 0 To Me.m_StanzaData.MaxAgeSpecies(isp)
                    'iage = ii
                    iage = ii + Me.m_StanzaData.MaxAgeSpecies(isp) - Me.m_StanzaData.AgeIndex1(isp)
                    If ii >= Me.m_StanzaData.AgeIndex1(isp) Then
                        iage = ii - Me.m_StanzaData.AgeIndex1(isp)
                    End If

                    Dim irow As Integer, icol As Integer
                    irow = CInt(Math.Truncate(Me.m_StanzaData.iPacket(isp, iage, ipkt)))
                    icol = CInt(Math.Truncate(Me.m_StanzaData.jPacket(isp, iage, ipkt)))

                    If Values(isp)(irow, icol) Is Nothing Then
                        Values(isp)(irow, icol) = New Single(Me.m_StanzaData.MaxAgeSpecies(isp)) {}
                        n(isp)(irow, icol) = New Single(Me.m_StanzaData.MaxAgeSpecies(isp)) {}
                    End If

                    Dim iRgn As Integer = Me.EcospaceData.Region(irow, icol)
                    If RegionValues(isp)(iRgn) Is Nothing Then
                        RegionValues(isp)(iRgn) = New Single(Me.m_StanzaData.MaxAgeSpecies(isp)) {}
                        '  n(isp)(irow, icol) = New Single(Me.m_StanzaData.MaxAgeSpecies(isp)) {}
                    End If


                    If Me.m_StanzaData.Npacket(isp, ii, ipkt) > 0 Then
                        Values(isp)(irow, icol)(ii) += Me.m_StanzaData.Npacket(isp, iage, ipkt)

                        RegionValues(isp)(iRgn)(ii) += Me.m_StanzaData.Npacket(isp, iage, ipkt)

                        Debug.Assert(Not Single.IsNaN(Values(isp)(irow, icol)(iage)))
                        n(isp)(irow, icol)(iage) += 1
                    End If
                Next ii

            Next ipkt

        Next isp


        'For isp As Integer = 1 To Me.StanzaData.Nsplit

        '    For ir As Integer = 1 To Me.EcoSpaceData.InRow
        '        For ic As Integer = 1 To Me.EcoSpaceData.InCol
        '            If Values(isp)(ir, ic) IsNot Nothing Then
        '                For ii As Integer = 0 To Me.StanzaData.MaxAgeSpecies(isp)

        '                    'iage = Me.StanzaData.AgeIndex1(isp) + ii : If iage > Me.StanzaData.MaxAgeSpecies(isp) Then iage = iage - Me.StanzaData.MaxAgeSpecies(isp) - 1

        '                    If n(isp)(ir, ic)(ii) > 0 Then
        '                        Values(isp)(ir, ic)(ii) = Values(isp)(ir, ic)(ii) / n(isp)(ir, ic)(ii)
        '                    End If

        '                Next ii
        '            End If 'Weight(ieco)(ir, ic) IsNot Nothing

        '        Next ic
        '    Next ir
        'Next isp

        Return Values

    End Function

#End If
#End Region


End Class
