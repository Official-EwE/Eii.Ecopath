Option Strict On
Imports System.ComponentModel

Namespace Import

    Public Class ucImportHeader

        <EditorBrowsable(EditorBrowsableState.Always), _
         Browsable(True), _
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), _
         Bindable(True)> _
        Public Overrides Property Text() As String
            Get
                Return Me.m_lblHeader.Text
            End Get
            Set(ByVal value As String)
                Me.m_lblHeader.Text = value
            End Set
        End Property

        <EditorBrowsable(EditorBrowsableState.Always), _
         Browsable(True), _
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), _
         Bindable(True), _
         Category("Appearance")> _
        Public Property SubText() As String
            Get
                Return Me.m_lblSubheader.Text
            End Get
            Set(ByVal value As String)
                Me.m_lblSubheader.Text = value
            End Set
        End Property

    End Class

End Namespace