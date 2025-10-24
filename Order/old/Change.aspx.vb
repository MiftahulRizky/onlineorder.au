Imports System.Data
Partial Class Order_Change
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("headerId") = "" Then
            Response.Redirect("~/order/", False)
            Exit Sub
        End If

        lblHeaderId.Text = Session("headerId").ToString()
        If Not IsPostBack Then
            Call BackColor()

            Call BindData(lblHeaderId.Text)
        End If
    End Sub

    Protected Sub ddlStatus_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        Call BindComponentForm(ddlStatus.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If ddlStatus.SelectedValue = "" Then
                Call MessageError(True, "PLEASE CHOOSE NEW STATUS ORDER !")
                ddlStatus.CssClass = "form-select is-invalid"
                ddlStatus.Focus()
                Exit Sub
            End If
            If ddlStatus.SelectedValue = lblStatusOri.Text Then
                Call MessageError(True, "PLEASE UPDATE TO OTHER STATUS !")
                ddlStatus.CssClass = "form-select is-invalid"
                ddlStatus.Focus()
                Exit Sub
            End If

            If ddlStatus.SelectedValue = "New Order" Then
                If Not IsDate(txtSubmittedDate.Text) Then
                    Call MessageError(True, "SUBMITTED DATE IS REQUIRED !")
                    txtSubmittedDate.CssClass = "form-control is-invalid"
                    txtSubmittedDate.Focus()
                    Exit Sub
                End If
            End If

            If ddlStatus.SelectedValue = "Completed" Then
                If Not IsDate(txtCompletedDate.Text) Then
                    Call MessageError(True, "SHIPMENT DATE IS REQUIRED !")
                    txtCompletedDate.CssClass = "form-control is-invalid"
                    txtCompletedDate.Focus()
                    Exit Sub
                End If
            End If

            If ddlStatus.SelectedValue = "Canceled" Then
                If Not IsDate(txtCanceledDate.Text) Then
                    Call MessageError(True, "CANCELED DATE IS REQUIRED !")
                    txtCanceledDate.CssClass = "form-control is-invalid"
                    txtCanceledDate.Focus()
                    Exit Sub
                End If
            End If

            ' If data.description = "" AndAlso (data.status <> "Draft" AndAlso data.status <> "On Hold" AndAlso data.status <> "In Production") Then
            If txtDescription.Text = "" AndAlso (ddlStatus.SelectedValue <> "Draft" AndAlso ddlStatus.SelectedValue <> "On Hold" AndAlso ddlStatus.SelectedValue <> "In Production") Then
                Call MessageError(True, "DESCRIPTION IS REQUIRED !")
                txtDescription.CssClass = "form-control is-invalid"
                txtDescription.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim description As String = String.Empty
                lblDescription.Text = ""

                If ddlStatus.SelectedValue = "Draft" Then
                    sdsDraft.Update()
                End If

                If ddlStatus.SelectedValue = "New Order" Then
                    description = "Status changed to new order by <b>" & Session("UserName") & "</b>"
                    description += "<br />"
                    description += "Notes from the office:"
                    description += "<br />"
                    description += txtDescription.Text
                    lblDescription.Text = description
                    sdsNewOrder.Update()
                End If

                If ddlStatus.SelectedValue = "In Production" Then
                    description = "Your order is currently in the production process"
                    description += "<br />"
                    description += "Notes from the office:"
                    description += "<br />"
                    description += txtDescription.Text

                    lblDescription.Text = description
                    sdsProduction.Update()
                End If

                If ddlStatus.SelectedValue = "On Hold" Then
                    description = "Your order on hold by <b>" & Session("UserName") & "</b>"
                    description += "<br />"
                    description += "Notes from the office:"
                    description += "<br />"
                    description += txtDescription.Text
                    lblDescription.Text = description

                    sdsHold.Update()
                End If

                If ddlStatus.SelectedValue = "Completed" Then
                    description += "Notes from the office:"
                    description += "<br />"
                    description += txtDescription.Text
                    lblDescription.Text = description

                    sdsCompleted.Update()
                End If

                If ddlStatus.SelectedValue = "Canceled" Then
                    description = "Your order has been canceled by <b>" & Session("UserName") & "</b>"
                    description += "<br />"
                    description += "Notes from the office:"
                    description += "<br />"
                    description += txtDescription.Text
                    lblDescription.Text = description

                    sdsCanceled.Update()
                End If

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "UPDATE STATUS ORDER HEADER. ID : " & lblHeaderId.Text)

                If Session("changeFrom") = "Header" Then
                    Response.Redirect("~/order", False)
                    Exit Sub
                End If
                Response.Redirect("~/order/detail", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnSubmit_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        If Session("changeFrom") = "Header" Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If
        Response.Redirect("~/order/detail", False)
    End Sub

    Private Sub BindData(HeaderId As String)
        Try
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM OrderHeaders WHERE Id = '" + HeaderId + "'")

            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/order/detail", False)
                Exit Sub
            End If

            lblOrderNo.Text = publicCfg.GetOrderNo(lblHeaderId.Text)
            lblOrderCust.Text = publicCfg.GetOrderCust(lblHeaderId.Text)

            Dim status As String = myData.Tables(0).Rows(0).Item("Status").ToString()
            lblStatusOri.Text = myData.Tables(0).Rows(0).Item("Status").ToString()
            statusOld.InnerHtml = "* Status Now : " & status

            Call BindStatus(status)
            Call BindComponentForm(status)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindData", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindStatus(Status As String)
        ddlStatus.Items.Clear()
        Try
            If Session("RoleName") = "Administrator" Then
                ddlStatus.Items.Add(New ListItem("DRAFT", "Draft"))
            End If
            If Status = "Draft" Then
                ddlStatus.Items.Add(New ListItem("NEW ORDER", "New Order"))
                ddlStatus.Items.Add(New ListItem("CANCELED", "Canceled"))
            End If
            If Status = "New Order" Then
                ddlStatus.Items.Add(New ListItem("IN PRODUCTION", "In Production"))
                ddlStatus.Items.Add(New ListItem("ON HOLD", "On Hold"))
                ddlStatus.Items.Add(New ListItem("CANCELED", "Canceled"))
            End If
            If Status = "In Production" Or Session("RoleName") = "On Hold" Then
                ddlStatus.Items.Add(New ListItem("COMPLETED", "Completed"))
                ddlStatus.Items.Add(New ListItem("CANCELED", "Canceled"))
            End If

            If ddlStatus.Items.Count > 1 Then
                ddlStatus.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindDataStatus", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindComponentForm(Status As String)
        Try
            divSubmittedDate.Visible = False
            divCompletedDate.Visible = False
            divCanceledDate.Visible = False
            divDescription.Visible = False
            If Status = "New Order" Then
                divSubmittedDate.Visible = True
                divDescription.Visible = True
            End If
            If Status = "Completed" Then
                divCompletedDate.Visible = True
                divDescription.Visible = True
            End If
            If Status = "Canceled" Then
                divCanceledDate.Visible = True
                divDescription.Visible = True
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindComponentForm", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        ddlStatus.CssClass = "form-select"
        txtSubmittedDate.CssClass = "form-control"
        txtCompletedDate.CssClass = "form-control"
        txtCanceledDate.CssClass = "form-control"
        txtDescription.CssClass = "form-control"
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then
            Dim escapedMsg As String = HttpUtility.JavaScriptStringEncode(Msg)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showMessageError('"& escapedMsg &"')", True)
        End If
    End Sub
End Class
