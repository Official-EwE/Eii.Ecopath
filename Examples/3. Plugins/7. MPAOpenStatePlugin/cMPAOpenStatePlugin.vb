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

Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.Ecospace
Imports EwECore.Plugins.UI

''' ---------------------------------------------------------------------------
''' <summary>
''' A sample plug-in that adds time dynamics to opening and closing an MPA.
''' The MPA will open up at the halfway point during an Ecospace run.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cMPAOpenStatePlugin
    Implements IEcospaceBeginTimestepPostPlugin
    Implements IEcospaceInitRunCompletedPlugin
    Implements IEcospaceRunCompletedPlugin
    Implements IAutoRunPlugin

    ''' <summary>Reference to the core</summary>
    Private m_core As cCore = Nothing
    ''' <summary>Preserved MPA closed state</summary>
    Private m_MPAClosed(12) As Boolean
    ''' <summary>Preserve whether EwE had pending changes.</summary>
    Private m_EwEIsChanged As Boolean = False

    Public Property Enabled As Boolean = False

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Capture a reference to the EwE core when the plug-in initializes. We need
    ''' the core later to find our MPA.
    ''' </summary>
    ''' <param name="core">The EwE core.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Initialize(ByVal core As Object) Implements IPlugin.Initialize
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
        Implements IEcospaceInitRunCompletedPlugin.EcospaceInitRunCompleted

        ' Santiy checks
        If (Me.m_core Is Nothing) Then Return
        If (Me.m_core.nMPAs = 0) Then Return

        Dim MPA = Me.m_core.EcospaceMPAs(1)

        ' Preserve original MPA month layout prior to an Ecospace run
        For i As Integer = 1 To 12
            Me.m_MPAClosed(i) = MPA.IsClosed(i)
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
        Implements IEcospaceBeginTimestepPostPlugin.EcospaceBeginTimeStepPost

        ' Sanity checks
        If (Me.m_core Is Nothing) Then Return
        If (Me.m_core.nMPAs = 0) Then Return
        If (Not Me.Enabled) Then Return

        Dim MPA As cEcospaceMPA = Me.m_core.EcospaceMPAs(1)
        Dim IsHalfWay As Boolean = iTime > Me.m_core.nEcospaceTimeSteps / 2

        ' In this hypothetical example, our first MPA opens up halfway the Ecospace run. Before that date
        ' there are no fishing restrictions for this MPA
        Dim AbsoluteDateForTimeStep As Date = Me.m_core.EcospaceTimestepToAbsoluteTime(iTime)
        MPA.IsClosed(AbsoluteDateForTimeStep.Month) = (IsHalfWay) And (Me.m_MPAClosed(AbsoluteDateForTimeStep.Month) = True)

        ' Extra feature: notify the world of the MPA change
        If (iTime = CInt(Me.m_core.nEcospaceTimeSteps / 2)) Then
            ' This message will appear in the EwE6 status panel
            Dim msg As New cMessage(Me.Name & ": MPA " & MPA.Name & " activated at time step " & iTime & ", " & AbsoluteDateForTimeStep.ToShortDateString,
                                    eMessageType.Any, eCoreComponentType.External, eMessageImportance.Information)
            Me.m_core.Messages.SendMessage(msg)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecospace has finished running. Restore the original layout of the MPA.
    ''' </summary>
    ''' <param name="EcoSpaceDatastructures">- ignored -</param>
    ''' -----------------------------------------------------------------------
    Public Sub EcospaceRunCompleted(EcoSpaceDatastructures As Object) _
        Implements IEcospaceRunCompletedPlugin.EcospaceRunCompleted

        ' Santiy checks
        If (Me.m_core Is Nothing) Then Return
        If (Me.m_core.nMPAs = 0) Then Return
        If (Not Me.Enabled) Then Return

        Dim MPA = Me.m_core.EcospaceMPAs(1)

        ' Restore original MPA closed layout after an Ecospace run
        For i As Integer = 1 To 12
            MPA.IsClosed(i) = Me.m_MPAClosed(i)
        Next

        ' Discard any changes that were caused by changing MPA data
        If Not Me.m_EwEIsChanged Then
            Me.m_core.DiscardChanges()
        End If

    End Sub

    Public Function AutoRunTypes() As eCoreComponentType() Implements IAutoRunPlugin.AutoRunTypes
        Return New eCoreComponentType() {eCoreComponentType.Ecospace}
    End Function

#Region " Generic plug-in bits "

    Public ReadOnly Property Author() As String Implements IPlugin.Author
        Get
            Return "Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact() As String Implements IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements IPlugin.Description
        Get
            Return "Plug-in that opens and closes MPAs"
        End Get
    End Property

    Public ReadOnly Property Name() As String Implements IPlugin.Name
        Get
            Return "MPAOpenStatePlugin"
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return "MPA open state plug-in"
        End Get
    End Property

    Public Property AutoRun(type As eCoreComponentType) As Boolean Implements IAutoRunPlugin.AutoRun
        Get
            Return Me.Enabled
        End Get
        Set(value As Boolean)
            Me.Enabled = value
        End Set
    End Property

#End Region ' Generic plug-in bits

End Class
