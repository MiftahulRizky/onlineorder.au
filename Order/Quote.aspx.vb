Imports System.Data

Partial Class Order_Quote
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("headerId") = "" Then
            Response.Redirect("~/order/detail", False)
            Exit Sub
        End If

        lblHeaderId.Text = Session("headerId")
        If Not IsPostBack Then
            Call BackColor()

            Call BindData(lblHeaderId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If txtDiscount.Text <> "" Then
                If Not IsNumeric(txtDiscount.Text) Then
                    Call MessageError(True, "DISCOUNT SHOULD BE NUMERIC !")
                    txtDiscount.BackColor = Drawing.Color.Red
                    txtDiscount.Focus()
                    Exit Sub
                End If
            End If

            If txtInstall.Text <> "" Then
                If Not IsNumeric(txtInstall.Text) Then
                    Call MessageError(True, "INSTALLATION SHOULD BE NUMERIC !")
                    txtInstall.BackColor = Drawing.Color.Red
                    txtInstall.Focus()
                    Exit Sub
                End If
            End If

            If txtCM.Text <> "" Then
                If Not IsNumeric(txtCM.Text) Then
                    Call MessageError(True, "CHECK MEASURE BE NUMERIC !")
                    txtCM.BackColor = Drawing.Color.Red
                    txtCM.Focus()
                    Exit Sub
                End If
            End If

            If msgError.InnerText = "" Then
                If ddlGST.SelectedValue = "" Then : ddlGST.SelectedValue = "No" : End If
                If txtDiscount.Text = "" Then : txtDiscount.Text = "0.00" : End If
                If txtInstall.Text = "" Then : txtInstall.Text = "0.00" : End If
                If txtCM.Text = "" Then : txtCM.Text = "0.00" : End If

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "UPDATE QUOTE DETAIL. ID : " & lblHeaderId.Text)
                sdsPage.Update()

                Response.Redirect("~/order/detail", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at reza@bigblinds.co.id")
                publicCfg.MailError(Page.Title, "btnSubmit_Click", Session("UserId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/order/detail", False)
    End Sub

    Private Sub BindData(HeaderId As String)
        Call BackColor()
        Try
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM OrderHeaders WHERE Id = '" + HeaderId + "'")
            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/design", False)
                Exit Sub
            End If

            lblOrderNo.Text = publicCfg.GetOrderNo(lblHeaderId.Text)
            lblOrderCust.Text = publicCfg.GetOrderCust(lblHeaderId.Text)

            txtAddress.Text = myData.Tables(0).Rows(0).Item("Address").ToString()
            txtSuburb.Text = myData.Tables(0).Rows(0).Item("Suburb").ToString()
            txtPostCode.Text = myData.Tables(0).Rows(0).Item("PostCode").ToString()
            txtStates.Text = myData.Tables(0).Rows(0).Item("States").ToString()
            txtPhone.Text = myData.Tables(0).Rows(0).Item("Phone").ToString()
            txtEmail.Text = myData.Tables(0).Rows(0).Item("Email").ToString()
            ddlGST.SelectedValue = myData.Tables(0).Rows(0).Item("QuoteGST").ToString()
            txtDiscount.Text = myData.Tables(0).Rows(0).Item("QuoteDisc")
            txtInstall.Text = myData.Tables(0).Rows(0).Item("QuoteInstall")
            txtCM.Text = myData.Tables(0).Rows(0).Item("QuoteMeasure")
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at reza@bigblinds.co.id")
                publicCfg.MailError(Page.Title, "BindData", Session("UserId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        txtAddress.BackColor = Drawing.Color.Empty
        txtSuburb.BackColor = Drawing.Color.Empty
        txtStates.BackColor = Drawing.Color.Empty
        txtPostCode.BackColor = Drawing.Color.Empty
        txtPhone.BackColor = Drawing.Color.Empty
        txtEmail.BackColor = Drawing.Color.Empty
        ddlGST.BackColor = Drawing.Color.Empty
        txtDiscount.BackColor = Drawing.Color.Empty
        txtInstall.BackColor = Drawing.Color.Empty
        txtCM.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
