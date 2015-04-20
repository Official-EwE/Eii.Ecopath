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
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Public Class cPluginPoint
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.IEcopathRunCompletedPlugin
    Implements EwEPlugin.IEcosimRunInitializedPlugin
    Implements EwEPlugin.IEcosimEndTimestepPlugin
    Implements EwEPlugin.IDisposedPlugin

#Region " Private vars "

    ''' <summary>The core to use.</summary>
    Private m_core As cCore = Nothing

    ''' <summary>The UI context to use.</summary>
    Private m_uic As cUIContext = Nothing

    ''' <summary>Ecopath data for detecting crashes.</summary>
    Private m_epdata As cEcopathDataStructures = Nothing

    ''' <summary>Stocks chrash at 1% of the original biomass.</summary>
    Private m_threshold As Single = 0.01

    ''' <summary>Pfrt</summary>
    Private m_berry As Media.SoundPlayer = Nothing

    ''' <summary>Thar she blows!</summary>
    Private m_bBlown As Boolean = False

#End Region ' Private vars

#Region " Generic bits "

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.Initialize"/>
    ''' <remarks>
    ''' Implemented to grab a reference to the EwE <see cref="cCore">core</see>.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub Initialize(core As Object) _
        Implements EwEPlugin.IPlugin.Initialize
        Try
            Me.m_core = DirectCast(core, cCore)
            Me.m_berry = New Media.SoundPlayer(My.Resources.berry)
            Me.m_berry.Load()
        Catch ex As Exception
            ' Umph
        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.Initialize"/>
    ''' <remarks>
    ''' Implemented to provide an author name.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "Anonymous"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.Contact"/>
    ''' <remarks>
    ''' Implemented to report contact information for this plug-in.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return "Please, no"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.Initialize"/>
    ''' <remarks>
    ''' Implemented to report description information for this plug-in.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Description As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return "Plug-in for EwE6 that blows a raspberry whenever the first group crashes in Ecosim. Pfrt."
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.Initialize"/>
    ''' <remarks>
    ''' Implemented to grab a reference to the EwE <see cref="cCore">core</see>.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Name As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            Return "EwERaspberryPlugin"
        End Get
    End Property

#End Region ' Generic bits

#Region " UI context plug-in implementation "

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IUIContextPlugin.UIContext"/>
    ''' <remarks>
    ''' Implemented to grab a reference to the EwE <see cref="cUIContext">UI context</see>.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub UIContext(uic As Object) Implements _
        EwEPlugin.IUIContextPlugin.UIContext
        Try
            Me.m_uic = DirectCast(uic, cUIContext)
        Catch ex As Exception
            ' Raah
        End Try
    End Sub

#End Region ' UI context plug-in implementation

#Region " EwE core integration "

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IEcopathRunCompletedPlugin.EcopathRunCompleted"/>
    ''' <remarks>
    ''' Implemented to grab a reference to the Ecopath data structures. We need
    ''' this to detect Ecosim biomass crashes relative to the original Ecopath biomass.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub EcopathRunCompleted(ByRef EcopathDataStructures As Object) _
        Implements EwEPlugin.IEcopathRunCompletedPlugin.EcopathRunCompleted
        Try
            Me.m_epdata = DirectCast(EcopathDataStructures, cEcopathDataStructures)
        Catch ex As Exception
            ' Aargh
        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IEcosimRunInitializedPlugin.EcosimRunInitialized"/>
    ''' <remarks>
    ''' Implemented to reset the raspberry flag to pfrt new crashes. Yippee.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub EcosimRunInitialized(EcosimDatastructures As Object) _
        Implements EwEPlugin.IEcosimRunInitializedPlugin.EcosimRunInitialized
        Try
            Me.Reset()
        Catch ex As Exception
            ' Ouch
        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IEcosimEndTimestepPlugin.EcosimEndTimeStep"/>
    ''' <remarks>
    ''' Implemented to check if a biomass has crashed below threshold level.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub EcosimEndTimeStep(ByRef Bt() As Single, simdata As Object, t As Integer, simresults As Object) _
        Implements EwEPlugin.IEcosimEndTimestepPlugin.EcosimEndTimeStep

        Try
            For i As Integer = 1 To Me.m_epdata.NumGroups
                If ((Me.m_epdata.B(i) * Me.m_threshold) > Bt(i)) Then
                    Me.Blow(i, 100 * Bt(i) / Me.m_epdata.B(i))
                End If
            Next
        Catch ex As Exception
            ' Oof
        End Try
    End Sub

#End Region ' EwE core integration

#Region " Disposal "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Implemented to clean up bits that were allocated.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub Dispose() _
        Implements EwEPlugin.IDisposedPlugin.Dispose
        Me.m_berry.Stop()
        Me.m_berry.Dispose()
        Me.m_berry = Nothing
    End Sub

#End Region ' Disposal

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Reset the pfrt flag.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub Reset()
        Me.m_bBlown = False
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Pfrt.
    ''' </summary>
    ''' <param name="iGroup">The group that crashed.</param>
    ''' <param name="sPerc">Percentage of biomass remaining.</param>
    ''' -----------------------------------------------------------------------
    Private Sub Blow(iGroup As Integer, sPerc As Single)

        If (Not Me.m_bBlown) And (Me.m_berry IsNot Nothing) Then

            Me.m_berry.Play()
            Me.m_bBlown = True

            Dim fmt As New cCoreInterfaceFormatter()
            Dim strMessge As String = String.Format(My.Resources.PROMPT_CRASH, _
                                                    fmt.GetDescriptor(Me.m_core.EcoPathGroupInputs(iGroup)), _
                                                    Me.m_uic.StyleGuide.FormatNumber(sPerc))
            Dim msg As New cMessage(strMessge, eMessageType.Any, eCoreComponentType.External, eMessageImportance.Warning)
            Me.m_core.Messages.SendMessage(msg)

        End If
    End Sub

#End Region ' Internals

End Class
