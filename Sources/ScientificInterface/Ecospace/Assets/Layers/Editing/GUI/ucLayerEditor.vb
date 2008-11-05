'==============================================================================
'
' $Log: ucLayerEditor.vb,v $
' Revision 1.2  2008/11/05 01:13:57  jeroens
' Update! Update!
'
' Revision 1.1  2008/11/04 04:40:33  jeroens
' Split into separate files, moved
'
' Revision 1.2  2008/10/15 23:57:19  jeroens
' Added GUI sync options
'
' Revision 1.1  2008/10/15 17:03:47  jeroens
' Initial version
'
'==============================================================================

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

#Region " Private vars "

        ''' <summary>Underlying editor.</summary>
        Private m_editor As cLayerEditor = Nothing

#End Region ' Private vars

#Region " Construction / destruction "

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

#End Region ' Construction / destruction

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridable method to inform the editor GUI that the user has 
        ''' started editing the layer.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub StartEdit()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridable method to inform the editor GUI that the user has 
        ''' finished editing the layer.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub EndEdit()
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
        ''' Update the controls and caption of the editor.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub UpdateControls()
            If Me.Editor.Layer IsNot Nothing Then
                Me.m_lbCaption.Text = Me.Editor.Layer.Name
            Else
                Me.m_lbCaption.Text = ""
            End If
        End Sub

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
            Set(ByVal editor As cLayerEditor)
                Me.m_editor = editor
                Me.UpdateControls()
            End Set
        End Property

#End Region ' Internals

    End Class

End Namespace

