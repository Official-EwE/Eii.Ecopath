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

#Region " Imports "

Option Strict On
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Monitor class for currently selected data.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cSelectionMonitor

    Private m_cmdSelect As cPropertySelectionCommand = Nothing

    Public Sub New()
        ' NOP
    End Sub

    Public Sub Attach(uic As cUIContext)

        ' Sanity checks
        Debug.Assert(Me.m_cmdSelect Is Nothing)
        Debug.Assert(uic IsNot Nothing)

        ' Start monitoring
        Me.m_cmdSelect = DirectCast(uic.CommandHandler.GetCommand(cPropertySelectionCommand.COMMAND_NAME), cPropertySelectionCommand)
        AddHandler Me.m_cmdSelect.OnPostInvoke, AddressOf HandleSelectionChanged

    End Sub

    Public Sub Detach()

        ' Sanity checks
        Debug.Assert(Me.m_cmdSelect IsNot Nothing)

        ' Stop monitoring
        RemoveHandler Me.m_cmdSelect.OnPostInvoke, AddressOf HandleSelectionChanged
        Me.m_cmdSelect = Nothing

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns an array of currently selected properties.
    ''' </summary>
    ''' <returns>An array of currently selected <see cref="cProperty">properties</see>.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Selection() As cProperty()
        If (Me.m_cmdSelect IsNot Nothing) Then
            Return Me.m_cmdSelect.Selection
        End If
        Return New cProperty() {}
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Selection change notification
    ''' </summary>
    ''' <param name="sender"></param>
    ''' -----------------------------------------------------------------------
    Event OnSelectionChanged(sender As cSelectionMonitor)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a string representation of the current selection.
    ''' </summary>
    ''' <returns>A string representation of the <see cref="Selection">current selection</see>.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ToString() As String

        Dim strSelection As String = My.Resources.SELECTION_NONE

        If (Me.m_cmdSelect Is Nothing) Then Return strSelection

        Dim props() As cProperty = Me.Selection
        Dim vd As New cVarnameTypeFormatter()

        If (props IsNot Nothing) Then
            Select Case props.Length

                Case 0
                    ' NOP

                Case 1
                    ' Get selection text
                    If (Not Object.ReferenceEquals(props(0).Source, Nothing)) Then

                        ' Get variable descriptor
                        Dim var As eVarNameFlags = props(0).VarName
                        ' Format message
                        If Not Object.ReferenceEquals(props(0).SourceSec, Nothing) Then
                            strSelection = String.Format(My.Resources.SELECTION_INDEXEDVAR, _
                                                         props(0).Source.Name, _
                                                         vd.GetDescriptor(var, eDescriptorTypes.Name), _
                                                         props(0).SourceSec.Name)
                        Else
                            strSelection = String.Format(SharedResources.GENERIC_LABEL_DETAILED, _
                                                         props(0).Source.Name, _
                                                         vd.GetDescriptor(var, eDescriptorTypes.Description))
                        End If
                    Else
                        strSelection = My.Resources.SELECTION_DERIVED
                    End If

                Case Else
                    Dim var As eVarNameFlags = eVarNameFlags.NotSet
                    Dim bMixed As Boolean = False
                    For Each prop As cProperty In props
                        If (var = eVarNameFlags.NotSet) Then
                            var = prop.VarName
                        Else
                            bMixed = bMixed Or (var <> prop.VarName)
                        End If
                    Next
                    If bMixed Then
                        strSelection = My.Resources.SELECTION_MULTIPLE
                    Else
                        strSelection = String.Format(My.Resources.SELECTION_SINGLEVAR, My.Resources.SELECTION_MULTIPLE, vd.GetDescriptor(var))
                    End If
            End Select

            If Not String.IsNullOrWhiteSpace(Me.m_cmdSelect.Status) Then
                strSelection = String.Format(SharedResources.GENERIC_LABEL_DOUBLE, strSelection, Me.m_cmdSelect.Status)
            End If

        End If
        Return strSelection
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="OnSelectionChanged">Selection change event</see> dispatch.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub HandleSelectionChanged(cmd As EwEUtils.Commands.cCommand)
        Try
            RaiseEvent OnSelectionChanged(Me)
        Catch ex As Exception

        End Try
    End Sub

End Class
