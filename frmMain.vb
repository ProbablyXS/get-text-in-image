Imports Tesseract
Imports System.Runtime.InteropServices
Imports System.Drawing.Imaging

Public Class frmMain

    Const DWMWA_EXTENDED_FRAME_BOUNDS = 9&      ' methode precise 
    <DllImport("dwmapi.dll")>      ' methode precise
    Private Shared Function DwmGetWindowAttribute(ByVal hwnd As IntPtr, ByVal dwAttribute As Integer, <Out> ByRef pvAttribute As RECT, ByVal cbAttribute As Integer) As Integer
    End Function

    Private Structure RECT

        Public left, top, right, bottom As Integer

    End Structure

    Declare Function FindWindow Lib "user32" Alias "FindWindowA" (ByVal lpClassName As String,
    ByVal lpWindowName As String) As IntPtr

    Dim hWnd As IntPtr
    Dim ox, oy, window_h, window_w As Integer

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click


        PictureBox1.Dispose()

    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If (IO.File.Exists("testdata/eng.traineddata")) = False Then
            IO.File.WriteAllBytes("testdata/eng.traineddata", My.Resources.eng)
        End If

    End Sub

    Private Sub cmdOpen_Click(sender As Object, e As EventArgs) Handles cmdOpen.Click

        hWnd = FindWindow("GLFW30", vbNullString)

        If hWnd <> 0 Then
            Dim wr As New RECT

            DwmGetWindowAttribute(hWnd, DWMWA_EXTENDED_FRAME_BOUNDS, wr, Marshal.SizeOf(wr))
            Label1.Text = "PokeMMO started!"
            ox = wr.left
            oy = wr.top
            window_h = wr.bottom - wr.top
            window_w = wr.right - wr.left

        End If

        Dim myBmp As New Bitmap(window_w, window_h)
        Dim g As Graphics = Graphics.FromImage(myBmp)
        g.CopyFromScreen(ox, oy, 0, 0, myBmp.Size)


        Dim CropRect As New Rectangle(0.002 * window_w, 0.04 * window_h, 110, 20) '298, 681
        Dim CropImage = New Bitmap(CropRect.Width, CropRect.Height)
        Dim grp As Graphics = Graphics.FromImage(CropImage)
        grp.DrawImage(myBmp, New Rectangle(0, 0, CropRect.Width, CropRect.Height), CropRect, GraphicsUnit.Pixel)
        myBmp.Dispose()

        CropImage.Save("test.png")

        Dim imgPix = PixConverter.ToPix(CropImage)

        Dim test As New TesseractEngine("testdata", "eng", EngineMode.Default)

        Dim page = test.Process(imgPix)

        Dim text = page.GetText()

        Dim iter = page.GetIterator()

        TextBox1.Clear()

        TextBox1.Text += text.Trim()


        grp.Dispose()
        CropImage.Dispose()
        g.Dispose()
        myBmp.Dispose()
        imgPix.Dispose()
        test.Dispose()

    End Sub

End Class