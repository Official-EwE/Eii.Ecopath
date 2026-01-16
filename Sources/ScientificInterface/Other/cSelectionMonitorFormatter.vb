' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports EwECore.Style
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources



Public Class cSelectionMonitorFormatter
    Implements ITypeFormatter

    Private m_core As cCore = Nothing

    Public Sub New(core As cCore)
        Me.m_core = core
    End Sub

    Public Function GetDescribedType() As Type Implements ITypeFormatter.GetDescribedType
        Return GetType(cSelectionMonitor)
    End Function

    Public Overloads Function ToString(value As Object, Optional descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String Implements ITypeFormatter.ToString

        Dim strSelection As String = ""

        If (value Is Nothing) Then Return strSelection
        If (Not TypeOf (value) Is cSelectionMonitor) Then Return ""

        Dim mon As cSelectionMonitor = DirectCast(value, cSelectionMonitor)

        Dim props() As cProperty = mon.Selection
        Dim vd As New cVarnameTypeFormatter()
        Dim units As New cUnits(Me.m_core)

        If (props IsNot Nothing) Then
            Select Case props.Length

                Case 0
                    ' NOP

                Case 1
                    ' Get selection text
                    If (props(0).Source IsNot Nothing) Then

                        ' Get variable descriptor
                        Dim var As eVarNameFlags = props(0).VarName
                        Dim fmt As New cCoreInterfaceFormatter()

                        ' Format message
                        If props(0).SourceSec IsNot Nothing Then
                            strSelection = String.Format(My.Resources.SELECTION_INDEXEDVAR,
                                                         fmt.ToString(props(0).Source),
                                                         vd.ToString(var, eDescriptorTypes.Name),
                                                         fmt.ToString(props(0).SourceSec))
                        Else
                            strSelection = String.Format(SharedResources.GENERIC_LABEL_DETAILED,
                                                         fmt.ToString(props(0).Source),
                                                         vd.ToString(var, eDescriptorTypes.Description))
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
                        strSelection = String.Format(My.Resources.SELECTION_SINGLEVAR, My.Resources.SELECTION_MULTIPLE, vd.ToString(var))
                    End If
            End Select

            'If (Not bMinimal) And (Not String.IsNullOrWhiteSpace(Me.m_cmdSelect.Status)) Then
            '    strSelection = String.Format(SharedResources.GENERIC_LABEL_DOUBLE, strSelection, Me.m_cmdSelect.Status)
            'End If

        End If
        Return strSelection

    End Function

End Class
