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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cEcospaceCapacitySummarizer
    Implements IHashSummarizer

    Private m_core As cCore

    Public Sub New(Core As cCore)
        Me.m_core = Core
    End Sub

    Public Function Name() As String Implements IHashSummarizer.Name
        Return "EcospaceAppliedEnvironmentalResponse"
    End Function

    Public Function HashValues() As cHashValues() Implements IHashSummarizer.HashValues

        Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
        Dim capacity As cMapResponseInteractionManager = m_core.CapacityMapInteractionManager
        Dim shapes As cCapMapResponseManager = Me.m_core.CapacityShapeManager
        Dim sb As New Text.StringBuilder

        Dim lstHashValues As New List(Of cHashValues)

        Try
            For igrp As Integer = 1 To m_core.nGroups
                Dim grp As cEcospaceGroup = Me.m_core.EcospaceGroups(igrp)
                For imap As Integer = 1 To capacity.nMaps
                    Dim map As IEnviroInputMap = capacity.Map(imap)
                    Dim ishp As Integer = map.ResponseIndexForGroup(igrp)
                    If ishp > 0 Then
                        sb.Append("grp=" & igrp)
                        ' JS: this is the only place where driver layers are hashed
                        sb.Append(",map=")
                        sb.Append(cStringConverters.LayerToString(bm.InRow, bm.InCol, map.Layer))
                        sb.Append(",data=")
                        sb.Append(cStringConverters.ShapeToString(shapes.Item(ishp - 1)))
                        sb.Append("|")
                    End If
                Next imap
            Next igrp

            lstHashValues.Add(New cHashValues(Me.Name, "EnvResponses", sb.ToString))

        Catch ex As Exception

        End Try

        Return lstHashValues.ToArray()

    End Function

    Public Sub Init() Implements IHashSummarizer.Init

    End Sub

End Class
