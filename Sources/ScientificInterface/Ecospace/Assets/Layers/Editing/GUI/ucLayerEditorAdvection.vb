'==============================================================================
'
' $Log: ucLayerEditorAdvection.vb,v $
' Revision 1.1  2008/11/04 04:40:33  jeroens
' Split into separate files, moved
'
' Revision 1.1  2008/10/14 20:21:25  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Ecospace.Basemap.Layers

    Public Class ucLayerEditorAdvection

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overloads Property Editor() As cLayerEditorAdvection
            Get
                Return DirectCast(MyBase.Editor, cLayerEditorAdvection)
            End Get
            Set(ByVal editor As cLayerEditorAdvection)
                ' Sanity check
                Debug.Assert(TypeOf editor Is cLayerEditorAdvection, "ucLayerEditorAdvection connected to wrong editor class")
                ' Set
                MyBase.Editor = editor
            End Set
        End Property

    End Class

End Namespace

