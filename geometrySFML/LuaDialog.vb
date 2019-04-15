Public Class LuaDialog
    Public Shared Function GetScript(Optional Initial As String = "") As String
        Dim f As New LuaDialog
        f.TextBox1.Text = Initial
        f.ShowDialog()
        Return f.TextBox1.Text
    End Function
    Private Documentation As New Dictionary(Of String, String) From {
    {"AddFigure", "<html><body><h1>AddFigure(<i>Figura</i>,<i>ArgumentosFigura</i>[]) DEVUELVE Identificador de la figura</h1><br/><ul><li><i>Figura</i>: Figura a ser creada. Valores posibles: {'Rectangle', 'Triangle', 'Circle'}</li><li><i>ArgumentosFigura</i>: Argumentos para la creación de la figura. <ul><li>Rectangle: [x, y, ancho, alto]</li><li>Triangle: [x, y, lado A, lado B, angulo AB]</li><li>Circle: [x, y, radio, vértices]</li></ul></li></ul><br/><b>Ejemplo</b>: AddFigure('Triangle', {20, 20, 30, 30, 90}) -> Triángulo en (20, 20) de lados AB(20, 30) y angulo AB(90º)</body></html>"},
    {"SetRotation", "<html><body><h1>SetRotation(<i>IDFigura</i>, </i>Angulo de rotación</i>) DEVUELVE Angulo de rotación</h1><br/>Rota a la figura alrededor de su posición</body></html>"},
    {"SetEvent", "<html><body><h1>SetEvent(<i>Tecla</i>, <i>Función</i>, <i>Descripción</i>)</h1><br/>Asigna a Función para que sea ejecutada cuando se presione la tecla Tecla. Descripción se muestra en el menú de ayuda.<br/><b>EJEMPLO</b>: SetEvent('K', Rotar, 'Rota el objeto 15 grados');</body></html>"},
    {"FigureData", "<html><body><h1>FigureData(<i>IDFigura</i>) DEVUELVE Tabla con información sobre la figura</h1></body></html>"},
    {"FigureArea", "<html><body><h1>FigureArea(<i>IDFigura</i>) DEVUELVE Área de la figura</h1></body></html>"},
    {"FigurePerimeter", "<html><body><h1>FigurePerimeter(<i>IDFigura</i>) DEVUELVE Perímetro de la figura</h1></body></html>"}}
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        WebBrowser1.DocumentText = Documentation(ListBox1.SelectedItem)
    End Sub
End Class