' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports System.Xml.Serialization
Imports EwECore.Common

Public Class ETinputtot

    Public ModelName As String
    Public ModelDescription As String
    Public Comments As String
    Public NumGroups As Integer
    Public NumLivingGroups As Integer
    Public NumFleet As Integer
    Public GroupName() As String
    Public FleetName() As String
    ''' <summary>This is a model output</summary>
    Public TL() As Single
    ''' <summary>Is this absolute B?</summary>
    Public B() As Single
    ''' <summary>Is this absolute P?</summary>
    Public PROD() As Single
    ''' <summary>What is this?</summary>
    Public accessibility() As Single
    ''' <summary>What is this?</summary>
    Public OI() As Single
    ''' <summary>How is this indexed, (Fleet x group) or (group x fleet)?</summary>
    Public Catches()() As Single

End Class
