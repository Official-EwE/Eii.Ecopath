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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Option Explicit On

Imports System.IO
Imports EwECore
Imports EwEUtils.Utilities
Imports LumenWorks.Framework.IO.Csv

#End Region ' Imports

Public Class cRegulations

    Public Const START_TAG As String = "<REGULATIONS_START>"
    Public Const END_TAG As String = "<REGULATIONS_END>"

    Enum eRegMethod
        None = 0
        NoQuota = 1
        WeakestStock
        HighestValue
        SelectiveFishing
    End Enum

    Public Class cReg
        Public mFleetID As Integer
        Public mRegMethod As eRegMethod
        Public mFleetName As String

        Public Sub New()

        End Sub

        Public Sub New(FleetName As String, FleetID As Integer, regMethod As eRegMethod)

            Me.New()
            mFleetID = FleetID
            mFleetName = FleetName
            mRegMethod = regMethod

        End Sub

    End Class

    Public ListofRegs As List(Of cReg)
    Private mMSE As cMSE
    Private mCore As cCore
    Public RegulationsFileExists As Boolean
    Public RegulationsLoaded As Boolean


    Sub New(MSE As cMSE, Core As cCore)
        mMSE = MSE
        mCore = Core

        Me.initDefaultRegs()
        RegulationsFileExists = False
        RegulationsLoaded = False
    End Sub

    Private Sub initDefaultRegs()

        ListofRegs = New List(Of cReg)
        For iFleet = 1 To mCore.nFleets
            ListofRegs.Add(New cReg(mCore.FleetInputs(iFleet).Name, iFleet, eRegMethod.None))
        Next

    End Sub


    Public Function GetReg(iFleet As Integer) As eRegMethod
        Dim FoundFleet As Boolean = False

        For FleetListPointer = 1 To ListofRegs.Count
            If iFleet = ListofRegs(FleetListPointer - 1).mFleetID Then
                Return ListofRegs(FleetListPointer - 1).mRegMethod
            End If
        Next

        Return eRegMethod.None

    End Function

    'Commented out 31-3-14 if still not required and commented out by 5-14 then delete
    'Public Function LoadRegsFromCSV(StrategyNumber As Integer) As Boolean

    '    Dim reader As StreamReader = Nothing
    '    Dim csv As CsvReader = Nothing
    '    Dim bSuccess As Boolean = True
    '    Dim filePath As String = cMSEUtils.MSEFile(mMSE.DataPath, cMSEUtils.eMSEPaths.Strategies, "Regulations.csv")
    '    Dim Reg As cReg

    '    If File.Exists(filePath) Then

    '        reader = cMSEUtils.GetReader(filePath)
    '        If (reader IsNot Nothing) Then
    '            Try
    '                csv = New CsvReader(reader, True)
    '                RegulationsFileExists = True
    '                If CInt(csv.Item(StrategyNumber - 1, 0)) <> StrategyNumber Then Return False

    '                For iFleet = 1 To mCore.nFleets
    '                    Reg = New cReg
    '                    Reg.mFleetID = iFleet
    '                    Reg.mRegMethod = CType(csv.Item(StrategyNumber - 1, iFleet), eRegMethod)
    '                    ListofRegs.Add(Reg)
    '                Next
    '                Return True
    '                csv.Dispose()
    '            Catch ex As Exception
    '                'Debug.Assert(False, Me.ToString & ".LoadEcosimParameters() Exception: " & ex.Message)
    '                bSuccess = False
    '            End Try
    '            cMSEUtils.ReleaseReader(reader)
    '        End If
    '    Else
    '        bSuccess = False
    '    End If

    '    Return bSuccess

    'End Function

    Public Function Read(Filename As String) As Boolean
        Dim buff As String
        Dim recs() As String
        Dim breturn As Boolean = False

        Try

            Dim reader As StreamReader = cMSEUtils.GetReader(Filename)
            If (reader IsNot Nothing) Then

                'Find the tag in the file
                If cMSEUtils.readToTag(reader, START_TAG) Then
                    'read the header line
                    reader.ReadLine()
                    Do Until reader.EndOfStream

                        buff = reader.ReadLine()
                        recs = buff.Split(","c)
                        If Not recs(0).Contains(END_TAG) Then
                            Dim iflt As Integer
                            Dim reg As cReg

                            iflt = cStringUtils.ConvertToInteger(recs(1))
                            'get the reg object out of the list based on the fleet index
                            reg = Me.ListofRegs.Item(iflt - 1)
                            Debug.Assert(reg.mFleetName = cMSEUtils.FromCSVField(recs(0)), "Oppss Fleetname in file does not match Core Fleetname for fleet." + iflt.ToString)
                            reg.mRegMethod = CType(cStringUtils.ConvertToInteger(recs(2)), eRegMethod)

                            breturn = True

                        Else
                            'end of the data bump out
                            Exit Do
                        End If 'Not recs(0).Contains(END_TAG)
                    Loop
                End If 'cMSEUtils.readToTag(reader, START_TAG)

                cMSEUtils.ReleaseReader(reader)

            End If '(reader IsNot Nothing)

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString + ".Read() Exception: " + ex.Message)
        End Try

        Debug.Assert(breturn, Me.ToString + ".Read() Failed to read regulations from file.")

        Return breturn

    End Function

    Public Function Save(filename As String) As Boolean
        Dim strm As StreamWriter
        'Append onto the end of an existing file
        strm = cMSEUtils.GetWriter(filename, True)
        If (strm IsNot Nothing) Then

            'msg.AddVariable(New cVariableStatus(eStatusFlags.OK, _
            '                                    String.Format(My.Resources.STATUS_SAVED_DETAIL, Path.GetFileName(iStrategy.FileName)), _
            '                                    eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))
            strm.WriteLine(START_TAG)
            strm.WriteLine("FleetName,FleetIndex,Regulation")
            For Each reg In Me.ListofRegs
                strm.WriteLine(cStringUtils.ToCSVField(reg.mFleetName) & "," & _
                                          cStringUtils.ToCSVField(reg.mFleetID) & "," & _
                                          cStringUtils.ToCSVField(reg.mRegMethod))
            Next
            strm.WriteLine(END_TAG)
            cMSEUtils.ReleaseWriter(strm)
        End If

        Return True

    End Function

End Class
