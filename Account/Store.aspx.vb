Imports System.Data

Partial Class Account_Store
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("UserId") = "" Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        txtId.Text = Session("StoreId") : txtId.ReadOnly = True
        If Not IsPostBack Then
            Call BindData(txtId.Text)
        End If
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/", False)
    End Sub

    Protected Sub btnEmail_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        txtEmail.BackColor = Drawing.Color.Empty
        Try
            If msgError.InnerText = "" Then
                sdsEmail.Update()
                Response.Redirect("~/account/store", False)
                Exit Sub
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnEmail_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            If txtName.Text = "" Then
                Call MessageError(True, "STORE NAME IS REQUIRED !")
                txtName.BackColor = Drawing.Color.Red
                txtName.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                sdsPage.Update()

                Response.Redirect("~/account/store", False)
                Exit Sub
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnSubmit_Click", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindData(Id As String)
        Call MessageError(False, String.Empty)
        Try
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM Stores WHERE Id = '" + Id + "'")

            txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
            txtPhone.Text = myData.Tables(0).Rows(0).Item("Phone").ToString()
            txtFax.Text = myData.Tables(0).Rows(0).Item("Fax").ToString()
            txtAbn.Text = myData.Tables(0).Rows(0).Item("Abn").ToString()
            txtEmail.Text = myData.Tables(0).Rows(0).Item("Email").ToString()
            lblMailOld.Text = myData.Tables(0).Rows(0).Item("Email").ToString()
            txtAddress.Text = myData.Tables(0).Rows(0).Item("Address").ToString()
            txtTerms.Text = myData.Tables(0).Rows(0).Item("Terms").ToString()

        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindData", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False
        msgError.InnerText = Msg
        If Show = True Then
            divError.Visible = True
        End If
    End Sub
End Class
