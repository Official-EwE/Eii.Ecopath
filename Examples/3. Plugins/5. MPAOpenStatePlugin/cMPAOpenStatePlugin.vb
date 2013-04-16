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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Option Strict On
Imports EwEPlugin
Imports EwECore

''' ---------------------------------------------------------------------------
''' <summary>
''' A sample plug-in that adds time dynamics to opening and closing an MPA
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cMPAOpenStatePlugin
    Implements IEcospaceBeginTimestepPostPlugin
    Implements IEcospaceInitRunCompletedPlugin
    Implements IEcospaceRunCompletedPlugin

    ''' <summary>Reference to the core</summary>
    Private m_core As cCore = Nothing
    ''' <summary>Preserved MPA state</summary>
    Private m_MPAMonths(12) As Boolean
    ''' <summary>Preserve whether EwE had pending changes.</summary>
    Private m_EwEIsChanged As Boolean = False

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Capture a reference to the EwE core when the plug-in initializes. We need
    ''' the core later to find our MPA.
    ''' </summary>
    ''' <param name="core">The EwE core.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize
        Try
            Me.m_core = DirectCast(core, cCore)
        Catch ex As Exception
            Me.m_core = Nothing
        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace is prepared to run, and is about to start executing its time steps.
    ''' In this plug-in point we want to preserve the original open/closed state
    ''' of 'our' MPA so we can restore this state after the Ecospace run.
    ''' </summary>
    ''' <param name="EcospaceDatastructures">- ignored -</param>
    ''' -----------------------------------------------------------------------
    Public Sub EcospaceInitRunCompleted(EcospaceDatastructures As Object) _
        Implements EwEPlugin.IEcospaceInitRunCompletedPlugin.EcospaceInitRunCompleted

        ' Santiy checks
        If (Me.m_core Is Nothing) Then Return
        If (Me.m_core.nMPAs = 0) Then Return

        Dim MPA = Me.m_core.EcospaceMPAs(1)

        ' Preserve original MPA month layout prior to an Ecospace run
        For i As Integer = 1 To 12
            Me.m_MPAMonths(i) = MPA.MPAMonth(i)
        Next

        ' Preserve whether EwE is in need of saving data changes
        Me.m_EwEIsChanged = Me.m_core.HasChanges

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace is about to compute a time step. Here we have to opportunity to
    ''' change the months an MPA is closed to fishing.
    ''' </summary>
    ''' <param name="EcospaceDatastructures">- ignored -</param>
    ''' <param name="iTime">The time step that is currently being executed.</param>
    ''' -----------------------------------------------------------------------
    Public Sub EcospaceBeginTimeStepPost(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer) _
        Implements EwEPlugin.IEcospaceBeginTimestepPostPlugin.EcospaceBeginTimeStepPost

        ' Sanity checks
        If (Me.m_core Is Nothing) Then Return
        If (Me.m_core.nMPAs = 0) Then Return

        Dim MPA As cEcospaceMPA = Me.m_core.EcospaceMPAs(1)

        ' In this hypothetical example, our first MPA opened up in 1979; there were no fishing restrictions before 1979
        ' Thus, before 1979 fishing is allowed, and in 1979 or later fishing is only allowed when the MPA is open to fishing

        Dim TimeStepDate As Date = Me.m_core.EcospaceTimestepToAbsoluteTime(iTime)
        MPA.MPAMonth(TimeStepDate.Month) = (TimeStepDate.Year < 1979) Or (Me.m_MPAMonths(TimeStepDate.Month) = True)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace has finished running. Restore the original layout of the MPA.
    ''' </summary>
    ''' <param name="EcoSpaceDatastructures">- ignored -</param>
    ''' -----------------------------------------------------------------------
    Public Sub EcospaceRunCompleted(EcoSpaceDatastructures As Object) _
        Implements EwEPlugin.IEcospaceRunCompletedPlugin.EcospaceRunCompleted

        ' Santiy checks
        If (Me.m_core Is Nothing) Then Return
        If (Me.m_core.nMPAs = 0) Then Return

        Dim MPA = Me.m_core.EcospaceMPAs(1)

        ' Restore original MPA month layout after an Ecospace run
        For i As Integer = 1 To 12
            MPA.MPAMonth(i) = Me.m_MPAMonths(i)
        Next

        ' Discard any changes that were caused by changing MPA data
        If Not Me.m_EwEIsChanged Then
            Me.m_core.DiscardChanges()
        End If

    End Sub

#Region " Generic plug-in bits "

    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Plug-in that opens and closes MPAs"
        End Get
    End Property

    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "MPAOpenStatePlugin"
        End Get
    End Property

#End Region ' Generic plug-in bits

End Class
