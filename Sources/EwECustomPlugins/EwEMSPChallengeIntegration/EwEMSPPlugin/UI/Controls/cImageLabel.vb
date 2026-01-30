' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing
Imports System.Windows.Forms



Namespace UI

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Label class that displays an image beside the label text.
    ''' </summary>
    ''' <remarks>
    ''' This class needs extending:
    ''' <list type="bullet">
    ''' <item>The current active reading order must be taken into account.</item>
    ''' </list>
    ''' </remarks>
    ''' <seealso cref="System.Windows.Forms.Label" />
    ''' ---------------------------------------------------------------------------
    Class cImageLabel
        Inherits Label

#Region " Private vars "

        Private m_image As Image = Nothing
        Private Const s_spacing As Integer = 4

#End Region ' Private vars

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Createas a new <see cref="cImageLabel"/>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New()
            Me.ImageAlign = ContentAlignment.MiddleLeft
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the image to display.
        ''' </summary>
        ''' <PermissionSet>
        '''   <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        '''   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        '''   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        '''   <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        ''' </PermissionSet>
        ''' -------------------------------------------------------------------
        Public Overloads Property Image() As Image
            Get
                Return m_image
            End Get
            Set(value As Image)
                ' Already has image?
                If (Me.m_image IsNot Nothing) Then
                    ' #Yes: Restore padding
                    Me.Padding = New Padding(Me.Padding.Left - s_spacing - Me.m_image.Width, Me.Padding.Top, Me.Padding.Right, Me.Padding.Bottom)
                End If
                ' Setting a new image?
                If (value IsNot Nothing) Then
                    ' #Yes: Calculate new padding
                    Me.Padding = New Padding(Me.Padding.Left + s_spacing + value.Width, Me.Padding.Top, Me.Padding.Right, Me.Padding.Bottom)
                End If
                Me.m_image = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Handles the <see cref="E:System.Windows.Forms.Control.Paint" /> event
        ''' to draw the image.
        ''' </summary>
        ''' <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> 
        ''' that contains the paint event data.</param>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            If (Me.Image IsNot Nothing) Then
                Dim r As Rectangle = Me.CalcImageRenderBounds(Me.Image, Me.ClientRectangle, Me.ImageAlign)
                e.Graphics.DrawImage(Me.Image, r)
            End If
            MyBase.OnPaint(e) ' Paint text
        End Sub

    End Class

End Namespace
