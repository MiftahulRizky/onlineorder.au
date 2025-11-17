Imports System.Data
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO

Partial Class Setting_Logo
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim mailCfg As New MailConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        lblCustomerId.Text = Session("CustomerId")

        If Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" Or Session("RoleName") = "Account" Or Session("RoleName") = "Sunlight Product" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If
        If Session("LevelName") = "Support" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If
        If Not IsPostBack Then
            BindData(lblCustomerId.Text)
        End If
    End Sub

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)

        Try
            ' Pastikan file diunggah
            If Not fuLogo.HasFile Then
                MessageError(True, "PLEASE UPLOAD YOUR LOGO!")
                Exit Sub
            End If

            ' Cek ekstensi file yang diunggah (hanya file gambar yang diizinkan)
            Dim allowedExtensions As String() = {".jpg", ".jpeg", ".png", ".gif"}
            Dim ext As String = IO.Path.GetExtension(fuLogo.FileName.ToString()).ToLower()

            If Not allowedExtensions.Contains(ext) Then
                MessageError(True, "Invalid file type. Only JPG, PNG, GIF are allowed.")
                Exit Sub
            End If

            ' Tentukan ukuran gambar yang diinginkan
            Dim width As Integer = 440
            Dim height As Integer = 120

            ' Ambil stream gambar dari file yang diunggah
            Dim inp_Stream As Stream = fuLogo.PostedFile.InputStream

            Using image = Drawing.Image.FromStream(inp_Stream)
                ' Hitung rasio agar gambar tidak terdistorsi
                Dim ratio As Double = Math.Min(CDbl(width) / image.Width, CDbl(height) / image.Height)
                Dim newWidth As Integer = CInt(image.Width * ratio)
                Dim newHeight As Integer = CInt(image.Height * ratio)

                ' Buat objek bitmap untuk gambar baru dengan ukuran yang diinginkan
                Dim myImg As New Bitmap(newWidth, newHeight)
                Dim myImgGraph As Graphics = Graphics.FromImage(myImg)
                myImgGraph.CompositingQuality = CompositingQuality.HighQuality
                myImgGraph.SmoothingMode = SmoothingMode.HighQuality
                myImgGraph.InterpolationMode = InterpolationMode.HighQualityBicubic

                ' Jika file lama ada (termasuk path lama pada lblLogoOld.Text), hapus file lama
                If Not String.IsNullOrEmpty(lblLogoOld.Text) Then
                    Dim oldFilePath As String = Server.MapPath("~/Content/static/customers/") & lblLogoOld.Text
                    If File.Exists(oldFilePath) Then
                        File.Delete(oldFilePath)
                    End If
                End If

                ' Gambar gambar yang telah diubah ukuran ke dalam bitmap
                Dim imgRectangle = New Rectangle(0, 0, newWidth, newHeight)
                myImgGraph.DrawImage(image, imgRectangle)

                ' Tentukan nama file untuk logo dan simpan gambar
                lblLogo.Text = Now.ToString("ddMMyyyyHHmmss") & lblCustomerId.Text & ext
                Dim path = IO.Path.Combine(Server.MapPath("~/Content/static/customers"), lblLogo.Text)

                ' Simpan gambar dengan format asli
                myImg.Save(path, image.RawFormat)

                ' Lakukan pembaruan data
                sdsPage.Update()

                ' Redirect ke halaman logo setelah proses selesai
                Response.Redirect("~/setting/logo", False)
            End Using

        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                End If
                mailCfg.MailError(Page.Title, "BindData", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub


    Private Sub BindData(Id As String)
        MessageError(False, String.Empty)
        Try
            Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM CustomerQuotes WHERE Id = '" + Id + "'")
            lblLogoOld.Text = myData.Tables(0).Rows(0).Item("Logo").ToString()
            imgLogo.ImageUrl = "~/Content/static/customers/" & myData.Tables(0).Rows(0).Item("Logo").ToString()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                End If
                mailCfg.MailError(Page.Title, "BindData", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
