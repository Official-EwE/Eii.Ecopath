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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Reflection
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Utilities

#End Region ' Imports

''' <summary>
''' All web links
''' </summary>
''' <remarks>
''' ToDo: provide this content via a web service
''' </remarks>
Public Class cWebLinks

    Private Const cStart As String = "http://www.ecopath.org/nonewe/eweexe/index.php"
    Private Const cHome As String = "http://www.ecopath.org"
    Private Const cRSS As String = "http://www.ecopath.org/aggregator/categories/1"
    Private Const cTrac As String = "http://sources.ecopath.org/trac/Ecopath/report/1"
    Private Const cCourse As String = "http://www.ecopath.org/courses"
    Private Const cForum As String = "http://www.ecopath.org/forum"
    Private Const cFacebook As String = "http://www.facebook.com/eweconsortium"
    Private Const cFeedback As String = "http://www.surveymonkey.com/s/5XD6HKC"

    Private m_core As cCore = Nothing

    Public Sub New(core As cCore)
        Me.m_core = core
    End Sub

    Public Enum eLinkType As Integer
        NotSet = 0
        Start
        Home
        HomeRSS
        Trac
        Facebook
        Forums
        Courses
        Feedback
    End Enum

    Public Function GetURL(type As eLinkType) As String

        Select Case type
            Case eLinkType.Start : Return Me.EwEHomeURL()
            Case eLinkType.Home : Return cWebLinks.cHome
            Case eLinkType.HomeRSS : Return cWebLinks.cRSS
            Case eLinkType.Trac : Return cWebLinks.cTrac
            Case eLinkType.Courses : Return cWebLinks.cCourse
            Case eLinkType.Forums : Return cWebLinks.cForum
            Case eLinkType.Facebook : Return cWebLinks.cFacebook
            Case eLinkType.Feedback : Return cWebLinks.cFeedback
        End Select
        Return ""

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Conjure the EwE base URL for invoking the EwE start page, including
    ''' version check.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function EwEHomeURL() As String

        Dim pm As cPluginManager = Me.m_core.PluginManager
        Dim aAssemblyNames As AssemblyName() = cAssemblyUtils.GetSummary(cAssemblyUtils.eSummaryFlags.EwECore)
        Dim ub As New cUriBuilder(cStart)

        For Each an As AssemblyName In aAssemblyNames
            ' Keep ewe component list really short; it's the plug-ins we're interested in
            If ((String.Compare(an.Name, "ewecore", True) = 0) Or (String.Compare(an.Name, "ewe6", True) = 0)) Then
                If Not ub.QueryString.ContainsKey(an.Name) Then ub.QueryString(an.Name) = an.Version.ToString
            End If
        Next an

        If (Not Object.ReferenceEquals(pm, Nothing)) Then
            aAssemblyNames = pm.PluginAssemblyNames
            For Each an As AssemblyName In aAssemblyNames
                If Not ub.QueryString.ContainsKey(an.Name) Then ub.QueryString(an.Name) = an.Version.ToString
            Next an
        End If

        Return ub.ToString()

    End Function

End Class
