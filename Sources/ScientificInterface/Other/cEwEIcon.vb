' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to pick the current Icon for EwE6.
''' </summary>
''' <remarks>
''' Thee hee hee...
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cEwEIcon

    Private Shared m_ico As Icon = Nothing
    Private Shared m_bTried As Boolean = False
    Private Shared m_syncroot As New Object()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the current icon for EwE.
    ''' </summary>
    ''' <returns>The current icon for EwE, catered to important events.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function Current() As Icon

        Dim ass As System.Reflection.Assembly = System.Reflection.Assembly.GetAssembly(GetType(cCore))
        Dim an As System.Reflection.AssemblyName = ass.GetName()
        Dim strVersion As String = cAssemblyUtils.GetVersion(an).ToString

        If strVersion.StartsWith("6.6.6") Then
            Return My.Resources.Ecopath666
        End If

        ' Prepare icon
        Select Case cDateUtils.GetNextEvent()
            Case cDateUtils.eNextEvent.Easter
                Return My.Resources.Ecopath3_easter
            Case cDateUtils.eNextEvent.Xmas
                Return My.Resources.Ecopath4_hohoho
            Case cDateUtils.eNextEvent.DagVanDeLiefde
                Return My.Resources.Ecopath6_joepie
            Case cDateUtils.eNextEvent.EwE40
                Return My.Resources.Ecopath40
            Case cDateUtils.eNextEvent.BlackFriday
                Return My.Resources.ecopath_blackfriday
        End Select

#If BETA = 1 Then
        Return My.Resources.Ecopath2_beta
#End If

        SyncLock m_syncroot
            If Not cEwEIcon.m_bTried Then

                Dim tf As String = cFileUtils.MakeTempFile(".ico")
                Try
                    My.Computer.Network.DownloadFile(New Uri("https://ecopath.org/EwE/current.ico"), tf)
                    cEwEIcon.m_ico = New Icon(tf)
                Catch ex As Exception
                    ' OK
                Finally

                End Try
                cEwEIcon.m_bTried = True
            End If
        End SyncLock

        If (cEwEIcon.m_ico IsNot Nothing) Then
            Return cEwEIcon.m_ico
        End If

        ' Fallback
        Return ScientificInterfaceShared.My.Resources.Ecopath0

    End Function

End Class
