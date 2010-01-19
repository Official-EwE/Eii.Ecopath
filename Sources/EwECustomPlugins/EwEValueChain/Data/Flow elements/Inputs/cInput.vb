#Region " Imports "

Option Strict On
Imports EwECore

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' A value that entered a cUnit during processing.
''' </summary>
''' ===========================================================================
Public Class cInput

    Private m_sTons As Single = 0.0!
    Private m_sValue As Single = 1.0!
    Private m_sCustomValuePerTon As Single = 1.0!

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' <param name="sTons">Weight of the product, in tons</param>
    ''' <param name="sValue">Total value of the product.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal sTons As Single, ByVal sValue As Single, _
                   Optional ByVal sCustomValuePerTon As Single = 1.0!)
        Me.m_sTons = sTons
        Me.m_sValue = sValue
        Me.m_sCustomValuePerTon = sCustomValuePerTon
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the weight of input in tons of this input.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Tons() As Single
        Get
            Return Me.m_sTons
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the total value of this input. This value should correspond to
    ''' <see cref="Tons">Tons</see> x <see cref="CustomValuePerTon">ValuePerTon</see>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Value() As Single
        Get
            Return Me.m_sValue
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get custom value per ton for this input.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property CustomValuePerTon() As Single
        Get
            Return Me.m_sCustomValuePerTon
        End Get
    End Property

End Class
