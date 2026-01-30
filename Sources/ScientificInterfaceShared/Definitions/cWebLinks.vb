' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Reflection
Imports EwEUtils.Utilities



''' <summary>
''' All web links
''' </summary>
''' <remarks>
''' ToDo: provide this content via a web service
''' </remarks>
Public Class cWebLinks

    Private Const g_PostInstall As String = "https://www.ecopath.org/post-install"
    Private Const g_Start As String = "https://www.ecopath.org/ewestart"
    Private Const g_Home As String = "https://www.ecopath.org"
    Private Const g_UsersRSS As String = "https://groups.google.com/forum/feed/eweusers/msgs/rss.xml?num=15"
    Private Const g_Trac As String = "https://sources.ecopath.org/trac/Ecopath/report/1"
    Private Const g_Course As String = "https://www.ecopath.org/courses"
    Private Const g_GoPro As String = "https://www.ecopath.org/gopro"
    Private Const g_Facebook As String = "https://www.facebook.com/eweconsortium"
    Private Const g_BetaFeedback As String = "https://www.surveymonkey.com/s/5XD6HKC"
    Private Const g_EcoBase As String = "https://ecobase.ecopath.org/"
    Private Const g_EcoBaseModelInfo As String = "https://ecobase.ecopath.org/index.php?ident=base_eco&pass=base_eco&provenance=ecopath&action=base&menu=0&model={0}"
    Private Const g_Access2010 As String = "https://www.microsoft.com/en-us/download/details.aspx?id=13255"
    Private Const g_TextBook As String = "https://pressbooks.bccampus.ca/ewemodel"
    Private Const g_UserGuide As String = "https://pressbooks.bccampus.ca/eweguide"


    Private m_core As cCore = Nothing

    Public Sub New(core As cCore)
        Me.m_core = core
    End Sub

    Public Enum eLinkType As Integer
        NotSet = 0
        PostInstall
        Start
        Home
        UsersRSS
        Trac
        Facebook
        Courses
        Feedback
        EcoBase
        EcoBaseModelInfo
        Access2010
        GoPro
        EwETextBook
        EwEUserGuide
    End Enum

    Public Function GetURL(type As eLinkType) As String

        Select Case type
            Case eLinkType.NotSet : Return Me.EwEHomeURL(False)
            Case eLinkType.PostInstall : Return Me.EwEHomeURL(True)
            Case eLinkType.Start : Return Me.EwEHomeURL(False)
            Case eLinkType.Home : Return cWebLinks.g_Home
            Case eLinkType.UsersRSS : Return cWebLinks.g_UsersRSS
            Case eLinkType.Trac : Return cWebLinks.g_Trac
            Case eLinkType.Courses : Return cWebLinks.g_Course
            Case eLinkType.Facebook : Return cWebLinks.g_Facebook
            Case eLinkType.Feedback : Return cWebLinks.g_BetaFeedback
            Case eLinkType.EcoBase : Return cWebLinks.g_EcoBase
            Case eLinkType.EcoBaseModelInfo : Return cWebLinks.g_EcoBaseModelInfo
            Case eLinkType.Access2010 : Return cWebLinks.g_Access2010
            Case eLinkType.GoPro : Return cWebLinks.g_GoPro
            Case eLinkType.EwETextBook : Return cWebLinks.g_TextBook
            Case eLinkType.EwEUserGuide : Return cWebLinks.g_UserGuide
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
    Private Function EwEHomeURL(bPostInstall As Boolean) As String

        Dim pm As cPluginManager = Me.m_core.PluginManager
        Dim aAssemblyNames As AssemblyName() = cAssemblyUtils.GetSummary(cAssemblyUtils.eSummaryFlags.EwECore)
        Dim ub As New cUriBuilder(If(bPostInstall, g_PostInstall, g_Start))

        For Each an As AssemblyName In aAssemblyNames
            ' Keep ewe component list really short; it's the plug-ins we're interested in
            If ((String.Compare(an.Name, "ewecore", True) = 0) Or (String.Compare(an.Name, "ewe6", True) = 0)) Then
                If Not ub.QueryString.ContainsKey(an.Name) Then ub.QueryString(an.Name) = an.Version.ToString
            End If
        Next an

        If (pm IsNot Nothing) Then
            aAssemblyNames = pm.PluginAssemblyNames
            For Each an As AssemblyName In aAssemblyNames
                If Not ub.QueryString.ContainsKey(an.Name) Then ub.QueryString(an.Name) = an.Version.ToString
            Next an
        End If

        Return ub.ToString()

    End Function

End Class
