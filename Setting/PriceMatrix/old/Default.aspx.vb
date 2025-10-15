Partial Class Setting_PriceMatrix_Default
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call MessageError(False, String.Empty)

            Call BindPriceGroup()

            Call BindData(ddlPriceGroup.SelectedValue, ddlType.SelectedValue, txtWidth.Text, txtDrop.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/pricematrix/add", False)
    End Sub

    Protected Sub btnImport_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/pricematrix/import", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindData(ddlPriceGroup.SelectedValue, ddlType.SelectedValue, txtWidth.Text, txtDrop.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Call MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            Call BindData(ddlPriceGroup.SelectedValue, ddlType.SelectedValue, txtWidth.Text, txtDrop.Text)
        Catch ex As Exception
            Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
            publicCfg.MailError(Session("UserId"), Page.Title, "gvList_PageIndexChanging", ex.ToString())
        End Try
    End Sub

    Protected Sub linkDelete_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            lblId.Text = UCase(row.Cells(1).Text)

            sdsPage.Delete()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "DELETE PRICE MATRIX. ID : " & lblId.Text)

            Call BindData(ddlPriceGroup.SelectedValue, ddlType.SelectedValue, txtWidth.Text, txtDrop.Text)
        Catch ex As Exception
            Call MessageError(True, "Please contact our IT team at reza@bigblinds.co.id")
            publicCfg.MailError(Session("UserId"), Page.Title, "linkDelete_Click", ex.ToString())
        End Try
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Try
            lblId.Text = txtPriceIdEdit.Text
            sdsPage.Update()

            Dim userId As String = UCase(Session("UserId")).ToString()
            Dim description As String = "UPDATE Price Matrix. ID : " & lblId.Text
            publicCfg.InsertActivity(userId, Page.Title, "INSERT NEW PRICE. PRICE DESC : " & description)

            Call BindData(ddlPriceGroup.SelectedValue, ddlType.SelectedValue, txtWidth.Text, txtDrop.Text)
        Catch ex As Exception
            Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
            publicCfg.MailError(Session("UserId"), Page.Title, "btnSubmit_Click", ex.ToString())
        End Try
    End Sub

    Private Sub BindData(G As String, T As String, W As String, D As String)
        Try
            Dim byWidth As String = " WHERE Prices.Width >= '" + W + "'"
            Dim byDrop As String = " AND Prices.[Drop] >= '" + D + "'"
            Dim byGroup As String = ""
            Dim byType As String = ""

            If W = "" Then
                byWidth = " WHERE Prices.Width >= '0'"
            End If

            If D = "" Then
                byDrop = " AND Prices.[Drop] >= '0'"
            End If

            If Not G = "" Then
                byGroup = " AND Prices.PriceGroupId = '" + UCase(G).ToString() + "'"
            End If

            If Not T = "" Then
                byType = " AND Prices.Type = '" + T + "'"
            End If

            Dim thisQuery As String = String.Format("SELECT Prices.*, PricesGroup.Name AS GroupName FROM Prices INNER JOIN PricesGroup ON Prices.PriceGroupId = PricesGroup.Id {0} {1} {2} {3} ORDER BY PricesGroup.Name, Prices.Type, Prices.Width, Prices.[Drop], Prices.Cost ASC", byWidth, byDrop, byGroup, byType)

            gvList.DataSource = publicCfg.GetListData(thisQuery)
            gvList.DataBind()
        Catch ex As Exception
            Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
            publicCfg.MailError(Session("UserId"), Page.Title, "BindData", ex.ToString())
        End Try
    End Sub

    Private Sub BindPriceGroup()
        ddlPriceGroup.Items.Clear()
        Try
            ddlPriceGroup.DataSource = publicCfg.GetListData("SELECT * FROM PricesGroup WHERE Name NOT LIKE '%Headbox%' ORDER BY Name ASC")
            ddlPriceGroup.DataTextField = "Name"
            ddlPriceGroup.DataValueField = "Id"
            ddlPriceGroup.DataBind()

            If ddlPriceGroup.Items.Count > 1 Then
                ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
            publicCfg.MailError(Session("UserId"), Page.Title, "BindPriceGroup", ex.ToString())
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
