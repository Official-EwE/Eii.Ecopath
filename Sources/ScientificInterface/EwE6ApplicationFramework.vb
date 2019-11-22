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
Imports System.IO
Imports System.Reflection
Imports EwEUtils.Core

#End Region ' Imports

Module EwE6ApplicationFramework

#Region " Private vars "

    Private m_splash As frmSplash = Nothing
    Private m_main As frmEwE6 = Nothing
    Private m_root As String = ""

    Private m_pluginfolders() As String = New String() {"", ".\plugins"}
    Private m_searchedLSA As New HashSet(Of String)

#End Region ' Private vars 

    Public Sub Main()

        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf OnResolveAssembly

        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        m_root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)

        ' Launch Splash window in separate thread
        ' https://stackoverflow.com/questions/32418695/show-splash-screen-during-loading-the-main-form
        If (My.Settings.ShowSplash) Then
            m_splash = New frmSplash()
            Dim threadSplashStart As New Threading.ThreadStart(AddressOf Splash)
            Dim threadSplash As New Threading.Thread(threadSplashStart)
            threadSplash.SetApartmentState(Threading.ApartmentState.STA)
            threadSplash.Start()
        End If

        ' Define new main UI
        m_main = New frmEwE6()

        ' Load plug-ins
        Dim alDisabledPlugins As ArrayList = My.Settings.DisabledPlugins
        Try
            m_main.PluginManager.LoadPlugins(alDisabledPlugins, m_root, [option]:=SearchOption.TopDirectoryOnly)
            For Each folder As String In m_pluginfolders
                m_main.PluginManager.LoadPlugins(alDisabledPlugins, Path.Combine(m_root, folder))
            Next
        Catch ex As Exception
            ' Ouch!
            cLog.Write(ex)
        End Try

        AddHandler m_main.OnLoadCompleted, AddressOf OnLoadCompleted
        Try
            Application.Run(m_main)
        Catch ex As Exception
            cLog.Write(ex)
        End Try
        RemoveHandler m_main.OnLoadCompleted, AddressOf OnLoadCompleted
        RemoveHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf OnResolveAssembly

        ' Termninate well
        My.Settings.Save()

    End Sub

#Region " Release mode "

    Public ReadOnly Property ReleaseMode As eReleaseMode
        Get
#If BETA = 1 Then
                Return eReleaseMode.Beta
#End If
#If DEBUG Then
            Return eReleaseMode.Dev
#Else
                Return eReleaseMode.Release
#End If
        End Get
    End Property

#End Region ' Release mode

#Region " Localization "

    Private Function OnResolveAssembly(sender As Object, args As ResolveEventArgs) As Assembly

        Dim an As New AssemblyName(args.Name)
        If (Not an.Name.EndsWith(".resources")) Then
            Return Nothing
        End If

        ' Not sure if sat ass is loaded more than once. I presume not, but does not hurt to check
        If m_searchedLSA.Contains(an.Name) Then Return Nothing

        ' Find in possible locations
        Dim fn As String = LocateResourceAssembly(m_root, an)
        Dim i As Integer = 0
        While (i < m_pluginfolders.Count - 1) And String.IsNullOrEmpty(fn)
            fn = LocateResourceAssembly(Path.Combine(m_root, m_pluginfolders(i)), an)
            i += 1
        End While
        m_searchedLSA.Add(an.Name)

        If String.IsNullOrWhiteSpace(fn) Then Return Nothing

        Return Assembly.LoadFile(fn)

    End Function

    Private Function LocateResourceAssembly(folder As String, an As AssemblyName) As String
        Dim fn As String = Path.Combine(folder, an.CultureInfo.Name, an.Name + ".dll")
        If (File.Exists(fn)) Then Return fn
        If (an.CultureInfo.Parent IsNot Nothing) Then
            fn = Path.Combine(folder, an.CultureInfo.Parent.Name, an.Name + ".dll")
            If (File.Exists(fn)) Then
                Return fn
            End If
        End If
        Return ""
    End Function

#End Region ' Localization

#Region " Splash screen "

    Private Sub Splash()
        Application.Run(m_splash)
        CloseSplash()
    End Sub

    Public Sub CloseSplash()
        If (m_splash Is Nothing) Then Return
        If (m_splash.Disposing) Or (m_splash.IsDisposed) Then Return
        m_splash.Invoke(New MethodInvoker(AddressOf m_splash.Close))
        m_splash.Dispose()
        m_splash = Nothing
    End Sub

    Private Sub OnLoadCompleted(sender As Object, args As EventArgs)
        CloseSplash()
    End Sub

#End Region ' Splash screen

End Module
