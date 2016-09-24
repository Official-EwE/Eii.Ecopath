Option Strict On
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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Imports System.IO
Imports EwECore

#End Region

Public Class cConfiguration

    Private m_core As cCore = Nothing
    Private Property Configuration As Boolean(,)

    Public Enum eResultTypes As Integer
        ''' <summary>Total system Biomass density (gC/m2)</summary>
        tsb
        ''' <summary>Total consumer Biomass density (gC/m2)</summary>
        tcb
        ''' <summary>B > 10cm (gC/m2)</summary>
        b10cm
        ''' <summary>B > 30cm (gC/m2)</summary>
        b30cm
        ''' <summary>Total catch (g wet biomass /m2)</summary>
        tc
        ''' <summary>Catch > 10cm (g wet biomass /m2)</summary>
        tc10cm
        ''' <summary>Catch > 30cm (g wet biomass /m2)</summary>
        tc30cm
        ''' <summary>Biomass of commercial species (gC/m2)</summary>
        bcom
    End Enum

    Public Sub New(core As cCore)
        Me.m_core = core
        ReDim Me.Configuration(core.nGroups, [Enum].GetValues(GetType(eResultTypes)).Length)
    End Sub

    Default Public Property Item(iGroup As Integer, cat As eResultTypes) As Boolean
        Get
            Return Me.Configuration(iGroup, cat)
        End Get
        Set(value As Boolean)
            Me.Configuration(iGroup, cat) = value
        End Set
    End Property

    Public Sub Load()

        Dim core As cCore = Me.m_core
        Dim strFile As String = Me.ConfigFileName()

        For i As Integer = 1 To core.nGroups
            For j As Integer = 0 To [Enum].GetValues(GetType(eResultTypes)).Length - 1
                Me.Configuration(i, j) = False
            Next
        Next

        If File.Exists(strFile) Then
            Dim r As New StreamReader(strFile)
            Dim l As String = ""

            Try
                While Not r.EndOfStream
                    l = r.ReadLine
                    If Not String.IsNullOrWhiteSpace(l) Then
                        If Not l.Trim.StartsWith("#"c) Then
                            Dim bits As String() = l.Split("="c)
                            Dim j As eResultTypes = 0
                            If [Enum].TryParse(bits(0), j) Then
                                For Each strGroup As String In bits(1).Split(" "c)
                                    Dim i As Integer = CInt(strGroup)
                                    Configuration(i, j) = True
                                Next
                            End If
                        End If
                    End If
                End While
            Catch ex As Exception
                ' Woopsy
            End Try
            r.Close()
        End If

    End Sub

    Public Sub Save()

        Dim core As cCore = Me.m_core
        Dim strFile As String = Me.ConfigFileName()
        Dim w As New StreamWriter(strFile)

        w.WriteLine("# FishMIP data aggregation scheme for model " & Me.m_core.EwEModel.Name)
        w.WriteLine("# Full model path " & Me.m_core.DataSource.ToString)
        w.WriteLine()

        For Each j As eResultTypes In [Enum].GetValues(GetType(eResultTypes))
            Dim b As Boolean = False
            w.Write(j.ToString & "=")
            For i As Integer = 1 To core.nGroups
                If (Me.Configuration(i, j)) Then
                    If (b) Then w.Write(" ")
                    w.Write(i)
                    b = True
                End If
            Next
            w.WriteLine()
        Next
        w.Flush()
        w.Close()

    End Sub

#Region " Presets "

    Public Sub LoadEcoOcean()

        Dim core As cCore = cFishMIPcore.GetInstance().Core
        Dim smalluns As Integer() = New Integer() {1, 4, 7, 10, 13, 16}
        Dim config As cConfiguration = cFishMIPcore.GetInstance().Configuration

        For Each cat As cConfiguration.eResultTypes In [Enum].GetValues(GetType(cConfiguration.eResultTypes))

            For igroup As Integer = 1 To core.nGroups

                Dim bChecked As Boolean = False
                Dim grp As cEcoPathGroupInput = core.EcoPathGroupInputs(igroup)
                Dim grpOut As cEcoPathGroupOutput = core.EcoPathGroupOutputs(igroup)
                Dim name As String = grp.Name.ToLower()

                Select Case cat
                    Case cConfiguration.eResultTypes.tsb
                        bChecked = grp.IsProducer() Or grp.IsConsumer()
                    Case cConfiguration.eResultTypes.tcb
                        bChecked = grp.IsConsumer() And grpOut.TTLX() > 1
                    Case cConfiguration.eResultTypes.b10cm
                        bChecked = grp.Index <= 24
                    Case cConfiguration.eResultTypes.b30cm
                        bChecked = grp.Index <= 24 And Array.IndexOf(smalluns, grp.Index) = -1
                    Case cConfiguration.eResultTypes.tc
                        bChecked = grp.IsFished()
                    Case cConfiguration.eResultTypes.tc10cm
                        bChecked = grp.IsFished() And grp.Index <= 24
                    Case cConfiguration.eResultTypes.tc30cm
                        bChecked = grp.IsFished() And grp.Index <= 24 And Array.IndexOf(smalluns, grp.Index) = -1
                End Select

                config(igroup, cat) = bChecked
            Next
        Next
        config.Save()

    End Sub

#End Region ' Presets 

    Private Function ConfigFileName() As String
        Dim strFile As String = Me.m_core.DataSource.ToString
        Return Path.Combine(Path.GetDirectoryName(strFile), Path.GetFileNameWithoutExtension(strFile) & "_fishmip.config")
    End Function

End Class
