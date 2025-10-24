
Partial Class Setting_CustomDiscount_Default
    Inherits System.Web.UI.Page
    Dim publicCfg As New PublicConfig


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call MessageError(False, String.Empty)
            Call BindDesign()
            ddlDesignId.SelectedValue = Session("customDiscountDesign")
            Call BindBlind(ddlDesignId.SelectedValue)
            ddlBlindId.SelectedValue = Session("customDiscountBlind")
            txtSearch.Text = Session("customDiscountSearch")
            Call BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("customDiscountSearch") = txtSearch.Text
        Session("customDiscountDesign") = ddlDesignId.SelectedValue
        Session("customDiscountBlind") = ddlBlindId.SelectedValue

        Response.Redirect("~/setting/customdiscount/add", False)
    End Sub

    Protected Sub btnReset_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            sdsReset.Delete()

            Call BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub ddlDesignId_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BindBlind(ddlDesignId.SelectedValue)

        Call BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub ddlBlindId_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Call MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            Call BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkDetail_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Session("customDiscountSearch") = txtSearch.Text
            Session("customDiscountDesign") = ddlDesignId.SelectedValue
            Session("customDiscountBlind") = ddlBlindId.SelectedValue
            Session("customDiscountDetail") = UCase(row.Cells(1).Text)

            Response.Redirect("~/setting/customdiscount/detail", False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkCopy_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            lblId.Text = UCase(row.Cells(1).Text)
            sdsPage.Insert()

            Call BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text

            sdsPage.Delete()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "DELETE SURCHARGE. ID : " & lblId.Text)

            Call BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkActive_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Dim active As String = row.Cells(7).Text

            Dim newActive As Integer = 0
            If active = "False" Then : newActive = 1 : End If

            lblId.Text = UCase(row.Cells(1).Text)
            lblActive.Text = newActive

            sdsPage.Update()

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "ACTIVE SURCHARGE. ID : " & lblId.Text)

            Call BindData(ddlDesignId.SelectedValue, ddlBlindId.SelectedValue, txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(Design As String, Blind As String, Search As String)
        Try
            Session("customDiscountDesign") = "" : Session("customDiscountBlind") = "" : Session("customDiscountDetail") = "" : Session("customDiscountSearch") = ""

            Dim designString As String = ""
            Dim blindString As String = ""
            Dim searchString As String = ""

            If Design <> "" Then
                designString = " And CustomDiscount.DesignId = '" + UCase(Design).ToString() + "'"
            End If

            If Blind <> "" Then
                blindString = " AND CustomDiscount.BlindId = '" + UCase(Blind).ToString() + "'"
            End If

            If Search <> "" Then
                searchString = " AND (CustomDiscount.BlindNo LIKE '%" + Search + "%' OR CustomDiscount.Name LIKE '%" + Search + "%' OR CustomDiscount.FieldName LIKE '%" + Search + "%' OR CustomDiscount.Formula LIKE '%" + Search + "%' OR CustomDiscount.Charge LIKE '%" + Search + "%' OR CustomDiscount.Description LIKE '%" + Search + "%')"
            End If

            ' Dim thisQuery As String = String.Format("SELECT CustomDiscount.*, Designs.Name + ' | ' + Blinds.Name AS Product FROM CustomDiscount INNER JOIN Designs ON CustomDiscount.DesignId = Designs.Id INNER JOIN Blinds ON CustomDiscount.BlindId = Blinds.Id WHERE CustomDiscount.Active <> '' {0} {1} {2} ORDER BY Designs.Name, Blinds.Name, CustomDiscount.BlindNo, CustomDiscount.FieldName ASC", designString, blindString, searchString)
            Dim thisQuery As String = String.Format("SELECT CustomDiscount.*, Designs.Name + ' | ' + Blinds.Name AS Product FROM CustomDiscount INNER JOIN Designs ON CustomDiscount.DesignId = Designs.Id INNER JOIN Blinds ON CustomDiscount.BlindId = Blinds.Id WHERE CustomDiscount.Active >= '0' {0} {1} {2} ORDER BY Designs.Name, Blinds.Name, CustomDiscount.BlindNo, CustomDiscount.FieldName ASC", designString, blindString, searchString)


            gvList.DataSource = publicCfg.GetListData(thisQuery)
            gvList.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDesign()
        ddlDesignId.Items.Clear()
        Try
            ddlDesignId.Items.Clear()
            ddlDesignId.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Designs WHERE Active=1 ORDER BY Name ASC")
            ddlDesignId.DataTextField = "NameText"
            ddlDesignId.DataValueField = "Id"
            ddlDesignId.DataBind()

            ddlDesignId.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindBlind(DesignId As String)
        ddlBlindId.Items.Clear()
        Try
            If Not DesignId = "" Then
                ddlBlindId.Items.Clear()
                ddlBlindId.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Blinds WHERE DesignId='" + UCase(DesignId).ToString() + "' ORDER BY Name ASC")
                ddlBlindId.DataTextField = "NameText"
                ddlBlindId.DataValueField = "Id"
                ddlBlindId.DataBind()

                If ddlBlindId.Items.Count > 1 Then
                    ddlBlindId.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Function IconActiveOnNames(Active As String, Name As String) As String
     Dim result As String = "<i class='fa-regular text-danger fa-circle-xmark'></i> " + Name
        If Active = "True" Then : Return "<i class='fa-regular text-success fa-circle-check'></i> " + Name : End If
        Return result
    End Function

    Protected Function TextActive(Active As String) As String
        Dim result As String = "<i class='bi bi-arrow-repeat me-2 opacity-50 fs-3'></i> Switch Activated"
        ' Dim result As String = "<i class='fa-solid fa-arrows-rotate'></i>"
        ' If Active = "True" Then : Return "<i class='fa-solid fa-arrows-rotate'></i>" : End If
        Return result
    End Function

    Protected Function CssActive(Active As Boolean) As String
        Dim result As String = "dropdown-item"
        ' Dim result As String = "btn btn-green"
        ' If Active = False Then : result = "btn btn-secondary" : End If
        Return result
    End Function

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub


End Class
