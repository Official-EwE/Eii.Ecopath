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

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

''' <summary>
''' This object represents the iStanza Group (index of stanza group) and Egg Production Forcing Shape index that is used by this Stanza group
''' </summary>
''' <remarks></remarks>
Public Class cGroupShapePair

    Private m_iStanza As Integer
    ''' <summary>Index of a shape in the cEggProductionManager.Item() list</summary>
    Private m_iManager As Integer
    Private m_shape As cForcingFunction
    Private m_manager As cEggProductionShapeManager
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cGroupShapePair)()


    Public Function Clear() As Boolean
        Me.ShapeID = cCore.NULL_VALUE
        Me.m_shape = Nothing
    End Function

    ''' <summary>
    ''' Index of this Shape in the cEggProductionManager.Item() lists
    ''' </summary>
    ''' <remarks>The shape to use for this pair from the cEggProductionManager. 
    ''' <example>
    ''' 'get the cGroupShapePair for the first stanza group from the EggProdManager.GroupShapeList
    ''' 'this cGroupShapePair will have an iStanzaGroup=0
    ''' Dim pair As cGroupShapePair = EggProdManager.GroupShapeList.Item(0)
    ''' 'make this cGroupShapePair use the first shape in the EggProdManager
    ''' pair.ShapeMangerIndex = 0
    ''' </example>
    '''</remarks>
    Public Property ShapeID() As Integer
        Get
            Return Me.m_iManager
        End Get

        Set(value As Integer)
            If (value < Me.m_manager.Count And value >= 0) Or (value = cCore.NULL_VALUE) Then
                'only set the value if it passed the lame validation
                Me.m_iManager = value

                If value >= 0 Then
                    Me.m_shape = Me.m_manager.Item(Me.m_iManager)
                Else
                    Me.m_shape = Nothing
                End If
                Me.Update()
            Else
                Debug.Assert(False)
                m_logger.LogError("Index out of bounds on egg shapes")
            End If
        End Set

    End Property

    ''' <summary>
    ''' A zero based index to the Stanzas. This is the same as is used by cCore.StanzaGroups.Item(iStanza) list
    ''' </summary>
    Public ReadOnly Property iStanzaGroup() As Integer
        Get
            Return Me.m_iStanza
        End Get
    End Property

    ''' <summary>
    ''' Index of the shape in the underlying core data 
    ''' </summary>
    ''' <remarks>
    ''' This is a friend because only the manager should care what the underlying core shape index is.
    ''' </remarks>
    Friend ReadOnly Property iShape() As Integer
        Get
            If Me.m_shape IsNot Nothing Then
                Return Me.m_shape.Index
            Else
                Return 0
            End If
        End Get
    End Property

    ''' <summary>
    ''' Index used by the core to update data
    ''' </summary>
    ''' <remarks>Stanzas are stored in a zeor base list for the interface. This is the one based index used by the core.</remarks>
    Friend ReadOnly Property iCoreStanzaIndex() As Integer
        Get
            Return Me.m_iStanza + 1
        End Get
    End Property

    Sub New(ByRef theManager As cEggProductionShapeManager, ByRef Shape As cForcingFunction, StanzaIndex As Integer)
        Me.m_manager = theManager

        'Zero based public stanza index for stanza list 
        'this is the same index as in the cCore.StanzaGroups.Item(iStanza) 
        Me.m_iStanza = StanzaIndex - 1

        If Shape Is Nothing Then
            Me.m_shape = Nothing
            Me.m_iManager = cCore.NULL_VALUE
        Else
            Me.m_shape = Shape
            Me.m_iManager = Shape.ID
        End If
    End Sub

    Friend Function Update() As Boolean
        Try

            'tell the manager that this pair has changed it data
            'this will validate the data

            If Me.m_shape IsNot Nothing Then
                'index of the shape in the managers list stored in ID during construction of the shape
                Me.m_iManager = Me.m_shape.ID
            Else
                Me.m_iManager = cCore.NULL_VALUE
            End If

            Return Me.m_manager.OnChanged(Me)

        Catch ex As Exception
            m_logger.LogError(ex, "Error updating GroupShapePair")
            Return False
        End Try

    End Function

End Class
