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


Public Class cEcospaceASCMapMOResultsWriter
    Inherits cEcospaceASCBaseResultsWriter
    Implements IEcospaceResultWriterPlugin

    Public Sub New()
        MyBase.New()
        Me.vars = New eVarNameFlags() {eVarNameFlags.LayerOtherMortLoss}
    End Sub

    Public Overrides Sub WriteResults(SpaceTimeStepResults As Object)
        MyBase.WriteResults(SpaceTimeStepResults)
    End Sub

    Public Overrides Sub Init(theCore As Object)
        MyBase.Init(theCore)
        Me.setAllGroupsSelected()
    End Sub

    Public Overrides ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            ' ToDo: globalize this
            Return "Other mortality response function loss (ACSII format)"
        End Get
    End Property

    Protected Overrides Sub WriteASCIIBody(writer As StreamWriter, SpaceTSData As cEcospaceTimestep, iIndex As Integer, varname As eVarNameFlags)

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'Overrides the base class implementation to get MOLoss() from the core data
        'instead of the cEcospaceTimestep object
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


        'get MO biomass from the core data
        Dim MoData(,) As Single = Me.EcospaceData.MOLoss(iIndex)
        Dim value As Double = 0
        Dim strValue As String = ""

        Debug.Assert(MoData IsNot Nothing)

        For ir As Integer = 1 To Me.EcospaceData.InRow
            For ic As Integer = 1 To Me.EcospaceData.InCol
                If ic > 1 Then writer.Write(" ")
                If Me.EcospaceData.Depth(ir, ic) > 0 Then
                    value = CSng(MoData(ir, ic))
                    If (value <> cCore.NULL_VALUE) Then
                        value = Me.ScaleValue(value, SpaceTSData, iIndex, varname)
                    End If
                Else
                    'land as NODATAVALUE
                    value = cCore.NULL_VALUE
                End If

                ' Fix #1321 - always make sure the first cell value is written as floating point
                strValue = EwEUtils.Utilities.cStringUtils.FormatNumber(value)
                If (ir = 1 And ic = 1) Then
                    If (strValue.IndexOf("."c) = -1) Then
                        strValue = strValue + ".0"
                    End If
                End If

                writer.Write(strValue)
            Next
            writer.WriteLine("")
        Next

    End Sub

    Public Sub Initialize(theCore As Object) Implements IPlugin.Initialize

    End Sub

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "MortalityOtherResultsWriter"
        End Get
    End Property


    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            ' ToDo: localize this
            Return "Mortality due to MO resonse function"
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


End Class
