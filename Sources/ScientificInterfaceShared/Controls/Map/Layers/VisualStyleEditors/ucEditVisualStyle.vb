#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Auxiliary

#End Region ' Imports

Namespace Controls

    Public Class ucEditVisualStyle
        Inherits UserControl

#Region " Factory "

        Public Shared Function GetEditor(ByVal uic As cUIContext, _
                                         ByVal vs As cVisualStyle, _
                                         ByVal style As cVisualStyle.eVisualStyleTypes) As ucEditVisualStyle
            ' Sanity checks
            Debug.Assert(vs IsNot Nothing)

            If ((style And cVisualStyle.eVisualStyleTypes.Image) = cVisualStyle.eVisualStyleTypes.Image) Then
                Return New ucEditImage(uic, vs, style)
            End If

            If ((style And cVisualStyle.eVisualStyleTypes.Font) = cVisualStyle.eVisualStyleTypes.Image) Then
                Return New ucEditFont(uic, vs, style)
            End If

            Return New ucEditHatch(uic, vs, style)

        End Function

#End Region ' Factory

#Region " Private vars "

        Private m_visualStyle As cVisualStyle = Nothing
        Private m_style As cVisualStyle.eVisualStyleTypes = cVisualStyle.eVisualStyleTypes.NotSet

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(ByVal vs As cVisualStyle, ByVal style As cVisualStyle.eVisualStyleTypes)
            ' Sanity check
            Debug.Assert(vs IsNot Nothing)

            Me.m_visualStyle = vs
            Me.m_style = style
        End Sub

#End Region ' Constructor

#Region " Event "

        Public Event OnVisualStyleChanged(ByVal sender As ucEditVisualStyle)

        Protected Sub FireStyleChangedEvent()
            RaiseEvent OnVisualStyleChanged(Me)
        End Sub

#End Region

#Region " Properties "

        Public Property VisualStyle() As cVisualStyle
            Get
                Return Me.m_visualStyle
            End Get
            Set(ByVal value As cVisualStyle)
                Me.m_visualStyle = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overridable Property RepresentationStyles() As cVisualStyle.eVisualStyleTypes
            Get
                Return Me.m_style
            End Get
            Set(ByVal value As cVisualStyle.eVisualStyleTypes)
                Me.m_style = value
            End Set
        End Property

#End Region ' Properties

#Region " Overridables "

        Public Overridable Function Apply(ByVal vs As cVisualStyle) As Boolean
            Return True
        End Function

#End Region ' Overridables

    End Class

End Namespace
