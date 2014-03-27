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

        Public Sub New()

        End Sub

        Public Sub New(FleetID As Integer, regMethod As eRegMethod)

            Me.New()
            mFleetID = FleetID : mRegMethod = regMethod

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
        ListofRegs = New List(Of cReg)
        RegulationsFileExists = False
        RegulationsLoaded = False
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

    Public Function LoadRegsFromCSV(StrategyNumber As Integer) As Boolean

        Dim reader As StreamReader = Nothing
        Dim csv As CsvReader = Nothing
        Dim bSuccess As Boolean = True
        Dim filePath As String = cMSEUtils.MSEFile(mMSE.DataPath, cMSEUtils.eMSEPaths.Fleet, "Regulations.csv")
        Dim Reg As cReg

        If File.Exists(filePath) Then

            reader = cMSEUtils.GetReader(filePath)
            If (reader IsNot Nothing) Then
                Try
                    csv = New CsvReader(reader, True)
                    RegulationsFileExists = True
                    If CInt(csv.Item(StrategyNumber - 1, 0)) <> StrategyNumber Then Return False

                    For iFleet = 1 To mCore.nFleets
                        Reg = New cReg
                        Reg.mFleetID = iFleet
                        Reg.mRegMethod = CType(csv.Item(StrategyNumber - 1, iFleet), eRegMethod)
                        ListofRegs.Add(Reg)
                    Next
                    Return True
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

End Class
