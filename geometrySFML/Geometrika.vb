Imports SFML
Imports SFML.Graphics
Imports SFML.System

Module Geometrika
    Private Events As New Dictionary(Of Char, Func(Of Integer))
    Private Desc As New Dictionary(Of Char, String)
    Private scr As MoonSharp.Interpreter.Script
    Public Function RunScript(Script As String) As List(Of IFigure)
        Dim flist As New List(Of IFigure)
        Dim addFigure As Func(Of String, Decimal(), Integer) = Function(FigureType As String, Values() As Decimal) As Integer
                                                                   Dim Figure As IFigure
                                                                   Select Case FigureType
                                                                       Case "Rectangle"
                                                                           Figure = New Rectangle(Values(0), Values(1), Values(2), Values(3))
                                                                       Case "Circle"
                                                                           Figure = New Circle(Values(0), Values(1), Values(2), Values(3))
                                                                       Case "Triangle"
                                                                           Figure = New Triangle(Values(0), Values(1), Values(2), Values(3), Values(4))
                                                                       Case Else
                                                                           Figure = Nothing
                                                                   End Select
                                                                   flist.Add(Figure)
                                                                   Return flist.Count - 1
                                                               End Function
        Dim figureArea As Func(Of Integer, Decimal) = Function(FigureIndex As Integer) As Decimal
                                                          Return flist(FigureIndex).Area
                                                      End Function
        Dim figurePerimeter As Func(Of Integer, Decimal) = Function(FigureIndex As Integer) As Decimal
                                                               Return flist(FigureIndex).Perimeter
                                                           End Function
        Dim figureData As Func(Of Integer, Dictionary(Of String, Decimal)) = Function(FigureIndex As Integer) As Dictionary(Of String, Decimal)
                                                                                 Return flist(FigureIndex).Data
                                                                             End Function
        Dim setRotation As Func(Of Integer, Decimal, Decimal) = Function(FigureIndex As Integer, Rotation As Decimal)
                                                                    flist(FigureIndex).Rotation = Rotation
                                                                    Return Rotation
                                                                End Function
        Dim setEvent As Func(Of String, MoonSharp.Interpreter.Closure, String, String) = Function(Key As String, LuaFunction As MoonSharp.Interpreter.Closure, Description As String) As String
                                                                                             Events(Key(0)) = Function()
                                                                                                                  scr.Call(LuaFunction)
                                                                                                                  Return 0
                                                                                                              End Function
                                                                                             Desc(Key(0)) = Description
                                                                                             Return Key
                                                                                         End Function
        scr = New MoonSharp.Interpreter.Script
        scr.Globals()("AddFigure") = addFigure
        scr.Globals()("FigureArea") = figureArea
        scr.Globals()("FigurePerimeter") = figurePerimeter
        scr.Globals()("FigureData") = figureData
        scr.Globals()("SetRotation") = setRotation
        scr.Globals()("SetEvent") = setEvent
        scr.DoString(Script)
        Return flist
    End Function

    Public Function DegToRad(D As Double) As Double
        Return D * Math.PI / 180
    End Function

    Public Interface IFigure
        Inherits SFML.Graphics.Drawable
        Property Position As Vector2f
        Property Rotation As Decimal
        ReadOnly Property Area As Decimal
        ReadOnly Property Perimeter As Decimal
        ReadOnly Property Data As Dictionary(Of String, Decimal)
    End Interface
    Public Function Transpose(ByVal Point As Vector2f, Rotation As Decimal, Around As Vector2f)
        Point.X -= Around.X
        Point.Y -= Around.Y
        Dim RetPoint As Vector2f = New Vector2f(Point.X, Point.Y)
        RetPoint.X = (Point.X * Math.Cos(DegToRad(Rotation))) - (Point.Y * Math.Sin(DegToRad(Rotation)))
        RetPoint.Y = (Point.X * Math.Sin(DegToRad(Rotation))) + (Point.Y * Math.Cos(DegToRad(Rotation)))
        RetPoint.X += Around.X
        RetPoint.Y += Around.Y
        Return RetPoint
    End Function

    Public Class Triangle
        Implements IFigure
        Public ALen As Decimal
        Public BLen As Decimal
        Public ABAngle As Decimal

        Public Sub New(positionX As Decimal, positionY As Decimal, aLen As Decimal, bLen As Decimal, aBAngle As Decimal)
            Me.ALen = aLen
            Me.BLen = bLen
            Me.ABAngle = aBAngle
            Me.Position = New Vector2f(positionX, positionY)
            Rotation = 0
        End Sub

        Public Property Position As Vector2f Implements IFigure.Position

        Public ReadOnly Property Points As Vector2f()
            Get
                Dim Point2 As New Vector2f(Position.X + ALen, Position.Y)
                Dim Point3 As New Vector2f(Point2.X + (Math.Cos(DegToRad(ABAngle)) * BLen), Point2.Y + Math.Sin(DegToRad(ABAngle)) * BLen)
                Return New Vector2f() {
                    Position, Point2, Point3
                    }
            End Get
        End Property

        Public ReadOnly Property Area As Decimal Implements IFigure.Area
            Get
                Return Math.Sin(DegToRad(ABAngle)) * BLen * ALen / 2
            End Get
        End Property

        Public ReadOnly Property Perimeter As Decimal Implements IFigure.Perimeter
            Get
                Return ALen + (Math.Cos(DegToRad(ABAngle)) * BLen) + (Math.Sin(DegToRad(ABAngle)) * BLen) + BLen
            End Get
        End Property

        Public ReadOnly Property Data As Dictionary(Of String, Decimal) Implements IFigure.Data
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Property Rotation As Decimal Implements IFigure.Rotation

        Public Sub Draw(target As RenderTarget, states As RenderStates) Implements Drawable.Draw
            Dim points As New VertexArray(PrimitiveType.LineStrip, 4)
            Dim p As Vector2f() = Me.Points
            For i As Integer = 0 To 2
                points(i) = New Vertex(Transpose(p(i), Rotation, Position), Color.Black)
            Next
            points(3) = New Vertex(Transpose(p(0), Rotation, Position), Color.Black)
            target.Draw(points, states)
        End Sub
    End Class

    Public Class Circle
        Implements IFigure

        Public Radio As Decimal
        Public Vertices As Integer

        Public Property Position As Vector2f Implements IFigure.Position

        Public ReadOnly Property Area As Decimal Implements IFigure.Area
            Get
                Return Radio * Radio * Math.PI
            End Get
        End Property

        Public ReadOnly Property Perimeter As Decimal Implements IFigure.Perimeter
            Get
                Return 2 * Radio * Math.PI
            End Get
        End Property

        Public ReadOnly Property Data As Dictionary(Of String, Decimal) Implements IFigure.Data
            Get
                Return New Dictionary(Of String, Decimal) From {{"X", Position.X}, {"Y", Position.Y},
                    {"Area", Area}, {"Perimeter", Perimeter}, {"Radio", Radio}, {"Vertices", Vertices}}
            End Get
        End Property

        Public Property Rotation As Decimal Implements IFigure.Rotation
            Get
                Return 0
            End Get
            Set(value As Decimal)
            End Set
        End Property

        Public Sub New(positionX As Decimal, positionY As Decimal, radio As Decimal, vertices As Integer)
            Me.Radio = radio
            Me.Vertices = vertices
            Me.Position = New Vector2f(positionX, positionY)
        End Sub

        Public Sub Draw(target As RenderTarget, states As RenderStates) Implements Drawable.Draw
            Dim vertices As New VertexArray(PrimitiveType.TriangleFan, Me.Vertices + 2)
            vertices(0) = New Vertex(Position)
            For i = 1 To Me.Vertices
                vertices(i) = New Vertex(Transpose(New Vector2f(
                                         Math.Cos(DegToRad(360 / Me.Vertices * (i - 1))) * Radio + Position.X,
                                         Math.Sin(DegToRad(360 / Me.Vertices * (i - 1))) * Radio + Position.Y
                                         ), Rotation, Position), Color.Black)
            Next
            vertices(Me.Vertices + 1) = New Vertex(Transpose(New Vector2f(
                                         Math.Cos(0) * Radio + Position.X,
                                         Position.Y
                                         ), Rotation, Position), Color.Black)
            target.Draw(vertices, states)
        End Sub
    End Class

    Public Class Rectangle
        Implements IFigure

        Public Width As Decimal
        Public Height As Decimal
        Public Property Position As Vector2f Implements IFigure.Position

        Public ReadOnly Property Area As Decimal Implements IFigure.Area
            Get
                Return Width * Height
            End Get
        End Property

        Public ReadOnly Property Perimeter As Decimal Implements IFigure.Perimeter
            Get
                Return 2 * Height + 2 * Width
            End Get
        End Property

        Public ReadOnly Property Data As Dictionary(Of String, Decimal) Implements IFigure.Data
            Get
                Return New Dictionary(Of String, Decimal) From {{"Width", Width}, {"Height", Height},
                    {"X", Position.X}, {"Y", Position.Y},
                    {"Area", Area}, {"Perimeter", Perimeter}}
            End Get
        End Property

        Public Property Rotation As Decimal Implements IFigure.Rotation

        Public Sub Draw(Target As RenderTarget, states As SFML.Graphics.RenderStates) Implements IFigure.Draw
            Dim vertex As New VertexArray(PrimitiveType.LineStrip, 5)
            vertex(0) = New Vertex(Me.Position, Color.Black)
            vertex(1) = New Vertex(Transpose(Me.Position + New Vector2f(Width, 0), Rotation, Position), Color.Black)
            vertex(2) = New Vertex(Transpose(Me.Position + New Vector2f(Width, Height), Rotation, Position), Color.Black)
            vertex(3) = New Vertex(Transpose(Me.Position + New Vector2f(0, Height), Rotation, Position), Color.Black)
            vertex(4) = New Vertex(Me.Position, Color.Black)
            Target.Draw(vertex, states)
        End Sub
        Public Sub New(ByRef x As Decimal, ByRef y As Decimal, ByRef w As Decimal, ByRef h As Decimal)
            Me.Position = New Vector2f(x, y)
            Me.Width = w
            Me.Height = h
        End Sub
    End Class

    Sub CloseHandler(sender As Object, e As EventArgs)
        DirectCast(sender, Window.Window).Close()
    End Sub
    Function Distance(p1 As Vector2f, p2 As Vector2f) As Single
        Dim x As Single = Math.Abs(p1.X - p2.X)
        Dim y As Single = Math.Abs(p1.Y - p2.Y)
        Return Math.Sqrt(x * x + y * y)
    End Function
    Sub Main()
        Dim Window As New SFML.Graphics.RenderWindow(New SFML.Window.VideoMode(640, 480), "Geometrika")
        Dim Script As String = LuaDialog.GetScript()
        Dim Figures As List(Of IFigure) = RunScript(Script)
        Dim zoom As Decimal = 1
        Dim xpos As Decimal = 0
        Dim ypos As Decimal = 0
        Dim initX As Integer
        Dim initY As Integer
        Dim p1 As New Vector2f
        Dim p2 As New Vector2f
        Dim moving As Boolean = False
        Dim text As New SFML.Graphics.Text("", New Graphics.Font("font.ttf"))
        Dim view As SFML.Graphics.View = Window.DefaultView
        text.FillColor = Color.Red
        text.CharacterSize = 12
        AddHandler Window.Closed, AddressOf CloseHandler
        AddHandler Window.KeyReleased, Sub(s As Object, e As Window.KeyEventArgs)
                                           Select Case e.Code
                                               Case SFML.Window.Keyboard.Key.Escape
                                                   Script = LuaDialog.GetScript(Script)
                                                   Figures = RunScript(Script)
                                               Case SFML.Window.Keyboard.Key.H
                                                   Dim msg As String = ("Left Click: Asignar vector 1 de medición" & vbNewLine & "Right Click: Asignar vector 2 de medición" & vbNewLine & "Middle Click: Mover gráfico" & vbNewLine & "Escape: Editar programa de construcción")
                                                   For Each i In Desc
                                                       msg += vbCrLf + i.Key + ": " + i.Value
                                                   Next
                                                   MsgBox(msg)
                                           End Select
                                           If Events.ContainsKey(e.Code.ToString()(0)) Then
                                               Events(e.Code.ToString()(0))()
                                           End If
                                       End Sub
        AddHandler Window.MouseButtonPressed, Sub(s As Object, e As Window.MouseButtonEventArgs)
                                                  If e.Button = SFML.Window.Mouse.Button.Left Then
                                                      p1 = New Vector2f(Math.Round((xpos + e.X / zoom) * 10) / 10, Math.Round((ypos + e.Y / zoom) * 10) / 10)
                                                  ElseIf e.Button = SFML.Window.Mouse.Button.Right Then
                                                      p2 = New Vector2f(Math.Round((xpos + e.X / zoom) * 10) / 10, Math.Round((ypos + e.Y / zoom) * 10) / 10)
                                                  ElseIf e.Button = SFML.Window.Mouse.Button.Middle Then
                                                      moving = True
                                                      initX = e.X
                                                      initY = e.Y
                                                  End If
                                              End Sub
        AddHandler Window.MouseButtonReleased, Sub(s As Object, e As Window.MouseButtonEventArgs)
                                                   If e.Button = SFML.Window.Mouse.Button.Middle Then
                                                       moving = False
                                                   End If
                                               End Sub
        AddHandler Window.MouseMoved, Sub(s As Object, e As Window.MouseMoveEventArgs)
                                          If moving Then
                                              xpos += Math.Round((initX - e.X) / zoom * 10) / 10
                                              ypos += Math.Round((initY - e.Y) / zoom * 10) / 10
                                              initX = e.X
                                              initY = e.Y
                                              view = (New View(New FloatRect(xpos, ypos, 640 / zoom, 480 / zoom)))
                                          End If
                                      End Sub
        AddHandler Window.MouseWheelScrolled, Sub(s As Object, e As Window.MouseWheelScrollEventArgs)
                                                  zoom += e.Delta / 2
                                                  If zoom <= 0.5 Then
                                                      zoom = 0.5
                                                  End If
                                                  view = (New View(New FloatRect(xpos, ypos, 640 / zoom, 480 / zoom)))
                                              End Sub
        While Window.IsOpen
            Window.Clear(SFML.Graphics.Color.White)
            Window.SetView(view)
            For Each i In Figures
                Window.Draw(i)
            Next
            text.DisplayedString = p1.ToString + " to " + p2.ToString + ": " + Distance(p1, p2).ToString + Environment.NewLine + "Zoom factor: " + zoom.ToString + " Position: (" + xpos.ToString + ", " + ypos.ToString + ")" + Environment.NewLine + "H: Help"
            Window.SetView(Window.DefaultView)
            Window.Draw(text)
            Window.Display()
            Window.WaitAndDispatchEvents()
        End While
        Window.Close()
    End Sub

End Module