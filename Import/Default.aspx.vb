Imports System.Data
Imports System.IO

Partial Class Import_Default
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" And Not Session("RoleName") = "PPIC & DE" Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call MessageError(False, String.Empty)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim strpath As String = Path.GetExtension(fuFile.FileName)

            If Not fuFile.HasFile Then
                Call MessageError(True, "PLEASE UPLOAD FILE (CSV) !")
                Exit Sub
            End If

            If strpath <> ".csv" Then
                Call MessageError(True, "PLEASE UPLOAD FILE (CSV) !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim directory As String = "~/Files/Import/"
                Dim fileName As String = Now.ToString("yyyy-MM-dd_hh-mm-ss") & "_by-" & Session("UserName") & ".txt"

                Dim vUpload As String = Server.MapPath(directory & fileName)
                fuFile.PostedFile.SaveAs(vUpload)

                Dim fileData As String = vUpload
                Dim sr As StreamReader
                sr = File.OpenText(fileData)
                Dim contents As String
                While sr.Peek() <> -1
                    contents = sr.ReadLine
                    Call RunSqlQuery(contents.Split(","))
                End While
                sr.Close()
            End If
        Catch ex As Exception
            Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
            publicCfg.MailError(Session("UserId"), Page.Title, "btnSubmit_Click", ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/", False)
    End Sub

    Private Sub RunSqlQuery(ByRef MyArray As String())
        Dim orderNo As String = MyArray(1)
        Dim orderCust As String = MyArray(3)
        Dim storeId As String = MyArray(9)

        ' UPDATE STATUS WEB JIKA DARI BOE STATUSNYA SEPERTI DI BAWAH INI
        If InStr(MyArray(5), "Ready To Print") > 0 Or InStr(MyArray(5), "In PPIC") > 0 Or InStr(MyArray(5), "In Preparation") > 0 Or InStr(MyArray(5), "In Packing") > 0 Or InStr(MyArray(5), "Completed") > 0 Then
            MyArray(5) = "Shipped Out"
        End If

        Dim checkData As DataSet = publicCfg.GetListData("SELECT * FROM OrderHeaders WHERE OrderNo = '" + orderNo + "' AND OrderCust = '" + orderCust + "' AND StoreId = '" + storeId + "'")

        If checkData.Tables(0).Rows.Count = 0 Then
            ' INSERT NEW LINE ORDER HEADER
            lblId.Text = publicCfg.CreateOrderHeaderId()
            ' UNTUK INSERT NANTI TAMBAH AKU USER ID BARU IMPORT BOE
            lblUserId.Text = UCase(Session("UserId")).ToString()
            lblStoreId.Text = MyArray(9).ToString()
            lblOrderNo.Text = MyArray(1).ToString()
            lblOrderCust.Text = MyArray(3).ToString()
            lblStatus.Text = MyArray(5).ToString()
            lblCreatedDate.Text = MyArray(4)
            lblSubmittedDate.Text = MyArray(4)
            lblShipmentDate.Text = MyArray(7)
            lblShipmentNo.Text = MyArray(6).ToString()
            lblCourier.Text = MyArray(12).ToString()
            lblContainerNo.Text = MyArray(13).ToString()

            sdsPage.Insert()
        End If

        If checkData.Tables(0).Rows.Count = 1 Then
            lblId.Text = checkData.Tables(0).Rows(0).Item("Id").ToString()
            lblStatus.Text = MyArray(5).ToString()
            lblShipmentDate.Text = MyArray(7)
            lblShipmentNo.Text = MyArray(6).ToString()
            lblCourier.Text = MyArray(12).ToString()
            lblContainerNo.Text = MyArray(13).ToString()

            sdsPage.Update()
        End If
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False
        ' msgError.InnerText = Msg
        If Show = True Then
            ' divError.Visible = True
            ' msgError.InnerText = Msg
            Dim escapedMsg As String = HttpUtility.JavaScriptStringEncode(Msg)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showMessageError('"& escapedMsg &"')", True)
        End If
    End Sub
End Class
