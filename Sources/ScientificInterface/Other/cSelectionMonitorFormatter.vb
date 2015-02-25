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
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Class for providing a textual description of the content of the 
''' <see cref="cSelectionMonitor"/>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cSelectionMonitorFormatter
    Implements ITypeFormatter

    Public Function GetDescriptor(ByVal value As Object, _
                                  Optional ByVal descriptor As eDescriptorTypes = eDescriptorTypes.Name) As String _
                                  Implements ITypeFormatter.GetDescriptor

        If (value Is Nothing) Then Return ""

        Debug.Assert(value.GetType.IsAssignableFrom(Me.GetDescribedType()))

        Dim mon As cSelectionMonitor = DirectCast(value, cSelectionMonitor)
        Dim strName As String = My.Resources.SELECTION_NONE
        Dim strDescription As String = ""
        Dim props() As cProperty = mon.Selection
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
                        Dim fmt As New cCoreInterfaceFormatter()
                        Dim strVarN As String = vd.GetDescriptor(var, eDescriptorTypes.Name)
                        Dim strVarD As String = vd.GetDescriptor(var, eDescriptorTypes.Description)

                        strDescription = strVarD

                        ' Format message
                        If Not Object.ReferenceEquals(props(0).SourceSec, Nothing) Then
                            strName = String.Format(My.Resources.SELECTION_INDEXEDVAR, _
                                                         fmt.GetDescriptor(props(0).Source), _
                                                         strVarN, _
                                                         fmt.GetDescriptor(props(0).SourceSec))
                        Else
                            strName = String.Format(SharedResources.GENERIC_LABEL_DETAILED, _
                                                         fmt.GetDescriptor(props(0).Source), _
                                                         strVarN)
                        End If
                    Else
                        strName = My.Resources.SELECTION_DERIVED
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
                        strName = My.Resources.SELECTION_MULTIPLE
                    Else
                        strName = String.Format(My.Resources.SELECTION_SINGLEVAR, My.Resources.SELECTION_MULTIPLE, vd.GetDescriptor(var))
                    End If
            End Select

        End If

        Select Case descriptor
            Case eDescriptorTypes.Abbreviation, eDescriptorTypes.Symbol, eDescriptorTypes.Name
                Return strName
            Case eDescriptorTypes.Description
                Return strDescription
        End Select

        Return ""

    End Function

    Public Function GetDescribedType() As System.Type _
        Implements ITypeFormatter.GetDescribedType
        Return GetType(cSelectionMonitor)
    End Function

End Class

