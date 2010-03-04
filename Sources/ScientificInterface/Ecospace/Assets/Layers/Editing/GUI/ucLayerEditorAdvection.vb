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

