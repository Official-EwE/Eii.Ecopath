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
Imports System.Media
Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.Ecopath
Imports EwECore.Plugins.Ecosim
Imports EwECore.Plugins.Ecospace
Imports EwECore.Plugins.MonteCarlo
Imports EwECore.Plugins.UI


Public Class cEwEJobDonePluginPoint
    Implements IAutoRunPlugin
    Implements IEcopathRunCompletedPostPlugin
    Implements IEcosimRunCompletedPostPlugin
    Implements IEcospaceRunCompletedPlugin
    Implements IMonteCarloPlugin

    Private m_bAutorun() As Boolean
    Private m_core As cCore = Nothing

    Public Sub New()
        ' Reserve space to remember autorun settings
        ReDim m_bAutorun([Enum].GetValues(GetType(eCoreComponentType)).Length)
    End Sub

    Public Property AutoRun(type As eCoreComponentType) As Boolean Implements IAutoRunPlugin.AutoRun
        Get
            Return Me.m_bAutorun(type)
        End Get
        Set(value As Boolean)
            Me.m_bAutorun(type) = value
        End Set
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "EwEJobDonePlugin"
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return My.Resources.DISPLAYNAME
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "Plays a notification when an EwE model has completed running"
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "EwE dev team"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "Please don't"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
    End Sub

    Public Function AutoRunTypes() As eCoreComponentType() Implements IAutoRunPlugin.AutoRunTypes
        ' Inform the world which run types we respond to
        Return New eCoreComponentType() {eCoreComponentType.Ecopath, eCoreComponentType.Ecosim, eCoreComponentType.EcoSimMonteCarlo, eCoreComponentType.Ecospace}
    End Function

    Public Sub EcopathRunCompletedPost(ByRef EcopathDataStructures As Object) Implements IEcopathRunCompletedPostPlugin.EcopathRunCompletedPost
        If Me.AutoRun(eCoreComponentType.Ecopath) Then
            Me.PlayNotification()
        End If
    End Sub

    Public Sub EcosimRunCompletedPost(EcosimDatastructures As Object) Implements IEcosimRunCompletedPostPlugin.EcosimRunCompletedPost
        If Me.AutoRun(eCoreComponentType.Ecosim) Then
            Me.PlayNotification()
        End If
    End Sub

    Public Sub EcospaceRunCompleted(EcoSpaceDatastructures As Object) Implements IEcospaceRunCompletedPlugin.EcospaceRunCompleted
        If Me.AutoRun(eCoreComponentType.Ecospace) Then
            Me.PlayNotification()
        End If
    End Sub

#Region " Monte Carlo integration "

    Public Sub MontCarloInitialized(MonteCarloAsObject As Object) Implements IMonteCarloPlugin.MontCarloInitialized
        ' Ignore this plug-in point
    End Sub

    Public Sub MonteCarloRunInitialized() Implements IMonteCarloPlugin.MonteCarloRunInitialized
        ' Ignore this plug-in point
    End Sub

    Public Sub MonteCarloBalancedEcopathModel(TrialNumber As Integer, nIterations As Integer) Implements IMonteCarloPlugin.MonteCarloBalancedEcopathModel
        ' Ignore this plug-in point
    End Sub

    Public Sub MonteCarloEcosimRunCompleted() Implements IMonteCarloPlugin.MonteCarloEcosimRunCompleted
        ' Ignore this plug-in point
    End Sub

    Public Sub MonteCarloRunCompleted() Implements IMonteCarloPlugin.MonteCarloRunCompleted
        If Me.AutoRun(eCoreComponentType.EcoSimMonteCarlo) Then
            Me.PlayNotification()
        End If
    End Sub

#End Region ' Monte Carlo integration

#Region " Notification "

    Private Sub PlayNotification()
        SystemSounds.Asterisk.Play()
    End Sub

#End Region ' Notification

End Class
