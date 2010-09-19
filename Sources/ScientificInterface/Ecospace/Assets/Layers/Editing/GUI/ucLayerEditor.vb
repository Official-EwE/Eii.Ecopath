#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for implementing a user interface for maintaining values in
    ''' an underlying <see cref="cLayerEditor">layer editor</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucLayerEditor
        Implements IUIElement
        Implements ILayerEditorGUI

#Region " Private vars "

        ''' <summary>Underlying editor.</summary>
        Private m_editor As cLayerEditor = Nothing
        ''' <summary>Underlying UI context.</summary>
        Private m_uic As cUIContext = Nothing

#End Region ' Private vars

#Region " Construction / destruction "

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

#End Region ' Construction / destruction

#Region " Public interfaces "

        Public Sub Attach(ByVal uic As cUIContext, ByVal editor As cLayerEditor)
            Me.UIContext = uic
            Me.Editor = editor
        End Sub

        Public Sub Detach()
            Me.UIContext = Nothing
            Me.Editor = Nothing
        End Sub

        Public Overridable Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Protected Set(ByVal value As cUIContext)
                Me.m_uic = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cLayerEditor">layer editor</see> that this
        ''' GUI operates on.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property Editor() As cLayerEditor
            Get
                Return Me.m_editor
            End Get
            Protected Set(ByVal editor As cLayerEditor)
                Me.m_editor = editor
                Me.UpdateContent(editor)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ILayerEditorGUI.Initialize"/>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Initialize(ByVal editor As cLayerEditor) _
            Implements ILayerEditorGUI.Initialize
            ' NOP
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ILayerEditorGUI.StartEdit"/>
        ''' -------------------------------------------------------------------
        Public Overridable Sub StartEdit(ByVal editor As cLayerEditor) _
            Implements ILayerEditorGUI.StartEdit
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ILayerEditorGUI.EndEdit"/>
        ''' -------------------------------------------------------------------
        Public Overridable Sub EndEdit(ByVal editor As cLayerEditor) _
            Implements ILayerEditorGUI.EndEdit
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ILayerEditorGUI.UpdateContent"/>
        ''' -------------------------------------------------------------------
        Public Overridable Sub UpdateContent(ByVal editor As cLayerEditor) _
            Implements ILayerEditorGUI.UpdateContent

            Dim strLabel As String = ""
            Dim img As Image = My.Resources.ProtectFormHS

            If (Me.IsAttached = True) Then
                ' Get label text (could use diplay text?)
                strLabel = Me.Editor.Layer.Name
                ' Get layer image
                If Me.Editor.IsReadOnly Then
                    img = My.Resources.ProtectFormHS
                Else
                    If Me.Editor.IsEditable Then
                        img = My.Resources.Editable
                    Else
                        img = My.Resources.NotEditable
                    End If
                End If
            End If

            Me.m_lbCaption.Text = strLabel
            Me.m_lbImage.Image = img

        End Sub

#End Region ' Public interfaces

#Region " Public events "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Public event to notify the outside world that the layer editor
        ''' has undergone a significant change.
        ''' </summary>
        ''' <param name="ucEditor">The editor that changed, e.g. moi.</param>
        ''' -------------------------------------------------------------------
        Public Event OnChanged(ByVal ucEditor As ucLayerEditor)

#End Region ' Public events

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Raise the event to notify the outside world that settings in this 
        ''' GUI have changed.
        ''' </summary>
        ''' <remarks>
        ''' The GUI may send change events when changes in its content may need
        ''' to be reflected in the basemap while the underlying GUI layer is 
        ''' not affected. Hence, the layer change event cannot be used for this
        ''' purpose.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Protected Sub RaiseChangedEvent()
            RaiseEvent OnChanged(Me)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the editor is correctly attached to a layer and
        ''' a UI context.
        ''' </summary>
        ''' <returns>True if connected.</returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function IsAttached() As Boolean
            If (Me.Editor Is Nothing) Then Return False
            If (Me.Editor.Layer Is Nothing) Then Return False
            Return (Me.UIContext IsNot Nothing)
        End Function

#End Region ' Internals

    End Class

End Namespace

