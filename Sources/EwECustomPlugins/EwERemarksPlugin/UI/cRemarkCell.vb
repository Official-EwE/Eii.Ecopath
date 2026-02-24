' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Properties
Imports SourceGrid2.VisualModels

''' ---------------------------------------------------------------------------
''' <summary>
''' Cell that provides edit capabilities of the remark text of a <see cref="cProperty"/>, 
''' and refreshes its content when the remark is changed externally.
''' </summary>
''' ---------------------------------------------------------------------------
Friend Class cRemarkCell
    Inherits cEwECell

#Region " Private vars "

    Private Shared m_vmRemarks As New ScientificInterfaceShared.Controls.EwEGrid.cEwECellVisualizer(Drawing.ContentAlignment.MiddleLeft)

    Private m_prop As cProperty
    Private m_bInUpdate As Boolean = False

#End Region ' Private vars

#Region " Construction and destruction "

    Public Sub New(prop As cProperty)
        MyBase.New(prop.GetRemark(), GetType(String))

        Me.VisualModel = m_vmRemarks
        Me.m_prop = prop
        AddHandler Me.m_prop.PropertyChanged, AddressOf Me.OnPropertyChanged

    End Sub

    Public Overrides Sub Dispose()

        If (Me.m_prop IsNot Nothing) Then
            RemoveHandler Me.m_prop.PropertyChanged, AddressOf Me.OnPropertyChanged
            Me.m_prop = Nothing
        End If
        MyBase.Dispose()

    End Sub

#End Region ' Construction and destruction

#Region " Monitoring "

    Private Sub OnPropertyChanged(prop As cProperty, ct As cProperty.eChangeFlags)
        If Me.m_bInUpdate Then Return
        ' Is a remark change?
        If ((ct And cProperty.eChangeFlags.Remarks) > 0) Then
            ' #Yes: update remark
            Me.Value = prop.GetRemark()
        End If
    End Sub

#End Region ' Monitoring

#Region " Overrides "

    Public Overrides Sub OnEditEnded(e As SourceGrid2.PositionCancelEventArgs)
        MyBase.OnEditEnded(e)
        If e.Cancel = False Then
            Me.m_bInUpdate = True
            Me.m_prop.SetRemark(CStr(Me.Value))
            Me.m_bInUpdate = False
        End If
    End Sub

    ''' <summary>
    ''' For quick editor access
    ''' </summary>
    ''' <param name="p_Position"></param>
    ''' <param name="p_Value"></param>
    Public Overrides Sub SetValue(p_Position As SourceGrid2.Position, p_Value As Object)
        MyBase.SetValue(p_Position, p_Value)
        Me.m_bInUpdate = True
        Me.m_prop.SetRemark(CStr(Me.Value))
        Me.m_bInUpdate = False
    End Sub

#End Region ' Overrides

End Class
